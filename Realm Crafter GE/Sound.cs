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
// Wrapper for BBDX sounds by Rob W (rottbott@hotmail.com)
// Written March 2007

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace IrrlichtSound
{
    // Function import
    internal static class SoundWrapper
    {
        [DllImport("BBDX.dll")]
        internal static extern uint BLoadSound(string Filename);

        [DllImport("BBDX.dll")]
        internal static extern void BFreeSound(uint Sound);

        [DllImport("BBDX.dll")]
        internal static extern void BLoopSound(uint Sound);

        [DllImport("BBDX.dll")]
        internal static extern void BSoundVolume(uint Sound, float Volume);

        [DllImport("BBDX.dll")]
        internal static extern uint BPlaySound(uint Sound);

        [DllImport("BBDX.dll")]
        internal static extern uint BPlayMusic(string Filename);

        [DllImport("BBDX.dll")]
        internal static extern void BStopChannel(uint Channel);

        [DllImport("BBDX.dll")]
        internal static extern void BPauseChannel(uint Channel);

        [DllImport("BBDX.dll")]
        internal static extern void BResumeChannel(uint Channel);

        [DllImport("BBDX.dll")]
        internal static extern void BChannelVolume(uint Channel, float Volume);

        [DllImport("BBDX.dll")]
        internal static extern void BChannelPan(uint Channel, float Pan);

        [DllImport("BBDX.dll")]
        internal static extern bool BChannelPlaying(uint Channel);

        [DllImport("BBDX.dll")]
        internal static extern uint BLoad3DSound(string Filename);

        [DllImport("BBDX.dll")]
        internal static extern void BCreateListener(uint Entity, float RolloffFactor, float DopplerScale,
                                                    float DistanceScale);

        [DllImport("BBDX.dll")]
        internal static extern uint BEmitSound(uint Sound, uint Entity);
    }

    // Sound class
    public class Sound : IDisposable
    {
        // Unmanaged handle
        private uint Handle;

        // Volume property
        public float Volume
        {
            set { SoundWrapper.BSoundVolume(Handle, value); }
        }

        // Constructor
        private Sound(uint Handle)
        {
            this.Handle = Handle;
        }

        // Static sound loader
        public static Sound Load(string Filename)
        {
            uint Handle = SoundWrapper.BLoadSound(Filename);
            if (Handle == 0)
            {
                return null;
            }
            else
            {
                return new Sound(Handle);
            }
        }

        // Plays a music file and returns the channel
        public static Channel PlayMusic(string Filename)
        {
            return new Channel(SoundWrapper.BPlayMusic(Filename));
        }

        // Plays this sound and returns the channel
        public Channel Play()
        {
            return new Channel(SoundWrapper.BPlaySound(Handle));
        }

        // Free unmanaged resources
        public void Dispose()
        {
            SoundWrapper.BFreeSound(Handle);
        }
    }

    // Channel class (sound being played)
    public class Channel
    {
        // Unmanaged handle
        private uint Handle;

        // Volume property
        public float Volume
        {
            set { SoundWrapper.BChannelVolume(Handle, value); }
        }

        // Pan property
        public float Pan
        {
            set { SoundWrapper.BChannelPan(Handle, value); }
        }

        // Playing property
        public bool Playing
        {
            get { return SoundWrapper.BChannelPlaying(Handle); }
        }

        // Constructor
        internal Channel(uint Handle)
        {
            this.Handle = Handle;
        }

        // Playback commands
        public void Pause()
        {
            SoundWrapper.BPauseChannel(Handle);
        }

        public void Resume()
        {
            SoundWrapper.BResumeChannel(Handle);
        }

        public void Stop()
        {
            SoundWrapper.BStopChannel(Handle);
        }
    }
}