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
#pragma once

#include <d3dx9.h>
#include "Blitzplus.h"
#include <IGUIManager.h>
#include "..\SDK\Main\ASyncJobModule.h"
using namespace NGin::GUI;

#ifdef MessageBox
#undef MessageBox
#endif

//typedef unsigned int uint;

#define DLLIN extern "C" _declspec(dllimport)

extern int Shader_LitObject1 , Shader_LitObject2 , Shader_Terrain , Shader_SAQuad , Shader_FullbrightAlpha , Shader_FullbrightAdd, Shader_FullbrightMultiply ;
extern int Shader_Animated , Shader_Line, Shader_Sky, Shader_Cloud;
extern uint TemplateMesh_Box , TemplateMesh_Sphere ;

typedef void (GeneralCallbackFN)(void);
typedef void (RTCallbackFN)(int);
typedef void GeneralCallbackFNp;
typedef void RTCallbackFNp;
typedef void (ShadowMapCallbackFN)(const float*);
typedef void (ShadowMapVPCallbackFN)(const float*, const float*);

DLLIN int StartRemoteDebugging(const char* address, int port);
DLLIN void StartDebugTimer(const char* name);
DLLIN void StopDebugTimer(/*int id*/);
DLLIN void BeginDebugFrame();
DLLIN void EndDebugFrame();
DLLIN void DebugActor(int id, unsigned short sx, unsigned short sz, float x, float z, char me);

struct IOThreadInfo
{
	unsigned char Priority;
	unsigned int Start;
	unsigned int Length;
	char* Buffer;
	std::string Path;
	NGin::IEventHandler<void, IOThreadInfo>* EventHandler;
};

DLLIN ASyncFuncs* bbdx2_ASyncGetLib();

DLLIN bool IOThreadReadFile(const char* path, unsigned char priority, NGin::IEventHandler<void, IOThreadInfo>* eventHandler);
DLLIN bool IOThreadReadFileEx(const char* path, unsigned char priority, unsigned int start, unsigned int length, NGin::IEventHandler<void, IOThreadInfo>* eventHandler);

DLLIN uint LoadThreadMesh(const char* filename, uint parent, int Animated);
DLLIN uint LoadThreadTexture(const char* filename, int flags);

DLLIN void bbdx2_AllowRenderFrame(int allow);
DLLIN void bbdx2_SetInheritAnimation(int node, int inherit);
DLLIN int bbdx2_GetInheritAnimation(int node);

DLLIN void SetRenderGUICallback(int index, GeneralCallbackFNp* Fn);
DLLIN void SetRenderSolidCallback(int index, GeneralCallbackFNp* Fn);
DLLIN void SetRenderSolidCallbackRT(int index, RTCallbackFNp* Fn);
DLLIN void SetDeviceLostCallback(int index, GeneralCallbackFNp* Fn);
DLLIN void SetDeviceResetCallback(int index, GeneralCallbackFNp* Fn);
DLLIN void SetRenderShadowDepthCallback(int index, ShadowMapCallbackFN* Fn);
DLLIN void SetRenderShadowDepthVPCallback(int index, ShadowMapVPCallbackFN* Fn);

