namespace BettyLang.Core.AST
{
    public class PrintStatementNode : ASTNode
    {
        public ASTNode Expression { get; private set; }

        public PrintStatementNode(ASTNode expression) { Expression = expression; }

        public override InterpreterResult Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}