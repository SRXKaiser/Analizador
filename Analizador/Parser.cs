using Analizador;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Analizador
{
    public sealed class ParseError : Exception
    {
        public int Line { get; }
        public int Column { get; }
        public ParseError(string msg, int line, int column) : base(msg) { Line = line; Column = column; }
        public override string ToString() => $"[L{Line},C{Column}] {Message}";
    }

    public sealed class Parser
    {
        private readonly List<Token> _tokens;
        private int _i;

        public Parser(IEnumerable<Token> tokens)
        {
            _tokens = new List<Token>(tokens);
        }

        private Token T => _i < _tokens.Count ? _tokens[_i] : _tokens[^1];
        private bool Match(TokenType tt)
        {
            if (_i < _tokens.Count && _tokens[_i].Type == tt) { _i++; return true; }
            return false;
        }
        private Token Expect(TokenType tt, string msg)
        {
            if (_i < _tokens.Count && _tokens[_i].Type == tt) return _tokens[_i++];
            throw new ParseError(msg, T.Line, T.Column);
        }

        public ProgramNode ParseProgram()
        {
            var stmts = new List<Stmt>();
            while (_i < _tokens.Count && _tokens[_i].Type != TokenType.EOF)
            {
                stmts.Add(ParseStatement());
            }
            return new ProgramNode(stmts);
        }

        private Stmt ParseStatement()
        {
            // var/let/const
            if (Look(TokenType.ReservedWord, out var w) && (w.Lexeme == "var" || w.Lexeme == "let" || w.Lexeme == "const"))
                return ParseVarDecl();

            // print(...)
            if (Look(TokenType.ReservedWord, out w) && w.Lexeme == "print")
                return ParsePrint();

            // asignación simple: id = expr ;
            if (Look(TokenType.Identifier, out var id) && LookAheadIs(1, TokenType.Assign))
                return ParseAssign();

            // expresión sola terminada en ;
            var e = ParseExpression();
            Expect(TokenType.Semicolon, "Se esperaba ';' al final de la expresión.");
            return new PrintStmt(e, e.Line, e.Column); // truco: tratamos una expr sola como print implícito para demo
        }

        private Stmt ParseVarDecl()
        {
            var kindTok = Expect(TokenType.ReservedWord, "Se esperaba 'var', 'let' o 'const'.");
            string kind = kindTok.Lexeme;
            var id = Expect(TokenType.Identifier, "Se esperaba nombre de variable.");
            Expr? init = null;
            if (Match(TokenType.Assign))
                init = ParseExpression();
            Expect(TokenType.Semicolon, "Se esperaba ';' al final de la declaración.");
            return new VarDecl(kind, id.Lexeme, init, kindTok.Line, kindTok.Column);
        }

        private Stmt ParseAssign()
        {
            var id = Expect(TokenType.Identifier, "Se esperaba identificador.");
            Expect(TokenType.Assign, "Se esperaba '='.");
            var expr = ParseExpression();
            Expect(TokenType.Semicolon, "Se esperaba ';'.");
            return new AssignStmt(id.Lexeme, expr, id.Line, id.Column);
        }

        private Stmt ParsePrint()
        {
            var p = Expect(TokenType.ReservedWord, "Se esperaba 'print'.");
            Expect(TokenType.LParen, "Se esperaba '(' después de 'print'.");
            var expr = ParseExpression();
            Expect(TokenType.RParen, "Se esperaba ')' después de la expresión.");
            Expect(TokenType.Semicolon, "Se esperaba ';' después de print.");
            return new PrintStmt(expr, p.Line, p.Column);
        }

        // Expr -> Term ((+|-) Term)*
        // Term -> Factor ((*|/) Factor)*
        // Factor -> (+|-) Factor | Number | Identifier | '(' Expr ')'
        private Expr ParseExpression()
        {
            var left = ParseTerm();
            while (true)
            {
                if (Match(TokenType.Plus))
                    left = new BinaryExpr(left, BinOp.Add, ParseTerm(), left.Line, left.Column);
                else if (Match(TokenType.Minus))
                    left = new BinaryExpr(left, BinOp.Sub, ParseTerm(), left.Line, left.Column);
                else break;
            }
            return left;
        }

        private Expr ParseTerm()
        {
            var left = ParseFactor();
            while (true)
            {
                if (Match(TokenType.Star))
                    left = new BinaryExpr(left, BinOp.Mul, ParseFactor(), left.Line, left.Column);
                else if (Match(TokenType.Slash))
                    left = new BinaryExpr(left, BinOp.Div, ParseFactor(), left.Line, left.Column);
                else break;
            }
            return left;
        }

        private Expr ParseFactor()
        {
            // unarios
            if (Match(TokenType.Plus))
            {
                var f = ParseFactor();
                return new UnaryExpr(false, f, f.Line, f.Column);
            }
            if (Match(TokenType.Minus))
            {
                var f = ParseFactor();
                return new UnaryExpr(true, f, f.Line, f.Column);
            }

            if (Match(TokenType.LParen))
            {
                var e = ParseExpression();
                Expect(TokenType.RParen, "Se esperaba ')'.");
                return e;
            }

            if (Look(TokenType.Number, out var num))
            {
                _i++;
                double v = double.Parse(num.Lexeme, CultureInfo.InvariantCulture);
                return new NumberExpr(v, num.Line, num.Column);
            }

            if (Look(TokenType.Identifier, out var id))
            {
                _i++;
                return new IdentifierExpr(id.Lexeme, id.Line, id.Column);
            }

            throw new ParseError($"Token inesperado: {T.Type} '{T.Lexeme}'", T.Line, T.Column);
        }

        private bool Look(TokenType tt, out Token tok)
        { tok = T; return _i < _tokens.Count && _tokens[_i].Type == tt; }
        private bool LookAheadIs(int k, TokenType tt)
        { var j = _i + k; return j < _tokens.Count && _tokens[j].Type == tt; }
    }
}