DLLIN void ghoop                 (int a,int b);
DLLIN void ackno                 (int a,int b,int c,int d,int e,int f,const char* g);
DLLIN void lickme                ();
DLLIN void qwedfy                (int a,int b,int c);
DLLIN int copliy                 ();
DLLIN int boompx                 ();
DLLIN void lovepixels            (int a,float b,float  c,float  d);
DLLIN void sukusul               (int a,float  b,float  c,float  d);
DLLIN void gabbama               (int a,float  b);
DLLIN void hoklig                (int a,int  b,int  c,int  d);
DLLIN void gonerum               (int a,int  b,int  c,int  d);
DLLIN void wareflog               (int a,int  b);
DLLIN void klopil                (int a,int  b);
DLLIN void lipphogg              (int a);
DLLIN void jewnjig               (int a);
DLLIN void gisranlo              (int a,int b,float c,int d);
DLLIN void jockgnome             (int a);
DLLIN int burryjimpol            (int a);
DLLIN int jalkming               (const char* a,int b, int c, bbdx2_ASyncJobFn completionCallback, void* userData);
DLLIN int pathlier               (int a);
DLLIN int pandamaka              (int a,int b);
DLLIN void laggysven             (int a,int b,int c,float d,int e);
DLLIN void tikamakis             (int a);
DLLIN int jacklingmo             (int a);
DLLIN int trexban                (int a,int b,int c);
DLLIN int carjacking             (int a);
DLLIN void chaosdigs             (int a,float b,float c,float d,int e);
DLLIN void rzrtool               (int a,float b,float c,float d,int e);
DLLIN void gobstoper             (int a,float b,float c,float d);
DLLIN void mingja                (int a,float b,float c,float d,int e);
DLLIN void moolad                (int a,float b,float c,float d);
DLLIN void jaffamak              (int a,float b,float c,float d,int e);
DLLIN float nottdos              (int a,int b);
DLLIN void babygoo               (int a,int b,float c);
DLLIN void chincrank             (int a,int b);
DLLIN void laghimout             ();
DLLIN void knowham               (int a,int b,int c,int d);
DLLIN void getmeshake            (int a);
DLLIN void localiva              (int a,int b);
DLLIN void makewow               (int a,float b);
DLLIN int deterkis               (const char* a);
DLLIN void slobing               (int a,int b);
DLLIN void bumofcow              (int a);
DLLIN void bumofcow2             (int a);
DLLIN void fluxcapa              (int a);
DLLIN int catonbox               (int a,int parent);
DLLIN float jingsu               (int a,int b);
DLLIN float sostrong             (int a,int b);
DLLIN float kisheadgone          (int a,int b);
DLLIN float tcpwnsall            (int a,int b);
DLLIN float habaki               (int a,int b);
DLLIN float kimono               (int a,int b);
DLLIN float kissme               (int a,int b);
DLLIN float lolatme              (int a,int b);
DLLIN float lambdin              (int a,int b);
DLLIN void missedit              (int a,float b);
DLLIN void chinook               (int a,float b,float c,float d,int e);
DLLIN float manonworld           (int Entity);
DLLIN float jumpin               (int Entity);
DLLIN float needsleep            (int Entity);
DLLIN char* setthefire           (int a);
DLLIN void softskin              (int a,float b,float c,float d);
DLLIN void setmedown             (int a,float b,float c,float d);
DLLIN void soreneck              (int a,float b,float c,float d);
DLLIN void reachout              (int a,float b,float c,float d,float e,float f,float g,int h);
DLLIN int andtakeit              (int a);
DLLIN void wannabe               (int a);
DLLIN int withoutyou             (int a,float b,float c,float d,float e,float f,float g);
DLLIN int tellme                 (int a,int b,int c,int d);
DLLIN int openeyes               (int a);
DLLIN void closeeyes             (int a, int b, int c, float d);
DLLIN int getupgetout            (int a,int b);
DLLIN int aboveandbeyond         (int a);
DLLIN int mindless               (int a);
DLLIN int penondesk              (int a,int b);
DLLIN int earthiswarm            (int a);
DLLIN void crystalclear          (int a,int b,int c);
DLLIN float laptop               (int a,int b);
DLLIN float watchingspace        (int a,int b);
DLLIN float garagedoor           (int a,int b);
DLLIN void waverider             (int a,int b,float c,float d,float e);
DLLIN void happyclap             (int a,int b,float c,float d);
DLLIN void urallnoobs            (int a,int b,int c,int d,int e,float f);
DLLIN void makemegame            (int a,float b,float c);
DLLIN int texttodate             (const char* a,int b);
DLLIN void smokinhot             (int a,int b,int c,int d,int e);
DLLIN void emokid                (int a);
DLLIN int jesusownzall           (int a,int b);
DLLIN void hardrock              (int a,float b,float c,float d,float e);
DLLIN void makemav               (int a,int b,int c,int d);
DLLIN void shogun                (int a,float b);
DLLIN void nofear                (int a,int b);
DLLIN void kevlarboobs           (int a,float b,float c);
DLLIN void goodoldkis            (int a,int b,int c,int d);
DLLIN void kickleaves            (int a,int b,int c,int d);
DLLIN void helplost              (float a,float b,float c,int d,int e);
DLLIN void kieransan             (float a,float b,float c,int d,int e);
DLLIN float itsnoteasy           ();
DLLIN float buymebeer            ();
DLLIN char* likeyoumean          (int a);

DLLIN void SetWaterLevel( float WaterHeight );

