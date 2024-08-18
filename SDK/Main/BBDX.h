//##############################################################################################################################
// Realm Crafter Professional SDK																								
// Copyright (C) 2010 Solstar Games, LLC. All rights reserved																	
// contact@solstargames.com																																																		
//																																																																#
// Programmer: Jared Belkus																										
//																																
// This is a licensed product:
// BY USING THIS SOURCECODE, YOU ARE CONFIRMING YOUR ACCEPTANCE OF THE SOFTWARE AND AGREEING TO BECOME BOUND BY THE TERMS OF 
// THIS AGREEMENT. IF YOU DO NOT AGREE TO BE BOUND BY THESE TERMS, THEN DO NOT USE THE SOFTWARE.
//																		
// Licensee may NOT: 
//  (i)   create any derivative works of the Engine, including translations Or localizations, other than Games;
//  (ii)  redistribute, encumber, sell, rent, lease, sublicense, Or otherwise transfer rights to the Engine; or
//  (iii) remove Or alter any trademark, logo, copyright Or other proprietary notices, legends, symbols Or labels in the Engine.
//  (iv)  licensee may Not distribute the source code Or documentation To the engine in any manner, unless recipient also has a 
//        license To the Engine.													
//  (v)  use the Software to develop any software or other technology having the same primary function as the Software, 
//        including but not limited to using the Software in any development or test procedure that seeks to develop like 
//        software or other technology, or to determine if such software or other technology performs in a similar manner as the
//        Software																																
//##############################################################################################################################
#pragma once

#include <d3d9.h>
#include <string>
#include "ASyncJobModule.h"

#ifndef BB_UINT_DEF
typedef unsigned int uint;
#define BB_UINT_DEF
#endif

