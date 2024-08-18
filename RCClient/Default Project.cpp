//##############################################################################################################################
// Realm Crafter Professional																									
// Copyright (C) 2013 Solstar Games, LLC. All rights reserved																	
// contact@solstargames.com																																																		
//
// Grand Poohbah: Mark Bryant
// Programmer: Jared Belkus
// Programmer: Frank Puig Placeres
// Programmer: Rob Williams
// 																										
// Program: 
//																																
//This is a licensed product:
//BY USING THIS SOURCECODE, YOU ARE CONFIRMING YOUR ACCEPTANCE OF THE SOFTWARE AND AGREEING TO BECOME BOUND BY THE TERMS OF 
//THIS AGREEMENT. IF YOU DO NOT AGREE TO BE BOUND BY THESE TERMS, THEN DO NOT USE THE SOFTWARE.
//																		
//Licensee may NOT: 
// (i)   create any derivative works of the Engine, including translations Or localizations, other than Games;
// (ii)  redistribute, encumber, sell, rent, lease, sublicense, Or otherwise transfer rights To the Engine// or
// (iii) remove Or alter any trademark, logo, copyright Or other proprietary notices, legends, symbols Or labels in the Engine.
// (iv)   licensee may Not distribute the source code Or documentation To the engine in any manner, unless recipient also has a 
//       license To the Engine.													
// (v)  use the Software to develop any software or other technology having the same primary function as the Software, 
//       including but not limited to using the Software in any development or test procedure that seeks to develop like 
//       software or other technology, or to determine if such software or other technology performs in a similar manner as the
//       Software																																
//##############################################################################################################################

#include <typeinfo>
#include "Default Project.h"
#include <matrix.h>
#include "LightFunctionList.h"
#include "..\NGUI\Source\CRadar.h"
#include "CControlLayout.h"
#include "LT.H"
#include "RCT.h"
#include <RealmCrafter.h>
#include "ClientSDKExport.h"
#include "SplashScreen.h"

// TODO: REMOVE THis
std::vector<ILabel*> ActorDBLs;
int LastSectorChange = 10;

ASyncFuncs* Perf;

uint MainWindow;
int PlayerTarget, ActorSelectEN, ClickMarkerEN, ClickMarkerTimer;
Scenery* SceneryTarget = NULL;
string UpdateGame;
bool UpdateMusic;
string UpdateHost;
int PeerToHost, Connection;
int Cam;
ActorInstance* Me = NULL;
int SelectedCharacter;
RealmCrafter::SQuestLog* MyQuestLog;
int CurrentAreaID, FlashEN, FlashStart, FlashLength;
bool PVPEnabled;
bool Flashing;
bool QuitComplete = false;
bool LogoutComplete = false;
bool PlayerHasTouchedDown = true;
int ZonedMS, RandomImages, EULAAccepted, AlwaysShowEULA;
int GFXWidth, GFXHeight, GFXDepth, GFXAnisotropyLevel;
int GFXAntiAlias, GFXShadowDetail, GFXShaderQuality;
bool GFXWindowed;


int DeltaBuffer[6] = {35, 35, 35, 35, 35, 35};
float FPS, Delta;
int LastNetwork;
float Gravity = 0.2f;//0.0125;
int GPP;
int LootBagEN;
FILE* MainLog;
uint DefaultDepthShader = 0, DefaultAnimatedDepthShader = 0;
uint ShadowBlurH = 0, ShadowBlurV = 0;
int ShadowQuality = 0;

// GUI
IGUIManager* GUIManager = 0;
RealmCrafter::COptionsMenu* OptionsMenu = 0;

// Terrain
ITerrainManager* TerrainManager = 0;

// LT
LT::ITreeRenderer* TreeRenderer = 0;
LT::ITreeManager* TreeManager = 0;
LT::ITreeZone* TreeZone = 0;
NGin::Math::Vector3 TCamPos;

void* GetBBDXCommandData();
RealmCrafter::ICameraController* CameraController = NULL;
HMODULE SDKLib = NULL;
RealmCrafter::tStartRCSDKfn* StartRCSDK = NULL;
tSDKMainfn* SDKMain = NULL;
typedef RealmCrafter::ICameraController* (tGetCameraControllerfn)();
tGetCameraControllerfn* GetCameraController = NULL;
tResolutionChangedfn* ResolutionChanged = NULL;
tCreateControlEditorfn* CreateControlEditor = NULL;
tChangeEnvironmentfn* ChangeEnvironment = NULL;
tSetEnvTimefn* SetEnvTime = NULL;
tSetEnvDatefn* SetEnvDate = NULL;
tUpdateEnvironmentfn* UpdateEnvironment = NULL;
tSetEnvCameraUnderWaterfn* SetEnvCameraUnderWater = NULL;
tProcessEnvNetPacketfn* ProcessEnvNetPacket = NULL;
tEnvPlayWetFootstepfn* EnvPlayWetFootstep = NULL;



namespace RealmCrafter
{
    SGlobals* Globals = NULL;
}


ILabel* DL1 = 0;
ILabel* DL2 = 0;
ILabel* DL3 = 0;
ILabel* DL4 = 0;
ILabel* DL5 = 0;

uint DebugShader = 0;
void ConvertMeshDatabase();

bool ShaderLoadComplete = false;
volatile int IntroStatus = -1;

int ASyncShaderLoadProc(ASyncJobDesc* desc)
{
    // Setup GUI
    HMODULE GUILib = LoadLibraryA("NGUID3D9.dll");
    if(GUILib == NULL)
        RuntimeError("Could not load DLL: NGUID3D9.dll");

    typedef NGin::GUI::IGUIRenderer* (CreateCmd)(void* device, const char* effectPath);
    CreateCmd* CreateDirect3D9GUIRenderer = (CreateCmd*)GetProcAddress(GUILib, "?CreateDirect3D9GUIRenderer@@YAPAVIGUIRenderer@GUI@NGin@@PAXPBD@Z");
    if(CreateDirect3D9GUIRenderer == NULL)
        RuntimeError("Could not find rendering EntryPoint in NGUID3D9.dll");

    NGin::GUI::IGUIRenderer* GUIRenderer = CreateDirect3D9GUIRenderer(GetIDirect3DDevice9(), "Data\\Game Data\\Shaders\\Default\\GUI.fx");
    if(GUIRenderer == NULL)
        RuntimeError("Could not create GUI Renderer");

    // declare local manager first, so that the rendering thread doesn't access it
    NGin::GUI::IGUIManager* guiManager = CreateGUIManager(GUIRenderer, NGin::Math::Vector2(1024, 768));

    string Str = "Fonts";
    guiManager->FontDirectory("Data\\Fonts");
    guiManager->LoadAndAddSkin("Data\\Skins\\Default\\Theme.xml");
    guiManager->ImportProperties("Data\\Game Data\\Interface.xml");

    guiManager->SetProperties("SpellError");

    GUIManager = guiManager;

    IntroStatus = LS_LoadDefaultShaders;

    LoadDefaultShaders();

    IntroStatus = LS_LoadShadowShaders;

    DefaultDepthShader = LoadShader("Data\\Game Data\\Shaders\\Default\\DepthMap.fx");
    if(DefaultDepthShader == 0)
        RuntimeError("Could not load default effect: Data\\Game Data\\Shaders\\Default\\DepthMap.fx");

    DefaultAnimatedDepthShader = LoadShader("Data\\Game Data\\Shaders\\Default\\AnimatedDepthMap.fx");
    if(DefaultAnimatedDepthShader == 0)
        RuntimeError("Could not load default effect: Data\\Game Data\\Shaders\\Default\\AnimatedDepthMap.fx");

    ShadowBlurH = LoadShader("Data\\Game Data\\Shaders\\Default\\ShadowBlurH.fx");
    if(ShadowBlurH == 0)
        RuntimeError("Could not load default effect: Data\\Game Data\\Shaders\\Default\\ShadowBlurH.fx");

    ShadowBlurV = LoadShader("Data\\Game Data\\Shaders\\Default\\ShadowBlurV.fx");
    if(ShadowBlurV == 0)
        RuntimeError("Could not load default effect: Data\\Game Data\\Shaders\\Default\\ShadowBlurV.fx");

    IntroStatus = LS_LoadUserShaders;

    LoadShaders();

    IntroStatus = LS_LoadUserParameters;

    LoadEntityParameters();

    ShadowShader(DefaultDepthShader);
    ShadowBlurShader(ShadowBlurH, ShadowBlurV);
    //ShadowDistance(768.0f);
    //LightDistance(3000.0f);
    SetShadowMapSize(1024);
    ShadowLevel(0);

    ShadowDistance(150.0f);
    LightDistance(500.0f);

    IntroStatus = LS_LoadTerrainManager;

    // Setup Terrains
    TerrainManager = CreateTerrainManager(GetIDirect3DDevice9(), 
        "Data\\Game Data\\Shaders\\Default\\RCT.fx",
        "",
        "Data\\Game Data\\Shaders\\Default\\Grass.fx",
        "Data\\Game Data\\Shaders\\Default\\RCTDepth.fx");
    TerrainManager->LoadGrassTypes("Data\\Game Data\\GrassTypes.xml");
    TerrainManager->CollisionChanged()->AddEvent(&Terrain_CollisionChanged);
    TerrainManager->SetEditorMode(false);

    IntroStatus = LS_LoadTreeManager;

    // Setup trees
    TreeRenderer = LT::CreateDirect3D9TreeRenderer(bbdx2_GetIDirect3DDevice9(),
        "Data\\Game Data\\Shaders\\Default\\Trunk.fx",
        "Data\\Game Data\\Shaders\\Default\\Leaf.fx",
        "Data\\Game Data\\Shaders\\Default\\LOD.fx");
    if(TreeRenderer == 0)
        RuntimeError("Failed to start CD3D9TreeRenderer");

    TreeManager = LT::ITreeManager::CreateTreeManager(TreeRenderer);
    if(TreeManager == 0)
        RuntimeError("Failed to start TreeManager");

    if(TreeManager->CollisionChanged() == 0)
        RuntimeError("TreeManager->CollisionChanged() EventHandler was NULL");

    if(&TreeManager_CollisionChanged == 0)
        RuntimeError("Event: TreeManager_CollisionChanged was NULL!");

    TreeManager->CollisionChanged()->AddEvent(&TreeManager_CollisionChanged);

    TreeRenderer->SetSwayPower(0.8f);
    Vector2 Dir(1, 1);
    Dir.Normalize();
    TreeRenderer->SetSwayDirection(Dir);
    TreeRenderer->SetQualityLevel(1);

    uint TreeDir = ReadDir("Data\\Trees");
    if(TreeDir != 0)
    {
        while(true)
        {
            std::string File = NextFile(TreeDir);

            if(File.size() == 0)
                break;

            File = std::string("Data\\Trees\\") + File;

            if(FileType(File) == 1)
            {
                if(stringToLower(File.substr(File.size() - 3)) == std::string(".lt"))
                {
                    TreeManager->LoadTreeType(File.c_str());
                    DebugLog(std::string("Loaded Tree Type: ") + File);
                }
            }
        }

    }

    IntroStatus = LS_LoadSDK;

    // Call SDKMain and get it running
#pragma region SDK Main
    void* bbCommands = BlitzPlus::GetBBCommandData();
    void* bbdxCommands = GetBBDXCommandData();

    SDKMain(bbdxCommands, bbCommands, Cam, GUIManager);

    CameraController = GetCameraController();
#pragma endregion
    
    IntroStatus = LS_LoadOptions;

    // Create a new questlog for ourself
    MyQuestLog = new RealmCrafter::SQuestLog();
    Environment = new CEnvironmentManager();

    for(int i = 0; i < 36; ++i)
        ActionBarSlots[i] = 65535;

    SeedRnd(MilliSecs());
    MainLog = StartLog("Client Log", false);
    if(MainLog == 0)
        RuntimeError("Could not write Main Log");

    // JB: 2.50 - Create controls with default layout
    ControlLayout = new RealmCrafter::CControlLayout();
    CameraController->SetControlLayout(ControlLayout);

    ControlLayout->AddInstance("Movement", "Always Run", 199);
    ControlLayout->AddInstance("Combat", "Cycle Target", 56);
    ControlLayout->AddInstance("Movement", "Jump", 57);
    ControlLayout->AddInstance("Movement", "Talk To", 502);
    ControlLayout->AddInstance("Movement", "Select", 501);
    ControlLayout->AddInstance("Movement", "Move To", 29);
    ControlLayout->AddInstance("Combat", "Attack", 29);
    ControlLayout->AddInstance("Movement", "Fly Up", 201);
    ControlLayout->AddInstance("Movement", "Fly Down", 209);

    ControlLayout->Load("Data\\Game Data\\Controls.xml");
    LoadOptions(false);

    ConvertMeshDatabase();

    OptionsMenu = new RealmCrafter::COptionsMenu();
    OptionsMenu->Initialize();

    RealmCrafter::Globals->ServerSelector->Initialize();
    RealmCrafter::Globals->GameMenu->Initialize();
    RealmCrafter::Globals->GameMenu->LogoutTimerStart()->AddEvent(&GameMenu_LogoutTimerStart);
    RealmCrafter::Globals->GameMenu->OptionsClick()->AddEvent(&GameMenu_OptionsClick);
    RealmCrafter::Globals->GameMenu->Logout()->AddEvent(&GameMenu_Logout);
    RealmCrafter::Globals->GameMenu->Quit()->AddEvent(&GameMenu_Quit);


    // Load Gooey
    WriteLog(MainLog, "Loading Gooey");
    GY_Load();

    IntroStatus = LS_LoadGubbins;

    // Load gubbin templates
    GubbinTemplate::Load();

    // Load animation sets
    int Result = LoadAnimSets("Data\\Game Data\\Animations.dat");
    if(Result == -1)
        RuntimeError("Could not open Data\\Game Data\\Animations.dat!");
    WriteLog(MainLog, "Loaded animation sets");

    IntroStatus = LS_LoadClient;

    LoadOptions(true);
    RealmCrafter::LightFunctionList::LoadFunctions();

    // Fixed attributes needed by engine
    FILE* F = ReadFile("Data\\Game Data\\Fixed Attributes.dat");
    if(F == 0)
        RuntimeError("Could not open Data\\Game Data\\Fixed Attributes.dat!");
    RealmCrafter::Globals->HealthStat = ReadShort(F);
    RealmCrafter::Globals->EnergyStat = ReadShort(F);
    RealmCrafter::Globals->BreathStat = ReadShort(F);
    RealmCrafter::Globals->StrengthStat = ReadShort(F);
    RealmCrafter::Globals->SpeedStat = ReadShort(F);
    CloseFile(F);

    if(RealmCrafter::Globals->HealthStat == 65535) 
        RuntimeError("A valid Health attribute must be selected!");
    if(RealmCrafter::Globals->EnergyStat == 65535) 
        RealmCrafter::Globals->EnergyStat = -1;
    if(RealmCrafter::Globals->BreathStat == 65535) 
        RealmCrafter::Globals->BreathStat = -1;
    if(RealmCrafter::Globals->StrengthStat == 65535) 
        RuntimeError("A valid Strength attribute must be selected!");
    if(RealmCrafter::Globals->SpeedStat == 65535)
        RuntimeError("A valid Speed attribute must be selected!");


    WriteLog(MainLog, "Game started");

    LoadGame(false);

    return 1;
}

