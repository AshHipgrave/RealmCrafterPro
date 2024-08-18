// Copyright (C) 2002-2006 Nikolaus Gebhardt
// This file is part of the "Irrlicht Engine".
// For conditions of distribution and use, see copyright notice in irrlicht.h

#ifndef __C_SCENE_MANAGER_H_INCLUDED__
#define __C_SCENE_MANAGER_H_INCLUDED__


#include "irrlicht.h"
#include "ISceneManager.h"
#include "ISceneNode.h"
#include "irrString.h"
#include "irrArray.h"
#include "IMeshLoader.h"
#include "CAttributes.h"
#include "S3DVertex.h"
//#include "vector3d.h"

namespace irr
{
//namespace video
//{/
//	struct S3DVertex2TCoords;
//}
namespace io
{
	class IXMLWriter;
}
namespace scene
{
	struct VertexBlends
	{
		VertexBlends()
		{
			BlendWeight[0] = 0.0f;
			BlendWeight[1] = 0.0f;
			BlendWeight[2] = 0.0f;
			BlendWeight[3] = 0.0f;
			BlendIndex[0] = 0;
			BlendIndex[1] = 0;
			BlendIndex[2] = 0;
			BlendIndex[3] = 0;
			IsTangentMesh = false;
		}
		float BlendWeight[4];
		int BlendIndex[4];
		video::S3DVertexTangents* Vert;
		core::vector3df Tangent;
		bool IsTangentMesh;
		core::matrix4 Trans[4];
	};

	class CMeshCache;
	
	struct NPointLight;
	struct NDirectionalLight;

	struct SolidNodeGroup
	{
		core::array<ISceneNode*> Nodes;
		u32 Effect;
	};

	struct TransparentNode
	{
		ISceneNode* Node;
		f32 Distance;
	};

	struct TransparentNodeGroup
	{
		core::array<TransparentNode> Nodes;
		u32 Effect;
	};

	/*!
		The Scene Manager manages scene nodes, mesh recources, cameras and all the other stuff.
	*/
	class CSceneManager : public ISceneManager, public ISceneNode
	{
	public:

		//! constructor
		CSceneManager(video::IVideoDriver* driver, io::IFileSystem* fs,
			CMeshCache* cache = 0, irr::IrrlichtDevice* device = 0);

		//! destructor
		virtual ~CSceneManager();

		virtual void Destroy();

		virtual void nodeOrder(ISceneNode* node, s32 Order);

		//! gets an animateable mesh. loads it if needed. returned pointer must not be dropped.
		virtual IAnimatedMesh* getMesh(const c8* filename, bool Animated, bbdx2_ASyncJobFn completionCallback, void* userData);

		//! Get an empty mesh
		//virtual IAnimatedMesh* getMesh(io::IReadFile* file, bool animated, bool threaded);

		//! Returns an interface to the mesh cache which is shared beween all existing scene managers.
		virtual IMeshCache* getMeshCache();

		//! returns the video driver
		virtual video::IVideoDriver* getVideoDriver();

		//! adds a scene node for rendering an animated mesh model
		virtual IAnimatedMeshSceneNode* addAnimatedMeshSceneNode(IAnimatedMesh* mesh, ISceneNode* parent=0, s32 id=-1,
			const core::vector3df& position = core::vector3df(0,0,0),
			const core::vector3df& rotation = core::vector3df(0,0,0),
			const core::vector3df& scale = core::vector3df(1.0f, 1.0f, 1.0f),
			bool alsoAddIfMeshPointerZero=false);

		//! adds a scene node for rendering a static mesh
		//! the returned pointer must not be dropped.
		virtual IMeshSceneNode* addMeshSceneNode(IMesh* mesh, ISceneNode* parent=0, s32 id=-1,
			const core::vector3df& position = core::vector3df(0,0,0), 
			const core::vector3df& rotation = core::vector3df(0,0,0),
			const core::vector3df& scale = core::vector3df(1.0f, 1.0f, 1.0f),
			bool alsoAddIfMeshPointerZero=false);

		//! renders the node.
		virtual void render();

		//! returns the axis aligned bounding box of this node
		virtual const core::aabbox3d<f32>& getBoundingBox() const;

		//! registers a node for rendering it at a specific time.
		virtual void registerNodeForRendering(ISceneNode* node, E_SCENE_NODE_RENDER_PASS = ESNRP_AUTOMATIC);

		//! draws all scene nodes
		virtual void drawAll();

		//! Adds a scene node for rendering using a octtree to the scene graph. This a good method for rendering 
		//! scenes with lots of geometry. The Octree is built on the fly from the mesh, much
		//! faster then a bsp tree.
		virtual ISceneNode* addOctTreeSceneNode(IAnimatedMesh* mesh, ISceneNode* parent=0, 
			s32 id=-1, s32 minimalPolysPerNode=128, bool alsoAddIfMeshPointerZero=false);

