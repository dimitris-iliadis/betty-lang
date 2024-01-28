namespace BettyLang.Tests.RuntimeTests
{
    public class CustomFunctionTests : InterpreterTest
    {
        [Fact]
        public void SimpleFunction_ReturnsConstantValue()
        {
            var code = @"
                function simple() { return 42; }
                main { return simple(); }
            ";
            var interpreter = SetupInterpreterCustom(code);
            var result = interpreter.Interpret();
            Assert.Equal(42, result.AsDouble());
        }

        [Fact]
        public void FunctionWithParameters_CalculatesSum()
        {
            var code = @"
                function sum(a, b) { return a + b; }
                main { return sum(5, 7); }
            ";
            var interpreter = SetupInterpreterCustom(code);
            var result = interpreter.Interpret();
            Assert.Equal(12, result.AsDouble());
        }

        [Fact]
        public void RecursiveFunction_CalculatesFactorial()
        {
            var code = @"
                function fact(n) {
                    if (n <= 1) { return 1; }
                    return n * fact(n - 1);
                }
                main { return fact(5); }
            ";
            var interpreter = SetupInterpreterCustom(code);
            var result = interpreter.Interpret();
            Assert.Equal(120, result.AsDouble());
        }

        [Fact]
        public void NestedFunctionCalls_WorkCorrectly()
        {
            var code = @"
                function inner(a) { return a * a; }
                function outer(b) { return inner(b) + inner(b + 1); }
                main { return outer(3); }
            ";
            var interpreter = SetupInterpreterCustom(code);
            var result = interpreter.Interpret();
            Assert.Equal(9 + 16, result.AsDouble()); // 3*3 + 4*4
        }

        [Fact]
        public void FunctionWithLoop_IteratesCorrectly()
        {
            var code = @"
                function sumToN(n) {
                    result = 0;
                    i = 1;
                    while (i <= n) {
                        result = result + i;
                        i = i + 1;
                    }
                    return result;
                }
                main { return sumToN(5); }
            ";
            var interpreter = SetupInterpreterCustom(code);
            var result = interpreter.Interpret();
            Assert.Equal(15, result.AsDouble()); // Sum of 1 to 5
        }
    }
}