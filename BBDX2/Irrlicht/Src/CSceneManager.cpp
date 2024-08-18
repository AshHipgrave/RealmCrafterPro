// Copyright (C) 2002-2006 Nikolaus Gebhardt
// This file is part of the "Irrlicht Engine".
// For conditions of distribution and use, see copyright notice in irrlicht.h

#include "CSceneManager.h"
#include "IVideoDriver.h"
#include "IFileSystem.h"
#include "IAnimatedMesh.h"
#include "CMeshCache.h"
#include "IWriteFile.h"
//#include "IXMLWriter.h"
#include "ISceneUserDataSerializer.h"

#include "os.h"
#include <wchar.h>
#include <string.h>

#include "CDefaultMeshFormatLoader.h"
#include "CXMeshFileLoader.h"


#include "CAnimatedMeshSceneNode.h"
#include "COctTreeSceneNode.h"
#include "CCameraSceneNode.h"
#include "CCameraMayaSceneNode.h"
#include "CCameraFPSSceneNode.h"
#include "CMeshSceneNode.h"
#include "CDummyTransformationSceneNode.h"
#include "CEmptySceneNode.h"
#include "CDefaultSceneNodeFactory.h"

#include "CMeshManipulator.h"

#include "CDefaultSceneNodeAnimatorFactory.h"

#ifdef NDEBUG
#define D3D_DEBUG_INFO
#endif
#include <d3d9.h>
#include "CD3D9Driver.h"
#include "CD3D9Texture.h"

#include "IAnimatedMeshB3d.h"
#include "windows.h"
#include "CD3D9Texture.h"

#include "bbdx\BBThread.h"
#include "CReadMemory.h"

namespace irr
{
namespace scene
{

	struct NPointLight
	{
		irr::core::vector3df	Position;
		irr::video::SColorf		Color;
		irr::s32				Radius;
		bool					Active;
		irr::f32				Distance;
	};

	struct NDirectionalLight
	{
		irr::core::vector3df	Normal;
		irr::video::SColorf		Color;
		bool					Active;
	};

