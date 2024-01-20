namespace BettyLang.Core.AST
{
    public class BinaryOperatorNode : Node
    {
        public Node Left { get; }
        public Token Operator { get; }
        public Node Right { get; }

        public BinaryOperatorNode(Node left, Token op, Node right)
        {
            Left = left;
            Operator = op;
            Right = right;
        }

        public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
    }
}