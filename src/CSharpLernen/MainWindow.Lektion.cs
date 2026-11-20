using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using CSharpLernen.Engine;

namespace CSharpLernen;

public partial class MainWindow
{
    Lektion aktuelleLektion;
    Aufgabe aktuelleAufgabe;
    string aktuellerReiter = "theorie";
    readonly HashSet<int> offeneStufen = new();
    bool reiterWirdGesetzt;
    StackPanel pruefErgebnisHost;

    // ===================================================== Status von Lektionen

    bool IstGeloest(Aufgabe a) => fortschritt.GeloesteAufgaben.Contains(a.Id);
    /// <summary>Ab 5 gelösten Aufgaben (siehe <see cref="Lektionsstatus"/>) – schaltet die nächste Lektion frei.</summary>
    bool IstAbgeschlossen(Lektion l) => Lektionsstatus.IstAbgeschlossen(l, fortschritt.GeloesteAufgaben);
    /// <summary>Alle Aufgaben gelöst.</summary>
    bool IstKomplett(Lektion l) => Lektionsstatus.IstKomplett(l, fortschritt.GeloesteAufgaben);
    int Geloest(Lektion l) => Lektionsstatus.Geloest(l, fortschritt.GeloesteAufgaben);

    bool IstFreigeschaltet(Lektion l) =>
        Lektionsstatus.IstFreigeschaltet(lektionen, lektionen.IndexOf(l), fortschritt.GeloesteAufgaben, fortschritt.AllesFreigeschaltet);

    string StufenName(int nr) => kurs.Stufen.FirstOrDefault(s => s.Nummer == nr)?.Titel ?? "";

    /// <summary>Öffnet eine Lektion – fragt nach, wenn sie noch gesperrt ist.</summary>
    async void OeffneLektion(Lektion l, string reiter = "theorie", Aufgabe aufgabe = null)
    {
        if (!IstFreigeschaltet(l))
        {
            int i = lektionen.IndexOf(l);
            string vorher = i > 0 ? lektionen[i - 1].Titel : "";
            var antwort = await DialogService.ShowAsync("Lektion gesperrt",
                $"„{l.Titel}“ ist noch gesperrt. Sobald du {Lektionsstatus.Schwelle} Aufgaben von „{vorher}“ gelöst hast, wird sie automatisch freigeschaltet.",
                DialogKind.Gesperrt, "Trotzdem öffnen", "Abbrechen", "Tipp: In den Einstellungen kannst du alle Lektionen freischalten.");
            if (antwort != DialogAntwort.Primaer) return;
        }
        if (seite != "lernpfad") NavLernpfad.IsChecked = true;
        ZeigeLektion(l, reiter, aufgabe);
    }

    void ZeigeLektion(Lektion l, string reiter, Aufgabe aufgabe = null)
    {
        aktuelleLektion = l;
        fortschritt.LetzteLektion = l.Id;
        fortschritt.GeleseneLektionen.Add(l.Id);
        int index = lektionen.IndexOf(l);

        Brotkrumen.Text = $"Lernpfad   ›   Stufe {l.Stufe}: {StufenName(l.Stufe)}   ›   Lektion {index + 1} von {lektionen.Count}";
        LektionTitel.Text = l.Titel;
        StufenAbzeichenText.Text = $"Stufe {l.Stufe}";
        LektionEinleitung.Text = l.Einleitung;
        LektionEinleitung.Visibility = string.IsNullOrWhiteSpace(l.Einleitung) ? Visibility.Collapsed : Visibility.Visible;
        VorherigeKnopf.IsEnabled = index > 0;
        UebungsReiterText();

        offeneStufen.Add(l.Stufe);
        BaueLernpfad();

        reiterWirdGesetzt = true;
        ReiterTheorie.IsChecked = reiter == "theorie";
        ReiterUebung.IsChecked = reiter == "uebung";
        ReiterQuiz.IsChecked = reiter == "quiz";
        reiterWirdGesetzt = false;
        BaueReiter(reiter, aufgabe);
    }

