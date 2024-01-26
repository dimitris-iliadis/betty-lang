using BettyLang.Core;

namespace BettyLang.CLI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string source = """
                    function myfunc()
                    {
                        print 5;
                        continue;
                    }

                    main
                    {
                        break;

                         while (true)
                            myfunc();
                    }
                    """;

                var lexer = new Lexer(source);
                var parser = new Parser(lexer);
                var interpreter = new Interpreter(parser);
                interpreter.Interpret();
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}