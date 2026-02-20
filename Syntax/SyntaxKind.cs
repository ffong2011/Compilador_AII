namespace Compilador_AII.Syntax
{
    public enum SyntaxKind
    {
        // --- TOKENS DE CONTROL (0 - 9) ---
        EndOfFileToken = 0,
        WhiteSpaceToken = 1,
        CommentToken = 2,

        // --- VALORES E IDENTIFICADORES (10 - 19) ---
        IntegerToken = 10,
        FloatToken = 11,
        IdentifierToken = 12,

        // --- OPERADORES Y PUNTUACIÓN (20 - 49) ---
        PlusToken = 20,              // +
        MinusToken = 21,             // -
        StarToken = 22,              // *
        SlashToken = 23,             // /
        EqualsToken = 24,            // =
        BangEqualsToken = 25,        // /=
        LessToken = 26,              // <
        LessOrEqualsToken = 27,      // <=
        GreaterToken = 28,           // >
        GreaterOrEqualsToken = 29,   // >=
        ColonToken = 30,             // :
        ColonEqualsToken = 31,       // :=
        DotToken = 32,               // .
        DotDotToken = 33,            // ..
        CommaToken = 34,             // ,
        SemicolonToken = 35,         // ;
        OpenParenthesisToken = 36,   // (
        CloseParenthesisToken = 37,  // )

        // --- PALABRAS RESERVADAS DE SPARK (50 - 99) ---
        ProcedureKeyword = 50,
        IsKeyword = 51,
        BeginKeyword = 52,
        EndKeyword = 53,
        IntegerKeyword = 54,
        FloatKeyword = 55,
        WhileKeyword = 56,
        LoopKeyword = 57,
        IfKeyword = 58,
        ThenKeyword = 59,
        ElseKeyword = 60,
        ExitKeyword = 61,
        WhenKeyword = 62,
        PutKeyword = 63,

        // --- ERRORES ---
        BadToken = 100               // Tokens no reconocidos o ilegales
    }
}