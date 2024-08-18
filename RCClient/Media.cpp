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

#include "Media.h"

int LoadedTextures[65535];
float LoadedMeshScales[65535];
float LoadedMeshX[65535];
float LoadedMeshY[65535];
float LoadedMeshZ[65535];
int LoadedMeshShaders[65535];
int LoadedSounds[65535];
int TextureFlags[65535];
Dictionary<string, IShaderParameter*> LoadedParameters[65535];

FILE* LockedMeshes = 0, *LockedTextures = 0, *LockedSounds = 0, *LockedMusic = 0;

ClassDef(MeshMinMaxVertices, MeshMinMaxVerticesList, MeshMinMaxVerticesDelete);

// Convert MeshDatabase To a New Format
void ConvertMeshDatabase()
{
	FILE *pFileOld = OpenFile("Data\\Game Data\\Meshes.dat");
	if ( ReadByte(pFileOld) == 255 ) {	fclose(pFileOld); return; }

	FILE *pFileNew = OpenFile("Data\\Game Data\\Meshes_.dat");
	WriteByte(pFileNew, 255); // Flag for New Format
	for (int i(0); i < 65535; ++i) WriteInt( pFileNew, 0 );

	long Length;
	byte Flags;
	float Scale, X, Y, Z;
	ushort ShaderID;
	string Name;
	int DataAddress;
	for (int ID (0); ID < 65535; ++ID)
	{
		// Find data address in file index
		SeekFile(pFileOld, ID * 4);
		DataAddress = ReadInt(pFileOld);
		if (DataAddress == 0) continue;

		// Move to DataAddress
		SeekFile(pFileOld, DataAddress);
		// Read in mesh data
		Flags = ReadByte(pFileOld);
		Scale = ReadFloat(pFileOld);
		X = ReadFloat(pFileOld);
		Y = ReadFloat(pFileOld);
		Z = ReadFloat(pFileOld);
		ShaderID = ReadShort(pFileOld);
		Name = ReadString(pFileOld);

		// Write into new MeshDatabase
		fseek(pFileNew, 0, SEEK_END);
		Length = ftell(pFileNew);
		SeekFile(pFileNew, 1+ID*4);
		WriteInt(pFileNew, Length);
		SeekFile(pFileNew, Length);

		WriteByte(pFileNew, Flags);		//	Flags
		WriteFloat(pFileNew, Scale);	//  Scale
		WriteFloat(pFileNew, X);		//  X
		WriteFloat(pFileNew, Y);		//  Y
		WriteFloat(pFileNew, Z);		//  Z
		WriteShort(pFileNew, ShaderID);	//  ShaderID  + 19

		WriteFloat(pFileNew, 0);        //  DistToMedium_LOD
		WriteShort(pFileNew, 65535);	//  IdMeshLOD_Medium
		WriteFloat(pFileNew, 0);        //  DistToLow_LOD
		WriteShort(pFileNew, 65535);	//  IdMeshLOD_Low
		WriteFloat(pFileNew, 0);        //  DistToHide_LOD

		WriteString(pFileNew, Name);	// MeshName
	}

	fclose(pFileOld); fclose(pFileNew); 

	DeleteFile("Data\\Game Data\\Meshes.dat");
	MoveFile("Data\\Game Data\\Meshes_.dat", "Data\\Game Data\\Meshes.dat");
}
// Locks the meshes database (keeps the file open for faster batched Get...() calls)
FILE* LockMeshes()
{
	LockedMeshes = OpenFile("Data\\Game Data\\Meshes.dat");
	return LockedMeshes;
}

// Unlocks the meshes database (closes the file again)
bool UnlockMeshes()
{
	if(LockedMeshes != 0)
	{
		CloseFile(LockedMeshes);
		LockedMeshes = 0;
		return true;
	}
	return false;
}

// Locks the textures database (keeps the file open for faster batched Get...() calls)
FILE* LockTextures()
{
	LockedTextures = OpenFile("Data\\Game Data\\Textures.dat");
	return LockedTextures;
}

// Unlocks the textures database (closes the file again)
bool UnlockTextures()
{
	if(LockedTextures != 0)
	{
		CloseFile(LockedTextures);
		LockedTextures = 0;
		return true;
	}
	return false;
}


