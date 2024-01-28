namespace BettyLang.Tests.RuntimeTests
{
    public class ConditionalStatementTests : InterpreterTestBase
    {
        [Fact]
        public void IfStatement_ExecutesCorrectBranch()
        {
            var code = @"
                    if (true) { return 42; }
                    return 0;
            ";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(42.0, result.AsDouble());
        }

        [Fact]
        public void IfStatement_SingleStatement_ExecutesCorrectBranch()
        {
            var code = @"
                    if (true) return 42;
                    return 0;
            ";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(42.0, result.AsDouble());
        }

        [Fact]
        public void IfStatement_ExecutesCorrectBranch_WithElse()
        {
            var code = @"
                    if (false) { return 42; }
                    else { return 0; }
            ";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(0.0, result.AsDouble());
        }

        [Fact]
        public void IfStatement_SingleStatement_ExecutesCorrectBranch_WithElse()
        {
            var code = @"
                    if (false) return 42;
                    else return 0;
            ";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(0.0, result.AsDouble());
        }

        [Fact]
        public void IfStatement_ExecutesCorrectBranch_WithElseIf()
        {
            var code = @"
                    if (false) { return 42; }
                    elif (true) { return 0; }
            ";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(0.0, result.AsDouble());
        }

        [Fact]
        public void IfStatement_SingleStatement_ExecutesCorrectBranch_WithElseIf()
        {
            var code = @"
                    if (false) return 42;
                    elif (true) return 0;
            ";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(0.0, result.AsDouble());
        }

        [Fact]
        public void IfStatement_ExecutesCorrectBranch_WithElseIf_WithElse()
        {
            var code = @"
                    if (false) { return 42; }
                    elif (false) { return 0; }
                    else { return 1; }
            ";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(1.0, result.AsDouble());
        }

        [Fact]
        public void IfStatement_SingleStatement_ExecutesCorrectBranch_WithElseIf_WithElse()
        {
            var code = @"
                    if (false) return 42;
                    elif (false) return 0;
                    else return 1;
            ";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(1.0, result.AsDouble());
        }

        [Fact]
        public void IfStatement_ExecutesCorrectBranch_WithElseIf_WithElse_WithNestedIf()
        {
            var code = @"
                    if (false) { return 42; }
                    elif (false) { return 0; }
                    elif (true) { if (true) { return 1; } }
                    else { return 2; }
            ";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(1.0, result.AsDouble());
        }

        [Fact]
        public void IfStatement_SingleStatement_ExecutesCorrectBranch_WithElseIf_WithElse_WithNestedIf()
        {
            var code = @"
                    if (false) return 42;
                    elif (false) return 0;
                    elif (true) if (true) return 1;
                    else return 2;
            ";
            var interpreter = SetupInterpreter(code);

            var result = interpreter.Interpret();
            Assert.Equal(1.0, result.AsDouble());
        }
    }
}