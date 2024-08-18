// Copyright (C) 2002-2006 Nikolaus Gebhardt
// This file is part of the "Irrlicht Engine".
// For conditions of distribution and use, see copyright notice in irrlicht.h

#ifndef __I_SCENE_MANAGER_H_INCLUDED__
#define __I_SCENE_MANAGER_H_INCLUDED__

#define ANIMATIONMATRIXCOUNT 50

#include "irrArray.h"
#include "IUnknown.h"
#include "vector3d.h"
#include "dimension2d.h"
#include "SColor.h"
#include "SMaterial.h"
#include "IEventReceiver.h"
#include "SceneParameters.h"
#include "ESceneNodeTypes.h"
#include "..\..\bbdx\ASyncJobModule.h"

namespace irr
{
	struct SKeyMap;

namespace io
{
	class IFileSystem;
	class IReadFile;
	class IAttributes;
	class IWriteFile;
}

namespace gui
{
	class IGUIFont;
}

namespace video
{
	class IVideoDriver;
}



namespace scene
{
	//! Enumeration for render passes.
	/** A parameter passed to the registerNodeForRendering() method of the ISceneManager,
	specifying when the mode wants to be drawn in relation to the other nodes. */
	enum E_SCENE_NODE_RENDER_PASS
	{
		//! Scene nodes which are lights or camera should use this,
		//! the very first pass.
		ESNRP_LIGHT_AND_CAMERA,

		//! This is used for sky boxes.
		ESNRP_SKY_BOX,

		//! All normal objects can use this for registering themselves.
		//! This value will never be returned by ISceneManager::getSceneNodeRenderPass().
		//! The scene manager will determine by itself if an object is
		//! transparent or solid and register the object as SNRT_TRANSPARENT or
		//! SNRT_SOLD automaticly if you call registerNodeForRendering with this
		//! value (which is default). Note that it will register the node only as ONE type.
		//! If your scene node has both solid and transparent material types register
		//! it twice (one time as SNRT_SOLID, the other time as SNRT_TRANSPARENT) and
		//! in the render() method call getSceneNodeRenderPass() to find out the current
		//! render pass and render only the corresponding parts of the node.
		ESNRP_AUTOMATIC,

		//! Solid scene nodes or special scene nodes without materials.
		ESNRP_SOLID,

		//! Drawn after the transparent nodes, the time for drawing shadow volumes
		ESNRP_SHADOW,

		//! Transparent scene nodes, drawn after shadow nodes. They are sorted from back
		//! to front and drawn in that order.
		ESNRP_TRANSPARENT,

		//! Never used, value specifing how much parameters there are.
		ESNRP_COUNT
	};

	class IMesh;
	class IAnimatedMesh;
	class IMeshCache;
	class ISceneNode;
	class ICameraSceneNode;
	class IAnimatedMeshSceneNode;
	class ISceneNodeAnimator;
	class ISceneNodeAnimatorCollisionResponse;
	class ILightSceneNode;
	class IBillboardSceneNode;
	class ITerrainSceneNode;
	class IMeshSceneNode;
	class IMeshLoader;
	class ISceneCollisionManager;
	class IParticleSystemSceneNode;
	class IDummyTransformationSceneNode;
	class ITriangleSelector;
	class IMetaTriangleSelector;
	class IMeshManipulator;
	class ITextSceneNode;
	class ISceneNodeFactory;
	class ISceneNodeAnimatorFactory;
	class ISceneUserDataSerializer;

	//!	The Scene Manager manages scene nodes, mesh recources, cameras and all the other stuff.
	/** All Scene nodes can be created only here. There is a always growing list of scene
	nodes for lots of purposes: Indoor rendering scene nodes like the Octree
	(addOctTreeSceneNode()) or the terrain renderer (addTerrainSceneNode()),
	different Camera scene nodes (addCameraSceneNode(), addCameraSceneNodeMaya()),
	scene nodes for Light (addLightSceneNode()), Billboards (addBillboardSceneNode())
	and so on.
	A scene node is a node in the hierachical scene graph. Every scene node may have children,
	which are other scene nodes. Children move relative the their parents position. If the parent of a node is not
	visible, its children won't be visible, too. In this way, it is for example easily possible
	to attach a light to a moving car or to place a walking character on a moving platform
	on a moving ship.
	The SceneManager is also able to load 3d mesh files of different formats. Take a look
	at getMesh() to find out what formats are supported. And if these formats are not enough
	use addExternalMeshLoader() to add new formats to the engine.
	*/
	class ISceneManager : public virtual IUnknown
	{
	public:

		//! destructor
		virtual ~ISceneManager() {};
		ISceneManager() {DebugObject = 0; WaterHeight = 0; RenderMask = 0;}

