namespace BettyLang.Core.AST
{
    public class ModuleNode : Node
    {
        public string Name { get; }
        public Node Block { get; }

        public ModuleNode(string name, Node block)
        {
            Name = name;
            Block = block;
        }

        public override T Accept<T>(NodeVisitor<T> visitor) => visitor.Visit(this);
    }
}