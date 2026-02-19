using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Compilador_AII.Syntax
{
    public class Lexer
    {
        private readonly string _text;
        private int _position;

        // Aquí guardaremos los errores para mostrarlos en consola
        public List<string> Diagnostics { get; } = new List<string>();

        public Lexer(string text)
        {
            _text = text;
        }

        private char Current => Peek(0);
        private char Lookahead => Peek(1);

        private char Peek(int offset)
        {
            var index = _position + offset;
            if (index >= _text.Length) return '\0';
            return _text[index];
        }

        private void Next() => _position++;

        public SyntaxToken NextToken()
        {
            // 1. Fin de archivo
            if (_position >= _text.Length)
                return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, "\0", null);

            var start = _position;

            // 2. Espacios en blanco
            if (char.IsWhiteSpace(Current))
            {
                while (char.IsWhiteSpace(Current)) Next();
                var text = _text.Substring(start, _position - start);
                return new SyntaxToken(SyntaxKind.WhiteSpaceToken, start, text, null);
            }

            // 3. Identificadores y Palabras Reservadas
            // Regla: [a-zA-Z](_?[a-zA-Z0-9])*
            if (char.IsLetter(Current))
            {
                while (char.IsLetterOrDigit(Current) || Current == '_')
                    Next();

                var text = _text.Substring(start, _position - start);

                // Validación estricta: No puede terminar en guion bajo
                if (text.EndsWith("_"))
                {
                    Diagnostics.Add($"ERROR LÉXICO [{start}]: El identificador '{text}' no puede terminar en guion bajo.");
                    return new SyntaxToken(SyntaxKind.BadToken, start, text, null);
                }

                // Usamos el diccionario para saber si es Keyword o ID normal
                var kind = SyntaxFacts.GetKeywordKind(text);
                return new SyntaxToken(kind, start, text, null);
            }

            // 4. Números (Enteros y Flotantes)
            if (char.IsDigit(Current))
            {
                while (char.IsDigit(Current)) Next();

                // Revisamos si viene un punto seguido de OTRA cosa que no sea un punto (para evitar el rango ..)
                if (Current == '.' && char.IsDigit(Lookahead))
                {
                    Next(); // Consumimos el punto
                    while (char.IsDigit(Current)) Next();

                    var text = _text.Substring(start, _position - start);
                    return new SyntaxToken(SyntaxKind.FloatToken, start, text, text); // Aquí podríamos parsearlo a float
                }

                var intText = _text.Substring(start, _position - start);
                return new SyntaxToken(SyntaxKind.IntegerToken, start, intText, intText);
            }

            // 5. Operadores Compuestos y Simples
            switch (Current)
            {
                case '-':
                    if (Lookahead == '-') // Comentarios: --
                    {
                        Next(); Next(); // Consumimos los dos guiones
                        while (Current != '\r' && Current != '\n' && Current != '\0')
                            Next();

                        var text = _text.Substring(start, _position - start);
                        return new SyntaxToken(SyntaxKind.CommentToken, start, text, null);
                    }
                    Next(); return new SyntaxToken(SyntaxKind.MinusToken, start, "-", null);

                case ':':
                    if (Lookahead == '=') { Next(); Next(); return new SyntaxToken(SyntaxKind.ColonEqualsToken, start, ":=", null); }
                    Next(); return new SyntaxToken(SyntaxKind.ColonToken, start, ":", null);

                case '/':
                    if (Lookahead == '=') { Next(); Next(); return new SyntaxToken(SyntaxKind.BangEqualsToken, start, "/=", null); }
                    Next(); return new SyntaxToken(SyntaxKind.SlashToken, start, "/", null);

                case '<':
                    if (Lookahead == '=') { Next(); Next(); return new SyntaxToken(SyntaxKind.LessOrEqualsToken, start, "<=", null); }
                    Next(); return new SyntaxToken(SyntaxKind.LessToken, start, "<", null);

                case '>':
                    if (Lookahead == '=') { Next(); Next(); return new SyntaxToken(SyntaxKind.GreaterOrEqualsToken, start, ">=", null); }
                    Next(); return new SyntaxToken(SyntaxKind.GreaterToken, start, ">", null);

                case '.':
                    if (Lookahead == '.') { Next(); Next(); return new SyntaxToken(SyntaxKind.DotDotToken, start, "..", null); }
                    Next(); return new SyntaxToken(SyntaxKind.DotToken, start, ".", null);

                // Operadores de un solo caracter
                case '+': Next(); return new SyntaxToken(SyntaxKind.PlusToken, start, "+", null);
                case '*': Next(); return new SyntaxToken(SyntaxKind.StarToken, start, "*", null);
                case '=': Next(); return new SyntaxToken(SyntaxKind.EqualsToken, start, "=", null);
                case '(': Next(); return new SyntaxToken(SyntaxKind.OpenParenthesisToken, start, "(", null);
                case ')': Next(); return new SyntaxToken(SyntaxKind.CloseParenthesisToken, start, ")", null);
                case ';': Next(); return new SyntaxToken(SyntaxKind.SemicolonToken, start, ";", null);
                case ',': Next(); return new SyntaxToken(SyntaxKind.CommaToken, start, ",", null);
            }

            // Si llegamos aquí, encontró un caracter basura (ej: @, $, #)
            Diagnostics.Add($"ERROR LÉXICO [{start}]: Caracter no reconocido '{Current}'");
            var badText = Current.ToString();
            Next();
            return new SyntaxToken(SyntaxKind.BadToken, start, badText, null);
        }
    }
}