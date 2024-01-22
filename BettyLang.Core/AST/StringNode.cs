namespace BettyLang.Core.AST
{
    public class StringNode : ASTNode
    {
        public string Value { get; }

        public StringNode(string value) { Value = value; }

        public override InterpreterResult Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}