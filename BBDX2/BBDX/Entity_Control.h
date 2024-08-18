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
//* BBDX - Entity Control
//*

// Declaration
DLLPRE void DLLEX missedit(IAnimatedMeshSceneNode* Node, float Alpha);

// Order Entity
DLLPRE void DLLEX chincrank(ISceneNode* Node, int Order)
{
	smgr->nodeOrder(Node, Order);

	if(Node->getType() == irr::scene::ESNT_ANIMATED_MESH)
		missedit((IAnimatedMeshSceneNode*)Node, Node->Alpha);
}

// Hide node, but keep collisions
DLLPRE void DLLEX bumofcow2(ISceneNode* Node)
{
	Node->setVisible(false);
}

// Hide node from view
DLLPRE void DLLEX bumofcow(ISceneNode* Node)
{
    Node->setVisible(false);
	//DeActivateObjPRO(Node->CollisionID);
}


// Show node
DLLPRE void DLLEX fluxcapa(ISceneNode* Node)
{
    Node->setVisible(true);
	//ActivateObjPRO(Node->CollisionID);
}


// Set node parent
DLLPRE void DLLEX crystalclear(ISceneNode* Node, ISceneNode* Parent, int Global)
{
	SEHSTART;

	// If no parent is set, its just clearing it
	if(Parent == 0)
	{
		if(Node->getType() != ESNT_SAQUAD && Node->getType() != ESNT_SATEXT)
		{
			// Preserve previous transform
			irr::core::vector3df Position = Node->getAbsolutePosition();
			irr::core::vector3df Scale = ::getAbsoluteScale(smgr, Node);
			irr::core::vector3df Rotation = ::getAbsoluteRotation(smgr, Node);
			
			Node->setParent((CSceneManager*)smgr);

			chaosdigs(Node, Position.X, Position.Y, Position.Z, 0);
			mingja(Node, Rotation.X, Rotation.Y, Rotation.Z, 0);
			jaffamak(Node, Scale.X, Scale.Y, Scale.Z, 0);
				
			return;
		}else
		{
			Node->setParent(0);
			return;
		}
	}

	// If its a SAQuad it needs SAQuad handling
	if(Node->getType() == ESNT_SAQUAD)
	{
		if(Global == 0)
		{
			Node->setParent(Parent);
			SetPosition(Node, 0.0f, 0.0f);
			SetScale(Node, 1.0f, 1.0f);
		}else
		{
			dimension2df APos = GetPosition(Node);
			dimension2df ASca = GetScale(Node);
			dimension2df PPos = GetPosition(Parent);
			dimension2df PSca = GetScale(Parent);
			dimension2df NPos;
			dimension2df NSca;

			//NPos.Width = APos.Width - PPos.Width;
			//NPos.Height = APos.Height - PPos.Height;
			NPos.Width  = (APos.Width - PPos.Width)   / PSca.Width;
			NPos.Height = (APos.Height - PPos.Height) / PSca.Height;
			NSca.Width = ASca.Width / PSca.Width;
			NSca.Height = ASca.Height / PSca.Height;

			Node->setParent(Parent);
			SetPosition(Node, NPos.Width, NPos.Height);
			SetScale(Node, NSca.Width, NSca.Height);
		}
	}else if(Node->getType() == ESNT_SATEXT)
	{
		if(Global == 0)
		{
			Node->setParent(Parent);
			SetPosition(Node, 0.0f, 0.0f);
		}else
		{
			dimension2df APos = GetPosition(Node);
			dimension2df PPos = GetPosition(Parent);
			dimension2df PSca = GetScale(Parent);
			dimension2df NPos;

			NPos.Width  = (APos.Width - PPos.Width)   / PSca.Width;
			NPos.Height = (APos.Height - PPos.Height) / PSca.Height;

			Node->setParent(Parent);
			SetPosition(Node, NPos.Width, NPos.Height);
		}
	}

	if(Global == 0)
	{
		Node->setParent(Parent);
		
		// position to 0,0,0 and rotate to 0,0,0
// 		Node->setPosition(vector3df(0,0,0));
// 		Node->setRotation(vector3df(0,0,0));
// 		Node->setScale(vector3df(1,1,1));

		chaosdigs(Node, 0, 0, 0, 0);
		mingja(Node, 0, 0, 0, 0);
		jaffamak(Node, 1, 1, 1, 0);
		
	}else
	{
		// Preserve transform
		irr::core::vector3df Position = Node->getAbsolutePosition();
		irr::core::vector3df Scale = getAbsoluteScale(smgr, Node);
		irr::core::vector3df Rotation = getAbsoluteRotation(smgr, Node);
		
		Node->setParent(Parent);

		chaosdigs(Node, Position.X, Position.Y, Position.Z, 1);
		mingja(Node, Rotation.X, Rotation.Y, Rotation.Z, 1);
		jaffamak(Node, Scale.X, Scale.Y, Scale.Z, 1);
	}

	SEHEND
}


