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
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RealmCrafter
{
    public class LightFunction : ICloneable
    {
        class Event : ICloneable
        {
            public class TriggerTime
            {
                public int TimeH = 0, TimeM = 0, TimeMS = 1;
                public bool UseGameTime = false;
            }

            public TriggerTime Time = new TriggerTime();
            public bool Interpolate = false;
            public RenderingServices.Vector3 Color = new RenderingServices.Vector3();
            public int Radius;

            public object Clone()
            {
                Event E = new Event();
                E.Time.TimeH = Time.TimeH;
                E.Time.TimeM = Time.TimeM;
                E.Time.TimeMS = Time.TimeMS;
                E.Time.UseGameTime = Time.UseGameTime;
                E.Interpolate = Interpolate;
                E.Color = Color.Clone() as RenderingServices.Vector3;
                E.Radius = Radius;

                return E;
            }
        }

        List<Event> Events = new List<Event>();
        int LastTick = 0;
        int CurrentEvent = 0;
        int LastTimeM = 0, LastTimeH = 0;
        int LastEventTimeH = 0, LastEventTimeM = 0, LastEventTimeS = 0, LastEventTimeMS = 0;
        bool WaitTick = false;
        string FunctionName = "";

        System.Drawing.Color CurrentColor = System.Drawing.Color.Black;
        int CurrentRadius = 0;

        public LightFunction()
        {
            LastTick = System.Environment.TickCount;
        }

        public System.Drawing.Color Color
        {
            get { return CurrentColor; }
        }

        public int Radius
        {
            get { return CurrentRadius; }
        }

        public object Clone()
        {
            LightFunction F = new LightFunction();
            F.LastTick = LastTick;
            F.CurrentEvent = CurrentEvent;
            F.LastTimeM = LastTimeM;
            F.LastTimeH = LastTimeH;
            F.LastEventTimeH = LastEventTimeH;
            F.LastEventTimeM = LastEventTimeM;
            F.LastEventTimeS = LastEventTimeS;
            F.LastEventTimeMS = LastEventTimeMS;
            F.WaitTick = WaitTick;
            F.FunctionName = FunctionName;

            foreach (Event E in Events)
            {
                F.Events.Add(E.Clone() as Event);
            }

            return F;
        }

        public void ResetFrom(LightFunction function)
        {
            LastTick = function.LastTick;
            CurrentEvent = function.CurrentEvent;
            LastTimeM = function.LastTimeM;
            LastTimeH = function.LastTimeH;
            LastEventTimeH = function.LastEventTimeH;
            LastEventTimeM = function.LastEventTimeM;
            LastEventTimeS = function.LastEventTimeS;
            LastEventTimeMS = function.LastEventTimeMS;
            WaitTick = function.WaitTick;
            FunctionName = function.FunctionName;

            Events.Clear();
            foreach (Event E in function.Events)
            {
                Events.Add(E.Clone() as Event);
            }
        }

        public string Name
        {
            get
            {
                return FunctionName;
            }

            set
            {
                if(value != null)
                    FunctionName = value;
            }
        }

        public override string ToString()
        {
            return FunctionName;
        }

        public void Update(int timeH, int timeM, int timeS)
        {
            if (Events.Count == 0)
                return;

            RenderingServices.Vector3 NewColor = new RenderingServices.Vector3();
            int NewRadius = 0;
            Event Current = Events[CurrentEvent];
            Event Next;

            if (CurrentEvent == Events.Count - 1)
                Next = Events[0];
            else
                Next = Events[CurrentEvent + 1];

            // Check to see if we're waiting for a time on the next event
            if (Next.Time.UseGameTime)
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
                    if (timeH > Next.Time.TimeH || (timeH == Next.Time.TimeH && timeM > Next.Time.TimeM))
                        Proc = true;
                }

                // Exit if we don't need to process
                if (!Next.Interpolate && !Proc)
                    return;

                if (Next.Interpolate)
                {
                    int UseTimeH = Next.Time.TimeH;
                    if (WaitTick)
                        UseTimeH += 24;

                    int TimeNow = (timeH * (60 * 60)) + (timeM * 60) + timeS;
                    int TimeEvent = (UseTimeH * (60 * 60)) + (Next.Time.TimeM * 60);
                    int TimeLast = (LastEventTimeH * (60 * 60)) + (LastEventTimeM * 60) + LastEventTimeS;

                    TimeNow -= TimeLast;
                    TimeEvent -= TimeLast;

                    float TimeInterp = Convert.ToSingle(TimeNow) / Convert.ToSingle(TimeEvent);
                    if (TimeInterp > 1.0f)
                        TimeInterp = 1.0f;
                    if (TimeInterp < 0.0f)
                        TimeInterp = 0.0f;

                    // Interpolate everything
                    NewColor.X = Current.Color.X + ((Next.Color.X - Current.Color.X) * TimeInterp);
                    NewColor.Y = Current.Color.Y + ((Next.Color.Y - Current.Color.Y) * TimeInterp);
                    NewColor.Z = Current.Color.Z + ((Next.Color.Z - Current.Color.Z) * TimeInterp);
                    NewRadius = Convert.ToInt32(Convert.ToSingle(Current.Radius) + ((Convert.ToSingle(Next.Radius) - Convert.ToSingle(Current.Radius)) * TimeInterp));
                }

                if (Proc)
                {
                    NewColor.X = Next.Color.X;
                    NewColor.Y = Next.Color.Y;
                    NewColor.Z = Next.Color.Z;
                    NewRadius = Next.Radius;

                    LastEventTimeH = timeH;
                    LastEventTimeM = timeM;
                    LastEventTimeS = timeS;
                    LastEventTimeMS = System.Environment.TickCount;

                    Event NextNext = Events[(CurrentEvent + 2) % Events.Count];
                    if (NextNext.Time.UseGameTime)
                    {
                        if (NextNext.Time.TimeH < timeH || (NextNext.Time.TimeH == timeH && NextNext.Time.TimeM < timeM))
                        {
                            WaitTick = true;
                            LastTimeH = timeH;
                            LastTimeM = timeM;
                        }
                    }
                    

                    ++CurrentEvent;
                    if (CurrentEvent >= Events.Count)
                        CurrentEvent = 0;
                }
            }
            else
            {
                bool Proc = false;

                // Using millisecs
                if (Next.Interpolate && System.Environment.TickCount - LastEventTimeMS < Next.Time.TimeMS)
                {
                    Proc = true;

                    int TimeNow = System.Environment.TickCount - LastEventTimeMS;
                    int TimeNext = Next.Time.TimeMS;

                    float TimeInterp = Convert.ToSingle(TimeNow) / Convert.ToSingle(TimeNext);
                    if (TimeInterp > 1.0f)
                        TimeInterp = 1.0f;
                    if (TimeInterp < 0.0f)
                        TimeInterp = 0.0f;
                    
                    // Interpolate everything
                    NewColor.X = Current.Color.X + ((Next.Color.X - Current.Color.X) * TimeInterp);
                    NewColor.Y = Current.Color.Y + ((Next.Color.Y - Current.Color.Y) * TimeInterp);
                    NewColor.Z = Current.Color.Z + ((Next.Color.Z - Current.Color.Z) * TimeInterp);
                    NewRadius = Convert.ToInt32(Convert.ToSingle(Current.Radius) + ((Convert.ToSingle(Next.Radius) - Convert.ToSingle(Current.Radius)) * TimeInterp));
                }

                if (System.Environment.TickCount - LastEventTimeMS > Next.Time.TimeMS)
                {
                    Proc = true;

                    NewColor.X = Next.Color.X;
                    NewColor.Y = Next.Color.Y;
                    NewColor.Z = Next.Color.Z;
                    NewRadius = Next.Radius;

                    LastEventTimeH = timeH;
                    LastEventTimeM = timeM;
                    LastEventTimeS = timeS;
                    LastEventTimeMS = System.Environment.TickCount;

                    Event NextNext = Events[(CurrentEvent + 2) % Events.Count];
                    if (NextNext.Time.UseGameTime)
                    {
                        if (NextNext.Time.TimeH < timeH || (NextNext.Time.TimeH == timeH && NextNext.Time.TimeM < timeM))
                        {
                            WaitTick = true;
                            LastTimeH = timeH;
                            LastTimeM = timeM;
                        }
                    }

                    ++CurrentEvent;
                    if (CurrentEvent >= Events.Count)
                        CurrentEvent = 0;
                }

                if (!Proc)
                    return;
            }

            // Process update
            CurrentRadius = NewRadius;
            CurrentColor = System.Drawing.Color.FromArgb(
                Convert.ToInt32(NewColor.X * 255.0f),
                Convert.ToInt32(NewColor.Y * 255.0f),
                Convert.ToInt32(NewColor.Z * 255.0f));
        }

        public void Compile(System.Xml.XmlTextReader x)
        {
            Events.Clear();
            CurrentEvent = 0;

            // Read each item
            while (x.Read())
            {
                if (x.NodeType == XmlNodeType.EndElement && x.Name.ToLower() == "function")
                    return;

                if (x.NodeType == XmlNodeType.Element && x.Name.ToLower() == "event")
                {
                    Event E = new Event();
                    E.Time.TimeH = Convert.ToInt32(x.GetAttribute("timeh"));
                    E.Time.TimeM = Convert.ToInt32(x.GetAttribute("timem"));
                    E.Time.TimeMS = Convert.ToInt32(x.GetAttribute("timems"));
                    E.Time.UseGameTime = Convert.ToBoolean(x.GetAttribute("usegametime"));
                    E.Interpolate = Convert.ToBoolean(x.GetAttribute("interpolate"));
                    E.Radius = Convert.ToInt32(x.GetAttribute("radius"));
                    E.Color.X = Convert.ToSingle(x.GetAttribute("r"));
                    E.Color.Y = Convert.ToSingle(x.GetAttribute("g"));
                    E.Color.Z = Convert.ToSingle(x.GetAttribute("b"));

                    Events.Add(E);
                }
            }
        }

        public void Save(System.Xml.XmlTextWriter x)
        {
            x.WriteStartElement("function");
            x.WriteAttributeString("name", this.Name);

            foreach (Event E in Events)
            {
                x.WriteStartElement("event");

                x.WriteAttributeString("timeh", E.Time.TimeH.ToString());
                x.WriteAttributeString("timem", E.Time.TimeM.ToString());
                x.WriteAttributeString("timems", E.Time.TimeMS.ToString());
                x.WriteAttributeString("usegametime", E.Time.UseGameTime.ToString());
                x.WriteAttributeString("interpolate", E.Interpolate.ToString());
                x.WriteAttributeString("radius", E.Radius.ToString());
                x.WriteAttributeString("r", E.Color.X.ToString());
                x.WriteAttributeString("g", E.Color.Y.ToString());
                x.WriteAttributeString("b", E.Color.Z.ToString());

                x.WriteEndElement();
            }

            x.WriteEndElement();
        }

        public void Compile(System.Windows.Forms.DataGridView dataGrid)
        {
            Events.Clear();
            CurrentEvent = 0;

            // Loop through each item
            foreach (System.Windows.Forms.DataGridViewRow Row in dataGrid.Rows)
            {
                Event E = new Event();

                LightFunctionTime GridTime = Row.Cells[0].Tag as LightFunctionTime;
                bool GridInterpolate = Convert.ToBoolean(Row.Cells[1].Tag.ToString());
                RenderingServices.Vector3 GridColor = Row.Cells[2].Tag as RenderingServices.Vector3;
                int GridRadius = Convert.ToInt32(Row.Cells[3].Tag.ToString());

                E.Time.TimeH = GridTime.TimeH;
                E.Time.TimeM = GridTime.TimeM;
                E.Time.TimeMS = GridTime.TimeMS;
                E.Time.UseGameTime = GridTime.UseGameTime;

                E.Interpolate = GridInterpolate;
                E.Color = GridColor.Clone() as RenderingServices.Vector3;
                E.Radius = GridRadius;

                Events.Add(E);
            }
        }

        public void DeCompile(ref System.Windows.Forms.DataGridView dataGrid)
        {
            dataGrid.Rows.Clear();

            foreach (Event E in Events)
            {
                LightFunctionTime Time = new LightFunctionTime();
                Time.TimeH = E.Time.TimeH;
                Time.TimeM = E.Time.TimeM;
                Time.TimeMS = E.Time.TimeMS;
                Time.UseGameTime = E.Time.UseGameTime;

                dataGrid.Rows.Add(new string[] { Time.ToString(), E.Interpolate.ToString(), "", E.Radius.ToString() });

                dataGrid.Rows[dataGrid.Rows.Count - 1].Cells[0].Tag = Time;
                dataGrid.Rows[dataGrid.Rows.Count - 1].Cells[1].Tag = E.Interpolate.ToString();
                dataGrid.Rows[dataGrid.Rows.Count - 1].Cells[2].Tag = E.Color.Clone();
                dataGrid.Rows[dataGrid.Rows.Count - 1].Cells[3].Tag = E.Radius.ToString();

                dataGrid.Rows[dataGrid.Rows.Count - 1].Cells[2].Style.BackColor = System.Drawing.Color.FromArgb(
                    Convert.ToInt32(E.Color.X * 255.0f),
                    Convert.ToInt32(E.Color.Y * 255.0f),
                    Convert.ToInt32(E.Color.Z * 255.0f));
            }
        }
    }
}