int ASyncShaderLoadSync(ASyncJobDesc* desc)
{
    ShaderLoadComplete = true;

    return 1;
}


int Main()
{
    //StartRemoteDebugging("localhost", 6543);

    // string constants
    bool langLoaded = LoadLanguage("Data\\Game Data\\Language.xml");
    if( !langLoaded )
        RuntimeError("Could not open Data\\Game Data\\Language.xml!");

#pragma region SDK Init
    RealmCrafter::SDK::RCCommandData* CommandData = new RealmCrafter::SDK::RCCommandData();
    CommandData->NGUIUpdateParameters = &BlitzPlus::NGUIUpdateParameters;
    CommandData->CanMovePlayer = &::CanMovePlayer;
    CommandData->ControlHit = &::ControlHit;
    CommandData->ControlDown = &::ControlDown;
    CommandData->ControlDownF = &::ControlDownF;
    CommandData->ControlName = &::ControlName;

    CommandData->RP_Update = &::RP_Update;
    CommandData->RP_ConfigLifespan = &::RP_ConfigLifespan;
    CommandData->RP_ConfigSpawnRate = &::RP_ConfigSpawnRate;
    CommandData->RP_ConfigVelocityMode = &::RP_ConfigVelocityMode;
    CommandData->RP_ConfigVelocityX = &::RP_ConfigVelocityX;
    CommandData->RP_ConfigVelocityY = &::RP_ConfigVelocityY;
    CommandData->RP_ConfigVelocityZ = &::RP_ConfigVelocityZ;
    CommandData->RP_ConfigVelocityRndX = &::RP_ConfigVelocityRndX;
    CommandData->RP_ConfigVelocityRndY = &::RP_ConfigVelocityRndY;
    CommandData->RP_ConfigVelocityRndZ = &::RP_ConfigVelocityRndZ;
    CommandData->RP_ConfigForceX = &::RP_ConfigForceX;
    CommandData->RP_ConfigForceY = &::RP_ConfigForceY;
    CommandData->RP_ConfigForceZ = &::RP_ConfigForceZ;
    CommandData->RP_ConfigForceModMode = &::RP_ConfigForceModMode;
    CommandData->RP_ConfigForceModX = &::RP_ConfigForceModX;
    CommandData->RP_ConfigForceModY = &::RP_ConfigForceModY;
    CommandData->RP_ConfigForceModZ = &::RP_ConfigForceModZ;
    CommandData->RP_ConfigInitialRed = &::RP_ConfigInitialRed;
    CommandData->RP_ConfigInitialGreen = &::RP_ConfigInitialGreen;
    CommandData->RP_ConfigInitialBlue = &::RP_ConfigInitialBlue;
    CommandData->RP_ConfigRedChange = &::RP_ConfigRedChange;
    CommandData->RP_ConfigGreenChange = &::RP_ConfigGreenChange;
    CommandData->RP_ConfigBlueChange = &::RP_ConfigBlueChange;
    CommandData->RP_ConfigInitialAlpha = &::RP_ConfigInitialAlpha;
    CommandData->RP_ConfigAlphaChange = &::RP_ConfigAlphaChange;
    CommandData->RP_ConfigFaceEntity = &::RP_ConfigFaceEntity;
    CommandData->RP_ConfigBlendMode = &::RP_ConfigBlendMode;
    CommandData->RP_ConfigMaxParticles = &::RP_ConfigMaxParticles;
    CommandData->RP_ConfigInitialScale = &::RP_ConfigInitialScale;
    CommandData->RP_ConfigScaleChange = &::RP_ConfigScaleChange;
    CommandData->RP_ConfigTexture = &::RP_ConfigTexture;
    CommandData->RP_ConfigTextureAnimSpeed = &::RP_ConfigTextureAnimSpeed;
    CommandData->RP_ConfigTextureRandomStartFrame = &::RP_ConfigTextureRandomStartFrame;
    CommandData->RP_ConfigShapeSphere = &::RP_ConfigShapeSphere;
    CommandData->RP_ConfigShapeCylinder = &::RP_ConfigShapeCylinder;
    CommandData->RP_ConfigShapeBox = &::RP_ConfigShapeBox;
    CommandData->RP_CreateEmitterConfig = &::RP_CreateEmitterConfig;
    CommandData->RP_CopyEmitterConfig = &::RP_CopyEmitterConfig;
    CommandData->RP_FreeEmitterConfig = &::RP_FreeEmitterConfig;
    CommandData->RP_LoadEmitterConfig = &::RP_LoadEmitterConfig;
    CommandData->RP_CreateEmitter = &::RP_CreateEmitter;
    CommandData->RP_EmitterActiveParticles = &::RP_EmitterActiveParticles;
    CommandData->RP_EnableEmitter = &::RP_EnableEmitter;
    CommandData->RP_DisableEmitter = &::RP_DisableEmitter;
    CommandData->RP_HideEmitter = &::RP_HideEmitter;
    CommandData->RP_ShowEmitter = &::RP_ShowEmitter;
    CommandData->RP_ScaleEmitter = &::RP_ScaleEmitter;
    CommandData->RP_KillEmitter = &::RP_KillEmitter;
    CommandData->RP_FreeEmitter = &::RP_FreeEmitter;
    CommandData->RP_Clear = &::RP_Clear;
    CommandData->RemoveUnderwaterParticles = &::RemoveUnderwaterParticles;
    CommandData->ScreenFlash = &::ScreenFlash;

    CommandData->InventorySwap = &RealmCrafter::InventorySwap;
    CommandData->InventoryDrop = &RealmCrafter::InventoryDrop;
    CommandData->UseItem = &RealmCrafter::UseItem;
    CommandData->GetItemPathFromID = &RealmCrafter::GetItemPathFromID;
    CommandData->GetSpellPathFromID = &RealmCrafter::GetSpellPathFromID;
    CommandData->GetSpellName = &RealmCrafter::GetSpellName;
    CommandData->GetItemName = &RealmCrafter::GetItemName;
    CommandData->GetItemTypeString = &RealmCrafter::GetItemTypeString;
    CommandData->GetItemTakesDamage = &RealmCrafter::GetItemTakesDamage;
    CommandData->GetItemHealth = &RealmCrafter::GetItemHealth;
    CommandData->GetItemValue = &RealmCrafter::GetItemValue;
    CommandData->GetItemMass = &RealmCrafter::GetItemMass;
    CommandData->GetItemStackable = &RealmCrafter::GetItemStackable;
    CommandData->GetItemType = &RealmCrafter::GetItemType;
    CommandData->GetItemWeaponDamage = &RealmCrafter::GetItemWeaponDamage;
    CommandData->GetItemWeaponDamageType = &RealmCrafter::GetItemWeaponDamageType;
    CommandData->GetItemWeaponType = &RealmCrafter::GetItemWeaponType;
    CommandData->GetItemArmourLevel = &RealmCrafter::GetItemArmourLevel;
    CommandData->GetItemEatEffects = &RealmCrafter::GetItemEatEffects;
    CommandData->GetItemExclusiveRace = &RealmCrafter::GetItemExclusiveRace;
    CommandData->GetItemExclusiveClass = &RealmCrafter::GetItemExclusiveClass;
    CommandData->GetPlayerIsExclusiveRace = &RealmCrafter::GetPlayerIsExclusiveRace;
    CommandData->GetPlayerIsExclusiveClass = &RealmCrafter::GetPlayerIsExclusiveClass;
    CommandData->GetSpellDescription = &RealmCrafter::GetSpellDescription;
    CommandData->GetSpellNameFromID = &RealmCrafter::GetSpellNameFromID;

    RealmCrafter::Globals = new RealmCrafter::SGlobals();
    RealmCrafter::Globals->AlwaysRun = true;
    RealmCrafter::Globals->SpeedStat = 0;
    RealmCrafter::Globals->StrengthStat = 0;
    RealmCrafter::Globals->HealthStat = 0;
    RealmCrafter::Globals->EnergyStat = 0;
    RealmCrafter::Globals->BreathStat = 0;
    RealmCrafter::Globals->AttackTarget = false;
    RealmCrafter::Globals->LastZoneLoad = 0;
    RealmCrafter::Globals->GameName = "GameName";
    RealmCrafter::Globals->ServerHost = "ServerHost";
    RealmCrafter::Globals->ServerPort = 0;
    RealmCrafter::Globals->AreaName = "AreaName";
    RealmCrafter::Globals->QuitActive = false;
    RealmCrafter::Globals->Money1 = "Money1";
    RealmCrafter::Globals->Money2 = "Money2";
    RealmCrafter::Globals->Money3 = "Money3";
    RealmCrafter::Globals->Money4 = "Money4";
    RealmCrafter::Globals->Money2x = 0;
    RealmCrafter::Globals->Money3x = 0;
    RealmCrafter::Globals->Money4x = 0;
    RealmCrafter::Globals->LastAttack = 0;
    RealmCrafter::Globals->CombatDelay = 0;
    RealmCrafter::Globals->FogR = 0;
    RealmCrafter::Globals->FogG = 0;
    RealmCrafter::Globals->FogB = 0;
    RealmCrafter::Globals->FogNear = 500.0f;
    RealmCrafter::Globals->FogFar = 1000.0f;
    RealmCrafter::Globals->DefaultVolume = 1.0f;
    RealmCrafter::Globals->ControlHostManager = NULL;
    RealmCrafter::Globals->TimerManager = CreateTimerManager();
    RealmCrafter::Globals->RequireMemorise = false;
    RealmCrafter::Globals->CurrentMouseItem = NULL;
    RealmCrafter::Globals->CurrentMouseToolTip = NULL;
    RealmCrafter::Globals->Attributes = new RealmCrafter::SAttributeDefinition();


    SDKLib = LoadLibraryA("SDK.dll");
    if(SDKLib == NULL)
        RuntimeError("Could not load DLL: SDK.dll");

    StartRCSDK = (RealmCrafter::tStartRCSDKfn*)GetProcAddress(SDKLib, "StartRCSDK");
    if(StartRCSDK == NULL)
        RuntimeError("Could not find SDK InitPoint: StartRCSDK(ptr)");

    SDKMain = (tSDKMainfn*)GetProcAddress(SDKLib, "SDKMain");
    if(SDKMain == NULL)
        RuntimeError("Could not find SDK EntryPoint: SDKMain(ptr, ptr, uint)");

    ResolutionChanged = (tResolutionChangedfn*)GetProcAddress(SDKLib, "ResolutionChanged");
    if(ResolutionChanged == NULL)
        RuntimeError("Could not find SDK Export: ResolutionChanged()");

    GetCameraController = (tGetCameraControllerfn*)GetProcAddress(SDKLib, "GetCameraController");
    if(GetCameraController == NULL)
        RuntimeError("Could not find SDK Export: GetCameraController()");

    CreateControlEditor = (tCreateControlEditorfn*)GetProcAddress(SDKLib, "CreateControlEditor");
    if(CreateControlEditor == NULL)
        RuntimeError("Could not find SDK Export: CreateControlEditor()");

    ChangeEnvironment = (tChangeEnvironmentfn*)GetProcAddress(SDKLib, "ChangeEnvironment");
    if(ChangeEnvironment == NULL)
        RuntimeError("Could not find SDK Export: ChangeEnvironment()");

    SetEnvTime = (tSetEnvTimefn*)GetProcAddress(SDKLib, "SetEnvTime");
    if(SetEnvTime == NULL)
        RuntimeError("Could not find SDK Export: SetEnvTime()");

    SetEnvDate = (tSetEnvDatefn*)GetProcAddress(SDKLib, "SetEnvDate");
    if(SetEnvDate == NULL)
        RuntimeError("Could not find SDK Export: SetEnvDate()");

    UpdateEnvironment = (tUpdateEnvironmentfn*)GetProcAddress(SDKLib, "UpdateEnvironment");
    if(UpdateEnvironment == NULL)
        RuntimeError("Could not find SDK Export: UpdateEnvironment()");

    SetEnvCameraUnderWater = (tSetEnvCameraUnderWaterfn*)GetProcAddress(SDKLib, "SetEnvCameraUnderWater");
    if(SetEnvCameraUnderWater == NULL)
        RuntimeError("Could not find SDK Export: SetEnvCameraUnderWater()");

    ProcessEnvNetPacket = (tProcessEnvNetPacketfn*)GetProcAddress(SDKLib, "ProcessEnvNetPacket");
    if(ProcessEnvNetPacket == NULL)
        RuntimeError("Could not find SDK Export: ProcessEnvNetProperty()");

    EnvPlayWetFootstep = (tEnvPlayWetFootstepfn*)GetProcAddress(SDKLib, "EnvPlayWetFootstep");
    if(EnvPlayWetFootstep == NULL)
        RuntimeError("Could not find SDK Export: EnvPlayWetFootstep()");

    

    if(StartRCSDK(CommandData, RealmCrafter::Globals, _MSC_VER) == -1)
    {
        RuntimeError(std::string("Could not load SDK.DLL: StartSDK failed _MSC_VER comparison, should have been '") + toString(_MSC_VER) + "'");
    }
#pragma endregion SDK Init


#pragma region Setup Graphics
    // Create main game window (keep it hidden for now)
    DebugLog("Dimension...");
    int X = BBClientWidth(BBDesktop()) / 2 - 400;
    int Y = BBClientHeight(BBDesktop()) / 2 - 300;
    uint Desk = BBDesktop();

    MainWindow = BBCreateWindow(RealmCrafter::Globals->GameName, X, Y, 800, 600, Desk, 0);
    Init(BBQueryObject(MainWindow, 1), 0);
    HidePointer();

    // Hide window for splash
    ShowWindow((HWND)BBQueryObject(MainWindow, 1), SW_HIDE);

    // Create splash
    CreateAndShowSplash(RealmCrafter::Globals->GameName.c_str());

    // Play menu music
    uint SndC_Music = PlayMusic("Data\\Music\\Menu.ogg");
    if(SndC_Music == 0)
        SndC_Music = PlayMusic("Data\\Music\\Menu.mid");
    if(SndC_Music == 0)
        SndC_Music = PlayMusic("Data\\Music\\Menu.mod");

    // Setup the graphics device
    //SetGUIEffectPath("Data\\Game Data\\Shaders\\Default\\GUI.fx");
    Graphics3D(1024, 768, 32, 2, 0, 0, "Data\\DefaultTex.png");

    // We need this for the SDK and intro
    Perf = bbdx2_ASyncGetLib();

    // Set default slope restriction to 45 degrees
    bbdx2_SetSlopeRestriction(0.707f);

    // Need this or it'll explode
    Cam = CreateCamera();
    CameraFogMode(Cam, 1);

    // Set default lighting
    AmbientLight(0, 0, 0);

    // All the rendering callbacks.
    SetRenderGUICallback(0, &RenderGUI);
    SetDeviceLostCallback(0, &DeviceLost);
    SetDeviceResetCallback(0, &DeviceReset);
    SetRenderSolidCallbackRT(0, &RenderSolid);
    SetRenderShadowDepthCallback(0, &RenderDepth);
    SetRenderShadowDepthVPCallback(0, &RenderDepthVP);

    int lastStatus = IntroStatus;

    // Start our async intro loader
    Perf->bbdx2_ASyncInsertJob( 1, &ASyncShaderLoadProc, &ASyncShaderLoadSync, NULL, 1 );
    while( !ShaderLoadComplete )
    {
        BBWaitEvent();

		// DV: Can't update world while the loader thread is running since
		// PhysX can't init during update. Instead call the internal job
		// sync (which is the desired result).
		Perf->bbdx2_ASyncJobSync();

        if( lastStatus != IntroStatus )
        {
            lastStatus = IntroStatus;

            if( IntroStatus != -1 )
                UpdateSplashStatus( LanguageString[IntroStatus].c_str() );
            else
                UpdateSplashStatus( "" );
        }
    }

    // Splash done
    CloseSplash();
    ShowWindow((HWND)BBQueryObject(MainWindow, 1), SW_SHOW);

    {
        //MessageBoxA(NULL, "2", "", 0);
        //exit(0);
    }

#pragma endregion Setup Graphics

    // Stop the menu music and return
    if(SndC_Music > 0 && ChannelPlaying(SndC_Music))
        StopChannel(SndC_Music);
    WriteLog(MainLog, "Menu finished - starting actual game");

    // Apply graphics settings
    OptionsMenu->Apply();

    // Onscreen Debug Labels (for testing only)
    DL1 = GUIManager->CreateLabel("DL1", Math::Vector2(100, 30) / GUIManager->GetResolution(), Math::Vector2(400, 16) / GUIManager->GetResolution());
    DL2 = GUIManager->CreateLabel("DL2", Math::Vector2(100, 50) / GUIManager->GetResolution(), Math::Vector2(400, 16) / GUIManager->GetResolution());
    DL3 = GUIManager->CreateLabel("DL3", Math::Vector2(100, 70) / GUIManager->GetResolution(), Math::Vector2(400, 16) / GUIManager->GetResolution());
    DL4 = GUIManager->CreateLabel("DL4", Math::Vector2(100, 90) / GUIManager->GetResolution(), Math::Vector2(400, 16) / GUIManager->GetResolution());
    DL5 = GUIManager->CreateLabel("DL5", Math::Vector2(100, 110) / GUIManager->GetResolution(), Math::Vector2(400, 16) / GUIManager->GetResolution());
    
    //
    DL1->ForeColor = Math::Color(0, 255, 0);
    DL2->ForeColor = Math::Color(0, 255, 0);
    DL3->ForeColor = Math::Color(0, 255, 0);
    DL4->ForeColor = Math::Color(0, 255, 0);
    DL5->ForeColor = Math::Color(0, 255, 0);
    

    DL1->Text = "";//"RCP (2.30) Development Build";
    DL2->Text = "";
    DL3->Text = "";
    DL4->Text = "";
    DL5->Text = "";

    
    for(int i = 0; i < 32; ++i)
    {
        ILabel* ALbl = GUIManager->CreateLabel("dfsdf", Math::Vector2(100, (i * 20) + 120) / GUIManager->GetResolution(), Math::Vector2(0, 0));
        ALbl->ForeColor = Math::Color(0, 255, 0);
        ALbl->Text = "";

        ActorDBLs.push_back(ALbl);
    }

    // Main Menu -------------------------------------------------------------------------------------------------------------------------
//	LoadGame(false);

    do
    {
        if(Me != NULL)
        {
            UnloadArea();
            SafeFreeActorInstance(Me);
            Me = NULL;
        }
        LogoutComplete = false;
        RealmCrafter::Globals->AreaName = "";
        CurrentAreaID = -1;



// 
// 		float W = GUIManager->GetResolution().X;
// 		float H = GUIManager->GetResolution().Y;
// 
// 		ITabControl* ChatTabControl = GUIManager->CreateTabControl("ZZ", Vector2(0.1f, 0.1f), Vector2(200 / W, 100/H));
// 		ChatTabControl->AddTab("Main", 0.1f);
// 
// 		IPictureBox* ChatTabPanel = ChatTabControl->TabPanel(0);
// 
// 		ILabel* ChatLabel = GUIManager->CreateLabel("ZZ", Vector2(12/W, 12/H), Vector2(160 / W, 0));
// 		ChatLabel->Parent = ChatTabPanel;
// 		ChatLabel->Multiline = true;
// 
// 		ChatLabel->Text = "Hello\nThisgggggggggggggggggggggggggggggggggggggggggggggggggggg\nis\ncomplex\ntext\nlol.";
// 
// 		IScrollBar* ChatScroll = GUIManager->CreateScrollBar("ZZ", Vector2(165/W, 12/H), Vector2(30/W, ChatTabPanel->Size.Y - (24 / H)), ScrollOrientation::VerticalScroll);
// 		ChatScroll->Parent = ChatTabPanel;

        //RealmCrafter::CChatBox* Chatbox = new RealmCrafter::CChatBox(GUIManager);
        //Chatbox->Initialize();

// 		RealmCrafter::Globals->ChatBox->SetVisible(true);
// 
// 		RealmCrafter::Globals->ChatBox->AddTab(0, string("Tab1"), 50);
// 		RealmCrafter::Globals->ChatBox->AddTab(1, string("Tab2"), 70);
// 		RealmCrafter::Globals->ChatBox->AddTab(200, string("Combat"), 50);
// 
// 		for(int i = 0; i < 20; ++i)
// 		{
// 			RealmCrafter::Globals->ChatBox->Output(2, toString(i), Color(1.0f, 1.0f, 1.0f));
// 		}
// 		//RealmCrafter::Globals->ChatBox->Output(2, std::string("Get up, come on get down with the sickness. You mother get up, come on get down with the sickness. You fucker get up come on get down with the sickness. Madness is the gift that has been given to me."), Color(1.0f, 0.0f, 1.0f));
// 		RealmCrafter::Globals->ChatBox->Output(200, std::string("Bens' Mum hit you for 1 point."), Color(1.0f, 0.0f, 0.0f));
// 		RealmCrafter::Globals->ChatBox->Output(200, std::string("You hit Bens' Mum for 100 points."), Color(0.0f, 1.0f, 0.0f));
// 		RealmCrafter::Globals->ChatBox->Output(200, std::string("Bens' Mum hit you for 2 points."), Color(1.0f, 0.0f, 0.0f));
// 		RealmCrafter::Globals->ChatBox->Output(200, std::string("Taken Critical Damage!"), Color(1.0f, 0.0f, 0.0f));
// 		RealmCrafter::Globals->ChatBox->Output(200, std::string("You hit Bens' Mum for 110 points."), Color(0.0f, 1.0f, 0.0f));
// 		RealmCrafter::Globals->ChatBox->Output(200, std::string("You have defeated Bens' Mum, she will do the dishes."), Color(0.0f, 1.0f, 0.0f));
// 
// 		for(int i = 0; i < 2000; ++i)
// 		{
// 			float R = Floor(Rnd(0, 1) * 3.0f) / 2.0f;
// 			float G = Floor(Rnd(0, 1) * 3.0f) / 2.0f;
// 			float B = Floor(Rnd(0, 1) * 3.0f) / 2.0f;
// 
// 			//RealmCrafter::Globals->ChatBox->Output(0, std::string("<Simmsman> WHAT SAY YOU?"), Color(R, G, B));
// 			RealmCrafter::Globals->ChatBox->Output(0, toString(i), Color(R, G, B));
// 		}
// 
// 		RealmCrafter::Globals->ChatBox->Output(1, std::string("<Simmsman> ADD LUA?\n<Simmsman> MORE COMPLAINT!\n<Simmsman> WHAT SAY ME?\n"), Color(1.0f, 1.0f, 1.0f));
// 		RealmCrafter::Globals->ChatBox->Output(1, std::string("FFFFFF"), Color(1.0f, 1.0f, 1.0f));
// 
// 		//RealmCrafter::Globals->ChatBox->RemoveTab(1);
// 
// // 		Graphics3D(800, 600, 32, 2, 0, 0, "");
// // 		GUIManager->SetProperties("MainMenu");
// // 		GUIManager->SetProperties("SpellError");
// // 		GUIManager->SetProperties("TextInput");
// // 		GUIManager->SetProperties("ScriptDialog");
// // 		GUIManager->SetProperties("BubbleOutput");
// // 		GUIManager->SetProperties("QuitProgress");
// // 		GUIManager->SetProperties("ChatHistory");
// // 		GUIManager->SetProperties("AttributeBars");
// // 		GUIManager->SetProperties("MapWindow");
// // 		GUIManager->SetProperties("ChatEntry");
// // 		GUIManager->SetProperties("Compass");
// // 		GUIManager->SetProperties("Radar");
// // 		GUIManager->SetProperties("EffectIconSlots");
// // 		GUIManager->SetProperties("ActionBar");
// // 		GUIManager->SetProperties("PartyWindow");
// // 		GUIManager->SetProperties("SpellsWindow");
// // 		GUIManager->SetProperties("SpellRemove");
// // 		GUIManager->SetProperties("QuestLog");
// // 		GUIManager->SetProperties("CharStats");
// // 		GUIManager->SetProperties("HelpWindow");
// // 		GUIManager->SetProperties("MouseSlot");
// // 		GUIManager->SetProperties("Inventory");
// // 		GUIManager->SetProperties("Trading");
// // 		GUIManager->SetProperties("GameMenu");
// // 		RealmCrafter::Globals->ChatBox->ResolutionChange(Vector2(800, 600));
// 		//RealmCrafter::Globals->ChatBox->Reset();


        HideInterface();
        RunMenu();
        LoadGame(true);
        Connect();
        ShowDefaultInterface();

        // Main loop -------------------------------------------------------------------------------------------------------------------------
        int DeltaTime = MilliSecs();
        int LastNetwork = MilliSecs();
        int DeltaBufferIndex = 0;
        int Cnt  = 0 ;

        uint LastFrames[] = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0};



        do
        {
            uint FrameStart = timeGetTime();

            BeginDebugFrame();
            StartDebugTimer("Frame Time");

            // Delta timing bits
            DeltaTime = MilliSecs() - DeltaTime;
            DeltaBuffer[DeltaBufferIndex] = DeltaTime;
            DeltaBufferIndex = DeltaBufferIndex + 1;
            if(DeltaBufferIndex >= DeltaFrames)
                DeltaBufferIndex = 0;

            // Take average of last N frames to get delta time coefficient
            float Time = 0.0f;
            for(int i = 0; i < DeltaFrames; ++i)
                Time = Time + DeltaBuffer[i];
            
            Time = Time / ((float)DeltaFrames);
            FPS = 1000.0f / Time;
            Delta = BaseFramerate / FPS;
            DeltaTime = MilliSecs();
            if(Delta > 3.5f)
                Delta = 3.5f; // Don't let delta go too OTT
            RealmCrafter::Globals->DeltaTime = Delta;

            RealmCrafter::Globals->TimerManager->Update();

            // Update game
            StartDebugTimer("Combat");
            UpdateCombat();                 // Attack my target etc.
            StopDebugTimer();
            
            StartDebugTimer("ChatBubbles");
            UpdateChatBubbles();
            StopDebugTimer();

            StartDebugTimer("NameTags");
            UpdateNametags();
            StopDebugTimer();

            StartDebugTimer("ActorInstance");
            UpdateActorInstances();         // Actor movement etc.
            StopDebugTimer();

            StartDebugTimer("Interface");
            UpdateInterface();              // Interface
            StopDebugTimer();
            
            StartDebugTimer("Environment");
            Environment->Update(Delta);
            StopDebugTimer();

            StartDebugTimer("Projectiles");
            UpdateProjectiles();            // Projectile instances
            StopDebugTimer();
            
            StartDebugTimer("Particles");
            SeedRnd(MilliSecs());
            RP_Update(Delta);               // RottParticles
            StopDebugTimer();

            StartDebugTimer("AnimatedScenery");
            UpdateAnimatedScenery();        // Animated scenery
            StopDebugTimer();

            StartDebugTimer("ScreenFlash");
            if(Flashing == true)
                UpdateScreenFlash();        // Screen flash effect
            StopDebugTimer();

            //if(DamageInfoStyle == 3)
            //UpdateFloatingNumbers();    // Combat damage information

            //string Act = "Actors: ";
            //Act.append(toString(ActorInstance::ActorInstanceList.Count()));
            //Act.append("   ");
            //foreachc(AIt, ActorInstance, ActorInstanceList)
            //{
            //	ActorInstance* AI = *AIt;

            //	Act.append("(");
            //	Act.append(AI->Name);
            //	Act.append(",");
            //	Act.append((AI->CollisionEN > 0) ? toString(EntityX(AI->CollisionEN)) : "N");
            //	Act.append(",");
            //	Act.append((AI->CollisionEN > 0) ? toString(EntityY(AI->CollisionEN)) : "N");
            //	Act.append(",");
            //	Act.append((AI->CollisionEN > 0) ? toString(EntityZ(AI->CollisionEN)) : "N");
            //	Act.append(")  ");
            //	

            //	nextc(AIt, ActorInstance, ActorInstanceList);
            //}

            //DL2->Text = Act;
            //DL2->Text = string("Camera: ") + NGin::Math::Vector3(EntityX(Cam, true), EntityY(Cam, true), EntityZ(Cam, true)).ToString();

            RealmCrafter::LightFunctionList::Update();
            foreachc(LIt, ZoneLight, ZoneLightList)
            {
                ZoneLight* L = (*LIt);

                if(L->Function != 0 && RealmCrafter::LightFunctionList::Verify(L->Function))
                {
                    SetPLightColor(L->Handle,
                        L->Function->CurrentColor.R * 255.0f,
                        L->Function->CurrentColor.G * 255.0f,
                        L->Function->CurrentColor.B * 255.0f);
                    SetLightRadius(L->Handle, L->Function->CurrentRadius);
                }

                nextc(LIt, ZoneLight, ZoneLightList);
            }

            StartDebugTimer("Terrain (Upd)");
            if(Me == NULL)
                TerrainManager->Update(NGin::Math::Vector3(EntityX(Cam, true), EntityY(Cam, true), EntityZ(Cam, true)), true);
            else
                TerrainManager->Update(NGin::Math::Vector3(Me->Position.X + (Me->Position.SectorX * RealmCrafter::SectorVector::SectorSize),
                    EntityY(Me->CollisionEN, true),
                    Me->Position.Z + (Me->Position.SectorZ * RealmCrafter::SectorVector::SectorSize)), true);

            StopDebugTimer();

            StartDebugTimer("Tree (Upd)");
            Math::Vector3 TreeUpdatePosition;
            if(Me == NULL)
                TreeUpdatePosition = NGin::Math::Vector3(EntityX(Cam, true), EntityY(Cam, true), EntityZ(Cam, true));
            else
                TreeUpdatePosition = NGin::Math::Vector3(Me->Position.X + (Me->Position.SectorX * RealmCrafter::SectorVector::SectorSize),
                EntityY(Me->CollisionEN, true),
                Me->Position.Z + (Me->Position.SectorZ * RealmCrafter::SectorVector::SectorSize));
            TreeManager->Update(reinterpret_cast<const float*>(&TreeUpdatePosition));
            StopDebugTimer();

#if 0
            // Debug labels
            if(Me != NULL)
            {
                DL1->Text = string("Actor Position: ") + Me->Position.ToString();
                DL2->Text = string("Actor Destination: ") + Me->Destination.ToString();
                DL3->Text = string("Ent Position: ") + Math::Vector3(EntityX(Me->CollisionEN, true), EntityY(Me->CollisionEN, true), EntityZ(Me->CollisionEN, true)).ToString();
            }else
            {
                DL1->Text = string("No Actor");
                DL2->Text = string("No Actor");
            }

            if(Cam != 0)
                DL4->Text = string("Camera Position: ") + Math::Vector3(EntityX(Cam, true), EntityY(Cam, true), EntityZ(Cam, true)).ToString();

            if(PlayerTarget != NULL)
            {
                DL5->Text = string("PlayerTarget: ") + ((ActorInstance*)PlayerTarget)->Name;
            }else
            {
                if(SceneryTarget != NULL)
                {
                    DL5->Text = string("SceneryTarget: ") + std::toString(SceneryTarget->MeshID);
                }else
                {
                    DL5->Text = string("Targeting: NONE");
                }
            }
#endif

            // Update *my* actor inventory
            if(PlayerInventory != NULL && Me != NULL && Me->Inventory != NULL)
            {
                for(int i = 0; i < 50; ++i)
                {
                    if(Me->Inventory->Items[i] == NULL)
                        Me->InventoryHandle.Items[i] = 65535;
                    else
                        Me->InventoryHandle.Items[i] = Me->Inventory->Items[i]->Item->ID;
                    Me->InventoryHandle.Amounts[i] = Me->Inventory->Amounts[i];
                }

                PlayerInventory->Update(&Me->InventoryHandle, BlitzPlus::NGUIUpdateParameters.MousePosition / GUIManager->GetResolution());
            }

            // Update player spells
            if(PlayerSpells != NULL && Me != NULL)
            {
                RealmCrafter::SSpells SpellsHandle;
                for(int i = 0; i < 10; ++i)
                {
                    if(Me->MemorisedSpells[i] == 5000)
                    {
                        SpellsHandle.MemSpells[i] = 65535;
                    }else
                    {
                        SpellsHandle.MemSpells[i] = Me->KnownSpells[Me->MemorisedSpells[i]];
                        SpellsHandle.MemLevels[i] = Me->SpellLevels[Me->MemorisedSpells[i]];
                    }
                }

                for(int i = 0; i < 1000; ++i)
                {
                    SpellsHandle.Spells[i] = Me->KnownSpells[KnownSpellSort[i]];
                    SpellsHandle.Levels[i] = Me->SpellLevels[KnownSpellSort[i]];
                }

                PlayerSpells->Update(&SpellsHandle, BlitzPlus::NGUIUpdateParameters.MousePosition / GUIManager->GetResolution());
            }

            if(PlayerActionBar != NULL && Me != NULL && Me->Inventory != NULL)
            {
                RealmCrafter::SActionBar ActionBar;

                for(int i = 0; i < 36; ++i)
                {
                    if(ActionBarSlots[i] < 0) // Spell
                    {
                        Spell* Sp = 0;

                        string StrO = "";

                        if(RealmCrafter::Globals->RequireMemorise)
                        {
                            int Num = ActionBarSlots[i] + 10;
                            
                            ActionBar.Spells[i] = Me->KnownSpells[Me->MemorisedSpells[Num]];
                            ActionBar.RechargeTimes[i] = (int)((((float)Me->SpellCharge[Num]) /
                                ((float)SpellsList[Me->KnownSpells[Me->MemorisedSpells[Num]]]->RechargeTime)) * 100.0f);

                        }else
                        {
                            int Num = ActionBarSlots[i] + 1000;

                            ActionBar.Spells[i] = Me->KnownSpells[Num];
                            ActionBar.RechargeTimes[i] = (int)((((float)Me->SpellCharge[Num]) /
                                ((float)SpellsList[Me->KnownSpells[Num]]->RechargeTime)) * 100.0f);
                        }

                    }else if(ActionBarSlots[i] < 65535) // Item
                    {
                        ActionBar.Items[i] = ActionBarSlots[i];
                        ActionBar.ItemAmounts[i] = 0;
                    }
                }

                PlayerActionBar->Update(&ActionBar, BlitzPlus::NGUIUpdateParameters.MousePosition / GUIManager->GetResolution());
            }

            if(RealmCrafter::Globals->CurrentMouseItem != NULL)
            {
                RealmCrafter::Globals->CurrentMouseItem->GetDisplay()->Location = BlitzPlus::NGUIUpdateParameters.MousePosition / GUIManager->GetResolution();
                RealmCrafter::Globals->CurrentMouseItem->GetDisplay()->Visible = true;
                RealmCrafter::Globals->CurrentMouseItem->GetDisplay()->BringToFront();

                // Tooltips and mouse items cannot co-exist
                if(RealmCrafter::Globals->CurrentMouseToolTip != NULL)
                {
                    delete RealmCrafter::Globals->CurrentMouseToolTip;
                    RealmCrafter::Globals->CurrentMouseToolTip = NULL;
                }
            }

            if(RealmCrafter::Globals->CurrentMouseToolTip != NULL)
            {
                RealmCrafter::IItemButton* MouseOver = RealmCrafter::Globals->CurrentMouseToolTip->GetParent();
                Vector2 MousePosition = BlitzPlus::NGUIUpdateParameters.MousePosition / GUIManager->GetResolution();
                if(MousePosition.X < MouseOver->GlobalLocation().X
                    || MousePosition.Y < MouseOver->GlobalLocation().Y
                    || MousePosition.X > MouseOver->GlobalLocation().X + MouseOver->Size.X
                    || MousePosition.Y > MouseOver->GlobalLocation().Y + MouseOver->Size.Y)
                {
                    delete RealmCrafter::Globals->CurrentMouseToolTip;
                    RealmCrafter::Globals->CurrentMouseToolTip = NULL;
                }else
                {
                    Vector2 Size = RealmCrafter::Globals->CurrentMouseToolTip->GetDisplay()->Size;
                    Vector2 Location = BlitzPlus::NGUIUpdateParameters.MousePosition / GUIManager->GetResolution();

                    if(Location.X + Size.X > 1.0f)
                        Location.X -= Size.X;
                    if(Location.Y + Size.Y > 1.0f)
                        Location.Y -= Size.Y;

                    RealmCrafter::Globals->CurrentMouseToolTip->GetDisplay()->Location = Location;
                    RealmCrafter::Globals->CurrentMouseToolTip->GetDisplay()->Visible = true;
                    RealmCrafter::Globals->CurrentMouseToolTip->GetDisplay()->BringToFront();
                }
            }


            StartDebugTimer("GUI (Upd)");
            GUIManager->Update(&BlitzPlus::NGUIUpdateParameters);
            StopDebugTimer();



            StartDebugTimer("Camera");
            UpdateCamera();                 // Camera movement
            StopDebugTimer();


            StartDebugTimer("World (Udp)");
            UpdateWorld();                  // Animation, collision etc.
            StopDebugTimer();


            StartDebugTimer("Scene Render");
            if(LastSectorChange < 1)
                bbdx2_AllowRenderFrame(0);
            RenderWorld();
            ++LastSectorChange;
            StopDebugTimer();

            StartDebugTimer("SoundZones");
            UpdateSoundZones();             // Sound zones
            StopDebugTimer();

            StartDebugTimer("Swimming");
            UpdateSwimmingActorInstances(); // Updates actors who are underwater, in case UpdateWorld's collision has moved them above the surface
            StopDebugTimer();

            StartDebugTimer("Riding");
            UpdateRidingActorInstances();   // Updates actors on horseback who don't have their own collision
            StopDebugTimer();

            // Update network1
            StartDebugTimer("Net (Ext)");
            RCE_Update();
            RCE_CreateMessages();
            StopDebugTimer();

            StartDebugTimer("Net (Int)");
            UpdateNetwork();
            StopDebugTimer();

            StopDebugTimer();
            EndDebugFrame();

            uint FrameTime = timeGetTime() - FrameStart;
            float FramesTotal = 0.0f;

            for(int i = 0; i < 9; ++i)
            {
                LastFrames[i] = LastFrames[i + 1];
                FramesTotal += (float)LastFrames[i];
            }

            LastFrames[9] = FrameTime;
            FramesTotal += (float)FrameTime;

            FramesTotal *= 0.1f;
            //AppTitle(std::toString(FramesTotal));


            Perf->DO_Update();

        }while(LogoutComplete != true);

        // Close client
        RCE_Disconnect(Connection);
        //RN_CreateMessages();
    }while(QuitComplete != true);

    exit(0);
    End3DGraphics();
    WriteLog(MainLog, "Client closed");
    exit(0);
}

