﻿using BettyLang.Core.Interpreter;

namespace BettyLang.Core.AST
{
    public class CharLiteral(char value) : Expression
    {
        public char Value { get; } = value;

        public override Value Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}