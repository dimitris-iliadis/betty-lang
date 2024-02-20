﻿using BettyLang.Core.AST;

namespace BettyLang.Core
{
    public class Parser
    {
        private readonly Lexer _lexer;
        private Token _currentToken;
        private readonly HashSet<string> _definedFunctions = [];

        private static readonly HashSet<TokenType> _comparisonOperators = new()
        {
            TokenType.GreaterThan,
            TokenType.LessThan,
            TokenType.GreaterThanOrEqual,
            TokenType.LessThanOrEqual,
            TokenType.EqualEqual,
            TokenType.NotEqual
        };

        private static readonly HashSet<TokenType> _assignmentOperators = new()
        {
            TokenType.Equal,
            TokenType.PlusEqual,
            TokenType.MinusEqual,
            TokenType.MulEqual,
            TokenType.DivEqual,
            TokenType.CaretEqual,
            TokenType.ModEqual,
            TokenType.IntDivEqual
        };

        private static readonly HashSet<TokenType> _multiplicativeOperators = new()
        {
            TokenType.Mul,
            TokenType.Div,
            TokenType.IntDiv,
            TokenType.Mod
        };

        public Parser(Lexer lexer)
        {
            _lexer = lexer;
            _currentToken = _lexer.GetNextToken(); // Set current token to the first token from the input
        }

        private void Consume(TokenType tokenType)
        {
            if (_currentToken.Type == tokenType)
            {
                _currentToken = _lexer.GetNextToken();
            }
            else
            {
                throw new Exception($"Unexpected token: Expected {tokenType}, found {_currentToken.Type} at line {_currentToken.Line}, column {_currentToken.Column}");
            }
        }

        private FunctionCall ParseRange()
        {
            var start = ParseExpression(); // Parse the start of the range
            Consume(TokenType.DotDot);
            var end = ParseExpression(); // Parse the end of the range
            Consume(TokenType.RBracket); // Consume the closing bracket (end of the range)

            // Construct and return a FunctionCall AST node for the range function
            return new FunctionCall("range", new List<Expression> { start, end });
        }

        private Expression ParseListOrRange()
        {
            Consume(TokenType.LBracket); // Consume the opening bracket

            // Check if the list is actually a range
            if (_lexer.PeekNextToken().Type == TokenType.DotDot)
                return ParseRange();

            var elements = new List<Expression>(); // Create a list to store the elements

            // Check if the list is not empty
            if (_currentToken.Type != TokenType.RBracket)
            {
                do
                {
                    if (_currentToken.Type == TokenType.Comma)
                        Consume(TokenType.Comma);

                    elements.Add(ParseExpression());
                }
                while (_currentToken.Type == TokenType.Comma);
            }

            Consume(TokenType.RBracket); // Consume the closing bracket
            return new ListValue(elements);
        }

        private IndexerExpression ParseIndexerExpression(Expression listExpr)
        {
            Consume(TokenType.LBracket);
            var indexExpr = ParseExpression();
            Consume(TokenType.RBracket);
            return new IndexerExpression(listExpr, indexExpr);
        }

        private Expression ParseFactor()
        {
            // Step 1: Handle primary expressions and unary operators
            Expression expr = ParsePrimaryOrUnaryExpression();

            // Step 2: Loop to handle postfix operations (function calls, indexers, postfix operators)
            while (true)
            {
                switch (_currentToken.Type)
                {
                    case TokenType.LParen:
                        // Function call
                        expr = ParseFunctionCall(expr);
                        break;

                    case TokenType.LBracket:
                        // Indexer expression
                        expr = ParseIndexerExpression(expr);
                        break;

                    case TokenType.Increment:
                    case TokenType.Decrement:
                        // Postfix increment or decrement
                        var operatorToken = _currentToken;
                        Consume(operatorToken.Type);
                        expr = new UnaryOperatorExpression(expr, operatorToken.Type, OperatorFixity.Postfix);
                        break;

                    default:
                        // No postfix operation, return the expression
                        return expr;
                }
            }
        }

