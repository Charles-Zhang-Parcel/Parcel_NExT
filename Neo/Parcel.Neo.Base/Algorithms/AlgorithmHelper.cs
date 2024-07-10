﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using Humanizer;
using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

namespace Parcel.Neo.Base.Algorithms
{
    public static class AlgorithmHelper
    {
        /// <summary>
        /// Detect cyclic connection
        /// </summary>
        public static bool FindSelf(BaseNode startingNode, BaseNode self)
        {
            if (self == startingNode)
                return true;
            
            if (self is KnotNode knot)
            {
                IEnumerable<BaseNode> outConnections = knot.Next;
                return outConnections.Any(c => FindSelf(startingNode, c));
            }
            else if (self is ProcessorNode processor)
            {
                var outConnections = processor.Output.SelectMany(o => o.Connections)
                    .Where(c => c.Output.IsConnected)
                    .Select(c => c.Output.Node);
                return outConnections.Any(c => FindSelf(startingNode, c));
            }
            else throw new ArgumentException("Invalid node type");
        }

        public static void ExecuteGraph(NodesCanvas canvas)
        {
            IEnumerable<ProcessorNode> processors = canvas.Nodes
                .Where(n => n is ProcessorNode node && node.IsPreview == true)
                .Select(n => n as ProcessorNode);
            
            IExecutionGraph graph = new ExecutionQueue();
            graph.InitializeGraph(processors);
            graph.ExecuteGraph();
        }

