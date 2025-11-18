using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Analizador;

namespace Analizador
{
    public partial class Form1 : Form
    {
        private string _codigoFuente = string.Empty;
        private string _rutaArchivo = string.Empty;

        public Form1()
        {
            InitializeComponent();
            Text = "Analizador Léxico - Operaciones Aritméticas";
        }

        private void btnAbrir_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Selecciona el archivo de código fuente";
            openFileDialog1.Filter = "Texto|*.txt;*.code;*.src|Todos|*.*";
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                txtRuta.Text = openFileDialog1.FileName;
                _rutaArchivo = openFileDialog1.FileName;
                _codigoFuente = File.ReadAllText(openFileDialog1.FileName, Encoding.UTF8);
                rtbSalida.Clear();
                rtbSalida.AppendText("Archivo cargado. Presiona \"Analizar\".\n");
            }
        }

        private void btnAnalizar_Click(object sender, EventArgs e)
        {
            rtbSalida.Clear();

            if (string.IsNullOrWhiteSpace(_codigoFuente))
            {
                rtbSalida.AppendText("No hay código cargado. Usa \"Abrir archivo…\" \n");
                return;
            }

            try
            {
                var lexer = new Lexer(_codigoFuente);
                var tokens = lexer.ScanAll().ToList();

                // Encabezado
                rtbSalida.SelectionColor = System.Drawing.Color.Black;
                rtbSalida.AppendText("Línea:Col  Tipo          Lexema               Descripción\n");
                rtbSalida.AppendText("---------------------------------------------------------\n");

                foreach (var t in tokens)
                {
                    // Columna Linea:Col y Tipo (negro)
                    rtbSalida.SelectionColor = System.Drawing.Color.Black;
                    rtbSalida.AppendText($"{t.Line}:{t.Column,-4} {t.Type,-12} ");

                    // Lexema coloreado según tipo
                    rtbSalida.SelectionColor = ColorFor(t.Type);
                    rtbSalida.AppendText($"'{t.Lexeme}'".PadRight(20));

                    // Descripcion (negro)
                    rtbSalida.SelectionColor = System.Drawing.Color.Black;
                    rtbSalida.AppendText($"  {Descripcion(t)}\n");
                }

                // Exportar CSV
                if (!string.IsNullOrWhiteSpace(_rutaArchivo) && File.Exists(_rutaArchivo))
                {
                    var dir = Path.GetDirectoryName(_rutaArchivo)!;
                    var baseName = Path.GetFileNameWithoutExtension(_rutaArchivo);
                    var csvPath = Path.Combine(dir, $"tokens_{baseName}.csv");

                    var sb = new StringBuilder();
                    sb.AppendLine("Linea,Columna,Tipo,Lexema,Descripcion");
                    foreach (var t in tokens)
                    {
                        var lexemaCsv = t.Lexeme.Replace("\"", "\"\"");
                        sb.AppendLine($"{t.Line},{t.Column},{t.Type},\"{lexemaCsv}\",\"{Descripcion(t)}\"");
                    }
                    File.WriteAllText(csvPath, sb.ToString(), Encoding.UTF8);

                    rtbSalida.AppendText("\n");
                    rtbSalida.SelectionColor = System.Drawing.Color.DarkSlateGray;
                    rtbSalida.AppendText($"[Exportación] CSV generado: {csvPath}\n");
                    rtbSalida.SelectionColor = System.Drawing.Color.Black;
                }
            }
            catch (Exception ex)
            {
                rtbSalida.SelectionColor = System.Drawing.Color.Red;
                rtbSalida.AppendText($"Error durante el análisis: {ex.Message}\n");
                rtbSalida.SelectionColor = System.Drawing.Color.Black;
            }
        }

        // ---- Helpers ----
        private static System.Drawing.Color ColorFor(TokenType tt) => tt switch
        {
            TokenType.Number => System.Drawing.Color.DarkBlue,
            TokenType.Identifier => System.Drawing.Color.DarkGreen,
            TokenType.ReservedWord => System.Drawing.Color.MediumVioletRed,
            TokenType.Plus or TokenType.Minus or TokenType.Star or TokenType.Slash
                                    => System.Drawing.Color.Firebrick,
            TokenType.Assign => System.Drawing.Color.Sienna,
            TokenType.Semicolon => System.Drawing.Color.SlateGray,
            TokenType.LParen or TokenType.RParen
                                    => System.Drawing.Color.SteelBlue,
            TokenType.Comment => System.Drawing.Color.DarkGray,
            _ => System.Drawing.Color.Black
        };

        private static string Descripcion(Token t) => t.Type switch
        {
            TokenType.Number => "Número (dígito/s)",
            TokenType.Identifier => "Identificador",
            TokenType.ReservedWord => "Palabra reservada",
            TokenType.Plus => "Operador aritmético: suma",
            TokenType.Minus => "Operador aritmético: resta",
            TokenType.Star => "Operador aritmético: multiplicación",
            TokenType.Slash => "Operador aritmético: división",
            TokenType.Assign => "Operador de asignación",
            TokenType.Semicolon => "Punto y coma",
            TokenType.LParen => "Paréntesis izquierdo",
            TokenType.RParen => "Paréntesis derecho",
            TokenType.Unknown => "Símbolo no reconocido",
            TokenType.EOF => "Fin de archivo",
            TokenType.Whitespace => "Espacio en blanco",
            TokenType.Comment => "Comentario",
            _ => ""
        };
    }
}
