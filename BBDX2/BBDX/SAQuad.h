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
//* BBDX - SAQuad - Screen Aligned Quads!
//*

// Include for compatibility
#include <CD3D9Driver.h>

// Default SaQuad Verts, used for quick setup
irr::video::S3DVertex SAQuad[4];

// Define the default SAQuad
void CreateAndDefineSAQuad()
{
	SAQuad[0] = irr::video::S3DVertex(-1, -1, 0, 0, 0, -1, irr::video::SColor(255,255,255,255), 0, 1);
	SAQuad[1] = irr::video::S3DVertex(1, -1, 0, 0, 0, -1, irr::video::SColor(255,255,255,255), 1, 1);
	SAQuad[2] = irr::video::S3DVertex(-1,  1, 0, 0, 0, -1, irr::video::SColor(255,255,255,255), 0, 0);
	SAQuad[3] = irr::video::S3DVertex(1,  1, 0, 0, 0, -1, irr::video::SColor(255,255,255,255), 1, 0);
}

// Screen Aligned Quad object
class SAQuadSceneNode : public irr::scene::ISceneNode
{
	irr::core::aabbox3d<irr::f32> SABox;		// Compatibility
	irr::video::SMaterial Material;				// Used for texture
	irr::core::dimension2d<irr::f32> TexMin;	// Min TexCoords
	irr::core::dimension2d<irr::f32> TexMax;	// Max TexCoords
	IDirect3DIndexBuffer9* Indices;				// Index Buffer
	IDirect3DVertexBuffer9* Vertices;			// Vertex Buffer
	IDirect3DDevice9* Device;					// DX Device

public:

	irr::video::SColor Color;	// Color of quad
	irr::f32 tx,ty,tw,th;	// Global dimensions
	irr::f32 lx,ly,lw,lh;	// Local dimensions

	// Constructor
	SAQuadSceneNode(irr::scene::ISceneNode* parent, irr::scene::ISceneManager* mgr, irr::s32 id)
		: irr::scene::ISceneNode(parent, mgr, id)
	{
		Material.Wireframe = false;
		Material.Lighting = false;
		Material.MaterialType = irr::video::EMT_TRANSPARENT_VERTEX_ALPHA;
		Color = irr::video::SColor(255,255,255,255);
		TexMin = irr::core::dimension2df(0.0f,0.0f);
		TexMax = irr::core::dimension2df(1.0f,1.0f);
		Device = ((irr::video::CD3D9Driver*)driver)->pID3DDevice;
		Indices = NULL;
		Vertices = NULL;
	}

	// Rebuild mesh
	virtual void RebuildVertexBuffers()
	{
		// If indices are null, make new ones
		if(Indices == NULL)
		{
			Device->CreateIndexBuffer(24, D3DUSAGE_WRITEONLY, D3DFMT_INDEX32, D3DPOOL_MANAGED, &Indices, NULL);
			
			irr::u32* LockedIndices;
			Indices->Lock(0, 0, (void**)&LockedIndices, 0);

			LockedIndices[0] = 2;
			LockedIndices[1] = 1;
			LockedIndices[2] = 0;
			LockedIndices[3] = 1;
			LockedIndices[4] = 2;
			LockedIndices[5] = 3;

			Indices->Unlock();
		}

		// If no vertices exist, make a new buffer
		if(Vertices == NULL)
		{
			Device->CreateVertexBuffer(sizeof(irr::video::S3DVertex) * 4,
				D3DUSAGE_WRITEONLY,
				0,
				D3DPOOL_MANAGED,
				&Vertices, NULL);
		}

		// Set the positions of the vertices
		SAQuad[0].Pos = vector3df(((tx*2)-1)     , -(((ty+th)*2)-1) ,0);
		SAQuad[1].Pos = vector3df((((tx+tw)*2)-1), -(((ty+th)*2)-1) ,0);
		SAQuad[2].Pos = vector3df(((tx*2)-1)     , -((ty*2)-1) ,0);
		SAQuad[3].Pos = vector3df((((tx+tw)*2)-1), -((ty*2)-1) ,0);

		// Set Coords of vertices
		SAQuad[0].TCoords.set(TexMin.Width,TexMax.Height);
		SAQuad[1].TCoords.set(TexMax.Width,TexMax.Height);
		SAQuad[2].TCoords.set(TexMin.Width,TexMin.Height);
		SAQuad[3].TCoords.set(TexMax.Width,TexMin.Height);

		// Set Color of verts
		for(int i = 0; i < 4; i++)
			SAQuad[i].Color = Color;

		// Lock hardware buffer
		irr::video::S3DVertex* LockedVerts;
		Vertices->Lock(0, 0, (void**)&LockedVerts, 0);

		// Copy Data
		LockedVerts[0] = SAQuad[0];
		LockedVerts[1] = SAQuad[1];
		LockedVerts[2] = SAQuad[2];
		LockedVerts[3] = SAQuad[3];

		// Unlock
		Vertices->Unlock();

	}

