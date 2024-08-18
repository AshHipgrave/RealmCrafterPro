using System;
using System.Collections.Generic;
using System.Text;
using Scripting;

namespace UserScripts
{
    /// <summary>
    /// Enter description here
    /// </summary>
    public class InteractivityTest : ScriptBase
    {
        // Interact with scenery object
        public void OnInteract(Actor actor, SceneryInstance scenery)
        {
            // Validity check on globals
            if (!scenery.Globals.ContainsKey("Count") || scenery.Globals["Count"].Length == 0)
                scenery.Globals["Count"] = new byte[] { 0, 0, 0, 0 };

            // Read in count
            PacketReader Reader = new PacketReader(scenery.Globals["Count"]);
            int Count = Reader.ReadInt32();

            // Increment and output
            ++Count;
            actor.Output("Current Count: " + Count);

            // Store
            PacketWriter Writer = new PacketWriter();
            Writer.Write(Count);
            scenery.Globals["Count"] = Writer.ToArray();
        }
    }
}
