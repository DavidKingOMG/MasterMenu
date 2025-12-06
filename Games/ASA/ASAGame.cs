using ImGuiNET;
using MamboDMA.Input;
using MamboDMA.Services;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;

namespace MamboDMA.Games.ASA
{
    internal class ASAGame : IGame
    {
        public string Name => "Ark Survival Ascended";

        private bool _initialized;
        private bool _running;

        private static ASAConfig Cfg => Config<ASAConfig>.Settings;

        private static readonly string[] ASAActions =
        {
            "ASA_ToggleBoxes",
            "ASA_ToggleNames",
            "ASA_ToggleDistance",
            "ASA_ToggleDino",
            "ASA_ToggleDebug",
            "ASA_Attach",
            "ASA_DisposeVmm",
        };

        private static ASAEntity[] _entities = Array.Empty<ASAEntity>();
        private static ASADino[] _dinos = Array.Empty<ASADino>();
        private static ASAItem[] _items = Array.Empty<ASAItem>();
        private static ASAStructure[] _structures = Array.Empty<ASAStructure>();

        public void Initialize()
        {
            if (_initialized) return;

            if (ScreenService.Current.W <= 0 || ScreenService.Current.H <= 0)
                ScreenService.UpdateFromMonitor(GameSelector.SelectedMonitor);

            Keybinds.RegisterCategory("ASA", ASAActions, HandleASAAction);
            EnsureASAKeybindProfile();

            _initialized = true;
        }

        public void Attach() => VmmService.Attach(Cfg.ASAExe ?? "ShooterGame.exe");

        public void Dispose()
        {
            Stop();
            DmaMemory.Dispose();
        }

        public void Start()
        {
            if (_running) return;
            _running = true;
        }

        public void Stop()
        {
            if (!_running) return;
            _running = false;
        }

        public void Tick()
        {
            if (!_running) return;

            // TODO: Add DMA entity gathering for:
            // players -> _entities
            // dinos -> _dinos
            // items/containers -> _items
            // structures -> _structures
        }

