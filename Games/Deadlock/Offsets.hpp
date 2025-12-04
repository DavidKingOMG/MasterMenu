https://github.com/neverlosecc/source2sdk/tree/deadlock/sdk/include/source2sdk
#pragma once
#include <cstdint>

//
//  Deadlock – ESP-Relevant Offsets
//  Extracted from Deadlock SDK patterns
//


namespace Offsets {

    //
    // ============================================================
    //  Global Signatures (Patterns to find in memory)
    // ============================================================
    //
    namespace Signatures {
        // Client.dll patterns
        inline constexpr const char* GetClientEntity = "48 89 5C 24 ? 48 89 74 24 ? 57 48 83 EC 20 8B DA 48 8B F9";
        inline constexpr const char* GetLocalPlayer = "48 83 EC 28 48 8B 0D ? ? ? ? E8 ? ? ? ? 48 85 C0 74 0A";

        // Engine.dll patterns
        inline constexpr const char* GetViewAngles = "48 8B C4 48 89 58 08 48 89 70 10 57 48 83 EC 50";
        inline constexpr const char* SetViewAngles = "40 53 48 83 EC 30 0F 29 74 24 ? 48 8B D9";
        inline constexpr const char* WorldToScreen = "48 89 5C 24 ? 48 89 74 24 ? 57 48 83 EC 70 48 8B F2";
    }



    //
    // ============================================================
    //  CEntityInstance  
    // ============================================================
    //
    namespace CEntityInstance {
        inline constexpr std::ptrdiff_t m_pEntity = 0x10;           // IEntity*
        inline constexpr std::ptrdiff_t m_CScriptComponent = 0x30;  // CScriptComponent*
        inline constexpr std::ptrdiff_t m_bDormant = 0x1A;          // bool
        inline constexpr std::ptrdiff_t m_flSimulationTime = 0x268; // float
        inline constexpr std::ptrdiff_t m_flAnimTime = 0x26C;       // float
    }



    //
    // ============================================================
    //  C_BaseEntity  (inherits CEntityInstance)
    // ============================================================
    //
    namespace C_BaseEntity {
        // Components (Verified from Source2 SDK patterns)
        inline constexpr std::ptrdiff_t m_CBodyComponent = 0x38;    // CBodyComponent*
        inline constexpr std::ptrdiff_t m_pGameSceneNode = 0x330;   // CGameSceneNode*
        inline constexpr std::ptrdiff_t m_pRenderComponent = 0x338; // CRenderComponent*

        // Health
        inline constexpr std::ptrdiff_t m_iMaxHealth = 0x350;
        inline constexpr std::ptrdiff_t m_iHealth = 0x354;

        // State
        inline constexpr std::ptrdiff_t m_lifeState = 0x358;        // byte

        // Team
        inline constexpr std::ptrdiff_t m_iTeamNum = 0x3BF;         // int32_t

        // Velocity
        inline constexpr std::ptrdiff_t m_vecVelocity = 0x438;      // Vector

        // Owner
        inline constexpr std::ptrdiff_t m_hOwnerEntity = 0x3C4;     // CHandle<C_BaseEntity>

        // Model & Bounds
        inline constexpr std::ptrdiff_t m_pModel = 0x120;           // CModel*
        inline constexpr std::ptrdiff_t m_vecMins = 0x328;          // Vector
        inline constexpr std::ptrdiff_t m_vecMaxs = 0x334;          // Vector

        // Spawn & Flags
        inline constexpr std::ptrdiff_t m_spawnflags = 0x340;       // uint32_t
        inline constexpr std::ptrdiff_t m_fFlags = 0x3C8;           // uint32_t
    }



    //
    // ============================================================
    //  C_BasePlayerPawn  
    // ============================================================
    //
    namespace C_BasePlayerPawn {
        // Base class inheritance
        inline constexpr std::ptrdiff_t BaseEntity = 0x0;  // C_BaseEntity

