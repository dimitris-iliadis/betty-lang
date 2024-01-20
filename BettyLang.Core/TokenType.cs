namespace BettyLang.Core
{
    public enum TokenType
    {
        NumberLiteral, StringLiteral, Identifier,

        Plus, Minus, Mul, Div, Caret,

        LParen, RParen, LBracket, RBracket, Semicolon,

        Module,

        Equals, EqualsEquals,

        EOF
    }
}