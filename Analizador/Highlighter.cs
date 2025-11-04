using Analizador;
using System.Drawing;
using System.Windows.Forms;

namespace Analizador
{
    public static class Highlighter
    {
        public static void Colorize(RichTextBox rtb, IEnumerable<Token> tokens, IEnumerable<(int line, int col, string msg)>? errors = null)
        {
            rtb.SuspendLayout();
            int start = 0;
            rtb.SelectAll();
            rtb.SelectionColor = Color.Black;

            foreach (var t in tokens)
            {
                var idx = IndexFromLineCol(rtb, t.Line, t.Column);
                if (idx < 0) continue;
                rtb.Select(idx, t.Lexeme.Length);
                rtb.SelectionColor = t.Type switch
                {
                    TokenType.Number => Color.DarkBlue,
                    TokenType.Identifier => Color.DarkGreen,
                    TokenType.ReservedWord => Color.MediumVioletRed,
                    TokenType.Plus or TokenType.Minus or TokenType.Star or TokenType.Slash => Color.Firebrick,
                    TokenType.Assign => Color.Sienna,
                    TokenType.Semicolon => Color.SlateGray,
                    TokenType.LParen or TokenType.RParen => Color.SteelBlue,
                    TokenType.Comment => Color.DarkGray,
                    _ => Color.Black
                };
            }

            // subrayado rojo de errores
            if (errors != null)
            {
                foreach (var e in errors)
                {
                    var i = IndexFromLineCol(rtb, e.line, e.col);
                    if (i < 0) continue;
                    rtb.Select(i, 1);
                    rtb.SelectionBackColor = Color.MistyRose;
                }
            }

            rtb.Select(start, 0);
            rtb.ResumeLayout();
        }

        // Mapear línea/columna a índice en RichTextBox
        private static int IndexFromLineCol(RichTextBox rtb, int line, int col)
        {
            // RichTextBox: Line index 0-based; nuestras líneas son 1-based.
            int lineIdx = line - 1;
            if (lineIdx < 0 || lineIdx >= rtb.Lines.Length) return -1;
            int baseIdx = rtb.GetFirstCharIndexFromLine(lineIdx);
            return baseIdx + (col - 1);
        }
    }
}
