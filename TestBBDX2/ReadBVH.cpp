#include "CAnimationSet.h"
#include "c:\Users\Jared Belkus\Desktop\Solstar Sources\Realm Crafter Internal\RC Pro - Internal\RCClient\Temp.h"

//using namespace NGin::IO;
using namespace NGin::Math;
using namespace std;

using namespace NGin;
//using namespace NGin::Rendering;

using namespace BlitzPlus;

namespace NGin
{
namespace Scene
{
	void CAnimationSet::RecursiveReadBVH(SBone* parentBone, FILE* file)
	{
		if(parentBone == 0)
			printf("RecurisveReadBVH(0)\n");
		else
			printf("RecurisveReadBVH(%s)\n", parentBone->Name.c_str());

		bool InEndSite = false;

		while(!BlitzPlus::Eof(file))
		{
			string NextData = ReadLine(file);
			NextData = std::trim(NextData);

			int Space = NextData.find(' ');
			if(Space == string::npos)
				Space = NextData.find('\t');
			if(Space != std::string::npos)
			{
				string Property = NextData.substr(0, Space);

				// Descend
				if(Property == string("ROOT") || Property == string("JOINT"))
				{
					SBone* NextBone = new SBone(NextData.substr(Space + 1), parentBone);
					BoneMap.push_back(NextBone);

					if(parentBone != 0)
						parentBone->Children.push_back(NextBone);
					
					if(Property == string("ROOT"))
						RootBone = NextBone;

					RecursiveReadBVH(NextBone, file);

					if(Property == string("ROOT"))
						return;
				}

				if(Property == string("End"))
				{
					InEndSite = true;
				}

				// Offset Data
				if(Property == string("OFFSET") && !InEndSite)
				{
					string OffsetVector = NextData.substr(Space + 1);

					Space = OffsetVector.find(' ');
					if(Space == string::npos)
						Space = OffsetVector.find('\t');

					if(Space != string::npos)
					{
						float X = std::toFloat(OffsetVector.substr(0, Space));
						OffsetVector = OffsetVector.substr(Space + 1);

						Space = OffsetVector.find(' ');
						if(Space == string::npos)
							Space = OffsetVector.find('\t');
						if(Space != string::npos)
						{
							float Y = std::toFloat(OffsetVector.substr(0, Space));
							float Z = std::toFloat(OffsetVector.substr(Space + 1));

							if(parentBone != 0)
								parentBone->ReadOffset.Reset(X, Y, Z);
						}
					}
				}

				// Channel Data
				if(Property == string("CHANNELS"))
				{
					string CommandData = NextData.substr(Space + 1);

					Space = CommandData.find(' ');
					if(Space == string::npos)
						Space = CommandData.find('\t');
					if(parentBone != 0 && Space != string::npos)
					{
						int Count = std::toInt(CommandData.substr(0, Space));
						CommandData = CommandData.substr(Space + 1);

						for(int i = 0; i < Count; ++i)
						{
							Space = CommandData.find(' ');
							if(Space == string::npos)
								Space = CommandData.find('\t');
							string ProcVal = CommandData;

							if(Space != string::npos)
							{
								ProcVal = CommandData.substr(0, Space);
							}

							ProcVal = std::stringToLower(ProcVal);

							if(ProcVal == string("xposition"))
								parentBone->ReadOrderBits[i] = 1;
							else if(ProcVal == string("yposition"))
								parentBone->ReadOrderBits[i] = 2;
							else if(ProcVal == string("zposition"))
								parentBone->ReadOrderBits[i] = 4;
							else if(ProcVal == string("xrotation"))
								parentBone->ReadOrderBits[i] = 8;
							else if(ProcVal == string("yrotation"))
								parentBone->ReadOrderBits[i] = 16;
							else if(ProcVal == string("zrotation"))
								parentBone->ReadOrderBits[i] = 32;
							else
								parentBone->ReadOrderBits[i] = 0;
						}

						parentBone->ChannelCount = Count;
					}
				}
			} // Read space

			// Closing Branch
			if(NextData == string("}"))
			{
				if(InEndSite)
				{
					InEndSite = false;
				}
				else
				{
					printf("return (%s)\n", parentBone > 0 ? parentBone->Name.c_str() : "");
					return;
				}
			}

		} // Eof
	}

	void CAnimationSet::SplitStringFloats( std::vector<float> &floats, std::string data )
	{
		while(true)
		{
			int Space = data.find(' ');
			string Next = "";
			if(Space == std::string::npos)
			{
				Next = data;
			}else
			{
				Next = data.substr(0, Space);
			}

			float Val = toFloat(trim(Next));
			floats.push_back(Val);

			if(Space == std::string::npos)
			{
				return;
			}else
			{
				data = data.substr(Space + 1);
			}
		}
	}

