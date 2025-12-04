using System;
using System.IO;
using System.Linq;

namespace MamboDMA.Services
{
    public static class VmmService
    {
        public static void InitOnly()
        {
            JobSystem.Schedule(() =>
            {
                try
                {
                    MamboDMA.DmaMemory.InitOnly("fpga", applyMMap: true);
                    Snapshots.Mutate(s => s with { Status = "VMM ready", VmmReady = true });
                }
                catch (Exception ex)
                {
                    Snapshots.Mutate(s => s with { Status = "VMM init failed: " + ex.Message, VmmReady = false });
                }
            });
        }

        public static void Attach(string exe)
        {
            JobSystem.Schedule(() =>
            {
                try
                {
                    if (MamboDMA.DmaMemory.TryAttachOnce(exe, out var err))
                    {
                        Snapshots.Mutate(s => s with
                        {
                            Status = $"Attached PID={MamboDMA.DmaMemory.Pid} base=0x{MamboDMA.DmaMemory.Base:X}",
                            VmmReady = true,
                            Pid = (int)MamboDMA.DmaMemory.Pid,
                            MainBase = MamboDMA.DmaMemory.Base
                        });
                    }
                    else
                    {
                        Snapshots.Mutate(s => s with { Status = "Attach failed: " + err });
                    }
                }
                catch (Exception ex)
                {
                    Snapshots.Mutate(s => s with { Status = "Attach error: " + ex.Message });
                }
            });
        }

        public static void RefreshProcesses()
        {
            JobSystem.Schedule(() =>
            {
                try
                {
                    var list = MamboDMA.DmaMemory.GetProcessList(); // List<ProcEntry>
                    Snapshots.Mutate(s => s with
                    {
                        Processes = (list != null) ? list.ToArray()
                                                   : Array.Empty<MamboDMA.DmaMemory.ProcEntry>()
                    });
                }
                catch (Exception ex)
                {
                    Snapshots.Mutate(s => s with { Status = "Proc list error: " + ex.Message });
                }
            });
        }

        public static void RefreshModules()
        {
            JobSystem.Schedule(() =>
            {
                if (!MamboDMA.DmaMemory.IsVmmReady)
                {
                    Snapshots.Mutate(s => s with { Status = "Init VMM first to enumerate processes." });
                    return;
                }
                try
                {
                    var mods = MamboDMA.DmaMemory.GetModules(); // List<ModuleInfo>
                    Snapshots.Mutate(s => s with
                    {
                        Modules = (mods != null) ? mods.ToArray()
                                                 : Array.Empty<MamboDMA.DmaMemory.ModuleInfo>()
                    });
                }
                catch (Exception ex)
                {
                    Snapshots.Mutate(s => s with { Status = "Modules error: " + ex.Message });
                }
            });
        }

        // ---------- PER-MODULE DUMPS ----------

        private static MamboDMA.DmaMemory.ModuleInfo FindModuleInSnapshot(string moduleName)
        {
            var mods = Snapshots.Current.Modules ?? Array.Empty<MamboDMA.DmaMemory.ModuleInfo>();
            foreach (var m in mods)
            {
                if (string.Equals(m.Name, moduleName, StringComparison.OrdinalIgnoreCase))
                    return m;
            }
            return null;
        }

        public static void DumpModule(string moduleName)
        {
            JobSystem.Schedule(() =>
            {
                try
                {
                    var mod = FindModuleInSnapshot(moduleName);
                    if (mod == null)
                    {
                        Snapshots.Mutate(s => s with
                        {
                            Status = $"Dump failed: module '{moduleName}' not found in snapshot."
                        });
                        return;
                    }

                    string safeName = moduleName.Replace(Path.DirectorySeparatorChar, '_')
                                                .Replace(Path.AltDirectorySeparatorChar, '_');

                    string file = Utils.MemoryDumper.MakeDefaultPath($"Dump_{safeName}_full.bin");

                    bool ok = Utils.MemoryDumper.DumpFullModule(mod, file);

                    Snapshots.Mutate(s => s with
                    {
                        Status = ok
                            ? $"Dumped full module: {file}"
                            : $"Failed dumping module: {moduleName}"
                    });
                }
                catch (Exception ex)
                {
                    Snapshots.Mutate(s => s with { Status = "Dump error: " + ex.Message });
                }
            });
        }


