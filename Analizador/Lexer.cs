using Analizador;
using System;
using System.Collections.Generic;
using System.Text;

namespace Analizador
{
    public sealed class Lexer
    {
        private readonly string _src;
        private int _pos;
        private int _line = 1;
        private int _col = 1;

        private static readonly HashSet<string> _reserved = new(StringComparer.Ordinal)
        {
            "var", "let", "const", "print", "if", "else", "while", "return"
        };

        public Lexer(string source)
        {
            _src = source ?? string.Empty;
        }

        private char Current => _pos < _src.Length ? _src[_pos] : '\0';

        private char Peek(int k = 1)
        {
            var i = _pos + k;
            return i < _src.Length ? _src[i] : '\0';
        }

        private void Advance(int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                if (_pos >= _src.Length) return;
                if (Current == '\n') { _line++; _col = 1; }
                else _col++;
                _pos++;
            }
        }

        private Token Make(TokenType type, string lexeme, int line, int col)
            => new Token(type, lexeme, line, col);

        public IEnumerable<Token> ScanAll(bool includeTrivia = false)
        {
            while (true)
            {
                var t = NextToken(includeTrivia);
                if (t.Type != TokenType.Whitespace && t.Type != TokenType.Comment)
                    yield return t;
                else if (includeTrivia)
                    yield return t;

                if (t.Type == TokenType.EOF) yield break;
            }
        }

        public Token NextToken(bool includeTrivia = false)
        {
            // Saltar espacios
            if (char.IsWhiteSpace(Current))
            {
                var (line0, col0) = (_line, _col);
                var sb = new StringBuilder();
                while (char.IsWhiteSpace(Current))
                {
                    sb.Append(Current);
                    Advance();
                }
                return new Token(TokenType.Whitespace, sb.ToString(), line0, col0);
            }

            // Comentario de línea: //...
            if (Current == '/' && Peek() == '/')
            {
                var (line0, col0) = (_line, _col);
                var sb = new StringBuilder();
                sb.Append(Current); Advance();
                sb.Append(Current); Advance();
                while (Current != '\n' && Current != '\0')
                {
                    sb.Append(Current);
                    Advance();
                }
                return new Token(TokenType.Comment, sb.ToString(), line0, col0);
            }

            // Fin de archivo
            if (Current == '\0')
                return new Token(TokenType.EOF, string.Empty, _line, _col);

            // Números: entero o decimal (simple)
            if (char.IsDigit(Current))
            {
                var (line0, col0) = (_line, _col);
                var sb = new StringBuilder();
                while (char.IsDigit(Current))
                {
                    sb.Append(Current);
                    Advance();
                }
                // parte decimal opcional
                if (Current == '.' && char.IsDigit(Peek()))
                {
                    sb.Append(Current);
                    Advance();
                    while (char.IsDigit(Current))
                    {
                        sb.Append(Current);
                        Advance();
                    }
                }
                return Make(TokenType.Number, sb.ToString(), line0, col0);
            }

            // Identificadores o palabras reservadas
            if (char.IsLetter(Current) || Current == '_')
            {
                var (line0, col0) = (_line, _col);
                var sb = new StringBuilder();
                sb.Append(Current);
                Advance();
                while (char.IsLetterOrDigit(Current) || Current == '_')
                {
                    sb.Append(Current);
                    Advance();
                }
                var lex = sb.ToString();
                if (_reserved.Contains(lex))
                    return Make(TokenType.ReservedWord, lex, line0, col0);
                return Make(TokenType.Identifier, lex, line0, col0);
            }

            // Símbolos sencillos
            {
                var (line0, col0) = (_line, _col);
                var ch = Current;
                Advance();

                return ch switch
                {
                    '+' => Make(TokenType.Plus, "+", line0, col0),
                    '-' => Make(TokenType.Minus, "-", line0, col0),
                    '*' => Make(TokenType.Star, "*", line0, col0),
                    '/' => Make(TokenType.Slash, "/", line0, col0), // si no era comentario, es slash operador
                    '=' => Make(TokenType.Assign, "=", line0, col0),
                    ';' => Make(TokenType.Semicolon, ";", line0, col0),
                    '(' => Make(TokenType.LParen, "(", line0, col0),
                    ')' => Make(TokenType.RParen, ")", line0, col0),
                    _ => Make(TokenType.Unknown, ch.ToString(), line0, col0)
                };
            }
        }
    }
}
