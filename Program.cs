using System;
using System.Collections.Generic;

namespace Compilador_All
{
    class Program
    {
        static void Main(string[] args)
        {
            string ruta = @"C:\Users\sebsa\Documents\escuela de mierda\codigo.txt";

            try
            {
                Console.WriteLine("Iniciando análisis léxico...");
                AnalizadorLexico lexico = new AnalizadorLexico(ruta);
                List<Token> tokens = lexico.Escanear();

                Console.WriteLine("\n===== TOKENS GENERADOS =====");
                foreach (var token in tokens)
                {
                    Console.WriteLine(token);
                }

                Console.WriteLine("\nIniciando análisis sintáctico...");
                AnalizadorSintactico sintactico = new AnalizadorSintactico(tokens);
                sintactico.Programa();

                // Si llega hasta aquí sin lanzar excepción (Error), significa que triunfaste
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n¡Análisis Sintáctico Exitoso! El código cumple con las reglas de Spark.");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n" + ex.Message);
                Console.ResetColor();
            }

            Console.WriteLine("\nPresione una tecla para salir...");
            Console.ReadKey();
        }
    }
}