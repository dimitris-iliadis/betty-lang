namespace BettyLang.Core.AST
{
    public class AssignmentNode : Node
    {
        public Node Left { get; }
        public Token Operator { get; }
        public Node Right { get; }

        public AssignmentNode(Node left, Token op, Node right)
        {
            Left = left;
            Operator = op;
            Right = right;
        }

        public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
    }
}