namespace BettyLang.Core.AST
{
    public class ProgramNode : Node
    {
        public List<FunctionDefinitionNode> Functions { get; }
        public CompoundStatementNode MainBlock { get; }

        public ProgramNode(List<FunctionDefinitionNode> functions, CompoundStatementNode mainBlock)
        {
            Functions = functions;
            MainBlock = mainBlock;
        }

        public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
    }
}