// Locks the sounds database (keeps the file open for faster batched Get...() calls)
FILE* LockSounds(){
	LockedSounds = OpenFile("Data\\Game Data\\Sounds.dat");
	return LockedSounds;
}

// Unlocks the sounds database (closes the file again)
bool UnlockSounds()
{
	if(LockedSounds != 0)
	{
		CloseFile(LockedSounds);
		LockedSounds = 0;
		return true;
	}
	return false;
}


// Locks the music database (keeps the file open for faster batched Get...() calls)
FILE* LockMusic(){
	LockedMusic = OpenFile("Data\\Game Data\\Music.dat");
	return LockedMusic;
}

// Unlocks the sounds database (closes the file again)
bool UnlockMusic()
{
	if(LockedMusic != 0)
	{
		CloseFile(LockedMusic);
		LockedMusic = 0;
		return true;
	}
	return false;
}


// Gets the name and animation byte for a given mesh
string GetMeshName(int ID)
{
	FILE* F;

	// Open index file
	if(LockedMeshes == 0)
	{
		F = OpenFile("Data\\Game Data\\Meshes.dat");
		if(F == 0)
			RuntimeError("Could not open Meshes.dat!");
	}else
		F = LockedMeshes;

	// Find data address in file index
	SeekFile(F, 1+ID * 4);
	int DataAddress = ReadInt(F);
	if(DataAddress == 0)
	{
		if(LockedMeshes == 0)
			CloseFile(F);
		return string("");
	}

	// Read in mesh data
	SeekFile(F, DataAddress);
	char IsAnim = ReadByte(F);

	ReadFloat(F);	//	Scale
	ReadFloat(F);	//	X
	ReadFloat(F);	//	Y
	ReadFloat(F);	//	Z
	ReadShort(F);	//	ShaderID
	ReadFloat(F);	//	distHigh
	ReadShort(F);	//	MediumID
	ReadFloat(F);	//	distMedium
	ReadShort(F);	//	LowID
	ReadFloat(F);	//	distMedium

	std::string Name = ReadString(F);

	if(LockedMeshes == 0) CloseFile(F);

	std::string NameLower = Name.substr(Name.length() - 1, 1);
	std::transform(NameLower.begin(), NameLower.end(), NameLower.begin(), ::tolower);

	if(NameLower.compare("x") == 0)
		Name = Name.substr(0, Name.length() - 1) + "eb3d";

	char cIsAnim[2] = {IsAnim, 0};

	Name.append(cIsAnim);
	return Name;
}

// Gets the name and flags for a given texture
std::string GetTextureName(int ID)
{
	FILE* F = 0;

	// Open Index file
	if(LockedTextures == 0)
	{
		F = OpenFile("Data\\Game Data\\Textures.dat");
		if(F == 0)
			RuntimeError("Could not open Textures.dat!");
	}else
		F = LockedTextures;

	// File the data address in file index
	SeekFile(F, ID * 4);
	int DataAddress = ReadInt(F);
	if(DataAddress == 0)
	{
		if(LockedTextures == 0)
			CloseFile(F);
		return string("");
	}
	
	// Read in texture data
	SeekFile(F, DataAddress);
	int Flags = ReadShort(F);
	string Name = ReadString(F);

	// Append
	Name.append(Chr(Flags));

	if(LockedTextures == 0)
		CloseFile(F);

	return Name;
}

// Gets the name without flags for a given texture
std::string GetTextureNameNoFlag(int ID)
{
	if(ID == 65535)
		return "";

	FILE* F = 0;

	// Open Index file
	if(LockedTextures == 0)
	{
		F = OpenFile("Data\\Game Data\\Textures.dat");
		if(F == 0)
			RuntimeError("Could not open Textures.dat!");
	}else
		F = LockedTextures;

	// File the data address in file index
	SeekFile(F, ID * 4);
	int DataAddress = ReadInt(F);
	if(DataAddress == 0)
	{
		if(LockedTextures == 0)
			CloseFile(F);
		return string("");
	}
	
	// Read in texture data
	SeekFile(F, DataAddress);
	int Flags = ReadShort(F);
	string Name = ReadString(F);

	if(LockedTextures == 0)
		CloseFile(F);

	return Name;
}

