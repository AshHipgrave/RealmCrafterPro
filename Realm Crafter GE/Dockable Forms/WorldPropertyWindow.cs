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
/* Realmcrafter World Property form for use in WinForms dockable interface
 * Author: Shane Smith, Aug 2008
 * ***********************************
 * Interfaces required:
 * Scenery   (Client/Server) (100%)
 * Water     (Client/Server) (100%)
 * Colbox    (Client)        (100%)
 * Emitter   (Client)        (100%)
 * Terrain   (Client)        (100%)
 * Light     (Client)        ( 50%) - Replicated original functionality
 * Sound     (Client)        ( 80%) - Replicated original functionality
 * Waypoint  (Server)        ( 80%)
 * Portal    (Server)        (100%)
 * Trigger   (Server)        (100%)
 */
namespace RealmCrafter_GE
{
    using System;
    using System.Windows.Forms;
    using RealmCrafter.ClientZone;
    using Property_Interfaces;
    using WeifenLuo.WinFormsUI.Docking;

    using NGUINet;

    using System.Reflection;
    using System.Reflection.Emit;
    using RenderingServices;
    using System.Collections.Generic;
    using System.ComponentModel;

    public partial class WorldPropertyWindow : DockContent
    {
        public float UpdateTimer;
        private SceneryPropertyInterface SceneryInterface;
        public BlankPropertyInterface BlankInterface;
        private WaterPropertyInterface WaterInterface;
        private CollisionBoxPropertyInterface CollisionInterface;
        private EmitterPropertyInterface EmitterInterface;
        private TerrainPropertyInterface TerrainInterface;
        private LightPropertyInterface LightInterface;
        private MenuControlPropertyInterface MenuControlInterface;
        private SoundPropertyInterface SoundInterface;
        private TriggerPropertyInterface TriggerInterface;
        private PortalPropertyInterface PortalInterface;
        private WaypointPropertyInterface WaypointInterface;
        private TreePropertyInterface TreeInterface;
        private PlacerAreaPropertyInterface PlacerAreaInterface;
        private PlacerNodePropertyInterface PlacerNodeInterface;
        private ZoneObject ZO;

        public WorldPropertyWindow()
        {
            UpdateTimer = 0;
            BlankInterface = new BlankPropertyInterface();
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        public void RefreshObjectWindow()
        {
            if ((Program.GE.ZoneSelected.Count > 0) && (Program.GE.ZoneSelected.Count < 2))
            {
                ZO = (ZoneObject) Program.GE.ZoneSelected[0];
                if (ZO is Waypoint)
                {
                    Program.GE.WaypointLinkingModeA.Enabled = true;
                    Program.GE.WaypointLinkingModeB.Enabled = true;
                    Program.GE.WaypointRemoveA.Enabled = true;
                    Program.GE.WaypointRemoveB.Enabled = true;
                }
                else
                {
                    Program.GE.WaypointLinkingModeA.Enabled = false;
                    Program.GE.WaypointLinkingModeB.Enabled = false;
                    Program.GE.WaypointRemoveA.Enabled = false;
                    Program.GE.WaypointRemoveB.Enabled = false;
                }
                UpdatePropertyGrid(ZO);

                // JB: Since this function is called every time an object is transformed,
                // we can inform any zone object to update is mover pivots
                ZO.UpdateTransformTool();
            }
            else
            {
                Program.GE.WaypointLinkingModeA.Enabled = false;
                Program.GE.WaypointLinkingModeB.Enabled = false;
                Program.GE.WaypointRemoveA.Enabled = false;
                Program.GE.WaypointRemoveB.Enabled = false;
                BlankPropertyGrid();
            }

            
        }

        public void TimedRefreshObjectWindow()
        {
            if (UpdateTimer < 0.1)
            {
                RefreshObjectWindow();
                UpdateTimer = 2.5f;
            }
        }

        private void WorldPropertyWindow_Load(object sender, EventArgs e)
        {
            if ((Program.GE.ZoneSelected.Count > 0) && (Program.GE.ZoneSelected.Count < 2))
            {
                ZO = (ZoneObject) Program.GE.ZoneSelected[0];
                UpdatePropertyGrid(ZO);
            }
            else
            {
                BlankPropertyGrid();
            }
        }

        private void ObjectProperties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (!Program.GE.WorldSaved == false)
            {
                Program.GE.SetWorldSavedStatus(false);
            }

            // Used in the Water Properties
            if ((e.ChangedItem.Parent != null) && (e.ChangedItem.Parent.PropertyDescriptor != null) && (e.ChangedItem.Parent.PropertyDescriptor.Category == "Shader Parameters"))
                WaterPropertyInterface.SetShaderParameter(e.ChangedItem.Parent.PropertyDescriptor.Name, e.ChangedItem.Parent.Value);
            if ((e.ChangedItem.Parent != null) && (e.ChangedItem.Parent.PropertyDescriptor == null) && (e.ChangedItem.PropertyDescriptor.Category == "Shader Parameters"))
                WaterPropertyInterface.SetShaderParameter(e.ChangedItem.PropertyDescriptor.Name, e.ChangedItem.Value);
        }