		//! Returns pointer to an animateable mesh. Loads the file if not loaded already.
		/**
		 * If you want to remove a loaded mesh from the cache again, use removeMesh().
		 *  Currently there are the following mesh formats supported:
		 *  <TABLE border="1" cellpadding="2" cellspacing="0">
		 *  <TR>
		 *    <TD>Format</TD>
		 *    <TD>Description</TD>
		 *  </TR>
		 *  <TR>
		 *    <TD>3D Studio (.3ds)</TD>
		 *    <TD>Loader for 3D-Studio files which lots of 3D packages are able to export.
		 *      Only static meshes are currently supported by this importer. </TD>
		 *  </TR>
		 *  <TR>
		 *    <TD>Bliz Basic B3D (.b3d)</TD>
		 *    <TD>Loader for blitz basic files, developed by Mark Sibly, also supports animations.</TD>
		 *  </TR>
		 *  <TR>
		 *    <TD>Cartography shop 4 (.csm)</TD>
		 *    <TD>Cartography Shop is a modeling program for creating architecture and calculating
		 *      lighting. Irrlicht can directly import .csm files thanks to the IrrCSM library
		 *      created by Saurav Mohapatra which is now integrated directly in Irrlicht.
		 *      If you are using this loader, please note that you'll have to set the path
		 *      of the textures before loading .csm files. You can do this using SceneManager-&gt;getParameters()-&gt;setParameter(scene::CSM_TEXTURE_PATH,
		 *      &quot;path/to/your/textures&quot;);</TD>
		 *  </TR>
		 *  <TR>
		 *    <TD>COLLADA (.dae, .xml)</TD>
		 *    <TD>COLLADA is an open Digital Asset Exchange Schema for the interactive 3D industry. There are
		 *        exporters and importers for this format available for most of the big 3d packages
		 *        at http://collada.org. Irrlicht can import COLLADA files by using the
		 *        ISceneManager::getMesh() method. COLLADA files need not contain only one single mesh
		 *        but multiple meshes and a whole scene setup with lights, cameras and mesh instances,
		 *        this loader can set up a scene as described by the COLLADA file instead of loading
		 *        and returning one single mesh. By default, this loader behaves like the other loaders
		 *        and does not create instances, but it can be switched into this mode by using
		 *        SceneManager->getParameters()->setParameter(COLLADA_CREATE_SCENE_INSTANCES, true);
		 *        Created scene nodes will be named as the names of the nodes in the
		 *        COLLADA file. The returned mesh is just a dummy object in this mode. Meshes included in
		 *        the scene will be added into the scene manager with the following naming scheme:
		 *        path/to/file/file.dea#meshname. The loading of such meshes is logged.
		 *        Currently, this loader is able to create meshes (made of only polygons), lights,
		 *        and cameras. Materials and animations are currently not supported but this will
		 *        change with future releases.
		 *    </TD>
		 *  </TR>
		 *  <TR>
		 *    <TD>Delgine DeleD (.dmf)</TD>
		 *    <TD>DeleD (delgine.com) is a 3D editor and level-editor combined into one and is specifically
		 *        designed for 3D game-development. With this loader, it is possible to directly load
		 *        all geometry is as well as textures and lightmaps from .dmf files. To set texture and
		 *        material paths, see scene::DMF_USE_MATERIALS_DIRS and scene::DMF_TEXTURE_PATH. It is also
		 *        possible to flip the alpha texture by setting scene::DMF_FLIP_ALPHA_TEXTURES to true and
		 *        to set the material transparent reference value by setting scene::DMF_ALPHA_CHANNEL_REF to
		 *        a float between 0 and 1. The loader is
		 *        based on Salvatore Russo's .dmf loader, I just changed some parts of it. Thanks to
		 *        Salvatore for his work and for allowing me to use his code in Irrlicht and put it under Irrlicht's
		 *        license. For newer and more enchanced versions of the loader, take a look at delgine.com.
		 *    </TD>
		 *  </TR>
		 *  <TR>
		 *    <TD>DirectX (.x)</TD>
		 *    <TD>Platform independent importer (so not D3D-only) for .x files. Most 3D
		 *      packages can export these natively and there are several tools for them
		 *      available. (e.g. the Maya exporter included in the DX SDK) .x files can
		 *      include skeletal animations and Irrlicht is able to play and display them.
		 *      Currently, Irrlicht only supports uncompressed .x files.</TD>
		 *  </TR>
		 *  <TR>
		 *    <TD>Maya (.obj)</TD>
		 *    <TD>Most 3D software can create .obj files which contain static geometry without
		 *      material data. The material files .mtl are also supported. This importer
		 *      for Irrlicht can load them directly. </TD>
		 *  </TR>
		 *  <TR>
		 *    <TD>Milkshape (.ms3d)</TD>
		 *    <TD>.MS3D files contain models and sometimes skeletal animations from the
		 *      Milkshape 3D modeling and animation software. This importer for Irrlicht
		 *      can display and/or animate these files.  </TD>
		 *  </TR>
		 *  <TR>
		 *  <TD>My3D (.my3d)</TD>
		 *      <TD>.my3D is a flexible 3D file format. The My3DTools contains plug-ins to
		 *        export .my3D files from several 3D packages. With this built-in importer,
		 *        Irrlicht can read and display those files directly. This loader was written
		 *        by Zhuck Dimitry who also created the whole My3DTools package. If you are using this loader, please
		 *      note that you can set the path of the textures before loading .my3d files.
		 *      You can do this using SceneManager-&gt;getParameters()-&gt;setParameter(scene::MY3D_TEXTURE_PATH,
		 *      &quot;path/to/your/textures&quot;); </TD>
		 *    </TR>
		 *    <TR>
		 *      <TD>OCT (.oct)</TD>
		 *      <TD>The oct file format contains 3D geometry and lightmaps and can be loaded
		 *        directly by Irrlicht. OCT files<br>
		 *        can be created by FSRad, Paul Nette's radiosity processor or exported from
		 *        Blender using OCTTools which can be found in the exporters/OCTTools directory
		 *        of the SDK. Thanks to Murphy McCauley for creating all this.</TD>
		 *    </TR>
		 *	  <TR>
		 *      <TD>OGRE Meshes (.mesh)</TD>
		 *      <TD>Ogre .mesh files contain 3D data for the OGRE 3D engine. Irrlicht can read and
		 *          display them directly with this importer. To define materials for the mesh,
		 *			copy a .material file named like the corresponding .mesh file where the .mesh
		 *			file is. (For example ogrehead.material for ogrehead.mesh). Thanks to Christian Stehno
		 *			who wrote and contributed this loader.</TD>
		 *    </TR>
		 *    <TR>
		 *      <TD>Pulsar LMTools (.lmts)</TD>
		 *      <TD>LMTools is a set of tools (Windows &amp; Linux) for creating lightmaps.
		 *        Irrlicht can directly read .lmts files thanks to<br>
		 *        the importer created by Jonas Petersen. If you are using this loader, please
		 *        note that you can set the path of the textures before loading .lmts files.
		 *        You can do this using SceneManager-&gt;getParameters()-&gt;setParameter(scene::LMTS_TEXTURE_PATH,
		 *        &quot;path/to/your/textures&quot;); Notes for<br>
		 *        this version of the loader:<br>
		 *        - It does not recognice/support user data in the *.lmts files.<br>
		 *        - The TGAs generated by LMTools don't work in Irrlicht for some reason (the
		 *        textures are upside down). Opening and resaving them in a graphics app will
		 *        solve the problem.</TD>
		 *    </TR>
		 *    <TR>
		 *      <TD>Quake 3 levels (.bsp)</TD>
		 *      <TD>Quake 3 is a popular game by IDSoftware, and .pk3 files contain .bsp files
		 *        and textures/lightmaps describing huge<br>
		 *        prelighted levels. Irrlicht can read .pk3 and .bsp files directly and thus
		 *        render Quake 3 levels directly. Written by Nikolaus Gebhardt enhanced by
		 *        Dean P. Macri with the curved surfaces feature. </TD>
		 *    </TR>
		 *    <TR>
		 *      <TD>Quake 2 models (.md2)</TD>
		 *      <TD>Quake 2 models are characters with morph target animation. Irrlicht can
		 *        read, display and animate them directly with this importer. </TD>
		 *    </TR>
		 *  </TABLE>
		 *
		 *  To load and display a mesh quickly, just do this:
		 *  \code
		 *  SceneManager->addAnimatedMeshSceneNode(
		 *		SceneManager->getMesh("yourmesh.3ds"));
		 *  \endcode
		 *  If you would like to implement and add your own file format loader to Irrlicht,
		 *  see addExternalMeshLoader().
		 *  \param filename: Filename of the mesh to load.
		 *  \return Returns NULL if failed and the pointer to the mesh if
		 *  successful.
		 *  This pointer should not be dropped. See IUnknown::drop() for more information.
		 **/
		virtual IAnimatedMesh* getMesh(const c8* filename, bool Animated, bbdx2_ASyncJobFn completionCallback, void* userData) = 0;
		//virtual IAnimatedMesh* getMesh(io::IReadFile* file, bool animated, bool threaded) = 0;

