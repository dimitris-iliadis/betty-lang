using BettyLang.Core;

namespace BettyLang.CLI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    Console.Write("betty> ");
                    string? input = Console.ReadLine();
                    if (input is null) continue;
                    var lexer = new Lexer(input);
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
}