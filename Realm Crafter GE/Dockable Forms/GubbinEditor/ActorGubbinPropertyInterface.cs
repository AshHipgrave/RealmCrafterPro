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
using RealmCrafter;
using System.ComponentModel;
using RenderingServices;

namespace RealmCrafter_GE.Dockable_Forms.GubbinEditor
{
    public class ActorGubbinPropertyInterface
    {
        public GubbinActorTemplate Template;
        public ActorInstance PreviewActor;
        AnimSet.Anim LastAnim;

        public ActorGubbinPropertyInterface(GubbinActorTemplate template, ActorInstance previewActor)
        {
            Template = template;
            PreviewActor = previewActor;
            LastAnim = AnimSet.Anim.Idle;
        }

        [Category("Actor")]
        public string Actor
        {
            get
            {
                return Template.Actor.Race + " [" + Template.Actor.Class + "]";
            }
        }

        [Category("Actor")]
        public string Gender
        {
            get
            {
                return ((Template.Gender == 0) ? "Male" : "Female");
            }
        }

        [Category("Actor")]
        public AnimSet.Anim Animation
        {
            get
            {
                return LastAnim;
            }
            set
            {
                LastAnim = value;
                AnimSet.PlayAnimation(PreviewActor, 1, 0.5f, (int)value);

                Program.GE.SetWorldButtonSelection = (int)RealmCrafter_GE.GE.WorldButtonSelection.CREATE;
                Program.GE.m_GubbinEditor.UpdateWorldButtonSelection();
                Program.GE.UncheckObjectControls();

                Program.GE.m_GubbinEditor.Saved = false;
            }
        }

        [Category("Actor"), TypeConverter(typeof(BoneTypeEditor))]
        public string Bone
        {
            get
            {
                return Template.AssignedBoneName;
            }
            set
            {
                Template.AssignedBoneName = value;

                try
                {
                    if (Program.GE.m_GubbinEditor.GubbinPreviewMesh != null)
                        Program.GE.m_GubbinEditor.GubbinPreviewMesh.Parent(((Entity)Program.GE.m_GubbinEditor.ActorPreview.EN).FindChild(value), false);
                    if (Program.GE.m_GubbinEditor.GubbinPreviewEmitterEN != null)
                    {
                        //Program.GE.m_GubbinEditor.GubbinPreviewEmitterEN.Parent(((Entity)Program.GE.m_GubbinEditor.ActorPreview.EN).FindChild(Template.AssignedBoneName), false);
                        //Program.GE.m_GubbinEditor.GubbinPreviewEmitterEN.Position(Template.Position.X, Template.Position.Y, Template.Position.Z);
                        //Program.GE.m_GubbinEditor.GubbinPreviewEmitterEN.Scale(Template.Scale.X * 10.0f, Template.Scale.Y * 10.0f, Template.Scale.Z * 10.0f);
                        //Program.GE.m_GubbinEditor.GubbinPreviewEmitterEN.Rotate(Template.Rotation.X, Template.Rotation.Y, Template.Rotation.Z);
                    }

                    if (Program.GE.m_GubbinEditor.TransformPivot != null)
                    {
                        Program.GE.m_GubbinEditor.TransformPivot.Parent(((Entity)Program.GE.m_GubbinEditor.ActorPreview.EN).FindChild(Template.AssignedBoneName), false);
                        Program.GE.m_GubbinEditor.TransformPivot.Position(Template.Position.X, Template.Position.Y, Template.Position.Z);

                        if (Program.GE.m_GubbinEditor.GubbinPreviewMesh != null)
                            Program.GE.m_GubbinEditor.GubbinPreviewMesh.Scale(Template.Scale.X, Template.Scale.Y, Template.Scale.Z);

                        Program.GE.m_GubbinEditor.UpdateWorldButtonSelection();
                    }

                }
                catch (System.Exception)
                {
                }

                Program.GE.m_GubbinEditor.Saved = false;
            }
        }

