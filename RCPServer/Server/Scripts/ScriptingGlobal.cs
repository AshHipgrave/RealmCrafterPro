using System;
using System.Collections.Generic;
using System.Text;
using Scripting;
using Scripting.Forms;

namespace Scripting
{
    /// <summary>
    /// Globally called functions on server init/shutdown
    /// </summary>
    public class ScriptingGlobal : ScriptBase
    {

        /// <summary>
        /// Method called when server starts
        /// </summary>
        public void OnStartup()
        {
            // Register event processors
            Control.RegisterFormProcessEventHandler(typeof(Form), new FormProcessEventHandler(Form.ProcessEvent));
            Control.RegisterFormProcessEventHandler(typeof(Button), new FormProcessEventHandler(Button.ProcessEvent));
            Control.RegisterFormProcessEventHandler(typeof(TextBox), new FormProcessEventHandler(TextBox.ProcessEvent));
            Control.RegisterFormProcessEventHandler(typeof(CheckBox), new FormProcessEventHandler(CheckBox.ProcessEvent));
            Control.RegisterFormProcessEventHandler(typeof(ComboBox), new FormProcessEventHandler(ComboBox.ProcessEvent));
            Control.RegisterFormProcessEventHandler(typeof(Label), new FormProcessEventHandler(Label.ProcessEvent));
            Control.RegisterFormProcessEventHandler(typeof(ListBox), new FormProcessEventHandler(ListBox.ProcessEvent));
            Control.RegisterFormProcessEventHandler(typeof(Panel), new FormProcessEventHandler(Panel.ProcessEvent));
            Control.RegisterFormProcessEventHandler(typeof(ProgressBar), new FormProcessEventHandler(Scripting.Forms.ProgressBar.ProcessEvent));
            Control.RegisterFormProcessEventHandler(typeof(TrackBar), new FormProcessEventHandler(TrackBar.ProcessEvent));
            Control.RegisterFormProcessEventHandler(typeof(ItemButton), new FormProcessEventHandler(ItemButton.ProcessEvent));

            // Completed
            RCScript.Log("Server Started");
        }

        /// <summary>
        /// Method called when server shuts down
        /// </summary>
        public void OnShutdown()
        {
        }
    }
}
