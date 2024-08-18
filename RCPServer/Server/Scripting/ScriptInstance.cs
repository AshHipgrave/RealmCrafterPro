using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

namespace Scripting
{
    /// <summary>
    /// Handle of an script instance pre-execution.
    /// </summary>
    /// <remarks>
    /// <b>This class should not be used from any custom scripts.</b>
    /// </remarks>
    public class ScriptInstance
    {
        object Script;
        MethodInfo EntryPoint;

        /// <summary>
        /// Script instance constructor.
        /// </summary>
        /// <param name="script">Instantiated script object.</param>
        /// <param name="entrypoint">Entrypoint method of the script</param>
        public ScriptInstance(object script, MethodInfo entrypoint)
        {
            Script = script;
            EntryPoint = entrypoint;
        }

        /// <summary>
        /// Invokes a script entrypoint.
        /// </summary>
        /// <param name="arguments">Array of arguments to sent to the entrypoint</param>
        /// <param name="tag">Associative initial tag</param>
        /// <param name="tag2">Associative initial tag</param>
        public void Execute(object[] arguments, object tag, object tag2)
        {

            try
            {
                if ((Script as ScriptBase) != null)
                {
                    (Script as ScriptBase).Tag = tag;
                    (Script as ScriptBase).Tag2 = tag2;
                }

                EntryPoint.Invoke(Script, arguments);
            }
            catch (System.Reflection.TargetException e)
            {
                HandleArgumentException(e, arguments, tag, tag2);
            }
            catch (System.ArgumentException e)
            {
                HandleArgumentException(e, arguments, tag, tag2);
            }
            catch (System.Reflection.TargetParameterCountException e)
            {
                HandleArgumentException(e, arguments, tag, tag2);
            }
            catch (System.MethodAccessException e)
            {
                HandleArgumentException(e, arguments, tag, tag2);
            }
            catch (System.Exception e)
            {
                System.Diagnostics.StackTrace Trace = new System.Diagnostics.StackTrace((e.InnerException == null) ? e : e.InnerException, true);

                int FrameIndex = Trace.FrameCount - 1;

                string ExecutingPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                if (!ExecutingPath.Substring(ExecutingPath.Length - 1, 1).Equals("/")
                    && !ExecutingPath.Substring(ExecutingPath.Length - 1, 1).Equals("\\"))
                {
                    ExecutingPath += Path.PathSeparator;
                }

                string Filename = Trace.GetFrame(FrameIndex).GetFileName();
                int LineNumber = Trace.GetFrame(FrameIndex).GetFileLineNumber();
                int CollumnNumber = Trace.GetFrame(FrameIndex).GetFileColumnNumber();
                string Message = (e.InnerException == null) ? e.Message : e.InnerException.Message;

                if (Filename.Length > ExecutingPath.Length)
                    Filename = Filename.Substring(ExecutingPath.Length);

                ScriptExceptionArgs Args = new ScriptExceptionArgs(Filename, LineNumber, CollumnNumber, Message, tag, tag2);
                ScriptManager.HandledException(Args);

                RCScript.Log(String.Format("{0} ({1}, {2}): {3}", new object[] { Path.GetFileName(Filename), LineNumber, CollumnNumber, Message }));
            }
        }

        /// <summary>
        /// Used to catch the variety of parameter related exceptions thrown before the script exception catcher can do its work.
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="tag"></param>
        /// <param name="tag2"></param>
        private void HandleArgumentException(System.Exception e, object[] arguments, object tag, object tag2)
        {
            string Message = e.GetType().FullName + ": " + e.Message + " ScriptMethod Signature: (";

            Type[] Expected = EntryPoint.GetGenericArguments();
            for (int i = 0; i < Expected.Length; ++i)
            {
                Message += Expected[i].Name;
                if (i < Expected.Length - 1)
                    Message += ", ";
            }

            Message += ") Received: (";

            for (int i = 0; i < arguments.Length; ++i)
            {
                Message += (arguments[i] == null) ? "'null object, cannot be evaluated'" : arguments[i].GetType().Name;
                if (i < arguments.Length - 1)
                    Message += ", ";
            }

            Message += ")";

            // Pass exception up to master server
            ScriptExceptionArgs Args = new ScriptExceptionArgs(Script.GetType().FullName, 0, 0, Message, tag, tag2);
            ScriptManager.HandledException(Args);

            RCScript.Log(String.Format("{0} ({1}, {2}): {3}", new object[] { Script.GetType().FullName, 0, 0, Message }));
        }
    }
}
