﻿using Parcel.CoreEngine.Helpers;
using Parcel.NExT.Interpreter;
using Parcel.NExT.Interpreter.Helpers;
using System.Collections.Generic;

namespace Pure
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Help
            if (args.Length == 1 && args.Single().Equals("--help", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"""
                    Pure 2 Interactive Interpreter (REPL) (Core Version: {Interpreter.DistributionVersion})
                    CLI Variants:
                      pure: REPL mode
                      pure --version: Print version
                      pure --help: Print this help
                      pure <File Path> [<Arguments>...]: Run script from default paths (current folder, PUREPATH)
                      pure -i <File Path> [<Arguments>...]: Run script from default paths (current folder, PUREPATH) and enter interactive mode
                      pure -m <Expression> [<Arguments>...]: Evaluate expression
                      pure -l/mi <File Path> <Expression>: Load file then evaluate expression
                    """);
            }
            // Version
            else if (args.Length == 1 && args.Single().Equals("--version", StringComparison.OrdinalIgnoreCase))
                Console.WriteLine(Interpreter.DistributionVersion);
            // REPL mode
            else if (args.Length == 0)
            {
                var interpreter = new Interpreter($"""
                    Pure 2 Interactive Interpreter (REPL) (Core Version: {Interpreter.DistributionVersion})
                    Powered by Parcel.NExT, a visual programming platform.
                    Type Help() or use --help to view help.{AdditionalWelcome()}
                    """, null, null, null, null);
                interpreter.Start();
                EnterInteractiveMode(interpreter);
            }
            // Evaluate expression
            else if (args.Length >= 2 && args[0] == "-m")
                new Interpreter(string.Empty, null, args.Skip(2).ToArray(), [args[1]], null)
                    .Start();
            // Load file and evaluate
            else if (args.Length == 3 && (args[0] == "-l" || args[0] == "-mi"))
            {
                string fileName = args[1];
                string expression = args[2];
                string scriptPath = PathHelper.FindScriptFileFromEnvPath(fileName);
                new Interpreter(string.Empty, scriptPath, null, [.. Interpreter.SplitScripts(File.ReadAllText(scriptPath)), .. new string[] { expression }], null)
                    .Start();
            }
            // Execute file with optional interactivity
            else if (args.Length >= 1) 
            {
                bool interactiveMode = args.Length >= 2 && (args[0].Equals("-i", StringComparison.OrdinalIgnoreCase) || args[0].Equals("--interactive", StringComparison.OrdinalIgnoreCase));
                string fileName = interactiveMode ? args[1] : args[0];

                string file = PathHelper.FindScriptFileFromEnvPath(fileName);
                if (!File.Exists(file))
                {
                    Console.WriteLine($"File {file} doesn't exist.");
                    return; 
                }
                var interpreter = new Interpreter(string.Empty, file, interactiveMode ? args.Skip(2).ToArray() : args.Skip(1).ToArray(), Interpreter.SplitScripts(File.ReadAllText(file)), file.GetDeterministicHashCode().ToString());
                interpreter.Start();
                if(interactiveMode)
                    EnterInteractiveMode(interpreter);
            }

            // Just for fun
            static string AdditionalWelcome()
            {
                string quote = string.Empty;
                if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
                    quote = "Friday is a good day!";
                else quote = QuotesHelper.GetInspiringQuotes((int)(DateTime.Now - DateTime.MinValue).TotalDays);
                if (quote != string.Empty)
                    return $"\n{quote}";
                else return quote;
            }
        }

        private static void EnterInteractiveMode(Interpreter interpreter)
        {
            while (true)
            {
                Console.Write(">>> ");
                string input = Console.ReadLine().Trim();

                if (input == "exit" || input == "exit()")
                    return;
                interpreter.Parse(input);
            }
        }
    }
}