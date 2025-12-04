using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MamboDMA.Games.Deadlock
{
    internal class DeadlockGame : IGame
    {
        public DeadlockGame() { }
        public string Name => "Deadlock";

        private bool _initialized;
        private bool _running;

        public void Initialize()
        {
            if (_initialized) return;
            // Initialization logic here
            _initialized = true;
        }

        public void Attach()
        {
            

        }

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
            if (!_running) return;
            // ImGui drawing logic here
        }

    }


}