        public static void DumpModuleText(string moduleName)
        {
            JobSystem.Schedule(() =>
            {
                try
                {
                    var modOpt = FindModuleInSnapshot(moduleName);
                    if (modOpt == null)
                    {
                        Snapshots.Mutate(s => s with
                        {
                            Status = $"Dump .text failed: module '{moduleName}' not found in snapshot."
                        });
                        return;
                    }

                    var mod = modOpt;
                    string safeName = moduleName.Replace(Path.DirectorySeparatorChar, '_')
                                                .Replace(Path.AltDirectorySeparatorChar, '_');
                    string file = Utils.MemoryDumper.MakeDefaultPath($"Dump_{safeName}_text.bin");

                    bool ok = Utils.MemoryDumper.DumpSection(mod, ".text", file);

                    Snapshots.Mutate(s => s with
                    {
                        Status = ok
                        ? $"Dumped .text section: {file}"
                        : $"Failed dumping .text for {moduleName}"
                    });
                }
                catch (Exception ex)
                {
                    Snapshots.Mutate(s => s with { Status = "Dump error: " + ex.Message });
                }
            });
        }

        public static void DumpModuleData(string moduleName)
        {
            JobSystem.Schedule(() =>
            {
                try
                {
                    var modOpt = FindModuleInSnapshot(moduleName);
                    if (modOpt == null)
                    {
                        Snapshots.Mutate(s => s with
                        {
                            Status = $"Dump .data failed: module '{moduleName}' not found in snapshot."
                        });
                        return;
                    }

                    var mod = modOpt;
                    string safeName = moduleName.Replace(Path.DirectorySeparatorChar, '_')
                                                .Replace(Path.AltDirectorySeparatorChar, '_');
                    string file = Utils.MemoryDumper.MakeDefaultPath($"Dump_{safeName}_data.bin");

                    bool ok = Utils.MemoryDumper.DumpSection(mod, ".data", file);

                    Snapshots.Mutate(s => s with
                    {
                        Status = ok
                        ? $"Dumped .data section: {file}"
                        : $"Failed dumping .data for {moduleName}"
                    });
                }
                catch (Exception ex)
                {
                    Snapshots.Mutate(s => s with { Status = "Dump error: " + ex.Message });
                }
            });
        }

        // ---------- BULK DUMPS ----------

        public static void DumpAllText()
        {
            JobSystem.Schedule(() =>
            {
                try
                {
                    var mods = MamboDMA.DmaMemory.GetModules();
                    foreach (var mod in mods)
                    {
                        string safeName = mod.Name.Replace(Path.DirectorySeparatorChar, '_')
                                                  .Replace(Path.AltDirectorySeparatorChar, '_');
                        string file = Utils.MemoryDumper.MakeDefaultPath($"Dump_{safeName}_text.bin");
                        Utils.MemoryDumper.DumpSection(mod, ".text", file);
                    }

                    Snapshots.Mutate(s => s with { Status = "Dumped ALL .text sections." });
                }
                catch (Exception ex)
                {
                    Snapshots.Mutate(s => s with { Status = "Dump error: " + ex.Message });
                }
            });
        }

        public static void DumpAllData()
        {
            JobSystem.Schedule(() =>
            {
                try
                {
                    var mods = MamboDMA.DmaMemory.GetModules();
                    foreach (var mod in mods)
                    {
                        string safeName = mod.Name.Replace(Path.DirectorySeparatorChar, '_')
                                                  .Replace(Path.AltDirectorySeparatorChar, '_');
                        string file = Utils.MemoryDumper.MakeDefaultPath($"Dump_{safeName}_data.bin");
                        Utils.MemoryDumper.DumpSection(mod, ".data", file);
                    }

                    Snapshots.Mutate(s => s with { Status = "Dumped ALL .data sections." });
                }
                catch (Exception ex)
                {
                    Snapshots.Mutate(s => s with { Status = "Dump error: " + ex.Message });
                }
            });
        }

        /// <summary>Dispose VMM + clear related state (safe to call repeatedly).</summary>
        public static void DisposeVmm()
        {
            JobSystem.Schedule(() =>
            {
                if (!MamboDMA.DmaMemory.IsVmmReady)
                {
                    Snapshots.Mutate(s => s with { Status = "Init VMM first to enumerate processes." });
                    return;
                }
                try
                {
                    MamboDMA.DmaMemory.Dispose();
                    Snapshots.Mutate(s => s with
                    {
                        Status = "Disposed.",
                        VmmReady = false,
                        Processes = Array.Empty<MamboDMA.DmaMemory.ProcEntry>(),
                        Modules = Array.Empty<MamboDMA.DmaMemory.ModuleInfo>(),
                        Pid = 0,
                        MainBase = 0
                    });
                }
                catch (Exception ex)
                {
                    Snapshots.Mutate(s => s with { Status = "Dispose error: " + ex.Message });
                }
            });
        }
    }
}