// Get a nodes parent
DLLPRE ISceneNode* DLLEX earthiswarm(ISceneNode* Node)
{
    return Node->getParent();
}


// Set the name of a node
DLLPRE void DLLEX commands(ISceneNode* Node, const char* NewName)
{
    Node->setName(NewName);
}


// Set the node tag
DLLPRE void DLLEX cones(ISceneNode* Node, const char* NewTag)
{
	Node->setTag(NewTag);
}

// Get the node tag
DLLPRE const char* DLLEX ocones(ISceneNode* Node)
{
	return Node->getTag();
}

// Declaration
void freechildentity(ISceneNode* Node);

void Realgetmeshake(ISceneNode* Node);

DLLPRE void DLLEX getmeshake(ISceneNode* Node)
{
	SEHSTART;

	Realgetmeshake(Node);

	SEHEND;
}

// Free the nodes instance
void Realgetmeshake(ISceneNode* Node)
{
	if(Node == 0)
		return;

	// Verify threading mode
	if(Node->LockedInThread)
	{
		Node->LockedDelete = true;
		return;
	}


	// Prepare to free children (heh)
	const irr::core::list<ISceneNode*>& Children = Node->getChildren();
	irr::core::list<ISceneNode*>::Iterator it = Children.begin();
	irr::core::array<ISceneNode*> Kids;

	for (; it != Children.end(); ++it)
		if(*it)
			Kids.push_back(*it);

	// Actually free the children
	for(irr::u32 i = 0; i < Kids.size(); ++i)
		getmeshake(Kids[i]);

	// I do this for some reason
	if(Node->getType() == irr::scene::ESNT_DUMMY_TRANSFORMATION)
		return;

	// Free collision instance
	//FreeObjPRO(Node->CollisionID);
	bbdx2_FreeCollisionInstance(Node);

	// Remove node from collision and picking lists as well as ordered rendering lists
	//for(irr::u32 i = 0; i < CollisionEntities.size(); ++i)
	//	if(CollisionEntities[i] == Node)
	//		CollisionEntities.erase(i);

	//for(irr::u32 i = 0; i < SpherePickingNode.size(); ++i)
	//	if(SpherePickingNode[i] == Node)
	//		SpherePickingNode.erase(i);

	//for(irr::u32 i = 0; i < PolyPickingNode.size(); ++i)
	//	if(PolyPickingNode[i] == Node)
	//		PolyPickingNode.erase(i);

	for(irr::u32 i = 0; i < smgr->FirstNodeList.size(); ++i)
		if(smgr->FirstNodeList[i] == Node)
			smgr->FirstNodeList.erase(i);

	for(irr::u32 i = 0; i < smgr->LastNodeList.size(); ++i)
		if(smgr->LastNodeList[i] == Node)
			smgr->LastNodeList.erase(i);

// 	// Free the mesh from cache
// 	IAnimatedMesh* Mesh = 0;
// 	if(Node->getType() == ESNT_ANIMATED_MESH)
// 	{
// 		Mesh = ((IAnimatedMeshSceneNode*)Node)->getLocalMesh();
// 
// 		if(Mesh->ReferenceCounter <= 2)
// 			smgr->getMeshCache()->removeMesh(Mesh);
// 	}

	// Remove children and the node itself
	Node->removeAll();
	Node->remove();
}