        // Service pointers (Deadlock specific)
        inline constexpr std::ptrdiff_t m_pWeaponServices = 0xF30;
        inline constexpr std::ptrdiff_t m_pItemServices = 0xF38;
        inline constexpr std::ptrdiff_t m_pAutoaimServices = 0xF40;
        inline constexpr std::ptrdiff_t m_pObserverServices = 0xF48;
        inline constexpr std::ptrdiff_t m_pWaterServices = 0xF50;
        inline constexpr std::ptrdiff_t m_pUseServices = 0xF58;
        inline constexpr std::ptrdiff_t m_pFlashlightServices = 0xF60;
        inline constexpr std::ptrdiff_t m_pCameraServices = 0xF68;
        inline constexpr std::ptrdiff_t m_pMovementServices = 0xF70;

        // View angles
        inline constexpr std::ptrdiff_t m_angEyeAngles = 0xFE8;     // QAngle
        inline constexpr std::ptrdiff_t v_angle = 0xFE8;            // Same as above
        inline constexpr std::ptrdiff_t v_anglePrevious = 0xFF4;    // QAngle

        // Camera & View
        inline constexpr std::ptrdiff_t m_iFOV = 0x1004;            // int32_t
        inline constexpr std::ptrdiff_t m_iDefaultFOV = 0x1008;     // int32_t
        inline constexpr std::ptrdiff_t m_flFOVTime = 0x100C;       // float

        // HUD
        inline constexpr std::ptrdiff_t m_iHideHUD = 0x1010;        // int32_t

        // Death & respawn
        inline constexpr std::ptrdiff_t m_flDeathTime = 0x1098;     // float
        inline constexpr std::ptrdiff_t m_iObserverMode = 0x10A0;   // int32_t
        inline constexpr std::ptrdiff_t m_hObserverTarget = 0x10A4; // CHandle<C_BaseEntity>

        // Prediction
        inline constexpr std::ptrdiff_t m_vecPredictionError = 0x109C;      // Vector
        inline constexpr std::ptrdiff_t m_flPredictionErrorTime = 0x10A8;   // float

        // Camera setup
        inline constexpr std::ptrdiff_t m_vecLastCameraSetupLocalOrigin = 0x10C8;  // Vector
        inline constexpr std::ptrdiff_t m_flLastCameraSetupTime = 0x10D4;          // float

        // Input
        inline constexpr std::ptrdiff_t m_flFOVSensitivityAdjust = 0x10D8;  // float
        inline constexpr std::ptrdiff_t m_flMouseSensitivity = 0x10DC;      // float

        // Origin & interpolation
        inline constexpr std::ptrdiff_t m_vOldOrigin = 0x10E0;              // Vector
        inline constexpr std::ptrdiff_t m_flOldSimulationTime = 0x10EC;     // float

        // Controllers
        inline constexpr std::ptrdiff_t m_hController = 0x10F8;             // CHandle<CBasePlayerController>
        inline constexpr std::ptrdiff_t m_hDefaultController = 0x10FC;      // CHandle<CBasePlayerController>

        // Weapon info
        inline constexpr std::ptrdiff_t m_hActiveWeapon = 0x1040;           // CHandle<C_BaseEntity>
        inline constexpr std::ptrdiff_t m_hMyWeapons = 0x1048;              // C_NetworkUtlVectorBase<CHandle<C_BaseEntity>>

        // Ammo
        inline constexpr std::ptrdiff_t m_iAmmo = 0x1060;                   // int32_t[32]
    }



    //
    // ============================================================
    //  PlayerController  
    // ============================================================
    //
    namespace PlayerController {
        // Player info
        inline constexpr std::ptrdiff_t m_iszPlayerName = 0x5A8;            // CUtlSymbolLarge
        inline constexpr std::ptrdiff_t m_szNetworkIDString = 0x5B0;        // char[128]

        // Team
        inline constexpr std::ptrdiff_t m_iTeamNum = 0x3BF;                 // int32_t (inherited)

        // Pawn
        inline constexpr std::ptrdiff_t m_hPawn = 0x5DC;                    // CHandle<C_BasePlayerPawn>

        // SteamID
        inline constexpr std::ptrdiff_t m_steamID = 0x5E0;                  // uint64_t

        // View angles
        inline constexpr std::ptrdiff_t m_vecViewAngles = 0x128;            // QAngle

