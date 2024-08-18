#include "BlitzPlus.h"
#include <windows.h>
#include <exception>
#include "MenuGUI.h"


#include <iostream>
using namespace std;

#define HideMain HideGadget(MStart);HideGadget(MGraphics);HideGadget(MControl);HideGadget(MOther);HideGadget(MQuit);
#define ShowMain ShowGadget(MStart);ShowGadget(MGraphics);ShowGadget(MControl);ShowGadget(MOther);ShowGadget(MQuit);
#define HideGfx HideGadget(GPnl);HideGadget(GResLabl);HideGadget(GResSel);HideGadget(GAntiLabl);HideGadget(GAnti);HideGadget(GShdwLabl);HideGadget(GShdw);HideGadget(GCnl);HideGadget(GDon);
#define ShowGfx ShowGadget(GPnl);ShowGadget(GResLabl);ShowGadget(GResSel);ShowGadget(GAntiLabl);ShowGadget(GAnti);ShowGadget(GShdwLabl);ShowGadget(GShdw);ShowGadget(GCnl);ShowGadget(GDon);
#define HideMusic HideGadget(GPnl);HideGadget(SMusLabl);HideGadget(SMusSld);HideGadget(SDLLabl);HideGadget(SDL);HideGadget(SCnl);HideGadget(SDon);
#define ShowMusic ShowGadget(GPnl);ShowGadget(SMusLabl);ShowGadget(SMusSld);ShowGadget(SDLLabl);ShowGadget(SDL);ShowGadget(SCnl);ShowGadget(SDon);
#define HideControls for(int i = 0; i < 19; ++i){HideGadget(CLabls[i]);}HideGadget(CPnl);HideGadget(CInv1);HideGadget(CInv2);HideGadget(CDone);
#define ShowControls for(int i = 0; i < 19; ++i){ShowGadget(CLabls[i]);}ShowGadget(CPnl);ShowGadget(CInv1);ShowGadget(CInv2);ShowGadget(CDone);


