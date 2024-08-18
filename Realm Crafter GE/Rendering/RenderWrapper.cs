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
using System.Runtime.InteropServices;
using System.Text;

namespace RenderingServices
{
    [StructLayout(LayoutKind.Sequential, Size = 24)]
    public class Matrix3x2 : ICloneable
    {
        public float m00, m01, m02;
        public float m10, m11, m12;

        public object Clone()
        {
            Matrix3x2 O = new Matrix3x2();

            O.m00 = m00;
            O.m01 = m01;
            O.m02 = m02;

            O.m10 = m10;
            O.m11 = m11;
            O.m12 = m12;

            return O;
        }

        public override string ToString()
        {
            return m00.ToString() + ", " + m01.ToString() + ", " + m02.ToString() + Environment.NewLine
                   + m10.ToString() + ", " + m11.ToString() + ", " + m12.ToString();
        }
    }

    [StructLayout(LayoutKind.Sequential, Size = 36)]
    public class Matrix3x3 : ICloneable
    {
        public float m00, m01, m02;
        public float m10, m11, m12;
        public float m20, m21, m22;

        public object Clone()
        {
            Matrix3x3 O = new Matrix3x3();

            O.m00 = m00;
            O.m01 = m01;
            O.m02 = m02;

            O.m10 = m10;
            O.m11 = m11;
            O.m12 = m12;

            O.m20 = m20;
            O.m21 = m21;
            O.m22 = m22;

            return O;
        }

        public override string ToString()
        {
            return m00.ToString() + ", " + m01.ToString() + ", " + m02.ToString() + Environment.NewLine
                   + m10.ToString() + ", " + m11.ToString() + ", " + m12.ToString() + Environment.NewLine
                   + m20.ToString() + ", " + m21.ToString() + ", " + m22.ToString();
        }
    }

    [StructLayout(LayoutKind.Sequential, Size = 64)]
    public class Matrix4x4 : ICloneable
    {
        public float m00, m01, m02, m03;
        public float m10, m11, m12, m13;
        public float m20, m21, m22, m23;
        public float m30, m31, m32, m33;

        public object Clone()
        {
            Matrix4x4 O = new Matrix4x4();

            O.m00 = m00;
            O.m01 = m01;
            O.m02 = m02;
            O.m03 = m03;

            O.m10 = m10;
            O.m11 = m11;
            O.m12 = m12;
            O.m13 = m13;

            O.m20 = m20;
            O.m21 = m21;
            O.m22 = m22;
            O.m23 = m23;

            O.m30 = m30;
            O.m31 = m31;
            O.m32 = m32;
            O.m33 = m33;

            return O;
        }

        public override string ToString()
        {
            return m00.ToString() + ", " + m01.ToString() + ", " + m02.ToString() + ", " + m03.ToString() +
                   Environment.NewLine
                   + m10.ToString() + ", " + m11.ToString() + ", " + m12.ToString() + ", " + m13.ToString() +
                   Environment.NewLine
                   + m20.ToString() + ", " + m21.ToString() + ", " + m22.ToString() + ", " + m23.ToString() +
                   Environment.NewLine
                   + m30.ToString() + ", " + m31.ToString() + ", " + m32.ToString() + ", " + m33.ToString();
        }
    }

