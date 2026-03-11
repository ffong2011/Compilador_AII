using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;

using System;
using System.Collections.Generic;
using System.IO;

using System;
using System.Collections.Generic;
using System.IO;

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

try
            {
                AnalizadorLexico lexico = new AnalizadorLexico(ruta);
                List<Token> tokens = lexico.Escanear();

                Console.WriteLine("\n===== TOKENS GENERADOS =====\n");

                foreach (var token in tokens)
                {
                    Console.WriteLine(token);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al procesar el archivo:");
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("\nPresione una tecla para salir...");
            Console.ReadKey();
        }
    }
}