		//! Returns an interface to the mesh cache which is shared beween all existing scene managers.
		/** With this interface, it is possible to manually add new loaded
		meshes (if ISceneManager::getMesh() is not sufficient), to remove them and to iterate
		through already loaded meshes. */
		virtual IMeshCache* getMeshCache() = 0;

		//! Returns the video driver.
		/** \return Returns pointer to the video Driver.
		 This pointer should not be dropped. See IUnknown::drop() for more information. */
		virtual video::IVideoDriver* getVideoDriver() = 0;

		//! Adds a scene node for rendering an animated mesh model.
		/** \param mesh: Pointer to the loaded animated mesh to be displayed.
		\param parent: Parent of the scene node. Can be NULL if no parent.
		\param id: Id of the node. This id can be used to identify the scene node.
		\param position: Position of the space relative to its parent where the
		scene node will be placed.
		\param rotation: Initital rotation of the scene node.
		\param scale: Initial scale of the scene node.
		\return Returns pointer to the created scene node.
		This pointer should not be dropped. See IUnknown::drop() for more information. */
		virtual IAnimatedMeshSceneNode* addAnimatedMeshSceneNode(IAnimatedMesh* mesh, ISceneNode* parent=0, s32 id=-1,
			const core::vector3df& position = core::vector3df(0,0,0),
			const core::vector3df& rotation = core::vector3df(0,0,0),
			const core::vector3df& scale = core::vector3df(1.0f, 1.0f, 1.0f),
			bool alsoAddIfMeshPointerZero=false) = 0;

