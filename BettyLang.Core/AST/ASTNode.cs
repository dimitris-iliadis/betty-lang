namespace BettyLang.Core.AST
{
    public abstract class ASTNode
    {
        public abstract InterpreterResult Accept(INodeVisitor visitor);
    }
}