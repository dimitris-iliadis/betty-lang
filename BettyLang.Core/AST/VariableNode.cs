namespace BettyLang.Core.AST
{
    public class VariableNode : Node
    {
        public Token Token { get; }
        public string Value { get; }

        public VariableNode(Token token)
        {
            Token = token;
            Value = token.Value;
        }

        public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
    }
}