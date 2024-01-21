using BettyLang.Core.AST;

public class ParameterNode : Node
{
    public string Name { get; }

    public ParameterNode(string name) { Name = name; }

    public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
}