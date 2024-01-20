namespace BettyLang.Core.AST
{
    public abstract class Node
    {
        public abstract T Accept<T>(NodeVisitor<T> visitor);
    }
}