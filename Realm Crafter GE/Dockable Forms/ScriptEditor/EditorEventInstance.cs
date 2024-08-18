using System;
using System.Collections.Generic;
using System.Text;

namespace RealmCrafter_GE.Dockable_Forms.ScriptEditor
{
    public class EditorEventInstance
    {
        public EditorEvent EditorEvent;
        public string FuncName = "";

        public EditorEventInstance(EditorEvent editorEvent)
        {
            this.EditorEvent = editorEvent;
        }
    }
}
