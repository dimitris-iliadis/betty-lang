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

        private void Consume(TokenType tokenType)
        {
            if (_currentToken.Type == tokenType)
                _currentToken = _lexer.GetNextToken();
            else
                throw new Exception($"Unexpected token: Expected {tokenType}, found {_currentToken.Type}");
        }

        private ASTNode ParseString()
        {
            var token = _currentToken;
            Consume(TokenType.StringLiteral);
            return new StringNode(token.Value.ToString()!);
        }

        private ASTNode ParseVariable()
        {
            var node = new VariableNode(_currentToken);
            Consume(TokenType.Identifier);
            return node;
        }

        private ASTNode ParseFactor()
        {
            var token = _currentToken;

            if (token.Type == TokenType.Plus)
            {
                Consume(TokenType.Plus);
                var node = new UnaryOperatorNode(token, ParseFactor());
                return node;
            }
            else if (token.Type == TokenType.Minus)
            {
                Consume(TokenType.Minus);
                var node = new UnaryOperatorNode(token, ParseFactor());
                return node;
            }
            else if (token.Type == TokenType.NumberLiteral)
            {
                Consume(TokenType.NumberLiteral);
                return new NumberNode(token);
            }
            else if (token.Type == TokenType.LParen)
            {
                Consume(TokenType.LParen);
                var node = ParseExpression();
                Consume(TokenType.RParen);
                return node;
            }
            else
            {
                var node = ParseVariable();
                return node;
            }
        }

        private ASTNode ParseTerm()
        {
            if (_currentToken.Type == TokenType.StringLiteral)
                return ParseString();

            var node = ParseExponentiation();

            while (_currentToken.Type == TokenType.Mul || _currentToken.Type == TokenType.Div)
            {
                var token = _currentToken;
                if (token.Type == TokenType.Mul)
                    Consume(TokenType.Mul);
                else if (token.Type == TokenType.Div)
                    Consume(TokenType.Div);

                node = new BinaryOperatorNode(node, token, ParseExponentiation());
            }

            return node;
        }

        private ASTNode ParseExponentiation()
        {
            var node = ParseFactor();

            while (_currentToken.Type == TokenType.Caret)
            {
                var token = _currentToken;
                Consume(TokenType.Caret);
                node = new BinaryOperatorNode(node, token, ParseExponentiation());
            }

            return node;
        }

        private ASTNode ParseExpression()
        {
            var node = ParseTerm();

            while (_currentToken.Type == TokenType.Plus || _currentToken.Type == TokenType.Minus)
            {
                var token = _currentToken;
                if (token.Type == TokenType.Plus)
                    Consume(TokenType.Plus);
                else if (token.Type == TokenType.Minus)
                    Consume(TokenType.Minus);

                node = new BinaryOperatorNode(node, token, ParseTerm());
            }

            return node;
        }

        private CompoundStatementNode ParseCompoundStatement()
        {
            Consume(TokenType.LBracket);
            var nodes = ParseStatementList();
            Consume(TokenType.RBracket);

            var root = new CompoundStatementNode();
            foreach (var node in nodes)
                root.Statements.Add(node);

            return root;
        }

        private List<ASTNode> ParseStatementList()
        {
            var node = ParseStatement();

            var results = new List<ASTNode>() { node };

            while (_currentToken.Type == TokenType.Semicolon)
            {
                Consume(TokenType.Semicolon);
                results.Add(ParseStatement());
            }

            if (_currentToken.Type == TokenType.Identifier)
                throw new Exception($"Unexpected token: {_currentToken.Type}");

            return results;
        }

        private ASTNode ParseStatement()
        {
            ASTNode node;
            if (_currentToken.Type == TokenType.LBracket)
                node = ParseCompoundStatement();
            else if (_currentToken.Type == TokenType.Identifier)
                node = ParseAssignmentStatement();
            else
                node = ParseEmptyStatement();

            return node;
        }

        private ASTNode ParseAssignmentStatement()
        {
            var left = ParseVariable();
            var token = _currentToken;
            Consume(TokenType.Equal);
            var right = ParseExpression();
            var node = new AssignmentNode(left, token, right);
            return node;
        }
        private ASTNode ParseEmptyStatement() => new EmptyStatementNode();

        private FunctionDefinitionNode ParseFunctionDefinition()
        {
            // Assuming "function" token is already consumed
            string functionName = _currentToken.Value;
            Consume(TokenType.Identifier); // Function name

            Consume(TokenType.LParen); // Opening parenthesis
            List<ParameterNode> parameters = ParseParameters();
            Consume(TokenType.RParen); // Closing parenthesis

            CompoundStatementNode body = ParseCompoundStatement(); // Function body

            return new FunctionDefinitionNode(functionName, parameters, body);
        }

        private List<ParameterNode> ParseParameters()
        {
            List<ParameterNode> parameters = new List<ParameterNode>();

            if (_currentToken.Type != TokenType.RParen) // Check if parameter list is empty
            {
                do
                {
                    if (_currentToken.Type == TokenType.Identifier)
                    {
                        string paramName = _currentToken.Value.ToString();
                        parameters.Add(new ParameterNode(paramName));
                        Consume(TokenType.Identifier);
                    }
                    else
                    {
                        throw new Exception($"Expected an identifier, found {_currentToken.Type}.");
                    }

                    if (_currentToken.Type == TokenType.Comma)
                    {
                        Consume(TokenType.Comma); // Eat comma and expect another parameter
                    }
                }
                while (_currentToken.Type != TokenType.RParen);
            }

            return parameters;
        }

        private CompoundStatementNode ParseMainBlock() => ParseCompoundStatement();

        private ASTNode ParseProgram()
        {
            List<FunctionDefinitionNode> functions = new List<FunctionDefinitionNode>();
            CompoundStatementNode mainBlock = null;

            while (_currentToken.Type != TokenType.EOF && _currentToken.Type != TokenType.Main)
            {
                if (_currentToken.Type == TokenType.Function)
                {
                    Consume(TokenType.Function);
                    functions.Add(ParseFunctionDefinition());
                }
                else
                    throw new Exception("Unexpected token: " + _currentToken.Type);
            }

            if (_currentToken.Type == TokenType.Main)
            {
                Consume(TokenType.Main);
                mainBlock = ParseMainBlock();
            }
            else
                throw new Exception("Missing main block in the program");

            return new ProgramNode(functions, mainBlock);
        }

        public ASTNode Parse()
        {
            var node = ParseProgram();

            if (_currentToken.Type != TokenType.EOF)
                throw new Exception($"Unexpected token: {_currentToken.Type}");

            return node;
        }
    }
}