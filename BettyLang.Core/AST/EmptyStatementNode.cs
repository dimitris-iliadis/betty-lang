namespace BettyLang.Core.AST
{
    public class EmptyStatementNode : ASTNode
    {
        public EmptyStatementNode() { }

        public override InterpreterResult Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}