//DLLIN void anyonebutme           (int a,int b,int c,int d);
//DLLIN void ladygarden            (int a,int b);
//DLLIN int bedtime               (int a);
//DLLIN void perfecteyes           (int a,float b,float c);
//DLLIN void endless               (int a);
//DLLIN void goingon               (int a);
//DLLIN int aweosme                (int a);
//DLLIN float mindmusic            (int a,int b);
//DLLIN float magicphone           (int a,int b);
//DLLIN float crapcake             (int a,int b);
//DLLIN float denote               (int a,int b);
//DLLIN float pure                 (int a,int b);
//DLLIN float tubeloop             (int a,int b);
//DLLIN void picket                (int a,int b);
//DLLIN int fighting               (float a,float b,float c,float d,float e,float f);
//DLLIN int mgscool                (int a,int b,int c);
//DLLIN float neverland            ();
//DLLIN float loadgays             ();
//DLLIN float remindyou            ();
//DLLIN float chocolate            ();
//DLLIN float areyou               ();
//DLLIN float gingerbread          ();
DLLIN void anyoneelse            ();
DLLIN float firstdance           ();
DLLIN void commands              (int a,const char* b);
DLLIN void cones              (int a,const char* b);
DLLIN const char* ocones (int a);
DLLIN int brightwork             (int a);
DLLIN int grandchild             (int a);
DLLIN int newfangled             (int a);
DLLIN float jellygraph           (int a);
DLLIN void jackhammer            (int a,int b);
DLLIN void jabberwock            (int a);
DLLIN void witchcraft            (int a,int b,int c);
DLLIN int pawnbroker             (int a);
DLLIN int thumbprint             ();
DLLIN int motorcycle             (int a);
DLLIN float fightening          (int a,int b);
DLLIN void factoid               (int a,int b,int c,int d,int e);
DLLIN void fathom                ();
DLLIN void recent                (int a,float b);
DLLIN int growth                 (int a,int b,int c);
DLLIN int rendering              (int a, const char* b);
DLLIN void performance           (int a);
DLLIN int rampant                (int a,int b);
DLLIN int desktopup              (int a);
DLLIN int pixelpushing           (int a,int b,int c);
DLLIN int grunt                  (int a);
DLLIN int military               (int a);
DLLIN int flight                 ();
DLLIN int simulators             ();
DLLIN int fiveyears              (const char* a,int b);
DLLIN int enoughpower            (const char* a,int b);
DLLIN int onour                  (int a);
DLLIN int desktops               (int a);
DLLIN int towipe                 (int a);
DLLIN int outa                   (int a);
DLLIN void hollyclass            (int a,int b);
DLLIN void withluck              (int a,int b);
DLLIN void weable                (int a,float b,float c);
DLLIN float shelveall              (int a,int b,int c);
DLLIN void thoseghastly          (int a,float b);
DLLIN void preening              (int a,float b,float c);
DLLIN void actorsand             (int a,int b,int c);
DLLIN void dothe                 (int a,int b);
DLLIN float wholejob             (int a,int b,int c);
DLLIN void digitally             (int a,int b);
DLLIN void updates               (int a,int b);
DLLIN float releases               (int a);
DLLIN float revelations            (int a);
DLLIN char* onceout              (int a);
DLLIN void twoversions           (float a,float b,float c,int d,int e);
DLLIN int evevista               (int a,int b,int c);
DLLIN void eveclassic            (int a);
DLLIN float supportdx            (int a,int b);
DLLIN int excitingtimes          (int a,int b);
DLLIN int concerned              (int a,int b);
DLLIN int broughtyou             (int a,int b);
DLLIN void groundbreaking        (int a,int b,float c,float d,float e);
DLLIN float unprecedented        (int a,int b);
DLLIN float processing           (int a,int b);
DLLIN float efficiency           (int a,int b);
DLLIN int EntityInView           (int a);
DLLIN void PointProject          (float x,float y,float z);
DLLIN void EntityProject         (int a);
DLLIN float ProjectedX           ();
DLLIN float ProjectedY           ();
DLLIN float ProjectedZ           ();
DLLIN char* TexNameFromSurf      (int a,int b,int c);
DLLIN void GrabTexture            (int a);
DLLIN int CopyMesh               (int a);
DLLIN int CreateSAText           (int Shader,int  Font);
DLLIN void SetSAText             (int Node, const char* Text);
DLLIN int LoadSAFont             (const char* FontFace, int Fontsize, int Bold, int Italic);
DLLIN void FreeSAFont            (int Font);
DLLIN void SetSAInlineProcessing (int Node, int Enabled);
DLLIN void SetScale              (int Entity, float Width, float Height);
DLLIN float GetSATextWidth       (int Entity, const char* Text);
DLLIN void SetTexCoords          (int Entity, float MinU, float MinV, float MaxU, float MaxV);
DLLIN void SetPosition           (int Quad, float X, float Y);
DLLIN void SetScale              (int Quad, float X, float Y);
DLLIN int CountGraphicsModes     ();
DLLIN int GraphicsModeWidth      (int Index);
DLLIN int GraphicsModeHeight     (int Index);
DLLIN void DebugOut              (const char* Msg);
DLLIN void ShadowDistance        (float Distance);
DLLIN void ShadowLevel           (int Level);
DLLIN void ShadowShader          (int Shader);
DLLIN void LightDistance         (float Distance);
DLLIN int CreatePPEffect         (int Shader);
DLLIN void getmeshake				(int Entity);
DLLIN float manonworld				(int Entity);
DLLIN float jumpin				(int Entity);
DLLIN float needsleep				(int Entity);
//DLLIN void EntityBox(int Entity, float X, float Y, float Z, float Width, float Height, float Depth);
DLLIN void BChannelVolume(uint Chn, float Lev);
DLLIN void BFreeSound(uint Snd);
DLLIN void BLoopSound(uint Snd);
DLLIN void BSoundVolume(uint Snd, float Lev);
DLLIN void BStopChannel(uint Chn);
DLLIN void BPauseChannel(uint Chn);
DLLIN void BResumeChannel(uint Chn);
DLLIN void BChannelPan(uint Chn, float Pan);
DLLIN void BCreateListener(uint Entity, float Roll, float Droppler, float Disance);
DLLIN int BLoadSound(const char* Filename);
DLLIN int BPlaySound(uint Snd);
DLLIN int BPlayMusic(const char* Filename);
DLLIN int BLoad3DSound(const char* Filename);
DLLIN int BEmitSound(uint Snd, uint Entity);
DLLIN int BChannelPlaying(uint Chn);
DLLIN void PointProject(float X, float Y, float Z);
DLLIN float GetMatElement(uint Entity, int Row, int Collumn);
DLLIN void WriteSceneGraph(const char* FileName);
DLLIN int CountMultipleSamples(int Windowed);
DLLIN void SetAlphaState(int Object, bool Mode);
DLLIN void EntityConstantFloat( uint Entity, const char* Name, float V0);
DLLIN void EntityConstantFloat2(uint Entity, const char* Name, float V0, float V1);
DLLIN void EntityConstantFloat3(uint Entity, const char* Name, float V0, float V1, float V2);
DLLIN void EntityConstantFloat4(uint Entity, const char* Name, float V0, float V1, float V2, float V3);
DLLIN void GlobalShaderConstantFloat4(const char* Name, float V0, float V1, float V2, float V3);
DLLIN int GetMaterialCount(uint Entity);
DLLIN void LoadPPChain(const char* File);
DLLIN void SetPPFolder(const char* Dir);
DLLIN void SetPPActive(const char* Tag, int Active);
DLLIN int GetPPActive(const char* Tag);
DLLIN IDirect3DDevice9* bbdx2_GetIDirect3DDevice9();
DLLIN void* bbdx2_GetNxScene();
DLLIN void* bbdx2_GetNxPhysicsSDK();
DLLIN void CalculateB3DTangents(int Entity);
DLLIN uint CreateLine(uint parent);
DLLIN void FreeLine(uint line);
DLLIN void SetLineSize(uint line, float sx, float sy, float sz, float ex, float ey, float ez);
DLLIN void SetLineColor(uint line, int r, int g, int b);
DLLIN void SetLineVisible(uint line, bool visible);
DLLIN uint LoadProfile(const char* Filename);
DLLIN uint CreateProfile(const char* Name);
DLLIN void FreeProfile(uint Profile);
DLLIN void SetProfileRange(uint Profile, float Range);
DLLIN float GetProfileRange(uint Profile);
DLLIN void SetProfileEffect(uint Profile, int QualityLevel, int Distance, uint Effect);
DLLIN uint GetProfileEffect(uint Profile, int QualityLevel, int Distance);
DLLIN void EntityProfile(int Entity, int Profile);
DLLIN void SetQualityLevel(int QualityLevel);
DLLIN void GetQualityLevel();

