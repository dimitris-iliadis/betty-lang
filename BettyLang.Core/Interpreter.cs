using BettyLang.Core.AST;

namespace BettyLang.Core
{
    public class BreakException : Exception { }
    public class ContinueException : Exception { }
    public class ReturnException : Exception
    {
        public InterpreterResult ReturnValue { get; }

        public ReturnException(InterpreterResult returnValue)
        {
            ReturnValue = returnValue;
        }
    }

    public class Interpreter : INodeVisitor
    {
        private readonly Parser _parser;

        private readonly Dictionary<string, FunctionDefinitionNode> _functions = new();
        private readonly Stack<Dictionary<string, object>> _scopes = new();

        private static readonly Dictionary<string, Func<double, double>> _mathFunctions = new Dictionary<string, Func<double, double>>
        {
            {"sin", Math.Sin},
            {"cos", Math.Cos},
            {"tan", Math.Tan},
            {"sqrt", Math.Sqrt}
        };

        private static readonly HashSet<string> _builtInFunctionNames = new HashSet<string>
        {
            "print",
            "println",
            "input",

            "sin",
            "cos",
            "tan",
            "sqrt"
        };

        private bool _isInLoop = false;

        public Interpreter(Parser parser)
        {
            _parser = parser;
        }

        public InterpreterResult Visit(ProgramNode node)
        {
            // Visit each function definition and store it in a dictionary
            foreach (var function in node.Functions)
                function.Accept(this);

            if (_functions.TryGetValue("main", out var mainFunction))
            {
                if (mainFunction.Parameters.Count != 0)
                    throw new Exception("main() function cannot have parameters.");

                // Execute the main function
                return Visit(new FunctionCallNode("main", []));
            }
            else
            {
                throw new Exception("No main function found.");
            }
        }

        public InterpreterResult Visit(TernaryOperatorNode node)
        {
            var conditionResult = node.Condition.Accept(this).AsBoolean();
            return conditionResult ? node.TrueExpression.Accept(this) : node.FalseExpression.Accept(this);
        }

        public InterpreterResult Visit(ReturnStatementNode node)
        {
            InterpreterResult returnValue = null;
            if (node.ReturnValue != null)
            {
                returnValue = node.ReturnValue.Accept(this);
            }

            // Use the exception to carry the return value
            throw new ReturnException(returnValue);
        }

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

        public InterpreterResult Visit(BreakStatementNode node)
        {
            if (!_isInLoop)
            {
                throw new Exception("Break statement not inside a loop");
            }

            throw new BreakException();
        }

        public InterpreterResult Visit(ContinueStatementNode node)
        {
            if (!_isInLoop)
            {
                throw new Exception("Continue statement not inside a loop");
            }

            throw new ContinueException();
        }

        public InterpreterResult Visit(WhileStatementNode node)
        {
            _isInLoop = true;

            try
            {
                while (node.Condition.Accept(this).AsBoolean())
                {
                    try
                    {
                        node.Body.Accept(this);
                    }
                    catch (ContinueException)
                    {
                        continue;
                    }
                }
            }
            catch (BreakException) { }
            finally { _isInLoop = false; }

            return new InterpreterResult(null);
        }

        public InterpreterResult Visit(IfStatementNode node)
        {
            var conditionResult = node.Condition.Accept(this).AsBoolean();

            if (conditionResult)
            {
                return node.ThenStatement.Accept(this);
            }
            else
            {
                foreach (var (Condition, Statement) in node.ElseIfStatements)
                {
                    if (Condition.Accept(this).AsBoolean())
                    {
                        return Statement.Accept(this);
                    }
                }

                if (node.ElseStatement != null)
                {
                    return node.ElseStatement.Accept(this);
                }
            }

            return new InterpreterResult(null);
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
                    case TokenType.Mod:
                        return new InterpreterResult(leftOperand % rightOperand);
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
                AssignVariable(variableName, rightResult.Value!);
                return new InterpreterResult(null);
            }
            else
                throw new Exception("The left-hand side of an assignment must be a variable.");
        }

        public InterpreterResult Visit(VariableNode node)
        {
            string variableName = node.Value;
            return new InterpreterResult(LookupVariable(variableName));
        }

        public InterpreterResult Visit(EmptyStatementNode node) => new InterpreterResult(null);

