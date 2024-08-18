#include "CProfiler.h"
#include <time.h>

using namespace NGin;

CProfiler* CProfiler::Instance()
{
	static CProfiler StaticProfile;

	return &StaticProfile;
}


CProfiler::CProfiler()
{
	RootNode = new Node(0, Manager);
	RootNode->Name = "Frame";
	RootNode->Start = clock();
	CurrentNode = RootNode;
	Capturing = false;
	Manager = 0;
	ProfileBG = 0;
	StartIndex = 0;
}

CProfiler::~CProfiler()
{
	
}



void CProfiler::Update()
{
	if(StartCapture)
	{
		StartCapture = false;
		Capturing = true;
		RootNode->Start = clock();
		return;
	}

	if(Capturing)
	{
		Capturing = false;
		RootNode->Time = clock() - RootNode->Start;
		GenerateNodeGUI();
		//Recursive(RootNode, 0, RootNode->Time);
	}
}

void CProfiler::CaptureFrame()
{
	StartCapture = true;

	delete RootNode;
	RootNode = new Node(0, Manager);
	CurrentNode = RootNode;
	StartCount = 1;
	StartIndex = 0;
}

void CProfiler::StartNode(NGin::String name)
{
	if(!Capturing)
		return;
	CurrentNode = new Node(CurrentNode, Manager);
	CurrentNode->Name = name;
	CurrentNode->Start = clock();
	CurrentNode->ID = StartCount;
	++StartCount;
}

void CProfiler::EndNode()
{
	if(!Capturing)
		return;
	CurrentNode->Time = clock() - CurrentNode->Start;
	CurrentNode = CurrentNode->Parent;
}

CProfiler::Node::Node(CProfiler::Node* parent, NGin::GUI::IGUIManager* manager)
{
	this->Parent = parent;
	this->Name = "Unknown";
	this->Time = 0;
	this->Manager = manager;
	this->ID = 0;

	Label = MSLabel = PLabel = 0;
	Button = 0;
	Expanded = false;

	if(parent != 0)
		parent->Nodes.Add(this);
}

CProfiler::Node::~Node()
{
	if(Label != 0)
		Manager->Destroy(Label);
	if(MSLabel != 0)
		Manager->Destroy(MSLabel);
	if(PLabel != 0)
		Manager->Destroy(PLabel);
	if(Button != 0)
		Manager->Destroy(Button);

	foreachf(NIt, CProfiler::Node, this->Nodes)
	{
		delete *NIt;

		nextf(NIt, CProfiler::Node, this->Nodes);
	}
}




