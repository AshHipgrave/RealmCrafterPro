using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.CSharp;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Reflection.Emit;
using Environment = System.Environment;

namespace Scripting
{
    /// <summary>
    ///
    /// </summary>
    public class ScriptManager
    {
        private sealed class DelayedScriptInstance
        {
            public ScriptInstance Instance;
            public object[] Arguments;
            public object Tag;
            public object Tag2;

            public DelayedScriptInstance(ScriptInstance instance, object[] arguments, object tag, object tag2)
            {
                Instance = instance;
                Arguments = arguments;
                Tag = tag;
                Tag2 = tag2;
            }
        }

        private static LinkedList<DelayedScriptInstance> Delayed = new LinkedList<DelayedScriptInstance>();
        private static List<Type> Scripts = new List<Type>();
        private static List<Type> SpecialScripts = new List<Type>();
        private static List<Type> AllTypes = new List<Type>();

        /// <summary>
        /// Script Exception Handler
        /// </summary>
        public static event ScriptExceptionHandler ExceptionCaught;

        /// <summary>
        /// Trigger the ExceptionCaught even when an exception is caught.
        /// </summary>
        /// <param name="args"></param>
        public static void HandledException(ScriptExceptionArgs args)
        {
            if (ExceptionCaught != null)
                ExceptionCaught.Invoke(args);
        }

        private static string[] GetFiles(string path, string search)
        {
            List<string> Files = new List<string>();

            string[] Dirs = Directory.GetDirectories(path);
            foreach (string Dir in Dirs)
                Files.AddRange(GetFiles(Dir, search));

            Files.AddRange(Directory.GetFiles(path, search));

            return Files.ToArray();
        }

