using ImGuiNET;
using MamboDMA.Services;
using System.Numerics;
using static MamboDMA.Misc;

public static class ServiceDemoUI
{
    private static string _exe = "example.exe";
    private static int _selectedModuleIndex = -1;
    public static void Draw()
    {
        ImGui.PushFont(Fonts.Bold);
        bool open = ImGui.Begin("Service · Control", ImGuiWindowFlags.NoCollapse);
        ImGui.PopFont();
        if (!open) { ImGui.End(); return; }

        var s = Snapshots.Current;

        ImGui.TextColored(new Vector4(0.6f, 0.8f, 1f, 1f), s.Status ?? "Idle");
        ImGui.Separator();

        // VMM control
        if (ImGui.Button("Init VMM (no attach)")) VmmService.InitOnly();
        ImGui.SameLine();
        if (ImGui.Button("Dispose VMM")) VmmService.DisposeVmm();

        ImGui.Separator();

        // Attach + processes
        ImGui.InputText("Process", ref _exe, 256);
        if (ImGui.Button("Attach")) VmmService.Attach(_exe);
        ImGui.SameLine();
        if (ImGui.Button("Refresh Processes")) VmmService.RefreshProcesses();

        ImGui.Separator();
        ImGui.TextDisabled($"VMM Ready: {s.VmmReady} | PID: {s.Pid} | Base: 0x{s.MainBase:X}");

        ImGui.Text("Processes:");
        ImGui.BeginChild("proc_child", new Vector2(0, 150), ImGuiChildFlags.None);
        foreach (var p in s.Processes)
            ImGui.TextUnformatted($"{p.Pid,6}  {p.Name}{(p.IsWow64 ? " (Wow64)" : "")}");
        if (s.Processes.Length == 0)
            ImGui.TextDisabled("(no processes – init VMM / refresh)");
        ImGui.EndChild();

        ImGui.Separator();

        // -------- MODULES + DUMPER --------
        if (ImGui.Button("Refresh Modules"))
            VmmService.RefreshModules();

        ImGui.SameLine();
        if (ImGui.Button("Dump ALL .text"))
            VmmService.DumpAllText();

        ImGui.SameLine();
        if (ImGui.Button("Dump ALL .data"))
            VmmService.DumpAllData();

        ImGui.Separator();
        ImGui.Text("Modules:");

        ImGui.BeginChild("mods_child", new Vector2(0, 200), ImGuiChildFlags.None);

        if (s.Modules.Length == 0)
        {
            ImGui.TextDisabled("(no modules – attach a process and refresh)");
        }
        else
        {
            for (int i = 0; i < s.Modules.Length; i++)
            {
                var m = s.Modules[i];

                // clickable row
                bool selected = (i == _selectedModuleIndex);
                if (ImGui.Selectable(
                        $"{m.Name,-28}  Base=0x{m.Base:X}  Size=0x{m.Size:X}",
                        selected))
                {
                    _selectedModuleIndex = i;
                }

                // buttons on their OWN line, indented
                ImGui.Indent();
                if (ImGui.Button($"Dump Full##{i}"))
                    VmmService.DumpModule(m.Name);

                ImGui.SameLine();
                if (ImGui.Button($"Dump .text##{i}"))
                    VmmService.DumpModuleText(m.Name);

                ImGui.SameLine();
                if (ImGui.Button($"Dump .data##{i}"))
                    VmmService.DumpModuleData(m.Name);

                ImGui.Unindent();
                ImGui.Separator();
            }
        }

        ImGui.EndChild();


        ImGui.End();
    }
}
