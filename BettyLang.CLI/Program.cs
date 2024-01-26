using BettyLang.Core;

namespace BettyLang.CLI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string source = args.Length > 0 && !string.IsNullOrEmpty(args[0])
                ? args[0]
                : throw new ArgumentException("No script file specified.");

                var lexer = new Lexer(source);
                var parser = new Parser(lexer);
                var interpreter = new Interpreter(parser);
                interpreter.Interpret();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}