        [Category("Appearance"), Editor(typeof(MeshTypeEditor), typeof(System.Drawing.Design.UITypeEditor)), TypeConverter(typeof(MeshOptionsConverter))]
        public MeshDisplay Mesh
        {
            get
            {
                return new MeshDisplay(Template.MeshID);
            }
            set
            {
                Template.MeshID = value.ID;

                if (Program.GE.m_GubbinEditor.GubbinPreviewMesh != null)
                {
                    Program.GE.m_GubbinEditor.GubbinPreviewMesh.Free();
                    Program.GE.m_GubbinEditor.GubbinPreviewMesh = null;
                }

                if (Template.MeshID != 65535)
                {
                    Program.GE.m_GubbinEditor.GubbinPreviewMesh = Media.GetMesh(Template.MeshID);
                    Program.GE.m_GubbinEditor.GubbinPreviewSeq = Program.GE.m_GubbinEditor.GubbinPreviewMesh.ExtractAnimSeq(Template.AnimationStartFrame, Template.AnimationEndFrame);
                    Program.GE.m_GubbinEditor.GubbinPreviewMesh.Animate(1, 1.0f, Program.GE.m_GubbinEditor.GubbinPreviewSeq);
                    if (Template.AnimationType == GubbinAnimationType.Inherited)
                    {   
                        Program.GE.m_GubbinEditor.GubbinPreviewMesh.Position(0, 0, 0);
                        Program.GE.m_GubbinEditor.GubbinPreviewMesh.Scale(1, 1, 1);
                        Program.GE.m_GubbinEditor.GubbinPreviewMesh.Rotate(0, 0, 0);
                        Program.GE.m_GubbinEditor.GubbinPreviewMesh.Parent(((Entity)Program.GE.m_GubbinEditor.ActorPreview.EN), false);
                        Program.GE.m_GubbinEditor.GubbinPreviewMesh.InheritAnimation = true;
                    }
                    else
                    {
                        Program.GE.m_GubbinEditor.GubbinPreviewMesh.Parent(((Entity)Program.GE.m_GubbinEditor.ActorPreview.EN).FindChild(Template.AssignedBoneName), false);
                        Program.GE.m_GubbinEditor.GubbinPreviewMesh.Position(Template.Position.X, Template.Position.Y, Template.Position.Z);
                        Program.GE.m_GubbinEditor.GubbinPreviewMesh.Scale(Template.Scale.X, Template.Scale.Y, Template.Scale.Z);
                        Program.GE.m_GubbinEditor.GubbinPreviewMesh.Rotate(Template.Rotation.X, Template.Rotation.Y, Template.Rotation.Z);
                        Program.GE.m_GubbinEditor.GubbinPreviewMesh.InheritAnimation = false;
                    }
                }

                Program.GE.m_GubbinEditor.Saved = false;
            }
        }

