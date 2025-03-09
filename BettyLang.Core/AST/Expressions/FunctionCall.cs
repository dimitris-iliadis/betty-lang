using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class FunctionCall(List<Expression> arguments, 
        Expression? expression = null,
        string? functionName = null) : Expression
    {
        public List<Expression> Arguments { get; } = arguments;
        public Expression? Expression = expression;
        public string? FunctionName = functionName;

        public override Value Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}