        // Tick
        inline constexpr std::ptrdiff_t m_nTickBase = 0x340;                // uint32_t

        // Local player
        inline constexpr std::ptrdiff_t m_bIsLocalPlayerController = 0x6A8; // bool

        // Buttons
        inline constexpr std::ptrdiff_t m_nButtons = 0x234;                 // int32_t
        inline constexpr std::ptrdiff_t m_nImpulse = 0x238;                 // int32_t

        // Crosshair
        inline constexpr std::ptrdiff_t m_hCrosshairEntity = 0x118;         // CHandle<C_BaseEntity>

        // Score
        inline constexpr std::ptrdiff_t m_iScore = 0x5F0;                   // int32_t
        inline constexpr std::ptrdiff_t m_iKills = 0x5F4;                   // int32_t
        inline constexpr std::ptrdiff_t m_iDeaths = 0x5F8;                  // int32_t
        inline constexpr std::ptrdiff_t m_iAssists = 0x5FC;                 // int32_t
    }



    //
    // ============================================================
    //  CGameSceneNode
    // ============================================================
    //
    namespace CGameSceneNode {
        inline constexpr std::ptrdiff_t m_vecAbsOrigin = 0xC8;              // Vector
        inline constexpr std::ptrdiff_t m_angAbsRotation = 0xD4;            // QAngle
        inline constexpr std::ptrdiff_t m_modelToWorld = 0x40;              // matrix3x4_t
        inline constexpr std::ptrdiff_t m_pParent = 0x18;                   // CGameSceneNode*
        inline constexpr std::ptrdiff_t m_pNextSibling = 0x20;              // CGameSceneNode*
        inline constexpr std::ptrdiff_t m_pFirstChild = 0x28;               // CGameSceneNode*
        inline constexpr std::ptrdiff_t m_vecScale = 0xE0;                  // Vector
        inline constexpr std::ptrdiff_t m_dwFlags = 0x38;                   // uint32_t
        inline constexpr std::ptrdiff_t m_hOwnerEntity = 0x3C;              // CHandle<C_BaseEntity>
        inline constexpr std::ptrdiff_t m_vRenderOrigin = 0x100;            // Vector (interpolated)
        inline constexpr std::ptrdiff_t m_bDormant = 0x1A0;                 // bool
        inline constexpr std::ptrdiff_t m_iParentAttachment = 0x1A4;        // int32_t
    }



    //
    // ============================================================
    //  RenderComponent
    // ============================================================
    //
    namespace RenderComponent {
        inline constexpr std::ptrdiff_t m_Color = 0x108;                    // Color
        inline constexpr std::ptrdiff_t m_bVisible = 0x104;                 // bool
        inline constexpr std::ptrdiff_t m_nRenderMode = 0x10C;              // int32_t
        inline constexpr std::ptrdiff_t m_nModelIndex = 0x110;              // int32_t
        inline constexpr std::ptrdiff_t m_pModel = 0x118;                   // CModel*
        inline constexpr std::ptrdiff_t m_vecRenderBoundsMin = 0x1A0;       // Vector
        inline constexpr std::ptrdiff_t m_vecRenderBoundsMax = 0x1AC;       // Vector
        inline constexpr std::ptrdiff_t m_bDisableShadowDepthRendering = 0x248; // bool
        inline constexpr std::ptrdiff_t m_bRenderInFastReflection = 0x249;  // bool
        inline constexpr std::ptrdiff_t m_nFXIndex = 0x24C;                 // int32_t
        inline constexpr std::ptrdiff_t m_nSceneObjectOverrideFlags = 0x250; // uint32_t
        inline constexpr std::ptrdiff_t m_iTextureFrameIndex = 0x254;       // int32_t
    }



