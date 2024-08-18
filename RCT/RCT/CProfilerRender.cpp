#include "CProfiler.h"

using namespace NGin;
using namespace NGin::GUI;

void Page_Click(IControl* Sender, EventArgs* E)
{
	int Page = (int)Sender->Tag;

	CProfiler::Instance()->StartIndex = Page * 50;
	CProfiler::Instance()->GenerateNodeGUI();
}

void Expand_Click(IControl* Sender, EventArgs* E)
{
	CProfiler::Node* node = (CProfiler::Node*)Sender->Tag;

	node->Expanded = true;
	CProfiler::Instance()->GenerateNodeGUI();
}

void Contract_Click(IControl* Sender, EventArgs* E)
{
	CProfiler::Node* node = (CProfiler::Node*)Sender->Tag;

	node->Expanded = false;
	CProfiler::Instance()->GenerateNodeGUI();
}

void CProfiler::GenerateNodeGUI()
{
	if(Manager == 0)
		return;

	if(ProfileBG != 0)
		Manager->Destroy(ProfileBG);

	for(int i = 0; i < PageButtons.Size(); ++i)
		Manager->Destroy(PageButtons[i]);
		
	ProfileBG = Manager->CreatePictureBox("pb", NGin::Math::Vector2(0.1f, 0.05f), NGin::Math::Vector2(0.5f, 0.9f));
	ProfileBG->SetImage(NGin::WString("BBDX\\Perf\\white.png"));
	ProfileBG->BackColor = NGin::Math::Color(0, 0, 0, 0.4f);

	uint Cnt = StartCount;
	
	int Pages = 0;
	while(Cnt > 50)
	{
		++Pages;
		Cnt -= 50;
	}

	if(Pages > 0)
	{
		float Px = 1.0f / Manager->GetResolution().X;
		float Py = 1.0f / Manager->GetResolution().Y;
		for(int i = 0; i < Pages; ++i)
		{
			IButton* B = Manager->CreateButton("bt", NGin::Math::Vector2(0.1f + (((float)i) * 65.0f * Px), 0.05f - (20.0f * Py)), Math::Vector2(60.0f * Px, 20.0f * Py));
			B->Text = String(i * 50) + " - " + String((i + 1) * 50);
			B->Tag = (void*)i;
			B->Click()->AddEvent(&Page_Click);

			PageButtons.Add(B);
		}
	}

	float Height = 0.06f;
	RecursiveFree(RootNode);
	Recursive(RootNode, 0, RootNode->Time, Height);

}

void CProfiler::RecursiveFree(CProfiler::Node* node)
{
	if(node->Label != 0)
		Manager->Destroy(node->Label);
	if(node->MSLabel != 0)
		Manager->Destroy(node->MSLabel);
	if(node->PLabel != 0)
		Manager->Destroy(node->PLabel);
	if(node->Button != 0)
		Manager->Destroy(node->Button);

	node->Label = node->MSLabel = node->PLabel = 0;
	node->Button = 0;

	foreachf(NIt, CProfiler::Node, node->Nodes)
	{
		RecursiveFree(*NIt);

		nextf(NIt, CProfiler::Node, node->Nodes);
	}
}

void CProfiler::Recursive(CProfiler::Node* node, int dec, uint totalTime, float &Height)
{
	//String Prefix = "";

	//for(int i = 0; i < dec * 4; ++i)
	//	Prefix.Append(" ");

	//if(Height > 0.8f + 0.1f)
	//	return;

	if(node->ID < StartIndex || node->ID > StartIndex + 50)
		return;

	float Percent = (100.0f * ((float)node->Time) / ((float)totalTime));

	float Px = 1.0f / Manager->GetResolution().X;

	String Out = node->Name;
	node->Label = Manager->CreateLabel("nodenode", Math::Vector2(0.12f + (10.0f * ((float)dec) * Px), Height), Math::Vector2(0, 0));
	node->Label->Text = Out;
	node->Label->ForeColor = Math::Color(255, 255, 255);
	node->Label->ForceScissoring = true;
	node->Label->ScissorWindow = Math::Vector2(0.3f, 1.0f);
	
	node->MSLabel = Manager->CreateLabel("nodenode", Math::Vector2(0.47f, Height), Math::Vector2(0, 0));
	node->MSLabel->Text = String(node->Time);
	node->MSLabel->ForeColor = Math::Color(255, 255, 255);

	node->PLabel = Manager->CreateLabel("nodenode", Math::Vector2(0.52f, Height), Math::Vector2(0, 0));
	node->PLabel->Text = String((int)Percent) + "%";
	node->PLabel->ForeColor = Math::Color(255, 255, 255);
	
	node->Button = Manager->CreateButton("nodenode", Math::Vector2(0.12f + (10.0f * ((float)dec) * Px) - (16.0f * Px), Height), Math::Vector2(16, 16) / Manager->GetResolution());
	node->Button->Text = node->Expanded ? "-" : "+";
	node->Button->Tag = node;
	node->Button->UseBorder = false;
	if(node->Nodes.Count() == 0)
		node->Button->Visible = false;

	if(node->Expanded)
		node->Button->Click()->AddEvent(&Contract_Click);
	else
		node->Button->Click()->AddEvent(&Expand_Click);

	Height += node->Label->Size.Y;


	if(!node->Expanded)
		return;


	foreachf(NIt, CProfiler::Node, node->Nodes)
	{
		//String Out = Prefix + (*NIt)->Name + String(": ") + String((*NIt)->Time) + ": " + String((100 * ((*NIt)->Time / totalTime))) + "\n";
		//OutputDebugString(Out.c_str());

		Recursive(*NIt, dec + 1, totalTime, Height);

		nextf(NIt, CProfiler::Node, node->Nodes);
	}
}

//void CProfiler::Recursive(CProfiler::Node* node, int dec, uint totalTime)
//{
//	String Prefix = "";
//
//	for(int i = 0; i < dec * 4; ++i)
//		Prefix.Append(" ");
//
//	float Percent = (100.0f * ((float)node->Time) / ((float)totalTime));
//
//	String Out = Prefix + node->Name + String(": ") + String(node->Time) + ": " + String((int)Percent) + "\n";
//	OutputDebugString(Out.c_str());
//
//	foreachf(NIt, CProfiler::Node, node->Nodes)
//	{
//		//String Out = Prefix + (*NIt)->Name + String(": ") + String((*NIt)->Time) + ": " + String((100 * ((*NIt)->Time / totalTime))) + "\n";
//		//OutputDebugString(Out.c_str());
//
//		Recursive(*NIt, dec + 1, totalTime);
//
//		nextf(NIt, CProfiler::Node, node->Nodes);
//	}
//}