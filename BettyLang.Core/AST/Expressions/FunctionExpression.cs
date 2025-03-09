﻿using BettyLang.Core.Interpreter;
using System.Reflection;

namespace BettyLang.Core.AST
{
    public class FunctionExpression(List<string> parameters, CompoundStatement body) : Expression
    {
        public List<string> Parameters { get; } = parameters;
        public CompoundStatement Body { get; } = body;

        public override Value Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }
}