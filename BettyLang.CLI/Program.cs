using BettyLang.Core;

namespace BettyLang.CLI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.Write("betty> ");
                string input = """
                    main
                    {
                        a = 10;
                        if (a < 1) {
                            a = 13; // This is a comment
                        }
                        elif (a > 5)
                        {
                            print "Yo!";
                        }
                        else
                        {
                            print "Wow! You're here\n";
                        }
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