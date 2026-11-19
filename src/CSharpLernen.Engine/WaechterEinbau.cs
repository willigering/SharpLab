using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CSharpLernen.Engine;

/// <summary>
/// Baut unsichtbare Prüf-Aufrufe in Schleifen und Methoden ein, damit Endlosschleifen und endlose
/// Rekursion abgebrochen werden können. Es werden KEINE Zeilenumbrüche eingefügt, damit die
/// Zeilennummern in Fehlermeldungen genau zum Code des Nutzers passen.
/// Ersetzt außerdem Console.ReadKey() und Console.Clear(), die ohne echtes Konsolenfenster nicht gehen.
/// </summary>
internal sealed class WaechterEinbau : CSharpSyntaxRewriter
{
    internal const string Quelltext = """
internal static class __Waechter
{
    public static System.Threading.CancellationToken Token;
    public static void Pruefe()
    {
        if (Token.IsCancellationRequested) throw new System.OperationCanceledException(Token);
        System.Runtime.CompilerServices.RuntimeHelpers.EnsureSufficientExecutionStack();
    }
    [System.ThreadStatic] static int Tiefe;
    public const int MaxTiefe = 3000;
    public static void Rein()
    {
        Pruefe();
        if (Tiefe >= MaxTiefe) throw new System.InsufficientExecutionStackException("Zu tiefe Rekursion (mehr als 3000 verschachtelte Methodenaufrufe).");
        Tiefe++;
    }
    public static void Raus() { if (Tiefe > 0) Tiefe--; }
    public static System.ConsoleKeyInfo ReadKey() => ReadKey(false);
    public static System.ConsoleKeyInfo ReadKey(bool intercept)
    {
        Pruefe();
        if (System.Console.In.Peek() >= 0)
        {
            string zeile = System.Console.In.ReadLine() ?? "";
            char c = zeile.Length > 0 ? zeile[0] : '\r';
            System.ConsoleKey key = char.IsLetterOrDigit(c) ? (System.ConsoleKey)char.ToUpperInvariant(c) : System.ConsoleKey.Enter;
            return new System.ConsoleKeyInfo(c, key, false, false, false);
        }
        return new System.ConsoleKeyInfo('\r', System.ConsoleKey.Enter, false, false, false);
    }
    public static void Clear() => System.Console.Out.Write('\f');
    public static System.Text.Encoding OutputEncoding { get; set; } = System.Text.Encoding.UTF8;
    public static System.Text.Encoding InputEncoding { get; set; } = System.Text.Encoding.UTF8;
    public static string Title { get; set; } = "";
    public static void Exit(int code) => throw new __ProgrammEnde();
}
internal sealed class __ProgrammEnde : System.Exception { }
""";

    static StatementSyntax Pruefaufruf() =>
        ParseStatement("global::__Waechter.Pruefe();").WithoutTrivia();

    static SyntaxToken Ohne(SyntaxKind kind) => Token(SyntaxTriviaList.Empty, kind, SyntaxTriviaList.Empty);

    static StatementSyntax MitPruefung(StatementSyntax s)
    {
        if (s is BlockSyntax b)
            return b.WithStatements(b.Statements.Insert(0, Pruefaufruf()));
        return Block(Ohne(SyntaxKind.OpenBraceToken), List(new[] { Pruefaufruf(), s }), Ohne(SyntaxKind.CloseBraceToken));
    }

    /// <summary>{ A; B; }  ->  { Rein(); try { A; B; } finally { Raus(); } }  (ohne neue Zeilen)</summary>
    static BlockSyntax MitTiefe(BlockSyntax b)
    {
        var innen = Block(Ohne(SyntaxKind.OpenBraceToken), b.Statements, Ohne(SyntaxKind.CloseBraceToken));
        var raus = Block(Ohne(SyntaxKind.OpenBraceToken), SingletonList(ParseStatement("global::__Waechter.Raus();").WithoutTrivia()), Ohne(SyntaxKind.CloseBraceToken));
        var versuch = TryStatement(Ohne(SyntaxKind.TryKeyword), innen, default, FinallyClause(Ohne(SyntaxKind.FinallyKeyword), raus));
        var rein = ParseStatement("global::__Waechter.Rein();").WithoutTrivia();
        return b.WithStatements(List(new StatementSyntax[] { rein, versuch }));
    }

    public override SyntaxNode? VisitWhileStatement(WhileStatementSyntax node)
    {
        var n = (WhileStatementSyntax)base.VisitWhileStatement(node)!;
        return n.WithStatement(MitPruefung(n.Statement));
    }

    public override SyntaxNode? VisitDoStatement(DoStatementSyntax node)
    {
        var n = (DoStatementSyntax)base.VisitDoStatement(node)!;
        return n.WithStatement(MitPruefung(n.Statement));
    }

