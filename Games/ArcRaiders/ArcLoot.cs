using MamboDMA.Games.ABI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MamboDMA.Games.ArcRaiders
{
    internal class ArcLoot
    {
        // ©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤
        // Debug structures
        // ©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤
        public enum LabelSource : byte { None, Common_DisplayName, Common_SimpleName, Fallback_ClassName }
        public enum PriceSource : byte { None, Common_StandardPrice, Provider_ClassName }

        public struct ItemDebug
        {
            public LabelSource LabelSrc;
            public PriceSource PriceSrc;
            public ulong CommonDataPtr;
            public int CommonDataSlot;
        }

        public struct ContainerDebug
        {
            public bool UsedManagerPath;
            public int ManagerOffsetTried;
            public int BaseListCount;
            public int HeuristicArraysTried;
            public int HeuristicItemsFound;
            public ulong CommonDataPtr;
            public int CommonDataSlot;
        }

        public struct DebugSnapshot
        {
            public int ActorsScanned;
            public int ItemsLoose;
            public int ContainersSeen;
            public int ContainersExpandedMgr;
            public int ContainersExpandedHeu;
            public int PricesFromCommon;
            public int PricesFromProvider;
            public int LabelsFromCommon;
        }

        // ©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤
        // Public data model
        // ©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤
        public struct Item
        {
            public ulong Actor;
            public ulong ContainerActor;
            public bool InContainer;
            public string ClassName;
            public string Label;
            public int Stack;
            public Vector3 Position;
            public int ApproxPrice;
            public int Rarity;           // NEW: From CommonData
            public ItemDebug Debug;
        }

        public struct Container
        {
            public ulong Actor;
            public Vector3 Position;
            public string ClassName;
            public string Label;
            public int ItemCount;
            public int ApproxPrice;      // NEW: Container value
            public int Rarity;           // NEW: Container rarity
            public bool IsSearched;
            public bool IsEmpty => ItemCount == 0;
            public ContainerDebug Debug;
        }

        public struct Frame
        {
            public long StampTicks;
            public List<Item> Items;
            public List<Container> Containers;
            public int TotalActorsSeen;
            public int ContainersFound;
            public int ContainersExpanded;
        }

        public interface IPriceProvider { int TryGetPrice(string className); }

        // ©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤
        // Confirmed offsets from SDK
        // ©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤
        internal static class InvCommonOffsets
        {
            public const int OFF_StandardPrice = 0x010C;
            public const int OFF_Rarity = 0x0110;
            public const int OFF_DisplayName = 0x0138;  // FText
            public const int OFF_Description = 0x0150;  // FText
            public const int OFF_SimpleName = 0x0168;  // FText
        }

        // CRITICAL: Different offsets for items vs containers!
        internal static class ComponentSlots
        {
            public const int ITEM_COMMONDATA = 0x08A0;      // Items: ABP_AmmoBase_C, etc.
            public const int CONTAINER_COMMONDATA = 0x08B0;  // Containers: ABP_ArmoredContainerBase_C
            public const int ITEM_PICKUPMESH = 0x08E0;      // For better item positions
        }

        internal static class LootSceneOffsets
        {
            public static ulong AActor_RootComponent = ABIOffsets.AActor_RootComponent;
            public static ulong USceneComponent_ComponentToWorld_Ptr = ABIOffsets.USceneComponent_ComponentToWorld_Ptr;
        }

        // Container manager offsets
        private static readonly int[] CANDIDATE_MGR_OFFS = { 0x8F0, 0x8E8, 0x8A0 };
        private const int OFF_MGR_BASELIST = 0x140;
        private const int OFF_INVBASE_ROW = 0x00;
        private const int OFF_INVBASE_COLUMN = 0x04;
        private const int OFF_INVBASE_CHILD_ACTORS = 0x10;
        private const int SIZEOF_INVBASE = 0x48;

        // ©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤
        // Controls
        // ©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤
        public static int UpdateIntervalMs { get => _intervalMs; set => _intervalMs = Math.Clamp(value, 16, 500); }
        public static bool IsRunning => _running;
        public static void SetPriceProvider(IPriceProvider provider) => _priceProvider = provider;
        public static bool EnableDebugLogging = false;

        public static void Start()
        {
            if (_running) return;
            _running = true;
            _thread = new Thread(Loop) { IsBackground = true, Priority = ThreadPriority.AboveNormal, Name = "ABI.Loot" };
            _thread.Start();
        }

        public static void Stop()
        {
            _running = false;
            try { _thread?.Join(250); } catch { }
            _thread = null;
        }

        public static bool TryGetLoot(out Frame f)
        {
            lock (_sync) { f = _latest; return f.StampTicks != 0; }
        }

        public static DebugSnapshot GetDebugSnapshot()
        {
            lock (_sync) return _dbg;
        }

        // ©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤
        // Internal state
        // ©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤©¤
        private static volatile bool _running;
        private static Thread _thread;
        private static readonly object _sync = new();
        private static Frame _latest;
        private static int _intervalMs = 60;
        private static IPriceProvider _priceProvider;

        private static readonly Dictionary<ulong, (long timestamp, List<Item> items)> _searchedContainers = new(256);
        private static readonly TimeSpan _searchedContainerExpiry = TimeSpan.FromMinutes(5);

        private static DebugSnapshot _dbg;
        private static int _dbgContainersHeu;
        private static int _dbgContainersMgr;
        private static int _dbgPricesFromCommon;
        private static int _dbgPricesFromProvider;
        private static int _dbgLabelsFromCommon;


        // 
        //  Main Loop
        // 

        private static void Loop()
        {
            var sw = new System.Diagnostics.Stopwatch();
            while (_running)
            {
                sw.Restart();
                int left = _intervalMs - (int)sw.ElapsedMilliseconds;
            }
        }


    }
}