		//! Adds a scene node for rendering a static mesh.
		/** \param mesh: Pointer to the loaded static mesh to be displayed.
		\param parent: Parent of the scene node. Can be NULL if no parent.
		\param id: Id of the node. This id can be used to identify the scene node.
		\param position: Position of the space relative to its parent where the
		scene node will be placed.
		\param rotation: Initital rotation of the scene node.
		\param scale: Initial scale of the scene node.
		\return Returns pointer to the created scene node.
		This pointer should not be dropped. See IUnknown::drop() for more information. */
		virtual IMeshSceneNode* addMeshSceneNode(IMesh* mesh, ISceneNode* parent=0, s32 id=-1,
			const core::vector3df& position = core::vector3df(0,0,0),
			const core::vector3df& rotation = core::vector3df(0,0,0),
			const core::vector3df& scale = core::vector3df(1.0f, 1.0f, 1.0f),
			bool alsoAddIfMeshPointerZero=false) = 0;

		 //! Adds a scene node for rendering using a octtree to the scene graph.
		 /** This a good method for rendering
		 scenes with lots of geometry. The Octree is built on the fly from the mesh.
		 \param mesh: The mesh containing all geometry from which the octtree will be build.
		 If this animated mesh has more than one frames in it, the first frame is taken.
		 \param parent: Parent node of the octtree node.
		 \param id: id of the node. This id can be used to identify the node.
		 \param minimalPolysPerNode: Specifies the minimal polygons contained a octree node.
		 If a node gets less polys the this value, it will not be splitted into
		 smaller nodes.
		 \return Returns the pointer to the octtree if successful, otherwise 0.
		 This pointer should not be dropped. See IUnknown::drop() for more information. */
		virtual ISceneNode* addOctTreeSceneNode(IAnimatedMesh* mesh, ISceneNode* parent=0,
			s32 id=-1, s32 minimalPolysPerNode=128, bool alsoAddIfMeshPointerZero=false) = 0;

		//! Adds a scene node for rendering using a octtree to the scene graph.
		/** This a good method for rendering
		 scenes with lots of geometry. The Octree is built on the fly from the mesh, much
		 faster then a bsp tree.
		 \param mesh: The mesh containing all geometry from which the octtree will be build.
		 \param parent: Parent node of the octtree node.
		 \param id: id of the node. This id can be used to identify the node.
		 \param minimalPolysPerNode: Specifies the minimal polygons contained a octree node.
		 If a node gets less polys the this value, it will not be splitted into
		 smaller nodes.
		 \return Returns the pointer to the octtree if successful, otherwise 0.
		 This pointer should not be dropped. See IUnknown::drop() for more information. */
		virtual ISceneNode* addOctTreeSceneNode(IMesh* mesh, ISceneNode* parent=0,
			s32 id=-1, s32 minimalPolysPerNode=128, bool alsoAddIfMeshPointerZero=false) = 0;

