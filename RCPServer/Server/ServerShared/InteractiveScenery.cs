using System;
using System.Collections.Generic;
using System.Text;
using Scripting.Math;

namespace RCPServer
{
    public class InteractiveScenery : Scripting.Scenery
    {
        SectorVector position;
        ushort meshID;
        string scriptName;

        public InteractiveScenery(SectorVector inPosition, ushort inMeshID, string inScriptName)
        {
            position = inPosition;
            meshID = inMeshID;
            scriptName = inScriptName;
        }

        public override SectorVector Position
        {
            get { return position; }
        }

        public override ushort MeshID
        {
            get { return meshID; }
        }

        public override string ScriptName
        {
            get { return scriptName; }
        }
    }
}