#ifdef RC_SDK
struct BBDXCommandData
{
	typedef int (tChannelPlayingfn)(uint channel);
	typedef uint (tEmitSoundfn)(uint sound, uint entity);
	typedef uint (tPlaySoundfn)(uint sound);
	typedef uint (tLoadSoundfn)(const char* path);
	typedef uint (tLoad3DSoundfn)(const char* path);
	typedef uint (tPlayMusicfn)(const char* path);
	typedef void (tChannelVolumefn)(uint channel, float level);
	typedef void (tFreeSoundfn)(uint sound);
	typedef void (tLoopSoundfn)(uint sound);
	typedef void (tSoundVolumefn)(uint snd, float level);
	typedef void (tStopChannelfn)(uint channel);
	typedef void (tPauseChannelfn)(uint channel);
	typedef void (tResumeChannelfn)(uint channel);
	typedef void (tChannelPanfn)(uint channel, float pan);
	typedef void (tFreeEntityfn)(uint entity);
	typedef float (tMeshWidthfn)(uint entity);
	typedef float (tMeshHeightfn)(uint entity);
	typedef float (tMeshDepthfn)(uint entity);
	typedef uint (tCreatePointLightfn)();
	typedef uint (tCreateDirectionalLightfn)();
	typedef void (tSetLightPositionfn)(uint light, float x, float y, float z);
	typedef void (tSetLightDirectionfn)(uint light, float x, float y, float z);
	typedef void (tSetLightRadiusfn)(uint light, float radius);
	typedef void (tSetPLightColorfn)(uint light, int r, int g, int b);
	typedef void (tSetDLightColorfn)(uint light, int r, int g, int b);
	typedef void (tAmbientLightfn)(int r, int g, int b);
	typedef void (tSetPLightActivefn)(uint light, int active);
	typedef void (tSetDLightActivefn)(uint light, int active);
	typedef void (tFreePLightfn)(uint light);
	typedef void (tFreeDLightfn)(uint light);
	typedef void (tUpdateHardwareBuffersfn)(uint entity);
	typedef uint (tCreateCamerafn)(uint parent);
	typedef uint (tLoadMeshfn)(const char* path, uint parent, int animated, bbdx2_ASyncJobFn completionCallback, void* userData);
	typedef uint (tCreatePivotfn)(uint parent);
	typedef void (tPositionEntityfn)(uint entity, float x, float y, float z, int global );
	typedef void (tTranslateEntityfn)(uint entity, float x, float y, float z, int global );
	typedef void (tRotateEntityfn)(uint entity, float x, float y, float z, int global );
	typedef void (tScaleEntityfn)(uint entity, float x, float y, float z, int global );
	typedef void (tMoveEntityfn)(uint entity, float x, float y, float z);
	typedef void (tTurnEntityfn)(uint entity, float x, float y, float z);
	typedef void (tPointEntityfn)(uint entity, uint target, float zero );
	typedef void (tEntityOrderfn)(uint entity, int order);
	typedef void (tCameraClsColorfn)(uint camera, int r, int g, int b);
	typedef void (tEntityFXfn)(uint entity, int fx);
	typedef void (tEntityAlphaNoSolidfn)(uint entity, float alpha);
	typedef uint (tLoadShaderfn)(const char* path);
	typedef void (tEntityShaderfn)(uint entity, uint shader);
	typedef void (tHideEntityfn)(uint entity);
	typedef void (tHideEntityKeepCollisionsfn)(uint entity);
	typedef void (tShowEntityfn)(uint entity);
	typedef uint (tCopyEntityfn)(uint entity, uint parent );
	typedef float (tEntityXfn)(uint entity, int global );
	typedef float (tEntityYfn)(uint entity, int global );
	typedef float (tEntityZfn)(uint entity, int global );
	typedef float (tEntityScaleXfn)(uint entity, int global );
	typedef float (tEntityScaleYfn)(uint entity, int global );
	typedef float (tEntityScaleZfn)(uint entity, int global );
	typedef float (tEntityPitchfn)(uint entity, int global );
	typedef float (tEntityYawfn)(uint entity, int global );
	typedef float (tEntityRollfn)(uint entity, int global );
	typedef void (tEntityAlphafn)(uint entity, float alpha);
	typedef void (tAlignToVectorfn)(uint entity, float x, float y, float z, int axis);
	typedef const char* (tEntityClassfn)(uint entity);
	typedef uint (tCountChildrenfn)(uint entity);
	typedef uint (tGetChildfn)(uint entity, uint index);
	typedef uint (tGetParentfn)(uint entity);
	typedef void (tEntityParentfn)(uint entity, uint parent, int global );
	typedef uint (tLoadTexturefn)(const char* path, uint flags);
	typedef void (tEntityTexturefn)(uint entity, uint texture, int index , int surface );
	typedef void (tFreeTexturefn)(uint texture);
	typedef void (tTFormVectorfn)(float x, float y, float z, uint w1, uint w2);
	typedef void (tTFormPointfn)(float x, float y, float z, uint w1, uint w2);
	typedef float (tTFormedXfn)();
	typedef float (tTFormedYfn)();
	typedef float (tTFormedZfn)();
	typedef const char* (tEntityNamefn)(uint entity);
	typedef void (tEntityTypefn)(uint entity, int type);
	typedef int (tGetEntityTypefn)(uint entity);
	typedef void (tEntityRadiusfn)(uint entity, float width, float height );
	typedef void (tSetCollisionMeshfn)(uint entity);
	typedef void (tResetEntityfn)(uint entity);
	typedef int (tCountCollisionsfn)(uint entity);
	typedef float (tCollisionXfn)(uint entity, int index);
	typedef float (tCollisionYfn)(uint entity, int index);
	typedef float (tCollisionZfn)(uint entity, int index);
	typedef float (tCollisionNXfn)(uint entity, int index);
	typedef float (tCollisionNYfn)(uint entity, int index);
	typedef float (tCollisionNZfn)(uint entity, int index);
	typedef void (tEntityPickModefn)(uint entity, int mode);
	typedef uint (tLinePickfn)(float x, float y, float z, float destX, float destY, float destZ, float radius );
	typedef uint (tCameraPickfn)(int x, int y);
	typedef float (tPickedXfn)();
	typedef float (tPickedYfn)();
	typedef float (tPickedZfn)();
	typedef float (tPickedNXfn)();
	typedef float (tPickedNYfn)();
	typedef float (tPickedNZfn)();
	typedef void (tNameEntityfn)(uint entity, const char* name);
	typedef void (tTagEntityfn)(uint entity, const char* tag);
	typedef const char* (tEntityTagfn)(uint entity);
	typedef int (tAnimatingfn)(uint entity);
	typedef int (tAnimLengthfn)(uint entity);
	typedef int (tAnimSeqfn)(uint entity);
	typedef float (tAnimTimefn)(uint entity);
	typedef uint (tCopyTexturefn)(uint oldTexture);
	typedef uint (tFindChildfn)(uint entity, const char* name);
	typedef void (tFlipMeshfn)(uint entity);
	typedef int (tGraphicsHeightfn)();
	typedef int (tGraphicsWidthfn)();
	typedef float (tQueryBoxfn)(uint entity, int index, int xyz);
	typedef float (tTextureWidthfn)(uint texture);
	typedef float (tTextureHeightfn)(uint texture);
	typedef const char* (tTextureNamefn)(uint texture);
	typedef void (tEntityConstantFloatfn)(uint entity, const char* name, float v0);
	typedef void (tEntityConstantFloat2fn)(uint entity, const char* name, float v0, float v1);
	typedef void (tEntityConstantFloat3fn)(uint entity, const char* name, float v0, float v1, float v2);
	typedef void (tEntityConstantFloat4fn)(uint entity, const char* name, float v0, float v1, float v2, float v3);
	typedef void (tGlobalShaderConstantFloat4fn)(const char* name, float v0, float v1, float v2, float v3);
	typedef void (tCalculateB3DTangentsfn)(uint entity);
	typedef uint (tCreateLinefn)(uint parent);
	typedef void (tFreeLinefn)(uint line);
	typedef void (tSetLineSizefn)(uint line, float startX, float startY, float startZ, float endX, float endY, float endZ);
	typedef void (tSetLineColorfn)(uint line, int r, int g, int b);
	typedef void (tSetLineVisiblefn)(uint line, int visible);
	typedef uint (tLoadProfilefn)(const char* path);
	typedef uint (tCreateProfilefn)(const char* name);
	typedef void (tFreeProfilefn)(uint profile);
	typedef void (tSetProfileRangefn)(uint profile, float range);
	typedef float (tGetProfileRangefn)(uint profile);
	typedef void (tSetProfileEffectfn)(uint profile, int qualityLevel, int distance, uint effect);
	typedef uint (tGetProfileEffectfn)(uint profile, int qualityLevel, int distance);
	typedef void (tEntityProfilefn)(uint entity, uint profile);
	typedef uint (tCreatePhysicsDescfn)(int isStatic, uint node);
	typedef void (tAddSpherefn)(uint desc, float x, float y, float z, float radius, float mass);
	typedef void (tAddCapsulefn)(uint desc, float x, float y, float z, float width, float height, float mass);
	typedef void (tAddBoxfn)(uint desc, float x, float y, float z, float width, float height, float depth, float mass);
	typedef void (tClosePhysicsDescfn)(uint desc);
	typedef void (tCameraFogColorfn)(uint camera, int r, int g, int b);
	typedef void (tCameraFogRangefn)(uint camera, float near, float far);
	typedef void (tCameraRangefn)(uint camera, float near, float far);
	typedef void (tScaleMeshfn)(uint entity, float x, float y, float z);
	typedef uint (tCreateMeshfn)(uint parent);
	typedef uint (tCreateSurfacefn)(uint mesh);
	typedef uint (tAddVertexfn)(uint surface, float x, float y, float z, float u, float v, float w);
	typedef void (tVertexNormalfn)(uint surface, int index, float nx, float ny, float nz);
	typedef uint (tAddTrianglefn)(uint surface, uint v0, uint v1, uint v2);
	typedef void* (tGetIDirect3DDevice9fn)();
	typedef void* (tbbdx2_GetNxScenefn)();
	typedef void* (tbbdx2_GetNxPhysicsSDKfn)();

