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

// 3D Line object
class LineSceneNode : public irr::scene::ISceneNode
{

	irr::scene::ISceneManager* Smgr;	// Scene Manager
	irr::core::vector3df Start;			// Line Start
	irr::core::vector3df End;			// Line End
	irr::video::SColor Color;			// Line Color
	bool IsVisible;						// Render it?

public:
	
	// Constructor
	LineSceneNode(irr::scene::ISceneNode* parent, irr::scene::ISceneManager* mgr, irr::s32 id)
		: irr::scene::ISceneNode(parent, mgr, id)
	{
		Smgr = mgr;
		Start = vector3df(0,0,0);
		End = vector3df(0,0,0);
		Color = SColor(255,255,255,255);
		this->setAutomaticCulling(false);
		IsVisible = true;
	}

	// Destructor
	virtual ~LineSceneNode()
	{
		
	}

	// Prerender
	virtual void OnPreRender()
	{
		if (IsVisible && Order == 0)
			SceneManager->registerNodeForRendering(this, ESNRP_SOLID); // Warning:4482 - irr::scene::E_SCENE_NODE_RENDER_PASS::ESNRP_SOLID);

		ISceneNode::OnPreRender();
	}

	// Post render
	virtual void OnPostRender(irr::u32 T)
	{
		ISceneNode::OnPostRender(T);
	}

	// Draw the line
	virtual void render()
	{
		if(IsVisible)
			driver->draw3DLine(Start,End,Color);
	}

	// Set is visbility
	virtual void SetVisible(bool Visible)
	{
		IsVisible = Visible;
	}

	// Set its color
	virtual void SetColor(SColor Col)
	{
		Color = Col;
	}

	// Set its start
	virtual void SetStart(vector3df S)
	{
		Start = S;
	}

	// Set End
	virtual void SetEnd(vector3df E)
	{
		End = E;
	}

	virtual void WriteSceneGraph(irr::io::IWriteFile* File, core::stringc Tabbage)
	{
		char o[128];
		sprintf(o, "%sLineSceneNode(%x, %s)\r\n", Tabbage.c_str(), this, Name.c_str());
		core::stringc Write = o;

		File->write(o, Write.size());
		WriteDesc(File, Tabbage);
	}

	// Compatibility
	virtual const irr::core::aabbox3d<irr::f32>& getBoundingBox() const
	{
		static aabbox3df boundingBox(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
		return boundingBox;
	}

};

// Create a 3D Line
DLLPRE LineSceneNode* DLLEX CreateLine(ISceneNode* parent)
{
	if(!parent)
		parent = smgr->getRootSceneNode();
	return new LineSceneNode(parent,smgr,-1);
}

// Free a Line
DLLPRE void DLLEX FreeLine(LineSceneNode* line)
{
	if(line->getParent())
		line->remove();
	line->removeAll();

	if(!line->drop())
		delete line;
}

// Set line layout
DLLPRE void DLLEX SetLineSize(LineSceneNode* line, irr::f32 sx, irr::f32 sy, irr::f32 sz, irr::f32 ex, irr::f32 ey, irr::f32 ez)
{
	line->SetStart(vector3df(sx,sy,sz));
	line->SetEnd(vector3df(ex,ey,ez));
}

// Set Line Color
DLLPRE void DLLEX SetLineColor(LineSceneNode* line, irr::s32 r, irr::s32 g, irr::s32 b)
{
	line->SetColor(irr::video::SColor(255,r,g,b));
}

// Set lines visibility
DLLPRE void DLLEX SetLineVisible(LineSceneNode* line, irr::s32 a)
{
	if(a == 1)
		line->SetVisible(true);
	else
		line->SetVisible(false);
}

