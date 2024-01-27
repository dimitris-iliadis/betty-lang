using BettyLang.Tests.TestUtilities;

namespace BettyLang.Tests.RuntimeTests
{
    public class ArithmeticExpressionsTests : InterpreterTestBase
    {
        [Fact]
        public void Addition_ReturnsCorrectSum()
        {
            var code = "return 2 + 3;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(5.0, result.AsDouble());
        }

        [Fact]
        public void Subtraction_ReturnsCorrectDifference()
        {
            var code = "return 5 - 2;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(3.0, result.AsDouble());
        }

        [Fact]
        public void Multiplication_ReturnsCorrectProduct()
        {
            var code = "return 3 * 4;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(12.0, result.AsDouble());
        }

        [Fact]
        public void Division_ReturnsCorrectQuotient()
        {
            var code = "return 10 / 2;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(5.0, result.AsDouble());
        }

        [Fact]
        public void MixedOperations_ReturnsCorrectValue()
        {
            var code = "return 2 + 3 * 4 - 5;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(9.0, result.AsDouble());
        }

        [Fact]
        public void PowerOperation_ReturnsCorrectValue()
        {
            var code = "return 2 ^ 3;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(8.0, result.AsDouble());
        }

        [Fact]
        public void CombinedAdditionAndPower_ReturnsCorrectValue()
        {
            var code = "return 2 + 3 ^ 2;";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(11.0, result.AsDouble());
        }

        [Fact]
        public void ComplexExpressionWithParentheses_ReturnsCorrectValue()
        {
            var code = "return (2 + 3) * (4 - 1);";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(15.0, result.AsDouble());
        }

        [Fact]
        public void NestedPowers_ReturnsCorrectValue()
        {
            var code = "return 2 ^ (1 + 1 ^ 3);";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(4.0, result.AsDouble());
        }
    }
}