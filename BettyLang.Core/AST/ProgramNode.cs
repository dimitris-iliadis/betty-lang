namespace BettyLang.Core.AST
{
    public class ProgramNode : ASTNode
    {
        public List<FunctionDefinitionNode> Functions { get; }
        public CompoundStatementNode MainBlock { get; }

        public ProgramNode(List<FunctionDefinitionNode> functions, CompoundStatementNode mainBlock)
        {
            Functions = functions;
            MainBlock = mainBlock;
        }

        public override InterpreterResult Accept(INodeVisitor visitor) => visitor.Visit(this);
    }
}