namespace BettyLang.Tests.ParserTests
{
    public class ControlFlowTests : ParserTest
    {
        [Fact]
        public void ParsesIfStatement()
        {
            var code = "func main() { if (x > 5) { x = x - 1; } }";
            var parser = SetupParser(code);
            var result = parser.Parse() as ProgramNode;

            Assert.NotNull(result);
            var mainFunction = result.Functions[0];
            var ifStatement = mainFunction.Body.Statements[0] as IfStatementNode;

            Assert.NotNull(ifStatement);
            Assert.NotNull(ifStatement.Condition);
            Assert.NotNull(ifStatement.ThenStatement);
            Assert.Null(ifStatement.ElseStatement);
        }

        [Fact]
        public void ParsesWhileStatement()
        {
            var code = "func main() { while (x < 10) { x = x + 1; } }";
            var parser = SetupParser(code);
            var result = parser.Parse() as ProgramNode;

            var mainFunction = result.Functions[0];
            var whileStatement = mainFunction.Body.Statements[0] as WhileStatementNode;

            Assert.NotNull(whileStatement);
            Assert.NotNull(whileStatement.Condition);
            Assert.Single((whileStatement.Body as CompoundStatementNode)!.Statements);
        }

        [Fact]
        public void ParsesCompoundStatement()
        {
            var code = "func main() { { x = 0; y = 1; } }";
            var parser = SetupParser(code);
            var result = parser.Parse() as ProgramNode;

            var mainFunction = result.Functions[0];
            var compoundStatement = mainFunction.Body.Statements[0] as CompoundStatementNode;

            Assert.NotNull(compoundStatement);
            Assert.Equal(2, compoundStatement.Statements.Count);
        }

        [Fact]
        public void ParsesNestedControlFlowStatements()
        {
            var code = "func main() { if (x > 5) { while (y < 10) { y = y + 1; } } }";
            var parser = SetupParser(code);
            var result = parser.Parse() as ProgramNode;

            var mainFunction = result.Functions[0];
            var ifStatement = mainFunction.Body.Statements[0] as IfStatementNode;
            var whileStatement = (ifStatement!.ThenStatement as CompoundStatementNode)!.Statements[0] as WhileStatementNode;

            Assert.NotNull(ifStatement);
            Assert.NotNull(whileStatement);
            Assert.Single((whileStatement.Body as CompoundStatementNode)!.Statements);
        }

        [Fact]
        public void ParsesIfElseStatement()
        {
            var code = "func main() { if (x > 5) { x = x - 1; } else { x = x + 1; } }";
            var parser = SetupParser(code);
            var result = parser.Parse() as ProgramNode;

            var mainFunction = result.Functions[0];
            var ifStatement = mainFunction.Body.Statements[0] as IfStatementNode;

            Assert.NotNull(ifStatement);
            Assert.NotNull(ifStatement.ElseStatement);
            Assert.Single((ifStatement.ThenStatement as CompoundStatementNode)!.Statements);
            Assert.Single((ifStatement.ElseStatement as CompoundStatementNode)!.Statements);
        }
    }
}