﻿namespace BettyLang.Core.AST
{
    public abstract class Statement
    {
        public abstract void Accept(IStatementVisitor visitor);
    }
}