	tChannelPlayingfn* ChannelPlaying;
	tEmitSoundfn* EmitSound;
	tPlaySoundfn* PlaySound;
	tLoadSoundfn* LoadSound;
	tLoad3DSoundfn* Load3DSound;
	tPlayMusicfn* PlayMusic;
	tChannelVolumefn* ChannelVolume;
	tFreeSoundfn* FreeSound;
	tLoopSoundfn* LoopSound;
	tSoundVolumefn* SoundVolume;
	tStopChannelfn* StopChannel;
	tPauseChannelfn* PauseChannel;
	tResumeChannelfn* ResumeChannel;
	tChannelPanfn* ChannelPan;
	tFreeEntityfn* FreeEntity;
	tMeshWidthfn* MeshWidth;
	tMeshHeightfn* MeshHeight;
	tMeshDepthfn* MeshDepth;
	tCreatePointLightfn* CreatePointLight;
	tCreateDirectionalLightfn* CreateDirectionalLight;
	tSetLightPositionfn* SetLightPosition;
	tSetLightDirectionfn* SetLightDirection;
	tSetLightRadiusfn* SetLightRadius;
	tSetPLightColorfn* SetPLightColor;
	tSetDLightColorfn* SetDLightColor;
	tSetPLightActivefn* SetPLightActive;
	tSetDLightActivefn* SetDLightActive;
	tFreePLightfn* FreePLight;
	tFreeDLightfn* FreeDLight;
	tUpdateHardwareBuffersfn* UpdateHardwareBuffers;
	tCreateCamerafn* CreateCamera;
	tLoadMeshfn* LoadMesh;
	tCreatePivotfn* CreatePivot;
	tPositionEntityfn* PositionEntity;
	tTranslateEntityfn* TranslateEntity;
	tRotateEntityfn* RotateEntity;
	tScaleEntityfn* ScaleEntity;
	tMoveEntityfn* MoveEntity;
	tTurnEntityfn* TurnEntity;
	tPointEntityfn* PointEntity;
	tEntityOrderfn* EntityOrder;
	tCameraClsColorfn* CameraClsColor;
	tEntityFXfn* EntityFX;
	tEntityAlphaNoSolidfn* EntityAlphaNoSolid;
	tLoadShaderfn* LoadShader;
	tEntityShaderfn* EntityShader;
	tHideEntityfn* HideEntity;
	tHideEntityKeepCollisionsfn* HideEntityKeepCollisions;
	tShowEntityfn* ShowEntity;
	tCopyEntityfn* CopyEntity;
	tEntityXfn* EntityX;
	tEntityYfn* EntityY;
	tEntityZfn* EntityZ;
	tEntityScaleXfn* EntityScaleX;
	tEntityScaleYfn* EntityScaleY;
	tEntityScaleZfn* EntityScaleZ;
	tEntityPitchfn* EntityPitch;
	tEntityYawfn* EntityYaw;
	tEntityRollfn* EntityRoll;
	tEntityAlphafn* EntityAlpha;
	tAlignToVectorfn* AlignToVector;
	tEntityClassfn* EntityClass;
	tCountChildrenfn* CountChildren;
	tGetChildfn* GetChild;
	tGetParentfn* GetParent;
	tEntityParentfn* EntityParent;
	tLoadTexturefn* LoadTexture;
	tEntityTexturefn* EntityTexture;
	tFreeTexturefn* FreeTexture;
	tTFormVectorfn* TFormVector;
	tTFormPointfn* TFormPoint;
	tTFormedXfn* TFormedX;
	tTFormedYfn* TFormedY;
	tTFormedZfn* TFormedZ;
	tEntityNamefn* EntityName;
	tEntityTypefn* EntityType;
	tGetEntityTypefn* GetEntityType;
	tEntityRadiusfn* EntityRadius;
	tSetCollisionMeshfn* SetCollisionMesh;
	tResetEntityfn* ResetEntity;
	tCountCollisionsfn* CountCollisions;
	tCollisionXfn* CollisionX;
	tCollisionYfn* CollisionY;
	tCollisionZfn* CollisionZ;
	tCollisionNXfn* CollisionNX;
	tCollisionNYfn* CollisionNY;
	tCollisionNZfn* CollisionNZ;
	tEntityPickModefn* EntityPickMode;
	tLinePickfn* LinePick;
	tCameraPickfn* CameraPick;
	tPickedXfn* PickedX;
	tPickedYfn* PickedY;
	tPickedZfn* PickedZ;
	tPickedNXfn* PickedNX;
	tPickedNYfn* PickedNY;
	tPickedNZfn* PickedNZ;
	tNameEntityfn* NameEntity;
	tTagEntityfn* TagEntity;
	tEntityTagfn* EntityTag;
	tAnimatingfn* Animating;
	tAnimLengthfn* AnimLength;
	tAnimSeqfn* AnimSeq;
	tAnimTimefn* AnimTime;
	tCopyTexturefn* CopyTexture;
	tFindChildfn* FindChild;
	tFlipMeshfn* FlipMesh;
	tGraphicsHeightfn* GraphicsHeight;
	tGraphicsWidthfn* GraphicsWidth;
	tQueryBoxfn* QueryBox;
	tTextureWidthfn* TextureWidth;
	tTextureHeightfn* TextureHeight;
	tTextureNamefn* TextureName;
	tEntityConstantFloatfn* EntityConstantFloat;
	tEntityConstantFloat2fn* EntityConstantFloat2;
	tEntityConstantFloat3fn* EntityConstantFloat3;
	tEntityConstantFloat4fn* EntityConstantFloat4;
	tGlobalShaderConstantFloat4fn* GlobalShaderConstantFloat4;
	tCalculateB3DTangentsfn* CalculateB3DTangents;
	tCreateLinefn* CreateLine;
	tFreeLinefn* FreeLine;
	tSetLineSizefn* SetLineSize;
	tSetLineColorfn* SetLineColor;
	tSetLineVisiblefn* SetLineVisible;
	tLoadProfilefn* LoadProfile;
	tCreateProfilefn* CreateProfile;
	tFreeProfilefn* FreeProfile;
	tSetProfileRangefn* SetProfileRange;
	tGetProfileRangefn* GetProfileRange;
	tSetProfileEffectfn* SetProfileEffect;
	tGetProfileEffectfn* GetProfileEffect;
	tEntityProfilefn* EntityProfile;
	tCreatePhysicsDescfn* CreatePhysicsDesc;
	tAddSpherefn* AddSphere;
	tAddCapsulefn* AddCapsule;
	tAddBoxfn* AddBox;
	tClosePhysicsDescfn* ClosePhysicsDesc;
	tCameraFogColorfn* CameraFogColor;
	tCameraFogRangefn* CameraFogRange;
	tCameraRangefn* CameraRange;
	tScaleMeshfn* ScaleMesh;
	tCreateMeshfn* CreateMesh;
	tCreateSurfacefn* CreateSurface;
	tAddVertexfn* AddVertex;
	tVertexNormalfn* VertexNormal;
	tAddTrianglefn* AddTriangle;
	tAmbientLightfn* AmbientLight;
	tGetIDirect3DDevice9fn* bbdx2_GetIDirect3DDevice9;
	tbbdx2_GetNxScenefn* bbdx2_GetNxScene;
	tbbdx2_GetNxPhysicsSDKfn* bbdx2_GetNxPhysicsSDK;
};
extern BBDXCommandData* BBDXCommands;

