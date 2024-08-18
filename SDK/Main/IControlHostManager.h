//##############################################################################################################################
// Realm Crafter Professional SDK																								
// Copyright (C) 2010 Solstar Games, LLC. All rights reserved																	
// contact@solstargames.com																																																		
//																																																																#
// Programmer: Jared Belkus																										
//																																
// This is a licensed product:
// BY USING THIS SOURCECODE, YOU ARE CONFIRMING YOUR ACCEPTANCE OF THE SOFTWARE AND AGREEING TO BECOME BOUND BY THE TERMS OF 
// THIS AGREEMENT. IF YOU DO NOT AGREE TO BE BOUND BY THESE TERMS, THEN DO NOT USE THE SOFTWARE.
//																		
// Licensee may NOT: 
//  (i)   create any derivative works of the Engine, including translations Or localizations, other than Games;
//  (ii)  redistribute, encumber, sell, rent, lease, sublicense, Or otherwise transfer rights to the Engine; or
//  (iii) remove Or alter any trademark, logo, copyright Or other proprietary notices, legends, symbols Or labels in the Engine.
//  (iv)  licensee may Not distribute the source code Or documentation To the engine in any manner, unless recipient also has a 
//        license To the Engine.													
//  (v)  use the Software to develop any software or other technology having the same primary function as the Software, 
//        including but not limited to using the Software in any development or test procedure that seeks to develop like 
//        software or other technology, or to determine if such software or other technology performs in a similar manner as the
//        Software																																
//##############################################################################################################################
#pragma once

#include <SDKMain.h>
#include "IControlHost.h"
#include "SControlFactoryEventArgs.h"

namespace RealmCrafter
{
	namespace SDK
	{
		class IControlHost;

		//! Interface for the control host manager
		/*!
		The control host manager (for network based GUI) requires
		and interface since the client needs to access it to pass
		any received messages. However, the entire implementation
		exists in the SDK for the sake of extensibility.
		*/
		class IControlHostManager
		{
		public:
			
			//! Type definition for the callback used to create a new control
			typedef NGin::IEventHandler<IControlHostManager, ControlFactoryEventArgs> ControlHostFactory;

			//! Register a factory for a specific control type
			/*!
			The given control type must be unique (ie, cannot clash with any NGUI types). The type GUI
			is used when creating a GUI packet on the server end to determine the controlhost to invoke
			on the client side.
			\returns A pointer to the ControlHostFactory which you must use to register a creation callback
			*/
			virtual ControlHostFactory* RegisterTemplateFactory(NGin::Type type) = 0;

			//! 'FormOpen' packet received from the server which must be processed
			virtual void ProcessFormOpen(PacketReader &packet) = 0;
			
			//! A control must be created from a packet (such as a child control)
			/*!
			\param master The topmost control in the hierarchy (such as the form on which controls exist)
			\param parent The parent control of the control to be created
			\param reader Data packet from which the control will be created
			*/
			virtual void CreateControlTreeFromPacket(IControlHost* master, IControlHost* parent, PacketReader &reader) = 0;

			//! Register a controlhost instance ready to receive properties
			virtual void RegisterControlHost(IControlHost* host) = 0;

			//! 'UpdateProperties' packet received from the server.
			virtual void ProcessUpdateProperties(PacketReader &packet) = 0;

			//! A local ControlHost has received an event which must be sent to server
			virtual void PostEventPacket(PacketWriter &pa) = 0;

			//! 'Pop' current event data.
			virtual void PopEventPacket(PacketWriter &pa) = 0;

			//! 'FormClose' packet received
			virtual void ProcessFormClosed(PacketReader &packet) = 0;

			//! Readout a 'global location' from a given control
			// This is used by the Form Designer in GE to locate a control onscreen
			virtual void ReadoutAbsoluteLocation(char* data, int length) = 0;

			//! Reset control sizes after resolution change
			virtual void ResetControlSizes() = 0;

			//! Get the NGUI manager instance
			virtual NGin::GUI::IGUIManager* GetGUIManager() = 0;

		};
	}
}