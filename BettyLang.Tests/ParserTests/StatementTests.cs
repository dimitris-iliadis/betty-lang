using BettyLang.Core.AST;

namespace BettyLang.Tests.ParserTests
{
    public class StatementTests : ParserTest
    {
        [Fact]
        public void Parse_AssignmentStatement_CorrectlyParses()
        {
            var code = "a = 5;";
            var node = ExtractFirstStatement(code);

            var assignmentNode = Assert.IsType<AssignmentNode>(node);
            Assert.Equal("a", (assignmentNode.Left as VariableNode)!.Value);
            var numberNode = Assert.IsType<NumberLiteralNode>(assignmentNode.Right);
            Assert.Equal(5.0, numberNode.Value);
        }

        [Fact]
        public void Parse_CompoundStatement_CorrectlyParses()
        {
            var code = "{ a = 5; b = 10; }";
            var node = ExtractFirstStatement(code);

            var compoundNode = Assert.IsType<CompoundStatementNode>(node);
            Assert.Equal(2, compoundNode.Statements.Count);
            Assert.All(compoundNode.Statements, stmt => Assert.IsType<AssignmentNode>(stmt));
        }

        [Fact]
        public void Parse_IfStatement_CorrectlyParses()
        {
            var code = "if (a > 5) { b = 10; }";
            var node = ExtractFirstStatement(code);

            var ifNode = Assert.IsType<IfStatementNode>(node);
            var binaryOpNode = Assert.IsType<BinaryOperatorNode>(ifNode.Condition);
            Assert.Equal(TokenType.GreaterThan, binaryOpNode.Operator.Type);

            var compoundNode = Assert.IsType<CompoundStatementNode>(ifNode.ThenStatement);
            Assert.Single(compoundNode.Statements);
            Assert.IsType<AssignmentNode>(compoundNode.Statements[0]);
        }

        [Fact]
        public void Parse_WhileStatement_CorrectlyParses()
        {
            var code = "while (a < 10) { a = a + 1; }";
            var node = ExtractFirstStatement(code);

            var whileNode = Assert.IsType<WhileStatementNode>(node);
            var binaryOpNode = Assert.IsType<BinaryOperatorNode>(whileNode.Condition);
            Assert.Equal(TokenType.LessThan, binaryOpNode.Operator.Type);

            var compoundNode = Assert.IsType<CompoundStatementNode>(whileNode.Body);
            Assert.Single(compoundNode.Statements);
            Assert.IsType<AssignmentNode>(compoundNode.Statements[0]);
        }

        [Fact]
        public void Parse_FunctionCallStatement_CorrectlyParses()
        {
            var code = "myFunction(10, 20);";
            var node = ExtractFirstStatement(code);

            var functionCallNode = Assert.IsType<FunctionCallNode>(node);
            Assert.Equal("myFunction".ToLower(), functionCallNode.FunctionName); // Case insensitive
            Assert.Equal(2, functionCallNode.Arguments.Count);
            Assert.All(functionCallNode.Arguments, arg => Assert.IsType<NumberLiteralNode>(arg));
        }

        [Fact]
        public void Parse_ReturnStatement_CorrectlyParses()
        {
            var code = "return a + b;";
            var node = ExtractFirstStatement(code);

            var returnNode = Assert.IsType<ReturnStatementNode>(node);
            Assert.NotNull(returnNode.ReturnValue);
            Assert.IsType<BinaryOperatorNode>(returnNode.ReturnValue);
        }

        [Fact]
        public void Parse_BreakStatement_CorrectlyParses()
        {
            var code = "break;";
            var node = ExtractFirstStatement(code);
            Assert.IsType<BreakStatementNode>(node);
        }

        [Fact]
        public void Parse_ContinueStatement_CorrectlyParses()
        {
            var code = "continue;";
            var node = ExtractFirstStatement(code);
            Assert.IsType<ContinueStatementNode>(node);
        }
    }
}