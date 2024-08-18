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

// Include sound library
#include <irrKlang.h>
#include <time.h>

// Set namespace
using namespace irrklang;

// Global sound manager
ISoundEngine* SoundEngine;

// Structure of a sound
struct BBDXSound
{
	ISoundSource* Snd;
	bool IsLooped;
	irr::f32 Volume;
	irr::core::stringc filename;
};

// Structure of a playing channel
struct BBDXChannel
{
	ISound* Chn;
	ISceneNode* Parent;
	int AliveTimer;
};

// Array of caches sound objects
irr::core::array<BBDXSound*> SoundCache;
irr::core::array<BBDXChannel*> SoundList;

// Setup the sound
void SoundSetup()
{

	//TODO: Static Linking!

	// Create engine
	SoundEngine = createIrrKlangDevice();

	// Check we could make it
	if(!SoundEngine)
	{
		MessageBox(NULL,"Error: Could not begin the sound engine!","BBDX - Error",MB_ICONERROR | MB_OK);
		exit(0);
		return;
	}

	return;
}

// If a channel stopped more than 60 seconds ago, but is accessed, we'll use this
// to check to see if it actually exists
bool ChannelExists(BBDXChannel* test)
{
	for(int i = 0; i < SoundList.size(); ++i)
		if(SoundList[i] == test)
			return true;
	return false;
}

// Load a sound
DLLPRE BBDXSound* DLLEX BLoadSound(const irr::c8* filename)
{
	// Create it
	BBDXSound* Sound = new BBDXSound();

	// Set it up
	Sound->Snd = SoundEngine->addSoundSourceFromFile(filename, ESM_AUTO_DETECT, false);

	if(Sound->Snd == 0)
	{
		delete Sound;
		return 0;
	}

	Sound->IsLooped = false;
	Sound->Volume = 1.0f;
	Sound->filename = filename;

	return Sound;
}

// Free a sound
DLLPRE void DLLEX BFreeSound(BBDXSound* Sound)
{
	if(Sound == 0)
		return;

	SoundEngine->removeSoundSource(Sound->Snd);
	delete Sound;
}

// Loop a sound
DLLPRE void DLLEX BLoopSound(BBDXSound* Sound)
{
	if(Sound == 0)
		return ;

	Sound->IsLooped = true;
}

// Set a sounds volume
DLLPRE void DLLEX BSoundVolume(BBDXSound* Sound, irr::f32 Volume)
{
	if(Sound == 0)
		return;

	Sound->Volume = Volume;
}

// Play a sound
DLLPRE BBDXChannel* DLLEX BPlaySound(BBDXSound* Sound)
{
	if(Sound == 0)
		return 0;

	// Play it
	ISound* Chn = SoundEngine->play2D(Sound->Snd,Sound->IsLooped,true,true);
	if(Chn == 0)
		return 0;
	Chn->setVolume(Sound->Volume);
	Chn->setIsPaused(false);

	// Setup the new channel
	BBDXChannel* Channel = new BBDXChannel();
	Channel->Parent = 0;
	Channel->Chn = Chn;
	Channel->AliveTimer = 0;
	SoundList.push_back(Channel);
	return Channel;
}

// Play a music file
DLLPRE BBDXChannel* DLLEX BPlayMusic(const irr::c8* filename)
{
	ISound* Chn = SoundEngine->play2D(filename,false,false,true);
	if(Chn == 0)
		return 0;
	BBDXChannel* Channel = new BBDXChannel();
	Channel->Parent = 0;
	Channel->Chn = Chn;
	Channel->AliveTimer = 0;

	SoundList.push_back(Channel);

	return Channel;
}

// Stop a channel
DLLPRE void DLLEX BStopChannel(BBDXChannel* Channel)
{
	if(Channel == 0 || !ChannelExists(Channel))
		return;

	Channel->Chn->stop();
	Channel->AliveTimer = clock();
}

// Pause a channel
DLLPRE void DLLEX BPauseChannel(BBDXChannel* Channel)
{
	if(Channel == 0 || !ChannelExists(Channel))
		return;

	Channel->Chn->setIsPaused(true);
}

