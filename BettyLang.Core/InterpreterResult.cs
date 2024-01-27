namespace BettyLang.Core
{
    public class InterpreterResult
    {
        public object? Value { get; }

        public InterpreterResult(object? value) { Value = value; }

        public bool AsBoolean() => Convert.ToBoolean(Value);
        public double AsDouble() => Value != null ? Convert.ToDouble(Value) : 0.0;
        public string AsString() => Value?.ToString() ?? string.Empty;
    }
}