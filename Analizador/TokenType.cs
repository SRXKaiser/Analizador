using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analizador
{
    public enum TokenType
    {
        // Léxicos “útiles”
        Number,             // 123, 45.67
        Identifier,         // foo, _x1
        ReservedWord,       // var, let, const, print
        Plus,               // +
        Minus,              // -
        Star,               // *
        Slash,              // /
        Assign,             // =
        Semicolon,          // ;
        LParen,             // (
        RParen,             // )

        // “No visibles” o auxiliares
        Whitespace,         // espacios/tabs/nuevas líneas
        Comment,            // // comentario hasta fin de línea

        // Errores
        Unknown,            // símbolo no reconocido
        EOF                 // fin de archivo
    }
}