		//! Adss a scene node for rendering using a octtree. This a good method for rendering 
		//! scenes with lots of geometry. The Octree is built on the fly from the mesh, much
		//! faster then a bsp tree.
		virtual ISceneNode* addOctTreeSceneNode(IMesh* mesh, ISceneNode* parent=0, 
			s32 id=-1, s32 minimalPolysPerNode=128, bool alsoAddIfMeshPointerZero=false);

		//! Adds a camera scene node to the tree and sets it as active camera.
		//! \param position: Position of the space relative to its parent where the camera will be placed.
		//! \param lookat: Position where the camera will look at. Also known as target.
		//! \param parent: Parent scene node of the camera. Can be null. If the parent moves,
		//! the camera will move too.
		//! \return Returns pointer to interface to camera
		virtual ICameraSceneNode* addCameraSceneNode(ISceneNode* parent = 0,
			const core::vector3df& position = core::vector3df(0,0,0), 
			const core::vector3df& lookat = core::vector3df(0,0,0), s32 id=-1);

		//! Adds a camera scene node which is able to be controlle with the mouse similar
		//! like in the 3D Software Maya by Alias Wavefront.
		//! The returned pointer must not be dropped.
		virtual ICameraSceneNode* addCameraSceneNodeMaya(ISceneNode* parent = 0,
			f32 rotateSpeed = -1500.0f, f32 zoomSpeed = 200.0f, f32 translationSpeed = 100.0f, s32 id=-1);

		//! Adds a camera scene node which is able to be controled with the mouse and keys
		//! like in most first person shooters (FPS): 
		virtual ICameraSceneNode* addCameraSceneNodeFPS(ISceneNode* parent = 0,
			f32 rotateSpeed = 1500.0f, f32 moveSpeed = 200.0f, s32 id=-1,
			SKeyMap* keyMapArray=0, s32 keyMapSize=0, bool noVerticalMovement=false);


		//! Adds a dummy transformation scene node to the scene graph.
		virtual IDummyTransformationSceneNode* addDummyTransformationSceneNode(
			ISceneNode* parent=0, s32 id=-1);

		//! Adds an empty scene node.
		virtual ISceneNode* addEmptySceneNode(ISceneNode* parent, s32 id=-1);

		//! Returns the root scene node. This is the scene node wich is parent 
		//! of all scene nodes. The root scene node is a special scene node which
		//! only exists to manage all scene nodes. It is not rendered and cannot
		//! be removed from the scene.
		//! \return Returns a pointer to the root scene node.
		virtual ISceneNode* getRootSceneNode();

		//! Returns the current active camera.
		//! \return The active camera is returned. Note that this can be NULL, if there
		//! was no camera created yet.
		virtual ICameraSceneNode* getActiveCamera();

		//! Sets the active camera. The previous active camera will be deactivated.
		//! \param camera: The new camera which should be active.
		virtual void setActiveCamera(ICameraSceneNode* camera);


		//! Adds an external mesh loader.
		virtual void addExternalMeshLoader(IMeshLoader* externalLoader);

		//! Returns a pointer to the scene collision manager.
		virtual ISceneCollisionManager* getSceneCollisionManager();

		//! Returns a pointer to the mesh manipulator.
		virtual IMeshManipulator* getMeshManipulator();
		
		//! Sets the color of stencil buffers shadows drawed by the scene manager.
		virtual void setShadowColor(video::SColor color);

		//! Returns the current color of shadows.
		virtual video::SColor getShadowColor() const;

		//! Adds a scene node to the deletion queue.
		virtual void addToDeletionQueue(ISceneNode* node);

		//! Returns the first scene node with the specified id.
		virtual ISceneNode* getSceneNodeFromId(s32 id, ISceneNode* start=0);

		//! Returns the first scene node with the specified name.
		virtual ISceneNode* getSceneNodeFromName(const char* name, ISceneNode* start=0);

		//! Posts an input event to the environment. Usually you do not have to
		//! use this method, it is used by the internal engine.
		virtual bool postEventFromUser(SEvent event);

		//! Clears the whole scene. All scene nodes are removed. 
		virtual void clear();

		//! Removes all children of this scene node
		virtual void removeAll();

		//! Returns interface to the parameters set in this scene.
		virtual io::IAttributes* getParameters();

		//! Returns current render pass. 
		virtual E_SCENE_NODE_RENDER_PASS getSceneNodeRenderPass();

		//! Creates a new scene manager. 
		virtual ISceneManager* createNewSceneManager();