inline uint CreateMesh(uint parent = 0)
{
	return BBDXCommands->CreateMesh(parent);
}

inline uint CreateSurface(uint mesh)
{
	return BBDXCommands->CreateSurface(mesh);
}

inline uint AddVertex(uint surface, float x, float y, float z, float u = 0.0f, float v = 0.0f, float w = 0.0f)
{
	return BBDXCommands->AddVertex(surface, x, y, z, u, v, w);
}

inline void VertexNormal(uint surface, int index, float nx, float ny, float nz)
{
	BBDXCommands->VertexNormal(surface, index, nx, ny, nz);
}

inline uint AddTriangle(uint surface, uint v0, uint v1, uint v2)
{
	return BBDXCommands->AddTriangle(surface, v0, v1, v2);
}

inline void CameraFogColor(uint camera, int r, int g, int b)
{
	BBDXCommands->CameraFogColor(camera, r, g, b);
}

inline void CameraFogRange(uint camera, float rnear, float tfar)
{
	BBDXCommands->CameraFogRange(camera, rnear, tfar);
}

inline void CameraRange(uint camera, float rnear, float tfar)
{
	BBDXCommands->CameraRange(camera, rnear, tfar);
}

inline void ScaleMesh(uint entity, float x, float y, float z)
{
	BBDXCommands->ScaleMesh(entity, x, y, z);
}

