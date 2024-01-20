using BettyLang.Core.AST;

namespace BettyLang.Core
{
    public class Parser
    {
        private readonly Lexer _lexer;
        private Token _currentToken;

        public Parser(Lexer lexer)
        {
            _lexer = lexer;
            _currentToken = lexer.GetNextToken();
        }

        private void Eat(TokenType tokenType)
        {
            if (_currentToken.Type == tokenType)
                _currentToken = _lexer.GetNextToken();
            else
                throw new Exception($"Unexpected token type {_currentToken.Type}, expected {tokenType}");
        }

        private Node ParseString()
        {
            var token = _currentToken;
            Eat(TokenType.StringLiteral);
            return new StringNode(token.Value.ToString()!);
        }

        private Node ParseFactor()
        {
            var token = _currentToken;

            if (token.Type == TokenType.Plus)
            {
                Eat(TokenType.Plus);
                var node = new UnaryOperatorNode(token, ParseFactor());
                return node;
            }
            else if (token.Type == TokenType.Minus)
            {
                Eat(TokenType.Minus);
                var node = new UnaryOperatorNode(token, ParseFactor());
                return node;
            }
            else if (token.Type == TokenType.NumberLiteral)
            {
                Eat(TokenType.NumberLiteral);
                return new NumberNode(token);
            }
            else if (token.Type == TokenType.LParen)
            {
                Eat(TokenType.LParen);
                var node = ParseExpression();
                Eat(TokenType.RParen);
                return node;
            }

            throw new Exception($"Unexpected token type {_currentToken.Type}");
        }

        private Node ParseTerm()
        {
            if (_currentToken.Type == TokenType.StringLiteral)
                return ParseString();

            var node = ParseExponentiation();

            while (_currentToken.Type == TokenType.Mul || _currentToken.Type == TokenType.Div)
            {
                var token = _currentToken;
                if (token.Type == TokenType.Mul)
                    Eat(TokenType.Mul);
                else if (token.Type == TokenType.Div)
                    Eat(TokenType.Div);

                node = new BinaryOperatorNode(node, token, ParseExponentiation());
            }

            return node;
        }

        private Node ParseExponentiation()
        {
            var node = ParseFactor();

            while (_currentToken.Type == TokenType.Caret)
            {
                var token = _currentToken;
                Eat(TokenType.Caret);
                node = new BinaryOperatorNode(node, token, ParseExponentiation());
            }

            return node;
        }

        private Node ParseExpression()
        {
            var node = ParseTerm();

            while (_currentToken.Type == TokenType.Plus || _currentToken.Type == TokenType.Minus)
            {
                var token = _currentToken;
                if (token.Type == TokenType.Plus)
                    Eat(TokenType.Plus);
                else if (token.Type == TokenType.Minus)
                    Eat(TokenType.Minus);

                node = new BinaryOperatorNode(node, token, ParseTerm());
            }

            return node;
        }

        public Node Parse() => ParseExpression();
    }
}