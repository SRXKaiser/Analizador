using Analizador;
using System;
using System.Collections.Generic;

namespace Analizador
{
    public sealed class EvalError : Exception
    {
        public int Line { get; }
        public int Column { get; }
        public EvalError(string msg, int line, int column) : base(msg) { Line = line; Column = column; }
        public override string ToString() => $"[L{Line},C{Column}] {Message}";
    }

    public sealed class Environment
    {
        private sealed class Entry { public double Value; public bool IsConst; }
        private readonly Dictionary<string, Entry> _map = new(StringComparer.Ordinal);

        public bool IsDefined(string name) => _map.ContainsKey(name);

        public void Declare(string kind, string name, double value)
        {
            if (_map.ContainsKey(name)) throw new EvalError($"'{name}' ya está declarado.", 0, 0);
            _map[name] = new Entry { Value = value, IsConst = (kind == "const") };
        }

        public void Assign(string name, double value)
        {
            if (!_map.TryGetValue(name, out var e)) throw new EvalError($"Variable no declarada: '{name}'.", 0, 0);
            if (e.IsConst) throw new EvalError($"No se puede asignar a constante '{name}'.", 0, 0);
            e.Value = value;
        }

        public double Get(string name)
        {
            if (!_map.TryGetValue(name, out var e)) throw new EvalError($"Variable no declarada: '{name}'.", 0, 0);
            return e.Value;
        }

        public IReadOnlyDictionary<string, double> Snapshot()
        {
            var d = new Dictionary<string, double>(StringComparer.Ordinal);
            foreach (var (k, v) in _map) d[k] = v.Value;
            return d;
        }
    }

    public sealed class Evaluator
    {
        private readonly Environment _env;
        public Evaluator(Environment env) { _env = env; }

        public List<string> Run(ProgramNode program)
        {
            var outLines = new List<string>();
            foreach (var s in program.Statements)
            {
                switch (s)
                {
                    case VarDecl v:
                        double init = v.Init is null ? 0.0 : Eval(v.Init);
                        _env.Declare(v.Kind, v.Name, init);
                        break;

                    case AssignStmt a:
                        _env.Assign(a.Name, Eval(a.Expr));
                        break;

                    case PrintStmt p:
                        outLines.Add(Format(Eval(p.Expr)));
                        break;
                }
            }
            return outLines;
        }

        private static string Format(double x)
            => Math.Abs(x % 1) < 1e-12 ? ((long)Math.Round(x)).ToString() : x.ToString(System.Globalization.CultureInfo.InvariantCulture);

        private double Eval(Expr e) => e switch
        {
            NumberExpr n => n.Value,
            IdentifierExpr i => _env.Get(i.Name),
            UnaryExpr u => u.IsNegative ? -Eval(u.Inner) : +Eval(u.Inner),
            BinaryExpr b => EvalBinary(b),
            _ => throw new EvalError("Expresión no soportada.", e.Line, e.Column),
        };

        private double EvalBinary(BinaryExpr b)
        {
            var L = Eval(b.Left);
            var R = Eval(b.Right);
            return b.Op switch
            {
                BinOp.Add => L + R,
                BinOp.Sub => L - R,
                BinOp.Mul => L * R,
                BinOp.Div => R == 0 ? throw new EvalError("División entre cero.", b.Line, b.Column) : L / R,
                _ => throw new EvalError("Operador binario desconocido.", b.Line, b.Column),
            };
        }
    }
}
