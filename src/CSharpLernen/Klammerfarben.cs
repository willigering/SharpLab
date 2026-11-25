using System.Windows.Media;
using System.Windows.Threading;
using CSharpLernen.Engine;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace CSharpLernen;

sealed class Klammerfarben : DocumentColorizingTransformer
{
    static readonly Brush[] Farben = { Ui.Farbe("#FFD580"), Ui.Farbe("#AFC7FF"), Ui.Farbe("#EFA9F0"), Ui.Farbe("#93E1CB") };
    Dictionary<int, int> ebenen = new();
    ITextSourceVersion version;

    public Klammerfarben(TextEditor editor)
    {
        var teilsicht = editor.TextArea.TextView;
        teilsicht.LineTransformers.Add(this);
        var timer = new DispatcherTimer(DispatcherPriority.Background) { Interval = TimeSpan.FromMilliseconds(120) };
        CancellationTokenSource anfrage = null;

        void Planen()
        {
            anfrage?.Cancel();
            timer.Stop();
            if (editor.IsVisible) timer.Start();
        }

        timer.Tick += async (_, _) =>
        {
            timer.Stop();
            var aktuell = anfrage = new CancellationTokenSource();
            string code = editor.Text;
            var quellversion = editor.Document.Version;
            try
            {
                var farben = await Task.Run(() => Klammerpaare.Ebenen(code, aktuell.Token), aktuell.Token);
                if (aktuell.IsCancellationRequested) return;
                ebenen = farben;
                version = quellversion;
                teilsicht.Redraw();
            }
            catch (OperationCanceledException) { }
            finally
            {
                if (anfrage == aktuell) anfrage = null;
                aktuell.Dispose();
            }
        };
        editor.TextChanged += (_, _) => Planen();
        editor.IsVisibleChanged += (_, _) => Planen();
        Planen();
    }

    protected override void ColorizeLine(DocumentLine line)
    {
        if (CurrentContext.Document.Version != version) return;
        for (int offset = line.Offset; offset < line.EndOffset; offset++)
        {
            if (!ebenen.TryGetValue(offset, out int ebene)) continue;
            char zeichen = CurrentContext.Document.GetCharAt(offset);
            if (zeichen is not ('(' or ')' or '[' or ']' or '{' or '}')) continue;
            ChangeLinePart(offset, offset + 1, element =>
                element.TextRunProperties.SetForegroundBrush(Farben[ebene % Farben.Length]));
        }
    }
}