        private void BlankPropertyGrid()
        {
            BlankInterface = new BlankPropertyInterface();
            ObjectProperties.SelectedObject = BlankInterface;
        }

        public void UpdatePropertyGridWith_Water( Water W )
        {
            // Generate the new assembly for the class
            AssemblyName AName = new AssemblyName("WaterPropertyDynamicInterface");
            AssemblyBuilder AB = AppDomain.CurrentDomain.DefineDynamicAssembly(AName, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder MB = AB.DefineDynamicModule(AName.Name, AName.Name + ".dll");

            // Create the class
            TypeBuilder TB = MB.DefineType("WaterPropertyDynamicInterface", TypeAttributes.Public, typeof(WaterPropertyInterface));
            MethodAttributes GetSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

            if (W.ShaderID > 0) 
            {
                int nParameters = RenderingServices.RenderWrapper.GetParameterCount(W.ShaderID);
                string[] aTextures = new string[4];
                aTextures[0] = aTextures[1] = aTextures[2] = aTextures[3] = "";

                for (int iParam =0; iParam < nParameters; ++iParam)
                {
                    int nAnnotations = RenderingServices.RenderWrapper.GetAnnotationCount(W.ShaderID, iParam);
                    string Param_Name  = RenderingServices.RenderWrapper.GetParameterName(W.ShaderID,  iParam);

                    // Find if should be property is an artist editable property
                        bool bArtistProp = false;
                        for (int iAnnotation = 0; iAnnotation < nAnnotations; ++iAnnotation)
                        {
                            string Annotation_Name = RenderingServices.RenderWrapper.GetAnnotationName(W.ShaderID, iParam, iAnnotation);
                            if (Annotation_Name.Equals("uidescription", StringComparison.CurrentCultureIgnoreCase) ||
                                Annotation_Name.Equals("uimin", StringComparison.CurrentCultureIgnoreCase) ||
                                Annotation_Name.Equals("uimax", StringComparison.CurrentCultureIgnoreCase) ||
                                Annotation_Name.Equals("uiwidget", StringComparison.CurrentCultureIgnoreCase) )
                            {
                                bArtistProp = true;
                                break;
                            }
                            if (Param_Name.Equals("TextureDef", StringComparison.CurrentCultureIgnoreCase))
                            {
                                if (Annotation_Name.Equals("Texture0", StringComparison.CurrentCultureIgnoreCase)) aTextures[0] = (string)RenderingServices.RenderWrapper.GetAnnotationValue(W.ShaderID, iParam, iAnnotation);
                                if (Annotation_Name.Equals("Texture1", StringComparison.CurrentCultureIgnoreCase)) aTextures[1] = (string)RenderingServices.RenderWrapper.GetAnnotationValue(W.ShaderID, iParam, iAnnotation);
                                if (Annotation_Name.Equals("Texture2", StringComparison.CurrentCultureIgnoreCase)) aTextures[2] = (string)RenderingServices.RenderWrapper.GetAnnotationValue(W.ShaderID, iParam, iAnnotation);
                                if (Annotation_Name.Equals("Texture3", StringComparison.CurrentCultureIgnoreCase)) aTextures[3] = (string)RenderingServices.RenderWrapper.GetAnnotationValue(W.ShaderID, iParam, iAnnotation);
                            }
                        }
    
                    if (Param_Name.Equals("TextureDef", StringComparison.CurrentCultureIgnoreCase)) continue;
                    if (!bArtistProp) continue;

                    object Param_Value = RenderingServices.RenderWrapper.GetParameterValue(W.ShaderID, iParam);
                    Type Type = Param_Value.GetType();

                    // Create the property
                    PropertyBuilder PB = TB.DefineProperty(Param_Name, System.Reflection.PropertyAttributes.HasDefault, Type, null);
                    MethodBuilder PBGet = TB.DefineMethod("get_" + Param_Name, GetSetAttr, Type, Type.EmptyTypes);
                    MethodBuilder PBSet = TB.DefineMethod("set_" + Param_Name, GetSetAttr, null, new Type[] { Type });

                    Type[] DescriptionCategParam = new Type[] { typeof(string) };
                    ConstructorInfo DescriptionCategInfo = typeof(CategoryAttribute).GetConstructor(DescriptionCategParam);
                    CustomAttributeBuilder DescriptionCatBuilder = new CustomAttributeBuilder(DescriptionCategInfo, new object[] { "Shader Parameters" });
                    PB.SetCustomAttribute(DescriptionCatBuilder);

                    // Get
                    ILGenerator IG = PBGet.GetILGenerator();
                    IG.Emit(OpCodes.Ldstr, Param_Name); //Ldc_I4_S, iParam);
                    IG.Emit(OpCodes.Call, typeof(WaterPropertyInterface).GetMethod("GetShaderParameter"));
                    IG.Emit(OpCodes.Unbox_Any, Type);
                    IG.Emit(OpCodes.Ret);

                    // Set
                    IG = PBSet.GetILGenerator();
                    IG.Emit(OpCodes.Ldstr, Param_Name);
                    IG.Emit(OpCodes.Ldarg_1);
                    IG.Emit(OpCodes.Box, Type);
                    IG.Emit(OpCodes.Call, typeof(WaterPropertyInterface).GetMethod("SetShaderParameter"));
                    IG.Emit(OpCodes.Ret);

                    PB.SetGetMethod(PBGet);
                    PB.SetSetMethod(PBSet);

                    // Give appropriate typeconverts
                    Type Tu = null;
                         if (Param_Value.GetType() == typeof(Vector1)) Tu = typeof(Vector1OptionsConverter);
                    else if (Param_Value.GetType() == typeof(Vector2)) Tu = typeof(Vector2OptionsConverter);
                    else if (Param_Value.GetType() == typeof(Vector3)) Tu = typeof(Vector3OptionsConverter);
                    else if (Param_Value.GetType() == typeof(Vector4)) Tu = typeof(Vector4OptionsConverter);


                    // Loop annotations
                    for (int iAnnotation = 0; iAnnotation < nAnnotations; ++iAnnotation)
                    {
                        string Annotation_Name = RenderingServices.RenderWrapper.GetAnnotationName(W.ShaderID, iParam, iAnnotation);
                        object Annotation_Value = RenderingServices.RenderWrapper.GetAnnotationValue(W.ShaderID, iParam, iAnnotation);

                        // Set property description
                        if (Annotation_Name.Equals("uidescription", StringComparison.CurrentCultureIgnoreCase))
                        {
                            string Desc = (string)Annotation_Value;
                            if (Desc == null)
                                continue;

                            Type[] DescriptionCtorParam = new Type[] { typeof(string) };
                            ConstructorInfo DescriptionCtorInfo = typeof(DescriptionAttribute).GetConstructor(DescriptionCtorParam);
                            CustomAttributeBuilder DescriptionBuilder = new CustomAttributeBuilder(DescriptionCtorInfo, new object[] { Desc });
                            PB.SetCustomAttribute(DescriptionBuilder);
                        }

                        // Set vector minimum value
                        if (Annotation_Name.Equals("uimin", StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (Annotation_Value is Vector4 && Param_Value is Vector4)
                            {
                                Vector4 V1 = Annotation_Value as Vector4;
                                Vector4 V2 = Param_Value as Vector4;

                                V2._MinX = V1.X;
                                V2._MinY = V1.Y;
                                V2._MinZ = V1.Z;
                                V2._MinW = V1.W;
                            }

                            if (Annotation_Value is Vector3 && Param_Value is Vector3)
                            {
                                Vector3 V1 = Annotation_Value as Vector3;
                                Vector3 V2 = Param_Value as Vector3;

                                V2._MinX = V1.X;
                                V2._MinY = V1.Y;
                                V2._MinZ = V1.Z;
                            }

                            if (Annotation_Value is Vector2 && Param_Value is Vector2)
                            {
                                Vector2 V1 = Annotation_Value as Vector2;
                                Vector2 V2 = Param_Value as Vector2;

                                V2._MinX = V1.X;
                                V2._MinY = V1.Y;
                            }

                            if (Annotation_Value is Vector1 && Param_Value is Vector1)
                            {
                                Vector1 V1 = Annotation_Value as Vector1;
                                Vector1 V2 = Param_Value as Vector1;

                                V2._MinX = V1.X;
                            }
                        }

                        // Set vector maximum value 
                        if (Annotation_Name.Equals("uimax", StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (Annotation_Value is Vector4 && Param_Value is Vector4)
                            {
                                Vector4 V1 = Annotation_Value as Vector4;
                                Vector4 V2 = Param_Value as Vector4;

                                V2._MaxX = V1.X;
                                V2._MaxY = V1.Y;
                                V2._MaxZ = V1.Z;
                                V2._MaxW = V1.W;
                            }

                            if (Annotation_Value is Vector3 && Param_Value is Vector3)
                            {
                                Vector3 V1 = Annotation_Value as Vector3;
                                Vector3 V2 = Param_Value as Vector3;

                                V2._MaxX = V1.X;
                                V2._MaxY = V1.Y;
                                V2._MaxZ = V1.Z;
                            }

                            if (Annotation_Value is Vector2 && Param_Value is Vector2)
                            {
                                Vector2 V1 = Annotation_Value as Vector2;
                                Vector2 V2 = Param_Value as Vector2;

                                V2._MaxX = V1.X;
                                V2._MaxY = V1.Y;
                            }

                            if (Annotation_Value is Vector1 && Param_Value is Vector1)
                            {
                                Vector1 V1 = Annotation_Value as Vector1;
                                Vector1 V2 = Param_Value as Vector1;

                                V2._MaxX = V1.X;
                            }
                        }
                    
                        // Set 'widget', UITypeEditor style
                        if (Annotation_Name.Equals("uiwidget", StringComparison.CurrentCultureIgnoreCase))
                        {
                            // UIWidget = "Color"
                            if (Annotation_Value is string && ((string)Annotation_Value).Equals("color", StringComparison.CurrentCultureIgnoreCase))
                            {
                                // Vector3
                                if (Param_Value is Vector3)
                                {
                                    Tu = typeof(Vector3ColorOptionsConverter);
                                    Type[] CtorParam = new Type[] { typeof(Type), typeof(Type) };
                                    ConstructorInfo CtorInfo = typeof(EditorAttribute).GetConstructor(CtorParam);
                                    CustomAttributeBuilder Builder = new CustomAttributeBuilder(CtorInfo, new object[] { typeof(Vector3ColorEditor), typeof(System.Drawing.Design.UITypeEditor) });
                                    PB.SetCustomAttribute(Builder);
                                }

                                // Vector4
                                if (Param_Value is Vector4)
                                {
                                    Tu = typeof(Vector4ColorOptionsConverter);
                                    Type[] CtorParam = new Type[] { typeof(Type), typeof(Type) };
                                    ConstructorInfo CtorInfo = typeof(EditorAttribute).GetConstructor(CtorParam);
                                    CustomAttributeBuilder Builder = new CustomAttributeBuilder(CtorInfo, new object[] { typeof(Vector4ColorEditor), typeof(System.Drawing.Design.UITypeEditor) });
                                    PB.SetCustomAttribute(Builder);
                                }
                            }

                            // UIWidget = "Slider"
                            if (Annotation_Value is string && ((string)Annotation_Value).Equals("slider", StringComparison.CurrentCultureIgnoreCase))
                            {
                                // Vector1
                                if (Param_Value is Vector1)
                                {
                                    Type[] CtorParam = new Type[] { typeof(Type), typeof(Type) };
                                    ConstructorInfo CtorInfo = typeof(EditorAttribute).GetConstructor(CtorParam);
                                    CustomAttributeBuilder Builder = new CustomAttributeBuilder(CtorInfo, new object[] { typeof(Vector1SliderEditor), typeof(System.Drawing.Design.UITypeEditor) });
                                    PB.SetCustomAttribute(Builder);
                                }

                                // Vector2
                                if (Param_Value is Vector2)
                                {
                                    Type[] CtorParam = new Type[] { typeof(Type), typeof(Type) };
                                    ConstructorInfo CtorInfo = typeof(EditorAttribute).GetConstructor(CtorParam);
                                    CustomAttributeBuilder Builder = new CustomAttributeBuilder(CtorInfo, new object[] { typeof(Vector2SliderEditor), typeof(System.Drawing.Design.UITypeEditor) });
                                    PB.SetCustomAttribute(Builder);
                                }

                                // Vector3
                                if (Param_Value is Vector3)
                                {
                                    Type[] CtorParam = new Type[] { typeof(Type), typeof(Type) };
                                    ConstructorInfo CtorInfo = typeof(EditorAttribute).GetConstructor(CtorParam);
                                    CustomAttributeBuilder Builder = new CustomAttributeBuilder(CtorInfo, new object[] { typeof(Vector3SliderEditor), typeof(System.Drawing.Design.UITypeEditor) });
                                    PB.SetCustomAttribute(Builder);
                                }

                                // Vector4
                                if (Param_Value is Vector4)
                                {
                                    Type[] CtorParam = new Type[] { typeof(Type), typeof(Type) };
                                    ConstructorInfo CtorInfo = typeof(EditorAttribute).GetConstructor(CtorParam);
                                    CustomAttributeBuilder Builder = new CustomAttributeBuilder(CtorInfo, new object[] { typeof(Vector4SliderEditor), typeof(System.Drawing.Design.UITypeEditor) });
                                    PB.SetCustomAttribute(Builder);
                                }
                            }
                        }
                    }

                    // Set the typeconverter AFTER UIWidget processing
                    //   'Color' Widget will modify Tu to become a color processor
                    if (Tu != null)
                    {
                        Type[] TInfoParam = new Type[] { typeof(Type) };
                        ConstructorInfo TInfoCtor = typeof(TypeConverterAttribute).GetConstructor(TInfoParam);
                        CustomAttributeBuilder TInfoBuilder = new CustomAttributeBuilder(TInfoCtor, new object[] { Tu });
                        PB.SetCustomAttribute(TInfoBuilder);
                    }
                }
                // Create Texture Properties
                for (int iTex = 0; iTex < 4; ++iTex)
                    if (!string.IsNullOrEmpty(aTextures[iTex]))
                    {
                        string TexLabel = aTextures[iTex].Substring(0, aTextures[iTex].LastIndexOf(':'));
                        string TexDesc  = aTextures[iTex].Substring(TexLabel.Length+1 );

                        Type Type = typeof(string);
                        PropertyBuilder PB = TB.DefineProperty(TexLabel, System.Reflection.PropertyAttributes.HasDefault, Type, null);
                        MethodBuilder PBGet = TB.DefineMethod("get_" + TexLabel, GetSetAttr, Type, Type.EmptyTypes);
                        MethodBuilder PBSet = TB.DefineMethod("set_" + TexLabel, GetSetAttr, null, new Type[] { Type });

                        // Category
                        Type[] DescriptionCategParam = new Type[] { typeof(string) };
                        ConstructorInfo DescriptionCategInfo = typeof(CategoryAttribute).GetConstructor(DescriptionCategParam);
                        CustomAttributeBuilder DescriptionCatBuilder = new CustomAttributeBuilder(DescriptionCategInfo, new object[] { "Shader Textures" });
                        PB.SetCustomAttribute(DescriptionCatBuilder);

                        // Get
                        ILGenerator IG = PBGet.GetILGenerator();
                        IG.Emit(OpCodes.Ldc_I4_S, iTex);
                        IG.Emit(OpCodes.Call, typeof(WaterPropertyInterface).GetMethod("GetShaderTexture"));
                        IG.Emit(OpCodes.Unbox_Any, Type);
                        IG.Emit(OpCodes.Ret);

                        // Set
                        IG = PBSet.GetILGenerator();
                        IG.Emit(OpCodes.Ldc_I4_S, iTex); //Ldstr, Param_Name);
                        IG.Emit(OpCodes.Ldarg_1);
                        IG.Emit(OpCodes.Box, Type);
                        IG.Emit(OpCodes.Call, typeof(WaterPropertyInterface).GetMethod("SetShaderTexture"));
                        IG.Emit(OpCodes.Ret);

                        PB.SetGetMethod(PBGet);
                        PB.SetSetMethod(PBSet);

                        // Give appropriate typeconverts
                        Type[] TInfoParam = new Type[] { typeof(Type) };
                        ConstructorInfo TInfoCtor = typeof(TypeConverterAttribute).GetConstructor(TInfoParam);
                        CustomAttributeBuilder TInfoBuilder = new CustomAttributeBuilder(TInfoCtor, new object[] { typeof(PropertyTextureList) });
                        PB.SetCustomAttribute(TInfoBuilder);
                    }
            }


            // Save assembly
            Type T = TB.CreateType();
            AB.Save(AName.Name + ".dll");

            // Create instance
            WaterPropertyInterface O = (WaterPropertyInterface)Activator.CreateInstance(T);
                O.SetWaterObject( W );

            ObjectProperties.SelectedObject = O;
        }


        private void UpdatePropertyGrid(ZoneObject ZObj)
        {
            if ((Program.GE.ZoneSelected.Count > 0) && (Program.GE.ZoneSelected.Count < 2))
            {
                // Select object type and populate property list
                if (ZObj is Scenery)
                {
                    SceneryInterface = new SceneryPropertyInterface((Scenery) Program.GE.ZoneSelected[0]);
                    ObjectProperties.SelectedObject = SceneryInterface;
                }
                else if (ZObj is Water)
                {
                    UpdatePropertyGridWith_Water((Water)Program.GE.ZoneSelected[0]);
                }
                else if (ZObj is RCTTerrain)
                {
                    TerrainInterface = new TerrainPropertyInterface((RCTTerrain) Program.GE.ZoneSelected[0]);
                    ObjectProperties.SelectedObject = TerrainInterface;
                }
                else if (ZObj is Emitter)
                {
                    EmitterInterface = new EmitterPropertyInterface((Emitter) Program.GE.ZoneSelected[0]);
                    ObjectProperties.SelectedObject = EmitterInterface;
                }
                else if (ZObj is ColBox)
                {
                    CollisionInterface = new CollisionBoxPropertyInterface((ColBox) Program.GE.ZoneSelected[0]);
                    ObjectProperties.SelectedObject = CollisionInterface;
                }
                else if (ZObj is SoundZone)
                {
                    SoundInterface = new SoundPropertyInterface((SoundZone) Program.GE.ZoneSelected[0]);
                    ObjectProperties.SelectedObject = SoundInterface;
                }
                else if (ZObj is RealmCrafter.ClientZone.Light)
                {
                    LightInterface =
                        new LightPropertyInterface((RealmCrafter.ClientZone.Light) Program.GE.ZoneSelected[0]);
                    ObjectProperties.SelectedObject = LightInterface;
                }
                else if (ZObj is RealmCrafter.ClientZone.MenuControl)
                {
                    MenuControlInterface = new MenuControlPropertyInterface((RealmCrafter.ClientZone.MenuControl)Program.GE.ZoneSelected[0]);
                    ObjectProperties.SelectedObject = MenuControlInterface;
                }
                else if (ZObj is Trigger)
                {
                    TriggerInterface = new TriggerPropertyInterface((Trigger)Program.GE.ZoneSelected[0]);
                    ObjectProperties.SelectedObject = TriggerInterface;
                }
                else if (ZObj is Portal)
                {
                    PortalInterface = new PortalPropertyInterface((Portal)Program.GE.ZoneSelected[0]);
                    ObjectProperties.SelectedObject = PortalInterface;
                }
                else if (ZObj is Waypoint)
                {
                    WaypointInterface = new WaypointPropertyInterface((Waypoint)Program.GE.ZoneSelected[0]);
                    ObjectProperties.SelectedObject = WaypointInterface;
                }
                else if (ZObj is Tree)
                {
                    TreeInterface = new TreePropertyInterface((Tree)Program.GE.ZoneSelected[0]);
                    ObjectProperties.SelectedObject = TreeInterface;
                }
                else if (ZObj is TreePlacerArea)
                {
                    PlacerAreaInterface = new PlacerAreaPropertyInterface((TreePlacerArea)Program.GE.ZoneSelected[0]);
                    ObjectProperties.SelectedObject = PlacerAreaInterface;
                }
                else if (ZObj is TreePlacerNode)
                {
                    PlacerNodeInterface = new PlacerNodePropertyInterface((TreePlacerNode)Program.GE.ZoneSelected[0]);
                    ObjectProperties.SelectedObject = PlacerNodeInterface;
                }
                else
                {
                    // Unhandled object
                    MessageBox.Show("Error: unhandled property object: " + ZObj.ToString());
                    ObjectProperties.SelectedObject = BlankInterface;
                }
            }
            else
            {
                BlankInterface = new BlankPropertyInterface();
                ObjectProperties.SelectedObject = BlankInterface;
            }
        }

        // Functionality testing - To be removed.
        private void ButtonRefresh_Click(object sender, EventArgs e)
        {
            WorldPropertyWindow_Load(null, null);
        }
    }
}