        #region Code Gen (Draft)
        public static void CompileGraphAOT(string saveFilePath, NodesCanvas canvas)
        {
            IEnumerable<ProcessorNode> processors = canvas.Nodes
                .Where(n => n is ProcessorNode node)
                .Select(n => n as ProcessorNode);

            // Generate scripts (a script contains functions and other information, a function contains sections and other information)
            ExecutionQueue graph = new();
            graph.InitializeGraph(processors);

            foreach (ProcessorNode item in graph.Queue)
            {
                
            }

            // Generate code files

            // Initialize C# proejct

            // Compile and validate

            // Copy compiled result to destination path
        }
        public static void GenerateGraphScripts(string folderPath, string mainScriptFilename, NodesCanvas canvas)
        {
            // Generate scripts (a script contains functions and other information, a function contains sections and other information)

            // Filter executable nodes
            IEnumerable<ProcessorNode> processors = canvas.Nodes
                .Where(n => n is ProcessorNode node)
                .Select(n => n as ProcessorNode);

            // Fetch dependency graph
            ExecutionQueue graph = new();
            graph.InitializeGraph(processors);

            // Gather essential information
            ScriptDependencySummary summary = GatherScriptDependencies(graph);

            // Pre-build scripts
            StringBuilder mainSection = new();
            foreach (string line in summary.ScriptSectionStatements)
                mainSection.AppendLine($"{line};");
            StringBuilder[] scriptSections = [mainSection];

            // Generate script contents
            StringBuilder mainScriptBuilder = new();
            // Import package references
            foreach ((string importName, string nickName) in summary.StandardPackageImports)
                mainScriptBuilder.AppendLine($"Import({importName})");
            mainScriptBuilder.AppendLine();
            // Make necessary namespace usage and static usage
            // Style preference: if the script is very short, use static, otherwise, use type name based addressing.
            if (summary.UniqueTypes.Count() < 10)
                foreach (Type type in summary.InvolvedStaticTypes)
                    mainScriptBuilder.AppendLine($"using static {type.FullName};");
            else
                foreach (string uniqueNamespace in summary.UniqueNamespaces.Select(n => n.Namespace).Distinct())
                    mainScriptBuilder.AppendLine($"using {uniqueNamespace};");
            mainScriptBuilder.AppendLine();
            // Do variable declarations first
            foreach ((string key, string value) in summary.VariableDeclarations)
                mainScriptBuilder.AppendLine($"var {key} = {value};");
            // Append script sections
            foreach (StringBuilder section in scriptSections)
            {
                mainScriptBuilder.Append(section.ToString().TrimEnd());
                mainScriptBuilder.AppendLine();
            }

            // Create output folder if not exist
            Directory.CreateDirectory(folderPath);

            // Save main script
            string mainScript = $"{GetPureScriptGeneratedHeader()}\n{mainScriptBuilder.ToString().TrimEnd()}";
            File.WriteAllText(Path.Combine(folderPath, mainScriptFilename), mainScript);

            // TODO: Implement generating other script files
            // ...

            static string GetPureScriptGeneratedHeader()
            {
                return """
                    /* Generated Script File Header
                    This file is generated with Parcel NExT CodeGen for Pure 2.
                    You should have Pure installed in order to execute this script; 
                    You can downoad Pure at https://methodox.io/Pure. Notice to run the script you must make sure all `Import()`
                    packages are available under your environment `PATH` variable.

                    Alternatively, if you are using PV1 Neo you can export a .csproj C# project to be used in Visual Studio,
                    or if you graph doesn't involve advanced features like reflection, 
                    you can export a native executable file directly from the GUI.
                    */
                    """;
            }
        }
        public static void GenerateGraphPythonScripts(string folderPath, string mainScriptFilename, NodesCanvas canvas)
        { 
            const string defaultIndentation = "    "; // Python default indentation is 4 spaces

            // Filter executable nodes
            IEnumerable<ProcessorNode> processors = canvas.Nodes
                .Where(n => n is ProcessorNode node)
                .Select(n => n as ProcessorNode);

            // Fetch dependency graph
            ExecutionQueue graph = new();
            graph.InitializeGraph(processors);
            // Gather essential information
            ScriptDependencySummary summary = GatherScriptDependencies(graph);

            // Pre-build scripts
            StringBuilder mainSection = new();
            foreach(var line in summary.ScriptSectionStatements)
                mainSection.AppendLine(line);
            StringBuilder[] scriptSections = [mainSection];

            // Generate script contents
            StringBuilder mainScriptBuilder = new();
            // Main import
            mainScriptBuilder.AppendLine("""
                # Import Root Parcel NExT Module
                from ParcelNExT import LoadPackage

                """);
            // Import package references
            mainScriptBuilder.AppendLine("# Load Parcel NExT packages");
            foreach ((string importName, string nickName) in summary.StandardPackageImports)
                mainScriptBuilder.AppendLine($"LoadPackage('{importName}')"); // TODO: Add brief summary of package description if available
            mainScriptBuilder.AppendLine();
            // Import static types from corresponding namespaces; Notice Pythonnet doesn't support using static methods at the top level
            mainScriptBuilder.AppendLine("# Import submodules");
            foreach (Type type in summary.InvolvedStaticTypes)
                mainScriptBuilder.AppendLine($"from {type.Namespace} import {type.Name}"); // TODO: Add brief summary of class description if available
            mainScriptBuilder.AppendLine();
            // Entry point
            mainScriptBuilder.AppendLine("# Main script content");
            mainScriptBuilder.AppendLine("""
                if __name__ == '__main__':
                """);
            // Do variable declarations first
            foreach ((string key, string value) in summary.VariableDeclarations)
                mainScriptBuilder.AppendLine($"{defaultIndentation}{key} = {value}");
            // Append script sections
            foreach (StringBuilder section in scriptSections)
            {
                mainScriptBuilder.Append(Regex.Replace(section.ToString().TrimEnd(), @"^", defaultIndentation, RegexOptions.Multiline)); // Apply line indentation
                mainScriptBuilder.AppendLine();
            }

            // Create output folder if not exist
            Directory.CreateDirectory(folderPath);

            // Save main script
            string mainScript = $"{GetPureScriptGeneratedHeader()}\n{mainScriptBuilder.ToString().TrimEnd()}";
            File.WriteAllText(Path.Combine(folderPath, mainScriptFilename), mainScript);

            // TODO: Implement generating other script files
            // ...

            static string GetPureScriptGeneratedHeader()
            {
                return """"
                    """
                    Parcel NExT Executable Workflow
                    This python script is generated with Parcel NExT CodeGen.
                    You should have Python installed in order to execute this script; 
                    You can downoad Python at https://www.python.org/downloads/. We recommend latest version.
                    Notice to run the script you must make sure all essential dependencies are available under PYTHONPATH.

                    Use `pip install pythonnet` to install essential dependency.
                    Requires .NET Core to be installed or the dotnet CLI tool to be on the PATH.
                    """

                    """"; // TODO: Publish Parcel NExT as a standalone python package
            }
        }

        // TODO: Do not handle statement generation inside this class; Handle it in a dedicated place instead
        public sealed class ScriptDependencySummary
        {
            #region Construction
            public ScriptDependencySummary(string subGraphID, ExecutionQueue graph)
            {
                SubgraphID = subGraphID;
                GatherScriptDependencies(graph);
            }
            #endregion

            #region Properties
            public string SubgraphID { get; }
            public Dictionary<string, string> VariableDeclarations { get; } = [];
            public HashSet<string> ScopedVariables { get; } = [];
            public string[] ScriptSectionStatements { get; private set; } = [];
            public (string PackageID, string Namespace)[] UniqueNamespaces { get; private set; }
            public Dictionary<string, string> StandardPackageImports { get; private set; }
            public Assembly[] UniqueAssemblies { get; private set; }
            public Type[] UniqueTypes { get; private set; }
            public Type[] InvolvedStaticTypes => UniqueTypes.Where(t => t.IsAbstract && t.IsSealed).ToArray();
            #endregion

