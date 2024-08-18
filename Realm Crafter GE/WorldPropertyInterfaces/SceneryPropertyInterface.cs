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
/* This class interfaces the property grid with the selected scenery object
 * retrieving information from the Scenery object class
 * Author: Shane Smith, Aug 2008
 */
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using RealmCrafter;
using RealmCrafter.ClientZone;
using RealmCrafter.ServerZone;
using RealmCrafter_GE.Property_Interfaces;
using RealmCrafter_GE.Property_Interfaces.PropertyUIs;
using RenderingServices;

namespace RealmCrafter_GE.Property_Interfaces
{
    /*Properties
     * Entity - Location, Rotation, Scale
     * MeshID, TextureID, AnimationMode
     * CatchRain // Generates a CatchPlane when loaded in client
     * Lightmap
     * NameScript
     * Collision
     * Lod
     */


    public class cMeshBrowser : System.Drawing.Design.UITypeEditor
    {
        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return System.Drawing.Design.UITypeEditorEditStyle.Modal;
        }
        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public ushort MeshID;
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            PropertyGrid HeldGrid = Program.GE.m_propertyWindow.ObjectProperties;
            Program.GE.m_propertyWindow.ObjectProperties = new PropertyGrid();

            MeshID = MediaDialogs.GetMesh(true, MeshID);

            Program.GE.m_propertyWindow.ObjectProperties = HeldGrid;

            if (MeshID == 65535) return "";