DLLIN int GetParameterCount(int Effect);
DLLIN const char* GetParameterName(int Effect, int ID);
DLLIN int GetParameterType(int Effect, int ID);
DLLIN int GetAnnotationCount(int Effect, int ID);
DLLIN const char* GetAnnotationName(int Effect, int eID, int aID);
DLLIN int GetAnnotationType(int Effect, int eID, int aID);
DLLIN int GetSizeOfType(int Type);
DLLIN int GetParameterData(int Effect, int ID, void* Data);
DLLIN int GetAnnotationData(int Effect, int eID, int aID, void* Data);
DLLIN void* GetNodeParameterValue(int Node, const char* ParamName, int &outType);
DLLIN int GetNodeParameterCount(int Node);
DLLIN char * GetNodeParameterName(int Node, int iParam);
DLLIN void ResetNodeParameters(int Node);

DLLIN void* bbdx2_GetViewMatrixPtr();
DLLIN void* bbdx2_GetProjectionMatrixPtr();
DLLIN void bbdx2_FreeMatrixPtr(void* M);
DLLIN void** bbdx2_GetPointLights(int* Count, unsigned int* Stride);
DLLIN void bbdx2_GetDirectionalLights(float** Directions, float** Colors);
//DLLIN void bbdx2_InjectCollisionMesh(uint Node, float* Triangles, unsigned int Count);
DLLIN void bbdx2_SetTextureAlpha(uint Texture);
DLLIN void bbdx2_ProjectVector3(D3DXVECTOR3* Point, D3DXVECTOR3* Out);
DLLIN void bbdx2_UnProjectVector3(D3DXVECTOR3* Point, D3DXVECTOR3* Out);
DLLIN float bbdx2_GetFogNear();
DLLIN float bbdx2_GetFogFar();
DLLIN uint bbdx2_GetFogColor();

DLLIN void bbdx2_SetupPhysX();
DLLIN void bbdx2_Collisions(int source, int destination);
DLLIN void bbdx2_SetNxType(uint node);
DLLIN void bbdx2_EntityType(uint node, int type);
DLLIN int bbdx2_GetEntityType(uint node);
DLLIN void bbdx2_FreeCollisionInstance(uint node);
DLLIN void bbdx2_SetSlopeRestriction(float restriction);
DLLIN void bbdx2_EntityRadius(uint node, float x, float y);
DLLIN void bbdx2_ResetEntity(uint node);
DLLIN void bbdx2_EntityBox(uint node, float x, float y, float z, float w, float h, float d);
DLLIN void bbdx2_ActorMove(uint node);
DLLIN void bbdx2_UpdateWorld();
DLLIN void bbdx2_SetCollisionMesh(uint node);
DLLIN void bbdx2_InjectCollisionMesh(uint node, float* triangleList, unsigned int vertexCount);
DLLIN void bbdx2_ASyncInjectCollisionMesh(uint node, float* triangleList, unsigned int vertexCount, int highPriority);
DLLIN void bbdx2_ASyncCancelInject(uint node);

DLLIN void bbdx2_PhysXMoveEntity(uint node, float x, float y, float z);

DLLIN void bbdx2_EntityPickMode(uint node, int picktype);
DLLIN uint bbdx2_LinePick(float startX, float startY, float startZ, float endX, float endY, float endZ, float radius);
DLLIN uint bbdx2_CameraPick(int x, int y);

DLLIN float bbdx2_PickedX();
DLLIN float bbdx2_PickedY();
DLLIN float bbdx2_PickedZ();
DLLIN float bbdx2_PickedNX();
DLLIN float bbdx2_PickedNY();
DLLIN float bbdx2_PickedNZ();

DLLIN int bbdx2_CountCollisions(uint node);

DLLIN float bbdx2_CollisionX(uint node, int index);
DLLIN float bbdx2_CollisionY(uint node, int index);
DLLIN float bbdx2_CollisionZ(uint node, int index);
DLLIN float bbdx2_CollisionNX(uint node, int index);
DLLIN float bbdx2_CollisionNY(uint node, int index);
DLLIN float bbdx2_CollisionNZ(uint node, int index);

DLLIN uint bbdx2_CreatePhysicsDesc(int isStatic, uint node);
DLLIN void bbdx2_AddSphere(uint desc, float x, float y, float z, float radius, float mass);
DLLIN void bbdx2_AddCapsule(uint desc, float x, float y, float z, float width, float height, float mass);
DLLIN void bbdx2_AddBox(uint desc, float x, float y, float z, float width, float height, float depth, float mass);
DLLIN void bbdx2_ClosePhysicsDesc(uint desc);

DLLIN int CountIndices(uint Surface);
DLLIN int GetIndex(uint Surface, int Index);
DLLIN float VertexU(uint Surface, int Index);
DLLIN float VertexV(uint Surface, int Index);

DLLIN void GetAnimatedBoundingBox(uint node, float* minX, float* minY, float* minZ, float* maxX, float* maxY, float* maxZ);
DLLIN void bbdx2_ReloadShaders();

DLLIN float ShadowBlurShader(uint blurH, uint blurV);
DLLIN void EntityShadowLevel(uint entity, int level);
DLLIN int GetEntityShadowLevel(uint entity);
DLLIN void EntityShadowShader(uint node, uint shader);
DLLIN uint GetEntityShadowShader(uint node);
DLLIN void SetShadowMapSize(int newSize);
DLLIN int GetShadowMapSize();
DLLIN const float* GetLightMatrix();
DLLIN void* GetShadowMap();

