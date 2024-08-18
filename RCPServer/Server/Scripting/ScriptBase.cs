using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace Scripting
{
    /// <summary>
    /// Just a stored event
    /// </summary>
    class StoredEvent
    {
        public Delegate Handler;
        public System.Reflection.EventInfo Event;
        public object Target;
    }

    /// <summary>
    /// Base class of all scripts, handles events
    /// </summary>
    public class ScriptBase
    {
        /// <summary>
        /// All open scripts (ones which rely on callbacks)
        /// </summary>
        protected static List<ScriptBase> ActiveScripts = new List<ScriptBase>();

        /// <summary>
        /// Get the number of active scripts waiting for callbacks
        /// </summary>
        /// <returns></returns>
        public static int GetActiveScriptsCount()
        {
            return ActiveScripts.Count;
        }

        /// <summary>
        /// Saves and closes all scripts that match searchTag.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="searchTag"></param>
        /// <param name="isZoning"></param>
        public static void SavePersistentInstances(ref PacketWriter writer, object searchTag, bool isZoning)
        {
            // We are in charge of how they are written to the stream, so we'll create a writer for each
            // instance and then append its details.

            // Copy ActiveScripts list... its going to be edited while we're working with it
            ScriptBase[] Instances = new ScriptBase[ActiveScripts.Count];
            ActiveScripts.CopyTo(Instances);

            for (int i = 0; i < Instances.Length; ++i)
            {
                // Save this instance
                if (Instances[i].Tag == searchTag)
                {
                    if (Instances[i].IsPersistent(isZoning))
                    {
                        PacketWriter TWriter = new PacketWriter();
                        Instances[i].SaveState(TWriter);

                        writer.Write(Instances[i].GetType().Name, true);
                        writer.Write(TWriter.Length);
                        writer.Write(TWriter.ToArray(), 0, TWriter.Length);
                    }

                    Instances[i].ForceClose();
                }
            }
            
        }

        /// <summary>
        /// Load a range of saved script states from a binary data stream.
        /// </summary>
        /// <param name="reader">Object containing saved instance data.</param>
        /// <param name="tag">Associative tag.</param>
        public static void LoadPersistentInstance(ref PacketReader reader, object tag)
        {
            while (reader.Location < reader.Length - 1)
            {
                string Name = reader.ReadString();
                int DataLength = reader.ReadInt32();
                byte[] DataBytes = reader.ReadBytes(DataLength);
                PacketReader TReader = new PacketReader(DataBytes);

                if (!ScriptManager.Execute(Name, "LoadState", TReader, tag, null))
                    Console.WriteLine("Fail");
            }
        }

        /// <summary>
        /// For all active/idle scripts to close if their tag matches the search object.
        /// </summary>
        /// <param name="searchTag"></param>
        public static void ForceClose(object searchTag)
        {
            // Copy ActiveScripts list... its going to be edited while we're working with it
            ScriptBase[] Instances = new ScriptBase[ActiveScripts.Count];
            ActiveScripts.CopyTo(Instances);

            for (int i = 0; i < Instances.Length; ++i)
            {
                // Save this instance
                if (Instances[i].Tag == searchTag)
                {
                    Instances[i].ClearCallbacks();
                    Instances[i].ForceClose();
                }
            }
        }

        /// <summary>
        /// Find a script instance from the given associative tags.
        /// </summary>
        /// <param name="tag1"></param>
        /// <param name="tag2"></param>
        /// <returns></returns>
        public static ScriptBase FindScriptFromTags(object tag1, object tag2)
        {
            foreach (ScriptBase Instance in ActiveScripts)
            {
                if (Instance.Tag == tag1 && Instance.Tag2 == tag2)
                    return Instance;
            }

            return null;
        }


        /// <summary>
        /// Stored delegates/callbacks for this script
        /// </summary>
        private List<StoredEvent> StoredEvents = new List<StoredEvent>();

        /// <summary>
        /// Number of stored events (used to track itself in ActiveScripts)
        /// </summary>
        private int Registered = 0;

        /// <summary>
        /// Tagging object
        /// </summary>
        private object TagObject = null;
        private object TagObject2 = null;

        /// <summary>
        /// Gets or Sets a Tag
        /// </summary>
        public object Tag
        {
            get { return TagObject; }
            set { TagObject = value; }
        }

        /// <summary>
        /// Gets or Sets a Second Tag
        /// </summary>
        public object Tag2
        {
            get { return TagObject2; }
            set { TagObject2 = value; }
        }

        /// <summary>
        /// Gets whether this script is persistent (saveable with the actor). This is false
        /// by default, override to set it to true.
        /// </summary>
        /// <returns>True is script instance must be saved</returns>
        public virtual bool IsPersistent(bool isZoning)
        {
            return false;
        }

        /// <summary>
        /// Save this script state to a data stream. Only works on a persistent scripts.
        /// </summary>
        /// <param name="packetWriter">Data object to write to</param>
        public virtual void SaveState(PacketWriter packetWriter)
        {

        }

        /// <summary>
        /// Load this script state from a data stream. Only works on a persistent script.
        /// </summary>
        /// <param name="packetReader">Data object to read from</param>
        public virtual void LoadState(PacketReader packetReader)
        {

        }

        /// <summary>
        /// This method is called when an associated object is removed (ie. an actor goes offline).
        /// </summary>
        public virtual void ForceClose()
        {

        }

        /// <summary>
        /// Register a callback for use on this script
        /// </summary>
        /// <param name="o">Object which owns the event.</param>
        /// <param name="ev">String name which represents the event.</param>
        /// <param name="handler">EventHandler Callback.</param>
        public void RegisterCallback(object o, string ev, Delegate handler)
        {
            try
            {
                // Get the event and assign callback
                System.Reflection.EventInfo Info = o.GetType().GetEvent(ev);
                Info.AddEventHandler(o, handler);


                // Create a stored event
                StoredEvent E = new StoredEvent();
                E.Handler = handler;
                E.Event = Info;
                E.Target = o;

                // Remember it
                StoredEvents.Add(E);

                if(Registered == 0)
                    ActiveScripts.Add(this);
                ++Registered;
            }
            catch (System.Exception e) // Internal exception!!
            {
                System.Diagnostics.StackTrace Trace = new System.Diagnostics.StackTrace(e, true);

                int FrameIndex = Trace.FrameCount - 1;

                string Filename = System.IO.Path.GetFileName(Trace.GetFrame(FrameIndex).GetFileName());
                string LineNumber = Trace.GetFrame(FrameIndex).GetFileLineNumber().ToString();
                string CollumnNumber = Trace.GetFrame(FrameIndex).GetFileColumnNumber().ToString();

                Console.WriteLine(String.Format("{0} ({1}, {2}): {3}", new string[] { Filename, LineNumber, CollumnNumber, e.Message }));
            }
        }

        /// <summary>
        /// UnRegister a callback thats in use on this script
        /// </summary>
        /// <param name="o">Object which owns the event.</param>
        /// <param name="ev">String name which represents the event.</param>
        /// <param name="handler">EventHandler Callback.</param>
        public void UnRegisterCallback(object o, string ev, Delegate handler)
        {
            try
            {
                // Get the event and assign callback
                System.Reflection.EventInfo Info = o.GetType().GetEvent(ev);
                Info.RemoveEventHandler(o, handler);

                List<StoredEvent> Removals = new List<StoredEvent>();
                foreach (StoredEvent E in StoredEvents)
                {
                    if (E.Handler == handler && E.Event == Info)
                        Removals.Add(E);
                }
                foreach (StoredEvent E in Removals)
                {
                    StoredEvents.Remove(E);
                }
                Removals.Clear();

                --Registered;
                if (Registered == 0)
                    ActiveScripts.Remove(this);
            }
            catch (System.Exception e) // Internal exception!!
            {
                System.Diagnostics.StackTrace Trace = new System.Diagnostics.StackTrace(e, true);

                int FrameIndex = Trace.FrameCount - 1;

                string Filename = System.IO.Path.GetFileName(Trace.GetFrame(FrameIndex).GetFileName());
                string LineNumber = Trace.GetFrame(FrameIndex).GetFileLineNumber().ToString();
                string CollumnNumber = Trace.GetFrame(FrameIndex).GetFileColumnNumber().ToString();

                Console.WriteLine(String.Format("{0} ({1}, {2}): {3}", new string[] { Filename, LineNumber, CollumnNumber, e.Message }));
            }
        }

        /// <summary>
        /// Remove all callback associated with this script
        /// </summary>
        public void ClearCallbacks()
        {
            foreach (StoredEvent E in StoredEvents)
            {
                E.Event.RemoveEventHandler(E.Target, E.Handler);
            }

            StoredEvents.Clear();

            Registered = 0;
            ActiveScripts.Remove(this);
        }
    }
}