// Gets the name and 3D byte for a given sound
std::string GetSoundName(int ID)
{
	FILE* F = 0;

	// Open index file
	if(LockedSounds == 0)
	{
		F = OpenFile("Data\\Game Data\\Sounds.dat");
		if(F == 0)
			RuntimeError("Could not open Sounds.dat!");
	}else
		F = LockedSounds;

	// Find data address in file index
	SeekFile(F, ID * 4);
	int DataAddress = ReadInt(F);
	if(DataAddress == 0)
	{
		if(LockedSounds == 0)
			CloseFile(F);
		return string("");
	}

	// Read in sound data
	SeekFile(F, DataAddress);
	int Is3D = (int)ReadByte(F);
	std::string Name = ReadString(F);

	Name.append(Chr(Is3D));

	if(LockedSounds == 0)
		CloseFile(F);

	return Name;
}

// Gets the name of a given piece of music
std::string GetMusicName(int ID)
{
	FILE* F = 0;

	// Open index file
	if(LockedMusic == 0)
	{
		F = OpenFile("Data\\Game Data\\Music.dat");
		if(F == 0)
			RuntimeError("Could not open Music.dat!");
	}else
		F = LockedMusic;

	// Find data address in file index
	SeekFile(F, ID * 4);
	int DataAddress = ReadInt(F);
	if(DataAddress == 0)
	{
		if(LockedMusic == 0)
			CloseFile(F);
		return string("");
	}

	// Read in sound data
	SeekFile(F, DataAddress);
	std::string Name = ReadString(F);

	if(LockedMusic == 0)
		CloseFile(F);

	return Name;
}

// Changes the scale for a mesh
bool SetMeshScale(int ID, float Scale)
{
	LoadedMeshScales[ID] = Scale;
	FILE* F = 0;

	// Open index file
	if(LockedMeshes == 0)
	{
		F = OpenFile("Data\\Game Data\\Meshes.dat");
		if(F == 0)
			RuntimeError("Could not open Meshes.dat!");
	}else
		F = LockedMeshes;

	// Find data address in file index
	SeekFile(F, 1+ID * 4);
	int DataAddress = ReadInt(F);
	if(DataAddress == 0)
	{
		if(LockedMeshes == 0)
			CloseFile(F);
		return false;
	}

	// Write new scale float
	SeekFile(F, DataAddress + 1);
	WriteFloat(F, Scale);

	if(LockedMeshes == 0)
		CloseFile(F);

	return true;
}

// Changes the offset for a mesh
bool SetMeshOffset(int ID, float X, float Y, float Z)
{
	LoadedMeshX[ID] = X;
	LoadedMeshY[ID] = Y;
	LoadedMeshZ[ID] = Z;
	FILE* F = 0;

	// Open index file
	if(LockedMeshes == 0)
	{
		F = OpenFile("Data\\Game Data\\Meshes.dat");
		if(F == 0)
			RuntimeError("Could not open Meshes.dat!");
	}else
		F = LockedMeshes;

	// Find data address in file index
	SeekFile(F, 1+ID * 4);
	int DataAddress = ReadInt(F);
	if(DataAddress == 0)
	{
		if(LockedMeshes == 0)
			CloseFile(F);
		return false;
	}

	// Write new scale float
	SeekFile(F, DataAddress + 5);
	WriteFloat(F, X);
	WriteFloat(F, Y);
	WriteFloat(F, Z);

	if(LockedMeshes == 0)
		CloseFile(F);

	return true;
}

// Changes the shader for a mesh
bool SetMeshShader(int ID, int Shader)
{
	LoadedMeshShaders[ID] = Shader;
	FILE* F = 0;

	// Open index file
	if(LockedMeshes == 0)
	{
		F = OpenFile("Data\\Game Data\\Meshes.dat");
		if(F == 0)
			RuntimeError("Could not open Meshes.dat!");
	}else
		F = LockedMeshes;

	// Find data address in file index
	SeekFile(F, 1+ID * 4);
	int DataAddress = ReadInt(F);
	if(DataAddress == 0)
	{
		if(LockedMeshes == 0)
			CloseFile(F);
		return false;
	}

	// Write new scale float
	SeekFile(F, DataAddress + 17);
	WriteShort(F, Shader);

	if(LockedMeshes == 0)
		CloseFile(F);

	return true;
}