void RenderDepth(const float* lightMatrix)
{
    if(TerrainManager != 0)// && GFXShadowDetail >= 3)
    {
        Math::Vector3 Offset;

        if(Me != NULL)
        {
            Offset.X -= Me->Position.SectorX * RealmCrafter::SectorVector::SectorSize;
            Offset.Z -= Me->Position.SectorZ * RealmCrafter::SectorVector::SectorSize;
        }

        StartDebugTimer("Terrain (Rnd, Dpt)");
        TerrainManager->RenderDepth(lightMatrix, Offset);
        StopDebugTimer();
    }
}

void RenderDepthVP(const float* lightView, const float* lightProjection)
{
    if(TreeManager != 0 && GFXShadowDetail >= 4)
    {
        StartDebugTimer("Tree (Rnd, Dpt)");

        Math::Vector3 Offset;

        if(Me != NULL)
        {
            Offset.X -= Me->Position.SectorX * RealmCrafter::SectorVector::SectorSize;
            Offset.Z -= Me->Position.SectorZ * RealmCrafter::SectorVector::SectorSize;
        }
        TCamPos.Reset(EntityX(Cam, true), EntityY(Cam, true), EntityZ(Cam, true));
        TCamPos -= Offset;

        NGin::Math::Matrix View, Projection;
        memcpy(View.M, lightView, 16 * sizeof(float));
        memcpy(Projection.M, lightProjection, 16 * sizeof(float));

        NGin::Math::Matrix ViewInverse;
        //D3DXMATRIX ViewInverse;
        D3DXMatrixInverse(reinterpret_cast<D3DXMATRIX*>(&ViewInverse), NULL, reinterpret_cast<D3DXMATRIX*>(&View));

        TreeManager->RenderDepth(View, Projection,  *(reinterpret_cast<NGin::Math::Vector3*>(&TCamPos)), Offset);

        StopDebugTimer();
    }
}