int main()
{
	int GFXWidth = 1024, GFXHeight = 768;
	bool GFXAntiAlias = true, GFXShadows = true;
	int InvertAxis1 = 1, InvertAxis3 = -1;
	bool UpdateMusic = true;
	float DefaultVolume = 0.75f;
	String GameName, UpdateGame;

	
	uint Win = BBCreateWindow("Hello!!",BBClientWidth(BBDesktop()) / 2 - 400, BBClientHeight(BBDesktop()) / 2 - 300, 800, 600, 0, 1);
	
	Init(BBQueryObject(Win, 1), 0);

	BBRegisterDrawCallback((DrawCallbackFN)&DrawCallback, Win);

	InitGUI(Win);

	String MenuDir = "C:\\Menu\\";
	SetBackground(MenuDir + "Menu");

#pragma region Main Panel
	// Main Menu bit
	uint MStart = CreateButton(MenuDir + "BStart", 272, 50);
	uint MGraphics = CreateButton(MenuDir + "BGraphics", 272, 150);
	uint MControl = CreateButton(MenuDir + "BControl", 272, 200);
	uint MOther = CreateButton(MenuDir + "BOther", 272, 250);
	uint MQuit = CreateButton(MenuDir + "BQuit", 272, 350);
#pragma endregion

#pragma region Graphics Panel
	// Graphics options panel
	uint GPnl = CreatePanel(MenuDir + "MenuPanel", 295, 200);

	
	uint GResLabl = CreateLabel("Resolution:", 305, 210);
	uint GResSel = CreateComboBox(MenuDir + "Combo", 305, 230, 100);

	int Count = CountGraphicsModes();
	for(int i = 0; i < Count; ++i)
	{
		int Width = GraphicsModeWidth(i);
		int Height = GraphicsModeHeight(i);
		if(Width >= 800 && Height >= 600)
		{
			String ModeName = String(Width) + " x " + String(Height);
			
			bool Found = false;
			for(int j = 0; j < CountComboItems(GResSel); ++j)
			{
				if(ComboItemText(GResSel, j) == ModeName)
				{
					Found = true;
					break;
				}
			}

			if(Found == false)
			{
				AddComboItem(GResSel, ModeName, String(i));
				if(Width == GFXWidth && Height == GFXHeight)
					SelectComboItem(GResSel, CountComboItems(GResSel) - 1);
			}	
		}
	}

	uint GAntiLabl = CreateLabel("Enable AntiAliasing", 320, 263);
	uint GAnti = CreateButton(MenuDir + "Check", 305, 265, true);

	uint GShdwLabl = CreateLabel("Enable Shadows", 320, 283);
	uint GShdw = CreateButton(MenuDir + "Check", 305, 285, true);

	uint GCnl = CreateButton(MenuDir + "B2DCancel", 405, 310, false);
	uint GDon = CreateButton(MenuDir + "BAccept", 305, 310, false);

	SetButtonHit(GAnti, GFXAntiAlias);
	SetButtonHit(GShdw, GFXShadows);

	HideGfx;
#pragma endregion

#pragma region Misc Panel
	// Music Settings
	uint SMusLabl = CreateLabel("Music Volume:", 305, 210);
	uint SMusSld = CreateSlider(MenuDir + "Slider", 305, 230);

	uint SDLLabl = CreateLabel("Omit music updates", 320, 263);
	uint SDL = CreateButton(MenuDir + "Check", 305, 265, true);

	uint SCnl = CreateButton(MenuDir + "B2DCancel", 405, 310, false);
	uint SDon = CreateButton(MenuDir + "BAccept", 305, 310, false);

	SetButtonHit(SDLLabl, !UpdateMusic);
	SetSliderValue(SMusSld, ((int)(DefaultVolume * 100.0f)));

	HideMusic;
#pragma endregion

#pragma region Controls Panel
	// Control options
	uint CPnl = CreatePanel(MenuDir + "MenuControlsPanel", 240, 80);

	uint CLabls[22];
	CLabls[0]  = CreateLabel("Move Forward", 250, 90);
	CLabls[1]  = CreateLabel("Move Back", 250, 110);
	CLabls[2]  = CreateLabel("Turn Right", 250, 130);
	CLabls[3]  = CreateLabel("Turn Left", 250, 150);
	CLabls[4]  = CreateLabel("Fly/Swim Up", 250, 170);
	CLabls[5]  = CreateLabel("Fly/Swim Down", 250, 190);
	CLabls[6]  = CreateLabel("Jump", 250, 210);
	CLabls[7]  = CreateLabel("Run", 250, 230);
	CLabls[8]  = CreateLabel("Always Run Toggle", 250, 250);
	CLabls[9]  = CreateLabel("Select", 250, 270);
	CLabls[10] = CreateLabel("Move To", 250, 290);
	CLabls[11] = CreateLabel("Talk To", 250, 310);
	CLabls[12] = CreateLabel("Attack Target", 250, 330);
	CLabls[13] = CreateLabel("Cycle Target", 250, 350);
	CLabls[14] = CreateLabel("Change View Mode", 250, 370);
	CLabls[15] = CreateLabel("Zoom In", 250, 390);
	CLabls[16] = CreateLabel("Zoom Out", 250, 410);
	CLabls[17] = CreateLabel("Invert Mouse (First Person)", 270, 448);
	CLabls[18] = CreateLabel("Invert Mouse (Third Person)", 270, 468);

	uint CInv1 = CreateButton(MenuDir + "Check", 250, 450, true);
	uint CInv2 = CreateButton(MenuDir + "Check", 250, 470, true);
	uint CDone = CreateButton(MenuDir + "BAccept", 450, 490);

	SetButtonHit(CInv1, (InvertAxis1 == -1) ? true : false);
	SetButtonHit(CInv2, (InvertAxis3 == -1) ? true : false);

	HideControls;
	//CreateLabel("Left Mouse Button", 400, 250);
#pragma endregion



	bool IsMenu = true;
	while(IsMenu)
	{
		uint ev = BBWaitEvent(10);
		if(ev == 0x803) exit(0);

		UpdateGUI();

		// Main menu updates
		if(ButtonHit(MQuit) || KeyHit(1))
			exit(0);

		if(ButtonHit(MStart))
			IsMenu = false;

		if(ButtonHit(MGraphics))
		{
			HideMain;
			ShowGfx;
		}

		if(ButtonHit(MControl))
		{
			HideMain;
			ShowControls;
		}

		if(ButtonHit(MOther))
		{
			HideMain;
			ShowMusic;
		}


		// Graphics Menu Updates
		if(ButtonHit(GCnl))
		{
			HideGfx;

			String ModeName = String(GFXWidth) + " x " + String(GFXHeight);
			for(int j = 0; j < CountComboItems(GResSel); ++j)
				if(ComboItemText(GResSel, j) == ModeName)
					SelectComboItem(GResSel, j);
	
			SetButtonHit(GAnti, GFXAntiAlias);
			SetButtonHit(GShdw, GFXShadows);

			ShowMain;
		}

		if(ButtonHit(GDon))
		{
			HideGfx;
			
			int ModeNum = ComboItemTag(GResSel, SelectedComboItem(GResSel)).ToInt();
			GFXWidth = GraphicsModeWidth(ModeNum);
			GFXHeight = GraphicsModeHeight(ModeNum);
			GFXAntiAlias = ButtonHit(GAnti);
			GFXShadows = ButtonHit(GShdw);

			FILE* F = WriteFile("Data\\Options.dat");
			if(F == 0)
				RuntimeError("Could not open Data\\Options.dat!");
			WriteShort(F, GFXWidth);
			WriteShort(F, GFXHeight);
			WriteByte(F, 32);
			WriteByte(F, GFXAntiAlias ? 1 : 0);
			WriteFloat(F, DefaultVolume);
			WriteByte(F, 0);
			CloseFile(F);

			ShowMain;
		}

		// Controls Menu Updates
		if(ButtonHit(CDone))
		{
			HideControls;


			ShowMain;
		}

		// Misc Menu Updates
		if(ButtonHit(SCnl))
		{
			HideMusic;

			SetButtonHit(SDLLabl, !UpdateMusic);
			SetSliderValue(SMusSld, ((int)(DefaultVolume * 100.0f)));

			ShowMain;
		}

		if(ButtonHit(SDon))
		{
			HideMusic;

			DefaultVolume = (float)(GetSliderValue(SMusSld) / 100.0f);
			UpdateMusic = !ButtonHit(SDL);

			FILE* F = WriteFile("Data\\Options.dat");
			if(F == 0)
				RuntimeError("Could not open Data\\Options.dat!");
			WriteShort(F, GFXWidth);
			WriteShort(F, GFXHeight);
			WriteByte(F, 32);
			WriteByte(F, GFXAntiAlias ? 1 : 0);
			WriteFloat(F, DefaultVolume);
			WriteByte(F, 0);
			CloseFile(F);

			F = WriteFile("Data\\Game Data\\Misc.dat");
			if(F == 0)
				RuntimeError("Could not open Data\\Game Data\\Misc.dat");

			WriteLine(F, GameName);
			WriteLine(F, UpdateGame);
			WriteLine(F, String(ButtonHit ? 1 : 0));

			CloseFile(F);

			ShowMain;
		}

	}

	exit(0);
	return 0;
}

//Image* BBLoadImage(CString filename)
//{
//    CStringW  sWideString(filename);
//    Image* img = new Image(sWideString);
//    return img;
//}
//
//void BBDrawImage(HDC DC, Image* image, int X, int Y)
//{
//    Graphics graphics( DC );
//    graphics.DrawImage( image, 10, 10);
//}