    internal static class RenderWrapper
    {
        public enum ShaderType
        {
            ST_Void,
            ST_Bool,
            ST_Int,
            ST_Float,
            ST_Float2,
            ST_Float3,
            ST_Float4,
            ST_Float3x2,
            ST_Float3x3,
            ST_Float4x4,
            ST_String,
            ST_Unknown
        } ;

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int GetBBDXCommandData();

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void ghoop(IntPtr hwnd, uint hinstance);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern void ackno(int width, int height, int depth, int mode, int aa, int anis,
                                        string defaulttex);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern uint rendering(uint obj, string name);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void lickme();

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void qwedfy(int r, int g, int b);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int copliy();

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int boompx();

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void lovepixels(int light, float x, float y, float z);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void gabbama(int light, float r);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void sukusul(int light, float x, float y, float z);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void hoklig(int light, int r, int g, int b);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void gonerum(int light, int r, int g, int b);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void wareflog(int light, int enable);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void klopil(int light, int enable);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void lipphogg(int light);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void jewnjig(int light);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void gisranlo(uint obj, int mode, float speed, int sequence);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int growth(uint obj, int start, int end);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void jockgnome(uint what);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint burryjimpol(uint parent);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint jalkming(string filename, uint parent, int Animated, int cb = 0, int ud = 0);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint pathlier(uint parent);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint pandamaka(int GridSize, uint parent);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void laggysven(uint obj, int x, int z, float height);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void tikamakis(uint obj);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int jacklingmo(uint obj);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void eveclassic(uint obj);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float trexban(uint obj, int x, int z);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint carjacking(uint parent);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void chaosdigs(uint what, float x, float y, float z, bool global);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void rzrtool(uint what, float x, float y, float z, bool global);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void gobstoper(uint what, float x, float y, float z);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void mingja(uint what, float x, float y, float z, bool global);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void moolad(uint obj, float x, float y, float z, bool global);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void jaffamak(uint obj, float x, float y, float z, bool global);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float nottdos(uint obj, uint obj2);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void babygoo(uint obj, uint target, float roll);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void chincrank(uint obj, int order);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void laghimout();

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void knowham(uint cam, int r, int g, int b);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void getmeshake(uint obj);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void localiva(uint obj, int fx);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void missedit(uint obj, float alpha);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void makewow(uint obj, float alpha);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern uint deterkis(string filename);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern void bbdx2_SaveLastFrame(string path);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void bbdx2_CameraClsColorAlpha(uint cam, int r, int g, int b, int a);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void slobing(uint obj, uint shader);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void bumofcow(uint obj);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void fluxcapa(uint obj);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint catonbox(uint obj, uint parent);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float jingsu(uint obj, bool global);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float sostrong(uint obj, bool global);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float kisheadgone(uint obj, bool global);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float tcpwnsall(uint obj, bool global);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float habaki(uint obj, bool global);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float kimono(uint obj, bool global);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float kissme(uint obj, bool global);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float lolatme(uint obj, bool global);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float lambdin(uint obj, bool global);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float chinook(uint obj, float vx, float vy, float vz, int axis);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float manonworld(uint obj);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float jumpin(uint obj);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float needsleep(uint obj);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "setthefire")]
        private static extern IntPtr _setthefire(uint obj);

        public static string setthefire(uint obj)
        {
            return Marshal.PtrToStringAnsi(_setthefire(obj));
        }

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void jackhammer(uint obj, int mode);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void softskin(uint obj, float x, float y, float z);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void setmedown(uint obj, float x, float y, float z);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void soreneck(uint obj, float x, float y, float z);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void reachout(uint obj, float x, float y, float z, float w, float h, float d, bool uniform);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint andtakeit(uint obj);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int wannabe(uint surf);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int withoutyou(uint surf, float x, float y, float z, float u, float v, float w);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int tellme(uint surf, int v0, int v1, int v2);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int openeyes(uint obj);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint getupgetout(uint obj, int number);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int aboveandbeyond(uint surf);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int mindless(uint obj);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void closeeyes(uint Node, uint NodeLOD, int iLOD, float Distance);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint penondesk(uint obj, int index);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint earthiswarm(uint obj);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void crystalclear(uint obj, uint parent, bool global);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float laptop(uint surf, int index);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float watchingspace(uint surf, int index);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float garagedoor(uint surf, int index);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float VertexU(uint surf, int index);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float VertexV(uint surf, int index);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int CountIndices(uint surf);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int GetIndex(uint surf, int index);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void waverider(uint surf, int index, float x, float y, float z);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void happyclap(uint surf, int index, float u, float v);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void urallnoobs(uint surf, int index, int r, int g, int b, float a);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void makemegame(uint obj, float min, float max);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint texttodate(string filename, int flags);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void smokinhot(uint obj, uint texture, int frame, int index, int surface);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void emokid(uint texture);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint jesusownzall(uint obj, uint shader);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void hardrock(uint obj, float x, float y, float w, float h);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void makemav(uint obj, int r, int g, int b);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void shogun(uint obj, float a);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void nofear(uint obj, int mode);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void kevlarboobs(uint obj, float min, float max);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void goodoldkis(uint obj, int r, int g, int b);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void kickleaves(uint obj, int r, int g, int b);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void helplost(float x, float y, float z, uint sourceobj, uint destinationobj);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void kieransan(float x, float y, float z, uint sourceobj, uint destinationobj);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float itsnoteasy();

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float buymebeer();

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float firstdance();

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern void commands(uint obj, string name);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "likeyoumean")]
        private static extern IntPtr _likeyoumean(uint obj);

        public static string likeyoumean(uint obj)
        {
            return Marshal.PtrToStringAnsi(_likeyoumean(obj));
        }

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void bbdx2_Collisions(int source, int destination);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void bbdx2_EntityType(uint node, int type);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int bbdx2_GetEntityType(uint node);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void bbdx2_FreeCollisionInstance(uint node);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void bbdx2_ASyncInjectCollisionMesh(uint node, IntPtr triangleList, uint vertexCount, int highPriority);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void bbdx2_ASyncCancelInject(uint node);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void bbdx2_InjectCollisionMesh(uint node, IntPtr triangleList, uint vertexCount);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void bbdx2_EntityRadius(uint node, float x, float y);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void bbdx2_ResetEntity(uint node);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void bbdx2_EntityBox(uint node, float x, float y, float z, float w, float h, float d);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void bbdx2_ActorMove(uint node);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void bbdx2_UpdateWorld();

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void bbdx2_SetCollisionMesh(uint node);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void bbdx2_EntityPickMode(uint node, int picktype);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint bbdx2_LinePick(float startX, float startY, float startZ, float endX, float endY, float endZ);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint bbdx2_CameraPick(int x, int y);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float bbdx2_PickedX();

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float bbdx2_PickedY();
        
        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float bbdx2_PickedZ();

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float bbdx2_PickedNX();

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float bbdx2_PickedNY();

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float bbdx2_PickedNZ();

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int bbdx2_CountCollisions(uint node);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float bbdx2_CollisionX(uint node, int index);
        
        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float bbdx2_CollisionY(uint node, int index);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float bbdx2_CollisionZ(uint node, int index);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float bbdx2_CollisionNX(uint node, int index);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float bbdx2_CollisionNY(uint node, int index);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float bbdx2_CollisionNZ(uint node, int index);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void bbdx2_ManagedProjectVector3(float x, float y, float z);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void bbdx2_ManagedUnProjectVector3(float x, float y, float z);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float bbdx2_ProjectedX();

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float bbdx2_ProjectedY();

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float bbdx2_ProjectedZ();

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int bbdx2_RayToAABB(float minx, float miny, float minz, float maxx, float maxy, float maxz, float sx, float sy, float sz, float ex, float ey, float ez);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int bbdx2_RayToPlane(float nx, float ny, float nz, float px, float py, float pz, float sx, float sy, float sz, float ex, float ey, float ez);

