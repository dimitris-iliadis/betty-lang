namespace BettyLang.Core.AST
{
    public class UnaryOperatorNode : Node
    {
        public Token Operator { get; }
        public Node Expression { get; }

        public UnaryOperatorNode(Token op, Node expression)
        {
            Operator = op;
            Expression = expression;
        }

        public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
    }
}