void RenderSolid(int rtIndex)
{
    // For the upcoming version
    if(TerrainManager != 0 || TreeRenderer != 0)
    {
        float** Directions = new float*[3];
        float** Colors = new float*[3];
        for(int i = 0; i < 3; ++i)
        {
            Directions[i] = new float[3];
            Colors[i] = new float[3];

            Directions[i][0] = 0.0f;
            Directions[i][1] = 0.0f;
            Directions[i][2] = 0.0f;
            Colors[i][0] = 0.0f;
            Colors[i][1] = 0.0f;
            Colors[i][2] = 0.0f;
        }

        bbdx2_GetDirectionalLights((float**)Directions, (float**)Colors);

        for(int i = 0; i < 3; ++i)
        {
            if(TerrainManager != 0)
            {
                TerrainManager->SetLightNormal(i, NGin::Math::Vector3(Directions[i][0], Directions[i][1], Directions[i][2]));
                TerrainManager->SetLightColor(i, NGin::Math::Color(Colors[i][0], Colors[i][1], Colors[i][2]));
            }

            if(TreeRenderer != 0)
            {
                TreeRenderer->SetLightNormal(i, NGin::Math::Vector3(Directions[i][0], Directions[i][1], Directions[i][2]));
                TreeRenderer->SetLightColor(i, NGin::Math::Color(Colors[i][0], Colors[i][1], Colors[i][2]));
            }
        }

        int Count = 0;
        unsigned int Stride = 0;
        void** LightPtr = bbdx2_GetPointLights(&Count, &Stride);

        if(TerrainManager != 0)
            TerrainManager->SetPointLights(LightPtr, Count, Stride);
        if(TreeRenderer != 0)
            TreeRenderer->SetPointLights(LightPtr, Count, Stride);

        float FogNear = bbdx2_GetFogNear();
        float FogFar = bbdx2_GetFogFar();
        unsigned int IntFogColor = bbdx2_GetFogColor();
        int R = (IntFogColor >> 16) & 0xff;
        int G = (IntFogColor >> 8) & 0xff;
        int B = IntFogColor & 0xff;
        NGin::Math::Color FogColor(R, G, B);

        if(TerrainManager != 0)
            TerrainManager->SetFog(FogColor, NGin::Math::Vector2(FogNear, FogFar));
        if(TreeRenderer != 0)
            TreeRenderer->SetFog(FogColor, NGin::Math::Vector2(FogNear, FogFar));

        TerrainManager->SetAmbientLight(NGin::Math::Color(bbdx2_GetAmbientR(), bbdx2_GetAmbientG(), bbdx2_GetAmbientB()));

        D3DXMATRIX* View = (D3DXMATRIX*)bbdx2_GetViewMatrixPtr();
        D3DXMATRIX* Projection = (D3DXMATRIX*)bbdx2_GetProjectionMatrixPtr();

        D3DXMATRIX VP;
        D3DXMatrixMultiply(&VP, View, Projection);

        RealmCrafter::RCT::FrameStatistics Stats;

        Math::Vector3 Offset;

        if(Me != NULL)
        {
            Offset.X -= Me->Position.SectorX * RealmCrafter::SectorVector::SectorSize;
            Offset.Z -= Me->Position.SectorZ * RealmCrafter::SectorVector::SectorSize;
        }

        if(TerrainManager != 0)
        {
            StartDebugTimer("Terrain (Rnd)");
            Stats = TerrainManager->Render(rtIndex, (float*)&VP, GetLightMatrix(), GetShadowMap(), Offset);
            StopDebugTimer();
        }

        StartDebugTimer("Tree (Rnd)");
        if(TreeManager != 0)
        {
            TCamPos.Reset(EntityX(Cam, true), EntityY(Cam, true), EntityZ(Cam, true));
            TCamPos -= Offset;

            NGin::Math::Matrix LightProj;
            memcpy(LightProj.M, GetLightMatrix(), sizeof(float) * 16);

            TreeManager->GetRenderer()->SetShadowMap(GetShadowMap());
            TreeManager->Render(rtIndex, *(reinterpret_cast<NGin::Math::Matrix*>(View)),
            *(reinterpret_cast<NGin::Math::Matrix*>(Projection)),
            *(reinterpret_cast<NGin::Math::Vector3*>(&TCamPos)),
            LightProj,
            Offset);
            TreeManager->GetRenderer()->SetShadowMap(NULL);
        }
        StopDebugTimer();

        bbdx2_FreeMatrixPtr((void*)View);
        bbdx2_FreeMatrixPtr((void*)Projection);
        for(int i = 0; i < 3; ++i)
        {
            delete Directions[i];
            delete Colors[i];
        }
        delete Directions;
        delete Colors;
    }
}