		//! Adds a camera scene node to the scene graph and sets it as active camera.
		/** This camera does not react on user input like for example the one created with
		addCameraSceneNodeFPS(). If you want to move or animate it, use animators or the
		ISceneNode::setPosition(), ICameraSceneNode::setTarget() etc methods.
		 \param position: Position of the space relative to its parent where the camera will be placed.
		 \param lookat: Position where the camera will look at. Also known as target.
		 \param parent: Parent scene node of the camera. Can be null. If the parent moves,
		 the camera will move too.
		 \param id: id of the camera. This id can be used to identify the camera.
		 \return Returns pointer to interface to camera if successful, otherwise 0.
		 This pointer should not be dropped. See IUnknown::drop() for more information. */
		virtual ICameraSceneNode* addCameraSceneNode(ISceneNode* parent = 0,
			const core::vector3df& position = core::vector3df(0,0,0),
			const core::vector3df& lookat = core::vector3df(0,0,100), s32 id=-1) = 0;

		//! Adds a maya style user controlled camera scene node to the scene graph.
		/** The maya camera is able to be controlled with the mouse similar
		 like in the 3D Software Maya by Alias Wavefront.
		 \param parent: Parent scene node of the camera. Can be null.
		 \param rotateSpeed: Rotation speed of the camera.
		 \param zoomSpeed: Zoom speed of the camera.
		 \param tranlationSpeed: TranslationSpeed of the camera.
		 \param id: id of the camera. This id can be used to identify the camera.
		 \return Returns a pointer to the interface of the camera if successful, otherwise 0.
		 This pointer should not be dropped. See IUnknown::drop() for more information. */
		virtual ICameraSceneNode* addCameraSceneNodeMaya(ISceneNode* parent = 0,
			f32 rotateSpeed = -1500.0f, f32 zoomSpeed = 200.0f, f32 translationSpeed = 1500.0f, s32 id=-1) = 0;

		//! Adds a camera scene node which is able to be controlled with the mouse and keys like in most first person shooters (FPS).
		/** Look with the mouse, move with cursor keys. If you do not like the default
		 key layout, you may want to specify your own. For example to make the camera
		 be controlled by the cursor keys AND the keys W,A,S, and D, do something
		 like this:
		 \code
		 SKeyMap keyMap[8];
		 keyMap[0].Action = EKA_MOVE_FORWARD;
		 keyMap[0].KeyCode = KEY_UP;
		 keyMap[1].Action = EKA_MOVE_FORWARD;
		 keyMap[1].KeyCode = KEY_KEY_W;

		 keyMap[2].Action = EKA_MOVE_BACKWARD;
		 keyMap[2].KeyCode = KEY_DOWN;
		 keyMap[3].Action = EKA_MOVE_BACKWARD;
		 keyMap[3].KeyCode = KEY_KEY_S;

		 keyMap[4].Action = EKA_STRAFE_LEFT;
		 keyMap[4].KeyCode = KEY_LEFT;
		 keyMap[5].Action = EKA_STRAFE_LEFT;
		 keyMap[5].KeyCode = KEY_KEY_A;

		 keyMap[6].Action = EKA_STRAFE_RIGHT;
		 keyMap[6].KeyCode = KEY_RIGHT;
		 keyMap[7].Action = EKA_STRAFE_RIGHT;
		 keyMap[7].KeyCode = KEY_KEY_D;

		 camera = sceneManager->addCameraSceneNodeFPS(0, 100, 500, -1, keyMap, 8);
		 \endcode
		 \param parent: Parent scene node of the camera. Can be null.
		 \param rotateSpeed: Speed with wich the camera is rotated. This can be done
		 only with the mouse.
		 \param movespeed: Speed with which the camera is moved. Movement is done with
		 the cursor keys.
		 \param id: id of the camera. This id can be used to identify the camera.
		 \param keyMapArray: Optional pointer to an array of a keymap, specifying what
		 keys should be used to move the camera. If this is null, the default keymap
		 is used. You can define actions more then one time in the array, to bind
		 multiple keys to the same action.
		 \param keyMapSize: Amount of items in the keymap array.
		 \param noVerticalMovement: Setting this to true makes the camera only move within a
		 horizontal plane, and disables vertical movement as known from most ego shooters. Default
		 is 'false', with which it is possible to fly around in space, if no gravity is there.
		 \return Returns a pointer to the interface of the camera if successful, otherwise 0.
		 This pointer should not be dropped. See IUnknown::drop() for more information. */
		virtual ICameraSceneNode* addCameraSceneNodeFPS(ISceneNode* parent = 0,
			f32 rotateSpeed = 100.0f, f32 moveSpeed = 500.0f, s32 id=-1,
			SKeyMap* keyMapArray=0, s32 keyMapSize=0, bool noVerticalMovement=false) = 0;

