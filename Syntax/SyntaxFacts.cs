using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace Compilador_AII.Syntax
{
    public static class SyntaxFacts
    {
        // Este método recibe un texto y revisa si es una palabra reservada.
        // Si lo es, devuelve su Token específico. Si no, dice que es un Identificador normal.
        public static SyntaxKind GetKeywordKind(string text)
        {
            // Convertimos a minúsculas para cumplir la regla "case-insensitive"
            switch (text.ToLower())
            {
                case "procedure": return SyntaxKind.ProcedureKeyword;
                case "is": return SyntaxKind.IsKeyword;
                case "begin": return SyntaxKind.BeginKeyword;
                case "end": return SyntaxKind.EndKeyword;
                case "if": return SyntaxKind.IfKeyword;
                case "then": return SyntaxKind.ThenKeyword;
                case "else": return SyntaxKind.ElseKeyword;
                case "while": return SyntaxKind.WhileKeyword;
                case "loop": return SyntaxKind.LoopKeyword;
                case "exit": return SyntaxKind.ExitKeyword;
                case "when": return SyntaxKind.WhenKeyword;
                case "put": return SyntaxKind.PutKeyword;
                case "integer": return SyntaxKind.IntegerKeyword;
                case "float": return SyntaxKind.FloatKeyword;
                default: return SyntaxKind.IdentifierToken;
            }
        }
    }
}