void RenderGUI()
{
    StartDebugTimer("GUI Render");

    if(GUIManager != 0)
        GUIManager->Render();
    StopDebugTimer();

    Perf->DO_Render();
}

void DeviceLost()
{
    if(GUIManager != 0)
        GUIManager->OnDeviceLost();
    if(TerrainManager != 0)
        TerrainManager->OnDeviceLost();
    if(TreeManager != NULL)
        TreeManager->GetRenderer()->OnDeviceLost();
}

void DeviceReset()
{
    if(GUIManager != 0)
        GUIManager->OnDeviceReset(NGin::Math::Vector2(GraphicsWidth(), GraphicsHeight()));
    if(TerrainManager != 0)
        TerrainManager->OnDeviceReset();
    if(TreeManager != NULL)
        TreeManager->GetRenderer()->OnDeviceReset();
    CameraProjMode(Cam, 1);

    if(RealmCrafter::Globals != NULL && RealmCrafter::Globals->ControlHostManager != NULL)
        RealmCrafter::Globals->ControlHostManager->ResetControlSizes();

    ResolutionChanged((float)GraphicsWidth(), (float)GraphicsHeight());
}

void UpdateChatBubbles()
{
    // Update chat bubbles
    foreachc(BubbleIt, Bubble, BubbleList)
    {
        Bubble* BubbleI = *BubbleIt;

        // Position above character's head
        ActorInstance* AI = BubbleI->ActorInstance;

        // Find a head, if not, use the body
        uint Head = FindChild(AI->EN, "Head");
        if(Head == 0)
            Head = AI->EN;
        if(Head != 0)
        {
            // Check if its in range
            float Px = EntityX(AI->EN, true);
            float Py = EntityY(AI->EN, true);
            float Pz = EntityZ(AI->EN, true);
            
            float Cx = EntityX(Cam, true);
            float Cy = EntityY(Cam, true);
            float Cz = EntityZ(Cam, true);

            float Dx = Abs(Px - Cx);
            float Dy = Abs(Py - Cy);
            float Dz = Abs(Pz - Cz);

            float Distance = (Dx * Dx + Dy * Dy + Dz * Dz);

            if(Distance > 1600.0f)
            {
                // Fade if necessary
                float Fade = 1.0f - ((Distance - 1600.0f) / 5.0f);
                if(Fade < 0.0f)
                    Fade = 0.0f;

                if(Fade == 0.0f)
                {
                    BubbleI->EN->Visible = false;
                }
                else
                {
                    NGin::Math::Color ForeFadeColor = BubbleI->EN->ForeColor;
                    NGin::Math::Color BackFadeColor = BubbleI->EN->BackColor;
                    ForeFadeColor.A = Fade;
                    BackFadeColor.A = Fade;
                    BubbleI->EN->ForeColor = ForeFadeColor;
                    BubbleI->EN->BackColor = BackFadeColor;
                    BubbleI->EN->Visible = true;
                }
            }else
            {
                    NGin::Math::Color ForeFadeColor = BubbleI->EN->ForeColor;
                    NGin::Math::Color BackFadeColor = BubbleI->EN->BackColor;
                    ForeFadeColor.A = 1.0f;
                    BackFadeColor.A = 1.0f;
                    BubbleI->EN->ForeColor = ForeFadeColor;
                    BubbleI->EN->BackColor = BackFadeColor;
                    BubbleI->EN->Visible = true;
            }

            bool DeleteBubble = false;
            int Time = MilliSecs() - BubbleI->Timer;

            // Fade in over a quater of a second
            if(Time < 250)
            {
                float Alpha = (((float)Time) / 312.5f);

                NGin::Math::Color ForeFadeColor = BubbleI->EN->ForeColor;
                NGin::Math::Color BackFadeColor = BubbleI->EN->BackColor;
                
                if(ForeFadeColor.A == 1.0f)
                {
                    ForeFadeColor.A = Alpha;
                    BackFadeColor.A = Alpha;
                }
                else
                {
                    ForeFadeColor.A *= Alpha;
                    BackFadeColor.A *= Alpha;
                }

                BubbleI->EN->ForeColor = ForeFadeColor;
                BubbleI->EN->BackColor = BackFadeColor;
            }else if(Time > 5000) // Fade out after 5 seconds
            {
                Time -= 5000;
                if(Time > 500)
                {
                    DeleteBubble = true;
                }else
                {
                    float Alpha = (0.8f - (((float)Time) / 625.0f));

                    NGin::Math::Color ForeFadeColor = BubbleI->EN->ForeColor;
                    NGin::Math::Color BackFadeColor = BubbleI->EN->BackColor;
                
                    if(ForeFadeColor.A == 1.0f)
                    {
                        ForeFadeColor.A = Alpha;
                        BackFadeColor.A = Alpha;
                    }
                    else
                    {
                        ForeFadeColor.A *= Alpha;
                        BackFadeColor.A *= Alpha;
                    }

                    BubbleI->EN->ForeColor = ForeFadeColor;
                    BubbleI->EN->BackColor = BackFadeColor;
                }
            }



            // If its in use
            if(BubbleI->EN->Visible)
            {
                // Use bbdxs projection feature to translate the 3D position into 2D
                D3DXVECTOR3 MePos(EntityX(Head, true), EntityY(Head, true) + 1.0f, EntityZ(Head, true));
                D3DXVECTOR3 ProjectedPos;

                bbdx2_ProjectVector3(&MePos, &ProjectedPos);

                // Usable projections are in a 0-1 range, hide out of range text
                if(ProjectedPos.z < 0.0f || ProjectedPos.z > 1.0f)
                {
                    BubbleI->EN->Location = NGin::Math::Vector2(-1, -1);
                }else
                {
                    BubbleI->EN->Location = NGin::Math::Vector2(ProjectedPos.x, ProjectedPos.y);
                }
            }else
            {
                BubbleI->EN->Location = NGin::Math::Vector2(-1, -1);
            }

            if(DeleteBubble)
            {
                GUIManager->Destroy(BubbleI->EN);
                Bubble::Delete(BubbleI);
            }
        }
    
        nextc(BubbleIt, Bubble, BubbleList);
    }
    Bubble::Clean();

}

void UpdateNametags()
{	
    
    foreachc(AIIt, ActorInstance, ActorInstanceList)
    {
        ActorInstance* AI = *AIIt;
    
        // Apply nametags to living actors
        if(AI->Attributes->Value[RealmCrafter::Globals->HealthStat] > 0)
        {
            if(AI->NametagEN != 0)
            {
                // Find a head, if not, use the body
                uint Head = FindChild(AI->EN, "Head");
                if(Head == 0)
                    Head = AI->EN;
                if(Head != 0)
                {
                    // Check if its in range
                    float Px = EntityX(AI->EN, true);
                    float Py = EntityY(AI->EN, true);
                    float Pz = EntityZ(AI->EN, true);

                    float Cx = EntityX(Cam, true);
                    float Cy = EntityY(Cam, true);
                    float Cz = EntityZ(Cam, true);

                    float Dx = Abs(Px - Cx);
                    float Dy = Abs(Py - Cy);
                    float Dz = Abs(Pz - Cz);

                    float Distance = (Dx * Dx + Dy * Dy + Dz * Dz);

                    if(Distance > 1600.0f)
                    {
                        // Fade if necessary
                        float Fade = 1.0f - ((Distance - 1600.0f) / 5.0f);
                        if(Fade < 0.0f)
                            Fade = 0.0f;

                        if(Fade == 0.0f)
                        {
                            AI->NametagEN->Visible = false;
                            if(AI->TagEN != 0)
                                AI->TagEN->Visible = false;
                        }
                        else
                        {
                            NGin::Math::Color NameFadeColor = AI->NametagEN->ForeColor;
                            NameFadeColor.A = Fade;
                            AI->NametagEN->ForeColor = NameFadeColor;
                            AI->NametagEN->Visible = true;
                            
                            if(AI->TagEN != 0)
                            {
                                NGin::Math::Color TagFadeColor = AI->TagEN->ForeColor;
                                TagFadeColor.A = Fade;
                                AI->TagEN->ForeColor = TagFadeColor;
                                AI->TagEN->Visible = true;
                            }
                        }
                    }else
                    {
                            NGin::Math::Color NameFadeColor = AI->NametagEN->ForeColor;
                            NameFadeColor.A = 1.0f;
                            AI->NametagEN->ForeColor = NameFadeColor;
                            AI->NametagEN->Visible = true;
                            
                            if(AI->TagEN != 0)
                            {
                                NGin::Math::Color TagFadeColor = AI->TagEN->ForeColor;
                                TagFadeColor.A = 1.0f;
                                AI->TagEN->ForeColor = TagFadeColor;
                                AI->TagEN->Visible = true;
                            }
                    }
                
                    // If its in use
                    if(AI->NametagEN->Visible)
                    {
                        // Use bbdxs projection feature to translate the 3D position into 2D
                        D3DXVECTOR3 MePos(EntityX(Head, true), EntityY(Head, true) + 1.0f, EntityZ(Head, true));
                        D3DXVECTOR3 ProjectedPos;

                        bbdx2_ProjectVector3(&MePos, &ProjectedPos);

                        // Usable projections are in a 0-1 range, hide out of range text
                        if(ProjectedPos.z < 0.0f || ProjectedPos.z > 1.0f)
                        {
                            AI->NametagEN->Location = NGin::Math::Vector2(-1, -1);
                            if(AI->TagEN != 0)
                                AI->TagEN->Location = NGin::Math::Vector2(-1, -1);
                        }else
                        {
                            float NameWidth = AI->NametagEN->InternalWidth;
                            AI->NametagEN->Location = NGin::Math::Vector2(ProjectedPos.x - (NameWidth * 0.5f), ProjectedPos.y);
                            //AI->NametagEN->SendToBack();
    
                            if(AI->TagEN != 0)
                            {
                                float TagWidth = AI->TagEN->InternalWidth;
                                AI->TagEN->Location = NGin::Math::Vector2(ProjectedPos.x - (TagWidth * 0.5f), ProjectedPos.y + AI->NametagEN->Size.Y);
                                //AI->TagEN->SendToBack();
                            }
                        }
                    }else
                    {
                        AI->NametagEN->Location = NGin::Math::Vector2(-1, -1);
                        if(AI->TagEN != 0)
                            AI->TagEN->Location = NGin::Math::Vector2(-1, -1);
                    }
                }
            }
        }

        nextc(AIIt, ActorInstance, ActorInstanceList);
    }
}

// TEMP: Move this
float ActorFrameGravity = 0.0f;

// Get a position value from an offset
Math::Vector3 PositionFromSectorOffset(RealmCrafter::SectorVector &sectorvector, int sectorX, int sectorZ)
{
    float OffsetX = (float)((int)sectorvector.SectorX - sectorX);
    float OffsetZ = (float)((int)sectorvector.SectorZ - sectorZ);

    OffsetX *= RealmCrafter::SectorVector::SectorSize;
    OffsetZ *= RealmCrafter::SectorVector::SectorSize;

    OffsetX += sectorvector.X;
    OffsetZ += sectorvector.Z;

    return Math::Vector3(OffsetX, sectorvector.Y, OffsetZ);
}

