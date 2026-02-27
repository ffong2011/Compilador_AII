using System;
using System.Collections.Generic;

namespace Compilador_AII.Syntax
{
    public class Lexer
    {
        private readonly string _text;
        private int _position;
        public List<string> Diagnostics { get; } = new List<string>();

        // =========================================================================
        // MATRIZ DE TRANSICIÓN CLÁSICA (Con Estados Intermedios y -1 para terminar)
        // Columnas (13): 
        // 0:L | 1:D | 2:_ | 3:. | 4:: | 5:= | 6:< | 7:> | 8:/ | 9:- | 10:Espacio | 11:Simbolos(+ * ( ) ; , =) | 12:Errores
        // =========================================================================
        private readonly int[,] _matriz = new int[,]
        {
            // L   D   _   .   :   =   <   >   /   -  Esp Sim Err
            {  1,  3, -1, 14,  6, 18,  8, 10, 12, 16, 19, 18, -1 }, // q0: Inicial
            {  1,  1,  2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 }, // q1: Identificador
            {  1,  1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 }, // q2: Id Intermedio (espera letra/digito tras el _)
            { -1,  3, -1,  4, -1, -1, -1, -1, -1, -1, -1, -1, -1 }, // q3: Entero
            { -1,  5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 }, // q4: Flotante Intermedio (espera digito tras el .)
            { -1,  5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 }, // q5: Flotante Final
            { -1, -1, -1, -1, -1,  7, -1, -1, -1, -1, -1, -1, -1 }, // q6: Dos puntos (:)
            { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 }, // q7: Asignacion (:=)
            { -1, -1, -1, -1, -1,  9, -1, -1, -1, -1, -1, -1, -1 }, // q8: Menor (<)
            { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 }, // q9: Menor o igual (<=)
            { -1, -1, -1, -1, -1, 11, -1, -1, -1, -1, -1, -1, -1 }, // q10: Mayor (>)
            { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 }, // q11: Mayor o igual (>=)
            { -1, -1, -1, -1, -1, 13, -1, -1, -1, -1, -1, -1, -1 }, // q12: Diagonal (/)
            { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 }, // q13: Diferente (/=)
            { -1, -1, -1, 15, -1, -1, -1, -1, -1, -1, -1, -1, -1 }, // q14: Punto (.)
            { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 }, // q15: Rango (..)
            { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 }, // q16: Resta (-)
            { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 }, // q17: Comentario (Manejado por Lookahead abajo)
            { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 }, // q18: Simbolos 1 char (+, *, (, ), ;, ,, =)
            { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 19, -1, -1 }  // q19: Espacios
        };

        public Lexer(string text) { _text = text; }

        private char Current => _position >= _text.Length ? '\0' : _text[_position];
        private char Lookahead => _position + 1 >= _text.Length ? '\0' : _text[_position + 1];
        private void Next() => _position++;

        private int ObtenerColumna(char c)
        {
            if (char.IsLetter(c)) return 0;
            if (char.IsDigit(c)) return 1;
            if (c == '_') return 2;
            if (c == '.') return 3;
            if (c == ':') return 4;
            if (c == '=') return 5;
            if (c == '<') return 6;
            if (c == '>') return 7;
            if (c == '/') return 8;
            if (c == '-') return 9;
            if (char.IsWhiteSpace(c)) return 10;
            if ("+*();,".Contains(c)) return 11;

            return 12; // Error de caracter (Ej. @, #)
        }

        public SyntaxToken NextToken()
        {
            if (_position >= _text.Length)
                return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, "\0", null);

            // Filtro de Comentarios (Si ve dos guiones, consume toda la linea y retorna sin usar la matriz)
            if (Current == '-' && Lookahead == '-')
            {
                var startComentario = _position;
                while (_position < _text.Length && Current != '\n' && Current != '\r') Next();
                return new SyntaxToken(SyntaxKind.CommentToken, startComentario, _text.Substring(startComentario, _position - startComentario), null);
            }

            int start = _position;
            int estado = 0;

            // Bucle clasico: Viajamos hasta encontrar un -1
            while (_position < _text.Length)
            {
                int columna = ObtenerColumna(Current);
                int siguienteEstado = _matriz[estado, columna];

                if (siguienteEstado == -1)
                    break;

                estado = siguienteEstado;
                Next();
            }

            // Si es 0 y se rompió el ciclo, es porque ingresaron basura desde el inicio
            if (estado == 0 && _position == start)
            {
                var caracterMalo = Current.ToString();
                Next();
                Diagnostics.Add($"ERROR LÉXICO [{start}]: Caracter no reconocido -> '{caracterMalo}'");
                return new SyntaxToken(SyntaxKind.Error_CaracterInvalido, start, caracterMalo, null);
            }

            string text = _text.Substring(start, _position - start);
            return CrearTokenPorEstado(estado, start, text);
        }

        private SyntaxToken CrearTokenPorEstado(int estado, int start, string text)
        {
            SyntaxKind kind = SyntaxKind.Error_CaracterInvalido;
            object value = null;

            switch (estado)
            {
                case 1: kind = SyntaxFacts.GetKeywordKind(text); break; // Verifica si es palabra reservada o ID
                case 3: kind = SyntaxKind.IntegerToken; value = int.Parse(text); break;
                case 5: kind = SyntaxKind.FloatToken; value = double.Parse(text, System.Globalization.CultureInfo.InvariantCulture); break;
                case 6: kind = SyntaxKind.ColonToken; break;
                case 7: kind = SyntaxKind.ColonEqualsToken; break;
                case 8: kind = SyntaxKind.LessToken; break;
                case 9: kind = SyntaxKind.LessOrEqualsToken; break;
                case 10: kind = SyntaxKind.GreaterToken; break;
                case 11: kind = SyntaxKind.GreaterOrEqualsToken; break;
                case 12: kind = SyntaxKind.SlashToken; break;
                case 13: kind = SyntaxKind.BangEqualsToken; break;
                case 14: kind = SyntaxKind.DotToken; break;
                case 15: kind = SyntaxKind.DotDotToken; break;
                case 16: kind = SyntaxKind.MinusToken; break;
                case 18:
                    // Mapeo de simbolos que caen en el estado 18
                    switch (text)
                    {
                        case "+": kind = SyntaxKind.PlusToken; break;
                        case "*": kind = SyntaxKind.StarToken; break;
                        case "=": kind = SyntaxKind.EqualsToken; break;
                        case "(": kind = SyntaxKind.OpenParenthesisToken; break;
                        case ")": kind = SyntaxKind.CloseParenthesisToken; break;
                        case ";": kind = SyntaxKind.SemicolonToken; break;
                        case ",": kind = SyntaxKind.CommaToken; break;
                    }
                    break;
                case 19: kind = SyntaxKind.WhiteSpaceToken; break;

                // --- MANEJO DE ESTADOS INTERMEDIOS (ERRORES) ---
                case 2: // Se quedó a la mitad en un guion bajo
                    kind = SyntaxKind.Error_IdentificadorInvalido;
                    Diagnostics.Add($"ERROR LÉXICO [{start}]: Identificador no puede terminar en '_' -> '{text}'");
                    break;
                case 4: // Se quedó a la mitad en un punto decimal
                    kind = SyntaxKind.Error_FlotanteIncompleto;
                    Diagnostics.Add($"ERROR LÉXICO [{start}]: Flotante incompleto -> '{text}'");
                    break;
            }

            return new SyntaxToken(kind, start, text, value);
        }
    }
}