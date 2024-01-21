namespace BettyLang.Core.AST
{
    public abstract class NodeVisitor<T>
    {
        public abstract T Visit(BinaryOperatorNode node);
        public abstract T Visit(NumberNode node);
        public abstract T Visit(StringNode node);
        public abstract T Visit(UnaryOperatorNode node);
        public abstract T Visit(CompoundStatementNode node);
        public abstract T Visit(AssignmentNode node);
        public abstract T Visit(VariableNode node);
        public abstract T Visit(EmptyStatementNode node);
        public abstract T Visit(ProgramNode node);
        public abstract T Visit(FunctionDefinitionNode node);
        public abstract T Visit(ParameterNode node);
    }
}