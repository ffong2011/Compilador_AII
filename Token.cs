namespace Compilador_AII
{
    public class Token
    {
        public int Tipo { get; }
        public string Lexema { get; }
        public int Linea { get; }

        public Token(int tipo, string lexema, int linea)
        {
            Tipo = tipo;
            Lexema = lexema;
            Linea = linea;
        }

        public override string ToString()
        {
            return $"Linea {Linea} -> Token {Tipo} : {Lexema}";
        }
    }
}