    /// <summary>Rechter Knopf unten: auf der Theorie-Seite „Zu den Übungen“ (primär). Im Übungs- und Quiz-Bereich
    /// „Nächste Lektion“ – unsichtbar bis 5 Aufgaben gelöst sind, dann im Rahmen-Stil, grün erst bei allen 8.</summary>
    void NaechsteKnopfStil()
    {
        if (aktuelleLektion == null) return;
        var l = aktuelleLektion;
        bool letzte = lektionen.IndexOf(l) >= lektionen.Count - 1;
        if (aktuellerReiter == "theorie" && l.Aufgaben.Count > 0)
        {
            NaechsteText.Text = "Zu den Übungen";
            NaechsteKnopf.Style = (Style)FindResource("AkzentKnopf");
            NaechsteKnopf.Visibility = Visibility.Visible;
            NaechsteKnopf.IsEnabled = true;
            NaechsteKnopf.ToolTip = $"{l.Aufgaben.Count} Übungen zu dieser Lektion";
            return;
        }
        NaechsteText.Text = "Nächste Lektion";
        NaechsteKnopf.IsEnabled = !letzte;
        NaechsteKnopf.Visibility = !letzte && Lektionsstatus.NaechsteSichtbar(l, fortschritt.GeloesteAufgaben) ? Visibility.Visible : Visibility.Collapsed;
        NaechsteKnopf.Style = (Style)FindResource(Lektionsstatus.NaechsteHervorgehoben(l, fortschritt.GeloesteAufgaben) ? "AkzentKnopf" : "RandKnopf");
        NaechsteKnopf.ToolTip = IstKomplett(l) ? "Alle Aufgaben gelöst – weiter geht's!" : $"{Geloest(l)} von {l.Aufgaben.Count} Aufgaben gelöst";
    }

    void UebungsReiterText()
    {
        if (aktuelleLektion != null)
            ReiterUebungText.Text = $"Übung ({Geloest(aktuelleLektion)}/{aktuelleLektion.Aufgaben.Count})";
    }

    void LektionReiter_Checked(object sender, RoutedEventArgs e)
    {
        if (!geladen || reiterWirdGesetzt || aktuelleLektion == null) return;
        string r = sender == ReiterUebung ? "uebung" : sender == ReiterQuiz ? "quiz" : "theorie";
        BaueReiter(r, null);
    }

    void BaueReiter(string reiter, Aufgabe aufgabe)
    {
        aktuellerReiter = reiter;
        NaechsteKnopfStil();
        LektionInhalt.Children.Clear();
        LektionScroller.ScrollToTop();
        switch (reiter)
        {
            case "uebung":
                aufgabe ??= (aktuelleAufgabe != null && aktuelleLektion.Aufgaben.Contains(aktuelleAufgabe) ? aktuelleAufgabe : null)
                            ?? aktuelleLektion.Aufgaben.FirstOrDefault(a => !IstGeloest(a))
                            ?? aktuelleLektion.Aufgaben.FirstOrDefault();
                BaueUebung(aufgabe);
                break;
            case "quiz":
                BaueQuiz();
                break;
            default:
                BaueTheorie();
                break;
        }
    }

    void Vorherige_Click(object sender, RoutedEventArgs e)
    {
        int i = lektionen.IndexOf(aktuelleLektion);
        if (i > 0) OeffneLektion(lektionen[i - 1]);
    }

    void Naechste_Click(object sender, RoutedEventArgs e)
    {
        var l = aktuelleLektion;
        if (l == null) return;
        if (aktuellerReiter == "theorie" && l.Aufgaben.Count > 0)
        {
            ReiterUebung.IsChecked = true;   // „Zu den Übungen“
            return;
        }
        int i = lektionen.IndexOf(l);
        if (i < 0 || i >= lektionen.Count - 1 || !Lektionsstatus.NaechsteSichtbar(l, fortschritt.GeloesteAufgaben)) return;
        // Ab 5 gelösten Aufgaben ist die nächste Lektion frei – ohne Nachfrage weiter.
        OeffneLektion(lektionen[i + 1]);
    }

    // ===================================================== Lernpfad (Spalte 2)

