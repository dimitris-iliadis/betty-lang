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
                        a = 10;
                        while (a >= 1)
                        {
                            print "Hehe\n";
                            a = a - 1;
                            if (a == 5)
                            {
                                print "Nah";
                                break;
                            }
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