        private Expression ParsePrimaryOrUnaryExpression()
        {
            var token = _currentToken;
            Expression expr;

            switch (token.Type)
            {
                // Literals
                case TokenType.NumberLiteral:
                case TokenType.CharLiteral:
                case TokenType.StringLiteral:
                case TokenType.BooleanLiteral:
                    expr = ParseLiteral(token);
                    Consume(token.Type);
                    break;

                // Lists or list ranges
                case TokenType.LBracket:
                    expr = ParseListOrRange();
                    break;

                // Unary operators and prefix increment/decrement
                case TokenType.Plus:
                case TokenType.Minus:
                case TokenType.Not:
                case TokenType.Increment:
                case TokenType.Decrement:
                    Consume(token.Type);
                    expr = new UnaryOperatorExpression(ParseFactor(), token.Type, OperatorFixity.Prefix);
                    break;

                // Parenthesized expressions
                case TokenType.LParen:
                    Consume(TokenType.LParen);
                    expr = ParseExpression();
                    Consume(TokenType.RParen);
                    break;

                // Variables or function call placeholders
                case TokenType.Identifier:
                    expr = new Variable((string)token.Value!);
                    Consume(TokenType.Identifier);
                    break;

                default:
                    throw new Exception($"Unexpected token: {token.Type}");
            }

            return expr;
        }

        private static Expression ParseLiteral(Token token)
        {
            return token.Type switch
            {
                TokenType.NumberLiteral => new NumberLiteral((double)token.Value!),
                TokenType.CharLiteral => new CharLiteral((char)token.Value!),
                TokenType.StringLiteral => new StringLiteral((string)token.Value!),
                TokenType.BooleanLiteral => new BooleanLiteral((bool)token.Value!),
                _ => throw new InvalidOperationException("Invalid literal type."),
            };
        }

        private Expression ParseTerm()
        {
            var node = ParseExponent();

            while (_multiplicativeOperators.Contains(_currentToken.Type))
            {
                var token = _currentToken;
                Consume(token.Type);
                node = new BinaryOperatorExpression(node, token.Type, ParseExponent());
            }

            return node;
        }

        private Expression ParseExponent()
        {
            var node = ParseFactor();

            while (_currentToken.Type == TokenType.Caret)
            {
                var token = _currentToken;
                Consume(TokenType.Caret);
                node = new BinaryOperatorExpression(node, token.Type, ParseExponent());
            }

            return node;
        }

        private Expression ParseExpression()
        {
            var node = ParseAssignmentExpression();

            while (_currentToken.Type == TokenType.QuestionMark)
            {
                Consume(TokenType.QuestionMark);
                var trueExpression = ParseExpression();
                Consume(TokenType.Colon);
                var falseExpression = ParseExpression();
                node = new TernaryOperatorExpression(condition: node, trueExpression, falseExpression);
            }

            return node;
        }

        private CompoundStatement ParseCompoundStatement()
        {
            Consume(TokenType.LBrace);
            var nodes = ParseStatementList();
            Consume(TokenType.RBrace);

            var root = new CompoundStatement();
            foreach (var node in nodes)
                root.Statements.Add(node);

            return root;
        }

        private List<Statement> ParseStatementList()
        {
            var results = new List<Statement>();

            while (_currentToken.Type != TokenType.RBrace && _currentToken.Type != TokenType.EOF)
            {
                var node = ParseStatement();
                results.Add(node);
            }

            return results;
        }

        private IfStatement ParseIfStatement()
        {
            Consume(TokenType.If);
            Consume(TokenType.LParen);
            var condition = ParseExpression();
            Consume(TokenType.RParen);

            // Parse thenStatement as either a compound statement or a single statement
            var thenStatement = (_currentToken.Type == TokenType.LBrace) ? ParseCompoundStatement() : ParseStatement();

            var elseIfStatements = new List<(Expression Condition, Statement Statement)>();
            Statement? elseStatement = null;

            while (_currentToken.Type == TokenType.Elif)
            {
                Consume(TokenType.Elif);
                Consume(TokenType.LParen);
                var elseIfCondition = ParseExpression();
                Consume(TokenType.RParen);
                // Parse elseIfStatement similarly
                var elseIfStatement = (_currentToken.Type == TokenType.LBrace) ? ParseCompoundStatement() : ParseStatement();
                elseIfStatements.Add((elseIfCondition, elseIfStatement));
            }

            if (_currentToken.Type == TokenType.Else)
            {
                Consume(TokenType.Else);
                // Parse elseStatement similarly
                elseStatement = (_currentToken.Type == TokenType.LBrace) ? ParseCompoundStatement() : ParseStatement();
            }

            return new IfStatement(condition, thenStatement, elseIfStatements, elseStatement);
        }

