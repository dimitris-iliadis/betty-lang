namespace BettyLang.Core.AST
{
    public class NumberNode : Node
    {
        public Token Token { get; }
        public double Value { get; }

        public NumberNode(Token token)
        {
            Token = token;
            Value = double.Parse(token.Value);
        }

        public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
    }
}