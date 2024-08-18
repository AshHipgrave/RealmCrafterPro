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
#include "GroupedObject.h"
#include "IAnimatedMeshSceneNode.h"
#include <XMLWrapper.h>
#include <list>
#include "ASyncJobModule.h"

using namespace irr;
using namespace irr::io;
using namespace irr::scene;
using namespace NGin;
using namespace std;

extern "C" __declspec(dllexport) ISceneNode* carjacking(irr::scene::ISceneNode* Parent);
extern "C" __declspec(dllexport) void crystalclear(irr::scene::ISceneNode* Node, irr::scene::ISceneNode* Parent, int Global);
extern "C" __declspec(dllexport) irr::scene::IAnimatedMeshSceneNode* jalkming(irr::c8* filename, irr::scene::ISceneNode* parent, int Animated, bbdx2_ASyncJobFn completionCallback, void* userData);
extern "C" __declspec(dllexport) irr::u32 deterkis(irr::c8* Filename);
extern "C" __declspec(dllexport) void slobing(irr::scene::ISceneNode* Node, irr::u32 Shader);
extern "C" __declspec(dllexport) void jaffamak(irr::scene::ISceneNode* Node, float x, float y, float z, int Global);
extern "C" __declspec(dllexport) void mingja(irr::scene::ISceneNode* Node, float x, float y, float z, int Global);
extern "C" __declspec(dllexport) void chaosdigs(irr::scene::ISceneNode* Node, float x, float y, float z, int Global);


extern "C" __declspec(dllexport) irr::scene::ISceneNode* bbdx2_LoadGroupedObject(const char* filename, irr::scene::ISceneNode* parent)
{
	NGin::XMLReader* Reader = NGin::ReadXMLFile(filename);
	if(Reader == 0)
		return 0;

	ISceneNode* MasterPivot = carjacking(parent);
	std::list<ISceneNode*> InMesh;
	std::list<ISceneNode*> Meshes;

	InMesh.push_front(MasterPivot);

	while(Reader->Read())
	{
		if(Reader->GetNodeType() == NGin::XNT_Element && Reader->GetNodeName() == "object")
		{
			std::string Src = Reader->GetAttributeString("src");
			std::string Shader = Reader->GetAttributeString("shader");

			ISceneNode* Mesh = jalkming((irr::c8*)Src.c_str(), 0, 0, 0, 0);
			if(Mesh != 0)
			{
				u32 Shdr = deterkis((irr::c8*)Shader.c_str());
				slobing(Mesh, Shdr);

				InMesh.push_front(Mesh);
				Meshes.push_back(Mesh);
			}
		}

		if(Reader->GetNodeType() == NGin::XNT_Element_End && Reader->GetNodeName() == "object")
		{
			InMesh.pop_front();
		}

		
		if(Reader->GetNodeType() == NGin::XNT_Element && Reader->GetNodeName() == "position")
		{
			float x = Reader->GetAttributeFloat("x");
			float y = Reader->GetAttributeFloat("y");
			float z = Reader->GetAttributeFloat("z");

			if(InMesh.size() > 0)
				chaosdigs(*(InMesh.begin()), x, y, z, 0);
		}

		if(Reader->GetNodeType() == NGin::XNT_Element && Reader->GetNodeName() == "rotation")
		{
			float x = Reader->GetAttributeFloat("x");
			float y = Reader->GetAttributeFloat("y");
			float z = Reader->GetAttributeFloat("z");

			if(InMesh.size() > 0)
				mingja(*(InMesh.begin()), x, y, z, 0);
		}

		if(Reader->GetNodeType() == NGin::XNT_Element && Reader->GetNodeName() == "scale")
		{
			float x = Reader->GetAttributeFloat("x");
			float y = Reader->GetAttributeFloat("y");
			float z = Reader->GetAttributeFloat("z");

			if(InMesh.size() > 0)
				jaffamak(*(InMesh.begin()), x, y, z, 0);
		}
	}

	for(std::list<ISceneNode*>::iterator It = Meshes.begin(); It != Meshes.end(); ++It)
	{
		crystalclear(*It, MasterPivot, 1);
	}

	delete Reader;

	return MasterPivot;
	
}