namespace Analizador
{
    public abstract class Node
    {
        public int Line { get; }
        public int Column { get; }
        protected Node(int line, int column) { Line = line; Column = column; }
    }

    // Expresiones
    public abstract class Expr : Node { protected Expr(int l, int c) : base(l, c) { } }
    public sealed class NumberExpr : Expr
    {
        public double Value { get; }
        public NumberExpr(double value, int l, int c) : base(l, c) { Value = value; }
        public override string ToString() => Value.ToString();
    }
    public sealed class IdentifierExpr : Expr
    {
        public string Name { get; }
        public IdentifierExpr(string name, int l, int c) : base(l, c) { Name = name; }
        public override string ToString() => Name;
    }
    public enum BinOp { Add, Sub, Mul, Div }
    public sealed class BinaryExpr : Expr
    {
        public Expr Left { get; }
        public BinOp Op { get; }
        public Expr Right { get; }
        public BinaryExpr(Expr left, BinOp op, Expr right, int l, int c) : base(l, c) { Left = left; Op = op; Right = right; }
        public override string ToString() => $"({Left} {Op} {Right})";
    }
    public sealed class UnaryExpr : Expr
    {
        public bool IsNegative { get; }
        public Expr Inner { get; }
        public UnaryExpr(bool isNegative, Expr inner, int l, int c) : base(l, c) { IsNegative = isNegative; Inner = inner; }
        public override string ToString() => (IsNegative ? "-" : "+") + Inner;
    }

    // Sentencias
    public abstract class Stmt : Node { protected Stmt(int l, int c) : base(l, c) { } }
    public sealed class VarDecl : Stmt
    {
        public string Kind { get; } // var|let|const
        public string Name { get; }
        public Expr? Init { get; }
        public VarDecl(string kind, string name, Expr? init, int l, int c) : base(l, c) { Kind = kind; Name = name; Init = init; }
        public override string ToString() => $"{Kind} {Name} = {Init}";
    }
    public sealed class AssignStmt : Stmt
    {
        public string Name { get; }
        public Expr Expr { get; }
        public AssignStmt(string name, Expr expr, int l, int c) : base(l, c) { Name = name; Expr = expr; }
        public override string ToString() => $"{Name} = {Expr}";
    }
    public sealed class PrintStmt : Stmt
    {
        public Expr Expr { get; }
        public PrintStmt(Expr expr, int l, int c) : base(l, c) { Expr = expr; }
        public override string ToString() => $"print({Expr})";
    }
    public sealed class ProgramNode : Node
    {
        public IReadOnlyList<Stmt> Statements { get; }
        public ProgramNode(List<Stmt> stmts) : base(1, 1) { Statements = stmts; }
        public override string ToString() => string.Join("\n", Statements);
    }
}