//////////////////////////////////////////////////////////////////////////
// Post Process
//////////////////////////////////////////////////////////////////////////
DLLIN void LoadUserDefinedPP_FromXML( const char* xmlName );

DLLIN void AddPP_Effect( const char* EffectName, const char* ShaderSource );
DLLIN void SwapPP_Effect( int indexPP0, int indexPP1 );
DLLIN void DeletePP_Effect( int IndexPP );

DLLIN void AddEffect_Param( const char* Effect, const char* Param, const char* Type, const char* Value );
DLLIN void SetEffect_Param( const char* Effect, const char* Param, const char* Value );

DLLIN void EnablePP_Pipeline( bool state );
DLLIN void CleanPP_Pipeline();
DLLIN bool SetPP_Pipeline( const char* PP_Name );
 
DLLIN int bbdx2_GetAmbientR();
DLLIN int bbdx2_GetAmbientG();
DLLIN int bbdx2_GetAmbientB();

enum ShaderType
{
	ST_Void,
	ST_Bool,
	ST_Int,
	ST_Float,
	ST_Float2,
	ST_Float3,
	ST_Float4,
	ST_Float3x2,
	ST_Float3x3,
	ST_Float4x4,
	ST_String,
	ST_Unknown
};


//DLLIN void bbdx2_SetGUIEffectPath(const char* Path);
//DLLIN IGUIManager* bbdx2_GetGUIManager();

//inline IGUIManager* GetGUIManager()
//{
//	return bbdx2_GetGUIManager();
//}
//
//inline void SetGUIEffectPath(NGin::string Path)
//{
//	bbdx2_SetGUIEffectPath(Path.c_str());
//}

inline IDirect3DDevice9* GetIDirect3DDevice9()
{
	return bbdx2_GetIDirect3DDevice9();
}

inline void SAQuadPosition(uint Quad, float X, float Y)
{
	SetPosition(Quad, X, Y);
}


inline void SAQuadScale(uint Quad, float X, float Y)
{
	SetScale(Quad, X, Y);
}

inline float SATextWidth(uint Quad, std::string Text)
{
	return GetSATextWidth(Quad, Text.c_str());
}

inline void SATextureCoords(uint Quad, float MinU, float MinV, float MaxU, float MaxV)
{
	SetTexCoords(Quad, MinU, MinV, MaxU, MaxV);
}

inline bool ChannelPlaying(uint Chn)
{
	return (BChannelPlaying(Chn) > 0) ? true : false;
}

inline uint EmitSound(uint Snd, uint Entity)
{
	return BEmitSound(Snd, Entity);
}

inline uint PlaySound(uint Snd)
{
	return BPlaySound(Snd);
}

inline uint LoadSound(std::string Filename)
{
	return BLoadSound(Filename.c_str());
}

inline uint Load3DSound(std::string Filename)
{
	return BLoad3DSound(Filename.c_str());
}


inline uint PlayMusic(std::string Filename)
{
	return BPlayMusic(Filename.c_str());
}


inline void ChannelVolume(uint Chn, float Level)
{
	BChannelVolume(Chn, Level);
}


inline void FreeSound(uint Snd)
{
	BFreeSound(Snd);
}


inline void LoopSound(uint Snd)
{
	BLoopSound(Snd);
}

inline void SoundVolume(uint Snd, float Level)
{
	BSoundVolume(Snd, Level);
}

inline void StopChannel(uint Chn)
{
	BStopChannel(Chn);
}

inline void PauseChannel(uint Chn)
{
	BPauseChannel(Chn);
}

inline void ResumeChannel(uint Chn)
{
	BResumeChannel(Chn);
}

inline void ChannelPan(uint Chn, float Pan)
{
	BChannelPan(Chn, Pan);
}

inline void CreateListener(uint Entity, float Roll, float Doppler, float Distance)
{
	BCreateListener(Entity, Roll, Doppler, Distance);
}




inline void Init(int Wnd, int=0)
{
	ghoop(Wnd, 0);
}

void Graphics3D(int Width, int Height, int Depth, int Fullscreen, int AntiAlias, int, std::string DefaultTexture);
void LoadDefaultShaders();

inline void FreeEntity(uint Entity)
{
	getmeshake(Entity);
}

void End3DGraphics();

inline float MeshWidth(uint Entity)
{
	return manonworld(Entity);
}

inline float MeshHeight(uint Entity)
{
	return jumpin(Entity);
}

inline float MeshDepth(uint Entity)
{
	return needsleep(Entity);
}



inline void AmbientLight(int r, int g, int b)
{
	qwedfy(r, g, b);
}

inline uint CreatePointLight()
{
	return copliy();
}

inline int CreateDirectionalLight()
{
	OutputDebugString("CreateDirectionalLight()\n");
	return boompx();
}

inline void SetLightPosition(uint Light, float X, float Y, float Z)
{
	lovepixels(Light, X, Y, Z);
}

inline void SetLightDirection(uint Light, float X, float Y, float Z)
{
	sukusul(Light, X, Y, Z);
}

inline void SetLightRadius(uint Light, float Radius)
{
	gabbama(Light, Radius);
}

inline void SetPLightColor(uint Light, int R, int G, int B)
{
	hoklig(Light, R, G, B);
}

inline void SetDLightColor(uint Light, int R, int G, int B)
{
	gonerum(Light, R, G, B);
}

inline void SetPLightActive(uint Light, bool Active)
{
	wareflog(Light, Active ? 1 : 0);
}

inline void SetDLightActive(uint Light, bool Active)
{
	klopil(Light, Active ? 1 : 0);
}

inline void FreePLight(uint Light)
{
	lipphogg(Light);
}

