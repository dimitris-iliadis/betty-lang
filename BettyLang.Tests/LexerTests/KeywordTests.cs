namespace BettyLang.Tests.LexerTests
{
    public class KeywordTests : LexerTest
    {
        [Fact]
        public void FunctionKeyword()
        {
            var input = "func";
            var token = GetSingleTokenFromLexer(input);
            Assert.Equal(TokenType.Function, token.Type);
            Assert.Equal("func", token.Value);
        }

        [Fact]
        public void TrueKeyword()
        {
            var input = "true";
            var token = GetSingleTokenFromLexer(input);
            Assert.Equal(TokenType.TrueLiteral, token.Type);
            Assert.Equal("true", token.Value);
        }

        [Fact]
        public void FalseKeyword()
        {
            var input = "false";
            var token = GetSingleTokenFromLexer(input);
            Assert.Equal(TokenType.FalseLiteral, token.Type);
            Assert.Equal("false", token.Value);
        }

        [Fact]
        public void IfKeyword()
        {
            var input = "if";
            var token = GetSingleTokenFromLexer(input);
            Assert.Equal(TokenType.If, token.Type);
            Assert.Equal("if", token.Value);
        }

        [Fact]
        public void ElifKeyword()
        {
            var input = "elif";
            var token = GetSingleTokenFromLexer(input);
            Assert.Equal(TokenType.Elif, token.Type);
            Assert.Equal("elif", token.Value);
        }

        [Fact]
        public void ElseKeyword()
        {
            var input = "else";
            var token = GetSingleTokenFromLexer(input);
            Assert.Equal(TokenType.Else, token.Type);
            Assert.Equal("else", token.Value);
        }

        [Fact]
        public void WhileKeyword()
        {
            var input = "while";
            var token = GetSingleTokenFromLexer(input);
            Assert.Equal(TokenType.While, token.Type);
            Assert.Equal("while", token.Value);
        }

        [Fact]
        public void BreakKeyword()
        {
            var input = "break";
            var token = GetSingleTokenFromLexer(input);
            Assert.Equal(TokenType.Break, token.Type);
            Assert.Equal("break", token.Value);
        }

        [Fact]
        public void ContinueKeyword()
        {
            var input = "continue";
            var token = GetSingleTokenFromLexer(input);
            Assert.Equal(TokenType.Continue, token.Type);
            Assert.Equal("continue", token.Value);
        }

        [Fact]
        public void ReturnKeyword()
        {
            var input = "return";
            var token = GetSingleTokenFromLexer(input);
            Assert.Equal(TokenType.Return, token.Type);
            Assert.Equal("return", token.Value);
        }
    }
}