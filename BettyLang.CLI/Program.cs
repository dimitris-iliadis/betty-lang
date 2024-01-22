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
                        a = "hello"; a = 3; // This is a comment
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