        private ForStatement ParseForStatement()
        {
            Consume(TokenType.For);
            Consume(TokenType.LParen);

            Expression? initializer = null;
            if (_currentToken.Type != TokenType.Semicolon)
            {
                initializer = ParseExpression();
            }
            Consume(TokenType.Semicolon);

            Expression? condition = null;
            if (_currentToken.Type != TokenType.Semicolon)
            {
                condition = ParseExpression();
            }
            Consume(TokenType.Semicolon);

            Expression? increment = null;
            if (_currentToken.Type != TokenType.RParen)
            {
                increment = ParseExpression();
            }
            Consume(TokenType.RParen);

            var body = (_currentToken.Type == TokenType.LBrace) ? ParseCompoundStatement() : ParseStatement();

            return new ForStatement(initializer, condition, increment, body);
        }

        private WhileStatement ParseWhileStatement()
        {
            Consume(TokenType.While);
            Consume(TokenType.LParen);
            var condition = ParseExpression();
            Consume(TokenType.RParen);
            var body = (_currentToken.Type == TokenType.LBrace) ? ParseCompoundStatement() : ParseStatement();
            return new WhileStatement(condition, body);
        }

        private DoWhileStatement ParseDoWhileStatement()
        {             
            Consume(TokenType.Do);
            var body = (_currentToken.Type == TokenType.LBrace) ? ParseCompoundStatement() : ParseStatement();
            Consume(TokenType.While);
            Consume(TokenType.LParen);
            var condition = ParseExpression();
            Consume(TokenType.RParen);
            Consume(TokenType.Semicolon);
            return new DoWhileStatement(condition, body);
        }

        private BreakStatement ParseBreakStatement()
        {
            Consume(TokenType.Break);
            Consume(TokenType.Semicolon);
            return new BreakStatement();
        }

        private ContinueStatement ParseContinueStatement()
        {
            Consume(TokenType.Continue);
            Consume(TokenType.Semicolon);
            return new ContinueStatement();
        }

        private ReturnStatement ParseReturnStatement()
        {
            Consume(TokenType.Return);
            Expression? returnValue = null;
            if (_currentToken.Type != TokenType.Semicolon)
            {
                returnValue = ParseExpression();
            }
            Consume(TokenType.Semicolon);
            return new ReturnStatement(returnValue);
        }

        private ForEachStatement ParseForEachStatement()
        {
            Consume(TokenType.ForEach);
            Consume(TokenType.LParen);
            var variableName = (string)_currentToken.Value!;
            Consume(TokenType.Identifier);
            Consume(TokenType.In);
            var listExpression = ParseExpression();
            Consume(TokenType.RParen);
            var body = (_currentToken.Type == TokenType.LBrace) ? ParseCompoundStatement() : ParseStatement();
            return new ForEachStatement(variableName, listExpression, body);
        }

        private Statement ParseStatement()
        {
            return _currentToken.Type switch
            {
                TokenType.LBrace => ParseCompoundStatement(),
                TokenType.If => ParseIfStatement(),
                TokenType.For => ParseForStatement(),
                TokenType.ForEach => ParseForEachStatement(),
                TokenType.While => ParseWhileStatement(),
                TokenType.Do => ParseDoWhileStatement(),
                TokenType.Break => ParseBreakStatement(),
                TokenType.Continue => ParseContinueStatement(),
                TokenType.Return => ParseReturnStatement(),
                TokenType.Semicolon => ParseEmptyStatement(),
                _ => ParseExpressionStatement()
            };
        }

        private ExpressionStatement ParseExpressionStatement()
        {
            // Parse an expression generally
            var expression = ParseExpression();
            Consume(TokenType.Semicolon); // Ensure it ends with a semicolon
            return new ExpressionStatement(expression);
        }

        private Expression ParseAssignmentExpression()
        {
            Expression lhs = ParseLogicalOrExpression();

            if (_assignmentOperators.Contains(_currentToken.Type))
            {
                TokenType operatorType = _currentToken.Type;
                Consume(operatorType); // Move past the operator

                Expression rhs = ParseAssignmentExpression(); // Recursively parse the RHS
                return new AssignmentExpression(lhs, rhs, operatorType);
            }

            return lhs; // If there's no assignment operator, just return the LHS
        }

        private EmptyStatement ParseEmptyStatement()
        {
            Consume(TokenType.Semicolon);
            return new EmptyStatement();
        }

