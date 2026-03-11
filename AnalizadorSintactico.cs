using System;
using System.Collections.Generic;

namespace Compilador_AII
{
    public class AnalizadorSintactico
    {
        private List<Token> tokens;
        private int indice = 0;
        private Token actual;

        public AnalizadorSintactico(List<Token> tokens)
        {
            this.tokens = tokens;
            actual = tokens[indice];
        }

        private void Avanzar()
        {
            indice++;

            if (indice < tokens.Count)
                actual = tokens[indice];
        }

        private void Match(string lexema)
        {
            if (actual.Lexema == lexema)
            {
                Avanzar();
            }
            else
            {
                Error($"Se esperaba '{lexema}' y se encontró '{actual.Lexema}'");
            }
        }

        private void MatchTipo(int tipo)
        {
            if (actual.Tipo == tipo)
            {
                Avanzar();
            }
            else
            {
                Error($"Se esperaba tipo {tipo} y se encontró '{actual.Lexema}'");
            }
        }

        private void Error(string mensaje)
        {
            throw new Exception($"Error sintáctico en línea {actual.Linea}: {mensaje}");
        }

        // =========================
        // PROGRAMA
        // =========================

        public void Programa()
        {
            Match("procedure");

            MatchTipo(Tokens.ID);

            Match("is");

            Vars();

            Match("begin");

            ListaSentencias();

            Match("end");

            MatchTipo(Tokens.ID);

            Match(".");
        }

        // =========================
        // VARS
        // VARS -> DECLARACION ; VARS | ε
        // =========================

        private void Vars()
        {
            if (actual.Tipo == Tokens.ID)
            {
                Declaracion();
                Match(";");
                Vars();
            }
        }

        // =========================
        // DECLARACION
        // DECLARACION -> LISTA_ID : TIPO
        // =========================

        private void Declaracion()
        {
            ListaID();
            Match(":");
            Tipo();
        }

        // =========================
        // LISTA_ID
        // LISTA_ID -> id , LISTA_ID | id
        // =========================

        private void ListaID()
        {
            MatchTipo(Tokens.ID);

            if (actual.Lexema == ",")
            {
                Match(",");
                ListaID();
            }
        }

        // =========================
        // TIPO
        // TIPO -> integer | real | string
        // =========================

        private void Tipo()
        {
            if (actual.Lexema == "integer")
                Match("integer");

            else if (actual.Lexema == "real")
                Match("real");

            else if (actual.Lexema == "string")
                Match("string");

            else
                Error("Se esperaba un tipo de dato");
        }

        // =========================
        // LISTA_SENTENCIAS (placeholder)
        // =========================

        private void ListaSentencias()
        {
            // Se implementará después
        }
    }
}