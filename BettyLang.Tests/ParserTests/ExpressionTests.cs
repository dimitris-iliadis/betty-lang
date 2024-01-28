using BettyLang.Core.AST;

namespace BettyLang.Tests.ParserTests
{
    public class ExpressionTests : ParserTest
    {
        [Fact]
        public void AssignmentWithArithmeticExpression_ParsesCorrectly()
        {
            var code = "main { result = 3 + 4; }";
            var parser = SetupParser(code);

            var program = parser.Parse() as ProgramNode;
            Assert.NotNull(program);

            var mainBlock = program.MainBlock as CompoundStatementNode;
            Assert.NotNull(mainBlock);
            Assert.Single(mainBlock.Statements);

            var assignment = mainBlock.Statements[0] as AssignmentNode;
            Assert.NotNull(assignment);

            var expression = assignment.Right as BinaryOperatorNode;
            Assert.NotNull(expression);
            Assert.Equal(TokenType.Plus, expression.Operator.Type);

            // Validate the left and right operands
            Assert.IsType<NumberLiteralNode>(expression.Left);
            Assert.IsType<NumberLiteralNode>(expression.Right);
            Assert.Equal(3.0, (expression.Left as NumberLiteralNode)!.Value);
            Assert.Equal(4.0, (expression.Right as NumberLiteralNode)!.Value);
        }

        [Fact]
        public void ReturnStatementWithArithmeticExpression_ParsesCorrectly()
        {
            var code = @"
        function sum() { 
            return 1 + 2; 
        }
        main { sum(); }";
            var parser = SetupParser(code);

            var program = parser.Parse() as ProgramNode;
            Assert.NotNull(program);

            var function = program.Functions.First();
            Assert.NotNull(function);

            var returnStatement = (function.Body as CompoundStatementNode)!.Statements.First();
            Assert.NotNull(returnStatement);

            var expression = (returnStatement as ReturnStatementNode)!.ReturnValue as BinaryOperatorNode;
            Assert.NotNull(expression);
            Assert.Equal(TokenType.Plus, expression.Operator.Type);
            Assert.Equal(1.0, (expression.Left as NumberLiteralNode)!.Value);
            Assert.Equal(2.0, (expression.Right as NumberLiteralNode)!.Value);
        }
    }
}