        private FunctionCall ParseFunctionCall(Expression functionExpr)
        {
            // Consume the opening parenthesis
            Consume(TokenType.LParen);

            // Parse the arguments
            var arguments = new List<Expression>();
            if (_currentToken.Type != TokenType.RParen) // Check if the next token is not a right parenthesis
            {
                do
                {
                    if (_currentToken.Type == TokenType.Comma)
                        Consume(TokenType.Comma); // Consume the comma before parsing the next argument

                    arguments.Add(ParseExpression());
                }
                while (_currentToken.Type == TokenType.Comma); // Continue if there's a comma (more arguments)
            }

            // Consume the closing parenthesis
            Consume(TokenType.RParen);

            // Return a new function call node
            return new FunctionCall((functionExpr as Variable)!.Name, arguments);
        }

        private FunctionDefinition ParseFunctionDefinition()
        {
            // "function" token is already consumed
            string functionName = (string)_currentToken.Value!;
            Consume(TokenType.Identifier); // Function name

            // Check for duplicate function definition
            if (!_definedFunctions.Add(functionName)) // Try to add the function name to the set
                throw new Exception($"Function '{functionName}' is already defined.");

            Consume(TokenType.LParen); // Opening parenthesis
            var parameters = ParseParameters();
            Consume(TokenType.RParen); // Closing parenthesis

            var body = ParseCompoundStatement(); // Function body

            return new FunctionDefinition(functionName, parameters, body);
        }

        private List<string> ParseParameters()
        {
            var parameters = new List<string>();

            if (_currentToken.Type != TokenType.RParen) // Check if parameter list is empty
            {
                do
                {
                    if (_currentToken.Type == TokenType.Identifier)
                    {
                        string paramName = (string)_currentToken.Value!;
                        parameters.Add(paramName);
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

        private List<string> ParseGlobalVariableDeclarations()
        {
            var globals = new List<string>();

            while (_currentToken.Type == TokenType.Global)
            {
                Consume(TokenType.Global);
                var variableName = (string)_currentToken.Value!;
                Consume(TokenType.Identifier);
                globals.Add(variableName);

                // Check for multiple global variable declarations on the same line
                while (_currentToken.Type == TokenType.Comma)
                {
                    Consume(TokenType.Comma);
                    variableName = (string)_currentToken.Value!;
                    Consume(TokenType.Identifier);
                    globals.Add(variableName);
                }

                Consume(TokenType.Semicolon);
            }

            return globals;
        }

        private Program ParseProgram()
        {
            // Parse global variable declarations at the beginning of the program
            var globals = ParseGlobalVariableDeclarations();

            var functions = new List<FunctionDefinition>();

            while (_currentToken.Type != TokenType.EOF)
            {
                if (_currentToken.Type == TokenType.Func)
                {
                    Consume(TokenType.Func);
                    functions.Add(ParseFunctionDefinition());
                }
                else
                    throw new Exception("Unexpected token: " + _currentToken.Type);
            }

            return new Program(globals, functions);
        }

        private Expression ParseComparisonExpression()
        {
            var node = ParseArithmeticExpression();

            while (_comparisonOperators.Contains(_currentToken.Type))
            {
                var token = _currentToken;
                Consume(token.Type); // Consume the comparison operator
                node = new BinaryOperatorExpression(node, token.Type, ParseArithmeticExpression());
            }

            return node;
        }

        private Expression ParseLogicalOrExpression()
        {
            var node = ParseLogicalAndExpression();

            while (_currentToken.Type == TokenType.Or)
            {
                var token = _currentToken;
                Consume(token.Type); // Consume the Or operator
                node = new BinaryOperatorExpression(node, token.Type, ParseLogicalAndExpression());
            }

            return node;
        }

        private Expression ParseLogicalAndExpression()
        {
            var node = ParseComparisonExpression();

            while (_currentToken.Type == TokenType.And)
            {
                var token = _currentToken;
                Consume(token.Type); // Consume the And operator
                node = new BinaryOperatorExpression(node, token.Type, ParseComparisonExpression());
            }

            return node;
        }

        private Expression ParseArithmeticExpression()
        {
            var node = ParseTerm();

            while (_currentToken.Type == TokenType.Plus || _currentToken.Type == TokenType.Minus)
            {
                var token = _currentToken;
                Consume(token.Type); // Consume the Plus or Minus operator

                node = new BinaryOperatorExpression(node, token.Type, ParseTerm());
            }

            return node;
        }

        public Expression Parse()
        {
            var node = ParseProgram();

            if (_currentToken.Type != TokenType.EOF)
                throw new Exception($"Unexpected token: {_currentToken.Type}");

            return node;
        }
    }
}