inline void FreeDLight(uint Light)
{
	jewnjig(Light);
}

inline void Animate(uint Entity, int Mode, float Speed = 1.0f, uint Sequence = 0, float Tween = 0.0f)
{
	gisranlo(Entity, Mode, Speed, Sequence);
}

inline void UpdateHardwareBuffers(uint Entity)
{
	jockgnome(Entity);
}

// iLOD (0..2) HIGH, MEDIUM, LOW
inline void setMeshLOD(uint _Handle, uint handle, int iLOD, float Distance )
{
	closeeyes(_Handle, handle, iLOD, Distance);
}

inline uint CreateCamera(uint Parent = 0)
{
	return burryjimpol(Parent);
}

uint LoadMesh(std::string Filename, uint Parent = 0, bool Animated = false, bbdx2_ASyncJobFn completionCallback = NULL, void* userData = NULL);

inline uint CreateMesh(uint Parent = 0)
{
	return pathlier(Parent);
}

inline uint CreatePivot(uint Parent = 0)
{
	return carjacking(Parent);
}

inline void PositionEntity(uint Entity, float X, float Y, float Z, bool Global = false)
{
	chaosdigs(Entity, X, Y, Z, Global);
}

inline void TranslateEntity(uint Entity, float X, float Y, float Z, bool Global = false)
{
	rzrtool(Entity, X, Y, Z, Global);
}

inline void RotateEntity(uint Entity, float X, float Y, float Z, bool Global = false)
{
	mingja(Entity, X, Y, Z, Global);
}

inline void ScaleEntity(uint Entity, float X, float Y, float Z, bool Global = false)
{
	jaffamak(Entity, X, Y, Z, Global);
}

inline void MoveEntity(uint Entity, float X, float Y, float Z)
{
	gobstoper(Entity, X, Y, Z);
}

inline void TurnEntity(uint Entity, float X, float Y, float Z)
{
	moolad(Entity, X, Y, Z);
}

inline float EntityDistance(uint EntityA, uint EntityB)
{
	return nottdos(EntityA, EntityB);
}

inline void PointEntity(uint Entity, uint Target, float Ignored = 0.0f)
{
	babygoo(Entity, Target, Ignored);
}

inline void EntityOrder(uint Entity, int Order)
{
	chincrank(Entity, Order);
}	

inline void RenderWorld()
{
	laghimout();
}

inline void CameraClsColor(uint Camera, int r, int g, int b)
{
	knowham(Camera, r, g, b);
}

inline void EntityFX(uint Entity, int FX)
{
	localiva(Entity, FX);
}

inline void EntityAlphaNoSolid(uint Entity, float Alpha)
{
	makewow(Entity, Alpha);
}

inline uint LoadShader(std::string File)
{
	return deterkis(File.c_str());
}

inline void EntityShader(uint Entity, uint Shader)
{
	return slobing(Entity, Shader);
}

inline void HideEntity(uint Entity, bool DisableCollisions = true)
{
	if(DisableCollisions)
		bumofcow(Entity);
	else
		bumofcow2(Entity);
}

inline void ShowEntity(uint Entity)
{
	fluxcapa(Entity);
}

inline uint CopyEntity(uint Entity, uint Parent = 0)
{
	return catonbox(Entity, Parent);
}

inline float EntityX(uint Entity, bool Global = false)
{
	return jingsu(Entity, Global ? 1 : 0);
}


inline float EntityY(uint Entity, bool Global = false)
{
	return sostrong(Entity, Global ? 1 : 0);
}


inline float EntityZ(uint Entity, bool Global = false)
{
	return kisheadgone(Entity, Global ? 1 : 0);
}


inline float EntityScaleX(uint Entity, bool Global = false)
{
	return tcpwnsall(Entity, Global ? 1 : 0);
}


inline float EntityScaleY(uint Entity, bool Global = false)
{
	return habaki(Entity, Global ? 1 : 0);
}


inline float EntityScaleZ(uint Entity, bool Global = false)
{
	return kimono(Entity, Global ? 1 : 0);
}


inline float EntityPitch(uint Entity, bool Global = false)
{
	return kissme(Entity, Global ? 1 : 0);
}


inline float EntityYaw(uint Entity, bool Global = false)
{
	return lolatme(Entity, Global ? 1 : 0);
}


inline float EntityRoll(uint Entity, bool Global = false)
{
	return lambdin(Entity, Global ? 1 : 0);
}

inline void EntityAlpha(uint Entity, float Alpha)
{
	missedit(Entity, Alpha);
}

inline void AlignToVector(uint Entity, float X, float Y, float Z, int Axis)
{
	chinook(Entity, X, Y, Z, Axis);
}

inline std::string EntityClass(uint Entity)
{
	return std::string(setthefire(Entity));
}

inline void PositionMesh(uint Entity, float X, float Y, float Z)
{
	softskin(Entity, X, Y, Z);
}

inline void RotateMesh(uint Entity, float X, float Y, float Z)
{
	setmedown(Entity, X, Y, Z);
}

inline void ScaleMesh(uint Entity, float X, float Y, float Z)
{
	soreneck(Entity, X, Y, Z);
}

inline void FitMesh(uint Entity, float X, float Y, float Z, float Width, float Height, float Scale, bool Uniform)
{
	reachout(Entity, X, Y, Z, Width, Height, Scale, Uniform ? 1 : 0);
}

inline uint CreateSurface(uint Mesh)
{
	return andtakeit(Mesh);
}

inline void ClearSurface(uint Surface)
{
	wannabe(Surface);
}

inline uint AddVertex(uint Surface, float X, float Y, float Z, float U = 0.0f, float V = 0.0f, float W = 0.0f)
{
	return withoutyou(Surface, X, Y, Z, U, V, W);
}

