using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador_AII.Syntax
{
    public class Parser
    {
        private readonly SyntaxToken[] _tokens;
        private int _position;
        public List<string> Diagnostics { get; } = new List<string>();

        public Parser(string text)
        {
            var tokens = new List<SyntaxToken>();
            var lexer = new Lexer(text);
            SyntaxToken token;

            // Consumimos todos los tokens del Lexer y los guardamos en una lista
            do
            {
                token = lexer.NextToken();
                // Ignoramos espacios en blanco y comentarios para el árbol sintáctico
                if (token.Kind != SyntaxKind.WhiteSpaceToken && token.Kind != SyntaxKind.CommentToken)
                {
                    tokens.Add(token);
                }
            } while (token.Kind != SyntaxKind.EndOfFileToken);

            _tokens = tokens.ToArray();
            Diagnostics.AddRange(lexer.Diagnostics); // Heredamos los errores léxicos
        }

        private SyntaxToken Peek(int offset)
        {
            var index = _position + offset;
            if (index >= _tokens.Length)
                return _tokens[_tokens.Length - 1]; // Retorna el EndOfFileToken
            return _tokens[index];
        }

        private SyntaxToken Current => Peek(0);

        // Avanza al siguiente token
        private SyntaxToken NextToken()
        {
            var current = Current;
            _position++;
            return current;
        }

        // Si el token actual es el que esperamos (ej. esperamos un '+'), lo consume. Si no, lanza un error.
        private SyntaxToken MatchToken(SyntaxKind kind)
        {
            if (Current.Kind == kind)
                return NextToken();

            Diagnostics.Add($"ERROR SINTÁCTICO: Se esperaba <{kind}> pero se encontró <{Current.Kind}> en la posición '{Current.Position}'.");
            return new SyntaxToken(kind, Current.Position, null, null); // Fabricamos un token falso para no romper el compilador
        }
        // Regla 1: PROGRAMA
        public ProgramSyntax ParseProgram()
        {
            var procedureKw = MatchToken(SyntaxKind.ProcedureKeyword);
            var startId = MatchToken(SyntaxKind.IdentifierToken);
            var isKw = MatchToken(SyntaxKind.IsKeyword);

            // Regla 2: VARS
            var declarations = ParseDeclarations();

            var beginKw = MatchToken(SyntaxKind.BeginKeyword);

            // Regla 7: LISTA_SENTENCIAS
            var statements = ParseStatements();

            var endKw = MatchToken(SyntaxKind.EndKeyword);
            var endId = MatchToken(SyntaxKind.IdentifierToken);
            var dotToken = MatchToken(SyntaxKind.DotToken);

            return new ProgramSyntax(procedureKw, startId, isKw, declarations, beginKw, statements, endKw, endId, dotToken);
        }

        // Reglas 3, 4 y 5: DECLARACION (Maneja variables separadas por comas)
        private List<DeclarationSyntax> ParseDeclarations()
        {
            var declarations = new List<DeclarationSyntax>();

            // Mientras veamos un Identificador, asumimos que es una declaración
            while (Current.Kind == SyntaxKind.IdentifierToken)
            {
                var identifiers = new List<SyntaxToken>();
                identifiers.Add(MatchToken(SyntaxKind.IdentifierToken));

                // Si hay comas, leemos más identificadores (Regla 5)
                while (Current.Kind == SyntaxKind.CommaToken)
                {
                    NextToken(); // Consumimos la coma
                    identifiers.Add(MatchToken(SyntaxKind.IdentifierToken));
                }

                var colon = MatchToken(SyntaxKind.ColonToken);

                // Tipo puede ser Integer o Float
                SyntaxToken typeKw;
                if (Current.Kind == SyntaxKind.IntegerKeyword || Current.Kind == SyntaxKind.FloatKeyword)
                    typeKw = NextToken();
                else
                    typeKw = MatchToken(SyntaxKind.IntegerKeyword); // Forzamos error si no es tipo válido

                declarations.Add(new DeclarationSyntax(identifiers, colon, typeKw));

                MatchToken(SyntaxKind.SemicolonToken); // Toda declaración termina en ;
            }
            return declarations;
        }

        // Regla 7: LISTA_SENTENCIAS
        private List<StatementSyntax> ParseStatements()
        {
            var statements = new List<StatementSyntax>();

            // Mientras no encontremos 'end' o 'else', seguimos leyendo sentencias
            while (Current.Kind != SyntaxKind.EndKeyword &&
                   Current.Kind != SyntaxKind.ElseKeyword &&
                   Current.Kind != SyntaxKind.EndOfFileToken)
            {
                var statement = ParseStatement();
                statements.Add(statement);
                MatchToken(SyntaxKind.SemicolonToken); // Toda sentencia en Spark termina en ;
            }

            return statements;
        }

        // Regla 8 a 14: Enrutador de Sentencias
        private StatementSyntax ParseStatement()
        {
            switch (Current.Kind)
            {
                case SyntaxKind.IfKeyword:
                    return ParseIfStatement();
                case SyntaxKind.WhileKeyword:
                    return ParseWhileStatement();
                case SyntaxKind.ExitKeyword:
                    return ParseExitStatement();
                case SyntaxKind.PutKeyword:
                    return ParsePutStatement();
                case SyntaxKind.IdentifierToken:
                default:
                    // Si empieza con Identificador, por regla 9 es una ASIGNACION
                    return ParseAssignment();
            }
        }

        private AssignmentSyntax ParseAssignment()
        {
            var id = MatchToken(SyntaxKind.IdentifierToken);
            var assign = MatchToken(SyntaxKind.ColonEqualsToken);
            var expr = ParseExpression();
            return new AssignmentSyntax(id, assign, expr);
        }

        private IfSyntax ParseIfStatement()
        {
            var ifKw = MatchToken(SyntaxKind.IfKeyword);
            var condition = ParseExpression();
            var thenKw = MatchToken(SyntaxKind.ThenKeyword);
            var statements = ParseStatements();

            SyntaxToken elseKw = null;
            var elseStatements = new List<StatementSyntax>();

            if (Current.Kind == SyntaxKind.ElseKeyword)
            {
                elseKw = MatchToken(SyntaxKind.ElseKeyword);
                elseStatements = ParseStatements();
            }

            var endKw = MatchToken(SyntaxKind.EndKeyword);
            var ifEndKw = MatchToken(SyntaxKind.IfKeyword);

            return new IfSyntax(ifKw, condition, thenKw, statements, elseKw, elseStatements, endKw, ifEndKw);
        }

        private WhileSyntax ParseWhileStatement()
        {
            var whileKw = MatchToken(SyntaxKind.WhileKeyword);
            var condition = ParseExpression();
            var loopKw = MatchToken(SyntaxKind.LoopKeyword);
            var statements = ParseStatements();
            var endKw = MatchToken(SyntaxKind.EndKeyword);
            var loopEndKw = MatchToken(SyntaxKind.LoopKeyword);

            return new WhileSyntax(whileKw, condition, loopKw, statements, endKw, loopEndKw);
        }

        private ExitSyntax ParseExitStatement()
        {
            var exitKw = MatchToken(SyntaxKind.ExitKeyword);
            var whenKw = MatchToken(SyntaxKind.WhenKeyword);
            var condition = ParseExpression();
            return new ExitSyntax(exitKw, whenKw, condition);
        }

        private PutSyntax ParsePutStatement()
        {
            var putKw = MatchToken(SyntaxKind.PutKeyword);
            var open = MatchToken(SyntaxKind.OpenParenthesisToken);
            var expr = ParseExpression();
            var close = MatchToken(SyntaxKind.CloseParenthesisToken);
            return new PutSyntax(putKw, open, expr, close);
        }

        // Método principal que inicia la construcción del árbol
        public ExpressionSyntax Parse()
        {
            return ParseExpression();
        }

        // Regla 15 y 16: EXPRESION -> EXP_SIMPLE RELACION EXP_SIMPLE
        private ExpressionSyntax ParseExpression()
        {
            var left = ParseSimpleExpression();

            // Verificamos si hay un operador relacional (Regla 16)
            while (Current.Kind == SyntaxKind.EqualsToken ||
                   Current.Kind == SyntaxKind.BangEqualsToken ||
                   Current.Kind == SyntaxKind.LessToken ||
                   Current.Kind == SyntaxKind.LessOrEqualsToken ||
                   Current.Kind == SyntaxKind.GreaterToken ||
                   Current.Kind == SyntaxKind.GreaterOrEqualsToken)
            {
                var operatorToken = NextToken();
                var right = ParseSimpleExpression(); // Parseamos el lado derecho
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }

        // Regla 17 y 18: EXP_SIMPLE -> TERMINO RESTO_EXP (Sumas y Restas)
        private ExpressionSyntax ParseSimpleExpression()
        {
            var left = ParseTerm();

            while (Current.Kind == SyntaxKind.PlusToken || Current.Kind == SyntaxKind.MinusToken)
            {
                var operatorToken = NextToken();
                var right = ParseTerm();
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }

        // Regla 19 y 20: TERMINO -> FACTOR RESTO_TERM (Multiplicación y División)
        private ExpressionSyntax ParseTerm()
        {
            var left = ParseFactor();

            while (Current.Kind == SyntaxKind.StarToken || Current.Kind == SyntaxKind.SlashToken)
            {
                var operatorToken = NextToken();
                var right = ParseFactor();
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }

        // Regla 21: FACTOR -> id | num_entero | num_float | ( EXPRESION )
        private ExpressionSyntax ParseFactor()
        {
            // Caso 1: ( EXPRESION )
            if (Current.Kind == SyntaxKind.OpenParenthesisToken)
            {
                var left = NextToken(); // Consumimos '('
                var expression = ParseExpression(); // Volvemos a empezar desde arriba (Recursividad)
                var right = MatchToken(SyntaxKind.CloseParenthesisToken); // Exigimos el ')'
                return new ParenthesizedExpressionSyntax(left, expression, right);
            }

            // Caso 2: id | num_entero | num_float
            if (Current.Kind == SyntaxKind.IntegerToken ||
                Current.Kind == SyntaxKind.FloatToken ||
                Current.Kind == SyntaxKind.IdentifierToken)
            {
                var literalToken = NextToken();
                return new LiteralExpressionSyntax(literalToken);
            }

            // Si no es nada de eso, es un error sintáctico
            Diagnostics.Add($"ERROR SINTÁCTICO: Se esperaba un número, identificador o '(' pero se encontró <{Current.Kind}> en la posición '{Current.Position}'.");

            // Fabricamos un nodo falso para que el compilador no crashee
            return new LiteralExpressionSyntax(new SyntaxToken(SyntaxKind.BadToken, Current.Position, null, null));
        }

    }
}