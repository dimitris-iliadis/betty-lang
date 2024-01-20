using BettyLang.Core;

namespace BettyLang.CLI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("betty> ");
                string? input = Console.ReadLine();
                if (input is null) continue;
                var lexer = new Lexer(input);
                var parser = new Parser(lexer);
                var interpreter = new Interpreter(parser);
                InterpreterResult result = interpreter.Interpret();
                Console.WriteLine(result.AsString());
            }
        }
    }
}