        /// <summary>
        /// Compile and store all the valid script files inside the given path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static int Load(string path, string refPrefix)
        {
            CSharpCodeProvider Provider = new CSharpCodeProvider();
            if (Provider == null)
            {
                RCScript.Log("Could not create code provider!");
                return -1;
            }

            string[] AdditionalAssemblies = new string[0];
            if (File.Exists("References.txt"))
            {
                AdditionalAssemblies = File.ReadAllLines("References.txt");

                for (int i = 0; i < AdditionalAssemblies.Length; ++i)
                {
                    if (AdditionalAssemblies[i].Length > refPrefix.Length
                        &&
                        (AdditionalAssemblies[i].Substring(0, 4).Equals("all:", StringComparison.CurrentCultureIgnoreCase)
                         || AdditionalAssemblies[i].Substring(0, refPrefix.Length).Equals(refPrefix, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        if (File.Exists(AdditionalAssemblies[i]))
                        {
                            AdditionalAssemblies[i] = Path.GetFullPath(AdditionalAssemblies[i]);
                        }
                    }
                }
            }

            CompilerParameters Parameters = new CompilerParameters();
            Parameters.GenerateInMemory = false;
            Parameters.GenerateExecutable = false;
            Parameters.ReferencedAssemblies.Add(Path.Combine(System.Environment.CurrentDirectory, "Scripting.dll"));
            Parameters.ReferencedAssemblies.Add(Path.Combine(System.Environment.CurrentDirectory, "Community.CsharpSqlite.dll"));
            Parameters.ReferencedAssemblies.Add(Path.Combine(System.Environment.CurrentDirectory, "Community.CsharpSqlite.SQLiteClient.dll"));
            Parameters.ReferencedAssemblies.Add("System.dll");
            Parameters.ReferencedAssemblies.Add("System.Data.dll");
            Parameters.ReferencedAssemblies.Add("System.Drawing.dll");
            Parameters.ReferencedAssemblies.AddRange(AdditionalAssemblies);
            Parameters.IncludeDebugInformation = true;
            Parameters.CompilerOptions = "/debug:pdbonly";
            Parameters.TempFiles.KeepFiles = true;
            //Parameters.TempFiles.AddExtension("pdb", true);

            string[] Files = GetFiles(path, "*.cs");

            string OldDir = System.Environment.CurrentDirectory;
            System.Environment.CurrentDirectory = path;
            int Count = 0;
            bool Errors = false;

            string[] NameArray = new string[Files.Length];

            //foreach (string F in Files)
            for(int i = 0; i < Files.Length; ++i)
            {
                if(Files[i].Length > path.Length)
                    NameArray[i] = Files[i].Substring(path.Length);// Path.GetFileName(Files[i]);
            }


            CompilerResults Results = Provider.CompileAssemblyFromFile(Parameters, NameArray);
            

            bool Failed = false;
            if (Results.Errors.Count > 0)
            {
                RCScript.Log("");
                foreach (CompilerError E in Results.Errors)
                {
                    if (!E.IsWarning)
                    {
                        Failed = true;
                        Errors = true;
                    }

                    RCScript.Log(String.Format("  {5}({0},{1}): {2} {3}: {4}", new object[] { E.Line, E.Column, E.IsWarning ? "Warning" : "Error", E.ErrorNumber, E.ErrorText, Path.GetFileName(E.FileName) }));
                }
            }

            if (Failed)
                return -1;

            Assembly LoadedAssembly = Results.CompiledAssembly;

            foreach (Type type in LoadedAssembly.GetTypes())
            {
                if (type.IsAbstract)
                    continue;


                if (type.BaseType == typeof(ScriptBase))
                {
                    Scripts.Add(type);
                    ++Count;
                }
                else if (type.GetInterface("Scripting.IChatProcessor") == typeof(IChatProcessor))
                {
                    SpecialScripts.Add(type);
                    ++Count;
                }
                else if (type.GetInterface("Scripting.IAccountDatabase") == typeof(IAccountDatabase))
                {
                    SpecialScripts.Add(type);
                    ++Count;
                }

                AllTypes.Add(type);

//                 {
//                     Console.WriteLine(String.Format("  {1} {0}", new object[] { "is not derived from ScriptBase (is this intentional?)", type.FullName }));
//                     continue;
//                 }


            }

            System.Environment.CurrentDirectory = OldDir;
            if (Errors)
                return -1;
            else
                return Count;
        }

        /// <summary>
        /// Create an instance of the given script name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static object Instantiate(string name)
        {
            foreach (Type I in AllTypes)
            {
                if (I.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                {
                    return Activator.CreateInstance(I);
                }
            }

            return null;
        }

        /// <summary>
        /// Create an instance of the named script and prepare the given entrypoint for execution.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="entrypoint"></param>
        /// <returns></returns>
        private static ScriptInstance CreateScriptInstance(string name, string entrypoint)
        {
            foreach (Type I in Scripts)
            {
                if (I.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                {
                    foreach (MethodInfo Mi in I.GetMethods())
                    {
                        if (Mi != null && Mi.Name.Equals(entrypoint, StringComparison.CurrentCultureIgnoreCase))
                        {
                            // Execute!
                            ScriptInstance Instance = new ScriptInstance(
                                Activator.CreateInstance(I),
                                Mi);

                            //Instance.Execute(arguments);

                            //Instances.Add(Instance);

                            return Instance;
                        }
                    }
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// Creates an instance of a 'special' script object. Such objects include scripted
        /// processing classes which are not attached to entities and classes which must
        /// stay alive for the server lifetime.
        /// </summary>
        /// <param name="baseType">Type of object to instantiate.</param>
        /// <returns></returns>
        public static object InstantiateSpecialScriptObject(Type baseType)
        {
            try
            {
                foreach (Type t in SpecialScripts)
                {
                    if (t.GetInterface(baseType.Name) == baseType)
                    {
                        return Activator.CreateInstance(t);
                    }
                }
            }
            catch (Exception e)
            {
                RCScript.Log(e.Message);
            }

            return null;
        }

        /// <summary>
        /// Call a method on a script created with 'InstantiateSpecialScriptObject'.
        /// 
        /// This method is particularly useful if script-based exception handling is required,
        /// since a caught error can be passed to an RC MasterServer with file/line tracking.
        /// </summary>
        /// <param name="script"></param>
        /// <param name="methodName"></param>
        /// <param name="argument"></param>
        public static void ExecuteSpecialScriptObject(object script, string methodName, object argument)
        {
            ExecuteSpecialScriptObject(script, methodName, new object[] { argument });
        }

        /// <summary>
        /// Call a method on a script created with 'InstantiateSpecialScriptObject'.
        /// 
        /// This method is particularly useful if script-based exception handling is required,
        /// since a caught error can be passed to an RC MasterServer with file/line tracking.
        /// </summary>
        /// <param name="script"></param>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        public static void ExecuteSpecialScriptObject(object script, string methodName, object[] arguments)
        {

            try
            {
                MethodInfo Info = script.GetType().GetMethod(methodName);
                if (Info == null)
                    throw new Exception("Method '" + methodName + "' not found!");

                Info.Invoke(script, arguments);
            }
            catch (System.Exception e)
            {
                if (e.InnerException == null)
                {
                    RCScript.Log("-----------");
                    RCScript.Log("Potential non-script exception occured:");
                    RCScript.Log(e.ToString());
                    RCScript.Log("-----------");
                    RCScript.Log("Trace:");
                    RCScript.Log(e.StackTrace);
                    RCScript.Log("-----------");

                    return;
                }


                System.Diagnostics.StackTrace Trace = new System.Diagnostics.StackTrace(e.InnerException, true);

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

                if (Filename.Length > ExecutingPath.Length)
                    Filename = Filename.Substring(ExecutingPath.Length);

                ScriptExceptionArgs Args = new ScriptExceptionArgs(Filename, LineNumber, CollumnNumber, e.InnerException.Message, null, null);
                ScriptManager.HandledException(Args);

                RCScript.Log(String.Format("{0} ({1}, {2}): {3}", new object[] { Path.GetFileName(Filename), LineNumber, CollumnNumber, e.InnerException.Message }));
            }
        }

        /// <summary>
        /// Call a static method on a script.
        /// 
        /// This method is particularly useful if script-based exception handling is required,
        /// since a caught error can be passed to an RC MasterServer with file/line tracking.
        /// </summary>
        /// <param name="script"></param>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        public static void ExecuteSpecialScriptObject(Type scriptObjectType, string methodName, object[] arguments)
        {

            try
            {
                MethodInfo Info = scriptObjectType.GetMethod(methodName);
                if (Info == null)
                    throw new Exception("Method '" + methodName + "' not found!");
                if (!Info.IsStatic)
                    throw new Exception("Method '" + methodName + "' is not static!");

                Info.Invoke(null, arguments);
            }
            catch (System.Exception e)
            {
                System.Diagnostics.StackTrace Trace = new System.Diagnostics.StackTrace(e.InnerException, true);

                string Filename = "";
                int LineNumber = 0;
                int CollumnNumber = 0;

                int FrameIndex = Trace.FrameCount - 1;

                if (FrameIndex < 0)
                {
                    string ExecutingPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    if (!ExecutingPath.Substring(ExecutingPath.Length - 1, 1).Equals("/")
                        && !ExecutingPath.Substring(ExecutingPath.Length - 1, 1).Equals("\\"))
                    {
                        ExecutingPath += Path.PathSeparator;
                    }

                    Filename = Trace.GetFrame(FrameIndex).GetFileName();
                    LineNumber = Trace.GetFrame(FrameIndex).GetFileLineNumber();
                    CollumnNumber = Trace.GetFrame(FrameIndex).GetFileColumnNumber();

                    if (Filename.Length > ExecutingPath.Length)
                        Filename = Filename.Substring(ExecutingPath.Length);
                }

                ScriptExceptionArgs Args = new ScriptExceptionArgs(Filename, LineNumber, CollumnNumber, e.InnerException.Message, null, null);
                ScriptManager.HandledException(Args);

                RCScript.Log(String.Format("{0} ({1}, {2}): {3}", new object[] { Path.GetFileName(Filename), LineNumber, CollumnNumber, e.InnerException.Message }));
            }
        }

        /// <summary>
        /// Extracts exception information for logging purposes
        /// </summary>
        /// <param name="e"></param>
        public static void HandleException(System.Exception e)
        {
            System.Diagnostics.StackTrace Trace = new System.Diagnostics.StackTrace(e.InnerException, true);

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

            if (Filename.Length > ExecutingPath.Length)
                Filename = Filename.Substring(ExecutingPath.Length);

            ScriptExceptionArgs Args = new ScriptExceptionArgs(Filename, LineNumber, CollumnNumber, e.InnerException.Message, null, null);
            ScriptManager.HandledException(Args);

            RCScript.Log(String.Format("{0} ({1}, {2}): {3}", new object[] { Path.GetFileName(Filename), LineNumber, CollumnNumber, e.InnerException.Message }));
            
        }

        /// <summary>
        /// Execute the given script.
        /// </summary>
        /// <param name="name">Script name to instance.</param>
        /// <param name="entrypoint">Entry method to invoke.</param>
        /// <param name="arguments">Array of arguments to send to the entrypoint.</param>
        /// <param name="tag">Associative Tag.</param>
        /// <param name="tag2">Associative Tag.</param>
        /// <returns></returns>
        public static bool Execute(string name, string entrypoint, object[] arguments, object tag, object tag2)
        {
            ScriptInstance Instance = CreateScriptInstance(name, entrypoint);
            if (Instance == null)
                return false;

            Instance.Execute(arguments, tag, tag2);

            return true;
        }

        /// <summary>
        /// Execute the given script.
        /// </summary>
        /// <param name="name">Script name to instance.</param>
        /// <param name="entrypoint">Entry method to invoke.</param>
        /// <param name="arguments">Argument to send to the entrypoint.</param>
        /// <param name="tag">Associative Tag.</param>
        /// <param name="tag2">Associative Tag.</param>
        /// <returns></returns>
        public static bool Execute(string name, string entrypoint, object arguments, object tag, object tag2)
        {
            return Execute(name, entrypoint, new object[] { arguments }, tag, tag2);
        }

        /// <summary>
        /// Setup a script for execution but defer until the end of the current update cycle.
        /// </summary>
        /// <param name="name">Script name to instance.</param>
        /// <param name="entrypoint">Entry method to invoke.</param>
        /// <param name="arguments">Array of arguments to send to the entrypoint.</param>
        /// <param name="tag">Associative Tag.</param>
        /// <param name="tag2">Associative Tag.</param>
        /// <returns></returns>
        public static bool DelayedExecute(string name, string entrypoint, object[] arguments, object tag, object tag2)
        {
            ScriptInstance Instance = CreateScriptInstance(name, entrypoint);
            if (Instance == null)
                return false;

            Delayed.AddLast(new DelayedScriptInstance(Instance, arguments, tag, tag2));

            return true;
        }

        /// <summary>
        /// Setup a script for execution but defer until the end of the current update cycle.
        /// </summary>
        /// <param name="name">Script name to instance.</param>
        /// <param name="entrypoint">Entry method to invoke.</param>
        /// <param name="arguments">Argument to send to the entrypoint.</param>
        /// <param name="tag">Associative Tag.</param>
        /// <param name="tag2">Associative Tag.</param>
        /// <returns></returns>
        public static bool DelayedExecute(string name, string entrypoint, object arguments, object tag, object tag2)
        {
            return DelayedExecute(name, entrypoint, new object[] { arguments }, tag, tag2);
        }

        /// <summary>
        /// Execute delayed script instances.
        /// </summary>
        public static void Update()
        {
            LinkedListNode<DelayedScriptInstance> Node = Delayed.First;
            while (Node != null)
            {
                DelayedScriptInstance Instance = Node.Value;

                Instance.Instance.Execute(Instance.Arguments, Instance.Tag, Instance.Tag2);

                Node = Node.Next;
            }
            Delayed.Clear();
        }


    }
}
