namespace BettyLang.Core.AST
{
    public class IfStatementNode : ASTNode
    {
        public ASTNode Condition { get; }
        public CompoundStatementNode Body { get; }

        public IfStatementNode(ASTNode condition, CompoundStatementNode body)
        {
            Condition = condition;
            Body = body;
        }

        public override InterpreterResult Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}