namespace BettyLang.Core
{
    public class Lexer
    {
        private readonly string _input;
        private int _position;
        private char _currentChar;

        private static readonly Dictionary<string, Token> ReservedKeywords = new()
        {
            ["module"] = new Token(TokenType.Module, "module"),
        };

        public Lexer(string input)
        {
            _input = input;
            _position = 0;
            _currentChar = _input.Length > 0 ? _input[_position] : '\0'; // Handle empty input
        }

        private void Advance()
        {
            _position++;
            if (_position > _input.Length - 1)
                _currentChar = '\0';
            else
                _currentChar = _input[_position];
        }

        private void SkipWhitespace()
        {
            while (_currentChar != '\0' && Char.IsWhiteSpace(_currentChar))
                Advance();
        }

        private string ScanStringLiteral()
        {
            var stringBuilder = new System.Text.StringBuilder();
            char quoteType = _currentChar; // should be either ' or "

            Advance(); // Skip the opening quote

            while (_currentChar != quoteType)
            {
                if (_currentChar == '\\') // Check for escape character
                {
                    Advance(); // Skip the escape character
                    switch (_currentChar)
                    {
                        case 'n': stringBuilder.Append('\n'); break; // Newline
                        case 't': stringBuilder.Append('\t'); break; // Tab
                        case '"': stringBuilder.Append('\"'); break; // Double quote
                        case '\'': stringBuilder.Append('\''); break; // Single quote
                        case '\\': stringBuilder.Append('\\'); break; // Backslash
                        default:
                            throw new Exception($"Unrecognized escape sequence: \\{_currentChar}");
                    }
                    Advance(); // Move past the character after the escape
                }
                else
                {
                    stringBuilder.Append(_currentChar);
                    Advance();
                }

                if (_currentChar == '\0')
                    throw new Exception("Unterminated string literal.");
            }

            Advance(); // Skip the closing quote
            return stringBuilder.ToString();
        }

        private string ScanNumberLiteral(bool hasLeadingDot)
        {
            var stringBuilder = new System.Text.StringBuilder();

            bool dotEncountered = hasLeadingDot;

            if (hasLeadingDot)
            {
                stringBuilder.Append("0.");
                Advance(); // Move past the dot character
            }

            while (Char.IsDigit(_currentChar) || _currentChar == '.')
            {
                if (_currentChar == '.')
                {
                    if (dotEncountered) // Throw when encountering multiple dots
                        throw new FormatException("Invalid numeric format with multiple dots.");

                    dotEncountered = true;
                }

                stringBuilder.Append(_currentChar);
                Advance();
            }

            return stringBuilder.ToString();
        }

        private Token ScanSingleCharToken()
        {
            (TokenType type, string value) = _currentChar switch
            {
                '+' => (TokenType.Plus, "+"),
                '-' => (TokenType.Minus, "-"),
                '*' => (TokenType.Mul, "*"),
                '/' => (TokenType.Div, "/"),
                '^' => (TokenType.Caret, "^"),
                '(' => (TokenType.LParen, "("),
                ')' => (TokenType.RParen, ")"),
                '{' => (TokenType.LBracket, "{"),
                '}' => (TokenType.RBracket, "}"),
                ';' => (TokenType.Semicolon, ";"),
                _ => throw new Exception($"Invalid character '{_currentChar}' at position {_position}")
            };

            Advance();
            return new Token(type, value);
        }

        private Token ScanIdentifierOrKeyword()
        {
            var stringBuilder = new System.Text.StringBuilder();

            while (_currentChar != '\0' && char.IsLetterOrDigit(_currentChar))
            {
                stringBuilder.Append(_currentChar);
                Advance();
            }

            var result = stringBuilder.ToString().ToLower();

            if (ReservedKeywords.TryGetValue(result, out Token token))
                return token;

            return new Token(TokenType.Identifier, result);
        }

        private char PeekChar() => (_position + 1 >= _input.Length) ? '\0' : _input[_position + 1];

        public Token GetNextToken()
        {
            while (_currentChar != '\0')
            {
                if (Char.IsWhiteSpace(_currentChar))
                {
                    SkipWhitespace();
                    continue;
                }

                if (Char.IsLetter(_currentChar))
                    return ScanIdentifierOrKeyword();

                if (Char.IsDigit(_currentChar))
                    return new Token(TokenType.NumberLiteral, ScanNumberLiteral(hasLeadingDot: false));

                if (_currentChar == '.' && Char.IsDigit(PeekChar()))
                    return new Token(TokenType.NumberLiteral, ScanNumberLiteral(hasLeadingDot: true));

                if (_currentChar == '=')
                {
                    if (PeekChar() == '=')
                    {
                        Advance();
                        Advance();
                        return new Token(TokenType.EqualsEquals, "==");
                    }

                    Advance();
                    return new Token(TokenType.Equals, "=");
                }

                if (_currentChar == '\'' || _currentChar == '"')
                    return new Token(TokenType.StringLiteral, ScanStringLiteral());

                return ScanSingleCharToken();
            }

            return new Token(TokenType.EOF, "");
        }
    }
}