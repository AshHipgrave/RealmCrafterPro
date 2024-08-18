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
#include "ThreadLoaders.h"
#include "IOThread.h"
#include <sys/stat.h>
#include "..\Irrlicht\Src\CReadMemory.h"
#include "..\Irrlicht\Src\CAnimatedMeshSceneNode.h"
#include "..\Irrlicht\Src\CD3D9Texture.h"
#include "..\BBDX\ASyncJobModule.h"

DLLPRE IAnimatedMeshSceneNode* DLLEX pathlier(IAnimatedMeshSceneNode* Parent);
bool IsMeshAlpha(IAnimatedMesh* mesh);
DLLPRE IAnimatedMeshSceneNode* DLLEX jalkming(irr::c8* filename, ISceneNode* parent, int Animated, bbdx2_ASyncJobFn completionCallback, void* userData);
DLLPRE IDirect3DDevice9* DLLEX bbdx2_GetIDirect3DDevice9();

struct ThreadMeshTagItem
{
	IAnimatedMeshSceneNode* Node;
	bool Animated;
	int TexturesToLoad;
};

struct ThreadTextureTagItem
{
	int Flags;
	ITexture* Texture;
};

void LoadThreadTexture_Loaded(void*, IOThreadInfo* info)
{
	ThreadTextureTagItem* TagItem = (ThreadTextureTagItem*)info->Tag;

	bool Cubemap = (TagItem->Flags & 128);
	bool Volume = (TagItem->Flags & 64);

	IDirect3DBaseTexture9* Texture;
	irr::core::dimension2d<irr::s32> Size;

	// Normal texture
	if(Cubemap)
	{
		IDirect3DCubeTexture9* CubeTexture = 0;
		D3DXCreateCubeTextureFromFileInMemoryEx(bbdx2_GetIDirect3DDevice9(),
			info->Buffer, info->Length, D3DX_DEFAULT, 0, 0, D3DFMT_UNKNOWN, D3DPOOL_MANAGED,
			D3DX_DEFAULT, D3DX_DEFAULT, ((TagItem->Flags & 4) > 0) ? 0xff000000 : 0x00000000, NULL, NULL, &CubeTexture);
		Texture = CubeTexture;

		D3DSURFACE_DESC desc;
		CubeTexture->GetLevelDesc(0, &desc);
		Size = irr::core::dimension2d<irr::s32>(desc.Width, desc.Height);

	}else if(Volume)
	{
		IDirect3DVolumeTexture9* VolumeTexture = 0;
		D3DXCreateVolumeTextureFromFileInMemoryEx(bbdx2_GetIDirect3DDevice9(),
			info->Buffer, info->Length, D3DX_DEFAULT, D3DX_DEFAULT, D3DX_DEFAULT, 0, 0, D3DFMT_UNKNOWN,
			D3DPOOL_MANAGED, D3DX_DEFAULT, D3DX_DEFAULT, ((TagItem->Flags & 4) > 0) ? 0xff000000 : 0x00000000, NULL, NULL, &VolumeTexture);
		Texture = VolumeTexture;

		D3DVOLUME_DESC desc;
		VolumeTexture->GetLevelDesc(0, &desc);
		Size = irr::core::dimension2d<irr::s32>(desc.Width, desc.Height);
	}else
	{
		IDirect3DTexture9* NormalTexture = 0;
		D3DXCreateTextureFromFileInMemoryEx(bbdx2_GetIDirect3DDevice9(),
			info->Buffer, info->Length, D3DX_DEFAULT, D3DX_DEFAULT, 0, 0, D3DFMT_UNKNOWN, D3DPOOL_MANAGED,
			D3DX_DEFAULT, D3DX_DEFAULT, ((TagItem->Flags & 4) > 0) ? 0xff000000 : 0x00000000, NULL, NULL, &NormalTexture);
		Texture = NormalTexture;

		D3DSURFACE_DESC desc;
		NormalTexture->GetLevelDesc(0, &desc);
		Size = irr::core::dimension2d<irr::s32>(desc.Width, desc.Height);
	}

	if(Texture != 0)
	{
		((irr::video::CD3D9Texture*)TagItem->Texture)->Texture = Texture;
	}
}

// Load Texture
DLLPRE ITexture* DLLEX LoadThreadTexture(irr::c8* Filename, int Flags)
{
	ITexture* Texture = driver->getEmptyTexture(Filename);
	Texture->grab();

	ThreadTextureTagItem* TagItem = new ThreadTextureTagItem();
	TagItem->Flags = Flags;
	TagItem->Texture = Texture;

	IOThreadReadFile(Filename, 0, new NGin::IEventHandler<void, IOThreadInfo>(&LoadThreadTexture_Loaded), TagItem);

	return Texture;

	//// Get the texture (with a masking flag)
 //   ITexture* Texture = driver->getTexture(Filename, (Flags&4), (Flags&128), (Flags&64));
	//if(Texture == driver->getDefaultTexture())
	//{
	//	if(DEBUGMODE)
	//	{
	//		char out[128];
	//		sprintf(out,"Error - Could not load texture: %s", Filename);
	//		MessageBox(NULL, out, "BBDX Debugger", MB_ICONERROR | MB_OK);
	//	}
	//}
	//
	//// Grab the texture as its going to be stored in blitz' "open" space
	//Texture->grab();

	//return Texture;
}