// Local player changed sector, we need to reposition the world!
void PlayerChangedSector(int sectorX, int sectorZ)
{
    uint StartTime = timeGetTime();

    foreachc(SIt, Scenery, SceneryList)
    {
        Scenery* S = (*SIt);
        
        Math::Vector3 Pos = PositionFromSectorOffset(S->Position, sectorX, sectorZ);
        if(S->EN != 0)
            PositionEntity(S->EN, Pos.X, Pos.Y, Pos.Z);
 
        nextc(SIt, Scenery, SceneryList);
    }

    uint T0 = timeGetTime();

    foreachc(WIt, Water, WaterList)
    {
        Water* W = (*WIt);
 
        Math::Vector3 Pos = PositionFromSectorOffset(W->Position, sectorX, sectorZ);
        if(W->EN != 0)
            PositionEntity(W->EN, Pos.X, Pos.Y, Pos.Z);
 
        nextc(WIt, Water, WaterList);
    }

    uint T1 = timeGetTime();
 
    foreachc(CIt, ColBox, ColBoxList)
    {
        ColBox* C = (*CIt);
 
        Math::Vector3 Pos = PositionFromSectorOffset(C->Position, sectorX, sectorZ);
        if(C->EN != 0)
            PositionEntity(C->EN, Pos.X, Pos.Y, Pos.Z);
 
        nextc(CIt, ColBox, ColBoxList);
    }

    uint T2 = timeGetTime();
 
    foreachc(EIt, Emitter, EmitterList)
    {
        Emitter* E = (*EIt);
 
        Math::Vector3 Pos = PositionFromSectorOffset(E->Position, sectorX, sectorZ);
        if(E->EN != 0)
            PositionEntity(E->EN, Pos.X, Pos.Y, Pos.Z);
 
        nextc(EIt, Emitter, EmitterList);
    }

    uint T3 = timeGetTime();
 
    foreachc(SZIt, SoundZone, SoundZoneList)
    {
        SoundZone* SZ = (*SZIt);
 
        Math::Vector3 Pos = PositionFromSectorOffset(SZ->Position, sectorX, sectorZ);
        if(SZ->EN != 0)
            PositionEntity(SZ->EN, Pos.X, Pos.Y, Pos.Z);
 
        nextc(SZIt, SoundZone, SoundZoneList);
    }

    uint T4 = timeGetTime();
 
    foreachc(LIt, ZoneLight, ZoneLightList)
    {
        ZoneLight* L = (*LIt);
 
        Math::Vector3 Pos = PositionFromSectorOffset(L->Position, sectorX, sectorZ);
        if(L->Handle != 0)
            SetLightPosition(L->Handle, Pos.X, Pos.Y, Pos.Z);
 
        nextc(LIt, ZoneLight, ZoneLightList);
    }

    uint T5 = timeGetTime();

    foreachc(AIIt, ActorInstance, ActorInstanceList)
    {
        ActorInstance* AI = *AIIt;
 
        if(AI != Me)
        {
            Math::Vector3 Pos = PositionFromSectorOffset(AI->Position, sectorX, sectorZ);
            if(AI->CollisionEN != 0)
            {
                PositionEntity(AI->CollisionEN, Pos.X, EntityY(AI->CollisionEN), Pos.Z);
                ResetEntity(AI->CollisionEN);
            }
        }
 
        nextc(AIIt, ActorInstance, ActorInstanceList);
    }

    uint T6 = timeGetTime();


    foreachc(TIt, Terrain, TerrainList)
    {
        Terrain* T = (*TIt);

        std::vector<TerrainTagItem*>* TerrainTag = (std::vector<TerrainTagItem*>*)T->Handle->GetTag();
        if(TerrainTag != NULL)
        {
            for(int i = 0; i < TerrainTag->size(); ++i)
            {
                if((*TerrainTag)[i] != NULL && (*TerrainTag)[i]->Active)
                {
                    NGin::Math::Vector3 Pos = (*TerrainTag)[i]->Position;
                    Pos.X -= (RealmCrafter::SectorVector::SectorSize * sectorX);
                    Pos.Z -= (RealmCrafter::SectorVector::SectorSize * sectorZ);

                    // Reposition it
                    PositionEntity((*TerrainTag)[i]->Mesh, Pos.X, Pos.Y, Pos.Z);
                }
            }
        }
        

        nextc(TIt, Terrain, TerrainList);
    }

    uint T7 = timeGetTime();

    for(std::list<unsigned int>::iterator TreeIt = TreesList.begin(); TreeIt != TreesList.end(); ++TreeIt)
    {
        // This is terrible... come up with an elegant solution when MT goes in.
        std::string ETag = EntityTag(*TreeIt);
        if(ETag.length() > 0)
        {
            NGin::Math::Vector3* Pos = reinterpret_cast<NGin::Math::Vector3*>(toInt(ETag));
            if(Pos != NULL)
            {
                float PosX = Pos->X - (RealmCrafter::SectorVector::SectorSize * Me->Position.SectorX);
                float PosZ = Pos->Z - (RealmCrafter::SectorVector::SectorSize * Me->Position.SectorZ);
                PositionEntity(*TreeIt, PosX, Pos->Y, PosZ);
            }
        }
    }

    uint EndTime = timeGetTime();
    int Diff = EndTime - StartTime;

    int dScenery = T0 - StartTime;
    int dWater = T1 - T0;
    int dColBox = T2 - T1;
    int dEmitter = T3 - T2;
    int dSoundZone = T4 - T3;
    int dZoneLight = T5 - T4;
    int dActorInstance = T6 - T5;
    int dTerrain = T7 - T6;
    int dTree = EndTime - T7;

    LastSectorChange = 0;
    printf("PCS: %i\n  S: %i; W: %i; C: %i; E: %i; SZ: %i; ZL: %i; AI: %i; T: %i; Tr: %i\n", Diff,
        dScenery, dWater, dColBox, dEmitter, dSoundZone, dZoneLight, dActorInstance, dTerrain, dTree);
    //UpdateCamera();

// 	if(TerrainManager != NULL && Cam != 0)
// 		TerrainManager->Update(NGin::Math::Vector3(EntityX(Cam, true), EntityY(Cam, true), EntityZ(Cam, true)), true);
}