inline bool ChannelPlaying(uint channel)
{
	return (BBDXCommands->ChannelPlaying(channel)) > 0 ? true : false;
}

inline uint EmitSound(uint sound, uint entity)
{
	return BBDXCommands->EmitSound(sound, entity);
}

inline uint BBPlaySound(uint sound)
{
	return BBDXCommands->PlaySound(sound);
}

inline uint LoadSound(std::string path)
{
	return BBDXCommands->LoadSound(path.c_str());
}

inline uint Load3DSound(std::string path)
{
	return BBDXCommands->Load3DSound(path.c_str());
}

inline uint PlayMusic(std::string path)
{
	return BBDXCommands->PlayMusic(path.c_str());
}

inline void ChannelVolume(uint channel, float level)
{
	return BBDXCommands->ChannelVolume(channel,  level);
}

inline void FreeSound(uint sound)
{
	return BBDXCommands->FreeSound(sound);
}

inline void LoopSound(uint sound)
{
	return BBDXCommands->LoopSound(sound);
}

inline void SoundVolume(uint sound, float level)
{
	return BBDXCommands->SoundVolume(sound, level);
}

inline void StopChannel(uint channel)
{
	return BBDXCommands->StopChannel(channel);
}

inline void PauseChannel(uint channel)
{
	return BBDXCommands->PauseChannel(channel);
}

inline void ResumeChannel(uint channel)
{
	return BBDXCommands->ResumeChannel(channel);
}

inline void ChannelPan(uint channel, float pan)
{
	return BBDXCommands->ChannelPan(channel,  pan);
}

inline void FreeEntity(uint entity)
{
	return BBDXCommands->FreeEntity(entity);
}

inline float MeshWidth(uint entity)
{
	return BBDXCommands->MeshWidth(entity);
}

inline float MeshHeight(uint entity)
{
	return BBDXCommands->MeshHeight(entity);
}

inline float MeshDepth(uint entity)
{
	return BBDXCommands->MeshDepth(entity);
}

inline uint CreatePointLight()
{
	return BBDXCommands->CreatePointLight();
}

inline uint CreateDirectionalLight()
{
	return BBDXCommands->CreateDirectionalLight();
}

inline void SetLightPosition(uint light, float x, float y, float z)
{
	return BBDXCommands->SetLightPosition(light,  x,  y,  z);
}

inline void SetLightDirection(uint light, float x, float y, float z)
{
	return BBDXCommands->SetLightDirection(light,  x,  y,  z);
}

inline void SetLightRadius(uint light, float radius)
{
	return BBDXCommands->SetLightRadius(light,  radius);
}

inline void SetPLightColor(uint light, int r, int g, int b)
{
	return BBDXCommands->SetPLightColor(light,  r,  g,  b);
}

inline void SetDLightColor(uint light, int r, int g, int b)
{
	return BBDXCommands->SetDLightColor(light,  r,  g,  b);
}

inline void AmbientLight(int r, int g, int b)
{
	BBDXCommands->AmbientLight(r, g, b);
}

inline void SetPLightActive(uint light, bool active)
{
	return BBDXCommands->SetPLightActive(light,  active);
}

inline void SetDLightActive(uint light, bool active)
{
	return BBDXCommands->SetDLightActive(light,  active);
}

