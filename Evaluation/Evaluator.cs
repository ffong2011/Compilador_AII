using System;
using System.Collections.Generic;
using System.Globalization;
using Compilador_AII.Syntax;

namespace Compilador_AII.Evaluation
{
    public class Evaluator
    {
        private readonly ProgramSyntax _root;

        // ¡Esta es tu Tabla de Símbolos y Memoria RAM al mismo tiempo!
        private readonly Dictionary<string, object> _variables = new Dictionary<string, object>();

        public List<string> Diagnostics { get; } = new List<string>();

        // Un pequeño truco de ingenieros para manejar la instrucción 'exit' dentro de los ciclos
        private class ExitLoopException : Exception { }

        public Evaluator(ProgramSyntax root)
        {
            _root = root;
        }

        public void Evaluate()
        {
            // ==========================================
            // FASE 1: ANÁLISIS SEMÁNTICO (Declaraciones)
            // ==========================================
            foreach (var declaration in _root.Declarations)
            {
                var isFloat = declaration.TypeKeyword.Kind == SyntaxKind.FloatKeyword;

                foreach (var idToken in declaration.Identifiers)
                {
                    var varName = idToken.Text;

                    // Regla semántica: No puedes declarar la misma variable dos veces
                    if (_variables.ContainsKey(varName))
                    {
                        Diagnostics.Add($"ERROR SEMÁNTICO: La variable '{varName}' ya fue declarada previamente.");
                    }
                    else
                    {
                        // La guardamos en memoria con un valor inicial (0 o 0.0)
                        _variables[varName] = isFloat ? 0.0 : 0;
                    }
                }
            }

            // Si falló el análisis semántico, detenemos el compilador
            if (Diagnostics.Count > 0) return;

            // ==========================================
            // FASE 2: EJECUCIÓN (Intérprete)
            // ==========================================
            try
            {
                foreach (var statement in _root.Statements)
                {
                    EvaluateStatement(statement);
                }
            }
            catch (Exception ex) when (ex is not ExitLoopException)
            {
                // Atrapamos errores de ejecución como "División por cero" o variables no declaradas
                Diagnostics.Add($"ERROR DE EJECUCIÓN: {ex.Message}");
            }
        }

        private void EvaluateStatement(StatementSyntax statement)
        {
            if (statement is AssignmentSyntax assign)
            {
                var varName = assign.Identifier.Text;

                // Regla semántica: No puedes asignar a una variable que no existe
                if (!_variables.ContainsKey(varName))
                    throw new Exception($"La variable '{varName}' no ha sido declarada.");

                var value = EvaluateExpression(assign.Expression);
                _variables[varName] = value; // Actualizamos la memoria
            }
            else if (statement is PutSyntax put)
            {
                var value = EvaluateExpression(put.Expression);
                // El famoso "print" para que podamos ver resultados en consola
                Console.WriteLine($"> SALIDA SPARK: {value}");
            }
            else if (statement is WhileSyntax w)
            {
                try
                {
                    // Evaluamos la condición. Si es verdadera, entra al ciclo.
                    while (Convert.ToBoolean(EvaluateExpression(w.Condition)))
                    {
                        foreach (var stmt in w.Statements)
                        {
                            EvaluateStatement(stmt);
                        }
                    }
                }
                catch (ExitLoopException)
                {
                    // Si alguien ejecutó un 'exit when', caemos aquí y el ciclo termina pacíficamente
                }
            }
            else if (statement is IfSyntax i)
            {
                var condition = Convert.ToBoolean(EvaluateExpression(i.Condition));
                if (condition)
                {
                    foreach (var stmt in i.Statements) EvaluateStatement(stmt);
                }
                else if (i.ElseKeyword != null)
                {
                    foreach (var stmt in i.ElseStatements) EvaluateStatement(stmt);
                }
            }
            else if (statement is ExitSyntax e)
            {
                var condition = Convert.ToBoolean(EvaluateExpression(e.Condition));
                if (condition)
                {
                    throw new ExitLoopException(); // Lanzamos la excepción para romper el ciclo while
                }
            }
        }

        private object EvaluateExpression(ExpressionSyntax node)
        {
            if (node is ParenthesizedExpressionSyntax p)
                return EvaluateExpression(p.Expression);

            if (node is LiteralExpressionSyntax l)
            {
                if (l.LiteralToken.Kind == SyntaxKind.IdentifierToken)
                {
                    var name = l.LiteralToken.Text;
                    if (!_variables.ContainsKey(name))
                        throw new Exception($"La variable '{name}' no ha sido declarada.");
                    return _variables[name];
                }
                else if (l.LiteralToken.Kind == SyntaxKind.FloatToken)
                {
                    // InvariantCulture evita que el programa crashee si tu Windows está en español (usa punto en vez de coma)
                    return double.Parse(l.LiteralToken.Text, CultureInfo.InvariantCulture);
                }
                else // IntegerToken
                {
                    return int.Parse(l.LiteralToken.Text);
                }
            }

            if (node is BinaryExpressionSyntax b)
            {
                var left = EvaluateExpression(b.Left);
                var right = EvaluateExpression(b.Right);

                if (b.OperatorToken.Kind == SyntaxKind.EqualsToken) return Equals(left, right);
                if (b.OperatorToken.Kind == SyntaxKind.BangEqualsToken) return !Equals(left, right);

                // Convertimos todo a decimal (double) internamente para simplificar la calculadora matemática
                var lVal = Convert.ToDouble(left);
                var rVal = Convert.ToDouble(right);

                switch (b.OperatorToken.Kind)
                {
                    case SyntaxKind.PlusToken: return lVal + rVal;
                    case SyntaxKind.MinusToken: return lVal - rVal;
                    case SyntaxKind.StarToken: return lVal * rVal;
                    case SyntaxKind.SlashToken:
                        if (rVal == 0) throw new Exception("Intento de división por cero.");
                        return lVal / rVal;
                    case SyntaxKind.LessToken: return lVal < rVal;
                    case SyntaxKind.LessOrEqualsToken: return lVal <= rVal;
                    case SyntaxKind.GreaterToken: return lVal > rVal;
                    case SyntaxKind.GreaterOrEqualsToken: return lVal >= rVal;
                }
            }

            throw new Exception($"No se pudo evaluar el nodo: {node.Kind}");
        }
    }
}