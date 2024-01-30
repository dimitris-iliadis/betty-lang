namespace BettyLang.Core
{
    public enum TokenType
    {
        NumberLiteral, StringLiteral, BooleanLiteral, Identifier,

        Not, And, Or,

        Plus, Minus, Star, Slash, Caret, Percent,

        LParen, RParen, LBrace, RBrace, Semicolon, Comma, QuestionMark, Colon,

        Function,

        Assignment, If, Elif, Else, While, Break, Continue, Return,
        
        Equal, LessThan, GreaterThan, LessThanOrEqual, GreaterThanOrEqual, NotEqual,

        EOF
    }
}