		//! Returns type of the scene node
		virtual ESCENE_NODE_TYPE getType() { return ESNT_UNKNOWN; }

		//! Returns the default scene node factory which can create all built in scene nodes
		virtual ISceneNodeFactory* getDefaultSceneNodeFactory();

		//! Adds a scene node factory to the scene manager.
		/** Use this to extend the scene manager with new scene node types which it should be 
		able to create automaticly, for example when loading data from xml files. */
		virtual void registerSceneNodeFactory(ISceneNodeFactory* factoryToAdd);

		//! Returns amount of registered scene node factories.
		virtual s32 getRegisteredSceneNodeFactoryCount();

		//! Returns a scene node factory by index
		virtual ISceneNodeFactory* getSceneNodeFactory(s32 index);

		//! Returns a typename from a scene node type or null if not found
		virtual const c8* getSceneNodeTypeName(ESCENE_NODE_TYPE type);

		//! Returns the default scene node animator factory which can create all built-in scene node animators
		virtual ISceneNodeAnimatorFactory* getDefaultSceneNodeAnimatorFactory();

		//! Adds a scene node animator factory to the scene manager.
		virtual void registerSceneNodeAnimatorFactory(ISceneNodeAnimatorFactory* factoryToAdd);

		//! Returns amount of registered scene node animator factories.
		virtual s32 getRegisteredSceneNodeAnimatorFactoryCount();

		//! Returns a scene node animator factory by index
		virtual ISceneNodeAnimatorFactory* getSceneNodeAnimatorFactory(s32 index);

		//! Saves the current scene into a file.
		//! \param filename: Name of the file .
		virtual bool saveScene(const c8* filename, ISceneUserDataSerializer* userDataSerializer=0);

		//! Saves the current scene into a file.
		virtual bool saveScene(io::IWriteFile* file, ISceneUserDataSerializer* userDataSerializer=0);

		//! Loads a scene. Note that the current scene is not cleared before.
		//! \param filename: Name of the file .
		virtual bool loadScene(const c8* filename, ISceneUserDataSerializer* userDataSerializer=0);

		//! Loads a scene. Note that the current scene is not cleared before.
		virtual bool loadScene(io::IReadFile* file, ISceneUserDataSerializer* userDataSerializer=0);

		virtual irr::f32 CalcDistance(irr::core::vector3df P1, irr::core::vector3df P2);

		virtual irr::s32 addPointLight();
		virtual irr::s32 addDirectionalLight();

		virtual void setLightPosition(irr::s32 light, irr::core::vector3df position);
		virtual void setLightDirection(irr::s32 light, irr::core::vector3df direction);

		virtual void setLightRadius(irr::s32 light, irr::s32 radius);
		virtual void setPLightColor(irr::s32 light, irr::video::SColorf color);
		virtual void setDLightColor(irr::s32 light, irr::video::SColorf color);
		virtual void setPLightActive(irr::s32 light, bool active );
		virtual void setDLightActive(irr::s32 light, bool active );

		virtual void FreePLight(irr::s32 light);
		virtual void FreeDLight(irr::s32 light);

		virtual bool isTriangleCulled(IAnimatedMeshSceneNode* node, core::aabbox3d<f32> box);


		virtual void updateDirectionalLights();

		//! returns if node is culled
		virtual bool isCulled(ISceneNode* node);

		virtual void setShadowDistance(f32 Distance)
		{
			ShadowDistance = Distance;
		}

		virtual f32 getShadowDistance()
		{
			return ShadowDistance;
		}

		virtual void setShadowLevel(f32 Level)
		{
			ShadowLevel = Level;
		}

		virtual void setShadowShader(u32 Shader)
		{
			ShadowShader = Shader;
		}

		virtual void setBlurShadowShader(int index, u32 Shader)
		{
			ShadowBlurShader[index] = Shader;
		}

		virtual void setLightDistance(f32 Distance)
		{
			LightDistance = Distance;
		}

		int CSceneManager::GetLights(NPointLight** Lights, irr::core::vector3df& Position);

		virtual void WriteScene(const char* Filename)
		{
			irr::io::IWriteFile* File = irr::io::createWriteFile(Filename, false);
			this->WriteSceneGraph(File, "");
		}

		virtual void WriteSceneGraph(irr::io::IWriteFile* File, core::stringc Tabbage)
		{
			char o[128];
			sprintf(o, "%sCSceneManager(%x, %s)\r\n", Tabbage.c_str(), this, Name.c_str());
			core::stringc Write = o;

			File->write(o, Write.size());
			WriteDesc(File, Tabbage);
		}

		virtual int GetQualityLevel() { return QualityLevel; }
		virtual void SetQualityLevel(int Level) { QualityLevel = Level; }

