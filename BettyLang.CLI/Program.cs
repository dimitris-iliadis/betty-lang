using BettyLang.Core;

namespace BettyLang.CLI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string input = """
                    main
                    {
                        a = 2^5 < 3 && true;
                    }
                    """;
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