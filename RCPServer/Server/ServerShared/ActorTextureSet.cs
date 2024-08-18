using System;
using System.Collections.Generic;
using System.Text;

namespace RCPServer
{
    public struct ActorTextureSet
    {
        public ushort Tex0;
        public ushort Tex1;
        public ushort Tex2;
        public ushort Tex3;

        public ActorTextureSet(ushort t0, ushort t1, ushort t2, ushort t3)
        {
            Tex0 = t0;
            Tex1 = t1;
            Tex2 = t2;
            Tex3 = t3;
        }
    }
}
