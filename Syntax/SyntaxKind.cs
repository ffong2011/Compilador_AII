namespace Compilador_AII.Syntax
{
    public enum SyntaxKind
    {
        // --- CONTROL (0 - 9) ---
        EndOfFileToken = 0,
        WhiteSpaceToken = 1,
        CommentToken = 2,

        // --- VALORES E IDENTIFICADORES (10 - 19) ---
        IntegerToken = 10,
        FloatToken = 11,
        IdentifierToken = 12,

        // --- OPERADORES DE UN CARACTER (20 - 24, 34 - 37) ---
        PlusToken = 20,              // +
        MinusToken = 21,             // -
        StarToken = 22,              // *
        SlashToken = 23,             // /
        EqualsToken = 24,            // =
        LessToken = 26,              // <
        GreaterToken = 28,           // >
        ColonToken = 30,             // :
        DotToken = 32,               // .
        CommaToken = 34,             // ,
        SemicolonToken = 35,         // ;
        OpenParenthesisToken = 36,   // (
        CloseParenthesisToken = 37,  // )

        // --- OPERADORES COMPUESTOS ---
        BangEqualsToken = 25,        // /=
        LessOrEqualsToken = 27,      // <=
        GreaterOrEqualsToken = 29,   // >=
        ColonEqualsToken = 31,       // :=
        DotDotToken = 33,            // ..

        // --- PALABRAS RESERVADAS SPARK (50 - 65) ---
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
        GetKeyword = 64,
        StringKeyword = 65,

        // --- ERRORES LÉXICOS (101 - 103) ---
        Error_CaracterInvalido = 101,      // Ej. @, #
        Error_IdentificadorInvalido = 102, // Ej. variable_ (Termina en guion)
        Error_FlotanteIncompleto = 103     // Ej. 15. (Termina en punto)
    }
}