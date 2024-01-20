namespace BettyLang.Core
{
    public class InterpreterResult
    {
        public object Value { get; }

        public InterpreterResult(object value) { Value = value; }

        public string AsString() => Value.ToString()!;
        public double AsNumber() => Convert.ToDouble(Value);
    }
}