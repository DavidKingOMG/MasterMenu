
#pragma once
#include <cstdint>

//
//  Deadlock – ESP-Relevant Offsets
//  Extracted from Deadlock SDK patterns
//


namespace DeadlockOffsets {

    //
    // ============================================================
    //  Global Signatures (Patterns to find in memory)
    // ============================================================
    //
    namespace Signatures {
        // Entity list access
        inline constexpr uintptr_t EntityList = 0x182A2E8;
        inline constexpr uintptr_t LocalPlayer = 0x1829F28;

        // View matrix
        inline constexpr uintptr_t ViewMatrix = 0x182A7D8;

        // Interface pointers
        inline constexpr uintptr_t Client = 0x1829F28;
        inline constexpr uintptr_t Engine = 0x61C3C0;



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
        // Basic info
        inline constexpr std::ptrdiff_t m_iHealth = 0x354;
        inline constexpr std::ptrdiff_t m_iTeamNum = 0x3BF;
        inline constexpr std::ptrdiff_t m_lifeState = 0x358;
        inline constexpr std::ptrdiff_t m_vecOrigin = 0x138;  // Actually in CGameSceneNode
        inline constexpr std::ptrdiff_t m_pGameSceneNode = 0x330;
        inline constexpr std::ptrdiff_t m_pRenderComponent = 0x338;
        inline constexpr std::ptrdiff_t m_hPawn = 0x5DC;
        inline constexpr std::ptrdiff_t m_bDormant = 0x1A;

        // For ESP
        inline constexpr std::ptrdiff_t m_iszPlayerName = 0x5A8;
        inline constexpr std::ptrdiff_t m_vecMins = 0x328;
        inline constexpr std::ptrdiff_t m_vecMaxs = 0x334;
        inline constexpr std::ptrdiff_t m_fFlags = 0x3C8;
    }



    //
    // ============================================================
    //  C_BasePlayerPawn  
    // ============================================================
    //
    namespace C_BasePlayerPawn {
        inline constexpr std::ptrdiff_t m_pWeaponServices = 0xF30;
        inline constexpr std::ptrdiff_t m_pItemServices = 0xF38;
        inline constexpr std::ptrdiff_t m_angEyeAngles = 0xFE8;
        inline constexpr std::ptrdiff_t v_angle = 0xFE8;
        inline constexpr std::ptrdiff_t m_iFOV = 0x1004;
        inline constexpr std::ptrdiff_t m_hActiveWeapon = 0x1040;
        inline constexpr std::ptrdiff_t m_hController = 0x10F8;
    }



    //
    // ============================================================
    //  PlayerController  
    // ============================================================
    //
    namespace PlayerController {
        inline constexpr std::ptrdiff_t m_iszPlayerName = 0x5A8;
        inline constexpr std::ptrdiff_t m_iTeamNum = 0x3BF;
        inline constexpr std::ptrdiff_t m_hPawn = 0x5DC;
        inline constexpr std::ptrdiff_t m_steamID = 0x5E0;
        inline constexpr std::ptrdiff_t m_vecViewAngles = 0x128;
    }



    //
    // ============================================================
    //  CGameSceneNode
    // ============================================================
    //
    namespace CGameSceneNode {
        inline constexpr std::ptrdiff_t m_vecAbsOrigin = 0xC8;
        inline constexpr std::ptrdiff_t m_angAbsRotation = 0xD4;
        inline constexpr std::ptrdiff_t m_modelToWorld = 0x40;
        inline constexpr std::ptrdiff_t m_pParent = 0x18;
    }inline constexpr std::ptrdiff_t m_iParentAttachment = 0x1A4;        // int32_t
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
        inline constexpr std::ptrdiff_t m_iClip1 = 0x140;
        inline constexpr std::ptrdiff_t m_iClip2 = 0x144;
        inline constexpr std::ptrdiff_t m_iPrimaryAmmoType = 0x148;
        inline constexpr std::ptrdiff_t m_iSecondaryAmmoType = 0x14C;
        inline constexpr std::ptrdiff_t m_flNextPrimaryAttack = 0x150;
        inline constexpr std::ptrdiff_t m_szWeaponName = 0x168;
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
