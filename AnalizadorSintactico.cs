using System;
using System.Collections.Generic;

namespace Compilador_All
{
    public class AnalizadorSintactico
    {
        private List<Token> tokens;
        private int indice = 0;
        private Token actual;

        public AnalizadorSintactico(List<Token> tokens)
        {
            this.tokens = tokens;
            actual = (tokens != null && tokens.Count > 0) ? tokens[indice] : new Token(-1, "EOF", 0);
        }

        private void Avanzar()
        {
            indice++;
            actual = (indice < tokens.Count) ? tokens[indice] : new Token(-1, "EOF", actual.Linea);
        }

        private void MatchTipo(int tipo)
        {
            if (actual.Tipo == tipo) Avanzar();
            else throw new Exception($"Error sintáctico en línea {actual.Linea}: Se esperaba el token {tipo} pero se encontró '{actual.Lexema}'");
        }

        public void Programa()
        {
            MatchTipo(TipoToken.PROCEDURE);
            MatchTipo(TipoToken.ID);
            MatchTipo(TipoToken.IS);
            Vars();
            MatchTipo(TipoToken.BEGIN);
            ListaSentencias();
            MatchTipo(TipoToken.END);
            MatchTipo(TipoToken.ID);
            MatchTipo(TipoToken.PUNTO); // Aquí es donde detectará si falta el punto
        }

        private void Vars()
        {
            if (actual.Tipo == TipoToken.ID)
            {
                Declaracion();
                MatchTipo(TipoToken.PUNTO_Y_COMA);
                Vars();
            }
        }

        private void Declaracion()
        {
            ListaID();
            MatchTipo(TipoToken.DOS_PUNTOS);
            if (actual.Tipo == TipoToken.INTEGER) MatchTipo(TipoToken.INTEGER);
            else MatchTipo(TipoToken.FLOAT);
        }

        private void ListaID()
        {
            MatchTipo(TipoToken.ID);
            if (actual.Tipo == TipoToken.COMA)
            {
                MatchTipo(TipoToken.COMA);
                ListaID();
            }
        }

        private void ListaSentencias()
        {
            // Mientras el token actual sea algo que inicia una sentencia
            if (actual.Tipo == TipoToken.ID || actual.Tipo == TipoToken.IF ||
                actual.Tipo == TipoToken.WHILE || actual.Tipo == TipoToken.PUT ||
                actual.Tipo == TipoToken.EXIT)
            {
                Sentencia();
                MatchTipo(TipoToken.PUNTO_Y_COMA);
                ListaSentencias();
            }
        }

        private void Sentencia()
        {
            switch (actual.Tipo)
            {
                case TipoToken.ID: Asignacion(); break;
                case TipoToken.IF: Condicional(); break;
                case TipoToken.WHILE: Bucle(); break;
                case TipoToken.PUT: Escritura(); break;
                case TipoToken.EXIT: Salida(); break;
            }
        }

        private void Asignacion()
        {
            MatchTipo(TipoToken.ID);
            MatchTipo(TipoToken.ASIGNACION);
            Expresion();
        }

        private void Condicional()
        {
            MatchTipo(TipoToken.IF);
            Expresion();
            MatchTipo(TipoToken.THEN);
            ListaSentencias();
            if (actual.Tipo == TipoToken.ELSE)
            {
                MatchTipo(TipoToken.ELSE);
                ListaSentencias();
            }
            MatchTipo(TipoToken.END);
            MatchTipo(TipoToken.IF);
        }

        private void Bucle()
        {
            MatchTipo(TipoToken.WHILE);
            Expresion();
            MatchTipo(TipoToken.LOOP);
            ListaSentencias();
            MatchTipo(TipoToken.END);
            MatchTipo(TipoToken.LOOP);
        }

        private void Escritura()
        {
            MatchTipo(TipoToken.PUT);
            MatchTipo(TipoToken.PAR_ABRE);
            Expresion();
            MatchTipo(TipoToken.PAR_CIERRA);
        }

        private void Salida()
        {
            MatchTipo(TipoToken.EXIT);
            MatchTipo(TipoToken.WHEN);
            Expresion();
        }

        // ==========================================
        // EXPRESIONES MATEMÁTICAS (Reglas 15 a 21)
        // ==========================================

        // REGLA 15: EXPRESION -> EXP_SIMPLE RELACION EXP_SIMPLE | EXP_SIMPLE
        private void Expresion()
        {
            ExpSimple();

            // Verificamos si hay una RELACION (Regla 16)
            if (actual.Tipo == TipoToken.IGUAL || actual.Tipo == TipoToken.DISTINTO ||
                actual.Tipo == TipoToken.MENOR || actual.Tipo == TipoToken.MAYOR ||
                actual.Tipo == TipoToken.MENOR_IGUAL || actual.Tipo == TipoToken.MAYOR_IGUAL)
            {
                Relacion();
                ExpSimple();
            }
        }

        // REGLA 16: RELACION -> = | /= | < | > | <= | >=
        private void Relacion()
        {
            // Como ya validamos en el if de Expresion() que es un operador relacional, lo consumimos
            MatchTipo(actual.Tipo);
        }

        // REGLA 17: EXP_SIMPLE -> TERMINO RESTO_EXP
        private void ExpSimple()
        {
            Termino();
            RestoExp();
        }

        // REGLA 18: RESTO_EXP -> + TERMINO RESTO_EXP | - TERMINO RESTO_EXP | epsilon
        private void RestoExp()
        {
            if (actual.Tipo == TipoToken.MAS || actual.Tipo == TipoToken.MENOS)
            {
                MatchTipo(actual.Tipo);
                Termino();
                RestoExp(); // Recursividad por la derecha
            }
            // Epsilon: Si no hay + o -, simplemente no hace nada y termina
        }

        // REGLA 19: TERMINO -> FACTOR RESTO_TERM
        private void Termino()
        {
            Factor();
            RestoTerm();
        }

        // REGLA 20: RESTO_TERM -> * FACTOR RESTO_TERM | / FACTOR RESTO_TERM | epsilon
        private void RestoTerm()
        {
            if (actual.Tipo == TipoToken.MULT || actual.Tipo == TipoToken.DIV)
            {
                MatchTipo(actual.Tipo);
                Factor();
                RestoTerm(); // Recursividad por la derecha
            }
            // Epsilon: Si no hay * o /, simplemente no hace nada y termina
        }

        // REGLA 21: FACTOR -> id | num_entero | num_float | ( EXPRESION )
        private void Factor()
        {
            if (actual.Tipo == TipoToken.ID) MatchTipo(TipoToken.ID);
            else if (actual.Tipo == TipoToken.NUM_ENTERO) MatchTipo(TipoToken.NUM_ENTERO);
            else if (actual.Tipo == TipoToken.NUM_FLOAT) MatchTipo(TipoToken.NUM_FLOAT);
            else if (actual.Tipo == TipoToken.PAR_ABRE)
            {
                MatchTipo(TipoToken.PAR_ABRE);
                Expresion();
                MatchTipo(TipoToken.PAR_CIERRA);
            }
            else throw new Exception($"Error en expresión: Se encontró '{actual.Lexema}' en línea {actual.Linea}");
        }
    }
}