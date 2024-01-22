namespace BettyLang.Core.AST
{
    public class FunctionDefinitionNode : ASTNode
    {
        public string FunctionName { get; }
        public List<ParameterNode> Parameters { get; }
        public CompoundStatementNode Body { get; }

        public FunctionDefinitionNode(string functionName, List<ParameterNode> parameters, CompoundStatementNode body)
        {
            FunctionName = functionName;
            Parameters = parameters;
            Body = body;
        }

        public override InterpreterResult Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}