void LoadThreadMesh_Loaded(void*, IOThreadInfo* info)
{
	ThreadMeshTagItem* TagItem = (ThreadMeshTagItem*)info->Tag;

	bool Encrypted = true;
	if(info->Path.Substr(info->Path.Length() - 4).AsLower() == String(".b3d"))
		Encrypted = false;

	irr::io::CReadMemory* Reader = new irr::io::CReadMemory(info->Buffer, info->Length, info->Path.c_str(), Encrypted);

	IAnimatedMesh* NewMesh = NULL;//smgr->getMesh(Reader, TagItem->Animated, true);

	//IAnimatedMesh* OldMesh = TagItem->Node->getLocalMesh();
	TagItem->Node->setMesh(NewMesh);

	LoadThreadMesh_Completed(0, info);
}

void LoadThreadMesh_Completed(void*, IOThreadInfo* info)
{
	ThreadMeshTagItem* TagItem = (ThreadMeshTagItem*)info->Tag;

	IAnimatedMeshSceneNode* node = TagItem->Node;

	node->PublicName = info->Path.c_str();

	// Test all of the textures
	for(irr::s32 i = 0; i < node->getMaterialCount(); ++i)
	{
		if( !node->getMaterial(i).Texture1 )
			node->getMaterial(i).Texture1 = driver->getDefaultTexture();

		if( !node->getMaterial(i).Texture2 )
			node->getMaterial(i).Texture2 = driver->getDefaultTexture();

		if( !node->getMaterial(i).Texture3 )
			node->getMaterial(i).Texture3 = driver->getDefaultTexture();

		if( !node->getMaterial(i).Texture4 )
			node->getMaterial(i).Texture4 = driver->getDefaultTexture();
	}

	// Use this for alpha checking
	bool isAlpha = false;

	// Determine if any textures are alpha
	IAnimatedMesh* mesh = node->getLocalMesh();
	for(irr::s32 i = 0; i < mesh->getMesh(0)->getMeshBufferCount(); ++i)
		for(irr::u32 f = 0; f < 4; ++f)
			if(mesh->getMesh(0)->getMeshBuffer(i)->getMaterial().Textures[f] && mesh->getMesh(0)->getMeshBuffer(i)->getMaterial().Textures[f]->isAlpha)
			{
				isAlpha = true;
				mesh->getMesh(0)->getMeshBuffer(i)->TextureAlpha = true;
			}


	// Make sure the node knows this
	bool TexAlpha = isAlpha;
	node->TexAlpha = TexAlpha;

	// Check for alpha in the mesh, and if its forced
	node->ForceAlpha = node->ForceAlpha || IsMeshAlpha(mesh) || isAlpha;
	if(node->ForceAlpha)
		node->setMaterialType(::EMT_TRANSPARENT_ALPHA_CHANNEL);

	// RCTE terrains which use the horrible surface-layer system need to be drawn as solid
	std::string str = info->Path.c_str();
	if(str.find("\\RCTE\\",0) != std::string::npos || str.find("TE_",0) != std::string::npos)
	{
		node->ForceAlpha = false;
		node->setMaterialType(::EMT_SOLID);
	}

	// Used for animations
	node->setAnimationCPY();
	node->Anicnt = 0;

	// Setup default datas
	node->setName(info->Path.c_str());
	node->setDebugDataVisible(DEBUGMODE);

	// Create all joint kiddies
	if(node->getLocalMesh()->getMeshType() == irr::scene::EAMT_B3D)
		for(int i = 0; i < node->getB3DJointCount(); ++i)
			node->getB3DJointNode(i);

	// Create an interator to search through children
	const irr::core::list<ISceneNode*>& Children = node->getChildren();
	irr::core::list<ISceneNode*>::Iterator it = Children.begin();
	irr::core::array<ISceneNode*> Kids;
	// Search through children
	for (; it != Children.end(); ++it)
		if(*it)
		{
			irr::core::stringc NDat = "Bone: ";
			NDat += (*it)->getName();
			//DB(NDat.c_str());
		}

	// Disable Mesh Animation
	int Seq = ExtractAnimSeq(node,0,0);
	Animate(node,1,1,Seq);
}


DLLPRE IAnimatedMeshSceneNode* DLLEX LoadThreadMesh(irr::c8* filename, ISceneNode* parent, int Animated)
{
	// Verify file.
	struct stat FileInfo;
	if(stat(filename, &FileInfo) != 0)
	{
		// File doesn't exist
		return 0;
	}

	String FilenameLower = filename;
	FilenameLower.Lower();

	// See if the mesh is cached!
	for(int i = 0; i < smgr->getMeshCache()->getMeshCount(); ++i)
	{
		if(String(smgr->getMeshCache()->getMeshFilename(smgr->getMeshCache()->getMeshByIndex(i))).AsLower() == FilenameLower)
		{
			// Don't need to load it
			return jalkming(filename, parent, Animated, NULL, NULL);
		}
	}

	IAnimatedMeshSceneNode* node = pathlier((IAnimatedMeshSceneNode*)parent);

	ThreadMeshTagItem* TagItem = new ThreadMeshTagItem();
	TagItem->Node = node;
	TagItem->Animated = Animated > 0;
	TagItem->TexturesToLoad = 0;

	// Tell the listener to load the file
	IOThreadReadFile(filename, 0, new NGin::IEventHandler<void, IOThreadInfo>(&LoadThreadMesh_Loaded), TagItem);
 
    return node;
}
