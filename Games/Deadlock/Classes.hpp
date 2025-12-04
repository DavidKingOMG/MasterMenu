#pragma once
#include <cstdint>
#include <string>
#include <array>

//
// Deadlock – ESP-Relevant Class Structures
// These DO NOT contain offsets or memory logic.
// Purely structural.
//

struct Vector3 {
    float x, y, z;
};

struct QAngle {
    float pitch, yaw, roll;
};

struct Transform {
    // Fill with whatever your engine uses
    float matrix[12];
};


//
// Base Components
//

struct RenderComponent {
    uint32_t Color;
    bool     bVisible;
    int32_t  ModelIndex;
    void* pModel;
};

struct HealthComponent {
    int32_t  Health;
    int32_t  MaxHealth;
    bool     Alive;
};

struct TeamComponent {
    int32_t TeamNumber;
    int32_t TeamRole;
};

struct WeaponHandle {
    uint32_t index;   // wrapper struct for handles
};

struct WeaponServices {
    WeaponHandle ActiveWeapon;
    std::array<WeaponHandle, 8> Weapons;
};


//
// Scene Node
//
struct CGameSceneNode {
    Vector3 AbsOrigin;
    QAngle  AbsRotation;
    Transform ModelToWorld;
    CGameSceneNode* Parent;
};


//
// Base Entity
//
struct C_BaseEntity {
    CGameSceneNode* SceneNode;
    RenderComponent* RenderComp;
    HealthComponent* HealthComp;

    Vector3              Origin;
    Vector3              Velocity;
};


//
// Player Pawn
//
struct C_BasePlayerPawn : public C_BaseEntity {
    uint32_t             PlayerControllerHandle;
    uint32_t             WeaponServicesHandle;
    uint32_t             CameraServicesHandle;
    int32_t              LifeState;
};


//
// Player Controller
//
struct PlayerController {
    std::string          PlayerName;
    int32_t              TeamNum;
    uint32_t             PawnHandle;
    uint64_t             SteamID;
};
