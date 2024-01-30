namespace BettyLang.Tests.ParserTests
{
    public class SingleStatementControlFlowTests : ParserTest
    {
        [Fact]
        public void ParsesSingleStatementIf()
        {
            var code = "func main() { if (x > 5) x = x - 1; }";
            var parser = SetupParser(code);
            var result = parser.Parse() as ProgramNode;

            var mainFunction = result.Functions[0];
            var ifStatement = mainFunction.Body.Statements[0] as IfStatementNode;

            Assert.NotNull(ifStatement);
            Assert.IsType<AssignmentNode>(ifStatement.ThenStatement);
            Assert.Null(ifStatement.ElseStatement);
        }

        [Fact]
        public void ParsesSingleStatementElseIf()
        {
            var code = "func main() { if (x > 5) x = x - 1; elif (x < 5) x = x + 1; }";
            var parser = SetupParser(code);
            var result = parser.Parse() as ProgramNode;

            var mainFunction = result.Functions[0];
            var ifStatement = mainFunction.Body.Statements[0] as IfStatementNode;

            Assert.NotNull(ifStatement);
            Assert.Single(ifStatement.ElseIfStatements);
            var elifStatement = ifStatement.ElseIfStatements[0].Statement;
            Assert.IsType<AssignmentNode>(elifStatement);
        }

        [Fact]
        public void ParsesSingleStatementElse()
        {
            var code = "func main() { if (x > 5) x = x - 1; else x = x + 1; }";
            var parser = SetupParser(code);
            var result = parser.Parse() as ProgramNode;

            var mainFunction = result.Functions[0];
            var ifStatement = mainFunction.Body.Statements[0] as IfStatementNode;

            Assert.NotNull(ifStatement);
            Assert.IsType<AssignmentNode>(ifStatement.ElseStatement);
        }

        [Fact]
        public void ParsesSingleStatementWhile()
        {
            var code = "func main() { while (x < 10) x = x + 1; }";
            var parser = SetupParser(code);
            var result = parser.Parse() as ProgramNode;

            var mainFunction = result.Functions[0];
            var whileStatement = mainFunction.Body.Statements[0] as WhileStatementNode;

            Assert.NotNull(whileStatement);
            Assert.IsType<AssignmentNode>(whileStatement.Body);
        }
    }
}