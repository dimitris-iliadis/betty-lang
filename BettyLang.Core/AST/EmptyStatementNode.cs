namespace BettyLang.Core.AST
{
    public class EmptyStatementNode : Node
    {
        public EmptyStatementNode() { }

        public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
    }
}