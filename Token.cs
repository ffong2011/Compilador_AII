using System;

namespace Compilador_AII
{
    public class Token
    {
        // Tipo de token (numero)
        public int Tipo { get; set; }

        // Lexema encontrado
        public string Lexema { get; set; }

        // Línea donde aparece
        public int Linea { get; set; }

        public Token(int tipo, string lexema, int linea)
        {
            Tipo = tipo;
            Lexema = lexema;
            Linea = linea;
        }

        public override string ToString()
        {
            return $"Token: {Tipo}  Lexema: {Lexema}  Línea: {Linea}";
        }
    }

    // Definición de todos los tipos de token
    public static class Tokens
    {
        public const int ID = 1;
        public const int NUM = 2;

        public const int PROCEDURE = 3;
        public const int IS = 4;
        public const int VAR = 5;
        public const int BEGIN = 6;
        public const int END = 7;

        public const int PUNTO = 8;       // .
        public const int PUNTO_COMA = 9;  // ;
        public const int COMA = 10;       // ,
        public const int DOS_PUNTOS = 11; // :

        public const int ASIGNACION = 12; // :=
    }
}