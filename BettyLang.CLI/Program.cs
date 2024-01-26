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
                    function fact(n)
                    {
                        if (n == 1) { return 1; }
                        return fact(n - 1) * n;
                    }

                    main
                    {
                        n = 1;
                        while (n <= 6)
                        {
                            print fact(n) + "\n";
                            n = n + 1;
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