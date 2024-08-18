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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;

namespace RealmCrafter_GE.KeyBinding
{
    public class KeyBindings
    {
        const string KeyFile = "KeyBindings.xml";

        public Dictionary<Action, Keys> KeyMap;

    

        public KeyBindings()
        {
            KeyMap = new Dictionary<Action, Keys>();

            Load();

            // Default keys if none are set
            if (KeyMap.Count == 0)
            {
                Default();
                Save();
            }
        }

        public void Default()
        {
            KeyMap.Clear();

            // Camera
            KeyMap.Add(Action.Camera_Forward, Keys.W);
            KeyMap.Add(Action.Camera_Backward, Keys.S);
            KeyMap.Add(Action.Camera_Left, Keys.A);
            KeyMap.Add(Action.Camera_Right, Keys.D);
            KeyMap.Add(Action.Camera_Rotate, Keys.MButton);
            KeyMap.Add(Action.Camera_Up, Keys.PageUp);
            KeyMap.Add(Action.Camera_Down, Keys.PageDown);

            // Rotation
            KeyMap.Add(Action.Object_RotateYaw_Minus, Keys.Left);
            KeyMap.Add(Action.Object_RotateYaw_Plus, Keys.Right);
            KeyMap.Add(Action.Object_RotatePitch_Plus, Keys.Up);
            KeyMap.Add(Action.Object_RotatePitch_Minus, Keys.Down);
            KeyMap.Add(Action.Object_RotateRoll_Minus, Keys.A);
            KeyMap.Add(Action.Object_RotateRoll_Plus, Keys.Z);

            // Scaling
            KeyMap.Add(Action.Object_ScaleX_Minus, Keys.Left);
            KeyMap.Add(Action.Object_ScaleX_Plus, Keys.Right);
            KeyMap.Add(Action.Object_ScaleY_Plus, Keys.Up);
            KeyMap.Add(Action.Object_ScaleY_Minus, Keys.Down);
            KeyMap.Add(Action.Object_ScaleZ_Minus, Keys.A);
            KeyMap.Add(Action.Object_ScaleZ_Plus, Keys.Z);

            // Translation
            KeyMap.Add(Action.Object_Translate_X_Minus, Keys.Left);
            KeyMap.Add(Action.Object_Translate_X_Plus, Keys.Right);
            KeyMap.Add(Action.Object_Translate_Z_Plus, Keys.Up);
            KeyMap.Add(Action.Object_Translate_Z_Minus, Keys.Down);
            KeyMap.Add(Action.Object_Translate_Y_Plus, Keys.A);
            KeyMap.Add(Action.Object_Translate_Y_Minus, Keys.Z);

            // Grid 
            KeyMap.Add(Action.Grid_Up, Keys.PageUp);
            KeyMap.Add(Action.Grid_Down, Keys.PageDown);
            KeyMap.Add(Action.Grid_Reset, Keys.F12);

            KeyMap.Add(Action.Object_Delete, Keys.Delete);

          
        }

        public bool IsActionPressed(Action action)
        {
            if (!KeyMap.ContainsKey(action))
                return false;


            return KeyState.Get(KeyMap[action]);
            
        }

        private static List<BoundKey> ToList(Dictionary<Action, Keys> bindings)
        {
            List<BoundKey> result = new List<BoundKey>();

            foreach (KeyValuePair<Action, Keys> keyBind in bindings)
                result.Add(new BoundKey() { Key = keyBind.Value, Action = keyBind.Key });

            return result;
        }

        private static Dictionary<Action, Keys> ToDictionary(List<BoundKey>  bindings)
        {
            Dictionary<Action, Keys> result = new Dictionary<Action, Keys>();

            foreach (BoundKey keyBind in bindings)
                result.Add(keyBind.Action, keyBind.Key);

            return result;
        }



        public void Load()
        {
          
            if (File.Exists(KeyFile))
            {
                List<BoundKey> serializableBindings = new List<BoundKey>();

                TextReader reader = new StreamReader(File.OpenRead(KeyFile));

                XmlSerializer serializer = new XmlSerializer(typeof(List<BoundKey>));
                serializableBindings = (List<BoundKey>)serializer.Deserialize(reader);

                reader.Close();

                KeyMap = ToDictionary(serializableBindings);
            }

        }

        public void Save()
        {
            List<BoundKey> serializableBindings = ToList(KeyMap);

            if (File.Exists(KeyFile))
                File.Delete(KeyFile);
            TextWriter writer = new StreamWriter(File.OpenWrite(KeyFile));

            XmlSerializer serializer = new XmlSerializer(typeof(List<BoundKey>));
            serializer.Serialize(writer, serializableBindings);

            writer.Close();
        }



    }
    // Note - do not rename these, buttons use reflection to assign to them
    public enum Action
    {
        Camera_Forward,
        Camera_Backward,
        Camera_Left,
        Camera_Right,
        Camera_Up,
        Camera_Down,
        Camera_Rotate,

        Object_RotateYaw_Plus,
        Object_RotateYaw_Minus,
        Object_RotatePitch_Plus,
        Object_RotatePitch_Minus,
        Object_RotateRoll_Plus,
        Object_RotateRoll_Minus,

        Object_ScaleX_Plus,
        Object_ScaleX_Minus,
        Object_ScaleY_Plus,
        Object_ScaleY_Minus,
        Object_ScaleZ_Plus,
        Object_ScaleZ_Minus,

        Object_Translate_X_Plus,
        Object_Translate_X_Minus,
        Object_Translate_Y_Plus,
        Object_Translate_Y_Minus,
        Object_Translate_Z_Plus,
        Object_Translate_Z_Minus,

        Grid_Up,
        Grid_Down,
        Grid_Reset,

        Object_Delete
    }

    public struct BoundKey
    {
        public Keys Key;
        public Action Action;
    }
}
