using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CSharpLernen.Engine;

public static class Klammerpaare
{
    public static Dictionary<int, int> Ebenen(string code, CancellationToken ct = default)
    {
        var wurzel = CSharpSyntaxTree.ParseText(code, CodeAusfuehrer.ParseOptionen, cancellationToken: ct).GetRoot(ct);
        var ebenen = new Dictionary<int, int>();
        var offen = new Stack<SyntaxKind>();
        foreach (var token in wurzel.DescendantTokens())
        {
            ct.ThrowIfCancellationRequested();
            if (token.IsMissing) continue;
            var schluss = token.Kind() switch
            {
                SyntaxKind.OpenParenToken => SyntaxKind.CloseParenToken,
                SyntaxKind.OpenBracketToken => SyntaxKind.CloseBracketToken,
                SyntaxKind.OpenBraceToken => SyntaxKind.CloseBraceToken,
                _ => SyntaxKind.None,
            };
            if (schluss != SyntaxKind.None)
            {
                ebenen[token.SpanStart] = offen.Count;
                offen.Push(schluss);
            }
            else if (offen.TryPeek(out var erwartet) && token.IsKind(erwartet))
            {
                offen.Pop();
                ebenen[token.SpanStart] = offen.Count;
            }
        }
        return ebenen;
    }
}
