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

//                                                                                    VV Remember, Double slash!
char* AllLetters = " ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789,./\\'#?<>[]();:!£$%^&*+-@~=_|àáâãäçèéêëïñòóôõöùúûüýÿßÆæØøÅåñÑ";

// Just a definition for later
class SAQuadSceneNode;

// Get X offset of a letter
float XOffset(unsigned char Letter)
{
	unsigned char C = 0;
	for(int y = 0; y < 16; ++y)
		for(int x = 0; x < 16; ++x)
			if(Letter == C)
				return x;
			else
				++C;
	return 0;
}

// Get Y offset of a letter
float YOffset(unsigned char Letter)
{
	unsigned char C = 0;
	for(int y = 0; y < 16; ++y)
		for(int x = 0; x < 16; ++x)
			if(Letter == C)
				return y;
			else
				++C;
	return 0;
}

// SAText Node
class SATextSceneNode : public irr::scene::ISceneNode
{
private:
	irr::core::aabbox3d<irr::f32> SABox;		// bounding box of node (unused, mostly)
	irr::video::SMaterial Material;				// Material (unused)
	IDirect3DDevice9* Device;					// DX Device
	irr::video::SColor Default;
	irr::core::stringc CText;					// String
	irr::s32 Alpha;								// Alpha
	irr::f32 X, Y;								// Position
	irr::f32 lx, ly;							// Local position (used in SAParents)

	float CumulativeWidth;						// Width

public: 

		ID3DXFont* Font;								// Font
	bool OverideInlineEscapes;	// Used to get rid of \X's
	float CHeight;				// Dimensions
	float CWidth;				// Dimensions

	// Constructor
	SATextSceneNode(irr::scene::ISceneNode* parent, irr::scene::ISceneManager* mgr, irr::s32 id)
		: irr::scene::ISceneNode(parent, mgr, id)
	{
		// Set Defaults
		Device = ((irr::video::CD3D9Driver*)driver)->pID3DDevice;
		X = 0.0f;
		Y = 0.0f;
		CText = "<Null>";
		Alpha = 255;
		CWidth = 0.2f;
		CHeight = 0.03f;
		Default = SColor(255, 255, 255, 255);
	}

	// Set the font
	virtual void SetFont(ID3DXFont* font)
	{
		Font = font;
	}
		
	// Set Text
	virtual void SetText(const irr::c8* NewText)
	{
		CText = NewText;
	}

	// Draw!
	virtual void render()
	{
		if(Font != NULL)
		{
			RECT DrawRect;
			irr::core::dimension2d<irr::s32> Size = driver->getScreenSize();
			DrawRect.top = (Y * Size.Height) + 2;
			DrawRect.left = (X * Size.Width) + 3;
			DrawRect.right = 10000;
			DrawRect.bottom = 10000;
			
			Font->DrawTextA(NULL, CText.c_str(), CText.size(), &DrawRect, 0, D3DCOLOR_ARGB(Alpha, 0, 0, 0));

			DrawRect.top -= 2;
			DrawRect.left -= 3;
			Font->DrawTextA(NULL, CText.c_str(), CText.size(), &DrawRect, 0, D3DCOLOR_ARGB(Alpha, Default.getRed(), Default.getGreen(), Default.getBlue()));
		}
	}

	// Here for compatibility
	virtual const irr::core::aabbox3d<irr::f32>& getBoundingBox() const
	{
		return SABox;
	}

	// Return text
	virtual stringc GetText()
	{
		return CText;
	}

	// Set position
	virtual void SetPosition(irr::f32 x, irr::f32 y)
	{
		lx = x;
		ly = y;
		RebuildAbsoluteTransform();
	}

	// Definition, filled later
	virtual void RebuildAbsoluteTransform();

	// Get Absolute position
	virtual irr::core::dimension2df GetAbsolutePosition()
	{
		return dimension2df(X,Y);
	}

	// Set Default Colour
	virtual void SetColour(irr::s32 r, irr::s32 g, irr::s32 b)
	{
		this->Default.setRed(r);
		this->Default.setGreen(g);
		this->Default.setBlue(b);
		this->SetText(this->CText.c_str());
	}

	// Set Alpha
	virtual void SetAlpha(irr::s32 a)
	{
		this->Alpha = a;
		this->SetText(this->CText.c_str());
	}

	// Get node type
	virtual irr::scene::ESCENE_NODE_TYPE getType()
	{
		return irr::scene::ESNT_SATEXT;
	}

	// Set Scale (NEVER USE THIS) (Honestly! it looks crap you don't need to!)
	virtual void SetScale(irr::f32 x, irr::f32 y)
	{
		CWidth = x;
		CHeight = y;
		this->SetText(this->CText.c_str());
	}

	// Return text width
	virtual float GetWidth()
	{
		return this->CumulativeWidth;
	}

	virtual void WriteSceneGraph(irr::io::IWriteFile* File, core::stringc Tabbage)
	{
		char o[128];
		sprintf(o, "%sSAText(%x, %s, %s)\r\n", Tabbage.c_str(), this, Name.c_str(), this->CText.c_str());
		core::stringc Write = o;

		File->write(o, Write.size());
		WriteDesc(File, Tabbage);
	}

};

// Load a font
DLLPRE void* DLLEX LoadSAFont(const irr::c8* Filename, int Size, int Bold, int Italic)
{
	return driver->LoadFont(Filename, Size, Bold, Italic);
}

// Free a font
DLLPRE void DLLEX FreeSAFont(void* Font)
{
	driver->FreeFont(Font);
}

// Create Text item
DLLPRE SATextSceneNode* DLLEX CreateSAText(irr::u32 Shader, ID3DXFont* Font)
{
	SATextSceneNode* Node = new SATextSceneNode(smgr->getRootSceneNode(), smgr, -1);
	Node->setEffect(Shader);
	Node->SetFont(Font);
	Node->SetText("<Null>");
	return Node;
}

// Set Text Text
DLLPRE void DLLEX SetSAText(SATextSceneNode* Node, const irr::c8* Text)
{
	Node->SetText(Text);	
}

// Is \c allowed?
DLLPRE void DLLEX SetSAInlineProcessing(SATextSceneNode* Node, int Process)
{

}

// Set Scale (Really, don't use it)
DLLPRE void DLLEX SetSATextScale(SATextSceneNode* Node, irr::f32 X, irr::f32 Y)
{
	Node->SetScale(X,Y);
}

// Get Width of any string
DLLPRE float DLLEX GetSATextWidth(SATextSceneNode* Node, const irr::c8* Text)
{
	RECT Size;
	ZeroMemory(&Size, sizeof(RECT));
	Node->Font->DrawTextA(NULL, Text, -1, &Size, DT_CALCRECT, D3DCOLOR_ARGB(255,0,0,0));

	float TW = (float)Size.right;
	float SW = (float)driver->getScreenSize().Width;

	return (TW / SW);
}

