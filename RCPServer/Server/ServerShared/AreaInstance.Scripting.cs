using System;
using System.Collections.Generic;
using System.Text;

namespace RCPServer
{
    public partial class Area
    {
        partial class AreaInstance
        {
            public override string Name
            {
                get
                {
                    return Area.Name;
                }
            }

            public override int InstanceIndex
            {
                get
                {
                    return ID;
                }
            }

            public override LinkedList<Scripting.Actor> GetActors(Scripting.ActorType actorType, ushort minSectorX, ushort minSectorZ, ushort maxSectorX, ushort maxSectorZ)
            {
                LinkedList<Scripting.Actor> Out = new LinkedList<Scripting.Actor>();

                if (maxSectorX > Sectors.GetLength(0))
                    maxSectorX = (ushort)Sectors.GetLength(0);
                if (maxSectorZ > Sectors.GetLength(1))
                    maxSectorZ = (ushort)Sectors.GetLength(1);

                if (minSectorX > maxSectorX)
                    minSectorX = maxSectorX;
                if (minSectorZ > maxSectorZ)
                    minSectorZ = maxSectorZ;

                for (ushort z = minSectorZ; z < maxSectorZ; ++z)
                {
                    for (ushort x = minSectorX; x < maxSectorX; ++x)
                    {
                        if (actorType == Scripting.ActorType.Player || actorType == Scripting.ActorType.Any)
                        {
                            foreach (ActorInstance AI in Sectors[x, z].Players)
                                Out.AddLast(AI);
                        }

                        if (actorType == Scripting.ActorType.NonPlayer || actorType == Scripting.ActorType.Any)
                        {
                            foreach (ActorInstance AI in Sectors[x, z].Actors)
                                Out.AddLast(AI);
                        }
                    }
                }

                return Out;
            }
        }
    }
}
