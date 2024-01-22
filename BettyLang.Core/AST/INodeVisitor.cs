namespace BettyLang.Core.AST
{
    public interface INodeVisitor
    {
        InterpreterResult Visit(BinaryOperatorNode node);
        InterpreterResult Visit(NumberLiteralNode node);
        InterpreterResult Visit(StringLiteralNode node);
        InterpreterResult Visit(UnaryOperatorNode node);
        InterpreterResult Visit(CompoundStatementNode node);
        InterpreterResult Visit(AssignmentNode node);
        InterpreterResult Visit(VariableNode node);
        InterpreterResult Visit(EmptyStatementNode node);
        InterpreterResult Visit(ProgramNode node);
        InterpreterResult Visit(FunctionDefinitionNode node);
        InterpreterResult Visit(ParameterNode node);
        InterpreterResult Visit(BooleanLiteralNode node);
    }
}