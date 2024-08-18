using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting
{
    /// <summary>
    /// Delete to capture script exceptions (server only).
    /// </summary>
    /// <param name="e"></param>
    public delegate void ScriptExceptionHandler(ScriptExceptionArgs e);

    /// <summary>
    /// Argument structure for handled script exceptions
    /// </summary>
    public class ScriptExceptionArgs
    {
        object tag1, tag2;
        string filename;
        int line, collumn;
        string message;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="line"></param>
        /// <param name="collumn"></param>
        /// <param name="message"></param>
        /// <param name="tag1"></param>
        /// <param name="tag2"></param>
        public ScriptExceptionArgs(string filename, int line, int collumn, string message, object tag1, object tag2)
        {
            this.filename = filename;
            this.line = line;
            this.collumn = collumn;
            this.message = message;
            this.tag1 = tag1;
            this.tag2 = tag2;
        }

        /// <summary>
        /// Primary Tag (Player Actor).
        /// </summary>
        public object Tag1
        {
            get { return tag1; }
            set { tag1 = value; }
        }

        /// <summary>
        /// Secondary Tag (Context Actor).
        /// </summary>
        public object Tag2
        {
            get { return tag2; }
            set { tag2 = value; }
        }

        /// <summary>
        /// Filename of the script which caused the exception.
        /// </summary>
        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }

        /// <summary>
        /// Line number of the exception fault.
        /// </summary>
        public int Line
        {
            get { return line; }
            set { line = value; }
        }

        /// <summary>
        /// Collumn number of the exception fault.
        /// </summary>
        public int Collumn
        {
            get { return collumn; }
            set { collumn = value; }
        }

        /// <summary>
        /// Exception message, cause of the fault.
        /// </summary>
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }
}
