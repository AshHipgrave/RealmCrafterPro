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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Reflection.Emit;
using RenderingServices;

namespace RealmCrafter_GE
{
    public partial class ConfigureParameter : Form
    {
        private ShaderProfile _Effect;
        private Entity _Entity;
        private bool _Changed = false;
        public static Dictionary<string, object> Parameters = new Dictionary<string, object>();
        public static Dictionary<string, object> OldParameters = new Dictionary<string, object>();

        public ConfigureParameter()
        {
            InitializeComponent();

            
        }

        public Entity Entity
        {
            get { return _Entity; }
            set { _Entity = value; CopyEntityToTemp(); _Changed = false; ApplyParams.Enabled = false; }
        }

        public ShaderProfile Effect
        {
            get { return _Effect; }
            set { _Effect = value; RebuildPropertyInterface(); _Changed = false; ApplyParams.Enabled = false; }
        }

        public bool Changed
        {
            get { return _Changed; }
            set { _Changed = value; }
        }

        public static object GetProperty(string Name)
        {
            return Parameters[Name];
        }

        public static void SetProperty(string Name, object Value)
        {
            Parameters[Name] = Value;
        }

        private void CopyEntityToTemp()
        {
            Parameters.Clear();
            OldParameters.Clear();

            foreach (KeyValuePair<string, object> KeyP in _Entity.Parameters)
            {
                Parameters.Add(KeyP.Key, ((ICloneable)KeyP.Value).Clone());
                OldParameters.Add(KeyP.Key, ((ICloneable)KeyP.Value).Clone());
            }
        }