	// Drop SaQuad
	virtual ~SAQuadSceneNode()
	{
		Material.Texture1->drop();
	}

	// Register this for rendering
	virtual void OnPreRender()
	{
		if (IsVisible && Order == 0)
			SceneManager->registerNodeForRendering(this);

		ISceneNode::OnPreRender();
	}

	//Render!
	virtual void render()
	{
		// Lame hack due to Gooey issue
		if(this->Alpha < 0.001f)
			return;

		// Render if verts and indices exist
		if(Vertices != NULL && Indices != NULL)
		{
			SceneManager->getVideoDriver()->setMaterial(Material);
			Device->SetIndices(Indices);
			Device->SetVertexDeclaration(driver->GetStandardDefinition());
			Device->SetStreamSource(0, Vertices, 0, sizeof(irr::video::S3DVertex));
			Device->DrawIndexedPrimitive(D3DPT_TRIANGLELIST, 0, 0, 4, 0, 2);
		}
	}

	// Set Combined dimensions
	virtual void SetLayout(irr::f32 x, irr::f32 y, irr::f32 w, irr::f32 h)
	{
		setScale(irr::core::vector3df(w*2.0f, h*2.0f, 1));
		setPosition(irr::core::vector3df((x*2.0f)-1.0f, 1.0f-(y*2.0f), 0));
		lx = x;
		ly = y;
		lw = w;
		lh = h;
		RebuildAbsoluteTransform();
		//RebuildVertexBuffers();
	}

	// Set Position
	virtual void SetPosition(irr::f32 x, irr::f32 y)
	{
		setPosition(irr::core::vector3df((x*2.0f)-1.0f, 1.0f-(y*2.0f), 0));

		lx = x;
		ly = y;

		RebuildAbsoluteTransform();
	}

	// Set Scale
	virtual void SetScale(irr::f32 x, irr::f32 y)
	{
		lw = x;
		lh = y;
		RebuildAbsoluteTransform();
		//RebuildVertexBuffers();
	}

	// Rebuild Transform from parents
	virtual void RebuildAbsoluteTransform()
	{
		// Setup temp dimensions
		irr::f32 x = lx;
		irr::f32 y = ly;
		irr::f32 w = lw;
		irr::f32 h = lh;

		// If there is a parent and its a quad
		if(this->Parent && this->Parent->getType() == ESNT_SAQUAD)
		{
			// Get it an calculate parenting
			SAQuadSceneNode* P = (SAQuadSceneNode*)this->Parent;
			x *= P->GetAbsoluteScale().Width;
			y *= P->GetAbsoluteScale().Height;
			x += P->GetAbsolutePosition().Width;
			y += P->GetAbsolutePosition().Height;
			w *= P->GetAbsoluteScale().Width;
			h *= P->GetAbsoluteScale().Height;
		}

		// Set globals
		tx = x;
		ty = y;
		tw = w;
		th = h;

		// Update children
		irr::core::list<ISceneNode*>::Iterator It = this->Children.begin();
		for(;It != this->Children.end(); ++It)
		{
			if((*It)->getType() == ESNT_SAQUAD)
				((SAQuadSceneNode*)(*It))->RebuildAbsoluteTransform();
			if((*It)->getType() == ESNT_SATEXT)
				((SATextSceneNode*)(*It))->RebuildAbsoluteTransform();
		}

		// Update mesh
		RebuildVertexBuffers();
	}

	// Get Position
	virtual irr::core::dimension2df GetAbsolutePosition()
	{
		return dimension2df(tx,ty);
	}

	// Get Scale
	virtual irr::core::dimension2df GetAbsoluteScale()
	{
		return dimension2df(tw,th);
	}
	
	// Set Colour
	virtual void SetColour(irr::s32 r, irr::s32 g, irr::s32 b)
	{
		Color = irr::video::SColor(Color.getAlpha(),r,g,b);
		RebuildVertexBuffers();
	}

	// Compatibility
	virtual const irr::core::aabbox3d<irr::f32>& getBoundingBox() const
	{
		return SABox;
	}

	// Compatibility
	virtual irr::s32 getMaterialCount()
	{
		return 1;
	}

	// Compatibility
	virtual irr::video::SMaterial& getMaterial(irr::s32 i)
	{
		return Material;
	}

	// Get Type
	virtual irr::scene::ESCENE_NODE_TYPE getType()
	{
		return irr::scene::ESNT_SAQUAD;
	}

	// Set Texture Coords
	virtual void SetCoords(irr::f32 LX, irr::f32 LY, irr::f32 HX, irr::f32 HY)
	{
		TexMin.Width = LX;
		TexMin.Height = LY;
		TexMax.Width = HX;
		TexMax.Height = HY;
		RebuildVertexBuffers();
	}