// Gets the handle for a given mesh (this will load it if it isn't present)
int GetMesh(int ID)
{
	FILE* F = 0;

	// Read in filename and other data from index file
	if(LockedMeshes == 0)
	{
		F = ReadFile("Data\\Game Data\\Meshes.dat");
		if(F == 0)
			RuntimeError("Could not open Meshes.dat");
	}else
		F = LockedMeshes;

	// Find data address in file index
	SeekFile(F, 1+ID * 4);
	int DataAddress = ReadInt(F);
	if(DataAddress == 0)
	{
		if(LockedMeshes == 0)
			CloseFile(F);
		return 0;
	}

	//printf("Load ID: %i; Addr: %i\n", ID, DataAddress);

	// Read in mesh data
	SeekFile(F, DataAddress);
	char Flags = ReadByte(F);
	LoadedMeshScales[ID] = ReadFloat(F);
	LoadedMeshX[ID] = ReadFloat(F);
	LoadedMeshY[ID] = ReadFloat(F);
	LoadedMeshZ[ID] = ReadFloat(F);
	LoadedMeshShaders[ID] = ReadShort(F);
	//////////////////////////////////////////////////////////////////////////
	// LOD
	//////////////////////////////////////////////////////////////////////////
		ReadFloat(F);	//	distHigh
		ReadShort(F);	//	MediumID
		ReadFloat(F);	//	distMedium
		ReadShort(F);	//	LowID
		ReadFloat(F);	//	distMedium
	//////////////////////////////////////////////////////////////////////////
	char IsAnim = (Flags & 1);
	bool CalcTangents = (Flags & 2) > 0;
	bool Reflect = (Flags & 4) > 0;
	bool Refract = (Flags & 8) > 0;

	//char OO[1024];
	//sprintf(OO,  "Anim: %i; Tan: %i; Reflect: %i; Refract: %i\n", IsAnim, CalcTangents ? 1 : 0, Reflect ? 1 : 0, Refract ? 1 : 0);
	//OutputDebugString(OO);

	//for(int i = 0; i < 5; ++i)
	//	printf("%x ", (int)ReadByte(F));


	
	std::string Name = ReadString(F);

	std::string NameLower = Name.substr(Name.length() - 1, 1);
	std::transform(NameLower.begin(), NameLower.end(), NameLower.begin(), ::tolower);

	if(NameLower.compare("x") == 0)
		Name = Name.substr(0, Name.length() - 1) + std::string("eb3d");


	if(LockedMeshes == 0)
		CloseFile(F);

	std::string MeshFile = "Data\\Meshes\\";
	MeshFile.append(Name);

	int Mesh = LoadMesh(MeshFile, 0, (int)IsAnim);
	if(Mesh == 0)
	{
		string Failure = string("Failed to load mesh: ") + MeshFile + "!";
		DebugLog(Failure);
		return 0;
	}

	ScaleEntity(Mesh, LoadedMeshScales[ID], LoadedMeshScales[ID], LoadedMeshScales[ID]);
	//if(LoadedMeshShaders[ID] < 65535)
	//	EntityProfile(Mesh, GetProfile(LoadedMeshShaders[ID]));
	//else
	//	EntityShader(Mesh, Shader_LitObject1);


    if (LoadedMeshShaders[ID] < 65535)
    {
        uint Pr = GetProfile(LoadedMeshShaders[ID]);

        if (Pr != 0)
            EntityProfile(Mesh, Pr);
        else
        {
            EntityShader(Mesh, Shader_LitObject1);

            if (IsAnim == 0 && DefaultStatic != 0)
                EntityProfile(Mesh, DefaultStatic);

            if (IsAnim == 1 && DefaultAnimated != 0)
                EntityProfile(Mesh, DefaultAnimated);
        }
    }
    else
    {
        EntityShader(Mesh, Shader_LitObject1);

        if (IsAnim == 0 && DefaultStatic != 0)
            EntityProfile(Mesh, DefaultStatic);

        if (IsAnim == 1 && DefaultAnimated != 0)
            EntityProfile(Mesh, DefaultAnimated);
        
    }

	if(CalcTangents && IsAnim == 0)
		CalculateB3DTangents(Mesh);

	// Parameters
	foreachd(PIt, string, IShaderParameter*, LoadedParameters[ID])
	{
		string Key = (*PIt)->Key;
		IShaderParameter* Value = (*PIt)->Value;

		if(Value->GetType() == SPVector1::TypeOf())
			EntityConstantFloat(Mesh, Key.c_str(), ((SPVector1*)Value)->X);
		if(Value->GetType() == SPVector2::TypeOf())
			EntityConstantFloat2(Mesh, Key.c_str(), ((SPVector2*)Value)->X, ((SPVector2*)Value)->Y);
		if(Value->GetType() == SPVector3::TypeOf())
			EntityConstantFloat3(Mesh, Key.c_str(), ((SPVector3*)Value)->X, ((SPVector3*)Value)->Y, ((SPVector3*)Value)->Z);
		if(Value->GetType() == SPVector4::TypeOf())
			EntityConstantFloat4(Mesh, Key.c_str(), ((SPVector4*)Value)->X, ((SPVector4*)Value)->Y, ((SPVector4*)Value)->Z, ((SPVector4*)Value)->W);

		nextd(PIt, string, IShaderParameter*, LoadedParameters[ID]);
	}

	return Mesh;
}