// Change mesh colour
DLLPRE void DLLEX kickleaves(IAnimatedMeshSceneNode* Node, int r, int g, int b)
{
	// This function is pretty innefficient. We should be doing this with shaders
	//OutputDebugString("EntityColor() Was called, but should not have been!");

	// Loop through every buffer
	for(int i = 0; i < Node->getLocalMesh()->getMesh(0)->getMeshBufferCount(); ++i)
	{
		// Obtain vertices
		void* vbd = Node->getLocalMesh()->getMesh(0)->getMeshBuffer(i)->getVertices();
		S3DVertex* vb = (S3DVertex*)vbd;
		irr::video::S3DVertex2TCoords* vb2 = (irr::video::S3DVertex2TCoords*)vbd;
		irr::video::S3DVertexTangents* vbt = (irr::video::S3DVertexTangents*)vbd;

		// Color vertices
		switch(Node->getLocalMesh()->getMesh(0)->getMeshBuffer(i)->getVertexType())
		{
		case irr::video::EVT_STANDARD:
			for(int f = 0; f < Node->getLocalMesh()->getMesh(0)->getMeshBuffer(i)->getVertexCount(); ++f)
			{
				vb[f].Color = SColor(255,r,g,b);
			}
			break;
		case irr::video::EVT_2TCOORDS:
			for(int f = 0; f < Node->getLocalMesh()->getMesh(0)->getMeshBuffer(i)->getVertexCount(); ++f)
			{
				vb2[f].Color = SColor(255,r,g,b);
			}
			break;
		case irr::video::EVT_TANGENTS:
			for(int f = 0; f < Node->getLocalMesh()->getMesh(0)->getMeshBuffer(i)->getVertexCount(); ++f)
			{
				vbt[f].Color = SColor(255,r,g,b);
			}
			break;

		}
	}

	// Update hardware buffers
	jockgnome(Node);
}

// Alpha the node
DLLPRE void DLLEX missedit(IAnimatedMeshSceneNode* Node, float Alpha)
{
	// If its SAText, then it has a special call
	if(Node->getType() == irr::scene::ESNT_SATEXT)
		shogun(Node, Alpha);

	// Setting it to opaque
	if(Alpha > 0.997f && Node->Order == 0)
	{
		Node->Alpha = 1.0f;
		bool isAlpha = false;

		if(Node->ForceAlpha == true )
		{
			Node->setMaterialType(irr::video::EMT_SOLID_ALPHA);
			return;
		}

		// Check for texture alpha
		for(int i = 0; i < Node->getMaterialCount(); ++i)
		{
			if(Node->getMaterial(i).Texture1)
				isAlpha = isAlpha || Node->getMaterial(i).Texture1->isAlpha;

			if(Node->getMaterial(i).Texture2)
				isAlpha = isAlpha || Node->getMaterial(i).Texture2->isAlpha;

			if(Node->getMaterial(i).Texture3)
				isAlpha = isAlpha || Node->getMaterial(i).Texture3->isAlpha;

			if(Node->getMaterial(i).Texture4)
				isAlpha = isAlpha || Node->getMaterial(i).Texture4->isAlpha;
		}

		// If the node still contain alpha information, change its material to reflect that
		if(isAlpha)
			Node->setMaterialType(irr::video::EMT_TRANSPARENT_ALPHA_CHANNEL);
		else	
			Node->setMaterialType(irr::video::EMT_SOLID);

		if(Node->getType() == scene::ESNT_ANIMATED_MESH)
		{
			IMesh* mesh = ((IAnimatedMeshSceneNode*)Node)->getLocalMesh()->getMesh(((IAnimatedMeshSceneNode*)Node)->getFrameNr());
			for(s32 i = 0; i < mesh->getMeshBufferCount(); ++i)
			{
				if(Node->RenderTimes.size() > i)
				{
					if(mesh->getMeshBuffer(i)->TextureAlpha || mesh->getMeshBuffer(i)->VertexAlpha || Node->ParticleAlpha)
						Node->RenderTimes[i] = 5;
					else if(Node->Alpha > 0.95f)
						Node->RenderTimes[i] = 3;
					else
						Node->RenderTimes[i] = 5;
				}
			}
		}

	}else
	{
		// Transparent
		Node->Alpha = Alpha;
		Node->setMaterialType(irr::video::EMT_TRANSPARENT_ALPHA_CHANNEL);
	}
}

// Quick alpha change
DLLPRE void DLLEX makewow(IAnimatedMeshSceneNode* Node, float Alpha)
{
	Node->Alpha = Alpha;
}

// Apply FX on an entity
DLLPRE void DLLEX localiva(IAnimatedMeshSceneNode* Node, int FX)
{
	Node->FogOn = !((FX & 8) == 0);
	Node->LightOn = ((FX & 1) > 0);
	Node->ForceAlpha = ((FX & 32) > 0);
	
	if(Node->ForceAlpha)
		Node->setMaterialType(irr::video::EMT_TRANSPARENT_ALPHA_CHANNEL);

	Node->ParticleAlpha = ((FX & 64) > 0);
}

// Is an entity viewable?
DLLPRE int DLLEX EntityInView(ISceneNode* Node)
{
	if(smgr->isCulled(Node))
		return 0;
	else
		return 1;
}

