using System.Numerics;

namespace MamboDMA.Games.ASA
{
    public sealed class ASAConfig
    {
        public string ASAExe = "ShooterGame.exe";

        // ©¤©¤© Player ESP ©¤©¤©
        public bool DrawPlayerBoxes = true;
        public bool DrawNames = true;
        public bool DrawDistance = true;
        public bool ShowDebug = false;

        public float MaxDrawDistance = 5000f;
        public float BoxThickness = 1.5f;

        public Vector4 ColorFriendly = new(0f, 1f, 0.3f, 1f);
        public Vector4 ColorEnemy = new(1f, 0f, 0f, 1f);
        public Vector4 DistanceColor = new(1f, 1f, 1f, 1f);
        public Vector4 ColorName = new(1f, 1f, 1f, 1f);


        // ©¤©¤© Dino ESP ©¤©¤©
        public bool DrawDinos = true;
        public bool DrawDinoNames = true;
        public bool DrawDinoDistance = true;
        public bool DrawDinoLevels = true;
        public bool HighlightTamed = true;
        public bool HighlightBabies = true;

        public float DinoMaxDistance = 10000f;

        public Vector4 ColorTamed = new(0.2f, 1f, 0.2f, 1f);
        public Vector4 ColorWild = new(1f, 0.8f, 0.3f, 1f);
        public Vector4 ColorAggressive = new(1f, 0.2f, 0.2f, 1f);
        public Vector4 ColorBaby = new(0.5f, 0.7f, 1f, 1f);


        // ©¤©¤© Item / Loot ESP ©¤©¤©
        public bool DrawItems = true;
        public bool DrawItemNames = true;
        public bool DrawItemDistance = true;

        public float ItemMaxDistance = 2500f;

        public Vector4 ColorItem = new(1f, 1f, 1f, 1f);
        public Vector4 ColorItemImportant = new(1f, 0.75f, 0f, 1f);

        public string ItemFilterInclude = "";
        public string ItemFilterExclude = "";

        // (Optional future expansion: loot rarity, item quality, etc.)


        // ©¤©¤© Structure ESP ©¤©¤©
        public bool DrawStructures = false;
        public bool DrawStructureNames = false;
        public bool DrawStructureDistance = false;

        public float StructureMaxDistance = 4000f;

        public Vector4 ColorStructure = new(0.6f, 0.6f, 1f, 1f);


        // ©¤©¤© Extra Visual Settings ©¤©¤©
        // Matches ArcRaidersConfig pattern
        // (You can add more here if ASA overlay evolves)
    }
}
