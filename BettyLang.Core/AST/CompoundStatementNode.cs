namespace BettyLang.Core.AST
{
    public class CompoundStatementNode : Node
    {
        public List<Node> Children { get; set; }

        public CompoundStatementNode() { Children = []; }

        public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
    }
}