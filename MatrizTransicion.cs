namespace Compilador_AII
{
    public static class MatrizTransicion
    {
        // Columnas
        public const int LETRA = 0;
        public const int DIGITO = 1;
        public const int GUION_BAJO = 2;
        public const int PUNTO = 3;
        public const int DOS_PUNTOS = 4;
        public const int IGUAL = 5;
        public const int MENOR = 6;
        public const int MAYOR = 7;
        public const int SLASH = 8;
        public const int MAS = 9;
        public const int MENOS = 10;
        public const int ASTERISCO = 11;
        public const int PUNTO_COMA = 12;
        public const int COMA = 13;
        public const int PAR_ABRE = 14;
        public const int PAR_CIERRA = 15;
        public const int ESPACIO = 16;
        public const int SALTO_LINEA = 17;
        public const int OTRO = 18;

   

        public static readonly int[,] Tabla = new int[,]
        {
        /*                 L   D   _   .   :   =   <   >   /   +   -   *   ;   ,   (   )  sp  nl  ot */
        /* 1 Inicio     */ { 2,  4, 92, 15,  7, 41,  9, 11, 13, 35, 17, 36, 37, 38, 39, 40,  1,  1, 92 },

        /* 2 ID         */ { 2,  2,  3, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 90 },

        /* 3 ID_        */ { 2,  2, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90 },

        /* 4 Entero     */ {21,  4, 21,  5, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21 },

        /* 5 PuntoFloat */ {91,  6, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91, 91 },

        /* 6 Float      */ {22,  6, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22 },

        /* 7 :          */ { 7,  7,  7,  7,  7,  8,  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,  7 },

        /* 8 :=         */ { 8,  8,  8,  8,  8,  8,  8,  8,  8,  8,  8,  8,  8,  8,  8,  8,  8,  8,  8 },

        /* 9 <          */ { 9,  9,  9,  9,  9, 10,  9,  9,  9,  9,  9,  9,  9,  9,  9,  9,  9,  9,  9 },

        /*10 <=         */ {10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 },

        /*11 >          */ {11, 11, 11, 11, 11, 12, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11 },

        /*12 >=         */ {12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12 },

        /*13 /          */ {13, 13, 13, 13, 13, 14, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13 },

        /*14 /=         */ {14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14 },

        /*15 .          */ {15, 15, 15, 16, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15 },

        /*16 ..         */ {16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16 },

        /*17 -          */ {17, 17, 17, 17, 17, 17, 17, 17, 17, 17, 18, 17, 17, 17, 17, 17, 17, 17, 17 },

        /*18 Comentario */ {18, 18, 18, 18, 18, 18, 18, 18, 18, 18, 18, 18, 18, 18, 18, 18, 18,  1, 18 }
        };

        public static bool EsEstadoFinal(int estado)
        {
            switch (estado)
            {
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 20:
                case 21:
                case 22:
                case 35:
                case 36:
                case 37:
                case 38:
                case 39:
                case 40:
                case 41:
                case 90:
                case 91:
                case 92:
                    return true;
                default:
                    return false;
            }
        }

        public static bool RequiereRetraccion(int estado)
        {
            switch (estado)
            {
                case 7:
                case 9:
                case 11:
                case 13:
                case 15:
                case 17:
                case 20:
                case 21:
                case 22:
                case 90:
                case 91:
                    return true;
                default:
                    return false;
            }
        }
    }
}