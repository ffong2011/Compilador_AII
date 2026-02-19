using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Compilador_AII.Syntax
{
    // 1. Heredamos de SyntaxNode
    public class SyntaxToken : SyntaxNode
    {
        // 2. Hacemos override a la propiedad Kind exigida por SyntaxNode
        public override SyntaxKind Kind { get; }

        public int Position { get; }
        public string Text { get; }
        public object Value { get; }

        public SyntaxToken(SyntaxKind kind, int position, string text, object value)
        {
            Kind = kind;
            Position = position;
            Text = text;
            Value = value;
        }

        // 3. Implementamos los hijos (Un token no tiene hijos, devolvemos vacío)
        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }
    }
}
