namespace Compilador_AII.Syntax
{
    public static class SyntaxFacts
    {
        public static SyntaxKind GetKeywordKind(string text)
        {
            switch (text.ToLower())
            {
                case "procedure": return SyntaxKind.ProcedureKeyword;
                case "is": return SyntaxKind.IsKeyword;
                case "begin": return SyntaxKind.BeginKeyword;
                case "end": return SyntaxKind.EndKeyword;
                case "integer": return SyntaxKind.IntegerKeyword;
                case "float": return SyntaxKind.FloatKeyword;
                case "string": return SyntaxKind.StringKeyword;
                case "while": return SyntaxKind.WhileKeyword;
                case "loop": return SyntaxKind.LoopKeyword;
                case "if": return SyntaxKind.IfKeyword;
                case "then": return SyntaxKind.ThenKeyword;
                case "else": return SyntaxKind.ElseKeyword;
                case "exit": return SyntaxKind.ExitKeyword;
                case "when": return SyntaxKind.WhenKeyword;
                case "put": return SyntaxKind.PutKeyword;
                case "get": return SyntaxKind.GetKeyword;
                default: return SyntaxKind.IdentifierToken; // Si no es reservada, es ID
            }
        }
    }
}