//        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
//        public static extern void anyonebutme(int sourcetype, int desttype, int method, int response);

        //[DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        //public static extern void ladygarden(uint obj, byte coltype);

        //[DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        //public static extern int bedtime(uint obj);

        //[DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        //public static extern void perfecteyes(uint obj, float xr, float yr);

        //[DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        //public static extern void endless(uint obj);

        //[DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        //public static extern void goingon(uint obj);

        //[DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        //public static extern int awesome(uint obj);

        //[DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        //public static extern float mindmusic(uint obj, int index);

        //[DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        //public static extern float magicphone(uint obj, int index);

        //[DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        //public static extern float crapcake(uint obj, int index);

        //[DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        //public static extern float denote(uint obj, int index);

        //[DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        //public static extern float pure(uint obj, int index);

        //[DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        //public static extern float tubeloop(uint obj, int index);

        //[DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        //public static extern void picket(uint obj, int mode);

        //[DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        //public static extern uint fighting(float x, float y, float z, float dx, float dy, float dz, float radius);

        //[DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        //public static extern uint mgscool(uint camera, int x, int y);

        //[DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        //public static extern float neverland();

        //[DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        //public static extern float loadgays();

        //[DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        //public static extern float remindyou();

        //[DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        //public static extern float chocolate();

        //[DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        //public static extern float areyou();

        //[DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        //public static extern float gingerbread();

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void anyoneelse();

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void preening(uint tex, float x, float y);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint pawnbroker(uint tex);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint GrabTexture(uint tex);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetAlphaState(uint obj, bool Mode);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern bool GetAlphaState(uint obj);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "TexNameFromSurf")]
        private static extern IntPtr _TexNameFromSurf(uint obj, int index, int surf);

        public static string TexNameFromSurf(uint obj, int index, int surf)
        {
            return Marshal.PtrToStringAnsi(_TexNameFromSurf(obj, index, surf));
        }

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint CreateLine(uint parent);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void FreeLine(uint line);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetLineSize(uint line, float sx, float sy, float sz, float ex, float ey, float ez);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetLineColor(uint line, int r, int g, int b);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetLineVisible(uint line, bool visible);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern bool EntityInView(uint obj);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void EntityProject(uint obj);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void PointProject(float x, float y, float z);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float ProjectedX();

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float ProjectedY();

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float ProjectedZ();

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void apples(byte[] Filename);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void pears(byte[] Filename);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr bbdx2_GetIDirect3DDevice9();

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetRenderGUICallback(int index, IntPtr Callback);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetDeviceLostCallback(int index, IntPtr Callback);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetDeviceResetCallback(int index, IntPtr Callback);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetDeviceResetXYCallback(int index, IntPtr Callback);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetRenderSolidCallback(int index, IntPtr Callback);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetRenderSolidCallbackRT(int index, IntPtr Callback);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetRenderShadowDepthCallback(int index, IntPtr Callback);

         

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint CreateProfile(string Filename);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void FreeProfile(uint Profile);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetProfileRange(uint Profile, float Range);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float GetProfileRange(uint Profile);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetProfileEffect(uint Profile, int QualityLevel, int Distance, uint Effect);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint GetProfileEffect(uint Profile, int QualityLevel, int Distance);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void EntityProfile(uint Entity, uint Profile);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetQualityLevel(int QualityLevel);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int GetQualityLevel();

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr GetLightMatrix();

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr GetShadowMap();

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void EntityShadowShader(uint node, uint shader);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void EntityShadowLevel(uint entity, int level);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint GetEntityShadowShader(uint node);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void ShadowShader(uint Shader);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void ShadowBlurShader(uint blurH, uint blurV);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void ShadowDistance(float Distance);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void LightDistance (float Distance);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void ShadowLevel (int Level);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetShadowMapSize (int newSize);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void cones(uint Entity, string tag);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl, EntryPoint = "ocones", CharSet = CharSet.Ansi)]
        public static extern IntPtr _ocones(uint Entity);

        public static string ocones(uint Entity)
        {
            return Marshal.PtrToStringAnsi(_ocones(Entity));
        }

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float bbdx2_GetTransformedBoundingBoxMinX(uint entity);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float bbdx2_GetTransformedBoundingBoxMinY(uint entity);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float bbdx2_GetTransformedBoundingBoxMinZ(uint entity);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float bbdx2_GetTransformedBoundingBoxMaxX(uint entity);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float bbdx2_GetTransformedBoundingBoxMaxY(uint entity);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float bbdx2_GetTransformedBoundingBoxMaxZ(uint entity);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void EntityConstantFloat(uint Entity, string Constant, float vX);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void EntityConstantFloat2(uint Entity, string Constant, float vX, float vY);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void EntityConstantFloat3(uint Entity, string Constant, float vX, float vY, float vZ);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void EntityConstantFloat4(uint Entity, string Constant, float vX, float vY, float vZ,
                                                       float vW);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void bbdx2_SetInheritAnimation(uint entity, int inherit);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int bbdx2_GetInheritAnimation(uint entity);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern uint bbdx2_CreatePhysicsDesc(int isStatic, uint node);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void bbdx2_AddSphere(uint desc, float x, float y, float z, float radius, float mass);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void bbdx2_AddCapsule(uint desc, float x, float y, float z, float width, float height, float mass);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void bbdx2_AddBox(uint desc, float x, float y, float z, float width, float height, float depth, float mass);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void bbdx2_ClosePhysicsDesc(uint desc);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int CalculateB3DTangents(uint Entity);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int GetParameterCount(uint Effect);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "GetParameterName")]
        public static extern IntPtr _GetParameterName(uint Effect, int ID);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "GetAnnotationName")]
        public static extern IntPtr _GetAnnotationName(uint Effect, int eID, int aID);

        public static string GetParameterName(uint Effect, int ID)
        {
            return Marshal.PtrToStringAnsi(_GetParameterName(Effect, ID));
        }

        public static string GetAnnotationName(uint Effect, int eID, int aID)
        {
            return Marshal.PtrToStringAnsi(_GetAnnotationName(Effect, eID, aID));
        }

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int GetParameterType(uint Effect, int ID);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int GetAnnotationCount(uint Effect, int ID);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int GetAnnotationType(uint Effect, int eID, int aID);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int GetSizeOfType(int Type);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int GetParameterData(uint Effect, int ID, IntPtr Data);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int GetAnnotationData(uint Effect, int eID, int aID, IntPtr Data);

        public static object GetAnnotationValue(uint Effect, int eID, int aID)
        {
            int Type = GetAnnotationType(Effect, eID, aID);
            if (Type == 10) // A string
                return null;
            int Size = GetSizeOfType(Type);

            IntPtr Data = Marshal.AllocHGlobal(Size);
            if (GetAnnotationData(Effect, eID, aID, Data) == 0)
                return null;

            object Out = GetObjectFromType(Type, Data);

            Marshal.FreeHGlobal(Data);
            return Out;
        }

        public static object GetParameterValue(uint Effect, int eID)
        {
            int Type = GetParameterType(Effect, eID);
            if (Type == 10)
                return null;

            int Size = GetSizeOfType(Type);

            IntPtr Data = Marshal.AllocHGlobal(Size);
            GetParameterData(Effect, eID, Data);

            object Out = GetObjectFromType(Type, Data);

            Marshal.FreeHGlobal(Data);
            return Out;
        }

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr GetNodeParameterValue(uint Node, string ParamName, out int outType);

        public static object GetEntityParameterValue(uint Node, string ParamName)
        {
            int Type;
            IntPtr Data = GetNodeParameterValue(Node, ParamName, out Type);

            object Out = GetObjectFromType(Type, Data);
            return Out;
        }

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int GetNodeParameterCount(uint Node);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "GetNodeParameterName")]
        public static extern IntPtr _GetNodeParameterName(uint Node, int iParam);

        public static string GetNodeParameterName(uint Node, int iParam)
        {
            return Marshal.PtrToStringAnsi(_GetNodeParameterName(Node, iParam));
        }

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void ResetNodeParameters(uint Node);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr bbdx2_GetViewMatrixPtr();

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr bbdx2_GetProjectionMatrixPtr();

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void bbdx2_FreeMatrixPtr(IntPtr Ptr);

        [DllImport("d3dx9_40.dll")]
        public static extern IntPtr D3DXMatrixMultiply(IntPtr Out, IntPtr A, IntPtr B);

        [DllImport("d3dx9_40.dll")]
        public static extern IntPtr D3DXMatrixRotationYawPitchRoll(IntPtr Out, float Yaw, float Pitch, float Roll);

        [DllImport("d3dx9_40.dll")]
        public static extern IntPtr D3DXMatrixInverse(IntPtr Out, float[] Determinant, IntPtr Src);

        [DllImport("d3dx9_40.dll")]
        public static extern IntPtr D3DXVec3Transform(IntPtr Out, float[] Vector, IntPtr Mat);


        [StructLayout(LayoutKind.Sequential, Size = 28)]
        public class SUnmanagedToManagedTexture : ICloneable
        {
            public int Loaded;
            public IntPtr Texture;
            public IntPtr LockedBits;
            public uint Pitch;
            public uint Width;
            public uint Height;
            public int Format;

            public object Clone()
            {
                SUnmanagedToManagedTexture O = new SUnmanagedToManagedTexture();
                O.Loaded = Loaded;
                O.Texture = Texture;
                O.LockedBits = LockedBits;
                O.Pitch = Pitch;
                O.Width = Width;
                O.Height = Height;
                O.Format = Format;

                return O;
            }
        }

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void bbdx2_LoadTextureBits(string path, IntPtr data);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void bbdx2_FreeTextureBits(IntPtr data);

        public static System.Drawing.Bitmap LoadTextureBitmap(string path)
        {
            SUnmanagedToManagedTexture O = new SUnmanagedToManagedTexture();

            int Size = Marshal.SizeOf(typeof(SUnmanagedToManagedTexture));
            IntPtr Ptr = Marshal.AllocHGlobal(Size);
            //Marshal.StructureToPtr(O, Ptr, false);

            bbdx2_LoadTextureBits(path, Ptr);

            O = Marshal.PtrToStructure(Ptr, typeof(SUnmanagedToManagedTexture)) as SUnmanagedToManagedTexture;

            if (O.Loaded == 0)
                return null;



            System.Drawing.Bitmap TempBitmap = new System.Drawing.Bitmap(
                Convert.ToInt32(O.Width),
                Convert.ToInt32(O.Height),
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            System.Drawing.Imaging.BitmapData MapInfo = TempBitmap.LockBits(
                new System.Drawing.Rectangle(System.Drawing.Point.Empty, TempBitmap.Size),
                System.Drawing.Imaging.ImageLockMode.WriteOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            int MapLength = MapInfo.Stride * TempBitmap.Height;
            int TexLength = Convert.ToInt32(O.Pitch * O.Height);

            byte[] MapData = new byte[MapLength];
            byte[] TexData = new byte[TexLength];

            Marshal.Copy(MapInfo.Scan0, MapData, 0, MapLength);
            Marshal.Copy(O.LockedBits, TexData, 0, TexLength);

            for (int i = 0; i < MapData.Length; ++i)
                MapData[i] = TexData[i];

            Marshal.Copy(MapData, 0, MapInfo.Scan0, MapLength);
            TempBitmap.UnlockBits(MapInfo);

            bbdx2_FreeTextureBits(Ptr);
            Marshal.FreeHGlobal(Ptr);

            return TempBitmap;
        }

        public static object GetObjectFromType(int Type, IntPtr Data)
        {
            object Out = "Type not usable";

            switch (Type)
            {
                case 3:
                    Out = Marshal.PtrToStructure(Data, typeof(Vector1));
                    break;
                case 4:
                    Out = Marshal.PtrToStructure(Data, typeof(Vector2));
                    break;
                case 5:
                    Out = Marshal.PtrToStructure(Data, typeof(Vector3));
                    break;
                case 6:
                    Out = Marshal.PtrToStructure(Data, typeof(Vector4));
                    break;
                case 7:
                    Out = Marshal.PtrToStructure(Data, typeof(Matrix3x2));
                    break;
                case 8:
                    Out = Marshal.PtrToStructure(Data, typeof(Matrix3x3));
                    break;
                case 9:
                    Out = Marshal.PtrToStructure(Data, typeof(Matrix4x4));
                    break;
                case 10:
                    Out = Marshal.PtrToStringAnsi(Data);
                    break;
            }
            ;

            return Out;
        }
        
    //////////////////////////////////////////////////////////////////////////
    // Post Process Imported Functions
    //////////////////////////////////////////////////////////////////////////
        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddPP_Effect(string EffectName, string ShaderSource);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void DeletePP_Effect(int IndexPP);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SwapPP_Effect(int indexPP0, int indexPP1);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AddEffect_Param(string Effect, string Param, string Type, string Value);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetEffect_Param(string Effect, string Param, string Value);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "GetActivePP_Effects")]
        private static extern IntPtr _GetActivePP_Effects();

        public static string GetActivePP_Effects()
        {
            return Marshal.PtrToStringAnsi(_GetActivePP_Effects());
        }

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void LoadUserDefinedPP_FromXML(string path);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetPP_Pipeline(string name);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void EnablePP_Pipeline(bool state);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void CleanPP_Pipeline();

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void bbdx2_GetDirectionalLights(IntPtr Directions, IntPtr Colors);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr bbdx2_GetPointLights(IntPtr Count, IntPtr Stride);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float bbdx2_GetFogNear();

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int bbdx2_GetAmbientR();

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int bbdx2_GetAmbientG();

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int bbdx2_GetAmbientB();

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern float bbdx2_GetFogFar();

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int bbdx2_GetFogColor();

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void bbdx2_ReExtractAnimSeq(uint Entity, int Sequence, int StartFrame, int EndFrame);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void SetWaterLevel( float WaterHeight );

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int StartRemoteDebugging(string address, int port);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void bbdx2_SetEntityRenderMask(uint node, int mask);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int bbdx2_GetEntityRenderMask(uint node);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void bbdx2_SetRenderMask(int mask);

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern int bbdx2_GetRenderMask();

        [DllImport("BBDX.dll", CallingConvention=CallingConvention.Cdecl)]
        public static extern void bbdx2_ReloadShaders();
    }
}
