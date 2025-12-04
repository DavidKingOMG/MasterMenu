using ImGuiNET;
using MamboDMA.Input;
using MamboDMA.Services;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace MamboDMA.Games.ArcRaiders
{
    // Simple entity representation for ESP

    public sealed class ArcRaidersGame : IGame
    {
        public string Name => "ArcRaiders";

        private bool _initialized;
        private bool _running;

        private static ArcRaidersConfig Cfg => Config<ArcRaidersConfig>.Settings;

        private static readonly string[] ArcActions =
        {
            "ARC_ToggleBoxes",
            "ARC_ToggleNames",
            "ARC_ToggleDistance",
            "ARC_ToggleSkeletons",
            "ARC_ToggleDebug",
            "ARC_ToggleWebRadar",
            "ARC_Attach",
            "ARC_DisposeVmm",
        };
        private static ArcEntity[] _entities = Array.Empty<ArcEntity>();
        public void Initialize()
        {
            if (_initialized) return;

            if (ScreenService.Current.W <= 0 || ScreenService.Current.H <= 0)
                ScreenService.UpdateFromMonitor(GameSelector.SelectedMonitor);

            Keybinds.RegisterCategory("ArcRaiders", ArcActions, HandleArcAction);
            EnsureArcKeybindProfile();

            _initialized = true;
        }
        public void Attach() => VmmService.Attach(Cfg.ArcExe ?? "PioneerGame.exe");
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
        }

        public void Draw(ImGuiWindowFlags winFlags)
        {
            if (UiVisibility.MenusHidden) return;

            Config<ArcRaidersConfig>.DrawConfigPanel(Name, cfg =>
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
                    ImGui.Checkbox("Draw Boxes", ref cfg.DrawBoxes);
                    ImGui.Checkbox("Draw Names", ref cfg.DrawNames);
                    ImGui.Checkbox("Draw Distance", ref cfg.DrawDistance);
                    ImGui.Checkbox("Draw Skeletons", ref cfg.DrawSkeletons);
                    ImGui.Checkbox("Show Debug Info", ref cfg.ShowDebug);

                    ImGui.SliderFloat("Max Draw Distance", ref cfg.MaxDrawDistance, 50f, 5000f);
                    ImGui.SliderFloat("Max Skeleton Distance", ref cfg.MaxSkeletonDistance, 50f, 2000f);
                    ImGui.SliderFloat("Box Thickness", ref cfg.BoxThickness, 0.5f, 5.0f);

                    ImGui.Spacing();
                    ImGui.Text("Player Colors:");
                    ImGui.ColorEdit4("Player Color", ref cfg.ColorPlayer);
                    ImGui.ColorEdit4("Enemy Color", ref cfg.ColorEnemy);
                    ImGui.ColorEdit4("Bot Color", ref cfg.ColorBot);

                    ImGui.Spacing();
                    ImGui.Text("Visibility Colors:");
                    ImGui.ColorEdit4("Visible Box", ref cfg.ColorBoxVisible);
                    ImGui.ColorEdit4("Invisible Box", ref cfg.ColorBoxInvisible);
                    ImGui.ColorEdit4("Visible Skeleton", ref cfg.ColorSkelVisible);
                    ImGui.ColorEdit4("Invisible Skeleton", ref cfg.ColorSkelInvisible);
                }

                ImGui.Separator();

                // ============================================================
                // GROUND LOOT ESP
                // ============================================================

                if (ImGui.CollapsingHeader("Ground Loot ESP"))
                {
                    ImGui.Checkbox("Draw Ground Loot", ref cfg.DrawGroundLoot);
                    ImGui.Checkbox("Loot: Draw Names", ref cfg.DrawGroundLootNames);
                    ImGui.Checkbox("Loot: Draw Distance", ref cfg.DrawGroundLootDistance);

                    ImGui.SliderFloat("Loot Max Distance", ref cfg.GroundLootMaxDistance, 20f, 1000f);
                    ImGui.SliderFloat("Loot Marker Size", ref cfg.GroundLootMarkerSize, 2f, 20f);

                    ImGui.ColorEdit4("Regular Loot Color", ref cfg.LootRegularColor);
                    ImGui.ColorEdit4("Important Loot Color", ref cfg.LootImportantColor);

                    ImGui.Checkbox("Show Loot Price", ref cfg.LootShowPrice);

                    ImGui.InputText("Filter (Include)", ref cfg.LootFilterInclude, 200);
                    ImGui.InputText("Filter (Exclude)", ref cfg.LootFilterExclude, 200);

                    int regPrice = cfg.LootMinPriceRegular;
                    if (ImGui.InputInt("Min Price: Regular", ref regPrice))
                        cfg.LootMinPriceRegular = Math.Max(0, regPrice);

                    int impPrice = cfg.LootMinPriceImportant;
                    if (ImGui.InputInt("Min Price: Important", ref impPrice))
                        cfg.LootMinPriceImportant = Math.Max(0, impPrice);
                }

                ImGui.Separator();

                // ============================================================
                // CONTAINER ESP
                // ============================================================

                if (ImGui.CollapsingHeader("Container ESP"))
                {
                    ImGui.Checkbox("Draw Containers", ref cfg.DrawContainers);
                    ImGui.Checkbox("Draw Container Names", ref cfg.DrawContainerNames);
                    ImGui.Checkbox("Draw Container Distance", ref cfg.DrawContainerDistance);
                    ImGui.Checkbox("Show Empty Containers", ref cfg.DrawEmptyContainers);

                    ImGui.SliderFloat("Container Max Distance", ref cfg.ContainerMaxDistance, 20f, 1000f);
                    ImGui.SliderFloat("Container Marker Size", ref cfg.ContainerMarkerSize, 2f, 20f);

                    ImGui.Text("Container Colors:");
                    ImGui.ColorEdit4("Filled Container", ref cfg.ContainerFilledColor);
                    ImGui.ColorEdit4("Empty Container", ref cfg.ContainerEmptyColor);
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

            // Your ESP rendering overlay
            DrawEntitiesOverlay();
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

        private static void DrawEntitiesOverlay()
        {
            var ents = Volatile.Read(ref _entities);
            if (ents == null || ents.Length == 0) return;

            var dl = ImGui.GetForegroundDrawList();
            var cfg = Cfg;

            foreach (var e in ents)
            {
                if (e.Distance > cfg.MaxDrawDistance) continue;

                if (cfg.DrawBoxes)
                {
                    var min = new Vector2(e.Pos.X - 20, e.Pos.Y - 20);
                    var max = new Vector2(e.Pos.X + 20, e.Pos.Y + 20);
                    dl.AddRect(min, max, ImGui.GetColorU32(cfg.ColorBoxVisible));
                    dl.AddRect(min, max, ImGui.GetColorU32(cfg.ColorBoxInvisible));
                }

                if (cfg.DrawNames)
                    dl.AddText(new Vector2(e.Pos.X, e.Pos.Y - 30),
                               ImGui.GetColorU32(cfg.ColorName), e.Name);

                if (cfg.DrawDistance)
                    dl.AddText(new Vector2(e.Pos.X, e.Pos.Y + 30),
                               ImGui.GetColorU32(cfg.DistanceColor), $"{e.Distance}m");
            }
        }

        private void HandleArcAction(string action)
        {
            switch (action)
            {
                case "ARC_ToggleBoxes": Cfg.DrawBoxes = !Cfg.DrawBoxes; break;
                case "ARC_ToggleNames": Cfg.DrawNames = !Cfg.DrawNames; break;
                case "ARC_ToggleDistance": Cfg.DrawDistance = !Cfg.DrawDistance; break;
                case "ARC_ToggleSkeletons": Cfg.DrawSkeletons = !Cfg.DrawSkeletons; break;
                case "ARC_ToggleDebug": Cfg.ShowDebug = !Cfg.ShowDebug; break;

                case "ARC_Attach": Attach(); break;
                case "ARC_DisposeVmm": Dispose(); break;
            }
        }

        private static void EnsureArcKeybindProfile()
        {
            var root = Path.Combine(AppContext.BaseDirectory, "Configs", "ARC");
            var path = Path.Combine(root, "arc.keybinds.json");
            if (File.Exists(path)) return;

            Directory.CreateDirectory(root);

            var prof = new KeybindProfile
            {
                ProfileName = "ARC Keybinds",
                Category = "ARC",
                Binds = new List<KeybindEntry>
                {
                    new KeybindEntry { Name="ARC: Toggle Boxes",   Vk=0xC0, Mode=KeybindMode.OnPress, Action="ARC_ToggleBoxes" },
                    new KeybindEntry { Name="ARC: Toggle Names",   Vk=0x4E, Mode=KeybindMode.OnPress, Action="ARC_ToggleNames" },
                    new KeybindEntry { Name="ARC: Toggle Dist",    Vk=0x44, Mode=KeybindMode.OnPress, Action="ARC_ToggleDistance" },
                    new KeybindEntry { Name="ARC: Toggle Skel",    Vk=0x53, Mode=KeybindMode.OnPress, Action="ARC_ToggleSkeletons" },
                    new KeybindEntry { Name="ARC: Toggle Debug",   Vk=0x47, Mode=KeybindMode.OnPress, Action="ARC_ToggleDebug" },
                    new KeybindEntry { Name="ARC: Web Radar",      Vk=0x52, Mode=KeybindMode.OnPress, Action="ARC_ToggleWebRadar" },
                    new KeybindEntry { Name="ARC: Attach",         Vk=0x41, Mode=KeybindMode.OnPress, Action="ARC_Attach" },
                    new KeybindEntry { Name="ARC: Dispose VMM",    Vk=0x58, Mode=KeybindMode.OnPress, Action="ARC_DisposeVmm" },
                }
            };

            KeybindRegistry.Save(path, prof);
        }
        private sealed class ArcEntity
        {
            public string Name;
            public int Distance;
            public Vector3 Pos;
        }
    }
}
