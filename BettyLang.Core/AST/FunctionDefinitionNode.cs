namespace BettyLang.Core.AST
{
    public class FunctionDefinitionNode : Node
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

        public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
    }
}