inline uint AddTriangle(uint Surface, uint V0, uint V1, uint V2)
{
	return tellme(Surface, V0, V1, V2);
}

inline uint CountSurfaces(uint Entity)
{
	return openeyes(Entity);
}

inline uint GetSurface(uint Entity, uint Index)
{
	return getupgetout(Entity, Index);
}

inline uint CountVertices(uint Entity)
{
	return aboveandbeyond(Entity);
}

inline uint CountChildren(uint Entity)
{
	return mindless(Entity);
}

inline uint GetChild(uint Entity, uint Index)
{
	return penondesk(Entity, Index);
}

inline uint GetParent(uint Entity)
{
	return earthiswarm(Entity);
}

inline void EntityParent(uint Entity, uint Parent, bool Global = true)
{
	crystalclear(Entity, Parent, Global ? 1 : 0);
}

inline float VertexX(uint Surface, uint Index)
{
	return laptop(Surface, Index);
}

inline float VertexY(uint Surface, uint Index)
{
	return watchingspace(Surface, Index);
}

inline float VertexZ(uint Surface, uint Index)
{
	return garagedoor(Surface, Index);
}

inline void VertexCoords(uint Surface, uint Index, float X, float Y, float Z)
{
	waverider(Surface, Index, X, Y, Z);
}

inline void VertexTexCoords(uint Surface, uint Index, float U, float V)
{
	happyclap(Surface, Index, U, V);
}

inline void VertexColor(uint Surface, uint Index, int R, int G, int B, float Alpha)
{
	urallnoobs(Surface, Index, R, G, B, Alpha);
}

inline void CameraRange(uint Camera, float Near, float Far)
{
	makemegame(Camera, Near, Far);
}

inline uint LoadTexture(std::string Filename, uint Flags = 0)
{
	return texttodate(Filename.c_str(), Flags);
}

inline void EntityTexture(uint Entity, uint Texture, int Index = 0, int Surface = -1)
{
	smokinhot(Entity, Texture, 0, Index, Surface);
}

inline void FreeTexture(uint Texture)
{
	emokid(Texture);
}




inline void SAQuadLayout(uint Quad, float X, float Y, float Width, float Height)
{
	hardrock(Quad, X, Y, Width, Height);
}

inline void SAQuadColor(uint Quad, int R, int G, int B)
{
	makemav(Quad, R, G, B);
}

inline void SAQuadAlpha(uint Quad, float Alpha)
{
	shogun(Quad, Alpha);
}

inline void CameraFogMode(uint Camera, int Mode)
{
	nofear(Camera, Mode);
}

inline void CameraFogRange(uint Camera, float Near, float Far)
{
	kevlarboobs(Camera, Near, Far);
}

inline void CameraFogColor(uint Camera, int R, int G, int B)
{
	goodoldkis(Camera, R, G, B);
}

inline void EntityColor(uint Entity, int R, int G, int B)
{
	kickleaves(Entity, R, G, B);
}

inline void TFormVector(float X, float Y, float Z, uint W1, uint W2)
{
	helplost(X, Y, Z, W1, W2);
}

inline void TFormPoint(float X, float Y, float Z, uint W1, uint W2)
{
	kieransan(X, Y, Z, W1, W2);
}

inline float TFormedX()
{
	return itsnoteasy();
}

inline float TFormedY()
{
	return buymebeer();
}

inline float TFormedZ()
{
	return firstdance();
}

inline std::string EntityName(uint Entity)
{
	return likeyoumean(Entity);
}

inline void Collisions(int SrcType, int DestType, int Method, int Response)
{
	//anyonebutme(SrcType, DestType, Method, Response);
	bbdx2_Collisions(SrcType, DestType);
}

inline void EntityType(uint Entity, int Type)
{
	//ladygarden(Entity, Type);
	bbdx2_EntityType(Entity, Type);
}

inline int GetEntityType(uint Entity)
{
	//return bedtime(Entity);
	return bbdx2_GetEntityType(Entity);
}

inline void EntityRadius(uint Entity, float Width, float Height = -1.0f)
{
	if(Height ==  -1.0f)
		Height = Width;
	//perfecteyes(Entity, Width, Height);
	bbdx2_EntityRadius(Entity, Width, Height);
}

inline void SetCollisionMesh(uint Entity)
{
	//endless(Entity);
	bbdx2_SetCollisionMesh(Entity);
}

inline void ResetEntity(uint Entity)
{
	//goingon(Entity);
	//OutputDebugString("Warning: ResetEntity not implemented!\n");
	//#pragma message("Warning: ResetEntity is not implemented!")
	bbdx2_ResetEntity(Entity);
}

inline int CountCollisions(uint Entity)
{
	//return aweosme(Entity);
	return bbdx2_CountCollisions(Entity);
}

inline float CollisionX(uint Entity, int Index)
{
	//return mindmusic(Entity, Index);
	return bbdx2_CollisionX(Entity, Index);
}


inline float CollisionY(uint Entity, int Index)
{
	//return magicphone(Entity, Index);
	return bbdx2_CollisionY(Entity, Index);
}


inline float CollisionZ(uint Entity, int Index)
{
	//return crapcake(Entity, Index);
	return bbdx2_CollisionZ(Entity, Index);
}


inline float CollisionNX(uint Entity, int Index)
{
	//return denote(Entity, Index);
	return bbdx2_CollisionNX(Entity, Index);
}


inline float CollisionNY(uint Entity, int Index)
{
	//return pure(Entity, Index);
	return bbdx2_CollisionNY(Entity, Index);
}


inline float CollisionNZ(uint Entity, int Index)
{
	//return tubeloop(Entity, Index);
	return bbdx2_CollisionNZ(Entity, Index);
}