        public void Draw(ImGuiWindowFlags winFlags)
        {
            if (UiVisibility.MenusHidden) return;

            Config<ASAConfig>.DrawConfigPanel(Name, cfg =>
            {
                bool ready = _running;
                var color = ready
                    ? new Vector4(0, 0.85f, 0, 1)
                    : new Vector4(1f, 0.25f, 0.20f, 1);

                DrawStatusInline(color, ready ? "Running" : "Click Start Workers");
                ImGui.Separator();

                // ============================================================
                // PLAYER ESP
                // ============================================================
                if (ImGui.CollapsingHeader("Player ESP", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    ImGui.Checkbox("Draw Player Boxes", ref cfg.DrawPlayerBoxes);
                    ImGui.Checkbox("Draw Names", ref cfg.DrawNames);
                    ImGui.Checkbox("Draw Distance", ref cfg.DrawDistance);
                    ImGui.Checkbox("Show Debug Info", ref cfg.ShowDebug);

                    ImGui.SliderFloat("Max Draw Distance", ref cfg.MaxDrawDistance, 50f, 10000f);
                    ImGui.SliderFloat("Box Thickness", ref cfg.BoxThickness, 0.5f, 5.0f);

                    ImGui.Spacing();
                    ImGui.Text("Player Colors:");
                    ImGui.ColorEdit4("Friendly", ref cfg.ColorFriendly);
                    ImGui.ColorEdit4("Enemy", ref cfg.ColorEnemy);
                }

                ImGui.Separator();

                // ============================================================
                // DINO ESP
                // ============================================================
                if (ImGui.CollapsingHeader("Dino ESP"))
                {
                    ImGui.Checkbox("Draw Dinos", ref cfg.DrawDinos);
                    ImGui.Checkbox("Draw Dino Names", ref cfg.DrawDinoNames);
                    ImGui.Checkbox("Draw Dino Distance", ref cfg.DrawDinoDistance);
                    ImGui.Checkbox("Draw Dino Level", ref cfg.DrawDinoLevels);
                    ImGui.Checkbox("Highlight Tames", ref cfg.HighlightTamed);
                    ImGui.Checkbox("Highlight Babies", ref cfg.HighlightBabies);

                    ImGui.SliderFloat("Dino Max Distance", ref cfg.DinoMaxDistance, 50f, 20000f);

                    ImGui.Text("Dino Colors:");
                    ImGui.ColorEdit4("Tamed Dino", ref cfg.ColorTamed);
                    ImGui.ColorEdit4("Wild Dino", ref cfg.ColorWild);
                    ImGui.ColorEdit4("Aggressive Dino", ref cfg.ColorAggressive);
                    ImGui.ColorEdit4("Baby Dino", ref cfg.ColorBaby);
                }

                ImGui.Separator();

                // ============================================================
                // ITEM / LOOT ESP
                // ============================================================
                if (ImGui.CollapsingHeader("Item ESP"))
                {
                    ImGui.Checkbox("Draw Items", ref cfg.DrawItems);
                    ImGui.Checkbox("Draw Item Names", ref cfg.DrawItemNames);
                    ImGui.Checkbox("Draw Item Distance", ref cfg.DrawItemDistance);

                    ImGui.SliderFloat("Item Max Distance", ref cfg.ItemMaxDistance, 10f, 5000f);

                    ImGui.ColorEdit4("Regular Item", ref cfg.ColorItem);
                    ImGui.ColorEdit4("Important Item", ref cfg.ColorItemImportant);

                    ImGui.InputText("Include Filter", ref cfg.ItemFilterInclude, 200);
                    ImGui.InputText("Exclude Filter", ref cfg.ItemFilterExclude, 200);
                }

                ImGui.Separator();

                // ============================================================
                // STRUCTURES
                // ============================================================
                if (ImGui.CollapsingHeader("Structure ESP"))
                {
                    ImGui.Checkbox("Draw Structures", ref cfg.DrawStructures);
                    ImGui.Checkbox("Draw Structure Names", ref cfg.DrawStructureNames);
                    ImGui.Checkbox("Draw Structure Distance", ref cfg.DrawStructureDistance);

                    ImGui.SliderFloat("Structure Max Distance", ref cfg.StructureMaxDistance, 10f, 8000f);

                    ImGui.ColorEdit4("Structure Color", ref cfg.ColorStructure);
                }

                ImGui.Separator();

                // ============================================================
                // WORKER CONTROL
                // ============================================================
                if (ImGui.Button(_running ? "Restart Workers" : "Start Workers"))
                {
                    if (_running) { Stop(); Start(); }
                    else Start();
                }
                ImGui.SameLine();
                if (ImGui.Button("Stop Workers"))
                    Stop();
            });

            DrawESPOverlay();
        }

        private static void DrawStatusInline(Vector4 color, string caption)
        {
            var dl = ImGui.GetWindowDrawList();
            var p = ImGui.GetCursorScreenPos();
            float y = p.Y + ImGui.GetTextLineHeight() * 0.5f;
            dl.AddCircleFilled(new Vector2(p.X + 5, y), 5, ImGui.ColorConvertFloat4ToU32(color));
            ImGui.Dummy(new Vector2(14, ImGui.GetTextLineHeight()));
            ImGui.SameLine();
            ImGui.TextDisabled(caption);
        }

        private static void DrawESPOverlay()
        {
            var dl = ImGui.GetForegroundDrawList();
            var cfg = Cfg;

            // TODO: Draw Players, Dinos, Items, Structures
            // Using the same patterns as Arc Raiders overlay
        }

        private void HandleASAAction(string action)
        {
            switch (action)
            {
                case "ASA_ToggleBoxes": Cfg.DrawPlayerBoxes = !Cfg.DrawPlayerBoxes; break;
                case "ASA_ToggleNames": Cfg.DrawNames = !Cfg.DrawNames; break;
                case "ASA_ToggleDistance": Cfg.DrawDistance = !Cfg.DrawDistance; break;
                case "ASA_ToggleDino": Cfg.DrawDinos = !Cfg.DrawDinos; break;
                case "ASA_ToggleDebug": Cfg.ShowDebug = !Cfg.ShowDebug; break;

                case "ASA_Attach": Attach(); break;
                case "ASA_DisposeVmm": Dispose(); break;
            }
        }

        private static void EnsureASAKeybindProfile()
        {
            var root = Path.Combine(AppContext.BaseDirectory, "Configs", "ASA");
            var path = Path.Combine(root, "asa.keybinds.json");
            if (File.Exists(path)) return;

            Directory.CreateDirectory(root);

            var prof = new KeybindProfile
            {
                ProfileName = "ASA Keybinds",
                Category = "ASA",
                Binds = new List<KeybindEntry>
                {
                    new KeybindEntry { Name="ASA: Toggle Boxes",       Vk=0xC0, Mode=KeybindMode.OnPress, Action="ASA_ToggleBoxes" },
                    new KeybindEntry { Name="ASA: Toggle Names",       Vk=0x4E, Mode=KeybindMode.OnPress, Action="ASA_ToggleNames" },
                    new KeybindEntry { Name="ASA: Toggle Dist",        Vk=0x44, Mode=KeybindMode.OnPress, Action="ASA_ToggleDistance" },
                    new KeybindEntry { Name="ASA: Toggle Dinos",       Vk=0x53, Mode=KeybindMode.OnPress, Action="ASA_ToggleDino" },
                    new KeybindEntry { Name="ASA: Toggle Debug",       Vk=0x47, Mode=KeybindMode.OnPress, Action="ASA_ToggleDebug" },
                    new KeybindEntry { Name="ASA: Attach",             Vk=0x41, Mode=KeybindMode.OnPress, Action="ASA_Attach" },
                    new KeybindEntry { Name="ASA: Dispose VMM",        Vk=0x58, Mode=KeybindMode.OnPress, Action="ASA_DisposeVmm" },
                }
            };

            KeybindRegistry.Save(path, prof);
        }

        private sealed class ASAEntity
        {
            public string Name;
            public int Distance;
            public Vector3 Pos;
        }

        private sealed class ASADino
        {
            public string Name;
            public int Level;
            public bool IsTamed;
            public bool IsBaby;
            public Vector3 Pos;
            public float Distance;
        }

        private sealed class ASAItem
        {
            public string Name;
            public Vector3 Pos;
            public float Distance;
        }

        private sealed class ASAStructure
        {
            public string Name;
            public Vector3 Pos;
            public float Distance;
        }
    }
}
