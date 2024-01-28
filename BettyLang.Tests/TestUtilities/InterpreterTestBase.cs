using BettyLang.Core;

namespace BettyLang.Tests.TestUtilities
{
    public class InterpreterTestBase
    {
        protected Interpreter SetupInterpreter(string code)
        {
            return new Interpreter(new Parser(new Lexer($"main {{ {code} }}")));
        }

        protected Interpreter SetupInterpreterCustom(string code)
        {
            return new Interpreter(new Parser(new Lexer(code)));
        }
    }
}