inline void FreePLight(uint light)
{
	return BBDXCommands->FreePLight(light);
}

inline void FreeDLight(uint light)
{
	return BBDXCommands->FreeDLight(light);
}

inline void UpdateHardwareBuffers(uint entity)
{
	return BBDXCommands->UpdateHardwareBuffers(entity);
}

inline uint CreateCamera(uint parent)
{
	return BBDXCommands->CreateCamera(parent);
}

inline uint LoadMesh(std::string path, uint parent = 0, bool animated = false, bbdx2_ASyncJobFn completionCallback = NULL, void* userData = NULL)
{
	return BBDXCommands->LoadMesh(path.c_str(), parent, animated, completionCallback, userData);
}

inline uint CreatePivot(uint parent)
{
	return BBDXCommands->CreatePivot(parent);
}

inline void PositionEntity(uint entity, float x, float y, float z, bool global = false)
{
	return BBDXCommands->PositionEntity(entity,  x,  y,  z,  global );
}

inline void TranslateEntity(uint entity, float x, float y, float z, bool global = false)
{
	return BBDXCommands->TranslateEntity(entity,  x,  y,  z,  global );
}

inline void RotateEntity(uint entity, float x, float y, float z, bool global = false)
{
	return BBDXCommands->RotateEntity(entity,  x,  y,  z,  global );
}

inline void ScaleEntity(uint entity, float x, float y, float z, bool global = false)
{
	return BBDXCommands->ScaleEntity(entity,  x,  y,  z,  global );
}

inline void MoveEntity(uint entity, float x, float y, float z)
{
	return BBDXCommands->MoveEntity(entity,  x,  y,  z);
}

inline void TurnEntity(uint entity, float x, float y, float z)
{
	return BBDXCommands->TurnEntity(entity,  x,  y,  z);
}

inline void PointEntity(uint entity, uint target, float zero = 0.0f)
{
	return BBDXCommands->PointEntity(entity, target,  zero );
}

inline void EntityOrder(uint entity, int order)
{
	return BBDXCommands->EntityOrder(entity,  order);
}

inline void CameraClsColor(uint camera, int r, int g, int b)
{
	return BBDXCommands->CameraClsColor(camera,  r,  g,  b);
}

inline void EntityFX(uint entity, int fx)
{
	return BBDXCommands->EntityFX(entity,  fx);
}

inline void EntityAlphaNoSolid(uint entity, float alpha)
{
	return BBDXCommands->EntityAlphaNoSolid(entity,  alpha);
}

inline uint LoadShader(std::string path)
{
	return BBDXCommands->LoadShader(path.c_str());
}

inline void EntityShader(uint entity, uint shader)
{
	return BBDXCommands->EntityShader(entity, shader);
}

inline void HideEntity(uint entity)
{
	return BBDXCommands->HideEntity(entity);
}

inline void HideEntityKeepCollisions(uint entity)
{
	return BBDXCommands->HideEntityKeepCollisions(entity);
}

inline void ShowEntity(uint entity)
{
	return BBDXCommands->ShowEntity(entity);
}

inline uint CopyEntity(uint entity, uint parent = 0)
{
	return BBDXCommands->CopyEntity(entity, parent );
}

inline float EntityX(uint entity, bool global = false)
{
	return BBDXCommands->EntityX(entity,  global );
}

inline float EntityY(uint entity, bool global = false)
{
	return BBDXCommands->EntityY(entity,  global );
}

inline float EntityZ(uint entity, bool global = false)
{
	return BBDXCommands->EntityZ(entity,  global );
}

inline float EntityScaleX(uint entity, bool global = false)
{
	return BBDXCommands->EntityScaleX(entity,  global );
}

inline float EntityScaleY(uint entity, bool global = false)
{
	return BBDXCommands->EntityScaleY(entity,  global );
}

inline float EntityScaleZ(uint entity, bool global = false)
{
	return BBDXCommands->EntityScaleZ(entity,  global );
}

inline float EntityPitch(uint entity, bool global = false)
{
	return BBDXCommands->EntityPitch(entity,  global );
}

inline float EntityYaw(uint entity, bool global = false)
{
	return BBDXCommands->EntityYaw(entity,  global );
}

inline float EntityRoll(uint entity, bool global = false)
{
	return BBDXCommands->EntityRoll(entity,  global );
}

inline void EntityAlpha(uint entity, float alpha)
{
	return BBDXCommands->EntityAlpha(entity,  alpha);
}

inline void AlignToVector(uint entity, float x, float y, float z, int axis)
{
	return BBDXCommands->AlignToVector(entity,  x,  y,  z,  axis);
}

inline std::string EntityClass(uint entity)
{
	return std::string(BBDXCommands->EntityClass(entity));
}

inline uint CountChildren(uint entity)
{
	return BBDXCommands->CountChildren(entity);
}