// Gets the handle for a given texture (this will load it if it isn't present)
int GetTexture(int ID, bool Copy)
{
	if(ID == 65535)
		return LoadTexture("BBDX2GETDEFAULTTEXTURE.int");

	if(LoadedTextures[ID] == 0)
	{
		FILE* F = 0;

		// Open Index file
		if(LockedTextures == 0)
		{
			F = OpenFile("Data\\Game Data\\Textures.dat");
			if(F == 0)
				RuntimeError("Could not open Textures.dat!");
		}else
			F = LockedTextures;

			// File the data address in file index
		SeekFile(F, ID * 4);
		int DataAddress = ReadInt(F);
		if(DataAddress == 0)
		{
			if(LockedTextures == 0)
				CloseFile(F);
			return 0;
		}

		// Read in texture data
		SeekFile(F, DataAddress);
		int Flags = ReadShort(F);
		string Name = ReadString(F);

		if(LockedTextures == 0)
			CloseFile(F);

		Name = string("Data\\Textures\\") + Name;

		LoadedTextures[ID] = LoadTexture(Name, Flags);
		TextureFlags[ID] = Flags;
	}

	if(Copy)
		GrabTexture(LoadedTextures[ID]);
	
	return LoadedTextures[ID];
}

// Gets the handle for a given sound (this will load it if it isn't present)
int GetSound(int ID)
{

	if(ID < 0 || ID > 65534)
		return 0;

	if(LoadedSounds[ID] == 0)
	{
		FILE* F = 0;

		// Read in filename and other data from index file
		if(LockedSounds == 0)
		{
			F = OpenFile("Data\\Game Data\\Sounds.dat");
			if(F == 0)
				RuntimeError("Could not open Sounds.dat!");
		}else
			F = LockedSounds;

		// Find data address in file index
		SeekFile(F, ID * 4);
		int DataAddress = ReadInt(F);
		if(DataAddress == 0)
		{
			if(LockedSounds == 0)
				CloseFile(F);
			return 0;
		}
		
		// Read in sound data
		SeekFile(F, DataAddress);
		char Is3D = ReadByte(F);
		string Name = ReadString(F);

		if(LockedSounds == 0)
			CloseFile(F);

		Name = string("Data\\Sounds\\") + Name;

		if(Is3D > 0)
			LoadedSounds[ID] = Load3DSound(Name);
		else
			LoadedSounds[ID] = LoadSound(Name);

		if(LoadedSounds[ID] == 0)
			DebugLog(string("Failed to load sound: ") + Name);
		else
			DebugLog(string("Loaded sound: ") + Name);
	}

	return LoadedSounds[ID];
}

// Unloads a texture
void UnloadTexture(int ID)
{
	if(LoadedTextures[ID] != 0)
		FreeTexture(LoadedTextures[ID]);
	LoadedTextures[ID] = 0;
}

