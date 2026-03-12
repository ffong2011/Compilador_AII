namespace Compilador_All
{
    public class Token
    {
        public int Tipo { get; set; }
        public string Lexema { get; set; }
        public int Linea { get; set; }

        public Token(int tipo, string lexema, int linea)
        {
            Tipo = tipo;
            Lexema = lexema;
            Linea = linea;
        }

        public override string ToString()
        {
            return $"Token: {Tipo,-4} Lexema: {Lexema,-15} Línea: {Linea}";
        }
    }
}