inline void EntityPickMode(uint Entity, int Mode)
{
	//picket(Entity, Mode);
	bbdx2_EntityPickMode(Entity, Mode);
}

inline uint LinePick(float X, float Y, float Z, float DestX, float DestY, float DestZ, float Radius = 0.0f)
{
	//return fighting(X, Y, Z, DestX, DestY, DestZ);
	return bbdx2_LinePick(X, Y, Z, DestX, DestY, DestZ, Radius);
}

inline uint CameraPick(uint Camera, int X, int Y)
{
	//return mgscool(Camera, X, Y);
	return bbdx2_CameraPick(X, Y);
}

inline float PickedX()
{
	//return neverland();
	return bbdx2_PickedX();
}

inline float PickedY()
{
	//return loadgays();
	return bbdx2_PickedY();
}

inline float PickedZ()
{
	//return remindyou();
	return bbdx2_PickedZ();
}

inline float PickedNX()
{
	//return chocolate();
	return bbdx2_PickedNX();
}

inline float PickedNY()
{
	//return areyou();
	return bbdx2_PickedNY();
}

inline float PickedNZ()
{
	//return gingerbread();
	return bbdx2_PickedNZ();
}

inline void UpdateWorld()
{
	anyoneelse();
}

inline void NameEntity(uint Entity, std::string Name)
{
	commands(Entity, Name.c_str());
}

inline void TagEntity(uint Entity, std::string Tag)
{
	cones(Entity, Tag.c_str());
}

inline std::string EntityTag(uint Entity)
{
	return ocones(Entity);
}

inline bool Animating(uint Entity)
{
	return (brightwork(Entity) > 0) ? true : false;
}

inline int AnimLength(uint Entity)
{
	return grandchild(Entity);
}

inline int AnimSeq(uint Entity)
{
	return newfangled(Entity);
}

inline float AnimTime(uint Entity)
{
	return jellygraph(Entity);
}

inline void CameraProjMode(uint Camera, int Mode)
{
	jackhammer(Camera, Mode);
}

inline void ClsColor(int R, int G, int B)
{
	witchcraft(R, G, B);
}

inline uint CopyTexture(uint OldTexture)
{
	return pawnbroker(OldTexture);
}

inline int CountGfxModes3D()
{
	return thumbprint();
}

inline uint CountTriangles(uint Surface)
{
	return motorcycle(Surface);
}

inline float DeltaYaw(uint SrcEntity, uint DestEntity)
{
	return fightening(SrcEntity, DestEntity);
}

inline void End()
{
	fathom();
}

inline uint ExtractAnimSeq(uint Entity, int Start, int End)
{
	return growth(Entity, Start, End);
}

inline uint FindChild(uint Entity, std::string Name)
{
	return rendering(Entity, Name.c_str());
}

inline void FlipMesh(uint Entity)
{
	performance(Entity);
}

inline uint GetMeshFromThread(uint Thread)
{
	return desktopup(Thread);
}

inline int GraphicsHeight()
{
	return flight();
}

inline int GraphicsWidth()
{
	return simulators();
}

inline void PaintSurface(uint Surface, uint Brush)
{
	withluck(Surface, Brush);
}

inline void PositionTexture(uint Texture, float U, float V)
{
	weable(Texture, U, V);
}

inline void RotateTexture(uint Texture, float Rotate)
{
	thoseghastly(Texture, Rotate);
}

inline void ScaleTexture(uint Texture, float X, float Y)
{
	preening(Texture, X, Y);
}

inline float QueryBox(uint Entity, int Index, int XYZ)
{
	return shelveall(Entity, Index, XYZ);
}

inline float TextureWidth(uint Texture)
{
	return releases(Texture);
}

inline float TextureHeight(uint Texture)
{
	return revelations(Texture);
}

inline std::string TextureName(uint Texture)
{
	return onceout(Texture);
}

inline void TFormNormal(float X, float Y, float Z, uint W1, uint W2)
{
	twoversions(X, Y, Z, W1, W2);
}

inline uint TriangleVertex(uint Buffer, int Index, int Corner)
{
	return evevista(Buffer, Index, Corner);
}

inline void UpdateNormals(uint Entity)
{
	return eveclassic(Entity);
}

inline float VertexAlpha(uint Surface, int Index)
{
	return supportdx(Surface, Index);
}

inline int VertexRed(uint Surface, int Index)
{
	return broughtyou(Surface, Index);
}

inline int VertexGreen(uint Surface, int Index)
{
	return concerned(Surface, Index);
}

inline int VertexBlue(uint Surface, int Index)
{
	return excitingtimes(Surface, Index);
}

inline void VertexNormal(uint Surface, int Index, float NX, float NY, float NZ)
{
	groundbreaking(Surface, Index, NX, NY, NZ);
}

inline float VertexNX(uint Surface, int Index)
{
	return unprecedented(Surface, Index);
}

inline float VertexNY(uint Surface, int Index)
{
	return processing(Surface, Index);
}

inline float VertexNZ(uint Surface, int Index)
{
	return efficiency(Surface, Index);
}

inline std::string SurfaceTexture(uint Entity, int TextureIndex, int SurfaceIndex)
{
	return TexNameFromSurf(Entity, TextureIndex, SurfaceIndex);
}

inline uint CreateSphere(uint Parent = 0)
{
	return CopyEntity(TemplateMesh_Sphere, Parent);
}

inline uint CreateCube(uint Parent = 0)
{
	return CopyEntity(TemplateMesh_Box, Parent);
}

inline uint CreateSAQuad(uint Parent = 0)
{
	uint EN = jesusownzall(Parent, Shader_SAQuad);
	char Str[24];sprintf(Str, "%i", EN);
	NameEntity(EN, Str);
	return EN;
}



