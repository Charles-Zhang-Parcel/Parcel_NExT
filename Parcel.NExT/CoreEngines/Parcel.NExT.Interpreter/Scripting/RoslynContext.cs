﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using Parcel.NExT.Interpreter.Helpers;
using Zora.Infrastructure.NuGet;

namespace Parcel.NExT.Interpreter.Scripting
{
    internal sealed class RedirectedTextWriter : TextWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
        public override void Write(bool value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString());
        }
        public override void Write(char value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString());
        }
        public override void Write(char[] buffer, int index, int count)
        {
            RoslynContext.OutputHandler.Invoke(new string(buffer.Skip(index).Take(count).ToArray()));
        }
        public override void Write(char[] buffer)
        {
            RoslynContext.OutputHandler.Invoke(new string(buffer));
        }
        public override void Write(decimal value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString());
        }
        public override void Write(double value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString());
        }
        public override void Write(float value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString());
        }
        public override void Write(int value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString());
        }
        public override void Write(long value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString());
        }
        public override void Write(object value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString());
        }
        public override void Write(ReadOnlySpan<char> buffer)
        {
            RoslynContext.OutputHandler.Invoke(buffer.ToString());
        }
        public override void Write([StringSyntax("CompositeFormat")] string format, object arg0)
        {
            RoslynContext.OutputHandler.Invoke(string.Format(format, arg0));
        }
        public override void Write([StringSyntax("CompositeFormat")] string format, object arg0, object arg1)
        {
            RoslynContext.OutputHandler.Invoke(string.Format(format, arg0, arg1));
        }
        public override void Write([StringSyntax("CompositeFormat")] string format, object arg0, object arg1, object arg2)
        {
            RoslynContext.OutputHandler.Invoke(string.Format(format, arg0, arg1, arg2));
        }
        public override void Write([StringSyntax("CompositeFormat")] string format, params object[] arg)
        {
            RoslynContext.OutputHandler.Invoke(string.Format(format, arg));
        }
        public override void Write(string value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString());
        }
        public override void Write(StringBuilder value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString());
        }
        public override void Write(uint value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString());
        }
        public override void Write(ulong value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString());
        }
        public override void WriteLine()
        {
            RoslynContext.OutputHandler.Invoke(Environment.NewLine);
        }
        public override void WriteLine(bool value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(char value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(char[] buffer, int index, int count)
        {
            RoslynContext.OutputHandler.Invoke(new string(buffer.Skip(index).Take(count).ToArray()) + Environment.NewLine);
        }
        public override void WriteLine(char[] buffer)
        {
            RoslynContext.OutputHandler.Invoke(new string(buffer) + Environment.NewLine);
        }
        public override void WriteLine(decimal value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(double value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(float value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(int value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(long value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(object value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(ReadOnlySpan<char> buffer)
        {
            RoslynContext.OutputHandler.Invoke(buffer.ToString() + Environment.NewLine);
        }
        public override void WriteLine([StringSyntax("CompositeFormat")] string format, object arg0)
        {
            RoslynContext.OutputHandler.Invoke(string.Format(format, arg0) + Environment.NewLine);
        }
        public override void WriteLine([StringSyntax("CompositeFormat")] string format, object arg0, object arg1)
        {
            RoslynContext.OutputHandler.Invoke(string.Format(format, arg0, arg1) + Environment.NewLine);
        }
        public override void WriteLine([StringSyntax("CompositeFormat")] string format, object arg0, object arg1, object arg2)
        {
            RoslynContext.OutputHandler.Invoke(string.Format(format, arg0, arg1, arg2) + Environment.NewLine);
        }
        public override void WriteLine([StringSyntax("CompositeFormat")] string format, params object[] arg)
        {
            RoslynContext.OutputHandler.Invoke(string.Format(format, arg) + Environment.NewLine);
        }
        public override void WriteLine(string value)
        {
            RoslynContext.OutputHandler.Invoke(value + Environment.NewLine);
        }
        public override void WriteLine(StringBuilder value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(uint value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(ulong value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString() + Environment.NewLine);
        }
    }
    /// <remarks>
    /// Provides contexted Roslyn.
    /// ~~(Pure) This is implementation detail and should be protected from external users~~ (In ParcelNExT, we don't exclude the possibility people would use it directly)
    /// Pending makeing it thread safe and properly instanced with proper context protection.
    /// </remarks>
    public partial class RoslynContext // TODO: Remove `partial`
    {
        #region Private States
        private List<string> ImportedModules { get; set; } = new List<string>();
        private string TotalScript = string.Empty;// Remark-cz: Notice we intentionally use a string instead of a StringBuilder to keep in sync the state with ScriptState
        private ScriptState<object> State { get; set; }
        private bool ContextLock { get; set; }
        /// <summary>
        /// Remark-cz: We require RoslynContext to be a singleton for some good reason - what was it?
        /// (There are a few likely reasons, pending further investigation: 
        /// 1. We touched process level env variable modifications; 
        /// 2. We are piping STD OUT through custom handler;
        /// 3. We are binding to AppDomain.CurrentDomain.ProcessExit.)
        /// </summary>
        private static RoslynContext _Singleton;
        /// <summary>
        /// Remark-cz: We are hiding this singleton because we want to avoid using it; When we figure out why we needed singleton in the first place, we might be able to not require a singleton patter nat all
        /// </summary>
        private static RoslynContext Singleton => _Singleton;
        #endregion

        #region Runtime Configurable Behaviors
        /// <summary>
        /// Configures the output handling to use
        /// </summary>
        public static Action<string> OutputHandler;
        #endregion

        #region Lifetime Event
        private Action ShutdownEvents { get; set; }
        private void CurrentDomainProcessExit(object sender, EventArgs e)
        {
            ShutdownEvents?.Invoke();
        }
        #endregion

        #region Construction
        public RoslynContext(bool importAdditional, Action<string> outputHandler, IEnumerable<Assembly> additionalReferences = null)
        {
            if (_Singleton != null)
                throw new InvalidOperationException("Roslyn Context is already initialized.");
            _Singleton = this;
            if (outputHandler != null)
            {
                OutputHandler = outputHandler;
                Console.SetOut(new RedirectedTextWriter());
            }
            // Bind Process Exit Event
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomainProcessExit);

            ScriptOptions options = ScriptOptions.Default
                // Remark-cz: Use Add* instead of With* to add stuff instead of replacing stuff
                .WithReferences(typeof(RoslynContext).Assembly)
                .AddReferences(typeof(Enumerable).Assembly)
                .AddImports(
                    "System",
                    "System.Collections.Generic",
                    "System.IO", 
                    "System.Linq")
                // Pure language essential namespaces, types, and global static functions
                .AddImports($"Parcel.NExT.Interpreter.{nameof(Scripting)}")
                .AddImports($"Parcel.NExT.Interpreter.{nameof(Scripting)}.{nameof(Construct)}");
            // Add additional assembly references (won't automatically import namespaces)
            if (additionalReferences != null )
                options.AddReferences(additionalReferences.ToArray());
            // Additional commonly used but secondary imports
            if (importAdditional)
            {
                // Commonly used .Net namespace
                options = options.AddImports("System.Math");
                options = options.AddImports("System.Console");
            }

            State = CSharpScript.RunAsync(string.Empty, options).Result;
        }
        #endregion

        #region Method
        internal string GetState()
        {
            return TotalScript.ToString().TrimEnd();
        }
        internal void Parse(string input, string currentScriptFile, string nugetRepoIdentifier)
        {
            if (ImportModuleRegex().IsMatch(input))
                ImportModule(input, nugetRepoIdentifier);
            else if (IncludeScriptRegex().IsMatch(input))
                IncludeScript(input, currentScriptFile, nugetRepoIdentifier);
            else if (HelpItemRegex().IsMatch(input))
                HelpItem(input);
            else if (IsolatedScriptLineForSpecialFunctionsRegex().IsMatch(input))
                ParseSingleUnsafe(input);
            else ParseSingle(input);
        }
        internal object Evaluate(string expression, string currentScriptFile, string nugetRepoIdentifier)
        {
            if (ImportModuleRegex().IsMatch(expression))
                return null;
            else if (IncludeScriptRegex().IsMatch(expression))
                return null;
            else if (IsolatedScriptLineForSpecialFunctionsRegex().IsMatch(expression))
            {
                ParseSingleUnsafe(expression); // Parse is safe when executed on its own (without side effect before or after, but with side effect to the context)
                return null;
            }
            else if (HelpItemRegex().IsMatch(expression))
                return null;
            else 
                return ParseSingle(expression, false);
        }
        #endregion

        #region Subroutines
        private void ImportModule(string input, string nugetRepoIdentifier)
        {
            // Remark-cz: Notice you might think we can do something similarly to how System.Reflection.Assembly.LoadFrom() works inside the script to load the assembly into the context of the script - indeed that will work for the assembly loading part, but more crucially, we want to import the namespaces as well, and that cannot be done programmatically, and is better done with interpretation.
            var match = ImportModuleRegex().Match(input);
            string dllName = match.Groups[1].Value.Trim('"');
            bool importNamespaces = string.IsNullOrWhiteSpace(match.Groups[2].Value)
                || bool.Parse(match.Groups[4].Value.ToLower());

            string filePath = dllName;
            if (!File.Exists(dllName))
                filePath = TryFindDLLFile(dllName, nugetRepoIdentifier);

            List<string> statements = new();
            if (filePath != null && File.Exists(filePath))
            {
                if (ImportedModules.Contains(filePath)) return;
                else ImportedModules.Add(filePath);

                // Remark-cz: Add the moment this fails to deal with most scenarios when the package is NOT properly including the single runtime i.e. there is a "runtimes" folder which contains corresponding runtimes (The Target runtime is selected as "Portable" instead of specifc runtime). In this case it will say "<Some module> is not supported on this platform" when the module is actually available in the published build.
                // Potential reference: https://stackoverflow.com/questions/1373100/how-to-add-folder-to-assembly-search-path-at-runtime-in-net
                Assembly assembly = Assembly.LoadFrom(filePath); // Remark: Might load from within the Roslyn state's context//app domain?
                AddReference(assembly);

                if (importNamespaces)
                    foreach (var ns in assembly.GetTypes()
                            .Where(t => t.IsVisible)
                            .Select(t => t.Namespace)
                            .Where(ns => !string.IsNullOrEmpty(ns))
                            .Distinct())
                        AddImport(ns);
                // Special handle Main class
                Type mainType = assembly.GetTypes().FirstOrDefault(t => t.Name == "Main" && t.IsVisible && t.IsAbstract && t.IsSealed);
                if (mainType != null)
                {
                    // Expose all functions at top level for the "Main" interface
                    AddImport($"{mainType.Namespace}.Main");
                    // Execute "StartUp" if any
                    MethodInfo startUp = mainType.GetMethod("StartUp", BindingFlags.NonPublic | BindingFlags.Static);
                    if (startUp != null && startUp.GetParameters().Length == 0 && startUp.IsPrivate == true)
                        startUp.Invoke(null, null);
                    // Handle "ShutDown" if any
                    MethodInfo shutDown = mainType.GetMethod("ShutDown", BindingFlags.NonPublic | BindingFlags.Static);
                    if (shutDown != null && shutDown.GetParameters().Length == 0 && shutDown.IsPrivate == true)
                        ShutdownEvents += (Action)Delegate.CreateDelegate(typeof(Action), shutDown);
                }
            }
            else Console.WriteLine($"Cannot find package: {dllName}");
        }
        private void IncludeScript(string input, string currentScriptFile, string nugetRepoIdentifier)
        {
            // Remark-cz: Include search order: Current working directory, script file path (if any), PUREPATH
            var match = IncludeScriptRegex().Match(input);
            string scriptName = match.Groups[1].Value.Trim('"');

            string scriptPath = scriptName;
            if (!File.Exists(scriptName))
                scriptPath = PathHelper.FindScriptFileFromEnvPath(scriptName, currentScriptFile);

            if (!File.Exists(scriptPath))
                throw new ArgumentException($"File {scriptPath ?? scriptName} doesn't exist.");

            string text = File.ReadAllText(scriptPath);
            foreach (var code in Interpreter.SplitScripts(text))
                // Remark-cz: This will cause issue with interrupting parsing state
                Parse(code, scriptPath, nugetRepoIdentifier);
        }
        private void HelpItem(string input)
        {
            var match = HelpItemRegex().Match(input);
            string name = match.Groups[1].Value;
            if (string.IsNullOrEmpty(name))
                PrintGeneralHelp();
            else
            {
                bool isPrintMetaData = PrintName(name);
                if (!isPrintMetaData)
                    ParseSingle(input);
            }
        }
        #endregion

        #region Parsing Helper
        /// <remarks>
        /// Remark-cz: Things like "using" statement cannot be put in the middle of code block like other statements and require special treatment
        /// </remarks>
        private object ParseSingle(string script, bool printToConsole = true)
        {
            if (ContextLock)
                throw new RecursiveParsingException("Cannot parse when already parsing!");

            try
            {
                ContextLock = true;
                // Remark-cz: This will NOT work with actions that modifies state by calling host functions
                // Basically, host functions cannot and should not have side-effects on the state object directly
                State = State.ContinueWithAsync(SyntaxWrap(script)).Result;
                TotalScript += string.IsNullOrEmpty(TotalScript) ? State.Script.Code : (Environment.NewLine + State.Script.Code);
                ContextLock = false;
                if (State.ReturnValue != null)
                {
                    if (printToConsole)
                        PrintReturnValuePreviews(State.ReturnValue);
                    return State.ReturnValue;
                }
            }
            catch (Exception e)
            {
                if (e.InnerException is RecursiveParsingException)
                    throw e.InnerException;
                e = e.InnerException ?? e;
                Console.WriteLine(Regex.Replace(e.Message, @"error CS\d\d\d\d: ", string.Empty));
                if (e is not ApplicationException && e is not CompilationErrorException)
                    Console.WriteLine(e.StackTrace);
                ContextLock = false;
            }
            return null;
        }
        /// <summary>
        /// Parse without enforcing context lock;
        /// Also will not log code
        /// </summary>
        private void ParseSingleUnsafe(string script)
        {
            try
            {
                // Just execute without modifying state
                var result = State.ContinueWithAsync(SyntaxWrap(script)).Result;
                if (State.ReturnValue != null)
                    PrintReturnValuePreviews(State.ReturnValue);
            }
            catch (Exception e)
            {
                if (e.InnerException is RecursiveParsingException)
                    throw e.InnerException;
                e = e.InnerException ?? e;
                Console.WriteLine(Regex.Replace(e.Message, @"error CS\d\d\d\d: ", string.Empty));
                if (e is not ApplicationException && e is not CompilationErrorException)
                    Console.WriteLine(e.StackTrace);
            }
        }
        private void AddReference(Assembly assembly)
            => State = State.ContinueWithAsync(string.Empty, State.Script.Options.AddReferences(assembly)).Result;
        private void AddImport(string import)
            => State = State.ContinueWithAsync(string.Empty, State.Script.Options.AddImports(import)).Result;
        private static void PrintReturnValuePreviews(object returnValue)
        {
            // Print string items preview
            if (returnValue is System.Collections.IList list)
            {
                Console.WriteLine($"{returnValue} (Count: {list.Count})");
                if (list.Count < 10)
                    foreach (var item in list)
                        Console.WriteLine(item.ToString());
                else
                {
                    for (int i = 0; i < 7; i++)
                        Console.WriteLine(list[i].ToString());
                    Console.WriteLine(".");
                    Console.WriteLine(".");
                    Console.WriteLine(".");
                    for (int i = 3; i > 0; i--)
                        Console.WriteLine(list[list.Count - i].ToString());
                }
            }
            // Print general preview of primitives and type
            else
                Console.WriteLine(returnValue.ToString());
        }
        #endregion

        #region Helpers
        public static string TryFindDLLFile(string dllName, string nugetRepoIdentifier)
        {
            // Try find from PATH
            string path = PathHelper.FindDLLFileFromEnvPath(dllName);
            if (path != null)
                return path;
            // Try downloading from Nuget
            return QuickEasyDirtyNugetPreparer.TryDownloadNugetPackage(dllName, nugetRepoIdentifier);
        }
        #endregion

        #region Routine
        private static string SyntaxWrap(string input)
        {
            // Single line assignment
            if (LineAssignmentRegex().IsMatch(input))
                return $"{input};";
            return input;
        }
        #endregion

        #region Helpers
        public static void PrintGeneralHelp()
        {
            Console.WriteLine("""
                Help() command can be used to view type information and member information.
                  Help(<Namespace>): Prints all types available under a namespace.
                  Help(<Type>): Prints all members of a given type.
                  Help(<Object Instance>): Prints members information of the type of a given instance.
                """);
        }
        public static bool PrintName(string name)
        {
            Assembly foundAssembly = AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(a => a.GetName().Name == name);
            string[] nameSpaces = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Select(t => t.Namespace)).Distinct().ToArray();
            string[] types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Select(t => t.Name)).Distinct().ToArray();
            string[] typesFullname = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Select(t => t.FullName)).Distinct().ToArray();
            bool returnValue = false;

            // Print module level information
            if (foundAssembly != null)
            {
                returnValue |= true;
                Console.WriteLine($"""
                    Assembly: {foundAssembly.FullName}
                    Location: {foundAssembly.Location}
                    Namespaces:
                      {string.Join(Environment.NewLine + "  ", foundAssembly.GetExportedTypes().Select(t => t.Namespace).Where(n => n != null).OrderBy(n => n).Distinct())}
                    Types:
                      {string.Join(Environment.NewLine + "  ", foundAssembly.GetExportedTypes().Select(t => t.Name).OrderBy(n => n))}
                    """);
            }
            
            if (nameSpaces.Contains(name))
            {
                var subNamespaces = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(a =>
                        a.GetTypes()
                        .Where(t => t.Namespace?.StartsWith($"{name}.") ?? false)
                        .Select(t => t.Namespace)
                    )
                    .Distinct()
                    .OrderBy(t => t)
                    .ToArray();
                var publicTypes = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(a =>
                        a.GetTypes()
                        .Where(t => t.Namespace == name)
                        .Select(t => t.Name)
                        .Where(n => !n.StartsWith("<>")) // Skip internal names
                    )
                    .Distinct()
                    .OrderBy(t => t)
                    .ToArray();
                Console.WriteLine($"""
                        Namespace: {name}
                        Namespaces: 
                          {string.Join(Environment.NewLine + "  ", subNamespaces)}
                        Types: 
                          {string.Join(Environment.NewLine + "  ", publicTypes)}
                        """);
                returnValue |= true;
            }
            else if (types.Contains(name))
            {
                var foundTypes = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(a => a.GetTypes().Where(t => t.Name == name)).ToArray();
                if (foundTypes.Length == 1)
                    PrintType(foundTypes.Single());
                else
                    Console.WriteLine(string.Join('\n', foundTypes.Select(t => t.FullName)));
                returnValue |= true;
            }
            else if (typesFullname.Contains(name))
            {
                var type = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(a => a.GetTypes().Where(t => t.FullName == name)).Single();
                PrintType(type);
                returnValue |= true;
            }
            else if (AppDomain.CurrentDomain.GetAssemblies().Any(a => a.GetTypes().Any(t => t.GetMethods().Any(m => m.Name == name))))
            {
                HashSet<MethodInfo> methodInfos = new();
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    foreach (var type in assembly.GetTypes())
                        foreach (var method in type.GetMethods())
                            if (method.Name == name)
                                methodInfos.Add(method);

                bool isMethodsNonUnique = methodInfos.Select(m => m.DeclaringType).Distinct().Count() > 1;
                if (!isMethodsNonUnique)
                    Console.WriteLine($"From type {methodInfos.Select(m => m.DeclaringType).Distinct().Single().Name}: ");
                foreach (var method in methodInfos)
                    Console.WriteLine(PrintMethod(method, isMethodsNonUnique)); // Is method is defined in many types, we print types names
                returnValue |= true;
            }
            return returnValue;
        }
        public static void PrintType(Type type)
        {
            var publicFields = type
                .GetFields()
                .Select(m => m.Name)
                .Distinct()
                .OrderBy(t => t)
                .ToArray();
            var publicProperties = type
                .GetProperties()
                .Select(m => m.Name)
                .Distinct()
                .OrderBy(t => t)
                .ToArray();
            var publicMethods = type
                .GetMethods()
                .Select(t => PrintMethod(t, false))
                .Distinct()
                .OrderBy(t => t)
                .ToArray();
            Console.WriteLine($"""
                Type: {type.Name}
                """);
            if (publicFields.Length > 0)
                Console.WriteLine($"""
                    Fields: 
                      {string.Join(Environment.NewLine + "  ", publicFields)}
                    """);
            if (publicProperties.Length > 0)
                Console.WriteLine($"""
                    Properties: 
                      {string.Join(Environment.NewLine + "  ", publicProperties)}
                    """);
            if (publicMethods.Length > 0)
                Console.WriteLine($"""
                    Methods: 
                      {string.Join(Environment.NewLine + "  ", publicMethods)}
                    """);
        }
        public static string PrintMethod(MethodInfo m, bool printTypeName = false)
        {
            string name = m.Name;
            string type = m.ReflectedType.Name;
            string[] arguments = m.GetParameters()
                .Select(p => $"{p.ParameterType.Name} {p.Name}")
                .ToArray();
            string returnType = m.ReturnType == typeof(void) ? string.Empty : $" : {m.ReturnType.Name}";
            if (m.ReturnType.GenericTypeArguments.Length != 0)
            {
                var generics = m.ReturnType.GetGenericArguments().Select(t => t.Name);
                returnType = $": {m.ReturnType.Name}<{string.Join(", ", generics)}>";
            }
            if (m.ContainsGenericParameters)
            {
                var generics = m.GetGenericArguments().Select(t => t.Name);
                return printTypeName 
                    ? $"{type}: {name}<{string.Join(", ", generics)}>({string.Join(", ", arguments)}){returnType}"
                    : $"{name}<{string.Join(", ", generics)}>({string.Join(", ", arguments)}){returnType}";
            }
            else 
                return printTypeName
                    ? $"{type}: {name}({string.Join(", ", arguments)}){returnType}"
                    : $"{name}({string.Join(", ", arguments)}){returnType}";
        }
        #endregion

        #region Regex
        [GeneratedRegex(@"^Import\((.*?)(, ?(.*?))?\);?$")]
        public static partial Regex ImportModuleRegex();

        [GeneratedRegex(@"^Include\((.*?)(, ?(.*?))?\);?$")]
        public static partial Regex IncludeScriptRegex();

        [GeneratedRegex(@"^(Parse\((.*?)(, ?(.*?))?\);?)|(Pull\((.*?)(, ?(.*?))?\);?)$")]
        public static partial Regex IsolatedScriptLineForSpecialFunctionsRegex();

        [GeneratedRegex(@"^Help\((.*?)\)$")]
        public static partial Regex HelpItemRegex();

        [GeneratedRegex(@"^(\S*)?\s*[a-zA-Z0-9_]+\s*=.*[^;]$")]
        public static partial Regex LineAssignmentRegex();
        #endregion
    }
}
