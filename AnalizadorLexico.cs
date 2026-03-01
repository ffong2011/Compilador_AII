using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Compilador_AII
{
    public class AnalizadorLexico
    {
        private string codigo;
        private int posicion;
        private int linea;

        private Dictionary<string, int> reservadas;

        public AnalizadorLexico(string ruta)
        {
            codigo = File.ReadAllText(ruta);
            posicion = 0;
            linea = 1;

            reservadas = new Dictionary<string, int>()
            {
                {"procedure", TipoToken.PROCEDURE},
                {"is", TipoToken.IS},
                {"begin", TipoToken.BEGIN},
                {"end", TipoToken.END},
                {"if", TipoToken.IF},
                {"then", TipoToken.THEN},
                {"else", TipoToken.ELSE},
                {"while", TipoToken.WHILE},
                {"loop", TipoToken.LOOP},
                {"exit", TipoToken.EXIT},
                {"when", TipoToken.WHEN},
                {"put", TipoToken.PUT},
                {"integer", TipoToken.INTEGER},
                {"float", TipoToken.FLOAT}
            };
        }

        public List<Token> Escanear()
        {
            List<Token> tokens = new List<Token>();

            while (posicion < codigo.Length)
            {
                int estado = 1;
                StringBuilder lexema = new StringBuilder();
                int inicio = posicion;

                while (posicion < codigo.Length)
                {
                    char c = codigo[posicion];

                    if (c == '\n') linea++;

                    int columna = ObtenerColumna(c);
                    int nuevoEstado = MatrizTransicion.Tabla[estado - 1, columna];

                    if (nuevoEstado >= 20)
                    {
                        estado = nuevoEstado;
                        break;
                    }

                    estado = nuevoEstado;
                    lexema.Append(c);
                    posicion++;
                }

                if (estado != 1)
                {
                    tokens.Add(GenerarToken(estado, lexema.ToString()));
                }

                posicion++;
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
            if (c == ' ') return 16;
            if (c == '\n') return 17;

            return 18;
        }

        private Token GenerarToken(int estado, string lexema)
        {
            string lex = lexema.ToLower();

            switch (estado)
            {
                case 20:
                    if (reservadas.ContainsKey(lex))
                        return new Token(reservadas[lex], lexema, linea);
                    return new Token(TipoToken.ID, lexema, linea);

                case 21: return new Token(TipoToken.NUM_ENTERO, lexema, linea);
                case 22: return new Token(TipoToken.NUM_FLOAT, lexema, linea);
                case 24: return new Token(TipoToken.ASIGNACION, lexema, linea);
                case 25: return new Token(TipoToken.MENOR, lexema, linea);
                case 26: return new Token(TipoToken.MENOR_IGUAL, lexema, linea);
                case 27: return new Token(TipoToken.MAYOR, lexema, linea);
                case 28: return new Token(TipoToken.MAYOR_IGUAL, lexema, linea);
                case 29: return new Token(TipoToken.DIV, lexema, linea);
                case 30: return new Token(TipoToken.DISTINTO, lexema, linea);
                case 31: return new Token(TipoToken.PUNTO, lexema, linea);
                case 32: return new Token(TipoToken.RANGO, lexema, linea);
                case 33: return new Token(TipoToken.MENOS, lexema, linea);
                case 34: return new Token(TipoToken.COMENTARIO, lexema, linea);
                case 35: return new Token(TipoToken.MAS, lexema, linea);
                case 36: return new Token(TipoToken.MULT, lexema, linea);
                case 37: return new Token(TipoToken.PUNTO_Y_COMA, lexema, linea);
                case 38: return new Token(TipoToken.COMA, lexema, linea);
                case 39: return new Token(TipoToken.PAR_ABRE, lexema, linea);
                case 40: return new Token(TipoToken.PAR_CIERRA, lexema, linea);
                case 41: return new Token(TipoToken.IGUAL, lexema, linea);
                case 90: return new Token(TipoToken.ERROR_ID, lexema, linea);
                case 91: return new Token(TipoToken.ERROR_NUM, lexema, linea);
                case 92: return new Token(TipoToken.ERROR_SIMBOLO, lexema, linea);
                default: return new Token(TipoToken.ERROR_SIMBOLO, lexema, linea);
            }
        }
    }
}