		//! Adds an empty scene node to the scene graph.
		/** Can be used for doing advanced transformations
		 or structuring the scene graph.
		 \return Returns pointer to the created scene node.
		 This pointer should not be dropped. See IUnknown::drop() for more information. */
		virtual ISceneNode* addEmptySceneNode(ISceneNode* parent=0, s32 id=-1) = 0;

		//! Adds a dummy transformation scene node to the scene graph.
		/** This scene node does not render itself, and does not respond to set/getPosition,
		 set/getRotation and set/getScale. Its just a simple scene node that takes a
		 matrix as relative transformation, making it possible to insert any transformation
		 anywhere into the scene graph.
		 \return Returns pointer to the created scene node.
		 This pointer should not be dropped. See IUnknown::drop() for more information. */
		virtual IDummyTransformationSceneNode* addDummyTransformationSceneNode(
			ISceneNode* parent=0, s32 id=-1) = 0;

		//! Returns the root scene node.
		/** This is the scene node wich is parent
		 of all scene nodes. The root scene node is a special scene node which
		 only exists to manage all scene nodes. It will not be rendered and cannot
		 be removed from the scene.
		 \return Returns a pointer to the root scene node. */
		virtual ISceneNode* getRootSceneNode() = 0;

		//! Returns the first scene node with the specified id.
		/** \param id: The id to search for
		 \param start: Scene node to start from. All children of this scene
		 node are searched. If null is specified, the root scene node is
		 taken.
		 \return Returns pointer to the first scene node with this id,
		 and null if no scene node could be found. */
		virtual ISceneNode* getSceneNodeFromId(s32 id, ISceneNode* start=0) = 0;

		//! Returns the first scene node with the specified name.
		/**\param start: Scene node to start from. All children of this scene
		 node are searched. If null is specified, the root scene node is
		 taken.
		 \return Returns pointer to the first scene node with this id,
		 and null if no scene node could be found. */
		virtual ISceneNode* getSceneNodeFromName(const char* name, ISceneNode* start=0) = 0;

		//! Returns the current active camera.
		/** \return The active camera is returned. Note that this can be NULL, if there
		 was no camera created yet. */
		virtual ICameraSceneNode* getActiveCamera() = 0;

		//! Sets the currently active camera.
		/** The previous active camera will be deactivated.
		 \param camera: The new camera which should be active. */
		virtual void setActiveCamera(ICameraSceneNode* camera) = 0;

		//! Sets the color of stencil buffers shadows drawn by the scene manager.
		virtual void setShadowColor(video::SColor color = video::SColor(150,0,0,0)) = 0;

		//! Returns the current color of shadows.
		virtual video::SColor getShadowColor() const = 0;

		//! Registers a node for rendering it at a specific time.
		/** This method should only be used by SceneNodes when they get a
		 ISceneNode::OnPreRender() call.
		 \param node: Node to register for drawing. Usually scene nodes would set 'this'
		 as parameter here because they want to be drawn.
		 \param pass: Specifies when the mode wants to be drawn in relation to the other nodes.
		 For example, if the node is a shadow, it usually wants to be drawn after all other nodes
		 and will use ESNRP_SHADOW for this. See E_SCENE_NODE_RENDER_PASS for details. */
		virtual void registerNodeForRendering(ISceneNode* node,
			E_SCENE_NODE_RENDER_PASS pass = ESNRP_AUTOMATIC) = 0;
		//virtual void registerNodeForRenderingOrdered(ISceneNode* node,
		//	E_SCENE_NODE_RENDER_PASS pass = ESNRP_AUTOMATIC) = 0;

		virtual void nodeOrder(ISceneNode* node, s32 Order) = 0;


		//! Draws all the scene nodes.
		/** This can only be invoked between
		 IVideoDriver::beginScene() and IVideoDriver::endScene(). Please note that
		 the scene is not only drawn when calling this, but also animated
		 by existing scene node animators, culling of scene nodes is done, etc. */
		virtual void drawAll() = 0;


		//! Adds an external mesh loader for extending the engine with new file formats.
		/** If you want the engine to be extended with
		 file formats it currently is not able to load (e.g. .cob), just implement
		 the IMeshLoader interface in your loading class and add it with this method.
		 Using this method it is also possible to override built-in mesh loaders with
		 newer or updated versions without the need of recompiling the engine.
		 \param externalLoader: Implementation of a new mesh loader. */
		virtual void addExternalMeshLoader(IMeshLoader* externalLoader) = 0;

		//! Returns a pointer to the scene collision manager.
		virtual ISceneCollisionManager* getSceneCollisionManager() = 0;

		//! Returns a pointer to the mesh manipulator.
		virtual IMeshManipulator* getMeshManipulator() = 0;

