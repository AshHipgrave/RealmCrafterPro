// Copyright (C) 2002-2006 Nikolaus Gebhardt
// This file is part of the "Irrlicht Engine".
// For conditions of distribution and use, see copyright notice in irrlicht.h

#include "CDefaultMeshFormatLoader.h"
#include "CAnimatedMeshB3d.h"
#include "CReadMemory.h"
#include <string.h>

namespace irr
{
namespace scene
{

//! Constructor
CDefaultMeshFormatLoader::CDefaultMeshFormatLoader(io::IFileSystem* fs, video::IVideoDriver* driver)
: FileSystem(fs), Driver(driver)
{
	if (FileSystem)
		FileSystem->grab();

	if (Driver)
		Driver->grab();
}



//! destructor
CDefaultMeshFormatLoader::~CDefaultMeshFormatLoader()
{
	if (FileSystem)
		FileSystem->drop();

	if (Driver)
		Driver->drop();
}



//! returns true if the file maybe is able to be loaded by this class
//! based on the file extension (e.g. ".bsp")
bool CDefaultMeshFormatLoader::isALoadableFileExtension(const c8* filename)
{
	return (strstr(filename, ".b3d") || strstr(filename, ".eb3d"));
}



//! creates/loads an animated mesh from the file.
//! \return Pointer to the created mesh. Returns 0 if loading failed.
//! If you no longer need the mesh, you should call IAnimatedMesh::drop().
//! See IUnknown::drop() for more information.
IAnimatedMesh* CDefaultMeshFormatLoader::createMesh(ISceneManager* smgr, irr::io::IReadFile* file, bool IsAnimated, bool skipReOpen, bool threaded)
{
	IAnimatedMesh* msh = 0;

	// This method loads a mesh if it cans.
	// Someday I will have to refactor this, and split the DefaultMeshFormatloader
	// into one loader for every format.

	bool success = false;

	// load blitz basic
	if (strstr(file->getFileName(), ".b3d"))
	{
		file->seek(0);

		msh = new CAnimatedMeshB3d(Driver, IsAnimated);
		success = ((CAnimatedMeshB3d*)msh)->loadFile(smgr, file, threaded);
		
		if (success)
			return msh;

		msh->drop();
	}

	// load encrypted blitz basic
	if (strstr(file->getFileName(), ".eb3d"))
	{
		irr::core::stringc fname = file->getFileName();
		//MessageBox(NULL,fname,"",MB_OK);
		//delete file;

		irr::io::IReadFile* TFile = file;

		// Removed the 'drop' here and used TFile instead so we can safely decrypt files
		if(!skipReOpen)
		{
			//file->drop();
			TFile = new irr::io::CReadMemory(fname.c_str());
		}

		TFile->seek(0);

		msh = new CAnimatedMeshB3d(Driver, IsAnimated);
		success = ((CAnimatedMeshB3d*)msh)->loadFile(smgr, TFile, threaded);

		// Close the file if we opened it here rather than the scenemanager
		if(TFile != file && TFile != 0)
			TFile->drop();

		if (success)
			return msh;

		msh->drop();
	}

	return 0;
}

} // end namespace scene
} // end namespace irr

