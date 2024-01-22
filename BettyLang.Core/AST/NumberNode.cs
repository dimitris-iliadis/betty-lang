namespace BettyLang.Core.AST
{
    public class NumberNode : ASTNode
    {
        public Token Token { get; }
        public double Value { get; }

        public NumberNode(Token token)
        {
            Token = token;
            Value = double.Parse(token.Value);
        }

        public override InterpreterResult Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}