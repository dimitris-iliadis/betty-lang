namespace BettyLang.Core.AST
{
    public class StringNode : Node
    {
        public string Value { get; }

        public StringNode(string value) { Value = value; }

        public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
    }
}