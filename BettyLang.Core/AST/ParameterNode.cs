namespace BettyLang.Core.AST
{
    public class ParameterNode : ASTNode
    {
        public string Name { get; }

        public ParameterNode(string name)
        { 
            Name = name;
        }

        public override InterpreterResult Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}