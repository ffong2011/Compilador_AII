using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador_AII.Syntax
{
    public enum SyntaxKind
    {
        BadToken,
        EndOfFileToken,
        WhiteSpaceToken,
        CommentToken,

        // Literales
        IntegerToken,   // [0-9]+
        FloatToken,     // [0-9]+.[0-9]+
        IdentifierToken,// Variables

        // Operadores
        PlusToken, MinusToken, StarToken, SlashToken,
        EqualsToken, BangEqualsToken, LessToken,
        LessOrEqualsToken, GreaterToken, GreaterOrEqualsToken,
        DotToken, DotDotToken, ColonToken, ColonEqualsToken,

        // Delimitadores
        OpenParenthesisToken, CloseParenthesisToken, CommaToken, SemicolonToken,

        // Palabras Reservadas
        ProcedureKeyword, IsKeyword, BeginKeyword, EndKeyword, IfKeyword,
        ThenKeyword, ElseKeyword, WhileKeyword, LoopKeyword, ExitKeyword,
        WhenKeyword, PutKeyword, IntegerKeyword, FloatKeyword
    }
}
