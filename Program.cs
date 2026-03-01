using System;
using System.Collections.Generic;
using System.IO;

namespace Compilador_AII
{
    class Program
    {
        static void Main(string[] args)
        {
            string ruta = @"C:\Users\sebsa\Desktop\PROGRAMMING SHI\CompiladorAM\CompiladorAM\src\compiladoram\codigo.txt";

            if (!File.Exists(ruta))
            {
                Console.WriteLine("Archivo no encontrado.");
                Console.ReadKey();
                return;
            }

            AnalizadorLexico lexico = new AnalizadorLexico(ruta);
            List<Token> tokens = lexico.Escanear();

            foreach (var token in tokens)
                Console.WriteLine(token);

            Console.ReadKey();
        }
    }
}