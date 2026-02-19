using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador_AII.Syntax
{
    // Clase base para cualquier expresión (que devuelve un valor)
    public abstract class ExpressionSyntax : SyntaxNode
    {
    }

    // Nodo para un número suelto (ej: 15 o 3.14)
    public sealed class LiteralExpressionSyntax : ExpressionSyntax
    {
        public SyntaxToken LiteralToken { get; }

        public LiteralExpressionSyntax(SyntaxToken literalToken)
        {
            LiteralToken = literalToken;
        }

        public override SyntaxKind Kind => SyntaxKind.IntegerToken; // Simplificado por ahora

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return LiteralToken; // Los tokens son las hojas del árbol
        }
    }

    // Nodo para operaciones de dos lados (ej: a + b, x < y)
    public sealed class BinaryExpressionSyntax : ExpressionSyntax
    {
        public ExpressionSyntax Left { get; }
        public SyntaxToken OperatorToken { get; }
        public ExpressionSyntax Right { get; }

        public BinaryExpressionSyntax(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            Left = left;
            OperatorToken = operatorToken;
            Right = right;
        }

        public override SyntaxKind Kind => SyntaxKind.BadToken; // Lo cambiaremos luego dependiendo del operador

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Left;
            yield return OperatorToken;
            yield return Right;
        }
    }

    // Nodo para expresiones entre paréntesis: ( a + b )
    public sealed class ParenthesizedExpressionSyntax : ExpressionSyntax
    {
        public SyntaxToken OpenParenthesisToken { get; }
        public ExpressionSyntax Expression { get; }
        public SyntaxToken CloseParenthesisToken { get; }

        public ParenthesizedExpressionSyntax(SyntaxToken openParenthesisToken, ExpressionSyntax expression, SyntaxToken closeParenthesisToken)
        {
            OpenParenthesisToken = openParenthesisToken;
            Expression = expression;
            CloseParenthesisToken = closeParenthesisToken;
        }

        public override SyntaxKind Kind => SyntaxKind.BadToken; // Lo actualizaremos luego

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OpenParenthesisToken;
            yield return Expression;
            yield return CloseParenthesisToken;
        }
    }
}