	virtual void WriteSceneGraph(irr::io::IWriteFile* File, core::stringc Tabbage)
	{
		char o[128];
		sprintf(o, "%sSAQuad(%x, %s)\r\n", Tabbage.c_str(), this, Name.c_str());
		core::stringc Write = o;

		File->write(o, Write.size());
		WriteDesc(File, Tabbage);
	}
};

// Rebuild Transform of SAText
void SATextSceneNode::RebuildAbsoluteTransform()
{
	// Set temps (NO SCALE, it'll screw it up)
	irr::f32 x= lx;
	irr::f32 y = ly;

	// if the parent is a saquad (not satext!)
	if(this->Parent && this->Parent->getType() == ESNT_SAQUAD)
	{
		// Calc
		SAQuadSceneNode* P = (SAQuadSceneNode*)this->Parent;
		x *= P->GetAbsoluteScale().Width;
		y *= P->GetAbsoluteScale().Height;
		x += P->GetAbsolutePosition().Width;
		y += P->GetAbsolutePosition().Height;
	}

	// Set Global position
	this->X = x;
	this->Y = y;

	// Update Children
	irr::core::list<ISceneNode*>::Iterator It = this->Children.begin();
	for(;It != this->Children.end(); ++It)
	{
		if((*It)->getType() == ESNT_SAQUAD)
			((SAQuadSceneNode*)(*It))->RebuildAbsoluteTransform();
	}

	// Update Mesh
	this->SetText(this->CText.c_str());
}

// Make a new SaQuad
DLLPRE SAQuadSceneNode* DLLEX jesusownzall(irr::scene::ISceneNode* parent, irr::u32 shd)
{
	SAQuadSceneNode* node = new SAQuadSceneNode(smgr->getRootSceneNode(),smgr,-1);
	node->setEffect(shd);
	node->getMaterial(0).Texture1 = driver->getDefaultTexture();

	if(parent && parent->getType() == ESNT_SAQUAD)
		crystalclear(node, parent, true);
	return node;
}

// Set Position and Scale of a SA Object
DLLPRE void DLLEX hardrock(ISceneNode* node, irr::f32 x, irr::f32 y, irr::f32 w, irr::f32 h)
{
	if(node->getType() == irr::scene::ESNT_SAQUAD)
		((SAQuadSceneNode*)node)->SetLayout(x,y,w,h);
	else if(node->getType() == irr::scene::ESNT_SATEXT)
		((SATextSceneNode*)node)->SetPosition(x,y);
}

// Set Color of SAObject
DLLPRE void DLLEX makemav(ISceneNode* node, irr::s32 r, irr::s32 g, irr::s32 b)
{
	if(node->getType() == irr::scene::ESNT_SAQUAD)
		((SAQuadSceneNode*)node)->SetColour(r,g,b);
	else if(node->getType() == irr::scene::ESNT_SATEXT)
		((SATextSceneNode*)node)->SetColour(r,g,b);
}

// Set Alpha of SAObject
DLLPRE void DLLEX shogun(ISceneNode* node, irr::f32 a)
{
	if(node->getType() == irr::scene::ESNT_SATEXT)
		((SATextSceneNode*)node)->SetAlpha((irr::s32)a * 255.0f);
}

// Set Position of SAObject
DLLPRE void DLLEX SetPosition(ISceneNode* node, irr::f32 x, irr::f32 y)
{
	if(node->getType() == irr::scene::ESNT_SAQUAD)
		((SAQuadSceneNode*)node)->SetPosition(x,y);
	else if(node->getType() == irr::scene::ESNT_SATEXT)
		((SATextSceneNode*)node)->SetPosition(x,y);
}

// Set Scale of SAObject
DLLPRE void DLLEX SetScale(ISceneNode* node, irr::f32 x, irr::f32 y)
{
	if(node->getType() == irr::scene::ESNT_SAQUAD)
		((SAQuadSceneNode*)node)->SetScale(x,y);
	else if(node->getType() == irr::scene::ESNT_SATEXT)
		((SATextSceneNode*)node)->SetScale(x,y);
}

// Set SAQuad TexCoords
DLLPRE void DLLEX SetTexCoords(SAQuadSceneNode* Node, irr::f32 LX, irr::f32 LY, irr::f32 HX, irr::f32 HY)
{
	Node->SetCoords(LX, LY, HX, HY);
}

// Get Scale of SAObject
dimension2df GetScale(ISceneNode* node)
{
	if(node->getType() == irr::scene::ESNT_SAQUAD)
		return ((SAQuadSceneNode*)node)->GetAbsoluteScale();
	return dimension2df(0.0f, 0.0f);
}

// Get Position of SAObject
dimension2df GetPosition(ISceneNode* node)
{
	if(node->getType() == irr::scene::ESNT_SAQUAD)
		return ((SAQuadSceneNode*)node)->GetAbsolutePosition();
	else if(node->getType() == irr::scene::ESNT_SATEXT)
		return ((SATextSceneNode*)node)->GetAbsolutePosition();
	return dimension2df(0.0f, 0.0f);
}