    void BaueLernpfad()
    {
        LernpfadListe.Children.Clear();
        int fertigGesamt = lektionen.Count(IstAbgeschlossen);
        LernpfadUntertitel.Text = $"{fertigGesamt} von {lektionen.Count} Lektionen abgeschlossen";

        foreach (var stufe in kurs.Stufen)
        {
            var inStufe = lektionen.Where(l => l.Stufe == stufe.Nummer).ToList();
            if (inStufe.Count == 0) continue;
            bool offen = offeneStufen.Contains(stufe.Nummer);
            int fertig = inStufe.Count(IstAbgeschlossen);

            var kopf = new Grid();
            kopf.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(22) });
            kopf.ColumnDefinitions.Add(new ColumnDefinition());
            kopf.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            kopf.Children.Add(Ui.Icon(offen ? "\uE70D" : "\uE76C", 10, "TextGedimmt"));
            var titel = new TextBlock { Text = $"Stufe {stufe.Nummer}: {stufe.Titel}", FontWeight = FontWeights.SemiBold, FontSize = 11.5, Foreground = Ui.B("TextHell"), TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 0, 6, 0) };
            Grid.SetColumn(titel, 1);
            kopf.Children.Add(titel);
            var zahl = new TextBlock { Text = $"{fertig}/{inStufe.Count}", FontSize = 11, Foreground = fertig == inStufe.Count ? Ui.B("Gruen") : Ui.B("TextGedimmt") };
            Grid.SetColumn(zahl, 2);
            kopf.Children.Add(zahl);

            var kopfKnopf = new Button { Style = (Style)FindResource("LektionKnopf"), Content = kopf, Margin = new Thickness(0, 6, 0, 2) };
            int nr = stufe.Nummer;
            kopfKnopf.Click += (_, _) =>
            {
                if (!offeneStufen.Remove(nr)) offeneStufen.Add(nr);
                BaueLernpfad();
            };
            LernpfadListe.Children.Add(kopfKnopf);

            if (!offen) continue;
            var liste = new StackPanel { Margin = new Thickness(10, 0, 0, 4) };
            foreach (var l in inStufe)
                liste.Children.Add(LektionEintrag(l));
            LernpfadListe.Children.Add(liste);
        }
    }

    UIElement LektionEintrag(Lektion l)
    {
        bool aktuell = l == aktuelleLektion && seite == "lernpfad";
        bool fertig = IstAbgeschlossen(l);
        bool frei = IstFreigeschaltet(l);
        int geloest = Geloest(l);

        UIElement symbol;
        if (fertig && !aktuell)
            symbol = Kreis("\uE73E", "#69B080", "#0D1A11");
        else if (aktuell)
            symbol = Kreis("\uE768", "#46B96C", "#0B180F");
        else if (!frei)
            symbol = Ui.Icon("\uE72E", 13, "TextGedimmt");
        else
            symbol = new Rectangle { Width = 16, Height = 16, Stroke = Ui.B("RandHell"), StrokeThickness = 2, HorizontalAlignment = HorizontalAlignment.Left };

        var raster = new Grid();
        raster.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(28) });
        raster.ColumnDefinitions.Add(new ColumnDefinition());
        raster.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        if (symbol is FrameworkElement fe) fe.VerticalAlignment = VerticalAlignment.Center;
        raster.Children.Add(symbol);
        var titel = new TextBlock
        {
            Text = $"{lektionen.IndexOf(l) + 1}. {l.Titel}",
            TextWrapping = TextWrapping.Wrap,
            FontSize = 11.5,
            Foreground = aktuell ? Ui.B("TextHell") : frei ? Ui.B("TextMittel") : Ui.B("TextGedimmt"),
            FontWeight = aktuell ? FontWeights.SemiBold : FontWeights.Normal,
            VerticalAlignment = VerticalAlignment.Center,
        };
        Grid.SetColumn(titel, 1);
        raster.Children.Add(titel);
        if (!IstKomplett(l) && geloest > 0)
        {
            var teil = new TextBlock { Text = $"{geloest}/{l.Aufgaben.Count}", FontSize = 10, Foreground = Ui.B("TextGedimmt"), VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(6, 0, 0, 0) };
            Grid.SetColumn(teil, 2);
            raster.Children.Add(teil);
        }

        var knopf = new Button
        {
            Style = (Style)FindResource("LektionKnopf"),
            Content = raster,
            Margin = new Thickness(0, 1, 0, 1),
            ToolTip = IstKomplett(l) ? "Alle Aufgaben gelöst" : fertig ? $"Abgeschlossen – {geloest} von {l.Aufgaben.Count} Aufgaben gelöst" : frei ? $"{geloest} von {l.Aufgaben.Count} Aufgaben gelöst" : $"Noch gesperrt – löse zuerst {Lektionsstatus.Schwelle} Aufgaben der vorigen Lektion",
        };
        if (aktuell)
        {
            knopf.Background = Ui.Farbe("#192A1F");
            knopf.BorderBrush = Ui.Farbe("#355841");
        }
        knopf.Click += (_, _) => OeffneLektion(l);
        return knopf;
    }

    static UIElement Kreis(string glyph, string hintergrund, string vordergrund)
    {
        return new Border
        {
            Width = 18, Height = 18, CornerRadius = new CornerRadius(0),
            Background = Ui.Farbe(hintergrund),
            HorizontalAlignment = HorizontalAlignment.Left,
            Child = new TextBlock { Text = glyph, FontFamily = Ui.Icons, FontSize = 8, Foreground = Ui.Farbe(vordergrund), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center },
        };
    }

    // ===================================================== Theorie

    void BaueTheorie()
    {
        var l = aktuelleLektion;
        aktuelleAufgabe = null;
        PruefenKnopf.Visibility = Visibility.Collapsed;
        SetzeEditorKontext("bsp:" + l.Id, l.Beispiel, l.BeispielEingabe, "Beispiel");

        Ui.Markdown(LektionInhalt, l.Erklaerung, CodeInEditor);

        if (!string.IsNullOrWhiteSpace(l.Beispiel))
        {
            LektionInhalt.Children.Add(Ui.Text("Beispiel", 16, "TextHell", FontWeights.SemiBold, new Thickness(0, 10, 0, 4)));
            LektionInhalt.Children.Add(Ui.Text("Das Beispiel steht schon rechts im Editor. Drück F5, um es auszuführen – und ändere ruhig etwas daran!", 12, "TextGedimmt", rand: new Thickness(0, 0, 0, 6)));
            LektionInhalt.Children.Add(Ui.CodeKarte(l.Beispiel, "Beispiel – Program.cs", CodeInEditor, "Wieder in den Editor"));
            if (l.BeispielEingabe.Count > 0)
                LektionInhalt.Children.Add(Ui.Text("Dieses Beispiel liest Eingaben – beim Ausführen tippst du sie unten in der Konsole ein.", 11.5, "TextGedimmt", rand: new Thickness(0, -6, 0, 12)));
        }
        if (!string.IsNullOrWhiteSpace(l.Hinweis))
            LektionInhalt.Children.Add(Ui.Hinweis("Hinweis", l.Hinweis));

        var knopfLeiste = new WrapPanel { Margin = new Thickness(0, 6, 0, 10) };
        knopfLeiste.Children.Add(Ui.Knopf($"Zu den Übungen ({l.Aufgaben.Count})", (_, _) => ReiterUebung.IsChecked = true, true, "\uE943"));
        if (l.Quiz.Count > 0)
            knopfLeiste.Children.Add(Ui.Knopf("Zum Quiz", (_, _) => ReiterQuiz.IsChecked = true, false, "\uE9CE"));
        LektionInhalt.Children.Add(knopfLeiste);
    }

    // ===================================================== Übung

    void BaueUebung(Aufgabe aufgabe)
    {
        var l = aktuelleLektion;
        if (aufgabe == null)
        {
            LektionInhalt.Children.Add(Ui.Text("Diese Lektion hat keine Übungen.", 12.5, "TextMittel"));
            return;
        }
        aktuelleAufgabe = aufgabe;
        PruefenKnopf.Visibility = Visibility.Visible;
        SetzeEditorKontext(aufgabe.Id, aufgabe.Startcode, aufgabe.Tests.FirstOrDefault()?.Eingabe ?? new List<string>(), "Übung " + (l.Aufgaben.IndexOf(aufgabe) + 1));

        // Auswahl der Aufgaben
        var pillen = new WrapPanel { Margin = new Thickness(0, 0, 0, 6) };
        for (int i = 0; i < l.Aufgaben.Count; i++)
        {
            var a = l.Aufgaben[i];
            var rb = new RadioButton
            {
                Style = (Style)FindResource("PillenKnopf"),
                GroupName = "Aufgaben" + l.Id,
                Content = (IstGeloest(a) ? "✓  " : "") + $"Aufgabe {i + 1}",
                IsChecked = a == aufgabe,
            };
            if (IstGeloest(a)) rb.Foreground = Ui.B("Gruen");
            rb.Checked += (_, _) => { if (a != aktuelleAufgabe) { LektionInhalt.Children.Clear(); BaueUebung(a); } };
            pillen.Children.Add(rb);
        }
        LektionInhalt.Children.Add(pillen);

        var kopf = new WrapPanel();
        kopf.Children.Add(Ui.Text($"Aufgabe {l.Aufgaben.IndexOf(aufgabe) + 1}: {aufgabe.Titel}", 17, "TextHell", FontWeights.SemiBold, new Thickness(0, 0, 12, 8)));
        if (IstGeloest(aufgabe))
            kopf.Children.Add(new Border
            {
                CornerRadius = new CornerRadius(0), Background = Ui.Farbe("#18251C"), BorderBrush = Ui.B("Gruen"), BorderThickness = new Thickness(1),
                Padding = new Thickness(8, 2, 8, 2), VerticalAlignment = VerticalAlignment.Top, Margin = new Thickness(0, 3, 0, 0),
                Child = Ui.Text("✓ Gelöst", 11, "Gruen", FontWeights.SemiBold),
            });
        LektionInhalt.Children.Add(kopf);

        Ui.Markdown(LektionInhalt, aufgabe.Aufgabentext, null);

        var ersteEingabe = aufgabe.Tests.FirstOrDefault()?.Eingabe;
        if (ersteEingabe != null && ersteEingabe.Count > 0)
        {
            string text = $"Diese Aufgabe liest Eingaben ein. Wenn du dein Programm mit F5 ausführst, fragt es dich unten nach den Eingaben – probier zum Beispiel {string.Join(", ", ersteEingabe)}. Beim Prüfen testet die App {aufgabe.Tests.Count} verschiedene Eingaben.";
            if (aufgabe.Vergleich == "lenient")
                text += " Eingabeaufforderungen wie „Zahl: “ darfst du gern ausgeben – geprüft werden nur die geforderten Zeilen.";
            LektionInhalt.Children.Add(Ui.Hinweis("Eingaben", text, "Akzent", "\uE765"));
        }
        else
        {
            LektionInhalt.Children.Add(Ui.Text("Schreib deinen Code rechts in den Editor. Mit F5 probierst du ihn aus, mit „Prüfen“ (F6) kontrolliert die App deine Lösung.", 12, "TextGedimmt", rand: new Thickness(0, 0, 0, 12)));
        }

        var leiste = new WrapPanel { Margin = new Thickness(0, 4, 0, 10) };
        bool geloest = IstGeloest(aufgabe);
        leiste.Children.Add(Ui.Knopf("Prüfen (F6)", (_, _) => _ = Pruefen(), !geloest, "\uE73E"));
        var tippHost = new StackPanel();
        var loesungHost = new StackPanel();
        leiste.Children.Add(Ui.Knopf("Tipp", (_, _) =>
        {
            tippHost.Children.Clear();
            string tipp = string.IsNullOrWhiteSpace(aufgabe.Tipp)
                ? "Lies die Aufgabe nochmal genau und vergleiche deine Ausgabe Zeichen für Zeichen mit der erwarteten – auch Leerzeichen und Satzzeichen zählen. Schau dir auch das Beispiel im Reiter „Theorie“ an."
                : aufgabe.Tipp;
            tippHost.Children.Add(Ui.Hinweis("Tipp", tipp, "Gelb", "\uEA80"));
        }, false, "\uEA80"));
        leiste.Children.Add(Ui.Knopf("Lösung zeigen", async (_, _) =>
        {
            if (loesungHost.Children.Count > 0) return;
            if (!IstGeloest(aufgabe) && await DialogService.ShowAsync("Lösung ansehen?",
                    "Willst du dir wirklich schon die Lösung ansehen? Versuch es ruhig erst noch ein bisschen – aus Fehlern lernt man am meisten.",
                    DialogKind.Frage, "Lösung zeigen", "Selbst probieren") != DialogAntwort.Primaer) return;
            if (loesungHost.Children.Count > 0) return;
            loesungHost.Children.Add(Ui.Text("Musterlösung", 14.5, "TextHell", FontWeights.SemiBold, new Thickness(0, 6, 0, 4)));
            loesungHost.Children.Add(Ui.Text("Es gibt oft mehrere richtige Lösungen – deine darf anders aussehen!", 11.5, "TextGedimmt", rand: new Thickness(0, 0, 0, 4)));
            loesungHost.Children.Add(Ui.CodeKarte(aufgabe.Loesung, "Lösung", CodeInEditor, "In den Editor übernehmen"));
        }, false, "\uE7B3"));
        leiste.Children.Add(NaechsteAufgabeKnopf(aufgabe, geloest));
        LektionInhalt.Children.Add(leiste);
        LektionInhalt.Children.Add(tippHost);
        pruefErgebnisHost = new StackPanel();
        LektionInhalt.Children.Add(pruefErgebnisHost);
        LektionInhalt.Children.Add(loesungHost);
    }

    /// <summary>„Nächste Aufgabe“: springt zur folgenden Aufgabe der Lektion; grün, sobald die aktuelle gelöst ist.
    /// Bei der letzten Aufgabe führt er zum Quiz (falls vorhanden), sonst ist er deaktiviert.</summary>
    Button NaechsteAufgabeKnopf(Aufgabe aufgabe, bool geloest)
    {
        var l = aktuelleLektion;
        int i = l.Aufgaben.IndexOf(aufgabe);
        if (i >= 0 && i < l.Aufgaben.Count - 1)
        {
            var naechste = l.Aufgaben[i + 1];
            return Ui.Knopf("Nächste Aufgabe", (_, _) => { LektionInhalt.Children.Clear(); LektionScroller.ScrollToTop(); BaueUebung(naechste); }, geloest, "\uE76C");
        }
        if (l.Quiz.Count > 0)
            return Ui.Knopf("Zum Quiz", (_, _) => ReiterQuiz.IsChecked = true, geloest, "\uE9CE");
        var k = Ui.Knopf("Nächste Aufgabe", (_, _) => { }, false, "\uE76C");
        k.IsEnabled = false;
        k.ToolTip = "Das ist die letzte Aufgabe dieser Lektion.";
        ToolTipService.SetShowOnDisabled(k, true);
        return k;
    }

    async Task Pruefen()
    {
        if (laeuft) return;
        if (aktuelleAufgabe == null || seite != "lernpfad")
        {
            Status("Zum Prüfen öffne eine Übung im Lernpfad (Reiter „Übung“).", "TextGedimmt");
            return;
        }
        if (aktuellerReiter != "uebung") ReiterUebung.IsChecked = true;
        var aufgabe = aktuelleAufgabe;
        SpeichereEditor();
        string code = Editor.Text;
        int nr = LaufStart();
        TabAusgabe.IsChecked = true;
        Status($"⏳ Prüfe deine Lösung mit {Math.Max(1, aufgabe.Tests.Count)} Test(s) …", "TextMittel");
        LogZeile($"Prüfe Aufgabe „{aufgabe.Titel}“ ({aufgabe.Id}) mit {aufgabe.Tests.Count} Test(s)");
        AufgabenPruefer.Zeitlimit = TimeSpan.FromSeconds(Math.Clamp(fortschritt.Zeitlimit, 3, 10));

        Pruefergebnis r;
        try
        {
            r = await AufgabenPruefer.PruefenAsync(aufgabe, code, laufCts.Token);
        }
        finally
        {
            LaufEnde();
        }
        // Während der Prüfung woanders hin gewechselt: Ergebnis nicht in die neue (leere) Konsole schreiben.
        if (nr != laufNr) return;
        fortschritt.Ausfuehrungen++;

        if (r.Lauf != null)
        {
            ZeigeMeldungen(r.Lauf.Meldungen);
            AusgabeBox.Text = r.Lauf.Ausgabe;
        }

        if (r.Lauf?.Ausgang == Ausgang.Kompilierfehler)
        {
            Status("✗ Kompilierung fehlgeschlagen – schau in den Tab „Fehler“.", "Rot");
            TabFehler.IsChecked = true;
            letzterLaufKompilierfehler = true;
        }
        else
        {
            if (letzterLaufKompilierfehler && r.Lauf != null) fortschritt.FehlerBehoben = true;
            if (r.Lauf != null) letzterLaufKompilierfehler = false;
            if (r.Lauf?.Ausgang is Ausgang.Zeitueberschreitung or Ausgang.ZuVielAusgabe) fortschritt.EndlosschleifeGestoppt = true;
            if (r.Bestanden)
            {
                fortschritt.GeloesteAufgaben.Add(aufgabe.Id);
                fortschritt.ErfolgreicheLaeufe++;
                Status("✓ Kompilierung erfolgreich · Richtig! Aufgabe gelöst. 🎉", "Gruen");
            }
            else
            {
                Status("✗ Noch nicht ganz – Details siehst du links unter der Aufgabe.", "Gelb");
            }
        }
        LogZeile($"Prüfergebnis: {(r.Bestanden ? "bestanden" : "nicht bestanden")} – {r.Titel}");
        fortschritt.Speichern();

        if (aufgabe == aktuelleAufgabe && aktuellerReiter == "uebung")
        {
            if (r.Bestanden)
            {
                // Seite neu aufbauen (Häkchen), dann Ergebnis anzeigen
                LektionInhalt.Children.Clear();
                ausgabeBehalten = true;
                try { BaueUebung(aufgabe); } finally { ausgabeBehalten = false; }
            }
            ZeigePruefergebnis(r, aufgabe);
        }
        UebungsReiterText();
        NaechsteKnopfStil();
        BaueLernpfad();
        AktualisiereFortschritt();
        PruefeErfolge();
    }

    void ZeigePruefergebnis(Pruefergebnis r, Aufgabe aufgabe)
    {
        pruefErgebnisHost.Children.Clear();
        var sp = new StackPanel();
        sp.Children.Add(Ui.Text(r.Titel, 16, r.Bestanden ? "Gruen" : "Gelb", FontWeights.Bold, new Thickness(0, 0, 0, 6)));
        if (!string.IsNullOrWhiteSpace(r.Details))
            sp.Children.Add(new TextBlock { Text = r.Details, FontFamily = r.Bestanden ? Ui.UiSchrift : Ui.CodeSchrift, FontSize = 11.5, Foreground = Ui.B("TextHell"), TextWrapping = TextWrapping.Wrap, LineHeight = 18 });

        if (!r.Bestanden && r.ErwarteteAusgabe != null && r.TatsaechlicheAusgabe != null)
        {
            var vergleich = new Grid { Margin = new Thickness(0, 12, 0, 0) };
            vergleich.ColumnDefinitions.Add(new ColumnDefinition());
            vergleich.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(12) });
            vergleich.ColumnDefinitions.Add(new ColumnDefinition());
            vergleich.Children.Add(AusgabeSpalte("Erwartete Ausgabe", r.ErwarteteAusgabe));
            var rechts = AusgabeSpalte("Deine Ausgabe", r.TatsaechlicheAusgabe.Length == 0 ? "(keine Ausgabe)" : r.TatsaechlicheAusgabe);
            Grid.SetColumn(rechts, 2);
            vergleich.Children.Add(rechts);
            sp.Children.Add(vergleich);
        }

        var leiste = new WrapPanel { Margin = new Thickness(0, 12, 0, 0) };
        if (r.Bestanden)
        {
            var l = aktuelleLektion;
            int i = l.Aufgaben.IndexOf(aufgabe);
            var naechsteOffen = l.Aufgaben.Skip(i + 1).FirstOrDefault(a => !IstGeloest(a)) ?? l.Aufgaben.FirstOrDefault(a => !IstGeloest(a));
            if (naechsteOffen != null)
                leiste.Children.Add(Ui.Knopf("Nächste Aufgabe", (_, _) => { LektionInhalt.Children.Clear(); BaueUebung(naechsteOffen); }, true, "\uE76C"));
            else
            {
                if (l.Quiz.Count > 0 && fortschritt.QuizPunkte.GetValueOrDefault(l.Id) < l.Quiz.Count)
                    leiste.Children.Add(Ui.Knopf("Lektion geschafft! Zum Quiz", (_, _) => ReiterQuiz.IsChecked = true, true, "\uE9CE"));
                int li = lektionen.IndexOf(l);
                if (li < lektionen.Count - 1)
                    leiste.Children.Add(Ui.Knopf("Nächste Lektion", (_, _) => OeffneLektion(lektionen[li + 1]), l.Quiz.Count == 0 || fortschritt.QuizPunkte.GetValueOrDefault(l.Id) >= l.Quiz.Count, "\uE76C"));
            }
        }
        else if (r.Lauf?.Ausgang == Ausgang.Kompilierfehler)
        {
            leiste.Children.Add(Ui.Knopf("Fehler anzeigen", (_, _) => TabFehler.IsChecked = true, false, "\uEA39"));
        }
        if (leiste.Children.Count > 0) sp.Children.Add(leiste);

        pruefErgebnisHost.Children.Add(new Border
        {
            Background = Ui.Farbe(r.Bestanden ? "#131F17" : "#221D0C"),
            BorderBrush = r.Bestanden ? Ui.B("Gruen") : Ui.Farbe("#7A6326"),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(0),
            Padding = new Thickness(16, 12, 16, 14),
            Margin = new Thickness(0, 4, 0, 14),
            Child = sp,
        });
        pruefErgebnisHost.BringIntoView();
    }

    static FrameworkElement AusgabeSpalte(string titel, string text)
    {
        var sp = new StackPanel();
        sp.Children.Add(Ui.Text(titel, 11, "TextGedimmt", FontWeights.SemiBold, new Thickness(0, 0, 0, 4)));
        sp.Children.Add(new Border
        {
            Background = Ui.B("CodeHintergrund"), BorderBrush = Ui.B("Rand"), BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(0), Padding = new Thickness(10, 8, 10, 8),
            Child = new TextBlock { Text = text, FontFamily = Ui.CodeSchrift, FontSize = 11, Foreground = Ui.Farbe("#B3CCBB"), TextWrapping = TextWrapping.Wrap },
        });
        return sp;
    }

    // ===================================================== Quiz

    void BaueQuiz()
    {
        var l = aktuelleLektion;
        PruefenKnopf.Visibility = Visibility.Collapsed;
        if (l.Quiz.Count == 0)
        {
            LektionInhalt.Children.Add(Ui.Text("Zu dieser Lektion gibt es kein Quiz.", 12.5, "TextMittel"));
            return;
        }
        int bester = fortschritt.QuizPunkte.GetValueOrDefault(l.Id, -1);
        LektionInhalt.Children.Add(Ui.Text($"Teste dein Wissen: {l.Quiz.Count} Fragen zu „{l.Titel}“.", 13.5, "TextMittel", rand: new Thickness(0, 0, 0, 4)));
        if (bester >= 0)
            LektionInhalt.Children.Add(Ui.Text($"Dein bestes Ergebnis bisher: {bester} von {l.Quiz.Count} richtig.", 11.5, bester == l.Quiz.Count ? "Gruen" : "TextGedimmt", rand: new Thickness(0, 0, 0, 10)));

        var antworten = new List<(Quizfrage Frage, List<RadioButton> Knoepfe, TextBlock Rueckmeldung)>();
        for (int i = 0; i < l.Quiz.Count; i++)
        {
            var q = l.Quiz[i];
            var sp = new StackPanel();
            sp.Children.Add(Ui.Text($"Frage {i + 1} von {l.Quiz.Count}", 11, "Akzent", FontWeights.SemiBold, new Thickness(0, 0, 0, 4)));
            sp.Children.Add(Ui.Text(q.Frage, 13.5, "TextHell", FontWeights.SemiBold, new Thickness(0, 0, 0, 8)));
            var knoepfe = new List<RadioButton>();
            for (int j = 0; j < q.Antworten.Count; j++)
            {
                var rb = new RadioButton
                {
                    Style = (Style)FindResource("AntwortKnopf"),
                    GroupName = $"quiz_{l.Id}_{i}",
                    Content = new TextBlock { Text = q.Antworten[j], TextWrapping = TextWrapping.Wrap, FontFamily = q.Antworten[j].Contains('(') || q.Antworten[j].Contains(';') ? Ui.CodeSchrift : Ui.UiSchrift },
                };
                knoepfe.Add(rb);
                sp.Children.Add(rb);
            }
            var rueck = Ui.Text("", 11.5, "TextMittel", rand: new Thickness(0, 6, 0, 0));
            rueck.Visibility = Visibility.Collapsed;
            sp.Children.Add(rueck);
            antworten.Add((q, knoepfe, rueck));
            LektionInhalt.Children.Add(Ui.Karte(sp));
        }

        var ergebnis = Ui.Text("", 13.5, "TextHell", FontWeights.SemiBold, new Thickness(0, 4, 0, 8));
        var leiste = new WrapPanel { Margin = new Thickness(0, 0, 0, 16) };
        leiste.Children.Add(Ui.Knopf("Antworten prüfen", (_, _) =>
        {
            if (antworten.Any(a => a.Knoepfe.All(k => k.IsChecked != true)))
            {
                ergebnis.Text = "Beantworte bitte zuerst alle Fragen.";
                ergebnis.Foreground = Ui.B("Gelb");
                return;
            }
            int richtig = 0;
            foreach (var (frage, knoepfe, rueck) in antworten)
            {
                int gewaehlt = knoepfe.FindIndex(k => k.IsChecked == true);
                bool ok = gewaehlt == frage.Richtig;
                if (ok) richtig++;
                for (int j = 0; j < knoepfe.Count; j++)
                {
                    knoepfe[j].BorderBrush = j == frage.Richtig ? Ui.B("Gruen") : j == gewaehlt ? Ui.B("Rot") : Ui.B("Rand");
                    knoepfe[j].IsEnabled = false;
                    knoepfe[j].Opacity = j == frage.Richtig || j == gewaehlt ? 1 : 0.6;
                }
                rueck.Text = (ok ? "✓ Richtig! " : $"✗ Leider falsch. Richtig ist: {frage.Antworten[frage.Richtig]}. ") + frage.Erklaerung;
                rueck.Foreground = ok ? Ui.B("Gruen") : Ui.B("Rot");
                rueck.Visibility = Visibility.Visible;
            }
            ergebnis.Text = richtig == antworten.Count
                ? $"Perfekt! Alle {richtig} Fragen richtig. 🎉"
                : $"Du hast {richtig} von {antworten.Count} Fragen richtig. Schau dir die Erklärungen an und versuch es nochmal!";
            ergebnis.Foreground = richtig == antworten.Count ? Ui.B("Gruen") : Ui.B("TextHell");
            if (richtig > fortschritt.QuizPunkte.GetValueOrDefault(l.Id, -1))
                fortschritt.QuizPunkte[l.Id] = richtig;
            fortschritt.Speichern();
            PruefeErfolge();
        }, true, "\uE73E"));
        leiste.Children.Add(Ui.Knopf("Nochmal", (_, _) => BaueReiter("quiz", null), false, "\uE72C"));
        LektionInhalt.Children.Add(ergebnis);
        LektionInhalt.Children.Add(leiste);
    }

    // ===================================================== Fortschrittsring

    void AktualisiereFortschritt()
    {
        int fertig = lektionen.Count(IstAbgeschlossen);
        int aufgabenGesamt = lektionen.Sum(l => l.Aufgaben.Count);
        int aufgabenGeloest = lektionen.Sum(Geloest);
        double anteil = aufgabenGesamt == 0 ? 0 : aufgabenGeloest / (double)aufgabenGesamt;

        FortschrittProzent.Text = $"{anteil * 100:0}%";
        FortschrittText.Text = $"{fertig} von {lektionen.Count} Lektionen";
        FortschrittBalken.Value = lektionen.Count == 0 ? 0 : fertig * 100.0 / lektionen.Count;
        FortschrittsRing.ToolTip = $"{aufgabenGeloest} von {aufgabenGesamt} Aufgaben gelöst";

        const double r = 24, m = 27;   // Ring 54 × 54, Strich 6
        if (anteil <= 0) FortschrittsRing.Data = null;
        else if (anteil >= 0.9999) FortschrittsRing.Data = new EllipseGeometry(new Point(m, m), r, r);
        else
        {
            double winkel = anteil * 2 * Math.PI;
            var ende = new Point(m + r * Math.Sin(winkel), m - r * Math.Cos(winkel));
            var fig = new PathFigure { StartPoint = new Point(m, m - r), IsClosed = false };
            fig.Segments.Add(new ArcSegment(ende, new Size(r, r), 0, anteil > 0.5, SweepDirection.Clockwise, true));
            var geo = new PathGeometry();
            geo.Figures.Add(fig);
            FortschrittsRing.Data = geo;
        }
    }
}