// Resume a channel
DLLPRE void DLLEX BResumeChannel(BBDXChannel* Channel)
{
	if(Channel == 0 || !ChannelExists(Channel))
		return;

	Channel->Chn->setIsPaused(false);
}

// Change the volume of a channel
DLLPRE void DLLEX BChannelVolume(BBDXChannel* Channel, irr::f32 Volume)
{
	if(Channel == 0 || !ChannelExists(Channel))
		return;

	Channel->Chn->setVolume(Volume);
}

// Pan a channel
DLLPRE void DLLEX BChannelPan(BBDXChannel* Channel, irr::f32 Pan)
{
	if(Channel == 0 || !ChannelExists(Channel))
		return;

	Channel->Chn->setPan(Pan);
}

// Check if a channel is still playing
DLLPRE int DLLEX BChannelPlaying(BBDXChannel* Channel)
{
	if(Channel == 0 || !ChannelExists(Channel))
		return 0;
	if(Channel->Chn == 0)
		return 0;
	if(Channel->Chn->isFinished())
		return 0;
	else
		return 1;
}

// Globals for an array of channels, and 3D sound
ISceneNode* Listener;
irr::f32 DistanceScale;

// Just for velocity
vector3df GetForwardVector(ISceneNode* Obj)
{
	helplost(0,0,1,Obj,0);
	return vector3df(TFormedX,TFormedY,TFormedZ);
}

// Create an audio listener
DLLPRE void DLLEX BCreateListener(ISceneNode* parent, irr::f32 rolloff, irr::f32 doppler, irr::f32 distance)
{
	Listener = parent;
	DistanceScale = distance;
	SoundEngine->setListenerPosition(parent->getAbsolutePosition(), GetForwardVector(parent));
}

// Load a sound in 3D (much the same as 2D)
DLLPRE BBDXSound* BLoad3DSound(const irr::c8* filename)
{
	BBDXSound* Sound = new BBDXSound();
	Sound->IsLooped = false;
	Sound->Volume = 1.0f;
	Sound->filename = filename;
	Sound->Snd = SoundEngine->addSoundSourceFromFile(filename, ESM_AUTO_DETECT, false);

	if(Sound->Snd == 0)
	{
		delete Sound;
		return 0;
	}
	return Sound;
}

// Play a 3D sound
DLLPRE BBDXChannel* DLLEX BEmitSound(BBDXSound* Sound, ISceneNode* parent)
{
	if(Sound == 0)
		return 0;

	// Setup the channel
	BBDXChannel* Channel = new BBDXChannel();
	Channel->Parent = parent;
	Channel->Chn = SoundEngine->play3D(Sound->Snd,parent->getAbsolutePosition(),Sound->IsLooped,false,true);
	if(Channel->Chn == 0)
	{
		delete Channel;
		return 0;
	}
	Channel->Chn->setMinDistance(DistanceScale);
	Channel->AliveTimer = 0;

	// Add to list to update
	SoundList.push_back(Channel);
	return Channel;
}

// Update Sounds
void UpdateSound()
{
	
	// If the listener exists, position it
	if(Listener > 0)
		SoundEngine->setListenerPosition(Listener->getAbsolutePosition(), GetForwardVector(Listener));

	// Position sounds
	int Dispose = -1;
	for(int i = 0; i < SoundList.size(); ++i)
		if(SoundList[i]->AliveTimer == 0 && SoundList[i]->Chn->isFinished())
			SoundList[i]->AliveTimer = clock();
		else if(SoundList[i]->AliveTimer != 0 && clock() > (SoundList[i]->AliveTimer + 60000))
			Dispose = i;
		else if(SoundList[i]->Parent != 0)
			SoundList[i]->Chn->setPosition(SoundList[i]->Parent->getAbsolutePosition());

	if(Dispose >= 0)
	{
		// Drop the sounds (Remove comments when new lib arrives)
		SoundList[Dispose]->Chn->drop();
		BBDXChannel* C = SoundList[Dispose];
		SoundList.erase(Dispose);
		delete C;
	}

}




