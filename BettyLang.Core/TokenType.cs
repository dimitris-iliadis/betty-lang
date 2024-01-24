namespace BettyLang.Core
{
    public enum TokenType
    {
        NumberLiteral, StringLiteral, TrueLiteral, FalseLiteral, Identifier,

        Not, And, Or,

        Plus, Minus, Mul, Div, Caret,

        LParen, RParen, LBracket, RBracket, Semicolon, Comma,

        Main, Function,

        Assign, Print, Input, If,
        
        Equal, LessThan, GreaterThan, LessThanOrEqual, GreaterThanOrEqual, NotEqual,

        EOF
    }
}