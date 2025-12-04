using ArmaReforgerFeeder;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using MamboDMA.Services;


namespace MamboDMA.Games.Deadlock
{
    internal class DeadlockESP
    {
        private static class Off
        {
            public const ulong EntityList = 0x182A2E8;
            public const ulong LocalPlayer = 0x1829F28;
            public const ulong ViewMatrix = 0x182A7D8;

            // C_BaseEntity
            public const int m_vecOrigin = 0x138;
            public const int m_iHealth = 0x354;
            public const int m_iTeamNum = 0x3BF;
            public const int m_bDormant = 0x1A;
            public const int m_iszPlayerName = 0x5A8;
            public const int m_vecMins = 0x328;
            public const int m_vecMaxs = 0x334;
        }
        // Simple runtime-config for ESP (can be expanded)
        public sealed class Settings
        {
            public bool Enabled = true;
            public bool DrawBoxes = true;
            public bool DrawNames = true;
            public bool DrawDistance = true;
            public float MaxDistance = 400f;
            public float BoxThickness = 1.5f;
            public Vector4 ColorEnemy = new Vector4(1f, 0.2f, 0.2f, 1f);
            public Vector4 ColorTeammate = new Vector4(0.2f, 1f, 0.2f, 1f);
            public Vector4 ColorFriendly = new Vector4(0.2f, 0.6f, 1f, 1f);
        }

        public static Settings Cfg { get; } = new Settings();

        // Lightweight entity DTO
        private sealed class Entity
        {
            public ulong Ptr;
            public Vector3 Pos;
            public int Health;
            public int Team;
            public string Name = "";
            public bool Dormant;
        }

        // Public UI to be called from DeadlockGame.Draw
        public static void DrawConfigPanel()
        {
            if (ImGui.CollapsingHeader("Deadlock ESP", ImGuiTreeNodeFlags.DefaultOpen))
            {
                ImGui.Checkbox("ESP Enabled", ref Cfg.Enabled);
                ImGui.Separator();

                ImGui.Checkbox("Draw Boxes", ref Cfg.DrawBoxes);
                ImGui.Checkbox("Draw Names", ref Cfg.DrawNames);
                ImGui.Checkbox("Draw Distance", ref Cfg.DrawDistance);
                ImGui.SliderFloat("Max Distance (m)", ref Cfg.MaxDistance, 50f, 2000f);
                ImGui.SliderFloat("Box Thickness", ref Cfg.BoxThickness, 0.5f, 6f);

                ImGui.Spacing();
                ImGui.Text("Colors:");
                ImGui.ColorEdit4("Enemy Color", ref Cfg.ColorEnemy);
                ImGui.ColorEdit4("Teammate Color", ref Cfg.ColorTeammate);
                ImGui.ColorEdit4("Friendly Color", ref Cfg.ColorFriendly);
            }
        }

