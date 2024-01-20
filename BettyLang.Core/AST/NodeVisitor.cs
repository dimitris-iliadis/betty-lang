namespace BettyLang.Core.AST
{
    public abstract class NodeVisitor<T>
    {
        public abstract T Visit(BinaryOperatorNode node);
        public abstract T Visit(NumberNode node);
        public abstract T Visit(StringNode node);
        public abstract T Visit(UnaryOperatorNode node);
    }
}