// Updates movement etc.  for all actor instances
void UpdateActorInstances()
{
    ActorInstance::Clean();

    // Update the players shadowing (its prone to changing due to scripting influences)
    if(Me != 0 && Me->EN != 0)
        EntityShadowLevel(Me->EN, 4);
    ++RealmCrafter::Globals->LastZoneLoad;

    // Down Vector
    NGin::Math::Vector3 Down(0, 1, 0);

    SetPickModes();

    //return;

    // Updates for every actor

// 	int DLIt = 0;
// 	for(DLIt = 0; DLIt < ActorDBLs.size(); ++DLIt)
// 	{
// 		ActorDBLs[DLIt]->Text = "";
// 	}
// 	DLIt = 0;

    

    foreachc(AIIt, ActorInstance, ActorInstanceList)
    {
        ActorInstance* AI = *AIIt;

        // Send this information to the debugger (If its attached).
        DebugActor((int)AI, AI->Position.SectorX, AI->Position.SectorZ, AI->Position.X, AI->Position.Z, (AI == Me) ? 1 : 0);

#if 0
        if(DLIt < ActorDBLs.size())
        {
            ActorDBLs[DLIt]->Text = toString((int)AI) + ": " + AI->Name + ": " + AI->Position.ToString() + ";     " + AI->Destination.ToString()  + ";     "
                + Math::Vector3(EntityX(AI->CollisionEN, true), EntityY(AI->CollisionEN, true), EntityZ(AI->CollisionEN, true)).ToString();
        }
        ++DLIt;
#endif

        AI->UpdateGubbins();
        AI->UpdateShaderParameters();


        ArrayList<int> DeleteList;
        for(int i = 0; i < AI->FloatingNumbers.Size(); ++i)
        {
            if(!UpdateFloatingNumber(AI->FloatingNumbers[i]))
                DeleteList.Add(i);
        }
        for(int i = 0; i < DeleteList.Size(); ++i)
        {
            delete AI->FloatingNumbers[DeleteList[i]];
            AI->FloatingNumbers.Remove(DeleteList[i]);
        }

        // if it's not dead
        if(AI->Attributes->Value[RealmCrafter::Globals->HealthStat] > 0)
        {
            // Fade in (uses ->AIMode field which is a server field, to save adding an extra field and using server memory)
            if(AI->AIMode < 501)
            {
                if(AI == Me || AI == Me->Mount)
                    AI->AIMode = 499; // The player and the player's mount do not fade in
                AI->AIMode = AI->AIMode + 2;
                float Alpha = ((float)AI->AIMode) / 501.0f;
//				EntityAlpha AI->EN, Alpha#
//				EntityAlpha AI->ShadowEN, Alpha#
//				if(AI->WeaponEN != 0 Then EntityAlpha AI->WeaponEN, Alpha#
//				if(AI->ShieldEN != 0 Then EntityAlpha AI->ShieldEN, Alpha#
//				if(AI->HatEN != 0 Then EntityAlpha AI->HatEN, Alpha#
            }

            // Check if(this actor is underwater
            AI->Underwater = 0;
            if(AI->Actor->Environment == Environment_Amphibious)
            {
                foreachc(WIt, Water, WaterList)
                {
                    Water* W = *WIt;

                    if(EntityY(AI->CollisionEN) < EntityY(W->EN) - 0.4f)
                        if(Abs(EntityX(AI->CollisionEN) - EntityX(W->EN)) < (W->ScaleX / 2.0f))
                            if(Abs(EntityZ(AI->CollisionEN) - EntityZ(W->EN)) < (W->ScaleZ / 2.0f))
                            {
                                AI->Underwater = Handle(W);
                                PositionEntity(AI->EN, 0.0f, 0.0f, 0.0f);
                                break;
                            }

                    nextc(WIt, Water, WaterList);
                }
            }
            if(AI->Underwater == 0)
                PositionEntity(AI->EN, 0.0f, MeshHeight(AI->EN) / -2.0f, 0.0f);

            // Show/hide nametags in HideNametags mode 2 (selected only)
            if(HideNametags == 2)
            {
                if(AI->NametagEN != 0)
                    if(Handle(AI) == PlayerTarget)
                        AI->NametagEN->Visible = true;
                    else
                        AI->NametagEN->Visible = false;
                if(AI->TagEN != 0)
                    if(Handle(AI) == PlayerTarget)
                        AI->TagEN->Visible = true;
                    else
                        AI->TagEN->Visible = false;
            }


            // Update only if this actor is not riding a mount
            if(AI->Mount == 0)
            {
                // Hide nametag if(this actor is a mount
                if(AI->NametagEN != 0 && HideNametags == 0)
                    if(AI->Rider == 0)
                        AI->NametagEN->Visible = true;
                    else
                        AI->NametagEN->Visible = false;
                if(AI->TagEN != 0 && HideNametags == 0)
                    if(AI->Rider == 0)
                        AI->TagEN->Visible = true;
                    else
                        AI->TagEN->Visible = false;


                // Gravity
                if(AI->Actor->Environment != Environment_Swim && AI->Actor->Environment != Environment_Fly && AI->Underwater == 0)
                {
                    bool ApplyGravity = false;

                    // Check world collisions, we need to know if we can apply gravity safely.
                    //if(AI != Me)
                    //{
                        uint Ground = LinePick(EntityX(AI->CollisionEN, true), EntityY(AI->CollisionEN, true), EntityZ(AI->CollisionEN, true), 
                            EntityX(AI->CollisionEN, true), EntityY(AI->CollisionEN, true) - 10000.0f, EntityZ(AI->CollisionEN, true));
                        NGin::Math::Vector3 GroundPos(PickedX(), PickedY(), PickedZ());
                        uint Sky = LinePick(EntityX(AI->CollisionEN, true), EntityY(AI->CollisionEN, true) + 10000.0f, EntityZ(AI->CollisionEN, true), 
                            EntityX(AI->CollisionEN, true), EntityY(AI->CollisionEN, true), EntityZ(AI->CollisionEN, true));
                        NGin::Math::Vector3 SkyPos(PickedX(), PickedY(), PickedZ());

                        NGin::Math::Vector3 ActorPos(EntityX(AI->CollisionEN, true), EntityY(AI->CollisionEN, true), EntityZ(AI->CollisionEN, true));
                        NGin::Math::Vector3 MyPos(EntityX(Me->CollisionEN, true), EntityY(Me->CollisionEN, true), EntityZ(Me->CollisionEN, true));


                        if(Ground == AI->CollisionEN)
                            RuntimeError("Picked myself... I shouldn't be able to do this!");
                        if(Ground == Sky)
                            Sky = 0;

                        if(Ground == 0 && Sky != 0)
                        {
                            ResetEntity(AI->CollisionEN);
                            PositionEntity(AI->CollisionEN, SkyPos.X, SkyPos.Y + 3.5f, SkyPos.Z);
                        }else if(Ground == 0 && Sky == 0)
                        {
                            ApplyGravity = false;
                        }else if(Ground != 0 && Sky == 0)
                        {
                            // Make sure its close to the player, otherwise this could trigger a false positive
                            if(ActorPos.DistanceSq(MyPos) < 350.0f * 350.0f)
                                ApplyGravity = true;
                        }else if(Ground != 0 && Sky != 0)
                        {
                            // Prevent false positive
                            if(ActorPos.DistanceSq(MyPos) < 350.0f * 350.0f)
                            {
                                if(SkyPos.DistanceSq(ActorPos) < GroundPos.DistanceSq(ActorPos))
                                {
                                    ResetEntity(AI->CollisionEN);
                                    PositionEntity(AI->CollisionEN, SkyPos.X, SkyPos.Y + 3.5f, SkyPos.Z);
                                }else
                                {
                                    ApplyGravity = true;
                                }
                            }
                        }
// 					}else
// 					{
// 						ApplyGravity = true;
// 					}

                    if(RealmCrafter::Globals->LastZoneLoad < 3)
                        ApplyGravity = false;

                    if(LastSectorChange > 2 && ApplyGravity)
                    {
                        AI->Position.Y += -(Gravity * Delta);
                        //AI->Y = -0.5f;
                        TranslateEntity(AI->CollisionEN, 0.0f, AI->Position.Y, 0.0f);

                        if(AI == Me)
                            ActorFrameGravity = AI->Position.Y;

                        // Reset velocity on contact with ground
                        if(AI->Position.Y < 0.0f)
                        {
                            for(int i = 1; i <= CountCollisions(AI->CollisionEN); ++i)
                            {
                                if(((PI * 0.5f) - acos(Abs(CollisionNY(AI->CollisionEN, i)))) / (PI * 0.5f) > SlopeRestrict)
                                {
                                    AI->Position.Y = 0.0f;
                                    
                                    // Remember when the player touches down after a jump
                                    if(AI == Me)
                                    {
                                        PlayerHasTouchedDown = true;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }

                // Correct to server Y position for flying actors
                if(AI->Actor->Environment == Environment_Fly && AI != Me && AI != Me->Mount)
                {
                    float YPos = CurveValue(EntityY(AI->CollisionEN), AI->Position.Y, 7.0f);
                    PositionEntity(AI->CollisionEN, EntityX(AI->CollisionEN), YPos, EntityZ(AI->CollisionEN));
                }

                //Tmp: Used to toggle test
                bool tmpGlobal = false;

                // Movement/animation

                int DestSectorOffsetX = (int)AI->Destination.SectorX - (int)AI->Position.SectorX;
                int DestSectorOffsetZ = (int)AI->Destination.SectorZ - (int)AI->Position.SectorZ;
                float DestX = AI->Destination.X + ((float)DestSectorOffsetX * RealmCrafter::SectorVector::SectorSize);
                float DestZ = AI->Destination.Z + ((float)DestSectorOffsetZ * RealmCrafter::SectorVector::SectorSize);

                int AISectorOffsetX = 0;
                int AISectorOffsetZ = 0;

                if(Me != NULL)
                {
                    AISectorOffsetX = (int)AI->Position.SectorX - (int)Me->Position.SectorX;
                    AISectorOffsetZ = (int)AI->Position.SectorZ - (int)Me->Position.SectorZ;
                }
                float AIX = AI->Position.X + ((float)AISectorOffsetX * RealmCrafter::SectorVector::SectorSize);
                float AIZ = AI->Position.Z + ((float)AISectorOffsetZ * RealmCrafter::SectorVector::SectorSize);
                DestX += ((float)AISectorOffsetX * RealmCrafter::SectorVector::SectorSize);
                DestZ += ((float)AISectorOffsetZ * RealmCrafter::SectorVector::SectorSize);

                // Regardless of distance, the Yaw must always be updated
                if(AI != Me)
                {
                    // Don't calculate turn if its not necessary
                    if(Abs(AI->Yaw - AI->NewYaw) > 4.0f)
                    {
                        float Prev = AI->PrevYaw;
                        float New = AI->NewYaw + 180.0f;
                        float T = ((float)(MilliSecs() - AI->LastUpdateTime)) / 200.0f; // 200.0f is the position broadcast time

                       //  Regular interpolate is too long, go backwards (wrap around)
                        if(Abs(Prev - New) > 180.0f)
                        {
                            float NNew = New + (0 - Prev);
                            while(NNew > 180.0f)
                                NNew -= 360.0f;
                            while(NNew < -180.0f)
                                NNew += 260.0f;

                            float NLrp = T * NNew;

                            AI->Yaw = Prev + NLrp;
                            RotateEntity(AI->CollisionEN, 0, AI->NewYaw, 0);
                        }else
                        {
                            AI->Yaw = Prev + T * (New - Prev);
                            RotateEntity(AI->CollisionEN, 0, AI->NewYaw, 0);
                        }

                        //AI->Yaw = New;
                        //RotateEntity(AI->CollisionEN, 0, AI->Yaw + 180.0f, 0);
                    }
                }

                PositionEntity(GPP, DestX, EntityY(AI->CollisionEN), DestZ);
                //PositionEntity(AI->CollisionEN, AI->DestX, EntityY(AI->CollisionEN), AI->DestZ);
                if(Pow(DestX - EntityX(AI->CollisionEN), 2) + Pow(DestZ - EntityZ(AI->CollisionEN), 2) > 4.0f || AI->ForceUpdate == true)
                {
                    AI->ForceUpdate = false;

                    // Smoothly correct to server position
                    float XPos = CurveValue(EntityX(AI->CollisionEN, tmpGlobal), AIX, 20.0f);
                    float ZPos = CurveValue(EntityZ(AI->CollisionEN, tmpGlobal), AIZ, 20.0f);

                    //PositionEntity(AI->CollisionEN, XPos, EntityY(AI->CollisionEN), ZPos);

                    // Emergency reset if(stuck somewhere
                    float XDist = Abs(XPos - AIX);
                    float ZDist = Abs(ZPos - AIZ);
                    float Dist = (XDist * XDist) + (ZDist * ZDist);

                    //// 2.21 TEMP removed for testing
                    if(Dist > 1000000.0f)
                        if(AI != Me && AI != Me->Mount)
                        {
                            ResetEntity(AI->CollisionEN);
                            PositionEntity(AI->CollisionEN, AIX, EntityY(AI->CollisionEN, tmpGlobal) + 10.0f, AIZ);
                    //		uint Result = LinePick(AI->X, EntityY(AI->CollisionEN, tmpGlobal) + 10000.0f, AI->Z, 0.0f, -20000.0f, 0.0f);
                    //		if(Result != 0)
                    //			PositionEntity(AI->CollisionEN, AI->X, PickedY() + (MeshHeight(AI->EN) * 0.05f), AI->Z);
                            
                        }



                    // Calculate speed
                    float Speed = 1.5f * (((float)AI->Attributes->Value[RealmCrafter::Globals->SpeedStat]) / ((float)AI->Attributes->Maximum[RealmCrafter::Globals->SpeedStat])) * Delta;
                    if(AI->IsRunning == true)
                        Speed = Speed * 2.0f;
                    else if(AI->WalkingBackward == true)
                        Speed = Speed / 2.0f;

                    // Move
                    PointEntity(AI->CollisionEN, GPP);
                    MoveEntity(AI->CollisionEN, 0.0f, 0.0f, Speed);
                    if(AI != Me)
                        RotateEntity(AI->CollisionEN, 0, AI->Yaw, 0);

                    if(AI == Me)
                    {
                        int PrevSectorX = Me->Position.SectorX;
                        int PrevSectorZ = Me->Position.SectorZ;


                        AI->Position.X = EntityX(AI->CollisionEN, true);
                        AI->Position.Z = EntityZ(AI->CollisionEN, true);
                        AI->Position.FixValues();


                        if(PrevSectorX != Me->Position.SectorX || PrevSectorZ != Me->Position.SectorZ)
                        {
                            int DiffX = Me->Position.SectorX - PrevSectorX;
                            int DiffZ = Me->Position.SectorZ - PrevSectorZ;

                            if(DiffX == 1 && Me->Position.X < 2.0f)
                            {
                                Me->Position.SectorX -= 1;
                                Me->Position.X += RealmCrafter::SectorVector::SectorSize;
                                PrevSectorX = Me->Position.SectorX;
                            }else if(DiffX == -1 && Me->Position.X > 766.0f)
                            {
                                Me->Position.SectorX += 1;
                                Me->Position.X -= RealmCrafter::SectorVector::SectorSize;
                                PrevSectorX = Me->Position.SectorX;
                            }

                            if(DiffZ == 1 && Me->Position.Z < 2.0f)
                            {
                                Me->Position.SectorZ -= 1;
                                Me->Position.Z += RealmCrafter::SectorVector::SectorSize;
                                PrevSectorZ = Me->Position.SectorZ;
                            }else if(DiffZ == -1 && Me->Position.Z > 766.0f)
                            {
                                Me->Position.SectorZ += 1;
                                Me->Position.Z -= RealmCrafter::SectorVector::SectorSize;
                                PrevSectorZ = Me->Position.SectorZ;
                            }

                            if(PrevSectorX != Me->Position.SectorX || PrevSectorZ != Me->Position.SectorZ)
                            {
                                DiffX = Me->Position.SectorX - PrevSectorX;
                                DiffZ = Me->Position.SectorZ - PrevSectorZ;

                                ResetEntity(AI->CollisionEN);
                                
                                PositionEntity(Me->CollisionEN,
                                    EntityX(Me->CollisionEN) - (DiffX * RealmCrafter::SectorVector::SectorSize),
                                    EntityY(Me->CollisionEN),
                                    EntityZ(Me->CollisionEN) - (DiffZ * RealmCrafter::SectorVector::SectorSize));

                                PlayerChangedSector(Me->Position.SectorX, Me->Position.SectorZ);

                                DestSectorOffsetX = (int)AI->Destination.SectorX - (int)AI->Position.SectorX;
                                DestSectorOffsetZ = (int)AI->Destination.SectorZ - (int)AI->Position.SectorZ;
                                DestX = AI->Destination.X + ((float)DestSectorOffsetX * RealmCrafter::SectorVector::SectorSize);
                                DestZ = AI->Destination.Z + ((float)DestSectorOffsetZ * RealmCrafter::SectorVector::SectorSize);
                            }
                        }
                    }

                    if(AI->WalkingBackward == false)
                        TurnEntity(AI->CollisionEN, 0.0f, 0.0, 0.0f);

                    // if actor is not already animating, animate it
                    int Seq = CurrentSeq(AI);
                    if(Seq == Anim_Idle || Animating(AI->EN) == false || Seq == Anim_RideIdle || Seq == Anim_SwimIdle)
                    {
                        if(AI->Underwater == 0)
                        {
                            if(AI->IsRunning == true)
                            {
                                PlayAnimation(AI, 1, 0.05f, Anim_Run);
                                if(AI->Rider != 0)
                                    if(CurrentSeq(AI->Rider) != Anim_RideRun)
                                        PlayAnimation(AI->Rider, 1, 0.05f, Anim_RideRun);
                            }else
                            {
                                if(AI->StrafingLeft)
                                    PlayAnimation(AI, 1, 0.04, Anim_StrafeLeft);
                                else if(AI->StrafingRight)
                                    PlayAnimation(AI, 1, 0.04, Anim_StrafeRight);
                                else if(AI->WalkingBackward)
                                    PlayAnimation(AI, 1, -0.02, Anim_Walk);
                                else
                                    PlayAnimation(AI, 1, 0.04, Anim_Walk);
                                    
                                
                                if(AI->Rider != 0)
                                    if(CurrentSeq(AI->Rider) != Anim_RideWalk)
                                        PlayAnimation(AI->Rider, 1, 0.05f, Anim_RideWalk);
                            }
                        }else
                        {
                            if(AI->IsRunning == true)
                            {
                                PlayAnimation(AI, 1, 0.05f, Anim_SwimFast);
                                if(AI->Rider != 0)
                                    if(CurrentSeq(AI->Rider) != Anim_RideRun)
                                        PlayAnimation(AI->Rider, 1, 0.05f, Anim_RideRun);
                            }else
                            {
                                PlayAnimation(AI, 1, 0.025f, Anim_SwimSlow);
                                if(AI->Rider != 0)
                                    if(CurrentSeq(AI->Rider) != Anim_RideWalk)
                                        PlayAnimation(AI->Rider, 1, 0.05f, Anim_RideWalk);
                            }
                        }
                    
                    }else if(Seq >= Anim_RideRun || Seq == Anim_StrafeLeft || Seq == Anim_StrafeRight) // if the actor is animating, check it's doing the right one
                    {
                        if(AI->Underwater == 0)
                        {
                            if(AI->IsRunning == true && Seq != Anim_Run)
                            {
                                PlayAnimation(AI, 1, 0.05f, Anim_Run);
                                if(AI->Rider != 0)
                                    if(CurrentSeq(AI->Rider) != Anim_RideRun)
                                        PlayAnimation(AI->Rider, 1, 0.05f, Anim_RideRun);
                            }else if(AI->IsRunning == false)// && Seq != Anim_Walk)
                            {
                                if(AI->WalkingBackward && Seq != Anim_Walk)
                                {
                                    if(Seq != Anim_Walk)
                                        PlayAnimation(AI, 1, -0.02f, Anim_Walk);
                                }
                                else if(AI->StrafingLeft)
                                {
                                    if(Seq != Anim_StrafeLeft)
                                        PlayAnimation(AI, 1, 0.04, Anim_StrafeLeft);
                                }
                                else if(AI->StrafingRight)
                                {
                                    if(Seq != Anim_StrafeRight)
                                        PlayAnimation(AI, 1, 0.04, Anim_StrafeRight);
                                }
                                else if(Seq != Anim_Walk)
                                {
                                    PlayAnimation(AI, 1, 0.04f, Anim_Walk);
                                }

                                if(AI->Rider != 0)
                                    if(CurrentSeq(AI->Rider) != Anim_RideWalk)
                                        PlayAnimation(AI->Rider, 1, 0.05f, Anim_RideWalk);
                            }
                        }else
                        {
                            if(AI->IsRunning == true && Seq != Anim_SwimFast)
                            {
                                PlayAnimation(AI, 1, 0.05f, Anim_SwimFast);
                                if(AI->Rider != 0)
                                    if(CurrentSeq(AI->Rider) != Anim_RideRun)
                                        PlayAnimation(AI->Rider, 1, 0.05f, Anim_RideRun);
                            }else if(AI->IsRunning == false && Seq != Anim_SwimSlow)
                            {
                                PlayAnimation(AI, 1, 0.025f, Anim_SwimSlow);
                                if(AI->Rider != 0)
                                    if(CurrentSeq(AI->Rider) != Anim_RideWalk)
                                        PlayAnimation(AI->Rider, 1, 0.05f, Anim_RideWalk);
                            }
                        }
                    }

                    // Footstep sound effects
                    if(AI->Underwater == 0)
                    {
                        static float LastAnimT = 0.0f;
                        float AnimT = AnimTime(AI->EN);
                        //DL2->Text = AnimT;
                        //if(AnimT < 1.0f || AnimT > ((float)AnimLength(AI->EN) - 1) || Abs(AnimT - (((float)AnimLength(AI->EN)) / 2.0f)) < 2.0f)
                        if(AnimT < LastAnimT)
                        {
                            if(AI->FootstepPlayedThisCycle == false)
                            {
                                uint Snd = 0;
                                if(AI->Gender == 0)
                                    if(Environment->PlayWetFootstep())
                                        Snd = GetSound(AI->Actor->MSpeechIDs[Speech_FootstepWet]);
                                    else
                                        Snd = GetSound(AI->Actor->MSpeechIDs[Speech_FootstepDry]);
                                else
                                    if(Environment->PlayWetFootstep())
                                        Snd = GetSound(AI->Actor->FSpeechIDs[Speech_FootstepWet]);
                                    else
                                        Snd = GetSound(AI->Actor->FSpeechIDs[Speech_FootstepDry]);
                            
                                //EmitSound(Snd, AI->EN);
                                PlaySound(Snd);
                                AI->FootstepPlayedThisCycle = true;
                            }
                        }else
                            AI->FootstepPlayedThisCycle = false;
                        LastAnimT = AnimT;
                    }
                }else
                {
                    bool SetIdle = true;
                    if(AI == Me && CameraController->MovedThisFrame())
                        SetIdle = false;
                    
                    if(SetIdle)
                    {
                        AI->IsRunning = false;
                        if(AI->Underwater == 0)
                        {
                            // Random yawn/stretch
                            if(Rand(1, 1000) == 1 && AnimSeq(AI->EN) == AI->AnimSeqs[Anim_Idle] && AI->Rider == 0)
                                PlayAnimation(AI, 3, 0.007f, Rand(Anim_LookRound, Anim_Yawn));

                            // Idle animation
                            if((CurrentSeq(AI) >= Anim_RideRun || CurrentSeq(AI) == Anim_StrafeLeft || CurrentSeq(AI) == Anim_StrafeRight || Animating(AI->EN) == false) && AnimSeq(AI->EN) != AI->AnimSeqs[Anim_Idle])
                            {
                                Animate(AI->EN, 0); // Workaround for a model bug - unfortunately causes jerky movement

                                PlayAnimation(AI, 1, 0.003f, Anim_Idle);
                            }

                            // Rider idle animation
                            if(AI->Rider != 0)
                                if(CurrentSeq(AI->Rider) != Anim_RideIdle)
                                    PlayAnimation(AI->Rider, 1, 0.003f, Anim_RideIdle);
                        }else
                        {
                            // Underwater idle animation
                            if((CurrentSeq(AI) >= Anim_RideRun || CurrentSeq(AI) == Anim_Idle || Animating(AI->EN) == false) && AnimSeq(AI->EN) != AI->AnimSeqs[Anim_SwimIdle])
                            {
                                PlayAnimation(AI, 1, 0.005f, Anim_SwimIdle);
                                if(AI->Rider != 0)
                                    if(CurrentSeq(AI->Rider) != Anim_RideIdle)
                                        PlayAnimation(AI->Rider, 1, 0.003f, Anim_RideIdle);
                            }
                        }
                    }
                }// Distance check
            } // Mount check

// Jared: Removed section - Culling handles animation updates internally
//			// Hide if(not in view to stop animations being updated
//			if(!EntityInView(AI->EN))
//				HideEntity(AI->EN);
//			else
//				ShowEntity(AI->EN);
        }else // If it is dead
        {
            // Apply gravity to flying/swimming creatures
            if(AI->Actor->Environment == Environment_Swim || AI->Actor->Environment == Environment_Fly)
            {
                AI->Position.Y = AI->Position.Y - (Gravity * Delta);

                // Reset velocity on contact with ground
                if(AI->Position.Y < 0.0f)
                    for(int i = 1; i <= CountCollisions(AI->CollisionEN); ++i)
                        if(CollisionNY(AI->CollisionEN, i) > SlopeRestrict)
                        {
                            AI->Position.Y = 0.0f;
                            break;
                        }

                TranslateEntity(AI->CollisionEN, 0.0f, AI->Position.Y, 0.0f);
            }

            // NPCs only fade out
            if(Animating(AI->EN) == false && AI->RNID == 0)
                if(AI->AIMode > 0)
                {
                    --AI->AIMode;
                    float Alpha = ((float)AI->AIMode) / 501.0f;
                    EntityAlpha(AI->EN, Alpha);

                    for(int gi = 0; gi < AI->WeaponENs.size(); ++gi)
                        if(AI->WeaponENs[gi]->Mesh != 0)
                            EntityAlpha(AI->WeaponENs[gi]->Mesh, Alpha);
                    for(int gi = 0; gi < AI->ShieldENs.size(); ++gi)
                        if(AI->ShieldENs[gi]->Mesh != 0)
                            EntityAlpha(AI->ShieldENs[gi]->Mesh, Alpha);
                    for(int gi = 0; gi < AI->HatENs.size(); ++gi)
                        if(AI->HatENs[gi]->Mesh != 0)
                            EntityAlpha(AI->HatENs[gi]->Mesh, Alpha);

                }else
                    SafeFreeActorInstance(AI);

        }
        nextc(AIIt, ActorInstance, ActorInstanceList);
    }

}

// Updates actors on horseback who don't have their own collision
void UpdateRidingActorInstances()
{
    ActorInstance::Clean();

    // Go through again and update positions of actors riding mounts
    foreachc(AIIt, ActorInstance, ActorInstanceList)
    {
        ActorInstance* AI = *AIIt;

        if(AI->Mount != 0)
        {
            // Move the rider to the saddle position
            uint SaddleEN = FindChild(AI->Mount->EN, "Mount");

            if(SaddleEN == 0)
                SaddleEN = AI->Mount->CollisionEN;
            PositionEntity(AI->CollisionEN, EntityX(SaddleEN, true), EntityY(SaddleEN, true), EntityZ(SaddleEN, true));
            TranslateEntity(AI->CollisionEN, 0.0f, EntityY(AI->CollisionEN) - EntityY(AI->EN, true), 0.0f);
            RotateEntity(AI->CollisionEN, 0.0f, EntityYaw(AI->Mount->CollisionEN), 0.0f);
        }

        nextc(AIIt, ActorInstance, ActorInstanceList);
    }

}

// Updates actors who are underwater, in case UpdateWorld's collision has moved them above the surface
void UpdateSwimmingActorInstances()
{
    ActorInstance::Clean();

    // Cycle through each underwater actor
    foreachc(AIIt, ActorInstance, ActorInstanceList)
    {
        ActorInstance* AI = *AIIt;

        if(AI->Underwater != 0)
        {
            if(AI->Actor->Environment == Environment_Amphibious)
            {
                foreachc(WIt, Water, WaterList)
                {
                    Water* W = *WIt;

                    if(EntityY(AI->CollisionEN) > EntityY(W->EN) - 0.5f && AI != Me)
                    {
                        AI->Underwater = 0;
                        // JB 2.22 swimming
                        //PositionEntity(AI->EN, 0.0f, MeshHeight(AI->EN) / -2.0f, 0.0f);
                    }

                    nextc(WIt, Water, WaterList);
                }
            }
        }
    
        nextc(AIIt, ActorInstance, ActorInstanceList);
    }

}

// Updates the camera
void UpdateCamera()
{
    CameraController->Update(Cam, Me);
    // Underwater camera effects
    bool WasUnderwater = CameraUnderwater;
    CameraUnderwater = false;
    foreachc(WIt, Water, WaterList)
    {
        Water* W = *WIt;

        if(EntityY(Cam) < EntityY(W->EN))
            if(fabs(EntityX(Cam) - EntityX(W->EN)) < (W->ScaleX / 2.0f))
                if(fabs(EntityZ(Cam) - EntityZ(W->EN)) < (W->ScaleZ / 2.0f))
                {
// 					CameraClsColor(Cam, W->Red, W->Green, W->Blue);
// 					CameraFogColor(Cam, W->Red, W->Green, W->Blue);
// 					FogNearNow = 1.0f;
// 					FogFarNow = 50.0f;
// 					Environment->SetViewDistance(FogNearNow, FogFarNow, false);
                    Environment->SetCameraUnderWater(true, W->Red, W->Green, W->Blue);
                    CameraUnderwater = true;
                    break;
                }

                nextc(WIt, Water, WaterList);
    }
    if(WasUnderwater == true)
        if(CameraUnderwater == false)
        {
            Environment->SetCameraUnderWater(false, 0, 0, 0);
        }

}

// Updates all animated scenery
void UpdateAnimatedScenery()
{
    foreachc(SIt, Scenery, SceneryList)
    {
        Scenery* S = *SIt;

        if(S->AnimationMode == 2)
            if(Animating(S->EN) == false)
                Animate(S->EN, 2);

        nextc(SIt, Scenery, SceneryList);
    }
}

// Updates all sound zones
void UpdateSoundZones()
{
    foreachc(SZIt, SoundZone, SoundZoneList)
    {
        SoundZone* SZ = *SZIt;

        if(SZ->ChannelStopped == false && SZ->Channel != 0)
            SZ->ChannelStopped = !ChannelPlaying(SZ->Channel);
        if(SZ->ChannelStopped)
            SZ->Channel = 0;

        

        // I am in the sound zone
// 		RealmCrafter::SectorVector Dist = RealmCrafter::SectorVector::Subtract(SZ->Position, Me->Position);
// 		float XDist = fabs(((float)Dist.SectorX * RealmCrafter::SectorVector::SectorSize) + Dist.X);
// 		float ZDist = fabs(((float)Dist.SectorZ * RealmCrafter::SectorVector::SectorSize) + Dist.Z);
// 		float YDist = fabs(Dist.Y);
// 
// 		float SPosX = ((float)SZ->Position.SectorX * RealmCrafter::SectorVector::SectorSize) + SZ->Position.X;
// 		float SPosZ = ((float)SZ->Position.SectorZ * RealmCrafter::SectorVector::SectorSize) + SZ->Position.Z;
// 
// 		float MePosX

        float XDist = fabs(EntityX(SZ->EN) - EntityX(Me->CollisionEN));
        float YDist = fabs(EntityY(SZ->EN) - EntityY(Me->CollisionEN));
        float ZDist = fabs(EntityZ(SZ->EN) - EntityZ(Me->CollisionEN));

        float DistSq = XDist * XDist + YDist * YDist + ZDist * ZDist;

        //printf("%s - %s = %f\n", SZ->Position.ToString().c_str(), Me->Position.ToString().c_str(), DistSq);
        //printf("%f\n", DistSq);

        float Radius = SZ->Radius * 2.0f;
        Radius *= Radius;

        if(DistSq < Radius)
        {
            //printf("In Rad: %f\n", SZ->Radius);
            // Sound has never been played - play it
            /*if(SZ->Channel == 0)
            {
                PlaySoundZone(SZ);
            }else */if(SZ->ChannelStopped) // Channel has stopped playing - play it when repeat timer is done
            {
                // Repeat it now
                if(SZ->RepeatTime == 0)
                {
                    PlaySoundZone(SZ);
                
                }else if(SZ->RepeatTime > 0) // Repeat it on a timer
                {
                    // Only just stopped, set timer
                    if(SZ->Timer == 0)
                    {
                        SZ->Timer = MilliSecs();
                    }else if(MilliSecs() - SZ->Timer > SZ->RepeatTime * 1000) // Time has elapsed - play it
                    {
                        SZ->Timer = 0;
                        PlaySoundZone(SZ);
                    }
                }

                
            }else
            {
                if(ChannelPlaying(SZ->Channel) && SZ->Fade < RealmCrafter::Globals->DefaultVolume * (((float)SZ->Volume) / 100.0f))
                {
                    SZ->Fade += 0.01f;
                    ChannelVolume(SZ->Channel, SZ->Fade);
                }
            }
        }else if(ChannelPlaying(SZ->Channel)) // I am outside it but it is playing - fade it out
        {
            if(SZ->Fade < 0.01f)
            {
                SZ->Fade = RealmCrafter::Globals->DefaultVolume * (((float)SZ->Volume) / 100.0f);
            }else
            {
                SZ->Fade -= 0.01f;
                ChannelVolume(SZ->Channel, SZ->Fade);
                if(SZ->Fade <= 0.03f)
                {
                    StopChannel(SZ->Channel);
                    SZ->Fade = 0.0f;
                }
            }
        }

        nextc(SZIt, SoundZone, SoundZoneList);
    }
}

// Plays a sound zone sound/music
void PlaySoundZone(SoundZone* SZ)
{

    if(SZ->SoundID != 65535)
        if(SZ->Is3D == 1)
            SZ->Channel = EmitSound(SZ->LoadedSound, SZ->EN);
        else
            SZ->Channel = PlaySound(SZ->LoadedSound);
    else
        SZ->Channel = PlayMusic(SZ->MusicFilename);
    
    if(SZ->Channel != 0)
    {
        ChannelVolume(SZ->Channel, RealmCrafter::Globals->DefaultVolume * (((float)SZ->Volume) / 100.0f));
        SZ->ChannelStopped = false;
    }


}



// Encrypts/decrypts a string (crappily but well enough to negate casual packet sniffing)
NGin::CString Encrypt(NGin::CString S, int Reverse)
{
    NGin::CString O = "";
    char T[2] = {0, 0};
    for(int i = 0; i < S.Length(); ++i)
    {
        T[0] = S.GetRealChar(i) + (26 * Reverse);
        O.Append(T, true, 1);
    }

    return O;
}

// Interpolates a variable smoothly between two values
float CurveValue(float Current, float Destination, float Curve)
{
    return Current + ((Destination - Current) / Curve);
}

// Sorts spells alphabetically
void SortSpells()
{
    // Sort known spells
    for(int i = 0; i < 1000; ++i)
        KnownSpellSort[i] = 65535;
    

    for(int i = 0; i < 1000; ++i)
        if(Me->SpellLevels[i] > 0)
            for(int j = 0; j < 1000; ++j)
                if(KnownSpellSort[j] == 65535) // Free slot in ordered list, fill it
                {
                    KnownSpellSort[j] = i;
                    break;
                }else// Spot taken, insert if(this spell is alphabetically previous to the contents
                {
                    Spell* Sp = SpellsList[Me->KnownSpells[i]];
                    Spell* Sp2 = SpellsList[Me->KnownSpells[KnownSpellSort[j]]];
                    if(HighAlphabetical(Sp->Name, Sp2->Name))
                    {
                        for(int k = 999; k > j; --k)
                            KnownSpellSort[k] = KnownSpellSort[k - 1];
                        KnownSpellSort[j] = i;
                        break;
                    }
                }

    for(int i = 0; i < 1000; ++i)
        if(KnownSpellSort[i] == 65535)
            KnownSpellSort[i] = 999;

}

// Returns true if A is before B alphabetically
bool HighAlphabetical(string &a, string &b)
{
    if(a.length() == 0) return true;
    if(b.length() == 0) return false;

    string A = stringToLower(a);
    string B = stringToLower(b);

    int Length = B.length();
    if(A.length() > Length)
        Length = A.length();

    string CharA, CharB;

    for(int i = 0; i < Length; ++i)
    {
        if(i <= A.length())
            CharA = A.substr(i, 1);
        else
            CharA = " ";
        if(i <= B.length())
            CharB = B.substr(i, 1);
        else
            CharB = " ";
        if(Asc(CharA) < Asc(CharB)) return true;
        if(Asc(CharA) > Asc(CharB)) return false;
    }

    return true;
}

// Flashes the screen with a given colour
void ScreenFlash(int R, int G, int B, int TextureID, int Length, float InitialAlpha)
{
    if(TextureID < 65535)
    {
        uint Tex = GetTexture(TextureID);
        EntityTexture(FlashEN, Tex);
    }else
        EntityTexture(FlashEN, -1);
    
    SAQuadColor(FlashEN, R, G, B);
    EntityAlpha(FlashEN, InitialAlpha);
    FlashLength = ((float)Length) / InitialAlpha;
    FlashStart = MilliSecs() - (FlashLength - Length);
    Flashing = true;
}


// Updates the screen flash
void UpdateScreenFlash()
{
    
    int Time = MilliSecs() - FlashStart;
    if(Time >= FlashLength)
    {
        Flashing = false;
        EntityAlpha(FlashEN, 0.0f);
    }else
        EntityAlpha(FlashEN, 1.0f - (((float)Time) / ((float)FlashLength)));

}

string Money(int Amount)
{
    string Amount4, Amount3, Amount2, Amount1;

    if(RealmCrafter::Globals->Money4.length() > 0)
    {
        Amount4 = RealmCrafter::Globals->Money4 + string(": ") + toString(Amount / (RealmCrafter::Globals->Money4x * RealmCrafter::Globals->Money3x * RealmCrafter::Globals->Money2x)) + string(", ");
        Amount = Amount % (RealmCrafter::Globals->Money4x * RealmCrafter::Globals->Money3x * RealmCrafter::Globals->Money2x);
    }

    if(RealmCrafter::Globals->Money3.length() > 0)
    {
        Amount3 = RealmCrafter::Globals->Money3 + string(": ") + toString(Amount / (RealmCrafter::Globals->Money3x * RealmCrafter::Globals->Money2x)) + string(", ");
        Amount = Amount % (RealmCrafter::Globals->Money3x * RealmCrafter::Globals->Money2x);
    }

    if(RealmCrafter::Globals->Money2.length() > 0)
    {
        Amount2 = RealmCrafter::Globals->Money2 + string(": ") + toString(Amount / RealmCrafter::Globals->Money2x) + string(", ");
        Amount = Amount % RealmCrafter::Globals->Money2x;
    }

    Amount1 = RealmCrafter::Globals->Money1 + string(": ") + toString(Amount);

    return Amount4 + Amount3 + Amount2 + Amount1;
}

uint CreateCentredWindow(string Title, int W, int H, int Parent, int Flags, bool Hidden)
{
    uint Win;

    // Create initial centred window
    if(Hidden == false)
        Win = BBCreateWindow(Title, (BBClientWidth(Parent) / 2) - (W / 2), (BBClientHeight(Parent) / 2) - (H / 2), W, H, Parent, Flags);
    else
        Win = BBCreateWindow(Title, BBClientWidth(Parent) * 2, BBClientHeight(Parent) * 2, W, H, Parent, Flags);

    // Resize to make sure the client area has the fill size
    W = (W * 2) - BBClientWidth(Win);
    H = (H * 2) - BBClientHeight(Win);
    int X = (BBClientWidth(Parent) / 2) + (W / -2) + BBGadgetX(Parent);
    int Y = (BBClientHeight(Parent) / 2) + (H / -2) + BBGadgetY(Parent);
    BBSetGadgetShape(Win, X, Y, W, H);

    return Win;
}
