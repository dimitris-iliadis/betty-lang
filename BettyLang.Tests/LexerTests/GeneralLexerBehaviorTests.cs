namespace BettyLang.Tests.LexerTests
{
    public class GeneralLexerBehaviorTests
    {
        [Theory]
        [InlineData("123", "123")]
        [InlineData("0", "0")]
        [InlineData("123.456", "123.456")]
        [InlineData(".789", "0.789")]
        [InlineData("0.1", "0.1")]
        public void GetNextToken_HandlesNumberLiteralsCorrectly(string input, string expectedValue)
        {
            var lexer = new Lexer(input);

            var token = lexer.GetNextToken();

            Assert.Equal(TokenType.NumberLiteral, token.Type);
            Assert.Equal(expectedValue, token.Value);
        }

        [Theory]
        [InlineData(@"Hello", "Hello")]
        [InlineData(@"", "")]
        [InlineData(@"\\", "\\")]
        [InlineData(@"String with spaces", "String with spaces")]
        [InlineData(@"123", "123")]
        [InlineData(@"String with \""escaped quotes\""", "String with \"escaped quotes\"")]
        [InlineData(@"Newline\nCharacter", "Newline\nCharacter")]
        [InlineData(@"Tab\tCharacter", "Tab\tCharacter")]
        public void GetNextToken_HandlesStringLiteralsCorrectly(string input, string expectedValue)
        {
            var lexer = new Lexer($"\"{input}\"");

            var token = lexer.GetNextToken();

            Assert.Equal(TokenType.StringLiteral, token.Type);
            Assert.Equal(expectedValue, token.Value);
        }

        [Theory]
        [InlineData("true", TokenType.BooleanLiteral)]
        [InlineData("false", TokenType.BooleanLiteral)]
        public void GetNextToken_HandlesBooleanLiteralsCorrectly(string input, TokenType expectedTokenType)
        {
            var lexer = new Lexer(input);

            var token = lexer.GetNextToken();

            Assert.Equal(expectedTokenType, token.Type);
            Assert.Equal(input, token.Value);
        }

        [Fact]
        public void GetNextToken_ReturnsEOFForEmptyInput()
        {
            var lexer = new Lexer("");

            var token = lexer.GetNextToken();

            Assert.Equal(TokenType.EOF, token.Type);
        }

        [Theory]
        [InlineData("func", TokenType.Function)]
        [InlineData("if", TokenType.If)]
        [InlineData("elif", TokenType.Elif)]
        [InlineData("else", TokenType.Else)]
        [InlineData("while", TokenType.While)]
        [InlineData("break", TokenType.Break)]
        [InlineData("continue", TokenType.Continue)]
        [InlineData("return", TokenType.Return)]
        public void GetNextToken_HandlesKeywordsCorrectly(string input, TokenType expectedTokenType)
        {
            var lexer = new Lexer(input);

            var token = lexer.GetNextToken();

            Assert.Equal(expectedTokenType, token.Type);
        }

        [Fact]
        public void GetNextToken_ParsesSequenceOfTokensCorrectly()
        {
            // Arrange
            var input = "x = 10;";
            var lexer = new Lexer(input);
            var expectedTokens = new List<TokenType> {
                TokenType.Identifier, TokenType.Assignment,
                TokenType.NumberLiteral, TokenType.Semicolon,
                TokenType.EOF };

            // Act
            var tokens = new List<TokenType>();
            Token token;
            do
            {
                token = lexer.GetNextToken();
                tokens.Add(token.Type);
            } while (token.Type != TokenType.EOF);

            // Assert
            Assert.Equal(expectedTokens, tokens);
        }

        [Fact]
        public void PeekNextToken_ReturnsNextTokenWithoutAdvancingPosition()
        {
            // Arrange
            var input = "var x = 10;";
            var lexer = new Lexer(input);

            // Act
            var peekedToken = lexer.PeekNextToken();
            var nextToken = lexer.GetNextToken();

            // Assert
            Assert.Equal(peekedToken.Type, nextToken.Type);
        }

        [Fact]
        public void ScanStringLiteral_ThrowsExceptionForUnterminatedString()
        {
            // Arrange
            var input = "\"Hello";
            var lexer = new Lexer(input);

            // Act & Assert
            Assert.Throws<Exception>(() => lexer.GetNextToken());
        }

        [Fact]
        public void ScanStringLiteral_ThrowsExceptionForUnrecognizedEscapeSequence()
        {
            // Arrange
            var input = "\"Hello\\x\"";
            var lexer = new Lexer(input);

            // Act & Assert
            Assert.Throws<Exception>(() => lexer.GetNextToken());
        }
    }
}