    //
    // ============================================================
    //  Weapon & Item Offsets
    // ============================================================
    //
    namespace Weapon {
        inline constexpr std::ptrdiff_t m_iClip1 = 0x140;                   // int32_t
        inline constexpr std::ptrdiff_t m_iClip2 = 0x144;                   // int32_t
        inline constexpr std::ptrdiff_t m_iPrimaryAmmoType = 0x148;         // int32_t
        inline constexpr std::ptrdiff_t m_iSecondaryAmmoType = 0x14C;       // int32_t
        inline constexpr std::ptrdiff_t m_flNextPrimaryAttack = 0x150;      // float
        inline constexpr std::ptrdiff_t m_flNextSecondaryAttack = 0x154;    // float
        inline constexpr std::ptrdiff_t m_iState = 0x158;                   // int32_t
        inline constexpr std::ptrdiff_t m_hOwner = 0x15C;                   // CHandle<C_BaseEntity>
        inline constexpr std::ptrdiff_t m_iViewModelIndex = 0x160;          // int32_t
        inline constexpr std::ptrdiff_t m_iWorldModelIndex = 0x164;         // int32_t
        inline constexpr std::ptrdiff_t m_szWeaponName = 0x168;             // char[64]
    }



    //
    // ============================================================
    //  Global Variables
    // ============================================================
    //
    namespace GlobalVars {
        inline constexpr std::ptrdiff_t interval_per_tick = 0x14;           // float
        inline constexpr std::ptrdiff_t current_time = 0x2C;                // float
        inline constexpr std::ptrdiff_t frame_count = 0x38;                 // int32_t
        inline constexpr std::ptrdiff_t max_clients = 0x44;                 // int32_t
        inline constexpr std::ptrdiff_t tick_count = 0x48;                  // int32_t
        inline constexpr std::ptrdiff_t interval_per_tick_raw = 0x50;       // float
    }



    //
    // ============================================================
    //  Interface Pointers (Windows x64)
    // ============================================================
    //
    namespace Interfaces {
        // Client.dll
        inline constexpr std::ptrdiff_t Client = 0x1829F28;
        inline constexpr std::ptrdiff_t EntityList = 0x182A2E8;
        inline constexpr std::ptrdiff_t Input = 0x182A4F8;
        inline constexpr std::ptrdiff_t GameMovement = 0x182A518;
        inline constexpr std::ptrdiff_t Prediction = 0x182A528;

        // Engine.dll
        inline constexpr std::ptrdiff_t Engine = 0x61C3C0;
        inline constexpr std::ptrdiff_t NetworkServerService = 0x61C3E8;
        inline constexpr std::ptrdiff_t NetworkClientService = 0x61C3F0;
        inline constexpr std::ptrdiff_t DebugOverlay = 0x61C408;

        // Materialsystem.dll
        inline constexpr std::ptrdiff_t MaterialSystem = 0x524390;
        inline constexpr std::ptrdiff_t StudioRender = 0x5243A8;

        // VGUI2.dll
        inline constexpr std::ptrdiff_t VGuiPanel = 0x26EE8;
        inline constexpr std::ptrdiff_t VGuiSurface = 0x26EF0;
    }



    //
    // ============================================================
    //  VTable Indexes (Common Functions)
    // ============================================================
    //
    namespace VTable {
        namespace IClientEntityList {
            inline constexpr std::ptrdiff_t GetClientEntity = 3;
            inline constexpr std::ptrdiff_t GetClientEntityFromHandle = 4;
            inline constexpr std::ptrdiff_t GetHighestEntityIndex = 6;
        }

        namespace IEngineClient {
            inline constexpr std::ptrdiff_t GetLocalPlayer = 12;
            inline constexpr std::ptrdiff_t GetViewAngles = 18;
            inline constexpr std::ptrdiff_t SetViewAngles = 19;
            inline constexpr std::ptrdiff_t GetMaxClients = 20;
            inline constexpr std::ptrdiff_t IsInGame = 26;
            inline constexpr std::ptrdiff_t IsConnected = 27;
            inline constexpr std::ptrdiff_t WorldToScreenMatrix = 37;
            inline constexpr std::ptrdiff_t GetNetChannelInfo = 78;
        }

        namespace C_BaseEntity {
            inline constexpr std::ptrdiff_t GetAbsOrigin = 10;
            inline constexpr std::ptrdiff_t GetAbsAngles = 11;
            inline constexpr std::ptrdiff_t GetRenderBounds = 17;
            inline constexpr std::ptrdiff_t SetupBones = 13;
            inline constexpr std::ptrdiff_t GetClientClass = 2;
        }
    }

} // namespace Offsets
