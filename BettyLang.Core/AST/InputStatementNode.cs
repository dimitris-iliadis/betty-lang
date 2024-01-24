namespace BettyLang.Core.AST
{
    public class InputStatementNode : ASTNode
    {
        public string VariableName { get; }

        public InputStatementNode(string variableName)
        {
            VariableName = variableName;
        }

        public override InterpreterResult Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}