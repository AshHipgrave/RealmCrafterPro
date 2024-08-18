#pragma once

#include <nginstring.h>
#include <list.h>
#include <windows.h>
#include <IGUIManager.h>

class CProfiler
{
public:

	struct Node
	{
		uint ID;
		Node* Parent;
		NGin::String Name;
		List<Node*> Nodes;
		uint Time;
		uint Start;

		NGin::GUI::IGUIManager* Manager;
		NGin::GUI::ILabel* Label, *MSLabel, *PLabel;
		NGin::GUI::IButton* Button;
		bool Expanded;

		Node(Node* Parent, NGin::GUI::IGUIManager* Manager);
		~Node();
	};

protected:

	Node* RootNode;
	Node* CurrentNode;

	bool Capturing;
	bool StartCapture;

	NGin::GUI::IPictureBox* ProfileBG;
	
	

public:

	NGin::GUI::IGUIManager* Manager;
	NGin::ArrayList<NGin::GUI::IButton*> PageButtons;
	NGin::uint StartCount, StartIndex;

	CProfiler();
	~CProfiler();

	void GenerateNodeGUI();

	void Update();

	void Recursive(CProfiler::Node* node, int dec, uint totalTime, float& Height);
	void RecursiveFree(CProfiler::Node* node);

	void StartNode(NGin::String name);
	void EndNode();

	void CaptureFrame();
	
	static CProfiler* Instance();
};