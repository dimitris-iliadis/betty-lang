﻿using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class IndexerExpression(Expression collection, Expression index) : Expression
    {
        public Expression Collection { get; } = collection;
        public Expression Index { get; } = index;

        public override Value Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}