		//! Adds a scene node to the deletion queue.
		/** The scene node is immediatly
		 deleted when it's secure. Which means when the scene node does not
		 execute animators and things like that. This method is for example
		 used for deleting scene nodes by their scene node animators. In
		 most other cases, a ISceneNode::remove() call is enough, using this
		 deletion queue is not necessary.
		 See ISceneManager::createDeleteAnimator() for details.
		 \param node: Node to detete. */
		virtual void addToDeletionQueue(ISceneNode* node) = 0;

		//! Posts an input event to the environment.
		/** Usually you do not have to
		 use this method, it is used by the internal engine. */
		virtual bool postEventFromUser(SEvent event) = 0;

		//! Clears the whole scene.
		/** All scene nodes are removed. */
		virtual void clear() = 0;

		//! Returns interface to the parameters set in this scene.
		/** String parameters can be used by plugins and mesh loaders.
		 For example the CMS and LMTS loader want a parameter named 'CSM_TexturePath'
		 and 'LMTS_TexturePath' set to the path were attached textures can be found. See
		 CSM_TEXTURE_PATH, LMTS_TEXTURE_PATH, MY3D_TEXTURE_PATH,
		 COLLADA_CREATE_SCENE_INSTANCES, DMF_TEXTURE_PATH and DMF_USE_MATERIALS_DIRS*/
		virtual io::IAttributes* getParameters() = 0;

		//! Returns current render pass.
		/** All scene nodes are being rendered in a specific order.
		 First lights, cameras, sky boxes, solid geometry, and then transparent
		 stuff. During the rendering process, scene nodes may want to know what the scene
		 manager is rendering currently, because for example they registered for rendering
		 twice, once for transparent geometry and once for solid. When knowing what rendering
		 pass currently is active they can render the correct part of their geometry. */
		virtual E_SCENE_NODE_RENDER_PASS getSceneNodeRenderPass() = 0;

		//! Returns the default scene node factory which can create all built in scene nodes
		virtual ISceneNodeFactory* getDefaultSceneNodeFactory() = 0;

		//! Adds a scene node factory to the scene manager.
		/** Use this to extend the scene manager with new scene node types which it should be
		able to create automaticly, for example when loading data from xml files. */
		virtual void registerSceneNodeFactory(ISceneNodeFactory* factoryToAdd) = 0;

		//! Returns amount of registered scene node factories.
		virtual s32 getRegisteredSceneNodeFactoryCount() = 0;

		//! Returns a scene node factory by index
		virtual ISceneNodeFactory* getSceneNodeFactory(s32 index) = 0;

		//! Returns the default scene node animator factory which can create all built-in scene node animators
		virtual ISceneNodeAnimatorFactory* getDefaultSceneNodeAnimatorFactory() = 0;

		//! Adds a scene node animator factory to the scene manager.
		/** Use this to extend the scene manager with new scene node animator types which it should be
		able to create automaticly, for example when loading data from xml files. */
		virtual void registerSceneNodeAnimatorFactory(ISceneNodeAnimatorFactory* factoryToAdd) = 0;

		//! Returns amount of registered scene node animator factories.
		virtual s32 getRegisteredSceneNodeAnimatorFactoryCount() = 0;

		//! Returns a scene node animator factory by index
		virtual ISceneNodeAnimatorFactory* getSceneNodeAnimatorFactory(s32 index) = 0;

		//! Returns a typename from a scene node type or null if not found
		virtual const c8* getSceneNodeTypeName(ESCENE_NODE_TYPE type) = 0;

		//! Creates a new scene manager.
		/** This can be used to easily draw and/or store two independent scenes at the same time.
		The mesh cache will be shared between all existing scene managers, which means if you load
		a mesh in the original scene manager using for example getMesh(), the mesh will be available
		in all other scene managers too, without loading.
		The original/main scene manager will still be there and accessible via IrrlichtDevice::getSceneManager().
		If you need input event in this new scene manager, for example for FPS cameras, you'll need
		to forward input to this manually: Just implement an IEventReceiver and call
		yourNewSceneManager->postEventFromUser(), and return true so that the original scene manager
		doesn't get the event. Otherwise, all input will go automaticly to the main scene manager.
		If you no longer need the new scene manager, you should call ISceneManager::drop().
		See IUnknown::drop() for more information. */
		virtual ISceneManager* createNewSceneManager() = 0;

