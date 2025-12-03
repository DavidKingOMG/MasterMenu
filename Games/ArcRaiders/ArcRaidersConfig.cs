using System.Numerics;

namespace MamboDMA.Games.ArcRaiders
{

    public sealed class ArcRaidersConfig
    {

        public string ArcExe = "PioneerGame.exe";

        // Player ESP Toggles
        public bool DrawBoxes = true;
        public bool DrawNames = true;
        public bool DrawDistance = true;
        public bool DrawSkeletons = false;
        public bool ShowDebug = false;

        // Player ESP Ranges
        public float MaxDrawDistance = 800f;          // meters
        public float MaxSkeletonDistance = 300f;  // meters

        // Line thickness, box scale, whatever else you add later
        public float BoxThickness = 1.5f;

        // Colors
        public Vector4 ColorPlayer = new(1.0f, 0.4f, 0.25f, 1f);
        public Vector4 ColorEnemy = new(1f, 0f, 0f, 1f);
        public Vector4 ColorBot = new(0.8f, 0.8f, 0.2f, 1f);
        public Vector4 ColorBoxVisible = new(0.2f, 0.6f, 1f, 1f);
        public Vector4 ColorBoxInvisible = new(0.2f, 0.6f, 1f, 0.4f);
        public Vector4 ColorSkelVisible = new(0f, 1f, 0f, 1f);
        public Vector4 ColorSkelInvisible = new(0f, 1f, 0f, 0.4f);
        public Vector4 DistanceColor = new(1f, 1f, 1f, 1f);
        public Vector4 ColorName = new(1f, 1f, 1f, 1f);


        // ©¤©¤© Loot Display ©¤©¤©
        // Ground Loot
        public bool DrawGroundLoot = true;
        public bool DrawGroundLootNames = true;
        public bool DrawGroundLootDistance = true;
        public float GroundLootMaxDistance = 150f;
        public float GroundLootMarkerSize = 6f;

        // Containers
        public bool DrawContainers = true;
        public bool DrawContainerNames = true;
        public bool DrawContainerDistance = true;
        public bool DrawEmptyContainers = true;  // Changed default to true
        public float ContainerMaxDistance = 200f;
        public float ContainerMarkerSize = 8f;
        public Vector4 ContainerFilledColor = new(1f, 0.84f, 0f, 1f);
        public Vector4 ContainerEmptyColor = new(0.5f, 0.5f, 0.5f, 1f);

        // Item Filtering
        public string LootFilterInclude = "";
        public string LootFilterExclude = "";
        public int LootMinPrice = 0;  // Legacy - kept for compatibility

        // Loot Display Style - Price-based filtering & coloring
        public int LootMinPriceRegular = 5000;      // Min price for regular loot to show
        public int LootMinPriceImportant = 50000;   // Min price for "important" (highlighted) loot
        public bool LootShowPrice = true;
        public Vector4 LootRegularColor = new(0f, 1f, 0.5f, 1f);      // Green for regular loot
        public Vector4 LootImportantColor = new(1f, 0.1f, 1f, 1f);    // Magenta for valuable loot

    }
}
