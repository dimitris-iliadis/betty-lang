namespace BettyLang.Core
{
    public class InterpreterResult
    {
        public object? Value { get; }

        public InterpreterResult(object? value) { Value = value; }
    }
}