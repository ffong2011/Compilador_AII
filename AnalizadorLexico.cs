using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Compilador_All
{
    public class AnalizadorLexico
    {
        private string codigo;
        private int posicion;
        private int linea;
        private Dictionary<string, int> reservadas;

        public AnalizadorLexico(string ruta)
        {
            codigo = File.ReadAllText(ruta) + " \n";
            posicion = 0;
            linea = 1;

            reservadas = new Dictionary<string, int>()
            {
                {"procedure", TipoToken.PROCEDURE}, {"is", TipoToken.IS},
                {"begin", TipoToken.BEGIN}, {"end", TipoToken.END},
                {"if", TipoToken.IF}, {"then", TipoToken.THEN},
                {"else", TipoToken.ELSE}, {"while", TipoToken.WHILE},
                {"loop", TipoToken.LOOP}, {"exit", TipoToken.EXIT},
                {"when", TipoToken.WHEN}, {"put", TipoToken.PUT},
                {"integer", TipoToken.INTEGER}, {"float", TipoToken.FLOAT}
            };
        }

        public List<Token> Escanear()
        {
            List<Token> tokens = new List<Token>();

            while (posicion < codigo.Length)
            {
                if (char.IsWhiteSpace(codigo[posicion]))
                {
                    if (codigo[posicion] == '\n') linea++;
                    posicion++;
                    continue;
                }

                int estado = 1;
                StringBuilder lexema = new StringBuilder();

                while (posicion < codigo.Length)
                {
                    char c = codigo[posicion];
                    int columna = ObtenerColumna(c);

                    // PROTECCIÓN VITAL: Evita el error "Index was outside the bounds"
                    if (estado <= 0 || estado > MatrizTransicion.Tabla.GetLength(0)) break;

                    int nuevoEstado = MatrizTransicion.Tabla[estado - 1, columna];

                    if (nuevoEstado >= 20)
                    {
                        estado = nuevoEstado;
                        if (!MatrizTransicion.RequiereRetraccion(estado))
                        {
                            lexema.Append(c);
                            posicion++;
                        }
                        break;
                    }

                    if (nuevoEstado <= 0)
                    {
                        estado = nuevoEstado;
                        break;
                    }

                    estado = nuevoEstado;
                    lexema.Append(c);
                    posicion++;
                }

                if (estado >= 20)
                {
                    string lex = lexema.ToString();
                    foreach (char ch in lex) { if (ch == '\n') linea++; }
                    if (estado != 34) // Ignorar comentarios
                    {
                        tokens.Add(GenerarToken(estado, lex));
                    }
                }
            }
            return tokens;
        }

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
            if (c == '+') return 9;
            if (c == '-') return 10;
            if (c == '*') return 11;
            if (c == ';') return 12;
            if (c == ',') return 13;
            if (c == '(') return 14;
            if (c == ')') return 15;
            if (c == '\n') return 17;
            if (char.IsWhiteSpace(c)) return 16;
            return 18;
        }

        private Token GenerarToken(int estado, string lexema)
        {
            string lexLimpio = lexema.TrimEnd('\r', '\n');
            string lex = lexLimpio.ToLower();

            switch (estado)
            {
                case 20: return reservadas.ContainsKey(lex) ? new Token(reservadas[lex], lexLimpio, linea) : new Token(TipoToken.ID, lexLimpio, linea);
                case 21: return new Token(TipoToken.NUM_ENTERO, lexLimpio, linea);
                case 22: return new Token(TipoToken.NUM_FLOAT, lexLimpio, linea);
                case 24: return new Token(TipoToken.ASIGNACION, lexLimpio, linea);
                case 25: return new Token(TipoToken.MENOR, lexLimpio, linea);
                case 26: return new Token(TipoToken.MENOR_IGUAL, lexLimpio, linea);
                case 27: return new Token(TipoToken.MAYOR, lexLimpio, linea);
                case 28: return new Token(TipoToken.MAYOR_IGUAL, lexLimpio, linea);
                case 29: return new Token(TipoToken.DIV, lexLimpio, linea);
                case 30: return new Token(TipoToken.DISTINTO, lexLimpio, linea);
                case 31: return new Token(TipoToken.PUNTO, lexLimpio, linea);
                case 32: return new Token(TipoToken.RANGO, lexLimpio, linea);
                case 33: return new Token(TipoToken.MENOS, lexLimpio, linea);
                case 34: return new Token(TipoToken.COMENTARIO, lexLimpio, linea);
                case 35: return new Token(TipoToken.MAS, lexLimpio, linea);
                case 36: return new Token(TipoToken.MULT, lexLimpio, linea);
                case 37: return new Token(TipoToken.PUNTO_Y_COMA, lexLimpio, linea);
                case 38: return new Token(TipoToken.COMA, lexLimpio, linea);
                case 39: return new Token(TipoToken.PAR_ABRE, lexLimpio, linea);
                case 40: return new Token(TipoToken.PAR_CIERRA, lexLimpio, linea);
                case 41: return new Token(TipoToken.IGUAL, lexLimpio, linea);
                case 50: return new Token(TipoToken.DOS_PUNTOS, lexLimpio, linea);
                default: return new Token(TipoToken.ERROR_SIMBOLO, lexLimpio, linea);
            }
        }
    }
}