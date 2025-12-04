using ImGuiNET;
using MamboDMA.Services;
using System;
using System.Numerics;

namespace MamboDMA.Games.Deadlock
{
    internal class DeadlockGame : IGame
    {
        public DeadlockGame() { }
        public string Name => "Deadlock";

        private bool _initialized;
        private bool _running;

        private static DeadlockESP.Settings Cfg => DeadlockESP.Cfg;

        public void Initialize()
        {
            if (_initialized) return;
            // Initialization logic here
            _initialized = true;
        }

        public void Attach() => VmmService.Attach("Deadlock.exe");
        public void Start()
        {
            if (_running) return;
            // Start worker threads or background tasks here
            _running = true;
        }

        public void Stop()
        {
            if (!_running) return;
            // Stop worker threads or background tasks here
            _running = false;
        }

        public void Tick()
        {
            if (!_running) return;
            // Per-frame update logic here
        }

        public void Draw(ImGuiNET.ImGuiWindowFlags winFlags)
        {
            // Draw configuration controls
            DeadlockESP.DrawConfigPanel();

            // Inline status indicator
            var dl = ImGui.GetWindowDrawList();
            var p = ImGui.GetCursorScreenPos();
            float y = p.Y + ImGui.GetTextLineHeight() * 0.5f;
            var col = _running ? new Vector4(0f, 0.85f, 0f, 1f) : new Vector4(0.9f, 0.2f, 0.2f, 1f);
            dl.AddCircleFilled(new Vector2(p.X + 5, y), 5, ImGui.ColorConvertFloat4ToU32(col));
            ImGui.SameLine();
            ImGui.TextDisabled(_running ? "Running" : "Stopped");

            ImGui.Separator();

            // Controls
            if (ImGui.Button(_running ? "Restart Workers" : "Start Workers"))
            {
                if (_running) { Stop(); Start(); }
                else Start();
            }
            ImGui.SameLine();
            if (ImGui.Button("Stop Workers"))
                Stop();

            ImGui.SameLine();
            if (ImGui.Button("Attach"))
                Attach();

            // Render overlay ESP regardless of panel visibility (keeps it simple)
            DeadlockESP.RenderOverlay();
        }

    }
}