inline uint GetChild(uint entity, uint index)
{
	return BBDXCommands->GetChild(entity, index);
}

inline uint GetParent(uint entity)
{
	return BBDXCommands->GetParent(entity);
}

inline void EntityParent(uint entity, uint parent, bool global = true)
{
	return BBDXCommands->EntityParent(entity, parent,  global );
}

inline uint LoadTexture(std::string path, uint flags = 0)
{
	return BBDXCommands->LoadTexture(path.c_str(), flags);
}

inline void EntityTexture(uint entity, uint texture, int index = 0, int surface = -1)
{
	return BBDXCommands->EntityTexture(entity, texture,  index ,  surface );
}

inline void FreeTexture(uint texture)
{
	return BBDXCommands->FreeTexture(texture);
}

inline void TFormVector(float x, float y, float z, uint w1, uint w2)
{
	return BBDXCommands->TFormVector( x,  y,  z, w1, w2);
}

inline void TFormPoint(float x, float y, float z, uint w1, uint w2)
{
	return BBDXCommands->TFormPoint( x,  y,  z, w1, w2);
}

inline float TFormedX()
{
	return BBDXCommands->TFormedX();
}

inline float TFormedY()
{
	return BBDXCommands->TFormedY();
}

inline float TFormedZ()
{
	return BBDXCommands->TFormedZ();
}

inline std::string EntityName(uint entity)
{
	return std::string(BBDXCommands->EntityName(entity));
}

inline void EntityType(uint entity, int type)
{
	return BBDXCommands->EntityType(entity,  type);
}

inline int GetEntityType(uint entity)
{
	return BBDXCommands->GetEntityType(entity);
}

inline void EntityRadius(uint entity, float width, float height = -1)
{
	return BBDXCommands->EntityRadius(entity,  width,  height );
}

inline void SetCollisionMesh(uint entity)
{
	return BBDXCommands->SetCollisionMesh(entity);
}

inline void ResetEntity(uint entity)
{
	return BBDXCommands->ResetEntity(entity);
}

inline int CountCollisions(uint entity)
{
	return BBDXCommands->CountCollisions(entity);
}

inline float CollisionX(uint entity, int index)
{
	return BBDXCommands->CollisionX(entity,  index);
}

inline float CollisionY(uint entity, int index)
{
	return BBDXCommands->CollisionY(entity,  index);
}

inline float CollisionZ(uint entity, int index)
{
	return BBDXCommands->CollisionZ(entity,  index);
}

inline float CollisionNX(uint entity, int index)
{
	return BBDXCommands->CollisionNX(entity,  index);
}

inline float CollisionNY(uint entity, int index)
{
	return BBDXCommands->CollisionNY(entity,  index);
}

inline float CollisionNZ(uint entity, int index)
{
	return BBDXCommands->CollisionNZ(entity,  index);
}

inline void EntityPickMode(uint entity, int mode)
{
	return BBDXCommands->EntityPickMode(entity,  mode);
}

inline uint LinePick(float x, float y, float z, float destX, float destY, float destZ, float radius = 0.0f)
{
	return BBDXCommands->LinePick( x,  y,  z,  destX,  destY,  destZ,  radius );
}

inline uint CameraPick(int x, int y)
{
	return BBDXCommands->CameraPick(x,  y);
}

inline float PickedX()
{
	return BBDXCommands->PickedX();
}

inline float PickedY()
{
	return BBDXCommands->PickedY();
}

inline float PickedZ()
{
	return BBDXCommands->PickedZ();
}

inline float PickedNX()
{
	return BBDXCommands->PickedNX();
}

inline float PickedNY()
{
	return BBDXCommands->PickedNY();
}

inline float PickedNZ()
{
	return BBDXCommands->PickedNZ();
}

inline void NameEntity(uint entity, std::string name)
{
	return BBDXCommands->NameEntity(entity, name.c_str());
}

inline void TagEntity(uint entity, std::string tag)
{
	return BBDXCommands->TagEntity(entity, tag.c_str());
}

inline std::string EntityTag(uint entity)
{
	return std::string(BBDXCommands->EntityTag(entity));
}

inline bool Animating(uint entity)
{
	return (BBDXCommands->Animating(entity)) > 0 ? true : false;
}

inline int AnimLength(uint entity)
{
	return BBDXCommands->AnimLength(entity);
}

inline int AnimSeq(uint entity)
{
	return BBDXCommands->AnimSeq(entity);
}

inline float AnimTime(uint entity)
{
	return BBDXCommands->AnimTime(entity);
}

inline uint CopyTexture(uint oldTexture)
{
	return BBDXCommands->CopyTexture(oldTexture);
}

inline uint FindChild(uint entity, std::string name)
{
	return BBDXCommands->FindChild(entity, name.c_str());
}

inline void FlipMesh(uint entity)
{
	return BBDXCommands->FlipMesh(entity);
}

