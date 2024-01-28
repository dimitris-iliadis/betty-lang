namespace BettyLang.Tests.LexerTests
{
    public class StringTests : LexerTest
    {
        [Fact]
        public void Lexer_RecognizesSimpleStringLiterals()
        {
            var token = GetSingleTokenFromLexer("\"Hello, world!\"");
            Assert.Equal(TokenType.StringLiteral, token.Type);
            Assert.Equal("Hello, world!", token.Value);
        }

        [Fact]
        public void Lexer_RecognizesStringsWithEscapedQuotes()
        {
            var token = GetSingleTokenFromLexer("\"She said, \\\"Hi!\\\"\"");
            Assert.Equal(TokenType.StringLiteral, token.Type);
            Assert.Equal("She said, \"Hi!\"", token.Value);
        }

        [Fact]
        public void Lexer_RecognizesStringsContainingNewLines()
        {
            var token = GetSingleTokenFromLexer("\"Line1\\nLine2\"");
            Assert.Equal(TokenType.StringLiteral, token.Type);
            Assert.Equal("Line1\nLine2", token.Value);
        }

        [Fact]
        public void Lexer_RecognizesStringsContainingTabs()
        {
            var token = GetSingleTokenFromLexer("\"Column1\\tColumn2\"");
            Assert.Equal(TokenType.StringLiteral, token.Type);
            Assert.Equal("Column1\tColumn2", token.Value);
        }

        [Fact]
        public void Lexer_ThrowsExceptionForUnterminatedStrings()
        {
            Assert.Throws<Exception>(() => GetSingleTokenFromLexer("\"Unterminated string"));
        }

        [Fact]
        public void Lexer_RecognizesEmptyStrings()
        {
            var token = GetSingleTokenFromLexer("\"\"");
            Assert.Equal(TokenType.StringLiteral, token.Type);
            Assert.Equal("", token.Value);
        }

        [Fact]
        public void Lexer_ThrowsExceptionForUnrecognizedEscapeSequences()
        {
            Assert.Throws<Exception>(() => GetSingleTokenFromLexer("\"Invalid \\escape\""));
        }
    }
}