        private InterpreterResult HandleBuiltInFunction(FunctionCallNode node)
        {
            if (node.FunctionName == "print" || node.FunctionName == "println")
                return HandlePrintFunction(node);
            else if (node.FunctionName == "input")
                return HandleInputFunction(node);
            else if (_mathFunctions.TryGetValue(node.FunctionName, out var func))
                return HandleMathFunction(node, func);
            else
                throw new Exception($"Unknown built-in function {node.FunctionName}");
        }

        private InterpreterResult HandleInputFunction(FunctionCallNode node)
        {
            // Check for correct number of arguments
            if (node.Arguments.Count > 1)
                throw new Exception("Input function requires at most one argument, which can be a prompt string.");

            // If an argument is provided, display it as a prompt
            if (node.Arguments.Count == 1)
                Console.Write(node.Arguments[0].Accept(this).Value);

            // Read input from the user
            string userInput = Console.ReadLine() ?? string.Empty;

            // Try to parse the input as a double. If it fails, treat it as a string
            object value = double.TryParse(userInput, out double numericValue) ? numericValue : userInput;

            return new InterpreterResult(value);
        }

        private InterpreterResult HandlePrintFunction(FunctionCallNode node)
        {
            // Ensure that only one argument is provided
            if (node.Arguments.Count != 1)
                throw new Exception($"{node.FunctionName} function requires exactly one argument.");

            // Evaluate the argument
            var argResult = node.Arguments[0].Accept(this).Value;

            if (node.FunctionName == "println")
                Console.WriteLine(argResult);
            else
                Console.Write(argResult);

            return new InterpreterResult(null);
        }


        private InterpreterResult HandleMathFunction(FunctionCallNode node, Func<double, double> func)
        {
            if (node.Arguments.Count != 1)
                throw new Exception($"{node.FunctionName}() takes exactly one argument");

            if (!(node.Arguments[0].Accept(this).Value is double))
                throw new Exception($"{node.FunctionName}() must be called with a numeric argument");

            var argValue = node.Arguments[0].Accept(this).AsDouble();
            return new InterpreterResult(func(argValue));
        }

        private bool IsBuiltInFunction(string functionName) => _builtInFunctionNames.Contains(functionName);

        public InterpreterResult Visit(FunctionCallNode node)
        {
            if (IsBuiltInFunction(node.FunctionName))
                return HandleBuiltInFunction(node);

            if (!_functions.TryGetValue(node.FunctionName, out var function))
                throw new Exception($"Function {node.FunctionName} is not defined.");

            // Enter a new scope for function execution
            EnterScope();

            InterpreterResult returnValue = null;

            // Store the current loop context
            bool wasInLoop = _isInLoop;
            _isInLoop = false; // Reset isInLoop because we're entering a new function context

            try
            {
                // Set function parameters in the new scope
                for (int i = 0; i < node.Arguments.Count; i++)
                {
                    var argValue = node.Arguments[i].Accept(this).Value;
                    _scopes.Peek()[function.Parameters[i]] = argValue;
                }

                // Execute function body
                function.Body.Accept(this);
            }
            catch (ReturnException ex)
            {
                // Catch the return value from the function
                returnValue = ex.ReturnValue;
            }
            finally
            {
                // Restore the loop context after function execution
                _isInLoop = wasInLoop;

                // Exit the function scope regardless of how we leave the function
                ExitScope();
            }

            return returnValue ?? new InterpreterResult(null); // return null or default if no value was returned
        }

        public InterpreterResult Visit(FunctionDefinitionNode node)
        {
            if (IsBuiltInFunction(node.FunctionName))
                throw new Exception($"Function name '{node.FunctionName}' is reserved for built-in functions.");

            _functions[node.FunctionName] = node;
            return new InterpreterResult(null);
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

        public InterpreterResult Interpret()
        {
            var tree = _parser.Parse();
            return tree.Accept(this);
        }

        private void EnterScope()
        {
            _scopes.Push(new Dictionary<string, object>());
        }

        private void ExitScope()
        {
            _scopes.Pop();
        }

        private void AssignVariable(string name, object value)
        {
            var currentScope = _scopes.Peek();
            currentScope[name] = value;
        }

        private object LookupVariable(string name)
        {
            foreach (var scope in _scopes)
            {
                if (scope.TryGetValue(name, out var value))
                {
                    return value;
                }
            }
            throw new Exception($"Variable '{name}' not found in any scope");
        }
    }
}