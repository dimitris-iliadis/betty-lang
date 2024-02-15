﻿using BettyLang.Core.AST;

namespace BettyLang.Core.Interpreter
{
    public static partial class IntrinsicFunctions
    {
        public static InterpreterResult IsDigitFunction(FunctionCall call, IExpressionVisitor visitor)
        {
            // Ensure that only one argument is provided
            if (call.Arguments.Count != 1)
                throw new Exception($"{call.FunctionName} function requires exactly one argument.");

            // Use the visitor to evaluate the argument
            var argResult = call.Arguments[0].Accept(visitor);

            if (argResult.Type == ResultType.Char)
            {
                // Return whether the character is a digit
                var isDigit = char.IsDigit(argResult.AsChar());
                return InterpreterResult.FromBoolean(isDigit);
            }

            throw new Exception($"{call.FunctionName} function is not defined for the given argument type.");
        }
    }
}