    public override SyntaxNode? VisitForStatement(ForStatementSyntax node)
    {
        var n = (ForStatementSyntax)base.VisitForStatement(node)!;
        return n.WithStatement(MitPruefung(n.Statement));
    }

    public override SyntaxNode? VisitForEachStatement(ForEachStatementSyntax node)
    {
        var n = (ForEachStatementSyntax)base.VisitForEachStatement(node)!;
        return n.WithStatement(MitPruefung(n.Statement));
    }

    public override SyntaxNode? VisitForEachVariableStatement(ForEachVariableStatementSyntax node)
    {
        var n = (ForEachVariableStatementSyntax)base.VisitForEachVariableStatement(node)!;
        return n.WithStatement(MitPruefung(n.Statement));
    }

    public override SyntaxNode? VisitGotoStatement(GotoStatementSyntax node) =>
        // goto kann Schleifen bauen: vor dem Sprung prüfen
        Block(Ohne(SyntaxKind.OpenBraceToken), List(new StatementSyntax[] { Pruefaufruf(), node }), Ohne(SyntaxKind.CloseBraceToken));

    public override SyntaxNode? VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        var n = (MethodDeclarationSyntax)base.VisitMethodDeclaration(node)!;
        if (n.Body != null) return n.WithBody(MitTiefe(n.Body));
        if (n.ExpressionBody != null && !n.Modifiers.Any(SyntaxKind.AsyncKeyword))
        {
            var (body, _) = AusAusdruck(n.ExpressionBody, n.SemicolonToken, IstVoid(n.ReturnType));
            return n.WithExpressionBody(null).WithSemicolonToken(default).WithBody(body);
        }
        return n;
    }

    public override SyntaxNode? VisitLocalFunctionStatement(LocalFunctionStatementSyntax node)
    {
        var n = (LocalFunctionStatementSyntax)base.VisitLocalFunctionStatement(node)!;
        if (n.Body != null) return n.WithBody(MitTiefe(n.Body));
        if (n.ExpressionBody != null && !n.Modifiers.Any(SyntaxKind.AsyncKeyword))
        {
            var (body, _) = AusAusdruck(n.ExpressionBody, n.SemicolonToken, IstVoid(n.ReturnType));
            return n.WithExpressionBody(null).WithSemicolonToken(default).WithBody(body);
        }
        return n;
    }

    public override SyntaxNode? VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
    {
        var n = (ConstructorDeclarationSyntax)base.VisitConstructorDeclaration(node)!;
        return n.Body != null ? n.WithBody(MitTiefe(n.Body)) : n;
    }

    static readonly HashSet<string> ErsetzteKonsolenTeile = new() { "ReadKey", "Clear", "OutputEncoding", "InputEncoding", "Title" };

    public override SyntaxNode? VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
    {
        var n = (MemberAccessExpressionSyntax)base.VisitMemberAccessExpression(node)!;
        string links = n.Expression.ToString().Replace(" ", "");
        string name = n.Name.Identifier.Text;
        if (links is "Console" or "System.Console" or "global::System.Console" && ErsetzteKonsolenTeile.Contains(name))
            return ParseExpression("global::__Waechter." + name).WithTriviaFrom(n);
        if (links is "Environment" or "System.Environment" && name == "Exit")
            return ParseExpression("global::__Waechter.Exit").WithTriviaFrom(n);
        return n;
    }

    static bool IstVoid(TypeSyntax t) => t is PredefinedTypeSyntax p && p.Keyword.IsKind(SyntaxKind.VoidKeyword);

    /// <summary>"=> ausdruck;" wird zu "{ Pruefe(); return ausdruck; }" – ohne neue Zeilen.</summary>
    static (BlockSyntax, SyntaxToken) AusAusdruck(ArrowExpressionClauseSyntax pfeil, SyntaxToken semikolon, bool istVoid)
    {
        var ausdruck = pfeil.Expression;
        StatementSyntax anweisung = istVoid
            ? ExpressionStatement(ausdruck, Ohne(SyntaxKind.SemicolonToken))
            : ReturnStatement(Token(SyntaxTriviaList.Empty, SyntaxKind.ReturnKeyword, TriviaList(Space)), ausdruck, Ohne(SyntaxKind.SemicolonToken));
        var auf = Token(pfeil.ArrowToken.LeadingTrivia, SyntaxKind.OpenBraceToken, pfeil.ArrowToken.TrailingTrivia);
        var zu = Token(semikolon.LeadingTrivia, SyntaxKind.CloseBraceToken, semikolon.TrailingTrivia);
        return (MitTiefe(Block(auf, SingletonList(anweisung), zu)), semikolon);
    }
}
