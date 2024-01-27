namespace BettyLang.Tests.RuntimeTests
{
    public class BuiltInFunctionsTests : InterpreterTestBase
    {
        [Fact]
        public void PrintFunction_PrintsCorrectValue()
        {
            var code = "print(2 + 3);";
            var interpreter = SetupInterpreter(code);
            var output = new StringWriter();
            Console.SetOut(output);

            interpreter.Interpret();

            Assert.Equal("5", output.ToString());
        }

        [Fact]
        public void PrintFunction_PrintsCorrectValueWithNewLine()
        {
            var code = """print(2 + 3, "\n");""";
            var interpreter = SetupInterpreter(code);
            var output = new StringWriter();
            Console.SetOut(output);

            interpreter.Interpret();

            Assert.Equal("5\n", output.ToString());
        }
    }
}