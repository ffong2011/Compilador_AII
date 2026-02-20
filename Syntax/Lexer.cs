using System.Collections.Generic;

namespace Compilador_AII.Syntax
{
    public class Lexer
    {
        private readonly string _text;
        private int _position;
        public List<string> Diagnostics { get; } = new List<string>();

        // =========================================================================
        // MATRIZ DE TRANSICIONES (Filas: Estados, Columnas: Alfabeto)
        // =========================================================================
        private readonly int[,] _matriz = new int[,]
        {
            // L   D   _   .   :   =   <   >   /   Esp Sim Err   (COLUMNAS)
            {  1,  2, -1,  9,  5, -1,  6,  7,  8, 16, 15, -1 }, // Estado 0: Inicial (q0)
            {  1,  1,  1, -1, -1, -1, -1, -1, -1, -1, -1, -1 }, // Estado 1: Identificador
            { -1,  2, -1,  3, -1, -1, -1, -1, -1, -1, -1, -1 }, // Estado 2: Numero Entero
            { -1,  4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 }, // Estado 3: Vio un punto tras entero
            { -1,  4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 }, // Estado 4: Numero Flotante
            { -1, -1, -1, -1, -1, 10, -1, -1, -1, -1, -1, -1 }, // Estado 5: Vio ':'
            { -1, -1, -1, -1, -1, 11, -1, -1, -1, -1, -1, -1 }, // Estado 6: Vio '<'
            { -1, -1, -1, -1, -1, 12, -1, -1, -1, -1, -1, -1 }, // Estado 7: Vio '>'
            { -1, -1, -1, -1, -1, 13, -1, -1, -1, -1, -1, -1 }, // Estado 8: Vio '/'
            { -1, -1, -1, 14, -1, -1, -1, -1, -1, -1, -1, -1 }, // Estado 9: Vio '.' inicial
            { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 }, // Estado 10: Asignacion (:=)
            { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 }, // Estado 11: Menor o Igual (<=)
            { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 }, // Estado 12: Mayor o Igual (>=)
            { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 }, // Estado 13: Diferente (/=)
            { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 }, // Estado 14: Rango (..)
            { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 }, // Estado 15: Simbolos de 1 char (+, -)
            { -1, -1, -1, -1, -1, -1, -1, -1, -1, 16, -1, -1 }  // Estado 16: Espacios en blanco
        };

        public Lexer(string text)
        {
            _text = text;
        }

        private char Current => _position >= _text.Length ? '\0' : _text[_position];
        private char Lookahead => _position + 1 >= _text.Length ? '\0' : _text[_position + 1];

        private void Next() => _position++;

        private int ObtenerColumnaAlfabeto(char c)
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
            if (char.IsWhiteSpace(c)) return 9;

            if (c == '+' || c == '-' || c == '*' || c == '(' || c == ')' || c == ';' || c == ',')
                return 10;

            return 11;
        }

        public SyntaxToken NextToken()
        {
            if (_position >= _text.Length)
                return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, "\0", null);

            // ========================================================
            // PRE-FILTRO: IGNORAR COMENTARIOS
            // ========================================================
            if (Current == '-' && Lookahead == '-')
            {
                var startComentario = _position;
                while (_position < _text.Length && Current != '\n' && Current != '\r')
                {
                    Next();
                }
                var textComentario = _text.Substring(startComentario, _position - startComentario);
                return new SyntaxToken(SyntaxKind.CommentToken, startComentario, textComentario, null);
            }

            var start = _position;
            int estadoActual = 0;
            int ultimoEstadoAceptado = 0;

            // NAVEGACIÓN DE LA MATRIZ
            while (_position < _text.Length)
            {
                int columna = ObtenerColumnaAlfabeto(Current);
                int siguienteEstado = _matriz[estadoActual, columna];

                if (siguienteEstado == -1)
                    break;

                estadoActual = siguienteEstado;
                ultimoEstadoAceptado = estadoActual;
                Next();
            }

            var text = _text.Substring(start, _position - start);

            return CrearTokenPorEstado(ultimoEstadoAceptado, start, text);
        }

        private SyntaxToken CrearTokenPorEstado(int estado, int start, string text)
        {
            switch (estado)
            {
                case 1:
                    if (text.EndsWith("_"))
                    {
                        Diagnostics.Add($"ERROR LÉXICO [{start}]: El identificador '{text}' no puede terminar en guion bajo.");
                        return new SyntaxToken(SyntaxKind.BadToken, start, text, null);
                    }
                    var kind = SyntaxFacts.GetKeywordKind(text);
                    return new SyntaxToken(kind, start, text, null);

                case 2: return new SyntaxToken(SyntaxKind.IntegerToken, start, text, int.Parse(text));
                case 4: return new SyntaxToken(SyntaxKind.FloatToken, start, text, double.Parse(text, System.Globalization.CultureInfo.InvariantCulture));

                case 5: return new SyntaxToken(SyntaxKind.ColonToken, start, text, null);
                case 6: return new SyntaxToken(SyntaxKind.LessToken, start, text, null);
                case 7: return new SyntaxToken(SyntaxKind.GreaterToken, start, text, null);
                case 8: return new SyntaxToken(SyntaxKind.SlashToken, start, text, null);
                case 9: return new SyntaxToken(SyntaxKind.DotToken, start, text, null);

                case 10: return new SyntaxToken(SyntaxKind.ColonEqualsToken, start, text, null);
                case 11: return new SyntaxToken(SyntaxKind.LessOrEqualsToken, start, text, null);
                case 12: return new SyntaxToken(SyntaxKind.GreaterOrEqualsToken, start, text, null);
                case 13: return new SyntaxToken(SyntaxKind.BangEqualsToken, start, text, null);
                case 14: return new SyntaxToken(SyntaxKind.DotDotToken, start, text, null);

                case 15:
                    return GenerarTokenSimple(text[0], start, text);

                case 16: return new SyntaxToken(SyntaxKind.WhiteSpaceToken, start, text, null);

                case 3:
                case 0:
                default:
                    if (estado == 0 && text.Length == 0)
                    {
                        text = Current.ToString();
                        Next();
                    }
                    Diagnostics.Add($"ERROR LÉXICO [{start}]: Texto no reconocido '{text}'");
                    return new SyntaxToken(SyntaxKind.BadToken, start, text, null);
            }
        }

        private SyntaxToken GenerarTokenSimple(char c, int start, string text)
        {
            switch (c)
            {
                case '+': return new SyntaxToken(SyntaxKind.PlusToken, start, text, null);
                case '-': return new SyntaxToken(SyntaxKind.MinusToken, start, text, null);
                case '*': return new SyntaxToken(SyntaxKind.StarToken, start, text, null);
                case '=': return new SyntaxToken(SyntaxKind.EqualsToken, start, text, null);
                case '(': return new SyntaxToken(SyntaxKind.OpenParenthesisToken, start, text, null);
                case ')': return new SyntaxToken(SyntaxKind.CloseParenthesisToken, start, text, null);
                case ';': return new SyntaxToken(SyntaxKind.SemicolonToken, start, text, null);
                case ',': return new SyntaxToken(SyntaxKind.CommaToken, start, text, null);
                default: return new SyntaxToken(SyntaxKind.BadToken, start, text, null);
            }
        }
    }
}
