﻿namespace BettyLang.Tests.LexerTests
{
    public class KeywordTests
    {
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
    }
}