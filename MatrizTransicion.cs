using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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