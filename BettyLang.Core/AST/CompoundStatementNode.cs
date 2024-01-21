namespace BettyLang.Core.AST
{
    public class CompoundStatementNode : Node
    {
        public List<Node> Statements { get; set; }

        public CompoundStatementNode() { Statements = []; }

        public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
    }
}