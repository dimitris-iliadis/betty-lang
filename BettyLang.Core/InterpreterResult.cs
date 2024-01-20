namespace BettyLang.Core
{
    public class InterpreterResult
    {
        public object Value { get; }

        public InterpreterResult(object value) { Value = value; }

        public string AsString() => Value?.ToString() ?? string.Empty;
        public double AsNumber() => Value != null ? Convert.ToDouble(Value) : 0.0;
    }
}