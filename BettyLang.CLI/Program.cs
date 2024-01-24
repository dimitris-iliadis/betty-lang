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
                        a = 10 + 2;
                        print "Hello World " + a + "\n";
                        if (true && 2 > 1) {
                            a = 13;
                            print "Alright!"; // This is a comment
                        }
                        print a;
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