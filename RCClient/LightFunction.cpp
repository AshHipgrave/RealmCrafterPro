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
#include "LightFunction.h"

namespace RealmCrafter
{

	LightFunction::LightFunction()
		: CurrentEvent(0), LastTimeM(0), LastTimeH(0),
		LastEventTimeH(0), LastEventTimeM(0), LastEventTimeS(0), LastEventTimeMS(0),
		CurrentRadius(0),
		WaitTick(false)
	{
		LastTick = MilliSecs();
	}

	LightFunction::~LightFunction()
	{
		ClearEvents();
	}

	void LightFunction::ClearEvents()
	{
		for(int i = 0; i < Events.size(); ++i)
			delete Events[i];
		Events.clear();
	}

	void LightFunction::Update(int timeH, int timeM, int timeS)
	{
		if (Events.size() == 0)
            return;

		NGin::Math::Color NewColor;
        int NewRadius = 0;
        Event* Current = Events[CurrentEvent];
        Event* Next;

        if (CurrentEvent == Events.size() - 1)
            Next = Events[0];
        else
            Next = Events[CurrentEvent + 1];

        // Check to see if we're waiting for a time on the next event
        if (Next->Time.UseGameTime)
        {
            bool Proc = false;

            // Waiting to pass midnight
            if (WaitTick)
            {
                // Passed midnight (the time 'inverted'), disable tick wait
                if (timeH < LastTimeH || (timeH == LastTimeH && timeM < LastTimeM))
                {
                    WaitTick = false;
                    LastEventTimeH -= 24;
                }
                else
                {
                    // Advance the wait
                    LastTimeH = timeH;
                    LastTimeM = timeM;
                }
            }
            else
            {
                if (timeH > Next->Time.TimeH || (timeH == Next->Time.TimeH && timeM > Next->Time.TimeM))
                    Proc = true;
            }

            // Exit if we don't need to process
            if (!Next->Interpolate && !Proc)
                return;

            if (Next->Interpolate)
            {
                int UseTimeH = Next->Time.TimeH;
                if (WaitTick)
                    UseTimeH += 24;

                int TimeNow = (timeH * (60 * 60)) + (timeM * 60) + timeS;
                int TimeEvent = (UseTimeH * (60 * 60)) + (Next->Time.TimeM * 60);
                int TimeLast = (LastEventTimeH * (60 * 60)) + (LastEventTimeM * 60) + LastEventTimeS;

                TimeNow -= TimeLast;
                TimeEvent -= TimeLast;

                float TimeInterp = ((float)(TimeNow)) / ((float)(TimeEvent));
                if (TimeInterp > 1.0f)
                    TimeInterp = 1.0f;
                if (TimeInterp < 0.0f)
                    TimeInterp = 0.0f;

                // Interpolate everything
				NewColor.R = Current->Color.R + ((Next->Color.R - Current->Color.R) * TimeInterp);
                NewColor.G = Current->Color.G + ((Next->Color.G - Current->Color.G) * TimeInterp);
                NewColor.B = Current->Color.B + ((Next->Color.B - Current->Color.B) * TimeInterp);
                NewRadius = (int)(((float)(Current->Radius)) + ((((float)(Next->Radius)) - ((float)(Current->Radius))) * TimeInterp));
            }

            if (Proc)
            {
                NewColor.R = Next->Color.R;
                NewColor.G = Next->Color.G;
                NewColor.B = Next->Color.B;
                NewRadius = Next->Radius;

                LastEventTimeH = timeH;
                LastEventTimeM = timeM;
                LastEventTimeS = timeS;
                LastEventTimeMS = MilliSecs();

                Event* NextNext = Events[(CurrentEvent + 2) % Events.size()];
                if (NextNext->Time.UseGameTime)
                {
                    if (NextNext->Time.TimeH < timeH || (NextNext->Time.TimeH == timeH && NextNext->Time.TimeM < timeM))
                    {
                        WaitTick = true;
                        LastTimeH = timeH;
                        LastTimeM = timeM;
                    }
                }
                

                ++CurrentEvent;
                if (CurrentEvent >= Events.size())
                    CurrentEvent = 0;
            }
        }
        else
        {
            bool Proc = false;

            // Using millisecs
            if (Next->Interpolate && MilliSecs() - LastEventTimeMS < Next->Time.TimeMS)
            {
                Proc = true;

                int TimeNow = MilliSecs() - LastEventTimeMS;
                int TimeNext = Next->Time.TimeMS;

                float TimeInterp = ((float)(TimeNow)) / ((float)(TimeNext));
                if (TimeInterp > 1.0f)
                    TimeInterp = 1.0f;
                if (TimeInterp < 0.0f)
                    TimeInterp = 0.0f;
                
                // Interpolate everything
                NewColor.R = Current->Color.R + ((Next->Color.R - Current->Color.R) * TimeInterp);
                NewColor.G = Current->Color.G + ((Next->Color.G - Current->Color.G) * TimeInterp);
                NewColor.B = Current->Color.B + ((Next->Color.B - Current->Color.B) * TimeInterp);
                NewRadius = (int)(((float)(Current->Radius)) + ((((float)(Next->Radius)) - ((float)(Current->Radius))) * TimeInterp));
            }

            if (MilliSecs() - LastEventTimeMS > Next->Time.TimeMS)
            {
                Proc = true;

                NewColor.R = Next->Color.R;
                NewColor.G = Next->Color.G;
                NewColor.B = Next->Color.B;
                NewRadius = Next->Radius;

                LastEventTimeH = timeH;
                LastEventTimeM = timeM;
                LastEventTimeS = timeS;
                LastEventTimeMS = MilliSecs();

                Event* NextNext = Events[(CurrentEvent + 2) % Events.size()];
                if (NextNext->Time.UseGameTime)
                {
                    if (NextNext->Time.TimeH < timeH || (NextNext->Time.TimeH == timeH && NextNext->Time.TimeM < timeM))
                    {
                        WaitTick = true;
                        LastTimeH = timeH;
                        LastTimeM = timeM;
                    }
                }

                ++CurrentEvent;
                if (CurrentEvent >= Events.size())
                    CurrentEvent = 0;
            }

            if (!Proc)
                return;
        }

        // Process update
        CurrentRadius = NewRadius;
        CurrentColor = NewColor;
	}