inline int GraphicsHeight()
{
	return BBDXCommands->GraphicsHeight();
}

inline int GraphicsWidth()
{
	return BBDXCommands->GraphicsWidth();
}

inline float QueryBox(uint entity, int index, int xyz)
{
	return BBDXCommands->QueryBox(entity,  index,  xyz);
}

inline float TextureWidth(uint texture)
{
	return BBDXCommands->TextureWidth(texture);
}

inline float TextureHeight(uint texture)
{
	return BBDXCommands->TextureHeight(texture);
}

inline std::string TextureName(uint texture)
{
	return std::string(BBDXCommands->TextureName(texture));
}

inline void EntityConstantFloat(uint entity, std::string name, float v0)
{
	return BBDXCommands->EntityConstantFloat(entity, name.c_str(), v0);
}

inline void EntityConstantFloat2(uint entity, std::string name, float v0, float v1)
{
	return BBDXCommands->EntityConstantFloat2(entity, name.c_str(), v0, v1);
}

inline void EntityConstantFloat3(uint entity, std::string name, float v0, float v1, float v2)
{
	return BBDXCommands->EntityConstantFloat3(entity, name.c_str(), v0, v1, v2);
}

inline void EntityConstantFloat4(uint entity, std::string name, float v0, float v1, float v2, float v3)
{
	return BBDXCommands->EntityConstantFloat4(entity, name.c_str(), v0, v1, v2, v3);
}

inline void GlobalShaderConstantFloat4(std::string name, float v0, float v1, float v2, float v3)
{
	BBDXCommands->GlobalShaderConstantFloat4(name.c_str(), v0, v1, v2, v3);
}

inline void CalculateB3DTangents(uint entity)
{
	return BBDXCommands->CalculateB3DTangents(entity);
}

inline uint CreateLine(uint parent)
{
	return BBDXCommands->CreateLine(parent);
}

inline void FreeLine(uint line)
{
	return BBDXCommands->FreeLine(line);
}

inline void SetLineSize(uint line, float startX, float startY, float startZ, float endX, float endY, float endZ)
{
	return BBDXCommands->SetLineSize(line,  startX,  startY,  startZ,  endX,  endY,  endZ);
}

inline void SetLineColor(uint line, int r, int g, int b)
{
	return BBDXCommands->SetLineColor(line,  r,  g,  b);
}

inline void SetLineVisible(uint line, bool visible)
{
	return BBDXCommands->SetLineVisible(line,  visible);
}

inline uint LoadProfile(std::string path)
{
	return BBDXCommands->LoadProfile(path.c_str());
}

inline uint CreateProfile(std::string name)
{
	return BBDXCommands->CreateProfile(name.c_str());
}

inline void FreeProfile(uint profile)
{
	return BBDXCommands->FreeProfile(profile);
}

inline void SetProfileRange(uint profile, float range)
{
	return BBDXCommands->SetProfileRange(profile,  range);
}

inline float GetProfileRange(uint profile)
{
	return BBDXCommands->GetProfileRange(profile);
}

inline void SetProfileEffect(uint profile, int qualityLevel, int distance, uint effect)
{
	return BBDXCommands->SetProfileEffect(profile,  qualityLevel,  distance, effect);
}

inline uint GetProfileEffect(uint profile, int qualityLevel, int distance)
{
	return BBDXCommands->GetProfileEffect(profile,  qualityLevel,  distance);
}

inline void EntityProfile(uint entity, uint profile)
{
	return BBDXCommands->EntityProfile(entity, profile);
}

inline uint CreatePhysicsDesc(bool isStatic, uint node)
{
	return BBDXCommands->CreatePhysicsDesc( isStatic, node);
}

inline void AddSphere(uint desc, float x, float y, float z, float radius, float mass)
{
	return BBDXCommands->AddSphere(desc,  x,  y,  z,  radius,  mass);
}

inline void AddCapsule(uint desc, float x, float y, float z, float width, float height, float mass)
{
	return BBDXCommands->AddCapsule(desc,  x,  y,  z,  width,  height,  mass);
}

inline void AddBox(uint desc, float x, float y, float z, float width, float height, float depth, float mass)
{
	return BBDXCommands->AddBox(desc,  x,  y,  z,  width,  height,  depth,  mass);
}

inline void ClosePhysicsDesc(uint desc)
{
	return BBDXCommands->ClosePhysicsDesc(desc);
}

inline IDirect3DDevice9* bbdx2_GetIDirect3DDevice9()
{
	return (IDirect3DDevice9*)BBDXCommands->bbdx2_GetIDirect3DDevice9();
}

inline void* bbdx2_GetNxScene()
{
	return BBDXCommands->bbdx2_GetNxScene();
}

inline void* bbdx2_GetNxPhysicsSDK()
{
	return BBDXCommands->bbdx2_GetNxPhysicsSDK();
}

#endif