            public record NodeHandlingResult(bool IsVariable = false, string? VariableName = null);

            #region Method
            private void GatherScriptDependencies(in ExecutionQueue graph)
            {
                List<AutomaticProcessorNode> automaticProcessors = [];
                // TOOD: Special handle math nodes that have corresponding C# operators: +-*/
                List<string> statements = [];
                Dictionary<ProcessorNode, NodeHandlingResult> handledNodes = [];
                foreach (ProcessorNode processorNode in graph.Queue) // Enumerate in the execution order
                {
                    // Remark-cz: notice as of PV1 Neo we no longer have c#-lambda impelemented nodes; Everything is eitehr completely front-end implemented as ProcessorNode or it's C# defined in packages

                    // Deal with package functions
                    if (processorNode is AutomaticProcessorNode autoNode)
                    {
                        automaticProcessors.Add(autoNode);
                        string[] parameters = autoNode.Input.Select(i => i.Title).ToArray();
                        string methodCallName = $"{(autoNode.Descriptor.Method.IsStatic ? autoNode.Descriptor.Method.DeclaringType.Name + ".": string.Empty)}{autoNode.Descriptor.NodeName}"; // TODO: We do not need to address full type name if we are using static (that's why we should not handle statement generation directly here and just parse essential information and let the actual code generation for specific target languages (pure vs c# vs python) handle it

                        // Translate parameters
                        string[] parameterDefaultValues = Enumerable.Range(0, parameters.Length).Select(i => autoNode.Descriptor.DefaultInputValues[i].ToString()).ToArray();
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            string parameter = parameters[i];
                            if (autoNode.Input[i].Connections.Any())
                            {
                                ProcessorNode source = autoNode.Input[i].Connections.Single().Input.Node as ProcessorNode;
                                if (!handledNodes.ContainsKey(source))
                                    throw new ApplicationException("Source should have already been handled.");
                                if (!handledNodes[source].IsVariable)
                                    throw new ApplicationException("Source should have generated some outputs.");
                                parameters[i] = handledNodes[source].VariableName;
                            }
                            else
                                parameters[i] = parameterDefaultValues[i];
                        }

                        // Save outputs
                        if (autoNode.Output.Any())
                        {
                            string outputVariableName = autoNode.MainOutput.Title.Camelize();
                            statements.Add($"{outputVariableName} = {methodCallName}({string.Join(", ", parameters)})");
                            ScopedVariables.Add(outputVariableName);

                            // Book keep node outputs
                            handledNodes[processorNode] = new(true, outputVariableName);
                        }
                        // Plain call
                        else
                        {
                            statements.Add($"{methodCallName}({string.Join(", ", parameters)})");
                            handledNodes[processorNode] = new();
                        }
                    }
                    // Deal with front-end implemented nodes
                    else
                    {
                        // Primitives are processed as variable definition
                        if (processorNode is PrimitiveNode primitive)
                        {
                            string variableName = processorNode.Title.Camelize();
                            VariableDeclarations[variableName] = primitive.Value; // TODO: Instead of using MainOutput which depdends on cache which requires us to execute the graph, we should fetch directly its stored values.
                            ScopedVariables.Add(variableName);
                            handledNodes[processorNode] = new(true, variableName);
                        }
                    }
                }

                // Gather unique dependent assemblies
                UniqueTypes = automaticProcessors.Select(p => p.Descriptor.Method.DeclaringType).Distinct().ToArray(); // TODO: Not sure whether type comparison in Disctin() works as expected
                UniqueAssemblies = automaticProcessors.Select(p => p.Descriptor.Method.DeclaringType.Assembly).Distinct().ToArray();
                UniqueNamespaces = automaticProcessors.Select(p => p.Descriptor.Method.DeclaringType)
                    .GroupBy(t => new ValueTuple<string, string>(t.Assembly.GetName().Name, t.Namespace))
                    .Select(g => g.Key)
                    .ToArray();
                StandardPackageImports = UniqueAssemblies.Select(a => a.CodeBase)
                    .Select(codeBase => Uri.UnescapeDataString(new UriBuilder(codeBase).Path))
                    .Where(File.Exists)
                    .Select(FindStandardPackageFriendlyName)
                    .ToDictionary(n => n, n => n.Split('.').Last());

                // Final assignments
                ScriptSectionStatements = statements.ToArray();
            }
            #endregion
        }
        private static ScriptDependencySummary GatherScriptDependencies(in ExecutionQueue graph)
            => new("Main Script", graph);
        #endregion

        #region Helpers
        private static string FindStandardPackageFriendlyName(string filePath)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            if (fileNameWithoutExtension.StartsWith("Parcel."))
                return fileNameWithoutExtension;
            else return filePath;
        }
        #endregion
    }
}