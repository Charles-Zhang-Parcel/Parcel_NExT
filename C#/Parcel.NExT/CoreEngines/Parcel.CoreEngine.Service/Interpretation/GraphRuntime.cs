using Parcel.CoreEngine.Document;
using Parcel.CoreEngine.InstructionSets;
using Parcel.CoreEngine.Service.CoreExtensions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Parcel.CoreEngine.Service.Interpretation
{
    public partial class GraphRuntime
    {
        #region Constructor
        public GraphRuntime(ParcelGraph mainGraph, Dictionary<ParcelNode, ParcelPayload> nodePayloadLookUps)
        {
            Graph = mainGraph;
            Payloads = nodePayloadLookUps;
        }
        #endregion

        #region Constants
        [GeneratedRegex(@"^({(.*?)})?(\[(.*?)\])?((.*?):)?(.*?)$")]
        public static partial Regex NodeTargetPathStructureProtocolSyntax();
        public record NodeTargetPathProtocolStructure(string System, string EngineSymbol, string AssemblyPath, string TargetPath);
        #endregion

        #region Properties
        public ParcelGraph Graph { get; }
        public Dictionary<ParcelNode, ParcelPayload> Payloads { get; }

        public Dictionary<string, string> Variables { get; } = [];
        #endregion

        #region Methods
        public void Execute()
        {
            foreach (ParcelNode node in Graph.MainLayout.Placements.Select(n => n.Node))
                ExecuteNode(node);
        }
        #endregion

        #region Routines
        private void ExecuteNode(ParcelNode node)
        {
            NodeTargetPathProtocolStructure targets = ParseNodeTargets(node.Target);

            if (!string.IsNullOrEmpty(targets.System))
                ExecuteSystemNode(node, targets);
            // Typical C#
            else if (string.IsNullOrEmpty(targets.EngineSymbol) || targets.EngineSymbol.ToUpper() == "C#")
            {
                // TODO: THIS SHOULD NOT BE HANDLED BY THE ENGINE
                string fullTargetPath = InstructionSetMapping.ParcelCustomNameMappings.ContainsKey(targets.TargetPath) ? InstructionSetMapping.ParcelCustomNameMappings[targets.TargetPath] : targets.TargetPath;

                // TODO: Might isolate those into core library?
                Assembly? core = Assembly.GetAssembly(typeof(Primitives.Number));
                Type? type = core.GetType(fullTargetPath);
                string[] arguments = NodeDefinitionHelper.SimpleExtractParameters(node);
                arguments = DereferenceParameters(arguments);
                if (type != null)
                {
                    object[] typedArguments = PackArguments(type.GetConstructors().First(), arguments);
                    var instance = Activator.CreateInstance(type, typedArguments);
                    Payloads[node] = new ParcelPayload(node, new Dictionary<string, object>()
                    {
                        { "value", instance }
                    });
                }
                else
                {
                    int splitterIndex = fullTargetPath.LastIndexOf('.');
                    string typeName = fullTargetPath.Substring(0, splitterIndex);
                    string function = fullTargetPath.Substring(splitterIndex + 1);

                    type = core.GetType(typeName);
                    if (type == null)
                        throw new ApplicationException("(Need handling)");
                    MethodInfo staticMethod = FindBestMatchingMethod(type, function, arguments);
                    object[] typedArguments = PackArguments(staticMethod, arguments);
                    object returnValue = CallMethodWithParamsHandling(type, staticMethod, typedArguments);
                    Payloads[node] = new ParcelPayload(node, new Dictionary<string, object>()
                    {
                        { "value", returnValue }
                    });

                    // ExecuteMethodStatic(targets, arguments);
                }
            }
        }

        private static object CallMethodWithParamsHandling(Type? type, MethodInfo staticMethod, object[] typedArguments)
        {
            object returnValue;
            // Deal with the case of `params` parameters array
            if (staticMethod.GetParameters().Last().GetCustomAttribute<ParamArrayAttribute>() != null)
            {
                var elementType = staticMethod.GetParameters().Last().ParameterType.GetElementType();
                var array = Array.CreateInstance(elementType, typedArguments.Length);
                for (int i = 0; i < typedArguments.Length; i++)
                    array.SetValue(typedArguments[i], i);
                returnValue = staticMethod.Invoke(type, [array]);
            }
            else
                returnValue = staticMethod.Invoke(type, typedArguments);
            return returnValue;
        }

        private MethodInfo FindBestMatchingMethod(Type? type, string name, string[] arguments)
        {
            var methods = type.GetMethods().Where(m => m.Name == name).ToArray();
            return methods.First(m => m.GetParameters().Length == arguments.Length || m.GetParameters().Last().GetCustomAttribute<ParamArrayAttribute>() != null);
        }
        private string[] DereferenceParameters(string[] arguments)
        {
            return arguments
                .Select(a =>
                {
                    if (a.StartsWith(':'))
                        return a.TrimStart(':');
                    else if (a.StartsWith('$'))
                        return Variables[a.TrimStart('$')];
                    else if (a.StartsWith('@'))
                        return ParcelNodeUnifiedAttributesHelper.GetFromUnifiedAttribute(Payloads, Graph.MainLayout.Placements.Select(n => n.Node).ToArray(), a.TrimStart('@'));
                    else if (a.StartsWith('!'))
                        throw new NotImplementedException();
                    else throw new ArgumentException($"Invalid argument (unrecognized format): {a}. Use : for literals, $ for variables, @ for nodes/attributes, and ! for actions/graphs.");
                })
                .ToArray();
        }

        private void ExecuteSystemNode(ParcelNode node, NodeTargetPathProtocolStructure targets)
        {
            // TODO: Definitely need to implement those in a dedicated class
            string action = targets.System;
            switch (action)
            {
                case "SetVariable":
                    Variables[node.Attributes["name"]] = node.Attributes["value"];
                    Payloads[node] = new ParcelPayload(node, new Dictionary<string, object>()
                    {
                        { "value", node.Attributes["value"] }
                    });
                    break;
                case "GetVariable":
                    Payloads[node] = new ParcelPayload(node, new Dictionary<string, object>()
                    {
                        { "value", Variables[node.Attributes["name"]] }
                    });
                    break;
                case "Preview":
                    Payloads[node] = new ParcelPayload(node, new Dictionary<string, object>()
                    {
                        { "value",  DereferenceParameters(NodeDefinitionHelper.SimpleExtractParameters(node)).Single()}
                    });
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Helpers
        public static object[] PackArguments(ConstructorInfo constructorInfo, string[] arguments)
        {
            // TODO: constructorInfo.GetParameters();
            return arguments.Select(double.Parse).OfType<object>().ToArray();
        }
        public static object[] PackArguments(MethodInfo methodInfo, string[] arguments)
        {
            // TODO: constructorInfo.GetParameters();
            return arguments.Select(double.Parse).OfType<object>().ToArray();
        }
        public static NodeTargetPathProtocolStructure ParseNodeTargets(string target)
        {
            Match match = NodeTargetPathStructureProtocolSyntax().Match(target);
            string system = match.Groups[2].Value;
            string engineSymbol = match.Groups[4].Value;
            string assemblyPath = match.Groups[6].Value;
            string objectPath = match.Groups[7].Value;
            return new NodeTargetPathProtocolStructure(system, engineSymbol, assemblyPath, objectPath);
        }
        public static void ExecuteMethodStatic(NodeTargetPathProtocolStructure targets, string[] arguments)
        {
            InvokationService.InvokeRemoteFunction(targets.AssemblyPath, targets.TargetPath, arguments);
        }
        #endregion
    }
}