        [Category("Appearance"), TypeConverter(typeof(EmitterTypeEditor))]
        public string Emitter
        {
            get
            {
                return Template.Emitter;
            }
            set
            {
                Template.Emitter = value;

                if (Program.GE.m_GubbinEditor.GubbinPreviewEmitterEN != null)
                {
                    RottParticles.General.FreeEmitter(Program.GE.m_GubbinEditor.GubbinPreviewEmitterEN, true, false);
                    Program.GE.m_GubbinEditor.GubbinPreviewEmitterEN = null;
                    Program.GE.m_GubbinEditor.GubbinPreviewConfig = null;
                }

                if (!string.IsNullOrEmpty(value))
                {
                    Program.GE.m_GubbinEditor.GubbinPreviewConfig = RottParticles.EmitterConfig.Load(@"Data\Emitter Configs\" + value + ".rpc", Program.GE.Camera, 0);

                    if (Program.GE.m_GubbinEditor.GubbinPreviewConfig != null)
                    {
                        uint Tex = Media.GetTexture(Program.GE.m_GubbinEditor.GubbinPreviewConfig.DefaultTextureID, false);
                        if (Tex != 0)
                        {
                            Program.GE.m_GubbinEditor.GubbinPreviewConfig.ChangeTexture(Tex);
                            //Media.UnloadTexture(Program.GE.m_GubbinEditor.GubbinPreviewConfig.DefaultTextureID);
                        }


                        Program.GE.m_GubbinEditor.GubbinPreviewEmitterEN = RottParticles.General.CreateEmitter(Program.GE.m_GubbinEditor.GubbinPreviewConfig);


                    }
                }

                if (Program.GE.m_GubbinEditor.GubbinPreviewEmitterEN != null)
                {
//                     Program.GE.m_GubbinEditor.GubbinPreviewEmitterEN.Parent(((Entity)Program.GE.m_GubbinEditor.ActorPreview.EN).FindChild(Template.AssignedBoneName), false);
//                     Program.GE.m_GubbinEditor.GubbinPreviewEmitterEN.Position(Template.Position.X, Template.Position.Y, Template.Position.Z);
//                     Program.GE.m_GubbinEditor.GubbinPreviewEmitterEN.Scale(Template.Scale.X * 10.0f, Template.Scale.Y * 10.0f, Template.Scale.Z * 10.0f);
//                     Program.GE.m_GubbinEditor.GubbinPreviewEmitterEN.Rotate(Template.Rotation.X, Template.Rotation.Y, Template.Rotation.Z);
                }

                Program.GE.m_GubbinEditor.Saved = false;
            }
        }

        [Category("Appearance")]
        public bool LightEnabled
        {
            get
            {
                return Template.UseLight;
            }
            set
            {
                if (Program.GE.m_GubbinEditor.PreviewLight != null)
                {
                    Program.GE.m_GubbinEditor.PreviewLight.Freeing();
                    if (Program.GE.m_GubbinEditor.PreviewLight.Handle != null)
                        Program.GE.m_GubbinEditor.PreviewLight.Handle.Free();
                    if (Program.GE.m_GubbinEditor.PreviewLight.EN != null)
                        Program.GE.m_GubbinEditor.PreviewLight.EN.Free();
                }
                Program.GE.m_GubbinEditor.PreviewLight = null;
                Template.UseLight = value;

                if (value == false)
                    return;

                Program.GE.m_GubbinEditor.PreviewLight = new RealmCrafter.ClientZone.Light(null, true);
                Program.GE.m_GubbinEditor.PreviewLight.Radius = Template.LightRadius;
                Program.GE.m_GubbinEditor.PreviewLight.Handle.Radius(Program.GE.m_GubbinEditor.PreviewLight.Radius);

                Program.GE.m_GubbinEditor.PreviewLight.Red = (byte)(Template.LightColor.X * 255.0f);
                Program.GE.m_GubbinEditor.PreviewLight.Green = (byte)(Template.LightColor.Y * 255.0f);
                Program.GE.m_GubbinEditor.PreviewLight.Blue = (byte)(Template.LightColor.Z * 255.0f);
                Program.GE.m_GubbinEditor.PreviewLight.Handle.Color(
                    Program.GE.m_GubbinEditor.PreviewLight.Red, Program.GE.m_GubbinEditor.PreviewLight.Green, Program.GE.m_GubbinEditor.PreviewLight.Blue);

                foreach (LightFunction LF in LightFunctionList.Functions)
                {
                    if (LF.Name.Equals(Template.LightFunction, StringComparison.CurrentCultureIgnoreCase))
                    {
                        Program.GE.m_GubbinEditor.PreviewLight.Function = LF;
                        break;
                    }
                }

                Program.GE.m_GubbinEditor.PreviewLight.EN.Position(Template.Position.X, Template.Position.Y, Template.Position.Z);
                Program.GE.m_GubbinEditor.PreviewLight.MoverPivot.Position(Template.Position.X, Template.Position.Y, Template.Position.Z);
                Program.GE.m_GubbinEditor.PreviewLight.Handle.Position(Template.Position.X, Template.Position.Y, Template.Position.Z);

                Program.GE.m_GubbinEditor.PreviewLight.UpdateLines();

                Program.GE.m_GubbinEditor.Saved = false;
            }
        }

        [Category("Appearance"), Editor(typeof(RenderingServices.Vector3ColorEditor), typeof(System.Drawing.Design.UITypeEditor)), TypeConverter(typeof(RenderingServices.Vector3ColorOptionsConverter))]
        public RenderingServices.Vector3 LightColor
        {
            get
            {
                return Template.LightColor;
            }

            set
            {
                Template.LightColor = value;

                if (Program.GE.m_GubbinEditor.PreviewLight == null)
                    return;

                Program.GE.m_GubbinEditor.PreviewLight.Red = (byte)(Template.LightColor.X * 255.0f);
                Program.GE.m_GubbinEditor.PreviewLight.Green = (byte)(Template.LightColor.Y * 255.0f);
                Program.GE.m_GubbinEditor.PreviewLight.Blue = (byte)(Template.LightColor.Z * 255.0f);
                Program.GE.m_GubbinEditor.PreviewLight.Handle.Color(
                    Program.GE.m_GubbinEditor.PreviewLight.Red, Program.GE.m_GubbinEditor.PreviewLight.Green, Program.GE.m_GubbinEditor.PreviewLight.Blue);
                Program.GE.m_GubbinEditor.PreviewLight.UpdateLines();

                Program.GE.m_GubbinEditor.Saved = false;
            }
        }

        [CategoryAttribute("Appearance")]
        public float LightRadius
        {
            get
            {
                return Template.LightRadius;
            }
            set
            {
                Template.LightRadius = value;

                if (Program.GE.m_GubbinEditor.PreviewLight == null)
                    return;

                Program.GE.m_GubbinEditor.PreviewLight.Radius = Template.LightRadius;
                Program.GE.m_GubbinEditor.PreviewLight.Handle.Radius(Program.GE.m_GubbinEditor.PreviewLight.Radius);
                Program.GE.m_GubbinEditor.PreviewLight.UpdateLines();

                Program.GE.m_GubbinEditor.Saved = false;
            }
        }

        [CategoryAttribute("Appearance"), Editor(typeof(RealmCrafter.LightFunctionListEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public RealmCrafter.LightFunction LightFunction
        {
            get
            {
                foreach (LightFunction LF in LightFunctionList.Functions)
                {
                    if (LF.Name.Equals(Template.LightFunction, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return LF;
                    }
                }

                return null;
            }

            set
            {
                Template.LightFunction = value.Name;

                if (Program.GE.m_GubbinEditor.PreviewLight == null)
                    return;

                Program.GE.m_GubbinEditor.PreviewLight.Function = value;

                Program.GE.m_GubbinEditor.Saved = false;
            }
        }

        [Category("Appearance")]
        public GubbinAnimationType AnimationType
        {
            get
            {
                return Template.AnimationType;
            }
            set
            {
                Template.AnimationType = value;

                if (Program.GE.m_GubbinEditor.GubbinPreviewMesh != null)
                {
                    if (Template.AnimationType == GubbinAnimationType.Inherited)
                    {
                        RenderWrapper.bbdx2_ReExtractAnimSeq(Program.GE.m_GubbinEditor.GubbinPreviewMesh.Handle, Program.GE.m_GubbinEditor.GubbinPreviewSeq, Template.AnimationStartFrame, Template.AnimationEndFrame);
                        Program.GE.m_GubbinEditor.GubbinPreviewMesh.Animate(1, 1.0f, Program.GE.m_GubbinEditor.GubbinPreviewSeq);
                        Program.GE.m_GubbinEditor.GubbinPreviewMesh.Position(0, 0, 0);
                        Program.GE.m_GubbinEditor.GubbinPreviewMesh.Scale(1, 1, 1);
                        Program.GE.m_GubbinEditor.GubbinPreviewMesh.Rotate(0, 0, 0);
                        Program.GE.m_GubbinEditor.GubbinPreviewMesh.Parent(((Entity)Program.GE.m_GubbinEditor.ActorPreview.EN), false);
                        Program.GE.m_GubbinEditor.GubbinPreviewMesh.InheritAnimation = true;
                    }
                    else
                    {
                        Program.GE.m_GubbinEditor.GubbinPreviewMesh.Parent(((Entity)Program.GE.m_GubbinEditor.ActorPreview.EN).FindChild(Template.AssignedBoneName), false);
                        Program.GE.m_GubbinEditor.GubbinPreviewMesh.Position(Template.Position.X, Template.Position.Y, Template.Position.Z);
                        Program.GE.m_GubbinEditor.GubbinPreviewMesh.Scale(Template.Scale.X, Template.Scale.Y, Template.Scale.Z);
                        Program.GE.m_GubbinEditor.GubbinPreviewMesh.Rotate(Template.Rotation.X, Template.Rotation.Y, Template.Rotation.Z);
                        Program.GE.m_GubbinEditor.GubbinPreviewMesh.InheritAnimation = false;
                        RenderWrapper.bbdx2_ReExtractAnimSeq(Program.GE.m_GubbinEditor.GubbinPreviewMesh.Handle, Program.GE.m_GubbinEditor.GubbinPreviewSeq, Template.AnimationStartFrame, Template.AnimationEndFrame);
                        Program.GE.m_GubbinEditor.GubbinPreviewMesh.Animate(1, 1.0f, Program.GE.m_GubbinEditor.GubbinPreviewSeq);
                    }
                }

                Program.GE.m_GubbinEditor.Saved = false;
            }
        }

        [Category("Appearance")]
        public ushort AnimationStartFrame
        {
            get
            {
                return Template.AnimationStartFrame;
            }
            set
            {
                Template.AnimationStartFrame = value;

                if (Template.AnimationType == GubbinAnimationType.PreAnimated)
                {
                    if (Program.GE.m_GubbinEditor.GubbinPreviewMesh != null)
                    {
                        RenderWrapper.bbdx2_ReExtractAnimSeq(Program.GE.m_GubbinEditor.GubbinPreviewMesh.Handle, Program.GE.m_GubbinEditor.GubbinPreviewSeq, Template.AnimationStartFrame, Template.AnimationEndFrame);
                        Program.GE.m_GubbinEditor.GubbinPreviewMesh.Animate(1, 1.0f, Program.GE.m_GubbinEditor.GubbinPreviewSeq);
                    }
                }

                Program.GE.m_GubbinEditor.Saved = false;
            }
        }

        [Category("Appearance")]
        public ushort AnimationEndFrame
        {
            get
            {
                return Template.AnimationEndFrame;
            }
            set
            {
                Template.AnimationEndFrame = value;

                if (Template.AnimationType == GubbinAnimationType.PreAnimated)
                {
                    if (Program.GE.m_GubbinEditor.GubbinPreviewMesh != null)
                    {
                        RenderWrapper.bbdx2_ReExtractAnimSeq(Program.GE.m_GubbinEditor.GubbinPreviewMesh.Handle, Program.GE.m_GubbinEditor.GubbinPreviewSeq, Template.AnimationStartFrame, Template.AnimationEndFrame);
                        Program.GE.m_GubbinEditor.GubbinPreviewMesh.Animate(1, 1.0f, Program.GE.m_GubbinEditor.GubbinPreviewSeq);
                    }
                }

                Program.GE.m_GubbinEditor.Saved = false;
            }
        }

        [Category("Transform"), TypeConverter(typeof(RenderingServices.Vector3OptionsConverter))]
        public RenderingServices.Vector3 Position
        {
            get
            {
                return Template.Position;
            }
            set
            {
                Template.Position = (RenderingServices.Vector3)value.Clone();

                if (Program.GE.m_GubbinEditor.GubbinPreviewMesh != null)
                    Program.GE.m_GubbinEditor.GubbinPreviewMesh.Position(Template.Position.X, Template.Position.Y, Template.Position.Z);
//                 if (Program.GE.m_GubbinEditor.GubbinPreviewEmitterEN != null)
//                     Program.GE.m_GubbinEditor.GubbinPreviewEmitterEN.Position(Template.Position.X, Template.Position.Y, Template.Position.Z);

                if (Program.GE.m_GubbinEditor.PreviewLight != null)
                {
                    Program.GE.m_GubbinEditor.PreviewLight.EN.Position(Template.Position.X, Template.Position.Y, Template.Position.Z);
                    Program.GE.m_GubbinEditor.PreviewLight.MoverPivot.Position(Template.Position.X, Template.Position.Y, Template.Position.Z);
                    Program.GE.m_GubbinEditor.PreviewLight.Handle.Position(Template.Position.X, Template.Position.Y, Template.Position.Z);

                    Program.GE.m_GubbinEditor.PreviewLight.UpdateLines();
                }

                Program.GE.m_GubbinEditor.Saved = false;
            }
        }

        [Category("Transform"), TypeConverter(typeof(RenderingServices.Vector3OptionsConverter))]
        public RenderingServices.Vector3 Scale
        {
            get
            {
                return Template.Scale;
            }
            set
            {
                Template.Scale = (RenderingServices.Vector3)value.Clone();

                if (Program.GE.m_GubbinEditor.GubbinPreviewMesh != null)
                    Program.GE.m_GubbinEditor.GubbinPreviewMesh.Scale(Template.Scale.X, Template.Scale.Y, Template.Scale.Z);
//                 if (Program.GE.m_GubbinEditor.GubbinPreviewEmitterEN != null)
//                     Program.GE.m_GubbinEditor.GubbinPreviewEmitterEN.Scale(Template.Scale.X * 10.0f, Template.Scale.Y * 10.0f, Template.Scale.Z * 10.0f);

                Program.GE.m_GubbinEditor.Saved = false;
            }
        }

        [Category("Transform"), TypeConverter(typeof(RenderingServices.Vector3OptionsConverter))]
        public RenderingServices.Vector3 Rotation
        {
            get
            {
                return Template.Rotation;
            }
            set
            {
                Template.Rotation = (RenderingServices.Vector3)value.Clone();

                if (Program.GE.m_GubbinEditor.GubbinPreviewMesh != null)
                    Program.GE.m_GubbinEditor.GubbinPreviewMesh.Rotate(Template.Rotation.X, Template.Rotation.Y, Template.Rotation.Z);
//                 if (Program.GE.m_GubbinEditor.GubbinPreviewEmitterEN != null)
//                     Program.GE.m_GubbinEditor.GubbinPreviewEmitterEN.Rotate(Template.Rotation.X, Template.Rotation.Y, Template.Rotation.Z);

                Program.GE.m_GubbinEditor.Saved = false;
            }
        }

        //[Category("Behaviour")]
        //public bool RemoveHat

    }
}
