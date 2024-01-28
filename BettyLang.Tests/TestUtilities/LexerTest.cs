namespace BettyLang.Tests.TestUtilities
{
    public class LexerTest
    {
        protected Token GetSingleTokenFromLexer(string input)
        {
            var lexer = new Lexer(input);
            return lexer.GetNextToken();
        }
    }
}