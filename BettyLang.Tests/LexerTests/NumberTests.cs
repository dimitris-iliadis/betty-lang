namespace BettyLang.Tests.LexerTests
{
    public class NumberTests : LexerTest
    {
        [Fact]
        public void Lexer_RecognizesIntegerLiterals()
        {
            var token = GetSingleTokenFromLexer("123");
            Assert.Equal(TokenType.NumberLiteral, token.Type);
            Assert.Equal("123", token.Value);
        }

        [Fact]
        public void Lexer_RecognizesFloatingPointLiterals()
        {
            var token = GetSingleTokenFromLexer("123.456");
            Assert.Equal(TokenType.NumberLiteral, token.Type);
            Assert.Equal("123.456", token.Value);
        }

        [Fact]
        public void Lexer_RecognizesNumbersWithLeadingDot()
        {
            var token = GetSingleTokenFromLexer(".456");
            Assert.Equal(TokenType.NumberLiteral, token.Type);
            Assert.Equal("0.456", token.Value);
        }

        [Fact]
        public void Lexer_ThrowsExceptionForInvalidNumberFormat()
        {
            Assert.Throws<FormatException>(() => GetSingleTokenFromLexer("123..456"));
        }

        [Fact]
        public void Lexer_RecognizesLargeNumbers()
        {
            var token = GetSingleTokenFromLexer("12345678901234567890");
            Assert.Equal(TokenType.NumberLiteral, token.Type);
            Assert.Equal("12345678901234567890", token.Value);
        }
    }
}