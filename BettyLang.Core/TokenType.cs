namespace BettyLang.Core
{
    public enum TokenType
    {
        NumberLiteral, StringLiteral, Identifier,

        Plus, Minus, Mul, Div, Caret,

        LParen, RParen, LBracket, RBracket, Semicolon, Comma,

        Main, Function,

        Equal, EqualEqual,

        EOF
    }
}