// Unloads a sound
void UnloadSound(int ID)
{
	if(LoadedSounds[ID] != 0)
		FreeSound(LoadedSounds[ID]);
	LoadedSounds[ID] = 0;
}

// Scales a mesh entity to be a certain size without altering the mesh (works on animated meshes)
void SizeEntity(int EN, float Width, float Height, float Depth, bool Uniform)
{
	// Find mesh edges
	MeshMinMaxVertices* Result = Mesh_MinMaxVertices(EN);
	float MWidth  = Result->MaxX - Result->MinX;
	float MHeight = Result->MaxY - Result->MinY;
	float MDepth  = Result->MaxZ - Result->MinZ;
	MeshMinMaxVertices::Delete(Result);
	MeshMinMaxVertices::Clean();

	// Scale
	if(Uniform == false)
		ScaleEntity(EN, Width / MWidth, Height / MHeight, Depth / MDepth);
	else
	{
		float XScale = Width / MWidth;
		float YScale = Height / MHeight;
		float ZScale = Depth / MDepth;

		if(YScale < XScale)
			XScale = YScale;
		if(ZScale < XScale)
			XScale = ZScale;

		ScaleEntity(EN, XScale, XScale, XScale);
	}
}

// Retrieves the min/max vertex positions of a mesh or heirarchy of meshes (RECURSIVE)
MeshMinMaxVertices* Mesh_MinMaxVertices(int EN)
{
	MeshMinMaxVertices* Result = new MeshMinMaxVertices();
	string Class = EntityClass(EN);
	std::transform(Class.begin(), Class.end(), Class.begin(), ::toupper);
	
	int SCnt = CountSurfaces(EN) + 1;

	if(Class.compare("MESH") == 0)
		for(int i = 1; i < SCnt; ++i)
		{
			int Surf = GetSurface(EN, i);
			int VCnt = CountVertices(Surf);
			for(int j = 0; j < VCnt; ++j)
			{			
				float X = VertexX(Surf, j);
				float Y = VertexY(Surf, j);
				float Z = VertexZ(Surf, j);
				if(X < Result->MinX)
					Result->MinX = X;
				else if(X > Result->MaxX)
					Result->MaxX = X;
				if(Y < Result->MinY)
					Result->MinY = Y;
				else if(Y > Result->MaxY)
					Result->MaxY = Y;
				if(Z < Result->MinZ)
					Result->MinZ = Z;
				else if(Z > Result->MaxZ)
					Result->MaxZ = Z;
			}
		}

	int CCnt = CountChildren(EN);
	for(int i = 1; i < CCnt + 1; ++i)
	{
		int Child = GetChild(EN, i);
		string ClassUpper = EntityClass(Child);
		std::transform(ClassUpper.begin(), ClassUpper.end(), ClassUpper.begin(), ::toupper);

		if(ClassUpper.compare("MESH") == 0)
		{
			MeshMinMaxVertices* ChildResult = Mesh_MinMaxVertices(Child);
			if(ChildResult->MinX < Result->MinX) Result->MinX = ChildResult->MinX;
			if(ChildResult->MinY < Result->MinY) Result->MinY = ChildResult->MinY;
			if(ChildResult->MinZ < Result->MinZ) Result->MinZ = ChildResult->MinZ;
			if(ChildResult->MaxX < Result->MaxX) Result->MaxX = ChildResult->MaxX;
			if(ChildResult->MaxY < Result->MaxY) Result->MaxY = ChildResult->MaxY;
			if(ChildResult->MaxZ < Result->MaxZ) Result->MaxZ = ChildResult->MaxZ;
			MeshMinMaxVertices::Delete(ChildResult);
			MeshMinMaxVertices::Clean();
		}
	}

	return Result;
}

