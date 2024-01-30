using BettyLang.Core.AST;

namespace BettyLang.Tests.TestUtilities
{
    public class ParserTest
    {
        protected Parser SetupParser(string code)
        {
            var lexer = new Lexer(code);
            return new Parser(lexer);
        }
    }
}