		//! Saves the current scene into a file.
		/** Scene nodes with the option isDebugObject set to true are not being saved.
		The scene is usually written to an .irr file, an xml based format. .irr files can
		Be edited with the Irrlicht Engine Editor, irrEdit (http://irredit.irrlicht3d.org).
		To load .irr files again, see ISceneManager::loadScene().
		\param filename: Name of the file.
		\param userDataSerializer: If you want to save some user data for every scene node into the
		file, implement the ISceneUserDataSerializer interface and provide it as parameter here.
		Otherwise, simply specify 0 as this parameter.
		\return Returns true if successful. */
		virtual bool saveScene(const c8* filename, ISceneUserDataSerializer* userDataSerializer=0) = 0;

		//! Saves the current scene into a file.
		/** Scene nodes with the option isDebugObject set to true are not being saved.
		The scene is usually written to an .irr file, an xml based format. .irr files can
		Be edited with the Irrlicht Engine Editor, irrEdit (http://irredit.irrlicht3d.org).
		To load .irr files again, see ISceneManager::loadScene().
		\param file: File where the scene is saved into.
		\param userDataSerializer: If you want to save some user data for every scene node into the
		file, implement the ISceneUserDataSerializer interface and provide it as parameter here.
		Otherwise, simply specify 0 as this parameter.
		\return Returns true if successful. */
		virtual bool saveScene(io::IWriteFile* file, ISceneUserDataSerializer* userDataSerializer=0) = 0;

		//! Loads a scene. Note that the current scene is not cleared before.
		/** The scene is usually load from an .irr file, an xml based format. .irr files can
		Be edited with the Irrlicht Engine Editor, irrEdit (http://irredit.irrlicht3d.org) or
		saved directly by the engine using ISceneManager::saveScene().		
		\param filename: Name of the file.
		\param userDataSerializer: If you want to load user data possibily saved in that file for
		some scene nodes in the file, implement the ISceneUserDataSerializer interface and provide it as parameter here.
		Otherwise, simply specify 0 as this parameter.
		\return Returns true if successful. */
		virtual bool loadScene(const c8* filename, ISceneUserDataSerializer* userDataSerializer=0) = 0;

		//! Loads a scene. Note that the current scene is not cleared before.
		/** The scene is usually load from an .irr file, an xml based format. .irr files can
		Be edited with the Irrlicht Engine Editor, irrEdit (http://irredit.irrlicht3d.org) or
		saved directly by the engine using ISceneManager::saveScene().		
		\param file: File where the scene is going to be saved into.
		\param userDataSerializer: If you want to load user data possibily saved in that file for
		some scene nodes in the file, implement the ISceneUserDataSerializer interface and provide it as parameter here.
		Otherwise, simply specify 0 as this parameter.
		\return Returns true if successful.	*/
		virtual bool loadScene(io::IReadFile* file, ISceneUserDataSerializer* userDataSerializer=0) = 0;	

		virtual irr::s32 addPointLight() = 0;
		virtual irr::s32 addDirectionalLight() = 0;

		virtual void setLightPosition(irr::s32 light, irr::core::vector3df position) = 0;
		virtual void setLightDirection(irr::s32 light, irr::core::vector3df direction) = 0;

		virtual void setLightRadius(irr::s32 light, irr::s32 radius) = 0;
		virtual void setPLightColor(irr::s32 light, irr::video::SColorf color) = 0;
		virtual void setDLightColor(irr::s32 light, irr::video::SColorf color) = 0;
		virtual void setPLightActive(irr::s32 light, bool active ) = 0;
		virtual void setDLightActive(irr::s32 light, bool active ) = 0;

		virtual void FreePLight(irr::s32 light) = 0;
		virtual void FreeDLight(irr::s32 light) = 0;

		virtual bool isCulled(ISceneNode* node) = 0;

		virtual void setShadowDistance(f32 Distance) = 0;
		virtual f32 getShadowDistance() = 0;
		virtual void setShadowLevel(f32 Level) = 0;
		virtual void setShadowShader(u32 Shader) = 0;
		virtual void setBlurShadowShader(int index, u32 Shader) = 0;
		virtual void setLightDistance(f32 Distance) = 0;

		virtual void WriteScene(const char* Filename) = 0;
		virtual void Destroy() = 0;

		virtual int GetQualityLevel() = 0;
		virtual void SetQualityLevel(int Level) = 0;

		virtual void** GetPointLights(int* Count, unsigned int* Stride) = 0;
		virtual void GetDirectionalLights(float** Directions, float** Colors) = 0;

		virtual const float* getLightMatrix() const = 0;

		//virtual video::IVideoDriver* getVideoDriver() = 0;

		core::array<ISceneNode*> FirstNodeList;
		core::array<ISceneNode*> LastNodeList;

		int DebugObject;
		float WaterHeight;
		int RenderMask;

	};


} // end namespace scene
} // end namespace irr

#endif

