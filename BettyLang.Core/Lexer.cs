namespace BettyLang.Core
{
    public class Lexer
    {
        private readonly string _input;
        private int _position;
        private char _currentChar;

        public Lexer(string input)
        {
            _input = input;
            _position = 0;
            _currentChar = _input.Length > 0 ? _input[_position] : '\0';
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
                                                                      // Add other escape sequences as needed
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
                stringBuilder.Append("0.");

            while (Char.IsDigit(_currentChar) || _currentChar == '.')
            {
                if (_currentChar == '.')
                {
                    if (dotEncountered)
                        throw new FormatException("Invalid numeric format with multiple dots.");

                    dotEncountered = true;
                }

                stringBuilder.Append(_currentChar);
                Advance();
            }

            return stringBuilder.ToString();
        }

        public Token GetNextToken()
        {
            while (_currentChar != '\0')
            {
                if (Char.IsWhiteSpace(_currentChar))
                {
                    SkipWhitespace();
                    continue;
                }

                if (Char.IsDigit(_currentChar))
                    return new Token(TokenType.NumberLiteral, ScanNumberLiteral(false));

                if (_currentChar == '.')
                {
                    Advance();

                    if (Char.IsDigit(_currentChar))
                        return new Token(TokenType.NumberLiteral, ScanNumberLiteral(true));
                }

                switch (_currentChar)
                {
                    case '+':
                        Advance();
                        return new Token(TokenType.Plus, "+");
                    case '-':
                        Advance();
                        return new Token(TokenType.Minus, "-");
                    case '*':
                        Advance();
                        return new Token(TokenType.Mul, "*");
                    case '/':
                        Advance();
                        return new Token(TokenType.Div, "/");
                    case '^':
                        Advance();
                        return new Token(TokenType.Caret, "^");
                    case '(':
                        Advance();
                        return new Token(TokenType.LParen, "(");
                    case ')':
                        Advance();
                        return new Token(TokenType.RParen, ")");
                    case '\'':
                    case '"':
                        return new Token(TokenType.StringLiteral, ScanStringLiteral());
                    default:
                        throw new Exception($"Invalid character '{_currentChar}'");
                }
            }

            return new Token(TokenType.EOF, "");
        }
    }
}