﻿using Parcel.CoreEngine.Helpers;
using Parcel.NExT.Interpreter.Types;
using System.ComponentModel;
using System.Reflection;

namespace Parcel.CoreEngine.Service.Interpretation
{
    /// <summary>
    /// Provides metadata about functions based on native implementations; Enables quickly define a large library of simple function nodes without explicitly defining classes for them by directly utilizing C# codes.
    /// </summary>
    public class FunctionalNodeDescription
    {
        #region Properties
        public string NodeName { get; }
        public Type[] InputTypes { get; }
        public object?[]? DefaultInputValues { get; }
        public Type[] OutputTypes { get; }
        public Func<object?[], object?[]> CallMarshal { get; }
        public string[]? InputNames { get; }
        public string[]? OutputNames { get; }
        #endregion

        #region Mapping
        public Callable Method { get; }
        #endregion

        #region Construction
        public FunctionalNodeDescription(string nodeName, Callable method)
        {
            // Parse definitions
            SeperateMethodParametersForNode(method, out (Type Type, string ParameterName, object? DefaultValue)[] nodeInputTypes, out (Type Type, string ParameterName)[] nodeOutputTypes);

            // Set properties
            NodeName = nodeName;
            Method = method;
            InputTypes = [.. nodeInputTypes.Select(p => p.Type)];
            OutputTypes = [.. nodeOutputTypes.Select(p => p.Type)];
            CallMarshal = nodeInputValues =>
            {
                // TODO: Handle "action event" (graph endpoint reference, embedded subgraph, code snippet) (per ActionEventPickerWindow)

                object[] nonOutMethodParameterValues = nodeInputValues.Skip(Method.IsStatic ? 0 : 1).ToArray();

                int current = 0;
                ParameterInfo[] declaredParameters = Method.Parameters;
                object?[] methodExpectedParameterValuesArray = new object[declaredParameters.Length];
                List<int> outParameterIndices = [];
                for (int i = 0; i < declaredParameters.Length; i++)
                {
                    ParameterInfo item = declaredParameters[i];
                    if (item.IsOut && item.ParameterType.IsByRef) // `Out` parameter
                    {
                        methodExpectedParameterValuesArray[i] = null;
                        outParameterIndices.Add(i);
                    }
                    else
                    {
                        // Remark: notice the addition of this step could potentially make reflection based interpretative execution significantly slower

                        // Automatic conversion of number values
                        if (nonOutMethodParameterValues[current] != null && nonOutMethodParameterValues[current].GetType() != item.ParameterType)
                        {
                            if (TypeHelper.TryConvert(nonOutMethodParameterValues[current], item.ParameterType, out object? result))
                                methodExpectedParameterValuesArray[i] = result;
                        }
                        else
                            methodExpectedParameterValuesArray[i] = nonOutMethodParameterValues[current];
                        current++;
                    }
                }
                if (Method.IsStatic)
                {
                    object? returnValue = Method.StaticInvoke(methodExpectedParameterValuesArray);
                    if (Method.ReturnType == typeof(void))
                        return [.. outParameterIndices.Select(i => methodExpectedParameterValuesArray[i])];
                    else
                        return [returnValue, .. outParameterIndices.Select(i => methodExpectedParameterValuesArray[i])];
                }
                else
                {
                    object? instance = nodeInputValues[0];
                    object? returnValue = Method.Invoke(instance, methodExpectedParameterValuesArray);
                    if (Method.ReturnType == typeof(void))
                        return [instance, .. outParameterIndices.Select(i => methodExpectedParameterValuesArray[i])];
                    else
                        return [instance, returnValue, .. outParameterIndices.Select(i => methodExpectedParameterValuesArray[i])];
                }
            };
            InputNames = [.. nodeInputTypes.Select(t => t.ParameterName)];
            OutputNames = [.. nodeOutputTypes.Select(t => t.ParameterName)];
            DefaultInputValues = [.. nodeInputTypes.Select(t => t.DefaultValue)];
        }
        #endregion

        #region Helper
        private static void SeperateMethodParametersForNode(Callable method, out (Type, string, object?)[] nodeInputTypes, out (Type ValueType, string ValueName)[] nodeOutputTypes)
        {
            ParameterInfo[] methodParameters = method.Parameters;

            // Goal: Deal with instance methods, `out` (and don't worry about `ref`) parameters and method return type
            // Rules:
            // - Instance methods will have an artificial "instance" reference pin, like C/Python style "static" instance function
            // - Instance methods also automatically have an additional instance output pin (DO NOT provide fluent API in package implementation on the instance level!)
            // - `Ref` parameter is not touched
            // - `Out` parameter is treated as additional return value
            // Example calculation for clarity:
            //int nodeInputsCount = methodParameters.Where(p => !(p.IsOut && p.ParameterType.IsByRef) /* Disregard `out` parameters, keep `ref` */).Count() + (method.IsStatic ? 1 : 0);
            //int nodeOutputsCount = methodParameters.Where(p => (p.IsOut && p.ParameterType.IsByRef) /* Keep `out` parameters*/).Count() + (method.IsStatic ? 0 : 1);

            // Gather inputs and outputs
            List<(Type, string, object?)> nodeInputTypesList = [];
            List<(Type, string)> nodeOutputTypesList = [];

            if (!method.IsStatic)
            {
                nodeInputTypesList.Add((method.DeclaringType!, method.DeclaringType!.Name, null));
                nodeOutputTypesList.Add((method.DeclaringType!, FormalizeUnnammedPin(method.DeclaringType!))); // TODO: Pending POS standardization how should we name the return value pin by default
            }

            // Input order: (Instance), remaining (non-out) parameters
            foreach (ParameterInfo parameter in methodParameters.Where(p => !(p.IsOut && p.ParameterType.IsByRef)))
                nodeInputTypesList.Add((parameter.ParameterType, parameter.Name!, parameter.DefaultValue));

            // Output order: (Instance), (method return), any out parameters
            if (method.ReturnType != typeof(void))
                nodeOutputTypesList.Add((method.ReturnType, FormalizeUnnammedPin(method.ReturnType!)));
            foreach (ParameterInfo parameter in methodParameters.Where(p => (p.IsOut && p.ParameterType.IsByRef)))
                nodeOutputTypesList.Add((parameter.ParameterType, parameter.Name!));

            // Return
            nodeInputTypes = [.. nodeInputTypesList];
            nodeOutputTypes = [.. nodeOutputTypesList];

            static string FormalizeUnnammedPin(Type type)
            {
                string defaultName = type.Name;
                if (type.IsArray) // TODO: Notice IsArray is potentially unsafe since it doesn't work on pass by ref arrays e.g. System.Double[]&; Consider using HasElementType
                    return $"{FormalizeUnnammedPin(type.GetElementType())} Array";
                else return defaultName;
            }
        }
        #endregion
    }
}