        // Main render call for overlay; call every frame when running.
        public static void RenderOverlay()
        {
            if (!Cfg.Enabled) return;
            if (!DmaMemory.IsAttached) return;

            // Read view matrix
            Matrix4x4 vm;
            try
            {
                vm = DmaMemory.Read<Matrix4x4>(DmaMemory.Base + Off.ViewMatrix);
            }
            catch
            {
                return;
            }

            // Read local player (some games store a pointer/address here)
            if (!DmaMemory.Read<ulong>(DmaMemory.Base + Off.LocalPlayer, out var localPtr) || localPtr == 0)
                return;

            // Try read local player's origin
            Vector3 localPos = Vector3.Zero;
            {
                if (DmaMemory.Read<Vector3>(localPtr + (ulong)Off.m_vecOrigin, out var lp))
                    localPos = lp;
            }

            // Enumerate entities from EntityList
            ulong listPtr;
            try
            {
                listPtr = DmaMemory.Read<ulong>(DmaMemory.Base + Off.EntityList);
            }
            catch
            {
                return;
            }

            if (listPtr == 0) return;

            // Try read a chunk of pointers (safe upper bound)
            const int MaxEntities = 1024;
            ulong[]? ptrs = null;
            try
            {
                ptrs = DmaMemory.ReadArray<ulong>(listPtr, MaxEntities);
            }
            catch
            {
                // fallback: try reading sequentially
            }
            if (ptrs == null || ptrs.Length == 0)
            {
                // no list array; bail gracefully
                return;
            }

            var dl = ImGui.GetForegroundDrawList();
            var io = ImGui.GetIO();
            float scrW = io.DisplaySize.X, scrH = io.DisplaySize.Y;

            for (int i = 0; i < ptrs.Length; i++)
            {
                var entPtr = ptrs[i];
                if (entPtr == 0) continue;

                // Read dormant flag (byte)
                bool dormant = false;
                try { dormant = DmaMemory.Read<byte>(entPtr + (ulong)Off.m_bDormant) != 0; } catch { }

                // Skip dormant entities
                if (dormant) continue;

                // Read origin
                if (!DmaMemory.Read<Vector3>(entPtr + (ulong)Off.m_vecOrigin, out var pos))
                    continue;

                float distM = Vector3.Distance(localPos, pos) / 100f;
                if (distM > Cfg.MaxDistance) continue;

                // Try health and team
                int hp = 0; int team = 0;
                try { hp = DmaMemory.Read<int>(entPtr + (ulong)Off.m_iHealth); } catch { }
                try { team = DmaMemory.Read<byte>(entPtr + (ulong)Off.m_iTeamNum); }
                catch
                {
                    try { team = DmaMemory.Read<int>(entPtr + (ulong)Off.m_iTeamNum); } catch { }
                }

                // Try name (utf16)
                string name = "";
                try
                {
                    var namePtr = DmaMemory.Read<ulong>(entPtr + (ulong)Off.m_iszPlayerName);
                    if (namePtr != 0) name = DmaMemory.ReadUtf16Z(namePtr, 64);
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        // fallback try ascii read at same offset
                        name = DmaMemory.ReadAsciiZ(entPtr + (ulong)Off.m_iszPlayerName, 64);
                    }
                }
                catch { /* ignore */ }

                // World-to-screen
                if (!WorldToScreen(vm, pos, scrW, scrH, out Vector2 screen)) continue;

                // Choose color by team relative to local (best-effort)
                Vector4 color = Cfg.ColorEnemy;
                if (team != 0 && team == 1) color = Cfg.ColorTeammate; // heuristic
                uint colU32 = ImGui.ColorConvertFloat4ToU32(color);

                // Draw simple box marker
                if (Cfg.DrawBoxes)
                {
                    float size = Math.Clamp(1000f / MathF.Max(distM, 1f), 8f, 120f);
                    var min = new Vector2(screen.X - size * 0.5f, screen.Y - size * 0.5f);
                    var max = new Vector2(screen.X + size * 0.5f, screen.Y + size * 0.5f);
                    dl.AddRect(min, max, colU32, 4f, ImDrawFlags.RoundCornersAll, Cfg.BoxThickness);
                }

                // Draw name
                if (Cfg.DrawNames)
                {
                    string label = string.IsNullOrWhiteSpace(name) ? $"Entity_{i}" : name;
                    dl.AddText(new Vector2(screen.X + 6f, screen.Y - 10f), colU32, label);
                }

                // Draw distance
                if (Cfg.DrawDistance)
                {
                    dl.AddText(new Vector2(screen.X + 6f, screen.Y + 6f), 0xFFFFFFFF, $"{distM:F0} m");
                }

                // Draw health bar (small)
                if (hp > 0)
                {
                    float pct = Math.Clamp(hp / 100f, 0f, 1f);
                    float barW = 40f;
                    var p0 = new Vector2(screen.X - barW / 2f, screen.Y + 14f);
                    var p1 = new Vector2(p0.X + barW * pct, p0.Y + 6f);
                    dl.AddRectFilled(p0, new Vector2(p0.X + barW, p0.Y + 6f), ImGui.ColorConvertFloat4ToU32(new Vector4(0.1f, 0f, 0f, 0.7f)), 2f);
                    dl.AddRectFilled(p0, p1, ImGui.ColorConvertFloat4ToU32(new Vector4(0.1f, 1f, 0.1f, 0.95f)), 2f);
                    dl.AddRect(p0, new Vector2(p0.X + barW, p0.Y + 6f), 0xFF000000);
                }
            }
        }

        // Simple matrix-based W2S (assumes view-proj quad stored as 4x4 matrix at Off.ViewMatrix)
        private static bool WorldToScreen(in Matrix4x4 mat, in Vector3 w, float scrW, float scrH, out Vector2 outScreen)
        {
            outScreen = Vector2.Zero;

            // Multiply vector by matrix (row-major assumption):
            // clip = M * [x y z 1]
            float clipX = w.X * mat.M11 + w.Y * mat.M21 + w.Z * mat.M31 + mat.M41;
            float clipY = w.X * mat.M12 + w.Y * mat.M22 + w.Z * mat.M32 + mat.M42;
            float clipZ = w.X * mat.M13 + w.Y * mat.M23 + w.Z * mat.M33 + mat.M43;
            float clipW = w.X * mat.M14 + w.Y * mat.M24 + w.Z * mat.M34 + mat.M44;

            if (clipW <= 0.001f) return false;

            float ndcX = clipX / clipW;
            float ndcY = clipY / clipW;

            float sx = (ndcX + 1.0f) * 0.5f * scrW;
            float sy = (1.0f - ndcY) * 0.5f * scrH;

            if (float.IsNaN(sx) || float.IsNaN(sy) || float.IsInfinity(sx) || float.IsInfinity(sy))
                return false;

            outScreen = new Vector2(sx, sy);
            return true;
        }
    }
}
