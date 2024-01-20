using BettyLang.Core.AST;

namespace BettyLang.Core
{
    public class Interpreter : NodeVisitor<InterpreterResult>
    {
        private readonly Parser _parser;

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

        public InterpreterResult Interpret()
        {
            var tree = _parser.Parse();
            return tree.Accept(this);
        }
    }
}