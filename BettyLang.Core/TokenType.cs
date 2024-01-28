namespace BettyLang.Core
{
    public enum TokenType
    {
        NumberLiteral, StringLiteral, TrueLiteral, FalseLiteral, Identifier,

        Not, And, Or,

        Plus, Minus, Mul, Div, Caret, Mod,

        LParen, RParen, LBracket, RBracket, Semicolon, Comma, QuestionMark, Colon,

        Main, Function,

        Assign, If, Elif, Else, While, Break, Continue, Return,
        
        Equal, LessThan, GreaterThan, LessThanOrEqual, GreaterThanOrEqual, NotEqual,

        EOF
    }
}