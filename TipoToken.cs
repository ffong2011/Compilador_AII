namespace Compilador_AII
{
    public static class TipoToken
    {
        // Reservadas
        public const int PROCEDURE = 101;
        public const int IS = 102;
        public const int BEGIN = 103;
        public const int END = 104;
        public const int IF = 105;
        public const int THEN = 106;
        public const int ELSE = 107;
        public const int WHILE = 108;
        public const int LOOP = 109;
        public const int EXIT = 110;
        public const int WHEN = 111;
        public const int PUT = 112;
        public const int INTEGER = 113;
        public const int FLOAT = 114;

        public const int ID = 200;

        public const int NUM_ENTERO = 300;
        public const int NUM_FLOAT = 301;

        public const int ASIGNACION = 401;
        public const int IGUAL = 402;
        public const int DISTINTO = 403;
        public const int MENOR = 404;
        public const int MAYOR = 405;
        public const int MENOR_IGUAL = 406;
        public const int MAYOR_IGUAL = 407;

        public const int MAS = 408;
        public const int MENOS = 409;
        public const int MULT = 410;
        public const int DIV = 411;

        public const int PUNTO_Y_COMA = 501;
        public const int DOS_PUNTOS = 502;
        public const int COMA = 503;
        public const int PAR_ABRE = 504;
        public const int PAR_CIERRA = 505;
        public const int PUNTO = 506;
        public const int RANGO = 507;

        public const int COMENTARIO = 600;

        public const int ERROR_ID = 901;
        public const int ERROR_NUM = 902;
        public const int ERROR_SIMBOLO = 903;
    }
}