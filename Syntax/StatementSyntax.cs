using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador_AII.Syntax
{
    // Clase base para las sentencias
    public abstract class StatementSyntax : SyntaxNode { }

    // Regla 1: PROGRAMA
    public sealed class ProgramSyntax : SyntaxNode
    {
        public SyntaxToken ProcedureKeyword { get; }
        public SyntaxToken StartId { get; }
        public SyntaxToken IsKeyword { get; }
        public List<DeclarationSyntax> Declarations { get; }
        public SyntaxToken BeginKeyword { get; }
        public List<StatementSyntax> Statements { get; }
        public SyntaxToken EndKeyword { get; }
        public SyntaxToken EndId { get; }
        public SyntaxToken DotToken { get; }

        public ProgramSyntax(SyntaxToken procedureKeyword, SyntaxToken startId, SyntaxToken isKeyword, List<DeclarationSyntax> declarations, SyntaxToken beginKeyword, List<StatementSyntax> statements, SyntaxToken endKeyword, SyntaxToken endId, SyntaxToken dotToken)
        {
            ProcedureKeyword = procedureKeyword; StartId = startId; IsKeyword = isKeyword;
            Declarations = declarations; BeginKeyword = beginKeyword; Statements = statements;
            EndKeyword = endKeyword; EndId = endId; DotToken = dotToken;
        }

        public override SyntaxKind Kind => SyntaxKind.ProcedureKeyword;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return ProcedureKeyword; yield return StartId; yield return IsKeyword;
            foreach (var decl in Declarations) yield return decl;
            yield return BeginKeyword;
            foreach (var stmt in Statements) yield return stmt;
            yield return EndKeyword; yield return EndId; yield return DotToken;
        }
    }

    // Regla 3 y 4: DECLARACION (ej: var1, var2 : Integer)
    public sealed class DeclarationSyntax : SyntaxNode
    {
        public List<SyntaxToken> Identifiers { get; }
        public SyntaxToken ColonToken { get; }
        public SyntaxToken TypeKeyword { get; }

        public DeclarationSyntax(List<SyntaxToken> identifiers, SyntaxToken colonToken, SyntaxToken typeKeyword)
        {
            Identifiers = identifiers; ColonToken = colonToken; TypeKeyword = typeKeyword;
        }
        public override SyntaxKind Kind => SyntaxKind.IsKeyword;
        public override IEnumerable<SyntaxNode> GetChildren()
        {
            foreach (var id in Identifiers) yield return id;
            yield return ColonToken; yield return TypeKeyword;
        }
    }

    // Regla 9: ASIGNACION
    public sealed class AssignmentSyntax : StatementSyntax
    {
        public SyntaxToken Identifier { get; }
        public SyntaxToken AssignmentToken { get; } // :=
        public ExpressionSyntax Expression { get; }

        public AssignmentSyntax(SyntaxToken identifier, SyntaxToken assignmentToken, ExpressionSyntax expression)
        {
            Identifier = identifier; AssignmentToken = assignmentToken; Expression = expression;
        }
        public override SyntaxKind Kind => SyntaxKind.ColonEqualsToken;
        public override IEnumerable<SyntaxNode> GetChildren() { yield return Identifier; yield return AssignmentToken; yield return Expression; }
    }

    // Regla 10: CONDICIONAL (If)
    public sealed class IfSyntax : StatementSyntax
    {
        public SyntaxToken IfKeyword { get; }
        public ExpressionSyntax Condition { get; }
        public SyntaxToken ThenKeyword { get; }
        public List<StatementSyntax> Statements { get; }
        public SyntaxToken ElseKeyword { get; } // Puede ser null
        public List<StatementSyntax> ElseStatements { get; } // Puede estar vacío
        public SyntaxToken EndKeyword { get; }
        public SyntaxToken IfEndKeyword { get; }

        public IfSyntax(SyntaxToken ifKeyword, ExpressionSyntax condition, SyntaxToken thenKeyword, List<StatementSyntax> statements, SyntaxToken elseKeyword, List<StatementSyntax> elseStatements, SyntaxToken endKeyword, SyntaxToken ifEndKeyword)
        {
            IfKeyword = ifKeyword; Condition = condition; ThenKeyword = thenKeyword; Statements = statements;
            ElseKeyword = elseKeyword; ElseStatements = elseStatements; EndKeyword = endKeyword; IfEndKeyword = ifEndKeyword;
        }
        public override SyntaxKind Kind => SyntaxKind.IfKeyword;
        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return IfKeyword; yield return Condition; yield return ThenKeyword;
            foreach (var stmt in Statements) yield return stmt;
            if (ElseKeyword != null) { yield return ElseKeyword; foreach (var stmt in ElseStatements) yield return stmt; }
            yield return EndKeyword; yield return IfEndKeyword;
        }
    }

    // Regla 12: CICLO (While)
    public sealed class WhileSyntax : StatementSyntax
    {
        public SyntaxToken WhileKeyword { get; }
        public ExpressionSyntax Condition { get; }
        public SyntaxToken LoopKeyword { get; }
        public List<StatementSyntax> Statements { get; }
        public SyntaxToken EndKeyword { get; }
        public SyntaxToken LoopEndKeyword { get; }

        public WhileSyntax(SyntaxToken whileKeyword, ExpressionSyntax condition, SyntaxToken loopKeyword, List<StatementSyntax> statements, SyntaxToken endKeyword, SyntaxToken loopEndKeyword)
        {
            WhileKeyword = whileKeyword; Condition = condition; LoopKeyword = loopKeyword;
            Statements = statements; EndKeyword = endKeyword; LoopEndKeyword = loopEndKeyword;
        }
        public override SyntaxKind Kind => SyntaxKind.WhileKeyword;
        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return WhileKeyword; yield return Condition; yield return LoopKeyword;
            foreach (var stmt in Statements) yield return stmt;
            yield return EndKeyword; yield return LoopEndKeyword;
        }
    }

    // Regla 13: EXIT
    public sealed class ExitSyntax : StatementSyntax
    {
        public SyntaxToken ExitKeyword { get; }
        public SyntaxToken WhenKeyword { get; }
        public ExpressionSyntax Condition { get; }

        public ExitSyntax(SyntaxToken exitKeyword, SyntaxToken whenKeyword, ExpressionSyntax condition)
        {
            ExitKeyword = exitKeyword; WhenKeyword = whenKeyword; Condition = condition;
        }
        public override SyntaxKind Kind => SyntaxKind.ExitKeyword;
        public override IEnumerable<SyntaxNode> GetChildren() { yield return ExitKeyword; yield return WhenKeyword; yield return Condition; }
    }

    // Regla 14: SALIDA (Put)
    public sealed class PutSyntax : StatementSyntax
    {
        public SyntaxToken PutKeyword { get; }
        public SyntaxToken OpenParen { get; }
        public ExpressionSyntax Expression { get; }
        public SyntaxToken CloseParen { get; }

        public PutSyntax(SyntaxToken putKeyword, SyntaxToken openParen, ExpressionSyntax expression, SyntaxToken closeParen)
        {
            PutKeyword = putKeyword; OpenParen = openParen; Expression = expression; CloseParen = closeParen;
        }
        public override SyntaxKind Kind => SyntaxKind.PutKeyword;
        public override IEnumerable<SyntaxNode> GetChildren() { yield return PutKeyword; yield return OpenParen; yield return Expression; yield return CloseParen; }
    }
}