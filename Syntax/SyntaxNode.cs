using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador_AII.Syntax
{
    // Esta es la clase base para absolutamente todo en tu árbol
    public abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }

        // Este método nos servirá después para imprimir el árbol en la consola y verlo visualmente
        public abstract IEnumerable<SyntaxNode> GetChildren();
    }
}