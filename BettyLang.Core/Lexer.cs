using BettyLang.Core.AST;

namespace BettyLang.Core
{
    public class Lexer
    {
        private readonly string _input;
        private int _position;
        private char _currentChar;

        private static readonly Dictionary<string, Token> _keywords = new()
        {
            ["main"] = new Token(TokenType.Main, "main"),
            ["function"] = new Token(TokenType.Function, "function"),
            ["true"] = new Token(TokenType.TrueLiteral, "true"),
            ["false"] = new Token(TokenType.FalseLiteral, "false"),
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

            Advance(); // Skip the opening quote

            while (_currentChar != '"')
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
                '!' => (TokenType.Not, "!"),
                '&' => (TokenType.And, "&"),
                '=' => (TokenType.Assign, "="),
                '|' => (TokenType.Or, "|"),
                _ => throw new Exception($"Invalid character '{_currentChar}' at position {_position}")
            };

            Advance();
            return new Token(type, value);
        }

        private Token ScanIdentifierOrKeyword()
        {
            var stringBuilder = new System.Text.StringBuilder();

            while (_currentChar != '\0' && Char.IsLetterOrDigit(_currentChar))
            {
                stringBuilder.Append(_currentChar);
                Advance();
            }

            var result = stringBuilder.ToString().ToLower();

            if (_keywords.TryGetValue(result, out Token? token))
                return token;

            return new Token(TokenType.Identifier, result);
        }

        private char PeekNextChar() => (_position + 1 >= _input.Length) ? '\0' : _input[_position + 1];

        private Token ScanComparisonOperator()
        {
            if (_currentChar == '=' && PeekNextChar() == '=')
            {
                Advance();
                Advance();
                return new Token(TokenType.Equal, "==");
            }

            if (_currentChar == '<' || _currentChar == '>')
            {
                char symbol = _currentChar;

                if (PeekNextChar() == '=')
                {
                    Advance();
                    Advance();
                    if (symbol == '<')
                        return new Token(TokenType.LessThanOrEqual, "<=");
                    else
                        return new Token(TokenType.GreaterThanOrEqual, ">=");
                }

                Advance();

                if (symbol == '<')
                    return new Token(TokenType.LessThan, "<");
                else
                    return new Token(TokenType.GreaterThan, ">");
            }

            if (_currentChar == '!' && PeekNextChar() == '=')
                return new Token(TokenType.NotEqual, "!=");

            throw new Exception($"Invalid character '{_currentChar}' at position {_position}");
        }

        private void SkipComment()
        {
            while (_currentChar != '\0' && _currentChar != '\n')
                Advance();
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

                if (_currentChar == '/' && PeekNextChar() == '/')
                {
                    SkipComment();
                    continue;
                }

                if (Char.IsLetter(_currentChar))
                    return ScanIdentifierOrKeyword();

                if (Char.IsDigit(_currentChar))
                    return new Token(TokenType.NumberLiteral, ScanNumberLiteral(hasLeadingDot: false));

                if (_currentChar == '.' && Char.IsDigit(PeekNextChar()))
                    return new Token(TokenType.NumberLiteral, ScanNumberLiteral(hasLeadingDot: true));

                if (_currentChar == '<' 
                    || _currentChar == '>' 
                    || (_currentChar == '=' && PeekNextChar() == '=') 
                    || (_currentChar == '!' && PeekNextChar() == '='))
                    return ScanComparisonOperator();

                if (_currentChar == '"')
                    return new Token(TokenType.StringLiteral, ScanStringLiteral());

                return ScanSingleCharToken();
            }

            return new Token(TokenType.EOF, "");
        }
    }
}