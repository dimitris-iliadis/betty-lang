using BettyLang.Core.AST;

namespace BettyLang.Core
{
    public class Interpreter : INodeVisitor
    {
        private readonly Parser _parser;

        private Dictionary<string, object> _globalScope = [];

        public Interpreter(Parser parser) { _parser = parser; }

        private InterpreterResult PerformComparison(object leftValue, object rightValue, TokenType operatorType)
        {
            if (leftValue is double leftDouble && rightValue is double rightDouble)
            {
                return operatorType switch
                {
                    TokenType.Equal => new InterpreterResult(leftDouble == rightDouble),
                    TokenType.NotEqual => new InterpreterResult(leftDouble != rightDouble),
                    TokenType.LessThan => new InterpreterResult(leftDouble < rightDouble),
                    TokenType.LessThanOrEqual => new InterpreterResult(leftDouble <= rightDouble),
                    TokenType.GreaterThan => new InterpreterResult(leftDouble > rightDouble),
                    TokenType.GreaterThanOrEqual => new InterpreterResult(leftDouble >= rightDouble),
                    _ => throw new Exception($"Unsupported operator for number comparison: {operatorType}")
                };
            }
            else if (leftValue is string leftString && rightValue is string rightString)
            {
                return operatorType switch
                {
                    TokenType.Equal => new InterpreterResult(leftString == rightString),
                    TokenType.NotEqual => new InterpreterResult(leftString != rightString),
                    _ => throw new Exception($"Unsupported operator for string comparison: {operatorType}")
                };
            }
            else if (leftValue is bool leftBool && rightValue is bool rightBool)
            {
                return operatorType switch
                {
                    TokenType.Equal => new InterpreterResult(leftBool == rightBool),
                    TokenType.NotEqual => new InterpreterResult(leftBool != rightBool),
                    _ => throw new Exception($"Unsupported operator for boolean comparison: {operatorType}")
                };
            }
            else
                throw new Exception("Type mismatch or unsupported types for comparison.");
        }

        public InterpreterResult Visit(BinaryOperatorNode node)
        {
            var leftResult = node.Left.Accept(this);
            var rightResult = node.Right.Accept(this);

            if (leftResult.Value is bool leftBool && rightResult.Value is bool rightBool)
            {
                switch (node.Operator.Type)
                {
                    case TokenType.And:
                        return new InterpreterResult(leftBool && rightBool);
                    case TokenType.Or:
                        return new InterpreterResult(leftBool || rightBool);
                }
            }

            if (node.Operator.Type == TokenType.Plus &&
                (leftResult.Value is string || rightResult.Value is string))
            {
                string leftString = leftResult.Value!.ToString()!;
                string rightString = rightResult.Value!.ToString()!;
                return new InterpreterResult(leftString + rightString);
            }

            if (leftResult.Value is double leftOperand && rightResult.Value is double rightOperand)
            {
                switch (node.Operator.Type)
                {
                    case TokenType.Plus:
                        return new InterpreterResult(leftOperand + rightOperand);
                    case TokenType.Minus:
                        return new InterpreterResult(leftOperand - rightOperand);
                    case TokenType.Mul:
                        return new InterpreterResult(leftOperand * rightOperand);
                    case TokenType.Div:
                        return new InterpreterResult(leftOperand / rightOperand);
                    case TokenType.Caret:
                        return new InterpreterResult(Math.Pow(leftOperand, rightOperand));
                }
            }

            switch (node.Operator.Type)
            {
                case TokenType.Equal:
                case TokenType.NotEqual:
                case TokenType.LessThan:
                case TokenType.LessThanOrEqual:
                case TokenType.GreaterThan:
                case TokenType.GreaterThanOrEqual:
                    return PerformComparison(leftResult.Value!, rightResult.Value!, node.Operator.Type);
            }

            throw new Exception($"Unsupported binary operator: {node.Operator.Type}");
        }

        public InterpreterResult Visit(BooleanLiteralNode node) => new InterpreterResult(node.Value);

        public InterpreterResult Visit(NumberLiteralNode node) => new InterpreterResult(node.Value);

        public InterpreterResult Visit(StringLiteralNode node) => new InterpreterResult(node.Value);

        public InterpreterResult Visit(ProgramNode node)
        {
            // Visit each function definition and store them in a function table or similar structure
            foreach (var function in node.Functions)
            {
                function.Accept(this);
            }

            // Execute the main block
            node.MainBlock.Accept(this);

            return new InterpreterResult(null);
        }

        public InterpreterResult Visit(CompoundStatementNode node)
        {
            foreach (var statement in node.Statements)
                statement.Accept(this);

            return new InterpreterResult(null);
        }

        public InterpreterResult Visit(AssignmentNode node)
        {
            if (node.Left is VariableNode variableNode)
            {
                string variableName = variableNode.Value;
                var rightResult = node.Right.Accept(this);
                _globalScope[variableName] = rightResult.Value!;
                return new InterpreterResult(null);
            }
            else
                throw new Exception("The left-hand side of an assignment must be a variable.");
        }

        public InterpreterResult Visit(VariableNode node)
        {
            string variableName = node.Value;

            if (_globalScope.TryGetValue(variableName, out var value))
                return new InterpreterResult(value);
            else
                throw new Exception($"Variable '{variableName}' is not defined.");
        }

        public InterpreterResult Visit(PrintStatementNode node)
        {
            // Evaluate the expression contained in the print statement
            var expressionResult = node.Expression.Accept(this);

            // Perform the print action - e.g., writing to the console
            Console.Write(expressionResult.Value);

            // Return a null or empty result, since print doesn't typically have a return value
            return new InterpreterResult(null);
        }

        public InterpreterResult Visit(EmptyStatementNode node) => new InterpreterResult(null);
        public InterpreterResult Visit(FunctionDefinitionNode node)
        {
            throw new NotImplementedException();
        }

        public InterpreterResult Visit(ParameterNode node)
        {
            throw new NotImplementedException();
        }

        public InterpreterResult Visit(UnaryOperatorNode node)
        {
            TokenType op = node.Operator.Type;
            var operandResult = node.Expression.Accept(this);

            switch (op)
            {
                case TokenType.Plus:
                    return new InterpreterResult(+(double)operandResult.Value!);
                case TokenType.Minus:
                    return new InterpreterResult(-(double)operandResult.Value!);
                case TokenType.Not:
                    return new InterpreterResult(!(bool)operandResult.Value!);
            }

            throw new InvalidOperationException($"Unsupported unary operator {op}");
        }

        public void Interpret()
        {
            var tree = _parser.Parse();
            tree.Accept(this);
        }
    }
}