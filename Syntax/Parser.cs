using System.Collections.Generic;

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

            // ========================================================
            // CAMBIO 1: El Lexer escupe los tokens y aquí filtramos
            // los espacios, comentarios y la BASURA (BadToken)
            // ========================================================
            do
            {
                token = lexer.NextToken();

                if (token.Kind != SyntaxKind.WhiteSpaceToken &&
                    token.Kind != SyntaxKind.CommentToken &&
                    token.Kind != SyntaxKind.BadToken) // Ignoramos la basura léxica para no romper el árbol
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

        private SyntaxToken NextToken()
        {
            var current = Current;
            _position++;
            return current;
        }

        private SyntaxToken MatchToken(SyntaxKind kind)
        {
            if (Current.Kind == kind)
                return NextToken();

            Diagnostics.Add($"ERROR SINTÁCTICO: Se esperaba <{kind}> pero se encontró <{Current.Kind}> en la posición '{Current.Position}'.");
            return new SyntaxToken(kind, Current.Position, null, null);
        }

        // Regla 1: PROGRAMA
        public ProgramSyntax ParseProgram()
        {
            var procedureKw = MatchToken(SyntaxKind.ProcedureKeyword);
            var startId = MatchToken(SyntaxKind.IdentifierToken);
            var isKw = MatchToken(SyntaxKind.IsKeyword);

            var declarations = ParseDeclarations();

            var beginKw = MatchToken(SyntaxKind.BeginKeyword);
            var statements = ParseStatements();

            var endKw = MatchToken(SyntaxKind.EndKeyword);
            var endId = MatchToken(SyntaxKind.IdentifierToken);
            var dotToken = MatchToken(SyntaxKind.DotToken);

            return new ProgramSyntax(procedureKw, startId, isKw, declarations, beginKw, statements, endKw, endId, dotToken);
        }

        // Reglas 3, 4 y 5: DECLARACION
        private List<DeclarationSyntax> ParseDeclarations()
        {
            var declarations = new List<DeclarationSyntax>();

            while (Current.Kind == SyntaxKind.IdentifierToken)
            {
                var identifiers = new List<SyntaxToken>();
                identifiers.Add(MatchToken(SyntaxKind.IdentifierToken));

                while (Current.Kind == SyntaxKind.CommaToken)
                {
                    NextToken(); // Consumimos la coma
                    identifiers.Add(MatchToken(SyntaxKind.IdentifierToken));
                }

                var colon = MatchToken(SyntaxKind.ColonToken);

                SyntaxToken typeKw;
                if (Current.Kind == SyntaxKind.IntegerKeyword || Current.Kind == SyntaxKind.FloatKeyword)
                    typeKw = NextToken();
                else
                    typeKw = MatchToken(SyntaxKind.IntegerKeyword);

                declarations.Add(new DeclarationSyntax(identifiers, colon, typeKw));
                MatchToken(SyntaxKind.SemicolonToken);
            }
            return declarations;
        }

        // Regla 7: LISTA_SENTENCIAS
        private List<StatementSyntax> ParseStatements()
        {
            var statements = new List<StatementSyntax>();

            while (Current.Kind != SyntaxKind.EndKeyword &&
                   Current.Kind != SyntaxKind.ElseKeyword &&
                   Current.Kind != SyntaxKind.EndOfFileToken)
            {
                // ========================================================
                // CAMBIO 2: Sistema Antibloqueo (Panic Mode)
                // ========================================================
                var startToken = Current;

                var statement = ParseStatement();
                statements.Add(statement);
                MatchToken(SyntaxKind.SemicolonToken);

                // Si el parser no consumió ningún token (se trabó con un error), lo forzamos a avanzar
                if (Current == startToken)
                {
                    NextToken();
                }
            }

            return statements;
        }

        // Regla 8 a 14: Enrutador de Sentencias
        private StatementSyntax ParseStatement()
        {
            switch (Current.Kind)
            {
                case SyntaxKind.IfKeyword: return ParseIfStatement();
                case SyntaxKind.WhileKeyword: return ParseWhileStatement();
                case SyntaxKind.ExitKeyword: return ParseExitStatement();
                case SyntaxKind.PutKeyword: return ParsePutStatement();
                case SyntaxKind.IdentifierToken:
                default:
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

        // EXPRESIONES MATEMÁTICAS (Reglas 15 a 21)
        public ExpressionSyntax Parse()
        {
            return ParseExpression();
        }

        private ExpressionSyntax ParseExpression()
        {
            var left = ParseSimpleExpression();

            while (Current.Kind == SyntaxKind.EqualsToken ||
                   Current.Kind == SyntaxKind.BangEqualsToken ||
                   Current.Kind == SyntaxKind.LessToken ||
                   Current.Kind == SyntaxKind.LessOrEqualsToken ||
                   Current.Kind == SyntaxKind.GreaterToken ||
                   Current.Kind == SyntaxKind.GreaterOrEqualsToken)
            {
                var operatorToken = NextToken();
                var right = ParseSimpleExpression();
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }

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

        private ExpressionSyntax ParseFactor()
        {
            if (Current.Kind == SyntaxKind.OpenParenthesisToken)
            {
                var left = NextToken();
                var expression = ParseExpression();
                var right = MatchToken(SyntaxKind.CloseParenthesisToken);
                return new ParenthesizedExpressionSyntax(left, expression, right);
            }

            if (Current.Kind == SyntaxKind.IntegerToken ||
                Current.Kind == SyntaxKind.FloatToken ||
                Current.Kind == SyntaxKind.IdentifierToken)
            {
                var literalToken = NextToken();
                return new LiteralExpressionSyntax(literalToken);
            }

            Diagnostics.Add($"ERROR SINTÁCTICO: Se esperaba un número, identificador o '(' pero se encontró <{Current.Kind}> en la posición '{Current.Position}'.");
            return new LiteralExpressionSyntax(new SyntaxToken(SyntaxKind.BadToken, Current.Position, null, null));
        }
    }
}