// Same thing but for transformed meshes
MeshMinMaxVertices* MeshMinMaxVerticesTransformed(int EN, float Pitch, float Yaw, float Roll, float ScaleX, float ScaleY, float ScaleZ)
{
	MeshMinMaxVertices* Result = new MeshMinMaxVertices();
	string Class = EntityClass(EN);
	std::transform(Class.begin(), Class.end(), Class.begin(), ::toupper);

	int SCnt = CountSurfaces(EN) + 1;

	if(Class.compare("MESH") == 0)
		for(int i = 1; i < SCnt; ++i)
		{
			int Surf = GetSurface(EN, i);
			int VCnt = CountVertices(Surf);
			for(int j = 0; j < VCnt; ++j)
			{			
				float X = VertexX(Surf, j);
				float Y = VertexY(Surf, j);
				float Z = VertexZ(Surf, j);
				int P = CreatePivot();
				RotateEntity(P, Pitch, Yaw, Roll);
				TFormPoint(X, Y, Z, P, 0);
				X = TFormedX();
				Y = TFormedY();
				Z = TFormedZ();
				FreeEntity(P);
				if(X < Result->MinX)
					Result->MinX = X;
				else if(X > Result->MaxX)
					Result->MaxX = X;
				if(Y < Result->MinY)
					Result->MinY = Y;
				else if(Y > Result->MaxY)
					Result->MaxY = Y;
				if(Z < Result->MinZ)
					Result->MinZ = Z;
				else if(Z > Result->MaxZ)
					Result->MaxZ = Z;
			}
		}

	int CCnt = CountChildren(EN);
	for(int i = 1; i < CCnt + 1; ++i)
	{
		int Child = GetChild(EN, i);
		string ClassUpper = EntityClass(Child);
		std::transform(ClassUpper.begin(), ClassUpper.end(), ClassUpper.begin(), ::toupper);

		if(ClassUpper.compare("MESH") == 0)
		{
			MeshMinMaxVertices* ChildResult = MeshMinMaxVerticesTransformed(Child, Pitch, Yaw, Roll, ScaleX, ScaleY, ScaleZ);
			if(ChildResult->MinX < Result->MinX) Result->MinX = ChildResult->MinX;
			if(ChildResult->MinY < Result->MinY) Result->MinY = ChildResult->MinY;
			if(ChildResult->MinZ < Result->MinZ) Result->MinZ = ChildResult->MinZ;
			if(ChildResult->MaxX < Result->MaxX) Result->MaxX = ChildResult->MaxX;
			if(ChildResult->MaxY < Result->MaxY) Result->MaxY = ChildResult->MaxY;
			if(ChildResult->MaxZ < Result->MaxZ) Result->MaxZ = ChildResult->MaxZ;
			MeshMinMaxVertices::Delete(ChildResult);
			MeshMinMaxVertices::Clean();
		}
	}

	return Result;
}


// Load entity parameters
void LoadEntityParameters()
{
	// Read xml file
	XMLReader* X = ReadXMLFile("Data\\Game Data\\ShaderParameters.xml");
	if(X == 0)
		RuntimeError("Could not open Data\\Game Data\\ShaderParameters.xml");

	int EntityID = -1;

	// Get values
	while(X->Read())
	{
		string NameLower = X->GetNodeName();
		std::transform(NameLower.begin(), NameLower.end(), NameLower.begin(), ::tolower);

		// If its a <entity> element
		if(X->GetNodeType() == XNT_Element && NameLower.compare("entity") == 0)
		{
			EntityID = X->GetAttributeInt("id");
		}
		
		// It's </entity>
		if(X->GetNodeType() == XNT_Element_End && NameLower.compare("entity") == 0)
		{
			EntityID = -1;
		}

		// Its <parameter>
		if(X->GetNodeType() == XNT_Element && NameLower.compare("parameter") == 0 && EntityID > -1 && EntityID < 65535)
		{
			string Name = X->GetAttributeString("name");
			string Type = X->GetAttributeString("type");
			std::transform(Type.begin(), Type.end(), Type.begin(), ::tolower);
			string Value = X->GetAttributeString("value");

			IShaderParameter* P = 0;

			if(Type.compare("vector1") == 0)
				P = new SPVector1();
			else if(Type.compare("vector2") == 0)
				P = new SPVector2();
			else if(Type.compare("vector3") == 0)
				P = new SPVector3();
			else if(Type.compare("vector4") == 0)
				P = new SPVector4();
			
			if(P == 0)
				continue;

			P->FromString(Value);
			LoadedParameters[EntityID].Add(Name, P);
		}
	}

	// Close file
	delete X;
}