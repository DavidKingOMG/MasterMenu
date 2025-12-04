using System;
using System.IO;
using System.Text;
using MamboDMA.Services;

namespace MamboDMA.Utils
{
    public static class MemoryDumper
    {
        // Dump directory: <exe>/Dumps
        private static readonly string DumpDir =
            Path.Combine(AppContext.BaseDirectory, "Dumps");

        static MemoryDumper()
        {
            try { Directory.CreateDirectory(DumpDir); }
            catch { /* ignore */ }
        }

        public static string MakeDefaultPath(string fileName)
        {
            return Path.Combine(DumpDir, fileName);
        }

        // -------------------------------------------------------------
        // PUBLIC API (cleaned, correct overloads)
        // -------------------------------------------------------------

        /// <summary>Dump module by name.</summary>
        public static bool DumpFullModule(string moduleName, string outputPath)
        {
            var mod = FindModule(moduleName);
            if (mod == null)
            {
                Console.WriteLine($"[Dumper] Module '{moduleName}' not found.");
                return false;
            }

            return DumpFullModule(mod, outputPath);
        }

        /// <summary>Dump module by ModuleInfo.</summary>
        public static bool DumpFullModule(DmaMemory.ModuleInfo mod, string outputPath)
        {
            uint size = (uint)Math.Min(mod.Size, (ulong)uint.MaxValue);

            if (size == 0)
            {
                Console.WriteLine($"[Dumper] Module '{mod.Name}' has size 0.");
                return false;
            }

            return DumpRegion(mod.Base, size, outputPath);
        }

        /// <summary>Dump section by module name.</summary>
        public static bool DumpSection(string moduleName, string sectionName, string outputPath)
        {
            var mod = FindModule(moduleName);
            if (mod == null)
            {
                Console.WriteLine($"[Dumper] Module '{moduleName}' not found.");
                return false;
            }

            return DumpSection(mod, sectionName, outputPath);
        }

        /// <summary>Dump section by ModuleInfo.</summary>
        public static bool DumpSection(DmaMemory.ModuleInfo mod, string sectionName, string outputPath)
        {
            if (!TryGetSection(mod, sectionName, out ulong sectionBase, out uint sectionSize))
            {
                Console.WriteLine($"[Dumper] Section '{sectionName}' not found in {mod.Name}.");
                return false;
            }

            return DumpRegion(sectionBase, sectionSize, outputPath);
        }

        /// <summary>Dump raw region.</summary>
        public static bool DumpRegion(ulong address, uint size, string outputPath)
        {
            try
            {
                if (size == 0)
                {
                    Console.WriteLine("[Dumper] Requested size=0.");
                    return false;
                }

                byte[]? data = DmaMemory.ReadBytes(address, size);

                if (data == null || data.Length == 0)
                {
                    Console.WriteLine("[Dumper] ReadBytes returned empty.");
                    return false;
                }

                string? dir = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);

                File.WriteAllBytes(outputPath, data);
                Console.WriteLine($"[Dumper] Dumped {data.Length:N0} bytes -> {outputPath}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Dumper] Exception: {ex}");
                return false;
            }
        }

        // -------------------------------------------------------------
        // MODULE + SECTION HELPERS
        // -------------------------------------------------------------

        private static DmaMemory.ModuleInfo FindModule(string name)
        {
            var mods = DmaMemory.GetModules();

            foreach (var m in mods)
            {
                if (string.Equals(m.Name, name, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(Path.GetFileName(m.FullName), name, StringComparison.OrdinalIgnoreCase))
                {
                    return m;
                }
            }
            return null;
        }

        private static bool TryGetSection(
            DmaMemory.ModuleInfo mod,
            string sectionName,
            out ulong sectionBase,
            out uint sectionSize)
        {
            sectionBase = 0;
            sectionSize = 0;

            byte[]? dos = DmaMemory.ReadBytes(mod.Base, 0x1000);
            if (dos == null || dos.Length < 0x40)
                return false;

            if (dos[0] != 'M' || dos[1] != 'Z')
                return false;

            uint e_lfanew = BitConverter.ToUInt32(dos, 0x3C);

            if (e_lfanew == 0 || e_lfanew > 0x800)
                return false;

            ulong ntAddr = mod.Base + e_lfanew;

            byte[]? nth = DmaMemory.ReadBytes(ntAddr, 0x400);
            if (nth == null || nth.Length < 0x18)
                return false;

            if (nth[0] != 'P' || nth[1] != 'E')
                return false;

            ushort numSections = BitConverter.ToUInt16(nth, 6);
            ushort optHeaderSize = BitConverter.ToUInt16(nth, 20);

            ulong firstSection = ntAddr + 0x18 + optHeaderSize;

            for (int i = 0; i < numSections; i++)
            {
                ulong secAddr = firstSection + (ulong)(i * 40);

                byte[]? header = DmaMemory.ReadBytes(secAddr, 40);
                if (header == null || header.Length < 40)
                    continue;

                string nm = Encoding.ASCII.GetString(header, 0, 8).TrimEnd('\0');

                if (string.Equals(nm, sectionName, StringComparison.OrdinalIgnoreCase))
                {
                    uint virtualSize = BitConverter.ToUInt32(header, 8);
                    uint rva = BitConverter.ToUInt32(header, 12);
                    uint rawSize = BitConverter.ToUInt32(header, 16);

                    sectionBase = mod.Base + rva;
                    sectionSize = rawSize != 0 ? rawSize : virtualSize;

                    return sectionSize != 0;
                }
            }

            return false;
        }
    }
}
