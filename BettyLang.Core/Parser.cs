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
                throw new Exception($"Unexpected token: Expected {tokenType}, found {_currentToken.Type}");
        }

        private Node ParseString()
        {
            var token = _currentToken;
            Eat(TokenType.StringLiteral);
            return new StringNode(token.Value.ToString()!);
        }

        private Node ParseVariable()
        {
            var node = new VariableNode(_currentToken);
            Eat(TokenType.Identifier);
            return node;
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
            else
            {
                var node = ParseVariable();
                return node;
            }
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

        private Node ParseProgram()
        {
            var node = ParseCompoundStatement();
            return node;
        }

        private Node ParseCompoundStatement()
        {
            Eat(TokenType.LBracket);
            var nodes = ParseStatementList();
            Eat(TokenType.RBracket);

            var root = new CompoundStatementNode();
            foreach (var node in nodes)
                root.Children.Add(node);

            return root;
        }

        private List<Node> ParseStatementList()
        {
            var node = ParseStatement();

            var results = new List<Node>() { node };

            while (_currentToken.Type == TokenType.Semicolon)
            {
                Eat(TokenType.Semicolon);
                results.Add(ParseStatement());
            }

            if (_currentToken.Type == TokenType.Identifier)
                throw new Exception($"Unexpected token: {_currentToken.Type}");

            return results;
        }

        private Node ParseStatement()
        {
            Node node;
            if (_currentToken.Type == TokenType.LBracket)
                node = ParseCompoundStatement();
            else if (_currentToken.Type == TokenType.Identifier)
                node = ParseAssignmentStatement();
            else
                node = ParseEmptyStatement();

            return node;
        }

        private Node ParseAssignmentStatement()
        {
            var left = ParseVariable();
            var token = _currentToken;
            Eat(TokenType.Equals);
            var right = ParseExpression();
            var node = new AssignmentNode(left, token, right);
            return node;
        }

        private Node ParseEmptyStatement() => new EmptyStatementNode();

        public Node Parse()
        {
            var node = ParseProgram();

            if (_currentToken.Type != TokenType.EOF)
                throw new Exception($"Unexpected token: {_currentToken.Type}");

            return node;
        }
    }
}