	bool CAnimationSet::ReadBVH(std::string path, std::string name)
	{
		FILE* File = BlitzPlus::ReadFile(path);
		if(File == 0)
		{
			DebugLog(string("ReadBVH could not open file '") + path + "'");
			return false;
		}

		std::string Hierarchy = ReadLine(File);
		if(Hierarchy != string("HIERARCHY"))
		{
			DebugLog(path + " has no header");
			return false;
		}

		RecursiveReadBVH(0, File);

		string Motion = ReadLine(File);
		if(Motion != string("MOTION"))
		{
			DebugLog(path + " has no motion header");
			return false;
		}

		string Frames = ReadLine(File);
		if(Frames.size() < 8 || Frames.substr(0, 7) != string("Frames:"))
		{
			DebugLog(path + " has no frame count header");
			return false;
		}

		int FrameCount = toInt(trim(Frames.substr(7)));

		string Frames2 = ReadLine(File);
		if(Frames2.size() < 8 || Frames2.substr(0, 11) != string("Frame Time:"))
		{
			DebugLog(path + " has no frame time header");
			return false;
		}

		float FrameTime = toFloat(trim(Frames2.substr(11)));


		for(int i = 0; (i < FrameCount) && !Eof(File); ++i)
		{
			int CurrentFloat = 0;
			string FloatLine = ReadLine(File);
			std::vector<float> Floats;
			SplitStringFloats(Floats, FloatLine);	

			for(int v = 0; v < BoneMap.size(); ++v)
			{
				SBone* Bone = BoneMap[v];
				
				Vector3 Translation;
				Vector3 Rotation;

				for(int c = 0; c < Bone->ChannelCount; ++c)
				{
					if(Bone->ReadOrderBits[c] == 0)
						continue;

					float F = Floats[CurrentFloat];
					++CurrentFloat;

					if(Bone->ReadOrderBits[c] == 1)
						Translation.X = F;
					if(Bone->ReadOrderBits[c] == 2)
						Translation.Y = F;
					if(Bone->ReadOrderBits[c] == 4)
						Translation.Z = F;
					if(Bone->ReadOrderBits[c] == 8)
						Rotation.X = F;
					if(Bone->ReadOrderBits[c] == 16)
						Rotation.Y = F;
					if(Bone->ReadOrderBits[c] == 32)
						Rotation.Z = F;
				}

				//Translation.Reset(0, 0, 0);
				//Rotation.Reset(0, 0, 0);

				SKey* Key = new SKey();

				Translation += BoneMap[v]->ReadOffset;
	
				D3DXMATRIX Trans, RotX, RotY, RotZ;
				D3DXMatrixTranslation(&Trans, Translation.X, Translation.Y, Translation.Z);
				D3DXMatrixRotationX(&RotX, Rotation.X * NGin::DEGTORAD);
				D3DXMatrixRotationY(&RotY, Rotation.Y * NGin::DEGTORAD);
				D3DXMatrixRotationZ(&RotZ, Rotation.Z * NGin::DEGTORAD);

				D3DXMATRIX T;
				D3DXMatrixMultiply(&T, &Trans, &RotX);
				D3DXMatrixMultiply(&T, &T, &RotY);
				D3DXMatrixMultiply(&T, &T, &RotZ);

				
				Matrix Rot;

				memcpy(Rot.M, &T, sizeof(float) * 16);
				//Rot.RotationRad(Rotation);
				//Rot.Translation(Translation);

				Key->LocalTransform = Rot;

				Bone->Keys.push_back(Key);
			}
		}

		RootBone->UpdateTransforms();

		//IEffect* Effect = Engine->LoadEffect("Media\\Shaders\\UntexturedLit.fx");

		//for(int i = 0; i < BoneMap.size(); ++i)
		//{
		//	printf("Bone: %s(%s) at (%s) Keys(%i)\n", BoneMap[i]->Name.c_str(), (BoneMap[i]->Parent != 0) ? BoneMap[i]->Parent->Name.c_str() : "Root!", BoneMap[i]->ReadOffset.ToString().c_str(), BoneMap[i]->Keys.size());

		//	IMesh* Mesh = Engine->LoadMesh("Media\\Meshes\\Testing\\sphere_blank.b3d");
		//	IMeshSceneNode* Node = Engine->CreateMeshSceneNode(Mesh);
		//	Node->SetEffect(Effect);
		//	Mesh->Drop();

		//	Vector3 Pos = BoneMap[i]->ReadOffset;

		//	BoneMap[i]->Keys[0]->GlobalTransform.TransformVector(&Pos);

		//	Node->SetPosition(Pos);
		//}


		return true;
	}
}
}