	void LightFunction::Compile(NGin::XMLReader* X)
	{
		ClearEvents();
		CurrentEvent = 0;

		// Read each item
		while(X->Read())
		{
			string ElementNameLower = stringToLower(X->GetNodeName());

			if(X->GetNodeType() == NGin::XNT_Element_End && ElementNameLower.compare("function") == 0)
				return;

			if(X->GetNodeType() == NGin::XNT_Element && ElementNameLower.compare("event") == 0)
			{
				Event* E = new Event();
				E->Time.TimeH = X->GetAttributeInt("timeh");
				E->Time.TimeM = X->GetAttributeInt("timem");
				E->Time.TimeMS = X->GetAttributeInt("timems");
				string UseGameTime = stringToLower(X->GetAttributeString("usegametime"));
				if(UseGameTime.compare("true") == 0)
					E->Time.UseGameTime = true;
				else
					E->Time.UseGameTime = false;

				string Interpolate = stringToLower(X->GetAttributeString("interpolate"));
				if(Interpolate.compare("true") == 0)
					E->Interpolate = true;
				else
					E->Interpolate = false;

				E->Radius = X->GetAttributeInt("radius");
				E->Color.R = X->GetAttributeFloat("r");
				E->Color.G = X->GetAttributeFloat("g");
				E->Color.B = X->GetAttributeFloat("b");
				
				Events.push_back(E);
			}
		}

	}



}