		virtual void** GetPointLights(int* Count, unsigned int* Stride);
		virtual void GetDirectionalLights(float** Directions, float** Colors);

		virtual const float* getLightMatrix() const { return reinterpret_cast<const float*>(&LightMatrix); }
		virtual const float* getLightView() const { return reinterpret_cast<const float*>(&TLightView); }
		virtual const float* getLightProjection() const { return reinterpret_cast<const float*>(&TLightProj); }


	private:		


		bool BLAH;
		bool BLAH2;

		//! Returns a typename from a scene node animator type or null if not found
		virtual const c8* getAnimatorTypeName(ESCENE_NODE_ANIMATOR_TYPE type);

		void DrawAll_SolidNodes( irr::video::IVideoDriver* services, int rtIndex = 0 );

		f32 ShadowDistance;
		f32 LightDistance;
		RealmCrafter::RCT::Frustum LightFrustum;
		irr::core::matrix4 LightMatrix;
		irr::core::matrix4 TLightView, TLightProj;
		//u32 ShadowShader;
		u32 ShadowBlurShader[2];
		u32 QualityLevel;

		bool RenderMasks[32];
		int RenderPow[32];
		
		//! clears the deletion list
		void clearDeletionList();

		//! writes a scene node
//		void writeSceneNode(io::IXMLWriter* writer, ISceneNode* node, ISceneUserDataSerializer* userDataSerializer);

		//! reads a scene node
//		void readSceneNode(io::IXMLReader* reader, ISceneNode* parent, ISceneUserDataSerializer* userDataSerializer);

		//! read materials
//		void readMaterials(io::IXMLReader* reader, ISceneNode* node);

		//! reads animators of a node
//		void readAnimators(io::IXMLReader* reader, ISceneNode* node);

		//! reads user data of a node
//		void readUserData(io::IXMLReader* reader, ISceneNode* node, ISceneUserDataSerializer* userDataSerializer);
		
		struct DefaultNodeEntry
		{
			DefaultNodeEntry() {};

			DefaultNodeEntry(ISceneNode* n)
			{
				textureValue = 0;

				if (n->getMaterialCount())
					textureValue = (n->getMaterial(0).Texture1);

				node = n;
			}

			ISceneNode* node;
			void* textureValue;

			bool operator < (const DefaultNodeEntry& other) const
			{
				return (textureValue < other.textureValue);
			}
		};


		struct TransparentNodeEntry
		{
			TransparentNodeEntry() {};

			TransparentNodeEntry(ISceneNode* n, core::vector3df camera)
			{
				node = n;

				// TODO: this could be optimized, by not using sqrt
				distance = (f32)(node->getAbsoluteTransformation().getTranslation().getDistanceFrom(camera));
			}

			ISceneNode* node;
			f32 distance;

			bool operator < (const TransparentNodeEntry& other) const
			{
				return (distance > other.distance);
			}
		};

		//! video driver
		video::IVideoDriver* Driver;

		//! file system
		io::IFileSystem* FileSystem;

		//! collision manager
		ISceneCollisionManager* CollisionManager;

		//! mesh manipulator
		IMeshManipulator* MeshManipulator;

		//! render pass lists
		core::array<ISceneNode*> LightAndCameraList;
		core::array<ISceneNode*> ShadowNodeList;
		core::array<ISceneNode*> SkyBoxList;
		core::array<DefaultNodeEntry> SolidNodeList;
		core::array<TransparentNodeEntry> TransparentNodeList;
		

		core::array<SolidNodeGroup> SolidNodes;
		//core::array<SolidNodeGroup> BatchSolidNodes;
		core::array<TransparentNode> TransparentNodes;
		core::array<SolidNodeGroup> SolidShadowNodes;
		core::array<TransparentNode> TransparentShadowNodes;

		core::array<IMeshLoader*> MeshLoaderList;
		core::array<ISceneNode*> DeletionList;
		core::array<ISceneNodeFactory*> SceneNodeFactoryList;
		core::array<ISceneNodeAnimatorFactory*> SceneNodeAnimatorFactoryList;

		core::array<NPointLight*> PointLightsList;
		core::array<NDirectionalLight*> DirectionalLightsList;
		core::array<NPointLight*> ActiveLightsList;

		//! current active camera
		ICameraSceneNode* ActiveCamera;
		core::vector3df camTransPos; // Position of camera for transparent nodes.

		video::SColor ShadowColor;

		//! String parameters
		io::CAttributes Parameters;

		//! Mesh cache
		CMeshCache* MeshCache;
		irr::IrrlichtDevice* Device;

		E_SCENE_NODE_RENDER_PASS CurrentRendertime;	
	};

} // end namespace video
} // end namespace scene

#endif

