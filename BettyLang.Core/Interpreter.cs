using BettyLang.Core.AST;

namespace BettyLang.Core
{
    public class Interpreter : NodeVisitor<InterpreterResult>
    {
        private readonly Parser _parser;

        private Dictionary<string, object> _globalScope = new();

        public Interpreter(Parser parser) { _parser = parser; }

        public override InterpreterResult Visit(BinaryOperatorNode node)
        {
            var leftResult = node.Left.Accept(this);
            var rightResult = node.Right.Accept(this);

            if (node.Operator.Type == TokenType.Plus &&
                (leftResult.Value is string || rightResult.Value is string))
            {
                string leftString = leftResult.AsString();
                string rightString = rightResult.AsString();
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
                    default:
                        throw new NotImplementedException($"Operation {node.Operator.Type} is not supported.");
                }
            }

            throw new InvalidOperationException("Unsupported types or operators");
        }

        public override InterpreterResult Visit(NumberNode node) => new InterpreterResult(node.Value);

        public override InterpreterResult Visit(StringNode node) => new InterpreterResult(node.Value);

        public override InterpreterResult Visit(ModuleNode node)
        {
            node.Block.Accept(this);

            return new InterpreterResult(null);
        }

        public override InterpreterResult Visit(CompoundStatementNode node)
        {
            foreach (var child in node.Children)
                child.Accept(this);

            return new InterpreterResult(null);
        }

        public override InterpreterResult Visit(AssignmentNode node)
        {
            if (node.Left is VariableNode variableNode)
            {
                string variableName = variableNode.Value;
                var rightResult = node.Right.Accept(this);
                _globalScope[variableName] = rightResult.Value;
                return new InterpreterResult(null);
            }
            else
                throw new Exception("The left-hand side of an assignment must be a variable.");
        }

        public override InterpreterResult Visit(VariableNode node)
        {
            string variableName = node.Value;

            if (_globalScope.TryGetValue(variableName, out var value))
                return new InterpreterResult(value);
            else
                throw new Exception($"Variable '{variableName}' is not defined.");
        }

        public override InterpreterResult Visit(EmptyStatementNode node) => new InterpreterResult(null);

        public override InterpreterResult Visit(UnaryOperatorNode node)
        {
            TokenType op = node.Operator.Type;
            var operandResult = node.Expression.Accept(this);

            if (op == TokenType.Plus)
                return new InterpreterResult(+operandResult.AsNumber());
            else if (op == TokenType.Minus)
                return new InterpreterResult(-operandResult.AsNumber());

            throw new InvalidOperationException($"Unsupported unary operator {op}");
        }

        public void Interpret()
        {
            var tree = _parser.Parse();
            tree.Accept(this);
        }
    }
}