	const irr::c8* PLightNames[5][3];
	const irr::c8* DLightNames[3][2];

const wchar_t* IRR_XML_FORMAT_SCENE				= L"irr_scene";
const wchar_t* IRR_XML_FORMAT_NODE				= L"node";
const wchar_t* IRR_XML_FORMAT_NODE_ATTR_TYPE	= L"type";

//! constructor
CSceneManager::CSceneManager(video::IVideoDriver* driver, io::IFileSystem* fs,
							 CMeshCache* cache, irr::IrrlichtDevice* device)
: ISceneNode(0, 0), Driver(driver), FileSystem(fs), ActiveCamera(0),
	CollisionManager(0),
	ShadowColor(150,0,0,0), MeshManipulator(0), CurrentRendertime(ESNRP_COUNT),
	MeshCache(cache), Device(device), BLAH(false), BLAH2(false), ShadowDistance(1000000.0f), LightDistance(5000.0f)
{
	#ifdef _DEBUG
	ISceneManager::setDebugName("CSceneManager ISceneManager");
	ISceneNode::setDebugName("CSceneManager ISceneNode");
	#endif

	RenderPow[0] = 1;
	RenderPow[1] = RenderPow[0] * 2;
	RenderPow[2] = RenderPow[1] * 2;
	RenderPow[3] = RenderPow[2] * 2;
	RenderPow[4] = RenderPow[3] * 2;
	RenderPow[5] = RenderPow[4] * 2;
	RenderPow[6] = RenderPow[5] * 2;
	RenderPow[7] = RenderPow[6] * 2;
	RenderPow[8] = RenderPow[7] * 2;
	RenderPow[9] = RenderPow[8] * 2;
	RenderPow[10] = RenderPow[9] * 2;
	RenderPow[11] = RenderPow[10] * 2;
	RenderPow[12] = RenderPow[11] * 2;
	RenderPow[13] = RenderPow[12] * 2;
	RenderPow[14] = RenderPow[13] * 2;
	RenderPow[15] = RenderPow[14] * 2;
	RenderPow[16] = RenderPow[15] * 2;
	RenderPow[17] = RenderPow[16] * 2;
	RenderPow[18] = RenderPow[17] * 2;
	RenderPow[19] = RenderPow[18] * 2;
	RenderPow[20] = RenderPow[19] * 2;
	RenderPow[21] = RenderPow[20] * 2;
	RenderPow[22] = RenderPow[21] * 2;
	RenderPow[23] = RenderPow[22] * 2;
	RenderPow[24] = RenderPow[23] * 2;
	RenderPow[25] = RenderPow[24] * 2;
	RenderPow[26] = RenderPow[25] * 2;
	RenderPow[27] = RenderPow[26] * 2;
	RenderPow[28] = RenderPow[27] * 2;
	RenderPow[29] = RenderPow[28] * 2;
	RenderPow[30] = RenderPow[29] * 2;
	RenderPow[31] = RenderPow[30] * 2;

	PLightNames[0][0] = "light1_position";
	PLightNames[0][1] = "light1_color";
	PLightNames[0][2] = "light1_radius";
	PLightNames[1][0] = "light2_position";
	PLightNames[1][1] = "light2_color";
	PLightNames[1][2] = "light2_radius";
	PLightNames[2][0] = "light3_position";
	PLightNames[2][1] = "light3_color";
	PLightNames[2][2] = "light3_radius";
	PLightNames[3][0] = "light4_position";
	PLightNames[3][1] = "light4_color";
	PLightNames[3][2] = "light4_radius";
	PLightNames[4][0] = "light5_position";
	PLightNames[4][1] = "light5_color";
	PLightNames[4][2] = "light5_radius";

	DLightNames[0][0] = "directional1_normal";
	DLightNames[0][1] = "directional1_color";
	DLightNames[1][0] = "directional2_normal";
	DLightNames[1][1] = "directional2_color";
	DLightNames[2][0] = "directional3_normal";
	DLightNames[2][1] = "directional3_color";

	if (Driver)
		Driver->grab();

	if (FileSystem)
		FileSystem->grab();

	// create mesh cache if not there already
	if (!MeshCache)
		MeshCache = new CMeshCache();
	else
		MeshCache->grab();


	// create manipulator
	MeshManipulator = new CMeshManipulator();

	// add default format loader

	MeshLoaderList.push_back(new CDefaultMeshFormatLoader(FileSystem, Driver));

	// factories

	ISceneNodeFactory* factory = new CDefaultSceneNodeFactory(this);
	registerSceneNodeFactory(factory);
	factory->drop();

	ISceneNodeAnimatorFactory* animatorFactory = new CDefaultSceneNodeAnimatorFactory(this);
	registerSceneNodeAnimatorFactory(animatorFactory);
	animatorFactory->drop();

	QualityLevel = 0;
	ShadowShader = 0;
	ShadowBlurShader[0] = 0;
	ShadowBlurShader[1] = 0;

	// Make a Tree Maker!
	

	//services->LT_Init();
	//IAnimatedMesh* msh = this->getMesh("tree.eb3d");

	//int i = services->LT_AddTreeType(
	//		((irr::video::CD3D9Texture*)msh->getMesh(0)->getMeshBuffer(0)->getMaterial().Texture1)->getDX9Texture(), 
	//			(IDirect3DIndexBuffer9 *)msh->getMesh(0)->getMeshBuffer(0)->indexB, 
	//			(IDirect3DVertexBuffer9*)msh->getMesh(0)->getMeshBuffer(0)->vertexB, 
	//									msh->getMesh(0)->getMeshBuffer(0)->getIndexCount(), 
	//									msh->getMesh(0)->getMeshBuffer(0)->getVertexCount());
	//
	//services->LT_AddTree(i, vector3df(0,0,0),vector3df(0,0,0),vector3df(1,1,1));
	//services->LT_AddTree(i, vector3df(200,0,0),vector3df(0,0,0),vector3df(1,1,1));

	//for(int xx = 0; xx < 32; ++xx)
	//	for(int yy = 0; yy < 32; ++yy)
	//		services->LT_AddTree(i, vector3df((f32)(xx*300),sin((((float)xx)/100.0f))*100,(f32)(yy*300)),vector3df(0,0,0),vector3df(1,1,1));
}

//int CSceneManager::AddTreeType(const irr::c8* file)
//{
//	irr::video::CD3D9Driver* services = (irr::video::CD3D9Driver*)this->getVideoDriver();
//	IAnimatedMesh* msh = this->getMesh("newtree.b3d");
//
//	int i = services->LT_AddTreeType(
//			((irr::video::CD3D9Texture*)msh->getMesh(0)->getMeshBuffer(0)->getMaterial().Texture1)->getDX9Texture(), 
//				(IDirect3DIndexBuffer9 *)msh->getMesh(0)->getMeshBuffer(0)->indexB, 
//				(IDirect3DVertexBuffer9*)msh->getMesh(0)->getMeshBuffer(0)->vertexB, 
//										msh->getMesh(0)->getMeshBuffer(0)->getIndexCount(), 
//										msh->getMesh(0)->getMeshBuffer(0)->getVertexCount());
//	return i;
//}
//
//void CSceneManager::AddTree(int T, core::vector3df P, core::vector3df R, core::vector3df S)
//{
//	irr::video::CD3D9Driver* services = (irr::video::CD3D9Driver*)this->getVideoDriver();
//	services->LT_AddTree(T,P,R,S);
//}

extern "C" _declspec(dllexport) void getmeshake(ISceneNode* Node);

//! destructor
CSceneManager::~CSceneManager()
{
//MessageBox(NULL,"Nothing yet Dropped","",MB_OK);
	getmeshake(this);

	clearDeletionList();
//MessageBox(NULL,"Deletion list Dropped","",MB_OK);
	if (Driver)
		Driver->drop();
//MessageBox(NULL,"Driver Dropped","",MB_OK);
	if (FileSystem)
		FileSystem->drop();
//MessageBox(NULL,"FS Dropped","",MB_OK);
//MessageBox(NULL,"CS Dropped","",MB_OK);
	if (CollisionManager)
		CollisionManager->drop();
//MessageBox(NULL,"CM Dropped","",MB_OK);
	if (MeshManipulator)
		MeshManipulator->drop();
//MessageBox(NULL,"MeshMan Dropped","",MB_OK);
	u32 i;

	for (i=0; i<MeshLoaderList.size(); ++i)
		MeshLoaderList[i]->drop();
//MessageBox(NULL,"MeshLoad Dropped","",MB_OK);
	if (ActiveCamera)
		ActiveCamera->drop();
//MessageBox(NULL,"Camera Dropped","",MB_OK);
	if (MeshCache)
		MeshCache->drop();
//MessageBox(NULL,"MeshCache Dropped","",MB_OK);
	for (i=0; i<SceneNodeFactoryList.size(); ++i)
		SceneNodeFactoryList[i]->drop();
//MessageBox(NULL,"SceneNode Fac Dropped","",MB_OK);
	for (i=0; i<SceneNodeAnimatorFactoryList.size(); ++i)
		SceneNodeAnimatorFactoryList[i]->drop();
//MessageBox(NULL,"Anim Fac Dropped","",MB_OK);
}

extern "C" _declspec(dllexport) void getmeshake(ISceneNode* Node);

void CSceneManager::Destroy()
{

}

#define MESH_LOAD_TYPE 0x4d455348
#define MESH_LOAD_PRIORITY 75

char* tempMeshLoaderBuffer = NULL;

struct LoadMeshData
{
	core::stringc Filename;
	CAnimatedMeshB3d* Mesh;
	ISceneManager* Manager;
	IVideoDriver* Driver;
	bool Encrypted;
	bbdx2_ASyncJobFn CompletionCallback;
	void* UserData;
	bool Success;
};

int LoadMeshTask( ASyncJobDesc* jobDesc )
{
	LoadMeshData* data = (LoadMeshData*)jobDesc->UserData;

	if( jobDesc->Buffer == NULL || data == NULL )
	{
		data->Success = false;
		return 1;
	}

	io::CReadMemory reader( (char*)jobDesc->Buffer, jobDesc->BufferSize, data->Filename.c_str(), data->Encrypted );

	if( !data->Mesh->loadFile( data->Manager, &reader, false ) )
	{
		data->Success = false;
		return 1;
	}

	return 1;
}

int LoadMeshSync( ASyncJobDesc* jobDesc )
{
	LoadMeshData* data = (LoadMeshData*)jobDesc->UserData;

	if( data == NULL )
		return 0;

	if( jobDesc->Buffer == NULL || data->Success == false )
	{
		ASyncJobDesc failedDesc;
		failedDesc.UserData = data->UserData;
		failedDesc.Buffer = (void*)data->Mesh;
		failedDesc.BufferSize = 0;

		if( data->CompletionCallback != NULL )
			data->CompletionCallback( &failedDesc );

		delete data;
		return 0;
	}

	IMesh* tmpMesh = data->Mesh->getMesh(0);
	for(int f = 0; f < tmpMesh->getMeshBufferCount(); ++f)
	{

		if(data->Mesh->getMeshType() == EAMT_B3D)
			if(((IAnimatedMeshB3d*)data->Mesh)->IsAnimated())
				data->Driver->updateAnimatedHardwareBuffer(tmpMesh->getMeshBuffer(f), &(((IAnimatedMeshB3d*)data->Mesh)->VBlends));
			else
				data->Driver->updateHardwareBuffer(tmpMesh->getMeshBuffer(f));
		else
			data->Driver->updateHardwareBuffer(tmpMesh->getMeshBuffer(f));
	}

	data->Mesh->setIsLoaded( true );

	ASyncJobDesc successDesc;
	successDesc.UserData = data->UserData;
	successDesc.Buffer = (void*)data->Mesh;
	successDesc.BufferSize = 1;

	if( data->CompletionCallback != NULL )
		data->CompletionCallback( &successDesc );

	return 1;
}

IAnimatedMesh* CSceneManager::getMesh(const c8* filename, bool animated, bbdx2_ASyncJobFn completionCallback, void* userData)
{
	// Check cache first
	IAnimatedMesh* msh = 0;

	core::stringc name = filename;
	name.make_lower();

	msh = MeshCache->findMesh(name.c_str());
	if (msh)
		return msh;

	// Check its loadable
	bool encrypted = false;

	if( !strstr( name.c_str(), ".b3d" ) )
	{
		if( !strstr( name.c_str(), ".eb3d" ) )
			return NULL;
		else
			encrypted = true;
	}

	CAnimatedMeshB3d* mesh = new CAnimatedMeshB3d( Driver, animated );

	LoadMeshData* data = new LoadMeshData();
	data->Filename = name;
	data->Mesh = mesh;
	data->Manager = this;
	data->Encrypted = encrypted;
	data->Driver = Driver;
	data->Success = true;
	data->CompletionCallback = completionCallback;
	data->UserData = userData;

	// new Async
	//bbdx2_ASyncInsertIOJob( name.c_str(), MESH_LOAD_TYPE, &LoadMeshTask, &LoadMeshSync, data, MESH_LOAD_PRIORITY );

	// Using temporary blocking reads, since the streaming is broken
	if(tempMeshLoaderBuffer == NULL)
		tempMeshLoaderBuffer = (char*)malloc(1024 * 1024 * 10);

	FILE* fp = fopen(name.c_str(), "rb");
	if( !fp )
	{
		return NULL;
	}

	fseek(fp, 0, SEEK_END);
	u32 len = ftell(fp);
	fseek(fp, 0, SEEK_SET);

	if( len > 1024 * 1024 * 10 )
	{
		GlobalFail("tempMeshLoaderBuffer is out of memory!");
	}

	fread(tempMeshLoaderBuffer, 1, len, fp);
	fclose(fp);

	ASyncJobDesc desc;
	desc.Buffer = tempMeshLoaderBuffer;
	desc.BufferSize = len;
	desc.UserData = data;

	if ( LoadMeshTask( &desc ) != 0 )
		LoadMeshSync( &desc );

	MeshCache->addMesh(name.c_str(), mesh);
	mesh->drop();

	return mesh;
}


//! gets an animateable mesh. loads it if needed. returned pointer must not be dropped.
/*IAnimatedMesh* CSceneManager::getMesh(const c8* filename, bool Animated)
{

	IAnimatedMesh* msh = 0;

	core::stringc name = filename;
	name.make_lower();
	
	msh = MeshCache->findMesh(name.c_str());
	if (msh)
		return msh;

	io::IReadFile* file = FileSystem->createAndOpenFile(name.c_str());
	if (!file)
	{
		return 0;
	}

	s32 count = MeshLoaderList.size();
	for (s32 i=count-1; i>=0; --i)
		if (MeshLoaderList[i]->isALoadableFileExtension(name.c_str()))
		{
			// reset file to avoid side effects of previous calls to createMesh
			file->seek(0);

			msh = MeshLoaderList[i]->createMesh(this, file, Animated, false, false);

			if (msh)
			{
				MeshCache->addMesh(name.c_str(), msh);
				msh->drop();
				break;
			}
		}

	file->drop();


	if(!msh)
		return 0;

//	if (!msh)
//		os::Printer::log("Could not load mesh, file format seems to be unsupported", filename, ELL_ERROR);
//	else
//		os::Printer::log("Loaded mesh", filename, ELL_INFORMATION);

	IMesh* tmpMesh = msh->getMesh(0);
	for(int f = 0; f < tmpMesh->getMeshBufferCount(); ++f)
	{
		
		if(msh->getMeshType() == EAMT_B3D)
			if(((IAnimatedMeshB3d*)msh)->IsAnimated())
				Driver->updateAnimatedHardwareBuffer(tmpMesh->getMeshBuffer(f), &(((IAnimatedMeshB3d*)msh)->VBlends));
			else
				Driver->updateHardwareBuffer(tmpMesh->getMeshBuffer(f));
		else
			Driver->updateHardwareBuffer(tmpMesh->getMeshBuffer(f));
	}

	return msh;

}*/

// IAnimatedMesh* CSceneManager::getMesh(io::IReadFile* file, bool animated, bool threaded)
// {
// 	IAnimatedMesh* msh = 0;
// 
// 	core::stringc name = file->getFileName();
// 	name.make_lower();
// 
// 	s32 count = MeshLoaderList.size();
// 	bool Found = false;
// 	for (s32 i=count-1; i>=0; --i)
// 		if (MeshLoaderList[i]->isALoadableFileExtension(name.c_str()))
// 		{
// 			Found = true;
// 			// reset file to avoid side effects of previous calls to createMesh
// 			file->seek(0);
// 
// 			msh = MeshLoaderList[i]->createMesh(this, file, animated, true, threaded);
// 
// 			if (msh)
// 			{
// 				MeshCache->addMesh(name.c_str(), msh);
// 				msh->drop();
// 				break;
// 			}
// 		}
// 
// 	file->drop();
// 
// 
// 	if(!msh)
// 		return 0;
// 
// //	if (!msh)
// //		os::Printer::log("Could not load mesh, file format seems to be unsupported", filename, ELL_ERROR);
// //	else
// //		os::Printer::log("Loaded mesh", filename, ELL_INFORMATION);
// 
// 	IMesh* tmpMesh = msh->getMesh(0);
// 	for(int f = 0; f < tmpMesh->getMeshBufferCount(); ++f)
// 	{
// 		
// 		if(msh->getMeshType() == EAMT_B3D)
// 			if(((IAnimatedMeshB3d*)msh)->IsAnimated())
// 				Driver->updateAnimatedHardwareBuffer(tmpMesh->getMeshBuffer(f), &(((IAnimatedMeshB3d*)msh)->VBlends));
// 			else
// 				Driver->updateHardwareBuffer(tmpMesh->getMeshBuffer(f));
// 		else
// 			Driver->updateHardwareBuffer(tmpMesh->getMeshBuffer(f));
// 	}
// 
// 	return msh;
// }


//! returns the video driver
video::IVideoDriver* CSceneManager::getVideoDriver()
{
	return Driver;
}

//! adds a scene node for rendering a static mesh
//! the returned pointer must not be dropped.
IMeshSceneNode* CSceneManager::addMeshSceneNode(IMesh* mesh, ISceneNode* parent, s32 id,
	const core::vector3df& position, const core::vector3df& rotation,
	const core::vector3df& scale, bool alsoAddIfMeshPointerZero)
{
	if (!alsoAddIfMeshPointerZero && !mesh)
		return 0;

	if (!parent)
		parent = this;

	IMeshSceneNode* node = new CMeshSceneNode(mesh, parent, this, id, position, rotation, scale);
	node->drop();

	return node;
}

//! adds a scene node for rendering an animated mesh model
IAnimatedMeshSceneNode* CSceneManager::addAnimatedMeshSceneNode(IAnimatedMesh* mesh, ISceneNode* parent, s32 id,
	const core::vector3df& position, const core::vector3df& rotation,
	const core::vector3df& scale, bool alsoAddIfMeshPointerZero)
{
	if (!alsoAddIfMeshPointerZero && !mesh)
		return 0;

	if (!parent)
		parent = this;

	

	IAnimatedMeshSceneNode* node =
		new CAnimatedMeshSceneNode(mesh, parent, this, id, position, rotation, scale);
	node->drop();

	node->setMaterialFlag(video::EMF_BACK_FACE_CULLING, true);
	if(mesh->getMeshType() == scene::EAMT_B3D)
	{
		if(((scene::IAnimatedMeshB3d*)mesh)->getEntityFXParam(32))
		{
			node->setMaterialType(video::EMT_SOLID_ALPHA);
		}
		node->FogOn = ((scene::IAnimatedMeshB3d*)mesh)->getEntityFXParam(8);
		node->LightOn = ((scene::IAnimatedMeshB3d*)mesh)->getEntityFXParam(1);
		node->ForceAlpha = ((scene::IAnimatedMeshB3d*)mesh)->getEntityFXParam(32);
		node->setMaterialFlag(video::EMF_BACK_FACE_CULLING, !((scene::IAnimatedMeshB3d*)mesh)->getEntityFXParam(16));
	}


	return node;
}


//! Adds a scene node for rendering using a octtree to the scene graph. This a good method for rendering
//! scenes with lots of geometry. The Octree is built on the fly from the mesh, much
//! faster then a bsp tree.
ISceneNode* CSceneManager::addOctTreeSceneNode(IAnimatedMesh* mesh, ISceneNode* parent,
			s32 id, s32 minimalPolysPerNode, bool alsoAddIfMeshPointerZero)
{
	if (!alsoAddIfMeshPointerZero && (!mesh || !mesh->getFrameCount()))
		return 0;

	return addOctTreeSceneNode(mesh ? mesh->getMesh(0) : 0,
							   parent, id, minimalPolysPerNode,
							   alsoAddIfMeshPointerZero);
}



//! Adss a scene node for rendering using a octtree. This a good method for rendering
//! scenes with lots of geometry. The Octree is built on the fly from the mesh, much
//! faster then a bsp tree.
ISceneNode* CSceneManager::addOctTreeSceneNode(IMesh* mesh, ISceneNode* parent,
											   s32 id, s32 minimalPolysPerNode,
											   bool alsoAddIfMeshPointerZero)
{
	if (!alsoAddIfMeshPointerZero && !mesh)
		return 0;

	if (!parent)
		parent = this;

	COctTreeSceneNode* node = new COctTreeSceneNode(parent, this, id, minimalPolysPerNode);

	if (mesh)
		node->createTree(mesh);

	node->drop();

	return node;
}


//! Adds a camera scene node to the tree and sets it as active camera.
//! \param position: Position of the space relative to its parent where the camera will be placed.
//! \param lookat: Position where the camera will look at. Also known as target.
//! \param parent: Parent scene node of the camera. Can be null. If the parent moves,
//! the camera will move too.
//! \return Returns pointer to interface to camera
ICameraSceneNode* CSceneManager::addCameraSceneNode(ISceneNode* parent,
	const core::vector3df& position, const core::vector3df& lookat, s32 id)
{
	if (!parent)
		parent = this;

	ICameraSceneNode* node = new CCameraSceneNode(parent, this, id, position, lookat);
	node->drop();

	setActiveCamera(node);

	return node;
}



//! Adds a camera scene node which is able to be controlle with the mouse similar
//! like in the 3D Software Maya by Alias Wavefront.
//! The returned pointer must not be dropped.
ICameraSceneNode* CSceneManager::addCameraSceneNodeMaya(ISceneNode* parent,
	f32 rotateSpeed, f32 zoomSpeed, f32 translationSpeed, s32 id)
{
	if (!parent)
		parent = this;

	ICameraSceneNode* node = new CCameraMayaSceneNode(parent, this, id, rotateSpeed,
		zoomSpeed, translationSpeed);
	node->drop();

	setActiveCamera(node);

	return node;
}



//! Adds a camera scene node which is able to be controled with the mouse and keys
//! like in most first person shooters (FPS):
ICameraSceneNode* CSceneManager::addCameraSceneNodeFPS(ISceneNode* parent,
	f32 rotateSpeed, f32 moveSpeed, s32 id,
	SKeyMap* keyMapArray, s32 keyMapSize, bool noVerticalMovement)
{
	if (!parent)
		parent = this;

	ICameraSceneNode* node = new CCameraFPSSceneNode(parent, this,
		id, rotateSpeed, moveSpeed, keyMapArray, keyMapSize, noVerticalMovement);
	node->drop();

	setActiveCamera(node);

	return node;
}

//! Adds an empty scene node.
ISceneNode* CSceneManager::addEmptySceneNode(ISceneNode* parent, s32 id)
{
	if (!parent)
		parent = this;

	ISceneNode* node = new CEmptySceneNode(parent, this, id);
	node->drop();

	return node;
}


//! Adds a dummy transformation scene node to the scene graph.
IDummyTransformationSceneNode* CSceneManager::addDummyTransformationSceneNode(
	ISceneNode* parent, s32 id)
{
	if (!parent)
		parent = this;

	IDummyTransformationSceneNode* node = new CDummyTransformationSceneNode(
		parent, this, id);
	node->drop();

	return node;
}

//! Returns the root scene node. This is the scene node wich is parent
//! of all scene nodes. The root scene node is a special scene node which
//! only exists to manage all scene nodes. It is not rendered and cannot
//! be removed from the scene.
//! \return Returns a pointer to the root scene node.
ISceneNode* CSceneManager::getRootSceneNode()
{
	return this;
}



//! Returns the current active camera.
//! \return The active camera is returned. Note that this can be NULL, if there
//! was no camera created yet.
ICameraSceneNode* CSceneManager::getActiveCamera()
{
	return ActiveCamera;
}



//! Sets the active camera. The previous active camera will be deactivated.
//! \param camera: The new camera which should be active.
void CSceneManager::setActiveCamera(ICameraSceneNode* camera)
{
	if (ActiveCamera)
		ActiveCamera->drop();

	ActiveCamera = camera;

	if (ActiveCamera)
		ActiveCamera->grab();
}



//! renders the node.
void CSceneManager::render()
{
}


//! returns the axis aligned bounding box of this node
const core::aabbox3d<f32>& CSceneManager::getBoundingBox() const
{
	_IRR_DEBUG_BREAK_IF(true) // Bounding Box of Scene Manager wanted.

	// should never be used.
	return *((core::aabbox3d<f32>*)0);
}

//JA Triangle based culling
bool CSceneManager::isTriangleCulled(IAnimatedMeshSceneNode* node, aabbox3d<f32> box)
{
	IAnimatedMesh* msh = node->getLocalMesh();
	IMesh* tmpMesh = msh->getMesh(0);
	
	vector3df VPos;
	VPos.set(0,0,0);
	video::S3DVertex2TCoords* vb2;
	video::S3DVertexTangents* vbt;
	


	for(int f = 0; f < tmpMesh->getMeshBufferCount(); ++f)
	{
		 Driver->updateHardwareBuffer(tmpMesh->getMeshBuffer(f));
		
		// Update bounding Box
		for(int j = 0; j < tmpMesh->getMeshBuffer(f)->getVertexCount(); ++j)
		{
			switch(tmpMesh->getMeshBuffer(f)->getVertexType())
			{
			case irr::video::EVT_STANDARD:
				VPos = ((SMeshBuffer*)tmpMesh->getMeshBuffer(f))->Vertices[j].Pos;
				break;
			case irr::video::EVT_2TCOORDS:
				vb2 = (video::S3DVertex2TCoords*)tmpMesh->getMeshBuffer(f)->getVertices();
				VPos = vb2[j].Pos;
				break;
			case irr::video::EVT_TANGENTS:
				vbt = (video::S3DVertexTangents*)tmpMesh->getMeshBuffer(f)->getVertices();
				VPos = vbt[j].Pos;
				break;
			}

			if(box.isPointInside(VPos) == true)
				return false;
		}
	}
	
	return true;
	
}

//! returns if node is culled
bool CSceneManager::isCulled(ISceneNode* node)
{
	ICameraSceneNode* cam = getActiveCamera();
	if (!cam)
		return false;

	if (!node->getAutomaticCulling())
		return false;

	if(node->TriangleCulling)
	{
		return isTriangleCulled((IAnimatedMeshSceneNode*)node,cam->getViewFrustrum()->boundingBox);
	}


	core::aabbox3d<f32> tbox = node->getBoundingBox();
	node->getAbsoluteTransformation().transformBoxEx(tbox);

	
	//if(node->getParent() && node->getParent() != this)
	//{
	//	matrix4 bmat = node->getAbsoluteTransformation();
	//	bmat.setRotationDegrees(vector3df(0,43,0));
	//	bmat.transformBox(tbox);
	//}

	return !(tbox.intersectsWithBox(cam->getViewFrustrum()->boundingBox));

	// This is a slower but more exact version:
	//
	// SViewFrustrum f = *cam->getViewFrustrum();
	// core::matrix4 invTrans;
	// node->getAbsoluteTransformation().getInverse(invTrans);
	// f.transform(invTrans);
	// return !(f.boundingBox.intersectsWithBox(node->getBoundingBox()));
}


//! registers a node for rendering it at a specific time.
void CSceneManager::registerNodeForRendering(ISceneNode* node, E_SCENE_NODE_RENDER_PASS time)
{
	for(int i = 0; i < 32; ++i)
	{
		if(RenderMasks[i] && (node->RenderMask & RenderPow[i]) > 0)
			return;
	}
	//return;
	//OutputDebugStringA("Reg\n");

	//char DBO[1024];
	//sprintf(DBO, "%x; Type: %i; Pass: %i;\n", node, node->getType(), time);
	//OutputDebugStringA(DBO);
	
	switch(time)
	{
	case ESNRP_LIGHT_AND_CAMERA:
		LightAndCameraList.push_back(node);
		break;
	case ESNRP_SKY_BOX:
		SkyBoxList.push_back(node);
		break;
	case ESNRP_SOLID:
		{
			if (!isCulled(node))
			{		
				//SolidNodeList.push_back(node);


				bool FoundHome = false;
				bool HasAlpha = false;

				// Check if the node has an alpha surface
				if(node->getType() == scene::ESNT_ANIMATED_MESH)
				{
					IMesh* mesh = ((IAnimatedMeshSceneNode*)node)->getLocalMesh()->getMesh(((IAnimatedMeshSceneNode*)node)->getFrameNr());
					for(s32 i = 0; i < mesh->getMeshBufferCount(); ++i)
						if(node->RenderTimes[i] != 5)
							node->RenderTimes[i] = 3;
						else
							HasAlpha = true;
				}

				// JB: Removed in 2.41 - Batched meshes are slower than just a random list.
				// Its an animated mesh
// 				if(node->getType() == scene::ESNT_ANIMATED_MESH && HasAlpha == false)
// 				{
// 					IAnimatedMesh* LMesh = ((IAnimatedMeshSceneNode*)node)->getLocalMesh();
// 
// 					// Don't batch animated meshes!!
// 					if(LMesh->getMeshType() == scene::EAMT_B3D)
// 					{
// 						if(((IAnimatedMeshB3d*)LMesh)->IsAnimated() == false)
// 						{
// 
// 							// Find a batch
// 							for(u32 i = 0; i < this->BatchSolidNodes.size(); ++i)
// 							{
// 								if(this->BatchSolidNodes[i].Effect == node->getEffect() && ((IAnimatedMeshSceneNode*)this->BatchSolidNodes[i].Nodes[0])->getLocalMesh() == LMesh)
// 								{
// 									// Add to batch
// 									this->BatchSolidNodes[i].Nodes.push_back(node);
// 									FoundHome = true;
// 								}
// 							}
// 
// 							if(FoundHome == false)
// 							{
// 								for(u32 i = 0; i < this->SolidNodes.size(); ++i)
// 								{
// 									if(SolidNodes[i].Effect == node->getEffect())
// 									{
// 										for(u32 f = 0; f < this->SolidNodes[i].Nodes.size(); ++f)
// 										{
// 											if(this->SolidNodes[i].Nodes[f]->getType() == ESNT_ANIMATED_MESH && ((IAnimatedMeshSceneNode*)this->SolidNodes[i].Nodes[f])->getLocalMesh() == LMesh)
// 											{
// 												SolidNodeGroup SNG;
// 												SNG.Effect = node->getEffect();
// 												SNG.Nodes.push_back(this->SolidNodes[i].Nodes[f]);
// 												SNG.Nodes.push_back(node);
// 												this->SolidNodes[i].Nodes.erase(f);
// 												this->BatchSolidNodes.push_back(SNG);
// 
// 												FoundHome = true;
// 											}
// 										}
// 									}
// 								}
// 							}
// 						}
// 					}
// 			
// 				}


				//if(strcmp(node->getTag(), "WATCHME") == 0)
				//{
				//	char OO[1024];
				//	sprintf(OO, "%x: registerSolid()\n", node);
				//	OutputDebugString(OO);
				//}

				// No batches for this lonely node
				if(FoundHome == false)
				{
					for(u32 i = 0; i < this->SolidNodes.size(); ++i)
					{
						if(SolidNodes[i].Effect == node->getEffect())
						{
							SolidNodes[i].Nodes.push_back(node);
							FoundHome = true;
						}
					}
				}

				// No effect groups for it
				if(FoundHome == false)
				{
					SolidNodeGroup SNG;
					SNG.Effect = node->getEffect();
					SNG.Nodes.push_back(node);
					SolidNodes.push_back(SNG);
				}
			}

			if(ShadowLevel > 0 && node->getShadowLevel() >= ShadowLevel)
			{
				//vector3df CamPos = this->getActiveCamera()->getAbsolutePosition();
				//vector3df NodePos = node->getAbsolutePosition();
				//f32 Dis = (f32)NodePos.getDistanceFromSQ(CamPos);

				core::aabbox3d<f32> tbox = node->getBoundingBox();
				node->getAbsoluteTransformation().transformBoxEx(tbox);
				NGin::Math::AABB NBox(NGin::Math::Vector3(tbox.MinEdge.X, tbox.MinEdge.Y, tbox.MinEdge.Z),
					NGin::Math::Vector3(tbox.MaxEdge.X, tbox.MaxEdge.Y, tbox.MaxEdge.Z));

				//bool InShadow = (tbox.intersectsWithBox(LightFrustum.boundingBox));
				bool InShadow = LightFrustum.BoxInFrustum(NBox);

				if(InShadow)
				{
					u32 NodeShadow = node->getShadowShader();
					if(NodeShadow == 0)
						NodeShadow = ShadowShader;

					bool FoundHome = false;

					if(node->getType() == scene::ESNT_ANIMATED_MESH)
					{
						IMesh* mesh = ((IAnimatedMeshSceneNode*)node)->getLocalMesh()->getMesh(((IAnimatedMeshSceneNode*)node)->getFrameNr());
						for(s32 i = 0; i < mesh->getMeshBufferCount(); ++i)
							if(node->RenderTimes[i] != 5)
								node->RenderTimes[i] = 3;
					}

					for(u32 i = 0; i < this->SolidShadowNodes.size(); ++i)
					{
						if(SolidShadowNodes[i].Effect == NodeShadow)
						{
							SolidShadowNodes[i].Nodes.push_back(node);
							FoundHome = true;
						}
					}

					if(FoundHome == false)
					{
						SolidNodeGroup SNG;
						SNG.Effect = NodeShadow;
						SNG.Nodes.push_back(node);
						SolidShadowNodes.push_back(SNG);
					}
				}
			}

			//vector3df CamPos = this->getActiveCamera()->getAbsolutePosition();
			//vector3df NodePos = node->getAbsolutePosition();
			//f32 Dis = (f32)NodePos.getDistanceFromSQ(CamPos);

			//if(Dis < ShadowDistance)
			//{
			//	bool FoundHome = false;

			//	if(node->getType() == scene::ESNT_ANIMATED_MESH)
			//	{
			//		IMesh* mesh = ((IAnimatedMeshSceneNode*)node)->getLocalMesh()->getMesh(((IAnimatedMeshSceneNode*)node)->getFrameNr());
			//		for(s32 i = 0; i < mesh->getMeshBufferCount(); ++i)
			//			if(node->RenderTimes[i] != 5)
			//				node->RenderTimes[i] = 3;
			//	}

			//	for(u32 i = 0; i < this->SolidShadowNodes.size(); ++i)
			//	{
			//		if(SolidShadowNodes[i].Effect == ShadowShader)
			//		{
			//			SolidShadowNodes[i].Nodes.push_back(node);
			//			FoundHome = true;
			//		}
			//	}

			//	if(FoundHome == false)
			//	{
			//		SolidNodeGroup SNG;
			//		SNG.Effect = this->ShadowShader;
			//		SNG.Nodes.push_back(node);
			//		SolidShadowNodes.push_back(SNG);
			//	}
			//}


		}
		break;
	case ESNRP_TRANSPARENT:
		{
			if (!isCulled(node))
			{
				//TransparentNodeList.push_back(TransparentNodeEntry(node, camTransPos));
				
				bool FoundHome = false;

				// Some Locals
				TransparentNode TN;
				TN.Node = node;
				aabbox3d<f32> tbox = node->getBoundingBox();
				node->getAbsoluteTransformation().transformBox(tbox);
				
				// Get the X/Y/Z Distance of each point
				vector3df P1 = (tbox.MinEdge - camTransPos);
				vector3df P2 = ((tbox.getExtent()/2)) - camTransPos;
				vector3df P3 = (tbox.MaxEdge - camTransPos);

				TN.Distance = (((pow(P1.X,2) + pow(P1.Y,2) + pow(P1.Z,2))
					+(pow(P2.X,2) + pow(P2.Y,2) + pow(P2.Z,2))
					+(pow(P3.X,2) + pow(P3.Y,2) + pow(P3.Z,2)))/3);

				
				//TN.Distance = (pow((P.X - camTransPos.X),2)) 
				//	+ (pow((P.Y - camTransPos.Y),2)) 
				//	+ (pow((P.Z - camTransPos.Z),2)); 

				// Register this node as semi-solid?
				bool SemiSolid = false;


				if(node->getType() == scene::ESNT_ANIMATED_MESH)
				{
					IMesh* mesh = ((IAnimatedMeshSceneNode*)node)->getLocalMesh()->getMesh(((IAnimatedMeshSceneNode*)node)->getFrameNr());
					for(s32 i = 0; i < mesh->getMeshBufferCount(); ++i)
						if(mesh->getMeshBuffer(i)->TextureAlpha || mesh->getMeshBuffer(i)->VertexAlpha || node->ParticleAlpha)
							node->RenderTimes[i] = 5;
						else if(node->Alpha > 0.95f)
						{
							// Its really solid :O
							node->RenderTimes[i] = -1;

							// We have to add it to the proper solid list as well
							SemiSolid = true;
						}else
							node->RenderTimes[i] = 5;
				}

				if(SemiSolid)
				{
					//if(strcmp(node->getTag(), "WATCHME") == 0)
					//	OutputDebugString("Pushing through Solid Pass\n");
					registerNodeForRendering(node,ESNRP_SOLID);
				}else
				{
					if(ShadowLevel > 0 && node->getShadowLevel() >= ShadowLevel)
					{
						//vector3df CamPos = this->getActiveCamera()->getAbsolutePosition();
						//vector3df NodePos = node->getAbsolutePosition();
						//f32 Dis = (f32)NodePos.getDistanceFromSQ(CamPos);

						core::aabbox3d<f32> tbox = node->getBoundingBox();
						node->getAbsoluteTransformation().transformBoxEx(tbox);
						NGin::Math::AABB NBox(NGin::Math::Vector3(tbox.MinEdge.X, tbox.MinEdge.Y, tbox.MinEdge.Z),
							NGin::Math::Vector3(tbox.MaxEdge.X, tbox.MaxEdge.Y, tbox.MaxEdge.Z));

						//bool InShadow = (tbox.intersectsWithBox(LightFrustum.boundingBox));
						bool InShadow = LightFrustum.BoxInFrustum(NBox);

						if(InShadow)
						{
							u32 NodeShadow = node->getShadowShader();
							if(NodeShadow == 0)
								NodeShadow = ShadowShader;

							bool FoundHome = false;

							if(node->getType() == scene::ESNT_ANIMATED_MESH)
							{
								IMesh* mesh = ((IAnimatedMeshSceneNode*)node)->getLocalMesh()->getMesh(((IAnimatedMeshSceneNode*)node)->getFrameNr());
								for(s32 i = 0; i < mesh->getMeshBufferCount(); ++i)
									if(node->RenderTimes[i] != 5)
										node->RenderTimes[i] = 3;
							}

							for(u32 i = 0; i < this->SolidShadowNodes.size(); ++i)
							{
								if(SolidShadowNodes[i].Effect == NodeShadow)
								{
									SolidShadowNodes[i].Nodes.push_back(node);
									FoundHome = true;
								}
							}

							if(FoundHome == false)
							{
								SolidNodeGroup SNG;
								SNG.Effect = NodeShadow;
								SNG.Nodes.push_back(node);
								SolidShadowNodes.push_back(SNG);
							}
						}
					}
				}

				//if(strcmp(node->getTag(), "WATCHME") == 0)
				//{
				//	char OO[1024];
				//	sprintf(OO, "%x: registerTransparent()\n", node);
				//	OutputDebugString(OO);
				//}

				
				for(u32 g = 0; g < this->TransparentNodes.size(); ++g)
				{
					if(TN.Distance > TransparentNodes[g].Distance)
					{
						TransparentNodes.insert(TN,g);
						FoundHome = true;
						break;
					}
				}

				if(FoundHome == false)
				{
					TransparentNodes.push_back(TN);
				}
			
			}

			return;

			vector3df CamPos = this->getActiveCamera()->getAbsolutePosition();
			vector3df NodePos = node->getAbsolutePosition();
			f32 Dis = (f32)NodePos.getDistanceFromSQ(CamPos);

			if(Dis < ShadowDistance)
			{
				bool FoundHome = false;

				// Some Locals
				TransparentNode TN;
				TN.Node = node;
				aabbox3d<f32> tbox = node->getBoundingBox();
				node->getAbsoluteTransformation().transformBox(tbox);
				
				// Get the X/Y/Z Distance of each point
				vector3df P1 = (tbox.MinEdge - camTransPos);
				vector3df P2 = ((tbox.getExtent()/2)) - camTransPos;
				vector3df P3 = (tbox.MaxEdge - camTransPos);

				TN.Distance = (((pow(P1.X,2) + pow(P1.Y,2) + pow(P1.Z,2))
					+(pow(P2.X,2) + pow(P2.Y,2) + pow(P2.Z,2))
					+(pow(P3.X,2) + pow(P3.Y,2) + pow(P3.Z,2)))/3);

				
				//TN.Distance = (pow((P.X - camTransPos.X),2)) 
				//	+ (pow((P.Y - camTransPos.Y),2)) 
				//	+ (pow((P.Z - camTransPos.Z),2)); 

				// Register this node as semi-solid?
				bool SemiSolid = false;

				if(node->getType() == scene::ESNT_ANIMATED_MESH)
				{
					IMesh* mesh = ((IAnimatedMeshSceneNode*)node)->getLocalMesh()->getMesh(((IAnimatedMeshSceneNode*)node)->getFrameNr());
					for(s32 i = 0; i < mesh->getMeshBufferCount(); ++i)
						if(mesh->getMeshBuffer(i)->TextureAlpha || mesh->getMeshBuffer(i)->VertexAlpha)
							node->RenderTimes[i] = 5;
						else if(node->Alpha > 0.95f)
						{
							// Its really solid :O
							node->RenderTimes[i] = -1;

							// We have to add it to the proper solid list as well
							SemiSolid = true;
						}else
							node->RenderTimes[i] = 5;
				}

				if(SemiSolid)
					registerNodeForRendering(node, ESNRP_SOLID);

				
				for(u32 g = 0; g < this->TransparentShadowNodes.size(); ++g)
				{
					if(TN.Distance > TransparentShadowNodes[g].Distance)
					{
						TransparentShadowNodes.insert(TN,g);
						FoundHome = true;
						break;
					}
				}

				if(FoundHome == false)
				{
					TransparentShadowNodes.push_back(TN);
				}
			}
		}
		break;
	case ESNRP_AUTOMATIC:
		
		break;
	case ESNRP_SHADOW:
		if (!isCulled(node))
			ShadowNodeList.push_back(node);
		break;
	case ESNRP_COUNT: // ignore this one
		break;
	}
}

void CSceneManager::updateDirectionalLights()
{
	// Get Video Driver!
	irr::video::CD3D9Driver* services = (irr::video::CD3D9Driver*)getVideoDriver();

}

irr::s32 CSceneManager::addPointLight()
{
	NPointLight* nl = new NPointLight();
	this->PointLightsList.push_back(nl);
	return (irr::s32)nl;
}

irr::s32 CSceneManager::addDirectionalLight()
{
	NDirectionalLight* nl = new NDirectionalLight();
	this->DirectionalLightsList.push_back(nl);
	updateDirectionalLights();
	return (irr::s32)nl;
}

void** CSceneManager::GetPointLights(int* Count, unsigned int* Stride)
{
	*Count = this->PointLightsList.size();
	*Stride = sizeof(NPointLight);

	return (void**)this->PointLightsList.pointer();
}

void CSceneManager::GetDirectionalLights(float** Directions, float** Colors)
{
	// Directional Lights
	int j = 0;
	for(u32 f = 0; f < this->DirectionalLightsList.size(); ++f)
	{
		if(this->DirectionalLightsList[f]->Active == true)
		{
			core::vector3df pos = this->DirectionalLightsList[f]->Normal;
			video::SColorf col = this->DirectionalLightsList[f]->Color;
			
			Directions[j][0] = pos.X;
			Directions[j][1] = pos.Y;
			Directions[j][2] = pos.Z;
			Colors[j][0] = col.r;
			Colors[j][1] = col.g;
			Colors[j][2] = col.b;

			++j;
			if( j == 3 ) break;
		}
	}
}

void CSceneManager::setLightPosition(irr::s32 light, irr::core::vector3df position)
{
	if(light == 0 || light == -1)
		return;

	((NPointLight*)light)->Position = position;
}

void CSceneManager::setLightDirection(irr::s32 light, irr::core::vector3df direction)
{
	if(light == 0 || light == -1)
		return;

	((NDirectionalLight*)light)->Normal = direction;
	updateDirectionalLights();
}

void CSceneManager::setLightRadius(irr::s32 light, irr::s32 radius)
{
	if(light == 0 || light == -1)
		return;

	((NPointLight*)light)->Radius = radius;
}

void CSceneManager::setPLightColor(irr::s32 light, irr::video::SColorf color)
{
	if(light == 0 || light == -1)
		return;

	((NPointLight*)light)->Color = color;
}

void CSceneManager::setDLightColor(irr::s32 light, irr::video::SColorf color)
{
	if(light == 0 || light == -1)
		return;

	((NDirectionalLight*)light)->Color = color;
	updateDirectionalLights();
}

void CSceneManager::setPLightActive(irr::s32 light, bool active )
{
	if(light == 0 || light == -1)
		return;

	((NPointLight*)light)->Active = active;
}

void CSceneManager::setDLightActive(irr::s32 light, bool active )
{
	if(light == 0 || light == -1)
		return;

	((NDirectionalLight*)light)->Active = active;
	updateDirectionalLights();
}

void CSceneManager::FreePLight(irr::s32 light)
{
	NPointLight* nl = (NPointLight*)light;

	int id = -1;
	for(int i = 0; i < this->PointLightsList.size(); ++i)
		if(this->PointLightsList[i] == nl)
			id = i;
	
	if(id > -1)
		this->PointLightsList.erase(id);

	if(nl != 0)
		delete nl;
}

void CSceneManager::FreeDLight(irr::s32 light)
{
	NDirectionalLight* nl = (NDirectionalLight*)light;
	
	int id = -1;
	for(int i = 0; i < this->DirectionalLightsList.size(); ++i)
		if(this->DirectionalLightsList[i] == nl)
			id = i;

	if(id > -1)
		this->DirectionalLightsList.erase(id);

	if(nl != 0)
		delete nl;
	updateDirectionalLights();
}


irr::f32 CSceneManager::CalcDistance(irr::core::vector3df P1, irr::core::vector3df P2)
{
	irr::core::vector3df Dist = P1-P2;

	return (Dist.X * Dist.X) + (Dist.Y * Dist.Y) + (Dist.Z + Dist.Z);
}

int CSceneManager::GetLights(NPointLight** Lights, vector3df& Position)
{
	int Pos = 0;

	irr::core::array<NPointLight*> Tops;

	for(u32 f = 0; f < PointLightsList.size(); ++f)
	{
		NPointLight* Light = PointLightsList[f];

		if(Light->Active)
		{
			Light->Distance = Position.getDistanceFromSQ(Light->Position);

			bool Found = false;

			for(u32 i = 0; i < Tops.size(); ++i)
				if(Light->Distance < Tops[i]->Distance)
				{
					Tops.insert(Light, i);
					Found = true;
					break;
				}

			if(!Found)
				Tops.push_back(Light);
		}
	}

	u32 CopySize = (Tops.size() > 5) ? 5 : Tops.size();

	for(u32 i = 0; i < CopySize; ++i)
		Lights[i] = Tops[i];

	return CopySize;


	// Setup the five active point lights
	for( u32 f = 0; f < this->PointLightsList.size(); ++f)
	{
		// If the point light is active
		if(this->PointLightsList[f]->Active == true)
		{
			// Set its distance
			this->PointLightsList[f]->Distance = this->CalcDistance( Position, this->PointLightsList[f]->Position );
			
			// Use this to track where it went
			bool Inserted = false;

			// If there are no active lights
			if(this->ActiveLightsList.size() == 0)
			{
				// Make this the first active light
				Lights[Pos] = this->PointLightsList[f];
				++Pos;if(Pos > 4) Pos = 4;
				Inserted = true;
			}
			else
			{
				// Loop through each light we already have
				for(u32 d = 0; d < Pos; ++d)
				{
					// If it is closer than this light
					if(this->PointLightsList[f]->Distance < this->ActiveLightsList[d]->Distance)
					{
						++Pos;if(Pos > 4) Pos = 4;
						for(u32 i = Pos; i > d; ++i)
							Lights[i] = Lights[i - 1];
						Lights[d] = this->PointLightsList[f];

						Inserted = true;
					}
				}
			}
			
			// If it was never inserted and there aren't enough lights already
			if(Inserted == false && this->ActiveLightsList.size() < 5)
			{
				Lights[Pos] = this->PointLightsList[f];
				++Pos;if(Pos > 4) Pos = 4;
			}
		}
	}

	return Pos;
}

inline D3DXVECTOR3& Mat4GetRight( D3DXMATRIX& Mat )		{ return *((D3DXVECTOR3*)&(Mat)._11); }
inline D3DXVECTOR3& Mat4GetUp( D3DXMATRIX& Mat )		{ return *((D3DXVECTOR3*)&(Mat)._21); }
inline D3DXVECTOR3& Mat4GetForward( D3DXMATRIX& Mat )	{ return  *((D3DXVECTOR3*)&(Mat)._31); }
inline D3DXVECTOR3& Mat4GetPosition( D3DXMATRIX& Mat )	{ return  *((D3DXVECTOR3*)&(Mat)._41); }

void Mat4SetRight(  D3DXMATRIX &Mat, D3DXVECTOR3 &V  )		{ Mat._11 = V.x; Mat._12 = V.y; Mat._13 = V.z;  }
void Mat4SetUp(  D3DXMATRIX &Mat, D3DXVECTOR3 &V  )			{ Mat._21 = V.x; Mat._22 = V.y; Mat._23 = V.z;  }
void Mat4SetForward(  D3DXMATRIX &Mat, D3DXVECTOR3 &V  )	{ Mat._31 = V.x; Mat._32 = V.y; Mat._33 = V.z;  }
void Mat4SetPosition(  D3DXMATRIX &Mat, D3DXVECTOR3 &V  )	{ Mat._41 = V.x; Mat._42 = V.y; Mat._43 = V.z;  }

//! This method is called just before the rendering process of the whole scene.
//! draws all scene nodes
void CSceneManager::drawAll()
{
	if (!Driver)
		return;

	for(int i = 0; i < 32; ++i)
	{
		RenderMasks[i] = (ISceneManager::RenderMask & RenderPow[i]) > 0;
	}


	
	// Get Driver
	irr::video::CD3D9Driver* services = (irr::video::CD3D9Driver*)getVideoDriver();



	//irr::video::CD3D9HLSLMaterialRenderer* LastShader = 0;
	//irr::video::CD3D9HLSLMaterialRenderer* CurrentShader;

	u32 CurrentShader = 0;
	u32 LastShader = 0;

	irr::core::vector3df lpos;
	irr::video::SColorf lcol;
	irr::f32 lradius;


	core::matrix4 AnimationMatrices[ANIMATIONMATRIXCOUNT];

	ISceneNode* LastObject = 0;

	// calculate camera pos.
	camTransPos.set(0,0,0);
	if (ActiveCamera)
		camTransPos = ActiveCamera->getAbsolutePosition();


#pragma region Compute shadows for pre-render
StartDebugTimer("B-Scene (Shd)");
	vector3df LightNormal;
	vector3df LightPosition;
	//video::ITexture* ShadowMap = 0;
	IDirect3DTexture9* ShadowMap = 0;

	bool Found = false;
	for(int i = 0; i < this->DirectionalLightsList.size(); ++i)
		if(DirectionalLightsList[i]->Active)
		{
			LightNormal = DirectionalLightsList[i]->Normal;
			Found = true;
			break;
		}

	if(!Found)
		LightNormal.set(0,-1,0);
	//LightNormal.set(1,-1,0);
	//LightNormal.normalize();

	//LightDistance = 100.0f;
	//ShadowDistance = 26.0f;

	// Make a position
	LightNormal.invert();
	LightPosition = getActiveCamera()->getAbsolutePosition() + (LightNormal * (LightDistance * 0.5f));

	D3DXMATRIX LightProj, LightView, LightViewProj;
	//D3DXMatrixPerspectiveFovLH( &LightProj, D3DX_PI / 2.0f, 1, 1.0f, 1000.0f );

	D3DXMatrixOrthoLH(&LightProj, ShadowDistance, ShadowDistance, 1.0f, LightDistance);

	LightNormal.invert();

	core::vector3df vRight = LightNormal.crossProduct(core::vector3df(0, 1, 0));
	if(vRight.getLength() == 0.0)
		vRight = LightNormal.crossProduct(core::vector3df(0, 0, 1));

	core::vector3df vUp = vRight.crossProduct(LightNormal);
	vUp.normalize();

	

	D3DXVECTOR3 LightUp(vUp.X, vUp.Y, vUp.Z);
	D3DXVECTOR3 LightLookAt(
		LightPosition.X + LightNormal.X, 
		LightPosition.Y + LightNormal.Y, 
		LightPosition.Z + LightNormal.Z);
	D3DXMatrixLookAtLH(&LightView, reinterpret_cast<const D3DXVECTOR3*>(&LightPosition), &LightLookAt, &LightUp);

	D3DXMatrixMultiply(&LightViewProj, &LightView, &LightProj);

	core::matrix4 LightProjection;
	memcpy(&LightProjection, &LightViewProj, sizeof(float) * 16);

	LightFrustum.FromViewProjection(LightViewProj);
	memcpy(&LightMatrix, &LightViewProj, sizeof(float) * 16);
	memcpy(&TLightView, &LightView, sizeof(float) * 16);
	memcpy(&TLightProj, &LightProj, sizeof(float) * 16);

#pragma endregion Compute shadows for pre-render

	// let all nodes register themselves
	services->getDefaultTexture()->ReferenceCounter = 65535;

	//OutputDebugStringA("Start PR\n");
//	unsigned int PS = timeGetTime();
	OnPreRender();
// 	unsigned int Diff = timeGetTime() - PS;
// 	printf("OnPre: %i\n", Diff);
	//OutputDebugStringA("End PR\n");

	//render lights and cameras

	CurrentRendertime = ESNRP_LIGHT_AND_CAMERA;

	Driver->deleteAllDynamicLights();

	u32 i; // new ISO for scoping problem in some compilers

	for (i=0; i<LightAndCameraList.size(); ++i)
		LightAndCameraList[i]->render();

	// Draw a bounding box


	LightAndCameraList.clear();

	irr::video::SColorf col1 = services->AmbientLight;

	// Precalc projection matrix
	irr::core::matrix4 ViewProj;
	ViewProj = services->getTransform(irr::video::ETS_PROJECTION);			
	ViewProj *= services->getTransform(irr::video::ETS_VIEW);

	// view Inverse
	irr::core::matrix4 ViewInverse = services->getTransform(irr::video::ETS_VIEW);
	ViewInverse.makeInverse();

	// View
	core::matrix4 View = services->getTransform(video::ETS_VIEW);

	// Projection
	core::matrix4 Projection = services->getTransform(video::ETS_PROJECTION);

	// Timer!
	irr::f32 Timer = (irr::f32)Device->getTimer()->getTime();

	vector3df CamPos = getActiveCamera()->getPosition();

	core::vector2df FogNearFar(services->FogStart,services->FogEnd);
	video::SColorf FogCol( (services->FogColor.getRed() / 255.0f), (services->FogColor.getGreen() / 255.0f), (services->FogColor.getBlue() / 255.0f));
	
	// Setup basic constants and reset all effects
	for(int i = 1; i < services->EffectsList.size(); ++i)
	{
		services->EffectsList[i]->Reset();
		services->EffectsList[i]->SetView(View);
		services->EffectsList[i]->SetProjection(Projection);
		services->EffectsList[i]->SetViewInverse(ViewInverse);
		services->EffectsList[i]->SetViewProjection(ViewProj);
		services->EffectsList[i]->SetCameraPosition(CamPos);
		services->EffectsList[i]->SetTime(Timer);
		services->EffectsList[i]->SetLightAmbient(col1);
		services->EffectsList[i]->SetFogColor(FogCol);
		services->EffectsList[i]->SetFogData(FogNearFar);

		core::rect<s32> vp = services->getViewPort();
		services->EffectsList[i]->SetViewPort(vector2df(vp.getWidth(), vp.getHeight()));
	}

	/*
	* #####  #   #   ###   ####    ###   #   #
	* #      #   #  #   #  #   #  #   #  #   #
	* #####  #####  #####  #   #  #   #  # # #
	*     #  #   #  #   #  #   #  #   #  # # #
	* #####  #   #  #   #  ####    ###    # # 
	*/
#pragma region Shadows


	CurrentRendertime = ESNRP_SOLID;



	//JB: Enable Shadows here!
	if(ShadowLevel != 0 && Found)
	{
		services->UnSetBackBuffer2();
		services->SetShadowTarget();

		services->setTransformN(video::ETS_VIEW, *(matrix4*)&LightView);
		getActiveCamera()->setViewMatrix( *(matrix4*)&LightView);

		
		for(i = 0; i < this->SolidShadowNodes.size(); ++i)
		{
			CurrentShader = this->SolidShadowNodes[i].Effect;

			if(CurrentShader)
			{

				// Get Effect
				ID3DXEffect* Effect = services->getEffect(CurrentShader);
				ShaderConstantSet* CurrentSet = services->EffectsList[CurrentShader];

				// Set Effect
				services->setShader(CurrentShader);

				CurrentSet->SetLightProjection(LightProjection);
				
				// Begin	
				UINT NumPasses = 0;
				Effect->Begin(&NumPasses,0);
				if(FAILED(Effect->BeginPass(0)))
					break;

				for(u32 g = 0; g < this->SolidShadowNodes[i].Nodes.size(); ++g)
				{
					CurrentSet->SetAlpha(SolidShadowNodes[i].Nodes[g]->Alpha);

					core::matrix4 World = SolidShadowNodes[i].Nodes[g]->getAbsoluteTransformation();
					CurrentSet->SetWorld(World);

//					core::matrix4 TempWVP = (*(matrix4*)&LightViewProj) * World;
					core::matrix4 TempWVP = ViewProj * World;
					CurrentSet->SetWorldViewProjection(TempWVP);
					/*CurrentSet->SetViewProjection(*(matrix4*)&LightViewProj);
					CurrentSet->SetView(*(matrix4*)&LightView);*/

					bool IsAnimated = false;

					// Animations
					if(SolidShadowNodes[i].Nodes[g]->getType() == ESNT_ANIMATED_MESH)
					{
						CAnimatedMeshSceneNode* N = (CAnimatedMeshSceneNode*)SolidShadowNodes[i].Nodes[g];
						IAnimatedMesh* M = N->getLocalMesh();
						if(M->getMeshType() == scene::EAMT_B3D)
							if(((IAnimatedMeshB3d*)M)->IsAnimated())
								if(CurrentSet->HasBones())
								{
									// Check their size
									if(N->MatNodes.size() > ANIMATIONMATRIXCOUNT)
									{
										MessageBox(NULL,"Error: AnimatedMesh Bonecount exceeds the limit that BBDX can handle","Runtime Error",MB_ICONERROR | MB_OK);
										exit(0);
									}

									CurrentSet->SetBones(N->MatNodes.pointer(), N->MatNodes.size());

									IsAnimated = true;
								}
					}

					// Notify, if we must
					CurrentSet->SetIsAnimated(IsAnimated);

					// render
 					Effect->CommitChanges();
 					SolidShadowNodes[i].Nodes[g]->render();
				}

				Effect->EndPass();
				Effect->End();
			}
		}

		

		CallShadowMapCallback(getLightMatrix(), getLightView(), getLightProjection());

		services->UnSetShadowTarget();
		ShadowMap = services->GetShadowTexture(false);

		if(ShadowBlurShader[0] > 0 && ShadowBlurShader[1] > 0)
		{
			for(int i = 0; i < 2; ++i)
			{
				services->setShader(ShadowBlurShader[i]);
				ID3DXEffect* Effect = services->getEffect(ShadowBlurShader[i]);

				D3DSURFACE_DESC Desc;
				ShadowMap->GetLevelDesc(0, &Desc);

				D3DXVECTOR4 vDimensions(1.0f, 1.0f, 1.0f / (float)Desc.Width, 1.0f / (float)Desc.Height);

				if(i == 0)
					services->SetBlurShadowTarget();
				else
					services->SetShadowTarget();

				Effect->SetVector("vDimensions", &vDimensions);

				D3DXHANDLE TexHandle = Effect->GetParameterBySemantic(NULL, "TextureStage0");
				Effect->SetTexture(TexHandle, ShadowMap);
				Effect->CommitChanges();

				UINT PassCount = 0;
				Effect->Begin(&PassCount, 0);
				Effect->BeginPass(0);

				//services->pID3DDevice->SetFVF(SAQUADFVF);
				services->pID3DDevice->SetVertexDeclaration(SAVertDecl);
				services->pID3DDevice->SetStreamSource(0, FullScreenQuad, 0, sizeof(SAVert));

				services->pID3DDevice->DrawPrimitive(D3DPT_TRIANGLESTRIP, 0, 2);

				Effect->EndPass();
				Effect->End();

				Effect->SetTexture(TexHandle, NULL);
				Effect->CommitChanges();

				if(i == 0)
				{
					services->UnSetBlurShadowTarget();
					ShadowMap = services->GetShadowTexture(true);
				}
				else
				{
					services->UnSetShadowTarget();
					ShadowMap = services->GetShadowTexture(false);
				}
			}
		}

		services->SetBackBuffer2();

		services->setTransformN(video::ETS_VIEW, View);
		getActiveCamera()->setViewMatrix( View);

	}



StopDebugTimer();
StartDebugTimer("B-SceneEarly");

	// render default objects
	CurrentRendertime = ESNRP_TRANSPARENT;

	video::SColorf cc = video::SColorf(1.0f,1.0,0.0f,1.0f);
	for(int i = 1; i < services->EffectsList.size(); ++i)
	{
		services->EffectsList[i]->SetPassC(cc);
	}

	for(i = 0; i < FirstNodeList.size(); ++i)
	{
		if(FirstNodeList[i]->isVisible())
		{
			CurrentShader = FirstNodeList[i]->getEffect();

			if(CurrentShader)
			{
				// Get Effect
				ID3DXEffect* Effect = services->getEffect(CurrentShader);
				ShaderConstantSet* CurrentSet = services->EffectsList[CurrentShader];

				// Set Effect
				services->setShader(CurrentShader);

				// Set the fog if required
				if(FirstNodeList[i]->FogOn == true)
				{
					FogNearFar.set(0xFFFE, 0xFFFF);
					CurrentSet->SetFogColor(FogCol);
					CurrentSet->SetFogData(FogNearFar);
				}else
				{
					FogNearFar.set(services->FogStart,services->FogEnd);
					CurrentSet->SetFogColor(FogCol);
					CurrentSet->SetFogData(FogNearFar);
				}

				core::matrix4 World = FirstNodeList[i]->getAbsoluteTransformation();
				CurrentSet->SetWorld(World);
				core::matrix4 TempWVP = ViewProj * World;
				CurrentSet->SetWorldViewProjection(TempWVP);
				CurrentSet->SetAlpha(FirstNodeList[i]->Alpha);
				CurrentSet->SetSceneNodeConstants(FirstNodeList[i]);

				// Render!
				UINT NumPasses;
				Effect->Begin(&NumPasses, 0);
				for(UINT Pass = 0; Pass < NumPasses; ++Pass)
				{
					if(FAILED(Effect->BeginPass(Pass)))
							break;
					Effect->CommitChanges();
					FirstNodeList[i]->render();
					Effect->EndPass();
				}
				Effect->End();
			}
		}
	}
StopDebugTimer();

#pragma endregion

	/******************************
	****  R E F L E C T I O N  ****
	*******************************/

#pragma region Reflection
StartDebugTimer("B-SceneRefl");

SetDebugPrefix("Refl-");

		D3DXMATRIX matReflectView, matReflectViewProj, matReflectViewInv;
		D3DXMATRIX mViewMat = *(D3DXMATRIX*)&getActiveCamera()->getViewMatrix();

		D3DXMatrixInverse(&matReflectViewInv, NULL, &mViewMat);
		// Reflect Camera
		D3DXVECTOR3 vCamPos		= Mat4GetPosition(matReflectViewInv);
		D3DXVECTOR3 vCamForward = Mat4GetForward(matReflectViewInv);
		D3DXVECTOR3 vCamRight	= Mat4GetRight(matReflectViewInv);
		D3DXVECTOR3 vCamUp		= Mat4GetUp(matReflectViewInv);

		vCamPos.y		= -vCamPos.y + 2*WaterHeight;
		vCamForward.y	= -vCamForward.y;
		vCamRight.y		= -vCamRight.y;
		vCamUp.y		= -vCamUp.y;
		vCamUp = -vCamUp;

		Mat4SetPosition( matReflectViewInv, vCamPos );
		Mat4SetForward( matReflectViewInv, vCamForward );
		Mat4SetRight( matReflectViewInv, vCamRight );
		Mat4SetUp( matReflectViewInv, vCamUp );

		D3DXMatrixInverse(&matReflectView, NULL, &matReflectViewInv);


		matReflectViewProj = matReflectView * (*(D3DXMATRIX*)&getActiveCamera()->getProjectionMatrix());

		cc = video::SColorf(0.0f,1.0,0.0f,1.0f);
		for(int i = 1; i < services->EffectsList.size(); ++i)
		{
			services->EffectsList[i]->SetPassC(cc);
			services->EffectsList[i]->SetLightProjection(LightProjection);
			services->EffectsList[i]->SetView(*(matrix4*)&matReflectView);
			services->EffectsList[i]->SetViewInverse(*(matrix4*)&matReflectViewInv);
			services->EffectsList[i]->SetViewProjection(*(matrix4*)&matReflectViewProj);
			services->EffectsList[i]->SetCameraPosition(*(vector3df*)&vCamPos);

			if(ShadowMap)
				services->EffectsList[i]->SetShadowMap(ShadowMap);
		}

		services->UnSetBackBuffer2();
		services->SetReflectionMapTarget();

		services->setTransformN(irr::video::ETS_VIEW, *(matrix4*)&matReflectView);
		getActiveCamera()->setViewMatrix( *(matrix4*)&matReflectView );

		services->clearZBuffer();

		D3DXVECTOR4 Plane;
		D3DXMATRIX	matReflect = matReflectView * (*(D3DXMATRIX*)&Projection);
		float WaterPlaneBias = 0.05;
			D3DXMatrixInverse(&matReflect, NULL, &matReflect);

			D3DXMatrixTranspose( &matReflect, &matReflect );
			Plane = D3DXVECTOR4(0, 1, 0, -WaterHeight+WaterPlaneBias);
			D3DXVec4Transform(&Plane, &Plane, &matReflect);

			plane3df p; p.Normal.X = Plane.x; p.Normal.Y = Plane.y; p.Normal.Z = Plane.z; p.D = Plane.w;
		services->SetClipPlane( 1, p );

		DrawAll_SolidNodes( services );

		services->SetClipPlane( -1, p );

		services->setTransformN(irr::video::ETS_VIEW, View);
		getActiveCamera()->setViewMatrix( View );

	services->UnSetReflectionMapTarget();



SetDebugPrefix("");
StopDebugTimer();
#pragma endregion Reflection

StartDebugTimer("B-SceneSol");
	/*
	* #####  #####  #      #####  ####
	* #      #   #  #        #    #   #
	* #####  #   #  #        #    #   #
	*     #  #   #  #        #    #   #
	* #####  #####  #####  #####  #### 
	*/
		IDirect3DTexture9* ReflectionTex = services->GetReflectionTexture();

		for(int i = 1; i < services->EffectsList.size(); ++i)
		{
			services->EffectsList[i]->SetReflectionMap(ReflectionTex);
			services->EffectsList[i]->SetReflectionViewProj( *(matrix4*)&(matReflectViewProj) );
			services->EffectsList[i]->SetView(View);
			services->EffectsList[i]->SetViewInverse(ViewInverse);
			services->EffectsList[i]->SetViewProjection(ViewProj);
			services->EffectsList[i]->SetCameraPosition(CamPos);
		}

		services->SetBackBuffer2();
		services->clearZBuffer();

		// Render first pass
		DrawAll_SolidNodes( services );

		// Render second pass

  		services->SetBackBuffer2Target();
  		DrawAll_SolidNodes( services, 1 );
  		services->UnSetBackBuffer2Target();

		services->UpdateFrameBuffer();
StopDebugTimer();

#pragma region Transparent

StartDebugTimer("B-SceneTrans");
	/*
	* #####  ####    ##   ##  #   ####   ###    ###   ####   #####  ##  #  #####
	*   #    #   #  #  #  # # #  #      #   #  #   #  #   #  #      # # #    #  
	*   #    ####   ####  # # #   ###   ####   #####  ####   ###    # # #    #  
	*   #    #  #   #  #  # # #      #  #      #   #  #  #   #      # # #    #  
	*   #    #   #  #  #  #  ##  ####   #      #   #  #   #  #####  #  ##    #  
	*/

	CurrentRendertime = ESNRP_TRANSPARENT;

	irr::core::vector3df pos;
	irr::video::SColorf col;
	irr::f32 radius;
	video::SColorf cc2 = video::SColorf(1.0f,0.0,0.0f,1.0f);
	for(int i = 1; i < services->EffectsList.size(); ++i)
	{
		services->EffectsList[i]->SetPassC(cc2);
	}

// 	for(int RTIndex = 0; RTIndex < 2; ++RTIndex)
// 	{
// 		if(RTIndex == 1)
// 			services->SetBackBuffer2Target(false);
	int RTIndex = 0;

	for(i = 0; i < this->TransparentNodes.size(); ++i)
	{		
		CurrentShader = this->TransparentNodes[i].Node->getEffect();

		// Get Effect
		ID3DXEffect* Effect = services->getEffect(CurrentShader);

		if(CurrentShader != NULL && Effect != NULL)
		{
			
			ShaderConstantSet* CurrentSet = services->EffectsList[CurrentShader];
			if(CurrentSet == NULL)
				continue;
			if(CurrentSet->SetTechnique(RTIndex) == false)
				continue;

			// Set Effect
			services->setShader(CurrentShader);

			// Directional Lights
			int j = 0;
			for(u32 f = 0; f < this->DirectionalLightsList.size(); ++f)
			{
				if(this->DirectionalLightsList[f]->Active == true)
				{
					pos = this->DirectionalLightsList[f]->Normal;
					col = this->DirectionalLightsList[f]->Color;
			
					CurrentSet->SetDirectionalColor(col, j);
					CurrentSet->SetDirectionalNormal(pos, j);

					++j;
					if( j == 3 ) break;
				}
			}

			// Begin
			UINT NumPasses = 0;
			Effect->Begin(&NumPasses,0);

			NPointLight* PLights[5];
			int PCount = GetLights(PLights, this->TransparentNodes[i].Node->getAbsolutePosition());

			// Actually set the point lights
			for(u32 f = 0; f < PCount; ++f)
			{
				CurrentSet->SetPointPosition(PLights[f]->Position, f);
				CurrentSet->SetPointColor(PLights[f]->Color, f);
				CurrentSet->SetPointRadius(PLights[f]->Radius, f);
			}
			CurrentSet->CommitLights();

			// Set the fog if required
			if(TransparentNodes[i].Node->FogOn == true)
			{
				FogNearFar.set(0xFFFE, 0xFFFF);
				CurrentSet->SetFogColor(FogCol);
				CurrentSet->SetFogData(FogNearFar);
			}else
			{
				FogNearFar.set(services->FogStart,services->FogEnd);
				CurrentSet->SetFogColor(FogCol);
				CurrentSet->SetFogData(FogNearFar);
			}

			// Set the ambient if required
			if(TransparentNodes[i].Node->LightOn == true)
			{
				video::SColorf tcol1(1.0f,1.0f,1.0f,1.0f);
				CurrentSet->SetLightAmbient(tcol1);
			}else
			{
				CurrentSet->SetLightAmbient(col1);
			}

			CurrentSet->SetAlpha(TransparentNodes[i].Node->Alpha);

			core::matrix4 World = TransparentNodes[i].Node->getAbsoluteTransformation();
			CurrentSet->SetWorld(World);

			core::matrix4 TempWVP = ViewProj * World;
			CurrentSet->SetWorldViewProjection(TempWVP);

			CurrentSet->SetSceneNodeConstants(TransparentNodes[i].Node);

			// Animations
			if(TransparentNodes[i].Node->getType() == ESNT_ANIMATED_MESH)
			{
				CAnimatedMeshSceneNode* N = (CAnimatedMeshSceneNode*)TransparentNodes[i].Node;
				IAnimatedMesh* M = N->getLocalMesh();
				if(M->getMeshType() == scene::EAMT_B3D)
					if(((IAnimatedMeshB3d*)M)->IsAnimated())
						if(CurrentSet->HasBones())
						{
							// Get BB Bones
//							core::array<IAnimatedMeshB3d::SB3dNode*>* Nodes = ((IAnimatedMeshB3d*)M)->GetAnimationNodes(N->getFrameNr());
							
							// Check their size
							if(N->MatNodes.size() > ANIMATIONMATRIXCOUNT)
							{
								MessageBox(NULL,"Error: AnimatedMesh Bonecount exceeds the limit that BBDX can handle","Runtime Error",MB_ICONERROR | MB_OK);
								exit(0);
							}

							// Build Array
//							for(int j = 0; j < Nodes->size(); ++j)
//								AnimationMatrices[j] = ((*Nodes)[j]->GlobalAnimatedMatrix * (*Nodes)[j]->GlobalInversedMatrix).getTransposed();				
							CurrentSet->SetBones(N->MatNodes.pointer(), N->MatNodes.size());
						}
			}

			// Render
			for(UINT Pass = 0; Pass < NumPasses; ++Pass)
			{


				if(FAILED(Effect->BeginPass(Pass)))
							break;


				Effect->CommitChanges();
				services->GetDXDevice()->SetRenderState(D3DRS_ZWRITEENABLE, FALSE);
				
				// If blending isn't enabled... setup simple blending
				DWORD State;
				services->GetDXDevice()->GetRenderState(D3DRS_ALPHABLENDENABLE, &State);
				if(State == FALSE)
				{
					services->GetDXDevice()->SetRenderState(D3DRS_ALPHABLENDENABLE, TRUE);
					services->GetDXDevice()->SetRenderState(D3DRS_SRCBLEND, D3DBLEND_SRCALPHA);
					services->GetDXDevice()->SetRenderState(D3DRS_DESTBLEND, D3DBLEND_INVSRCALPHA);
				}

				
				//if(strcmp(TransparentNodes[i].Node->getTag(), "WATCHME") == 0)
				//	OutputDebugString("Transparent Render Pass\n");


				//services->GetDXDevice()->SetRenderState(D3DRS_ZENABLE, TRUE);
				TransparentNodes[i].Node->render();
				Effect->EndPass();
			}
			
			Effect->End();
		}
	}

// 	if(RTIndex == 1)
// 		services->UnSetBackBuffer2Target();
// 
// 	} // RTIndex

StopDebugTimer();
StartDebugTimer("B-SceneLas");
	video::SColorf cc3 = video::SColorf(0.0f, 1.0, 1.0f,1.0f);
	for(int i = 1; i < services->EffectsList.size(); ++i)
	{
		services->EffectsList[i]->SetPassC(cc3);
	}

	for(i=0; i<LastNodeList.size(); ++i)
	{
		if(LastNodeList[i]->isVisible())
		{
			CurrentShader = LastNodeList[i]->getEffect();

			if(CurrentShader)
			{
				// Get Effect
				ID3DXEffect* Effect = services->getEffect(CurrentShader);
				ShaderConstantSet* CurrentSet = services->EffectsList[CurrentShader];

				// Set Effect
				services->setShader(CurrentShader);

				// Do World
				core::matrix4 World = LastNodeList[i]->getAbsoluteTransformation();
				CurrentSet->SetWorld(World);

				core::matrix4 TempWVP = ViewProj * World;
				CurrentSet->SetWorldViewProjection(TempWVP);

				// Do Alpha
				CurrentSet->SetAlpha(LastNodeList[i]->Alpha);

				// Render!
				UINT NumPasses;
				Effect->Begin(&NumPasses, 0);
				for(UINT Pass = 0; Pass < NumPasses; ++Pass)
				{
					if(FAILED(Effect->BeginPass(Pass)))
							break;
					LastNodeList[i]->render();
					Effect->EndPass();
				}
				Effect->End();
			}
		}
	}

StopDebugTimer();
#pragma endregion Transparent

	for(i = 0; i < this->SolidNodes.size(); ++i)
		SolidNodes[i].Nodes.clear();

	for(i = 0; i < this->TransparentNodes.size(); ++i)
		TransparentNodes.clear();

	//BatchSolidNodes.clear();
	SolidNodes.clear();
	TransparentNodes.clear();
	SolidShadowNodes.clear();
	TransparentShadowNodes.clear();

	for(int i = 1; i < services->EffectsList.size(); ++i)
	{
		if(ShadowMap)
			services->EffectsList[i]->SetShadowMap(NULL);
	}



	// do animations and other stuff.
	OnPostRender(os::Timer::getTime());

	services->UpdateFrameBuffer();

	clearDeletionList();

	CurrentRendertime = ESNRP_COUNT;
}

void CSceneManager::DrawAll_SolidNodes( irr::video::IVideoDriver* pServices, int rtIndex  )
{
	irr::video::CD3D9Driver* services = (irr::video::CD3D9Driver*)pServices;

	CurrentRendertime = ESNRP_SOLID;

	core::vector2df FogNearFar(services->FogStart,services->FogEnd);
	video::SColorf FogCol( (services->FogColor.getRed() / 255.0f), (services->FogColor.getGreen() / 255.0f), (services->FogColor.getBlue() / 255.0f));



	// Reset this silly state
	services->GetDXDevice()->SetRenderState(D3DRS_ZWRITEENABLE, TRUE);
	services->GetDXDevice()->SetRenderState(D3DRS_ZENABLE, TRUE);

	//
	// BATCH NODE RENDERS
	//

	u32 CurrentShader = 0;
	u32 LastShader = 0;
	irr::core::vector3df lpos;
	irr::video::SColorf lcol;
	irr::f32 lradius;
	irr::video::SColorf col1 = services->AmbientLight;
	irr::core::matrix4 ViewProj;
	ViewProj = services->getTransform(irr::video::ETS_PROJECTION);			
	ViewProj *= services->getTransform(irr::video::ETS_VIEW);

	bool fogSet = false;
	bool ambientLightSet = false;

/*	#pragma region BatchSolidNodes
	for(int i = 0; i < this->BatchSolidNodes.size(); ++i)
	{
		CurrentShader = this->BatchSolidNodes[i].Effect;

		if(CurrentShader)
		{
			// Get Effect
			ID3DXEffect* Effect = services->getEffect(CurrentShader);
			ShaderConstantSet* CurrentSet = services->EffectsList[CurrentShader];
			if(CurrentSet == NULL)
				continue;
			if(CurrentSet->SetTechnique(rtIndex) == false)
				continue;

			// Set Effect
			services->setShader(CurrentShader);

			// Directional Lights
			int j = 0;
			for(u32 f = 0; f < this->DirectionalLightsList.size(); ++f)
			{
				if(this->DirectionalLightsList[f]->Active == true)
				{
					irr::core::vector3df pos = this->DirectionalLightsList[f]->Normal;
					irr::video::SColorf col = this->DirectionalLightsList[f]->Color;

					CurrentSet->SetDirectionalColor(col, j);
					CurrentSet->SetDirectionalNormal(pos, j);

					++j;
					if( j == 3 ) break;
				}
			}

			// Begin
			UINT NumPasses = 0;
			Effect->Begin(&NumPasses,0);
			if(NumPasses == 1)
				Effect->BeginPass(0);

			for(u32 u = 0; u < ((IAnimatedMeshSceneNode*)this->BatchSolidNodes[i].Nodes[0])->getLocalMesh()->getMesh(0)->getMeshBufferCount(); ++u)
			{

				for(u32 g = 0; g < this->BatchSolidNodes[i].Nodes.size(); ++g)
				{
					NPointLight* PLights[5];
					int PCount = GetLights(PLights, this->BatchSolidNodes[i].Nodes[g]->getAbsolutePosition());

					// Actually set the point lights
					for(u32 f = 0; f < PCount; ++f)
					{
						CurrentSet->SetPointPosition(PLights[f]->Position, f);
						CurrentSet->SetPointColor(PLights[f]->Color, f);
						CurrentSet->SetPointRadius(PLights[f]->Radius, f);
					}
					CurrentSet->CommitLights();

					// Set the fog if required
					if(BatchSolidNodes[i].Nodes[g]->FogOn == true)
					{
						FogNearFar.set(0xFFFE, 0xFFFF);
						CurrentSet->SetFogColor(FogCol);
						CurrentSet->SetFogData(FogNearFar);
					}else
					{
						FogNearFar.set(services->FogStart,services->FogEnd);
						CurrentSet->SetFogColor(FogCol);
						CurrentSet->SetFogData(FogNearFar);
					}

					// Set the ambient if required
					if(BatchSolidNodes[i].Nodes[g]->LightOn == true)
					{
						video::SColorf tcol1(1.0f,1.0f,1.0f,1.0f);
						CurrentSet->SetLightAmbient(tcol1);
					}else
					{
						CurrentSet->SetLightAmbient(col1);
					}

					CurrentSet->SetAlpha(BatchSolidNodes[i].Nodes[g]->Alpha);

					core::matrix4 World = BatchSolidNodes[i].Nodes[g]->getAbsoluteTransformation();
					CurrentSet->SetWorld(World);

					core::matrix4 TempWVP = ViewProj * World;
					CurrentSet->SetWorldViewProjection(TempWVP);

					CurrentSet->SetSceneNodeConstants(BatchSolidNodes[i].Nodes[g]);


					//if(strcmp(BatchSolidNodes[i].Nodes[g]->getTag(), "WATCHME") == 0)
					//{
					//	char OO[1024];
					//	sprintf(OO, "%x: Batch Solids\n", BatchSolidNodes[i].Nodes[g]);
					//	OutputDebugString(OO);
					//}

					if(NumPasses == 1)
					{
						Effect->CommitChanges();
						((CAnimatedMeshSceneNode*)BatchSolidNodes[i].Nodes[g])->renderBuffer(u);
					}else
					{



						// render
						for(UINT Pass = 0; Pass < NumPasses; ++Pass)
						{

							if(FAILED(Effect->BeginPass(Pass)))
								break;
							Effect->CommitChanges();
							((CAnimatedMeshSceneNode*)BatchSolidNodes[i].Nodes[g])->renderBuffer(u);
							Effect->EndPass();
						}
					}

				}
			}
			if(NumPasses == 1)
				Effect->EndPass();
			Effect->End();
		}
	}
	#pragma endregion*/



	#pragma region SolidNodes

	u32 solidNodesSize = SolidNodes.size();
	for(int i = 0; i < solidNodesSize; ++i)
	{
		CurrentShader = this->SolidNodes[i].Effect;

		if(CurrentShader)
		{
			// Get Effect
			ID3DXEffect* Effect = services->getEffect(CurrentShader);
			ShaderConstantSet* CurrentSet = services->EffectsList[CurrentShader];
			if(CurrentSet == NULL)
				continue;
			if(CurrentSet->SetTechnique(rtIndex) == false)
				continue;

			// Set Effect
			services->setShader(CurrentShader);

			// Directional Lights
			int j = 0;
			for(u32 f = 0; f < this->DirectionalLightsList.size(); ++f)
			{
				if(this->DirectionalLightsList[f]->Active == true)
				{
					irr::core::vector3df pos = this->DirectionalLightsList[f]->Normal;
					irr::video::SColorf col = this->DirectionalLightsList[f]->Color;

					CurrentSet->SetDirectionalColor(col, j);
					CurrentSet->SetDirectionalNormal(pos, j);

					++j;
					if( j == 3 ) break;
				}
			}

			// Begin
			UINT NumPasses = 0;
			Effect->Begin(&NumPasses,0);
			if(NumPasses == 1)
				Effect->BeginPass(0);


			u32 count = SolidNodes[i].Nodes.size();
			for(u32 g = 0; g < count; ++g)
			{
				ISceneNode* renderNode = SolidNodes[i].Nodes[g];

				// Set the fog if required
				if(renderNode->FogOn == true)
				{
					FogNearFar.set(0xFFFE, 0xFFFF);
					CurrentSet->SetFogColor(FogCol);
					CurrentSet->SetFogData(FogNearFar);
					fogSet = false;
				}else if(!fogSet)
				{
					FogNearFar.set(services->FogStart,services->FogEnd);
					CurrentSet->SetFogColor(FogCol);
					CurrentSet->SetFogData(FogNearFar);
					fogSet = true;
				}

				// Set the ambient if required
				if(renderNode->LightOn == true)
				{
					video::SColorf tcol1(1.0f,1.0f,1.0f,1.0f);
					CurrentSet->SetLightAmbient(tcol1);
					ambientLightSet = false;
				}else if(!ambientLightSet)
				{
					CurrentSet->SetLightAmbient(col1);
					ambientLightSet = true;
				}

				// JB: Don't set lights on LOD meshes (since they are too far away to even matter!)
				if(renderNode->GetLastLOD() == 0)
				{
					NPointLight* PLights[5];
					int PCount = GetLights(PLights, renderNode->getAbsolutePosition());

					// Actually set the point lights
					for(u32 f = 0; f < PCount; ++f)
					{
						CurrentSet->SetPointPosition(PLights[f]->Position, f);
						CurrentSet->SetPointColor(PLights[f]->Color, f);
						CurrentSet->SetPointRadius(PLights[f]->Radius, f);
					}
					CurrentSet->CommitLights();
				}

				CurrentSet->SetAlpha(renderNode->Alpha);

				core::matrix4 World = renderNode->getAbsoluteTransformation();
				CurrentSet->SetWorld(World);

				core::matrix4 TempWVP = ViewProj * World;
				CurrentSet->SetWorldViewProjection(TempWVP);

				CurrentSet->SetSceneNodeConstants(renderNode);

				// Animations
				if(renderNode->getType() == ESNT_ANIMATED_MESH)
				{
					CAnimatedMeshSceneNode* N = (CAnimatedMeshSceneNode*)renderNode;
					IAnimatedMesh* M = N->getLocalMesh();
					if(M->getMeshType() == scene::EAMT_B3D)
						if(((IAnimatedMeshB3d*)M)->IsAnimated())
							if(CurrentSet->HasBones())
							{
								//printf("P\n");
								//char VV[128];sprintf(VV,"%i\n", N->getFrameNr());OutputDebugString(VV);
								// Get BB Bones
								//								core::array<IAnimatedMeshB3d::SB3dNode*>* Nodes = ((IAnimatedMeshB3d*)M)->GetAnimationNodes(N->getFrameNr());
								//printf("O\n");

								// Check their size
								if(N->MatNodes.size() > ANIMATIONMATRIXCOUNT)
								{
									MessageBox(NULL,"Error: AnimatedMesh Bonecount exceeds the limit that BBDX can handle","Runtime Error",MB_ICONERROR | MB_OK);
									exit(0);
								}

								// Build Array
								//								for(int j = 0; j < Nodes->size(); ++j)
								//									AnimationMatrices[j] = ((*Nodes)[j]->GlobalAnimatedMatrix * (*Nodes)[j]->GlobalInversedMatrix).getTransposed();				
								//CurrentSet->SetBones(N->MatNodes.pointer(), N->MatNodes.size());
							}
				}

				if(NumPasses == 1)
				{
					Effect->CommitChanges();
					renderNode->render();
				}else
				{
					// render
					for(UINT Pass = 0; Pass < NumPasses; ++Pass)
					{
						if(FAILED(Effect->BeginPass(Pass)))
							break;

						Effect->CommitChanges();
						renderNode->render();
						Effect->EndPass();
					}
				}

			}
			if(NumPasses == 1)
				Effect->EndPass();
			Effect->End();
		}
	}

	// Render callback objects
	if(rtIndex == 0)
		CallRenderSolidCallback();
	CallRenderSolidCallbackRT(rtIndex);

	#pragma endregion


	//services->UpdateFrameBuffer();
}


void CSceneManager::nodeOrder(ISceneNode* node, s32 order)
{
	if(node->Order > 0)
	{
		for(u32 i = 0; i < FirstNodeList.size(); ++i)
		{
			if(FirstNodeList[i] == node)
			{
				FirstNodeList.erase(i);
			}
		}
	}else if(node->Order < 0)
	{
		for(u32 i = 0; i < LastNodeList.size(); ++i)
		{
			if(LastNodeList[i] == node)
			{
				LastNodeList.erase(i);
			}
		}
	}

	node->Order = order;
	//if(node->Order != 0)
	//{
	//	node->setMaterialFlag(irr::video::EMF_ZBUFFER, false);
	//	node->setMaterialFlag(irr::video::EMF_ZWRITE_ENABLE, false);
	//}

	if(order > 0)
	{
		if(FirstNodeList.size() == 0)
		{
			FirstNodeList.push_back(node);
			return;
		}

		for(u32 i = 0; i < FirstNodeList.size(); ++i)
		{
			if(order >= FirstNodeList[i]->Order)
			{
				FirstNodeList.insert(node,i);
				return;
			}
		}

		FirstNodeList.push_back(node);
	}else if(order < 0)
	{
		if(LastNodeList.size() == 0)
		{
			LastNodeList.push_back(node);
			return;
		}

		for(u32 i = 0; i < LastNodeList.size(); ++i)
		{
			if(order >= LastNodeList[i]->Order)
			{
				LastNodeList.insert(node,i);
				return;
			}
		}

		LastNodeList.push_back(node);
	}
}


//! Sets the color of stencil buffers shadows drawed by the scene manager.
void CSceneManager::setShadowColor(video::SColor color)
{
	ShadowColor = color;
}


//! Returns the current color of shadows.
video::SColor CSceneManager::getShadowColor() const
{
	return ShadowColor;
}





//! Adds an external mesh loader.
void CSceneManager::addExternalMeshLoader(IMeshLoader* externalLoader)
{
	if (!externalLoader)
		return;

	externalLoader->grab();
	MeshLoaderList.push_back(externalLoader);
}



//! Returns a pointer to the scene collision manager.
ISceneCollisionManager* CSceneManager::getSceneCollisionManager()
{
	return CollisionManager;
}


//! Returns a pointer to the mesh manipulator.
IMeshManipulator* CSceneManager::getMeshManipulator()
{
	return MeshManipulator;

}



//! Adds a scene node to the deletion queue.
void CSceneManager::addToDeletionQueue(ISceneNode* node)
{
	if (!node)
		return;

	node->grab();
	DeletionList.push_back(node);
}


//! clears the deletion list
void CSceneManager::clearDeletionList()
{
	if (DeletionList.empty())
		return;

	for (s32 i=0; i<(s32)DeletionList.size(); ++i)
	{
		DeletionList[i]->remove();
		DeletionList[i]->drop();
	}

	DeletionList.clear();
}


//! Returns the first scene node with the specified name.
ISceneNode* CSceneManager::getSceneNodeFromName(const char* name, ISceneNode* start)
{
	if (start == 0)
		start = getRootSceneNode();

	if (!strcmp(start->getName(),name))
		return start;

	ISceneNode* node = 0;

	const core::list<ISceneNode*>& list = start->getChildren();
	core::list<ISceneNode*>::Iterator it = list.begin();
	for (; it!=list.end(); ++it)
	{
		node = getSceneNodeFromName(name, *it);
		if (node)
			return node;
	}

	return 0;
}


//! Returns the first scene node with the specified id.
ISceneNode* CSceneManager::getSceneNodeFromId(s32 id, ISceneNode* start)
{
	if (start == 0)
		start = getRootSceneNode();

	if (start->getID() == id)
		return start;

	ISceneNode* node = 0;

	const core::list<ISceneNode*>& list = start->getChildren();
	core::list<ISceneNode*>::Iterator it = list.begin();
	for (; it!=list.end(); ++it)
	{
		node = getSceneNodeFromId(id, *it);
		if (node)
			return node;
	}

	return 0;
}


//! Posts an input event to the environment. Usually you do not have to
//! use this method, it is used by the internal engine.
bool CSceneManager::postEventFromUser(SEvent event)
{
	bool ret = false;
	ICameraSceneNode* cam = getActiveCamera();
	if (cam)
		ret = cam->OnEvent(event);

	_IRR_IMPLEMENT_MANAGED_MARSHALLING_BUGFIX;
	return ret;
}


//! Removes all children of this scene node
void CSceneManager::removeAll()
{
	ISceneNode::removeAll();
	setActiveCamera(0);
}


//! Clears the whole scene. All scene nodes are removed.
void CSceneManager::clear()
{
	removeAll();
}


//! Returns interface to the parameters set in this scene.
io::IAttributes* CSceneManager::getParameters()
{
	return &Parameters;
}


//! Returns current render pass.
E_SCENE_NODE_RENDER_PASS CSceneManager::getSceneNodeRenderPass()
{
	return CurrentRendertime;
}



//! Returns an interface to the mesh cache which is shared beween all existing scene managers.
IMeshCache* CSceneManager::getMeshCache()
{
	return MeshCache;
}


//! Creates a new scene manager.
ISceneManager* CSceneManager::createNewSceneManager()
{
	return new CSceneManager(Driver, FileSystem, MeshCache);
}


//! Returns the default scene node factory which can create all built in scene nodes
ISceneNodeFactory* CSceneManager::getDefaultSceneNodeFactory()
{
	return getSceneNodeFactory(0);
}


//! Adds a scene node factory to the scene manager.
void CSceneManager::registerSceneNodeFactory(ISceneNodeFactory* factoryToAdd)
{
	if (factoryToAdd)
	{
		factoryToAdd->grab();
		SceneNodeFactoryList.push_back(factoryToAdd);
	}
}


//! Returns amount of registered scene node factories.
s32 CSceneManager::getRegisteredSceneNodeFactoryCount()
{
	return SceneNodeFactoryList.size();
}


//! Returns a scene node factory by index
ISceneNodeFactory* CSceneManager::getSceneNodeFactory(s32 index)
{
	if (index>=0 && index<(int)SceneNodeFactoryList.size())
		return SceneNodeFactoryList[index];

	return 0;
}


//! Returns the default scene node animator factory which can create all built-in scene node animators
ISceneNodeAnimatorFactory* CSceneManager::getDefaultSceneNodeAnimatorFactory()
{
	return getSceneNodeAnimatorFactory(0);
}

//! Adds a scene node animator factory to the scene manager.
void CSceneManager::registerSceneNodeAnimatorFactory(ISceneNodeAnimatorFactory* factoryToAdd)
{
	if (factoryToAdd)
	{
		factoryToAdd->grab();
		SceneNodeAnimatorFactoryList.push_back(factoryToAdd);
	}
}


//! Returns amount of registered scene node animator factories.
s32 CSceneManager::getRegisteredSceneNodeAnimatorFactoryCount()
{
	return SceneNodeAnimatorFactoryList.size();
}


//! Returns a scene node animator factory by index
ISceneNodeAnimatorFactory* CSceneManager::getSceneNodeAnimatorFactory(s32 index)
{
	if (index>=0 && index<(int)SceneNodeAnimatorFactoryList.size())
		return SceneNodeAnimatorFactoryList[index];

	return 0;
}


//! Saves the current scene into a file.
//! \param filename: Name of the file .
bool CSceneManager::saveScene(const c8* filename, ISceneUserDataSerializer* userDataSerializer)
{
	io::IWriteFile* file = FileSystem->createAndWriteFile(filename);
	if (!file)
		return false;

	bool ret = saveScene(file, userDataSerializer);
	file->drop();
	return ret;
}


//! Saves the current scene into a file.
bool CSceneManager::saveScene(io::IWriteFile* file, ISceneUserDataSerializer* userDataSerializer)
{
	if (!file)
		return false;

	//io::IXMLWriter* writer = 0;//FileSystem->createXMLWriter(file);
	//if (!writer)
	//	return false;

	//writer->writeXMLHeader();
 //   writeSceneNode(writer, this, userDataSerializer);
	//writer->drop();

	return true;
}


//! Loads a scene. Note that the current scene is not cleared before.
//! \param filename: Name of the file .
bool CSceneManager::loadScene(const c8* filename, ISceneUserDataSerializer* userDataSerializer)
{
	io::IReadFile* read = FileSystem->createAndOpenFile(filename);
	if (!read)
	{
	    os::Printer::log("Unable to open scene file", filename, ELL_ERROR);
		return false;
	}

	bool ret = loadScene(read, userDataSerializer);
	read->drop();

	return ret;
}


//! Loads a scene. Note that the current scene is not cleared before.
bool CSceneManager::loadScene(io::IReadFile* file, ISceneUserDataSerializer* userDataSerializer)
{
	if (!file)
	{
	    os::Printer::log("Unable to open scene file", ELL_ERROR);
		return false;
	}

	//io::IXMLReader* reader = 0;//FileSystem->createXMLReader(file);
	//if (!reader)
	//{
 //       os::Printer::log("Scene is not a valid XML file", file->getFileName(), ELL_ERROR);
	//	return false;
	//}

	//while(reader->read())
	//{
	//	readSceneNode(reader, 0, userDataSerializer);
	//}

	//os::Printer::log("Finished loading scene", file->getFileName(), ELL_INFORMATION);

	//reader->drop();
	return true;

}




//! Returns a typename from a scene node type or null if not found
const c8* CSceneManager::getSceneNodeTypeName(ESCENE_NODE_TYPE type)
{
	const char* name = 0;

	for (int i=0; !name && i<(int)SceneNodeFactoryList.size(); ++i)
		name = SceneNodeFactoryList[i]->getCreateableSceneNodeTypeName(type);

	return name;
}

//! Returns a typename from a scene node animator type or null if not found
const c8* CSceneManager::getAnimatorTypeName(ESCENE_NODE_ANIMATOR_TYPE type)
{
	const char* name = 0;

	for (int i=0; !name && i<(int)SceneNodeAnimatorFactoryList.size(); ++i)
		name = SceneNodeAnimatorFactoryList[i]->getCreateableSceneNodeAnimatorTypeName(type);

	return name;
}


// creates a scenemanager
ISceneManager* createSceneManager(video::IVideoDriver* driver, io::IFileSystem* fs,
								  irr::IrrlichtDevice* device)
{
	return new CSceneManager(driver, fs, 0, device);
}



} // end namespace scene
} // end namespace irr