// Copy an entity
DLLPRE IAnimatedMeshSceneNode* DLLEX catonbox(IAnimatedMeshSceneNode* Node, ISceneNode* Parent)
{
	IAnimatedMeshSceneNode* New = smgr->addAnimatedMeshSceneNode(Node->getLocalMesh(), Parent);
	New->CollisionID = GetCollisionID();

	New->setPosition(Node->getPosition());
	New->setRotation(Node->getRotation());
	New->setScale(Node->getScale());

	New->setVisible(true);

	for(irr::s32 i = 0; i < Node->getMaterialCount(); ++i)
	{
		New->getMaterial(i).AmbientColor = Node->getMaterial(i).AmbientColor;
		New->getMaterial(i).AnisotropicFilter = Node->getMaterial(i).AnisotropicFilter;
		New->getMaterial(i).BackfaceCulling = Node->getMaterial(i).BackfaceCulling;
		New->getMaterial(i).BilinearFilter = Node->getMaterial(i).BilinearFilter;
		New->getMaterial(i).DiffuseColor = Node->getMaterial(i).DiffuseColor;
		New->getMaterial(i).EmissiveColor = Node->getMaterial(i).EmissiveColor;
		New->getMaterial(i).FogEnable = Node->getMaterial(i).FogEnable;
		New->getMaterial(i).GouraudShading = Node->getMaterial(i).GouraudShading;
		New->getMaterial(i).Lighting = Node->getMaterial(i).Lighting;
		New->getMaterial(i).MaterialType = Node->getMaterial(i).MaterialType;
		New->getMaterial(i).MaterialTypeParam = Node->getMaterial(i).MaterialTypeParam;
		New->getMaterial(i).MaterialTypeParam2 = Node->getMaterial(i).MaterialTypeParam2;
		New->getMaterial(i).NormalizeNormals = Node->getMaterial(i).NormalizeNormals;
		New->getMaterial(i).PointCloud = Node->getMaterial(i).PointCloud;
		New->getMaterial(i).Shininess = Node->getMaterial(i).Shininess;
		New->getMaterial(i).SpecularColor = Node->getMaterial(i).SpecularColor;
		New->getMaterial(i).Texture1 = Node->getMaterial(i).Texture1;
		New->getMaterial(i).Texture2 = Node->getMaterial(i).Texture2;
		New->getMaterial(i).Texture3 = Node->getMaterial(i).Texture3;
		New->getMaterial(i).Texture4 = Node->getMaterial(i).Texture4;
		New->getMaterial(i).TrilinearFilter = Node->getMaterial(i).TrilinearFilter;
		New->getMaterial(i).Wireframe = Node->getMaterial(i).Wireframe;
		New->getMaterial(i).ZBuffer = Node->getMaterial(i).ZBuffer;
		New->getMaterial(i).ZWriteEnable = Node->getMaterial(i).ZWriteEnable;

		if(New->getMaterial(i).Texture1)
			New->getMaterial(i).Texture1->grab();

		if(New->getMaterial(i).Texture2)
			New->getMaterial(i).Texture2->grab();

		if(New->getMaterial(i).Texture3)
			New->getMaterial(i).Texture3->grab();

		if(New->getMaterial(i).Texture4)
			New->getMaterial(i).Texture4->grab();

		

		for(irr::s32 f = 0; f < 12; ++f)
			New->getMaterial(i).Flags[f] = Node->getMaterial(i).Flags[f];
	}

	New->FogOn = Node->FogOn;
	New->LightOn = Node->LightOn;
	New->ForceAlpha = Node->ForceAlpha;
	
	if(New->ForceAlpha)
		New->setMaterialType(irr::video::EMT_TRANSPARENT_ALPHA_CHANNEL);

	New->ParticleAlpha = Node->ParticleAlpha;

	New->setEffect(Node->getEffect());
	//New->EntityType = 250;
	//CollisionTypePRO(New->CollisionID, 250);
	//CollisionEntities.push_back(New);

	New->pMeshLOD_HIGH = Node->pMeshLOD_HIGH;
	New->DistLOD_HIGH = Node->DistLOD_HIGH;

	New->pMeshLOD_MEDIUM = Node->pMeshLOD_MEDIUM;
	New->DistLOD_MEDIUM = Node->DistLOD_MEDIUM;

	New->pMeshLOD_LOW = Node->pMeshLOD_LOW;
	New->DistLOD_LOW = Node->DistLOD_LOW;

	return New;

}