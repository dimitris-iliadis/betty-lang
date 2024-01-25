namespace BettyLang.Core.AST
{
    public class ContinueStatementNode : ASTNode
    {
        public ContinueStatementNode() { }

        public override InterpreterResult Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}