        private void RebuildPropertyInterface()
        {
            ParameterGrid.SelectedObject = new NoModelSelected();

            if (_Effect == null)
                return;

            LoadedEffect[] Effects = new LoadedEffect[6];
            Effects[0] = ShaderManager.LoadedShaders[ShaderManager.GetShaderID(_Effect.HighNear)];
            Effects[1] = ShaderManager.LoadedShaders[ShaderManager.GetShaderID(_Effect.HighFar)];
            Effects[2] = ShaderManager.LoadedShaders[ShaderManager.GetShaderID(_Effect.MediumNear)];
            Effects[3] = ShaderManager.LoadedShaders[ShaderManager.GetShaderID(_Effect.MediumFar)];
            Effects[4] = ShaderManager.LoadedShaders[ShaderManager.GetShaderID(_Effect.LowNear)];
            Effects[5] = ShaderManager.LoadedShaders[ShaderManager.GetShaderID(_Effect.LowFar)];

            // Use this for deletion
            List<string> Removers = new List<string>();

            foreach (KeyValuePair<string, object> LocalP in Parameters)
            {
                bool Found = false;

                for(int z = 0; z < Effects.Length; ++z)
                    foreach (KeyValuePair<string, ShaderParameter> EffectP in Effects[z].Parameters)
                        if (EffectP.Key == LocalP.Key)
                            Found = true;

                if (!Found)
                    Removers.Add(LocalP.Key);
                    //Parameters.Remove(LocalP.Key);
            }

            foreach (String Key in Removers)
                Parameters.Remove(Key);
            Removers.Clear();



            // Generate the new assembly for the class
            AssemblyName AName = new AssemblyName("ShaderParameterDynamicAssembly");
            AssemblyBuilder AB = AppDomain.CurrentDomain.DefineDynamicAssembly(AName, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder MB = AB.DefineDynamicModule(AName.Name, AName.Name + ".dll");

            // Create the class
            TypeBuilder TB = MB.DefineType("ShaderParameterDynamicType", TypeAttributes.Public);
            MethodAttributes GetSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

            // Loop through range/profile effects
            for (int z = 0; z < Effects.Length; ++z)
            {
                // Loop through the parameters in this effect
                foreach (KeyValuePair<string, ShaderParameter> KeyP in Effects[z].Parameters)
                {
                    // See if it exists in the parameters list already (Don't want to have two 'Tint' variables appearing)
                    bool Found = false;
                    foreach (KeyValuePair<string, object> LocalP in Parameters)
                        if (LocalP.Key == KeyP.Key)
                            Found = true;

                    if (!Found && KeyP.Value.Annotations.Count != 0)
                    {
                        Parameters.Add(KeyP.Key, ((ICloneable)KeyP.Value.Value).Clone());
                        OldParameters.Add(KeyP.Key, ((ICloneable)KeyP.Value.Value).Clone());
                    }
                    else if (Found && KeyP.Value.Annotations.Count != 0)
                    {
                        //Parameters[KeyP.Key] = ((ICloneable)Parameters.Value).Clone();
                        //OldParameters[KeyP.Key] = ((ICloneable)KeyP.Value.Value).Clone();
                    }
                    else if (KeyP.Value.Annotations.Count == 0)
                    {
                        continue;
                    }

                    // Locals
                    ShaderParameter P = KeyP.Value;
                    string Name = P.Name;
                    Type Type = P.Value.GetType();

                    // Create the property
                    PropertyBuilder PB = TB.DefineProperty(Name, System.Reflection.PropertyAttributes.HasDefault, Type, null);
                    MethodBuilder PBGet = TB.DefineMethod("get_" + Name, GetSetAttr, Type, Type.EmptyTypes);
                    MethodBuilder PBSet = TB.DefineMethod("set_" + Name, GetSetAttr, null, new Type[] { Type });

                    // Get
                    ILGenerator IG = PBGet.GetILGenerator();
                    IG.Emit(OpCodes.Ldstr, Name);
                    IG.Emit(OpCodes.Call, typeof(ConfigureParameter).GetMethod("GetProperty"));
                    IG.Emit(OpCodes.Unbox_Any, Type);
                    IG.Emit(OpCodes.Ret);

                    // Set
                    IG = PBSet.GetILGenerator();
                    IG.Emit(OpCodes.Ldstr, Name);
                    IG.Emit(OpCodes.Ldarg_1);
                    IG.Emit(OpCodes.Box, Type);
                    IG.Emit(OpCodes.Call, typeof(ConfigureParameter).GetMethod("SetProperty"));
                    IG.Emit(OpCodes.Ret);

                    PB.SetGetMethod(PBGet);
                    PB.SetSetMethod(PBSet);

                    // Give appropriate typeconverts
                    Type Tu = null;
                    if (P.Value.GetType() == typeof(Vector1))
                        Tu = typeof(Vector1OptionsConverter);
                    else if (P.Value.GetType() == typeof(Vector2))
                        Tu = typeof(Vector2OptionsConverter);
                    else if (P.Value.GetType() == typeof(Vector3))
                        Tu = typeof(Vector3OptionsConverter);
                    else if (P.Value.GetType() == typeof(Vector4))
                        Tu = typeof(Vector4OptionsConverter);

                    // Loop annotations
                    foreach (KeyValuePair<string, ShaderAnnotation> AnnotP in P.Annotations)
                    {
                        ShaderAnnotation A = AnnotP.Value;

                        // Set property description
                        if (A.Name.Equals("uidescription", StringComparison.CurrentCultureIgnoreCase))
                        {
                            string Desc = (string)A.Value;
                            if(Desc == null)
                                continue;

                            Type[] DescriptionCtorParam = new Type[] { typeof(string) };
                            ConstructorInfo DescriptionCtorInfo = typeof(DescriptionAttribute).GetConstructor(DescriptionCtorParam);
                            CustomAttributeBuilder DescriptionBuilder = new CustomAttributeBuilder(DescriptionCtorInfo, new object[] { Desc });
                            PB.SetCustomAttribute(DescriptionBuilder);
                        }

                        // Set vector minimum value
                        if (A.Name.Equals("uimin", StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (A.Value is Vector4 && P.Value is Vector4)
                            {
                                Vector4 V1 = A.Value as Vector4;
                                Vector4 V2 = Parameters[P.Name] as Vector4;

                                V2._MinX = V1.X;
                                V2._MinY = V1.Y;
                                V2._MinZ = V1.Z;
                                V2._MinW = V1.W;
                            }

                            if (A.Value is Vector3 && P.Value is Vector3)
                            {
                                Vector3 V1 = A.Value as Vector3;
                                Vector3 V2 = Parameters[P.Name] as Vector3;

                                V2._MinX = V1.X;
                                V2._MinY = V1.Y;
                                V2._MinZ = V1.Z;
                            }

                            if (A.Value is Vector2 && P.Value is Vector2)
                            {
                                Vector2 V1 = A.Value as Vector2;
                                Vector2 V2 = Parameters[P.Name] as Vector2;

                                V2._MinX = V1.X;
                                V2._MinY = V1.Y;
                            }

                            if (A.Value is Vector1 && P.Value is Vector1)
                            {
                                Vector1 V1 = A.Value as Vector1;
                                Vector1 V2 = Parameters[P.Name] as Vector1;

                                V2._MinX = V1.X;
                            }
                        }

                        // Set vector maximum value
                        if (A.Name.Equals("uimax", StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (A.Value is Vector4 && P.Value is Vector4)
                            {
                                Vector4 V1 = A.Value as Vector4;
                                Vector4 V2 = Parameters[P.Name] as Vector4;

                                V2._MaxX = V1.X;
                                V2._MaxY = V1.Y;
                                V2._MaxZ = V1.Z;
                                V2._MaxW = V1.W;
                            }

                            if (A.Value is Vector3 && P.Value is Vector3)
                            {
                                Vector3 V1 = A.Value as Vector3;
                                Vector3 V2 = Parameters[P.Name] as Vector3;

                                V2._MaxX = V1.X;
                                V2._MaxY = V1.Y;
                                V2._MaxZ = V1.Z;
                            }

                            if (A.Value is Vector2 && P.Value is Vector2)
                            {
                                Vector2 V1 = A.Value as Vector2;
                                Vector2 V2 = Parameters[P.Name] as Vector2;

                                V2._MaxX = V1.X;
                                V2._MaxY = V1.Y;
                            }

                            if (A.Value is Vector1 && P.Value is Vector1)
                            {
                                Vector1 V1 = A.Value as Vector1;
                                Vector1 V2 = Parameters[P.Name] as Vector1;

                                V2._MaxX = V1.X;
                            }
                        }

                        // Set 'widget', UITypeEditor style
                        if (A.Name.Equals("uiwidget", StringComparison.CurrentCultureIgnoreCase))
                        {
                            // UIWidget = "Color"
                            if (A.Value is string && ((string)A.Value).Equals("color", StringComparison.CurrentCultureIgnoreCase))
                            {
                                // Vector3
                                if (KeyP.Value.Value is Vector3)
                                {
                                    Tu = typeof(Vector3ColorOptionsConverter);
                                    Type[] CtorParam = new Type[] { typeof(Type), typeof(Type) };
                                    ConstructorInfo CtorInfo = typeof(EditorAttribute).GetConstructor(CtorParam);
                                    CustomAttributeBuilder Builder = new CustomAttributeBuilder(CtorInfo, new object[] { typeof(Vector3ColorEditor), typeof(System.Drawing.Design.UITypeEditor) });
                                    PB.SetCustomAttribute(Builder);
                                }

                                // Vector4
                                if (KeyP.Value.Value is Vector4)
                                {
                                    Tu = typeof(Vector4ColorOptionsConverter);
                                    Type[] CtorParam = new Type[] { typeof(Type), typeof(Type) };
                                    ConstructorInfo CtorInfo = typeof(EditorAttribute).GetConstructor(CtorParam);
                                    CustomAttributeBuilder Builder = new CustomAttributeBuilder(CtorInfo, new object[] { typeof(Vector4ColorEditor), typeof(System.Drawing.Design.UITypeEditor) });
                                    PB.SetCustomAttribute(Builder);
                                }
                            }

                            // UIWidget = "Slider"
                            if (A.Value is string && ((string)A.Value).Equals("slider", StringComparison.CurrentCultureIgnoreCase))
                            {
                                // Vector1
                                if (KeyP.Value.Value is Vector1)
                                {
                                    Type[] CtorParam = new Type[] { typeof(Type), typeof(Type) };
                                    ConstructorInfo CtorInfo = typeof(EditorAttribute).GetConstructor(CtorParam);
                                    CustomAttributeBuilder Builder = new CustomAttributeBuilder(CtorInfo, new object[] { typeof(Vector1SliderEditor), typeof(System.Drawing.Design.UITypeEditor) });
                                    PB.SetCustomAttribute(Builder);
                                }

                                // Vector2
                                if (KeyP.Value.Value is Vector2)
                                {
                                    Type[] CtorParam = new Type[] { typeof(Type), typeof(Type) };
                                    ConstructorInfo CtorInfo = typeof(EditorAttribute).GetConstructor(CtorParam);
                                    CustomAttributeBuilder Builder = new CustomAttributeBuilder(CtorInfo, new object[] { typeof(Vector2SliderEditor), typeof(System.Drawing.Design.UITypeEditor) });
                                    PB.SetCustomAttribute(Builder);
                                }

                                // Vector3
                                if (KeyP.Value.Value is Vector3)
                                {
                                    Type[] CtorParam = new Type[] { typeof(Type), typeof(Type) };
                                    ConstructorInfo CtorInfo = typeof(EditorAttribute).GetConstructor(CtorParam);
                                    CustomAttributeBuilder Builder = new CustomAttributeBuilder(CtorInfo, new object[] { typeof(Vector3SliderEditor), typeof(System.Drawing.Design.UITypeEditor) });
                                    PB.SetCustomAttribute(Builder);
                                }

                                // Vector4
                                if (KeyP.Value.Value is Vector4)
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
            }

            OldParameters.Clear();
            foreach (KeyValuePair<string, object> LocalP in Parameters)
            {
                OldParameters.Add(LocalP.Key, ((ICloneable)LocalP.Value).Clone());
            }

            // Save assembly
            Type T = TB.CreateType();
            AB.Save(AName.Name + ".dll");

            // Create instance
            object O = Activator.CreateInstance(T);
            ParameterGrid.SelectedObject = O;
        }

        private void ParameterGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            foreach (KeyValuePair<string, object> LocalP in Parameters)
            {
                string Name = LocalP.Key;
                object Value = LocalP.Value;

                if (Value is Vector1)
                    RenderingServices.RenderWrapper.EntityConstantFloat(_Entity.Handle, Name, (Value as Vector1).X);
                if (Value is Vector2)
                    RenderingServices.RenderWrapper.EntityConstantFloat2(_Entity.Handle, Name, (Value as Vector2).X, (Value as Vector2).Y);
                if (Value is Vector3)
                    RenderingServices.RenderWrapper.EntityConstantFloat3(_Entity.Handle, Name, (Value as Vector3).X, (Value as Vector3).Y, (Value as Vector3).Z);
                if (Value is Vector4)
                    RenderingServices.RenderWrapper.EntityConstantFloat4(_Entity.Handle, Name, (Value as Vector4).X, (Value as Vector4).Y, (Value as Vector4).Z, (Value as Vector4).W);
            }

            _Changed = true;
            ApplyParams.Enabled = true;
        }

        public void SaveParams_Click(object sender, EventArgs e)
        {
            ApplyParams_Click(sender, e);
            this.Hide();
        }

        public void ApplyParams_Click(object sender, EventArgs e)
        {
            if (_Effect == null)
                return;

            LoadedEffect[] Effects = new LoadedEffect[6];
            Effects[0] = ShaderManager.LoadedShaders[ShaderManager.GetShaderID(_Effect.HighNear)];
            Effects[1] = ShaderManager.LoadedShaders[ShaderManager.GetShaderID(_Effect.HighFar)];
            Effects[2] = ShaderManager.LoadedShaders[ShaderManager.GetShaderID(_Effect.MediumNear)];
            Effects[3] = ShaderManager.LoadedShaders[ShaderManager.GetShaderID(_Effect.MediumFar)];
            Effects[4] = ShaderManager.LoadedShaders[ShaderManager.GetShaderID(_Effect.LowNear)];
            Effects[5] = ShaderManager.LoadedShaders[ShaderManager.GetShaderID(_Effect.LowFar)];

            _Entity.Parameters.Clear();
            foreach (KeyValuePair<string, object> LocalP in Parameters)
                _Entity.Parameters.Add(LocalP.Key, ((ICloneable)LocalP.Value).Clone());
            Entity.ResendAllParameters();

            RealmCrafter.Media.SaveEntityParameters();
            ApplyParams.Enabled = false;
            _Changed = false;
        }

        public void CancelParams_Click(object sender, EventArgs e)
        {
            foreach (KeyValuePair<string, object> LocalP in OldParameters)
            {
                string Name = LocalP.Key;
                object Value = LocalP.Value;

                if (Value is Vector1)
                    RenderingServices.RenderWrapper.EntityConstantFloat(_Entity.Handle, Name, (Value as Vector1).X);
                if (Value is Vector2)
                    RenderingServices.RenderWrapper.EntityConstantFloat2(_Entity.Handle, Name, (Value as Vector2).X, (Value as Vector2).Y);
                if (Value is Vector3)
                    RenderingServices.RenderWrapper.EntityConstantFloat3(_Entity.Handle, Name, (Value as Vector3).X, (Value as Vector3).Y, (Value as Vector3).Z);
                if (Value is Vector4)
                    RenderingServices.RenderWrapper.EntityConstantFloat4(_Entity.Handle, Name, (Value as Vector4).X, (Value as Vector4).Y, (Value as Vector4).Z, (Value as Vector4).W);
            }

            _Entity.Parameters.Clear();
            foreach (KeyValuePair<string, object> LocalP in OldParameters)
                _Entity.Parameters.Add(LocalP.Key, ((ICloneable)LocalP.Value).Clone());

            if(sender == CancelParams) // Hide the form if the use actually hit cancel
                this.Hide();
        }


    }

    public class NoModelSelected
    {
        [Description("Temp")]
        public String Nothing
        {
            get { return "No object selected"; }
        }
    }
}