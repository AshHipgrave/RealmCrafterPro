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
#include "stdio.h"
//#include <string>
#include <list>
#include <vector>
#include <irrlicht.h>
using namespace std;

#define CHUNK_FUNCTIONS  		virtual int  GetLength(); \
								virtual void Export(FILE* file);

class cB3DChunk
{
	public:
		virtual ~cB3DChunk() {}

		char Tag[4];

		bool ExportToFile( char* FileName );

		virtual int  GetLength();
		virtual void Export(FILE* file);
};

class cB3DChunk_TEXS : public cB3DChunk
{
	public:
		cB3DChunk_TEXS();
		string FileName;
		int		flags, blend;
		float	x, y, 
				x_scale, y_scale, 
				rotation;

		int Index;

		CHUNK_FUNCTIONS
};

class cB3DChunk_BRUS : public cB3DChunk
{
	public:
		cB3DChunk_BRUS();

		string MaterialName;
		float red,green,blue,alpha;
		float shininess;
		int blend,fx;
		list<cB3DChunk_TEXS*> lpTextures;

		int Index;

		CHUNK_FUNCTIONS
};

struct cB3DVertex
{
	cB3DVertex() : x(0), y(0), z(0), nx(0), ny(0), nz(0), red(1), green(1), blue(1), alpha(1) {}
	float x,y,z;
	float nx,ny,nz;
	float red,green,blue,alpha;
	vector<float> aTexcoordsU;
	vector<float> aTexcoordsV;	
};

class cB3DChunk_VRTS : public cB3DChunk
{
	public:
		cB3DChunk_VRTS();

		vector<cB3DVertex> aVertices;

		CHUNK_FUNCTIONS
};

	struct cB3DTriangle
	{
		int aIndices[3];
	};

class cB3DChunk_TRIS : public cB3DChunk
{
	public:
		cB3DChunk_TRIS();

		cB3DChunk_BRUS* pMaterial;
		vector<cB3DTriangle> aTriangles;

		CHUNK_FUNCTIONS
};


class cB3DChunk_MESH : public cB3DChunk
{
	public:
		cB3DChunk_MESH();
		~cB3DChunk_MESH();

		cB3DChunk_BRUS* pMaterial;
		cB3DChunk_VRTS Vertices;
		vector<cB3DChunk_TRIS> aTrianglesGroup;

		CHUNK_FUNCTIONS
};

	struct cVertex_Weight { unsigned int iVertex; float weight; };

class cB3DChunk_BONE : public cB3DChunk
{
	public:
		cB3DChunk_BONE();

		vector<cVertex_Weight> aSkinInfo;

		CHUNK_FUNCTIONS
};

	struct cB3DKeyFrame { 
		int frame;
		float position[3];
		float scale[3];
		float rotation[4];
	};

class cB3DChunk_KEYS : public cB3DChunk
{
	public:
		cB3DChunk_KEYS();

		int Flags;						// BIT_1 = has position, BIT_2 = has scale, BIT_3 = has rotation
		vector<cB3DKeyFrame> aKeyFrames;

		CHUNK_FUNCTIONS
};

class cB3DChunk_ANIM : public cB3DChunk
{
	public:
		cB3DChunk_ANIM();

		int nFrames;	// Number of frames
		float fps;

		CHUNK_FUNCTIONS
};

class cB3DChunk_NODE : public cB3DChunk
{
	public:
		cB3DChunk_NODE();
		~cB3DChunk_NODE();

		string Name;
		float position[3];
		float scale[3];
		float rotation[4]; // quaternion: w,x,y,z

		cB3DChunk* pTypeChunk;

		list<cB3DChunk_KEYS*> lpKeys;
		list<cB3DChunk_NODE*> lpNodes;
		cB3DChunk_ANIM* pAnim;


		CHUNK_FUNCTIONS
};


class cB3DChunk_BB3D : public cB3DChunk
{
	public:
		cB3DChunk_BB3D();
		virtual ~cB3DChunk_BB3D();

		void InsertTexture(cB3DChunk_TEXS* pTex);
		void InsertMaterial(cB3DChunk_BRUS* pMat);
			
		list<cB3DChunk_TEXS*> lpTextures;
		list<cB3DChunk_BRUS*> lpMaterials;
		list<cB3DChunk_NODE*> lpNodes;

		CHUNK_FUNCTIONS
};