            string meshName = Media.GetMeshName(MeshID);
            if (!string.IsNullOrEmpty(meshName))
            {
                meshName = meshName.Substring(0, meshName.Length - 1);
                meshName += ":" + MeshID.ToString();
                return meshName;
            }
            return "";
        }
    }

    public class SceneryPropertyInterface 
    {
        // Internal use
        private Scenery _Object;
        private int _InvSize;

        public cMeshBrowser MeshLOD_Low, MeshLOD_Medium;

        public SceneryPropertyInterface(Scenery Object)
        {
            _Object = Object;

            MeshLOD_Low = new cMeshBrowser();
            MeshLOD_Low.MeshID = Object.EN.MeshLOD_Low;
            MeshLOD_Medium = new cMeshBrowser();
            MeshLOD_Medium.MeshID = Object.EN.MeshLOD_Medium;
        }

        // Hackity-Hack enumerations to show data we want
        public enum _CollType
        {
            None = 0,
            Sphere = 1,
            Box = 2,
            Polygon = 3
        }

        public enum _AnimMode
        {
            None = 0,
            ConstantLoop = 1,
            ConstantPingPong = 2,
            WhenSelected = 3
        }

        #region Property interface

        #region Advanced
        [CategoryAttribute("Advanced"),
         DescriptionAttribute(
             "Changes raw meshID - Zone must be saved and reloaded to see changes. Zone could be corrupted if the ID does not exist"
             ),
         TypeConverter(typeof(PropertyMeshList))]
        public string MeshName
        {
            get { return GE.NiceMeshName(_Object.MeshID); }
            set
            {
                string s = value;
                int ColonIndex = s.LastIndexOf(":");
                string q = s.Substring(ColonIndex + 2, s.Length - ColonIndex - 2);
                ushort SelectedMeshID = Convert.ToUInt16(q);
                string Mesh = Media.GetMeshName(SelectedMeshID);
                if (Mesh != "") 
                {
                    if ( _Object.CollisionMeshID == _Object.MeshID )
                        _Object.CollisionMeshID = SelectedMeshID;
                    _Object.MeshID = SelectedMeshID;
                }
            }
        }

        [CategoryAttribute("Advanced"),
         DescriptionAttribute("Mesh path")]
        public string MeshPath
        {
            get { return System.IO.Path.GetDirectoryName(Media.GetMeshName(_Object.MeshID)); }
        }

//         [CategoryAttribute("Advanced"),
//          DescriptionAttribute("Changes raw TextureID that can be found inside the Media tab"),
//          TypeConverter(typeof(PropertyTextureList))]
//         public string TextureID
//         {
//             get { return GE.NiceTextureName(_Object.TextureID); }
//             set
//             {
//                 if (value == "No Texture")
//                 {
//                     _Object.TextureID = 65535;
//                     return;
//                 }
//                 string s = value;
//                 int ColonIndex = s.LastIndexOf(":");
//                 string q = s.Substring(ColonIndex + 2, s.Length - ColonIndex - 2);
//                 ushort SelectedTextureID = Convert.ToUInt16(q);
//                 uint Tex = Media.GetTexture(SelectedTextureID, false);
//                 if (Tex != 0)
//                 {
//                     _Object.EN.Texture(Tex);
//                     _Object.TextureID = SelectedTextureID;
//                 }
//             }
//         }
        #endregion

        #region Scenery
        [CategoryAttribute("Scenery"),
         DescriptionAttribute("0 = none, 1 = constant loop, 2 = constant ping-pong, 3 = when selected")]
        public _AnimMode Animationmode
        {
            get { return (_AnimMode) _Object.AnimationMode; }
            set { _Object.AnimationMode = (byte) value; }
        }

        [CategoryAttribute("Scenery"),
         DescriptionAttribute("Generates a CatchPlane when loaded in client")]
        public bool Catchrain
        {
            get { return _Object.CatchRain; }
            set { _Object.CatchRain = value; }
        }

        /*
        [CategoryAttribute("Scenery")]
        public string lightmap
        {
            get { return _Object.Lightmap; }
            set { _Object.Lightmap = value; }
        }

        [CategoryAttribute("Scenery")]
        public string rcte
        {
            get { return _Object.RCTE; }
            set { _Object.RCTE = value; }
        }
        */
        #endregion


        [TypeConverter(typeof(PropertyScriptList)),
         CategoryAttribute("Scripts")]
        public string InteractScript
        {
            get
            {
                return _Object.NameScript;
            }
            set
            {
                _Object.NameScript = value;

                if (!string.IsNullOrEmpty(value))
                    _Object.Interactive = true;
                else
                    _Object.Interactive = false;

            }
        }

        #region Owned
        [CategoryAttribute("Ownable"),
         DescriptionAttribute("Can scenery be owned by players. Maximum of 500 per zone.")]
        public bool Ownable
        {
            get
            {
                if (_Object.SceneryID > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            set
            {
                if ((value == false) && (_Object.SceneryID > 0))
                {
                    if (Program.GE.CurrentServerZone.Instances[0].OwnedScenery[_Object.SceneryID - 1] != null)
                    {
                        Program.GE.CurrentServerZone.Instances[0].OwnedScenery[_Object.SceneryID - 1] = null;
                    }

                    _Object.SceneryID = 0;
                    return;
                }
                else if ((value == false) && (_Object.SceneryID == 0))
                {
                    _Object.SceneryID = 0;
                    return;
                }
                else if ((value == true) && (_Object.SceneryID == 0))
                {
                    bool Found = false;
                    for (int i = 0; i < 500; ++i)
                    {
                        if (Program.GE.CurrentServerZone.Instances[0].OwnedScenery[i] == null)
                        {
                            Program.GE.CurrentServerZone.Instances[0].OwnedScenery[i] = new OwnedScenery();
                            _Object.SceneryID = (byte) (i + 1);
                            Found = true;
                            break;
                        }
                    }

                    if (!Found)
                    {
                        MessageBox.Show("Maximum ownable scenery objects already set in this zone.", "Error");
                    }

                    return;
                }
            }
        }

        [CategoryAttribute("Ownable"),
         ReadOnly(true),
         DescriptionAttribute("Scenery ID value, -1 is shown if it is not ownable. Maximum of 500 per zone.")]
        public int Scenery
        {
            get { return _Object.SceneryID - 1; }
        }

        [CategoryAttribute("Ownable"),
         DescriptionAttribute("Sets scenery object inventory size, Maximum size: 50")]
        public int InventorySize
        {
            get
            {
                if (_Object.SceneryID == 0)
                {
                    _InvSize = 0;
                }
                else
                {
                    _InvSize =
                        Program.GE.CurrentServerZone.Instances[0].OwnedScenery[_Object.SceneryID - 1].InventorySize;
                }

                return _InvSize;
            }

            set
            {
                if (value > 50)
                {
                    value = 50;
                }
                else if (value < 0)
                {
                    value = 0;
                }

                if (value == 0)
                {
                    if (Program.GE.CurrentServerZone.Instances[0].OwnedScenery[_Object.SceneryID - 1].Inventory != null)
                    {
                        Program.GE.CurrentServerZone.Instances[0].OwnedScenery[_Object.SceneryID - 1].Inventory = null;
                    }

                    Program.GE.CurrentServerZone.Instances[0].OwnedScenery[_Object.SceneryID - 1].InventorySize =
                        (byte) value;
                }
                else
                {
                    if (_Object.SceneryID > 0)
                    {
                        Program.GE.CurrentServerZone.Instances[0].OwnedScenery[_Object.SceneryID - 1].InventorySize =
                            (byte) value;
                        if (Program.GE.CurrentServerZone.Instances[0].OwnedScenery[_Object.SceneryID - 1].Inventory ==
                            null)
                        {
                            Program.GE.CurrentServerZone.Instances[0].OwnedScenery[_Object.SceneryID - 1].Inventory =
                                new Inventory();
                        }

                        _InvSize = value;
                    }

                    _InvSize = 0;
                }
            }
        }
        #endregion

        #region Entity handling

        #region Location
        [CategoryAttribute("Location")]
        public float LocX
        {
            get { return _Object.EN.X(); }
            set { _Object.EN.Position(value, _Object.EN.Y(), _Object.EN.Z()); }
        }

        [CategoryAttribute("Location")]
        public float LocY
        {
            get { return _Object.EN.Y(); }
            set { _Object.EN.Position(_Object.EN.X(), value, _Object.EN.Z()); }
        }

        [CategoryAttribute("Location")]
        public float LocZ
        {
            get { return _Object.EN.Z(); }
            set { _Object.EN.Position(_Object.EN.X(), _Object.EN.Y(), value); }
        }
        #endregion

        #region Scale
        [CategoryAttribute("Scale")]
        public float ScaleX
        {
            get { return _Object.EN.ScaleX() * 20; }

            set { _Object.EN.Scale(value / 20, _Object.EN.ScaleY(), _Object.EN.ScaleZ()); }
        }

        [CategoryAttribute("Scale")]
        public float ScaleY
        {
            get { return _Object.EN.ScaleY() * 20; }

            set { _Object.EN.Scale(_Object.EN.ScaleX(), value / 20, _Object.EN.ScaleZ()); }
        }

        [CategoryAttribute("Scale")]
        public float ScaleZ
        {
            get { return _Object.EN.ScaleZ() * 20; }

            set { _Object.EN.Scale(_Object.EN.ScaleX(), _Object.EN.ScaleY(), value / 20); }
        }
        #endregion

        #region Rotation
        [CategoryAttribute("Rotation")]
        public float Pitch
        {
            get { return _Object.EN.Pitch(); }

            set { _Object.EN.Rotate(value, _Object.EN.Yaw(), _Object.EN.Roll()); }
        }

        [CategoryAttribute("Rotation")]
        public float Roll
        {
            get { return _Object.EN.Roll(); }

            set { _Object.EN.Rotate(_Object.EN.Pitch(), _Object.EN.Yaw(), value); }
        }

        [CategoryAttribute("Rotation")]
        public float Yaw
        {
            get { return _Object.EN.Yaw(); }

            set { _Object.EN.Rotate(_Object.EN.Pitch(), value, _Object.EN.Roll()); }
        }
        #endregion

        #region Collision
        [CategoryAttribute("Collision"),
         DescriptionAttribute("Sets collision type")]
        public _CollType CollisionStyle
        {
            get
            {
                _CollType ReturnValue;
                if ((byte) Collision.EntityType(_Object.EN) == 9)
                {
                    ReturnValue = _CollType.None;
                }
                else
                {
                    ReturnValue = (_CollType) Collision.EntityType(_Object.EN);
                }

                return ReturnValue;
            }

            set
            {
                byte SaveType;
                if (value == _CollType.None)
                {
                    // Saves to the correct none type
                    SaveType = 9;
                }
                else
                {
                    SaveType = (byte) value;
                }

                Collision.EntityType(_Object.EN, SaveType);
            }
        }

        [CategoryAttribute("Collision"), DescriptionAttribute("Set collision Mesh"),
        Editor(typeof(cMeshBrowser), typeof(System.Drawing.Design.UITypeEditor))]
        public string CollisionMesh
        {
            get { return GE.NiceMeshName(_Object.CollisionMeshID); }
            set
            {
                if (!string.IsNullOrEmpty(value)) 
                {
                    string s = value;
                    int ColonIndex = s.LastIndexOf(":") + 1;
                    string q = s.Substring(ColonIndex, s.Length - ColonIndex);
                    ushort SelectedMeshID = Convert.ToUInt16(q);
                    string Mesh = Media.GetMeshName(SelectedMeshID);
                    if (Mesh != "") _Object.CollisionMeshID = SelectedMeshID;
                }
            }
        }
        #endregion

        #region LOD
        [CategoryAttribute("LOD"), DescriptionAttribute("Level of Detail: High")]
//         [Editor(typeof(cMeshBrowser), typeof(System.Drawing.Design.UITypeEditor))]
        public string High
        {
            get { return GE.NiceMeshName(_Object.MeshID); }
        }
        [CategoryAttribute("LOD"), DescriptionAttribute("Distance for change High To Medium")]
        public float HighToMedium_Distance
        {
            get { return _Object.EN.distLOD_High; }
            set { _Object.EN.distLOD_High = (float)Convert.ToDouble(value); }
        }

        [CategoryAttribute("LOD"), DescriptionAttribute("Level of Detail: Medium")]
        [Editor(typeof(cMeshBrowser), typeof(System.Drawing.Design.UITypeEditor))]
        public string Medium
        {
            get { return GE.NiceMeshName(_Object.EN.MeshLOD_Medium); }
            set
            {
                if (value == "")
                {
                    MeshLOD_Medium.MeshID = 65535;
                    _Object.EN.MeshLOD_Medium = 65535;
                }
                else
                {
                    string s = value;
                    int ColonIndex = s.LastIndexOf(":") + 1;
                    string q = s.Substring(ColonIndex, s.Length - ColonIndex);
                    ushort SelectedMeshID = Convert.ToUInt16(q);
                    string Mesh = Media.GetMeshName(SelectedMeshID);
                    if (Mesh != "")
                    {
                        MeshLOD_Medium.MeshID = SelectedMeshID;
                        _Object.EN.MeshLOD_Medium = SelectedMeshID;
                    }
                }
            }
        }
        [CategoryAttribute("LOD"), DescriptionAttribute("Distance for change Medium To Low")]
        public float MediumToLow_Distance
        {
            get { return _Object.EN.distLOD_Medium; }
            set { _Object.EN.distLOD_Medium = (float)Convert.ToDouble(value); }
        }

        [CategoryAttribute("LOD"), DescriptionAttribute("Level of Detail: Low")]
        [Editor(typeof(cMeshBrowser), typeof(System.Drawing.Design.UITypeEditor))]
        public string Low
        {
            get { return GE.NiceMeshName(_Object.EN.MeshLOD_Low); }
            set
            {
                if (value == "")
                {
                    MeshLOD_Low.MeshID = 65535;
                    _Object.EN.MeshLOD_Low = 65535;
                }
                else
                {
                    string s = value;
                    int ColonIndex = s.LastIndexOf(":") + 1;
                    string q = s.Substring(ColonIndex, s.Length - ColonIndex);
                    ushort SelectedMeshID = Convert.ToUInt16(q);
                    string Mesh = Media.GetMeshName(SelectedMeshID);
                    if (Mesh != "")
                    {
                        MeshLOD_Low.MeshID = SelectedMeshID;
                        _Object.EN.MeshLOD_Low = SelectedMeshID;
                    }
                }
            }
        }
        [CategoryAttribute("LOD"), DescriptionAttribute("Distance for change Low To Hide")]
        public float LowToHide_Distance
        {
            get { return _Object.EN.distLOD_Low; }
            set { _Object.EN.distLOD_Low = (float)Convert.ToDouble(value); }
        }

        #endregion

        #endregion

        #endregion
    }

}
