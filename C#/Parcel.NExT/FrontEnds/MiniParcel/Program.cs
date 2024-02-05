using Parcel.CoreEngine;
using Parcel.CoreEngine.Document;
using Parcel.CoreEngine.MiniParcel;
using Parcel.CoreEngine.Service.CoreExtensions;
using Parcel.CoreEngine.Versioning;
using System.Text;

namespace MiniParcel
{
    internal class Program
    {
        #region Sub-Commands
        private static Dictionary<string, Action<string[]>> SubCommands = new ()
        {
            { "Convert", Convert }
        };
        #endregion

        #region Entry Point
        static void Main(string[] args)
        {
            // REPL
            if (args.Length == 0)
            {
                Console.WriteLine($"""
                    Welcome to MiniParcel - the REPL & CLI front-end for Parcel
                    Engine Version: {EngineVersion.Version}
                    This is the REPL mode. 
                    Type `help` for available commands; Type `exit` to leave.
                    """);
                REPL();
            }
            else if (args.Length == 1 && args.First() == "--io")
                IOMode();
            // Print help
            else if (args.Length == 1 && args.First() == "--help")
                PrintHelp();
            // Run script
            else if (File.Exists(args.First()))
                RunScript(Path.GetFullPath(args.First()), args.Skip(1).ToArray());
            // Run subcommands
            else if (SubCommands.ContainsKey(args.First()))
                RunCommand(SubCommands[args.First()], args.Skip(1).ToArray());
            // Invalid
            else
            {
                Console.WriteLine($"Invalid command line format: {Environment.CommandLine}");
                PrintHelp();
            }
        }

        #endregion

        #region Execution Paths
        private static void IOMode()
        {
            StringBuilder script = new();
            while (true)
            {
                Console.Write("> ");
                string? input = Console.ReadLine();
                if (input == null || input == "exit")
                    break;

                script.AppendLine(input);
            }

            MiniParcelService.Parse(script.ToString()).Execute();
        }
        private static void REPL()
        {
            StringBuilder history = new();

            ParcelDocument document = new();
            while (true)
            {
                Console.Write("> ");
                string? input = Console.ReadLine();
                if (input == null || input == "exit")
                    break;
                else if (input == "help")
                    Console.WriteLine(GatherREPLHelp());
                else if (input == "save")
                    File.WriteAllText("Command History.txt", history.ToString().TrimEnd());

                // Run nodes interactively
                ParcelNode node = MiniParcelService.ParseAsNode(document, input);
                ParcelPayload payload = node.Execute();

                // Append to history
                history.AppendLine(input);
            }

            static string GatherREPLHelp()
            {
                return $"""
                    Available commands:
                      help: Print this help.
                      exit: Leave the REPL.
                      save: Save command history.
                    """;
            }
        }
        private static void PrintHelp()
        {
            Console.WriteLine($"""
                Arguments:
                  (No Argument): Enter REPL mode.
                  --help: Print this help.
                  <FilePath> [<Arguments>...]: Run document.
                  <Command> [<Arguments>...]: Run subcommand.
                Subcommands:
                  {string.Join("\n  ", SubCommands.Keys)}
                """);
        }
        private static void RunScript(string inputFile, string[] strings)
        {
            string script = File.ReadAllText(inputFile);
            MiniParcelService.Parse(script).Execute();
        }
        private static void RunCommand(Action<string[]> action, string[] arguments)
            => action(arguments);
        #endregion

        #region Sub-Commands
        public static void Convert(string[] arguments)
        {
            string inputFile = arguments.First();
            string outputFile = arguments.Last();

            ParcelDocument document = MiniParcelService.Parse(File.ReadAllText(inputFile));
            document.Save(outputFile);
        }
        #endregion
    }
}
