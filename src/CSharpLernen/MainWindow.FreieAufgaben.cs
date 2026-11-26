using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using CSharpLernen.Engine;

namespace CSharpLernen;

/// <summary>
/// Reiter „Freie Aufgaben“: Schwierigkeit wählen, Aufgaben lösen, erledigte bekommen ein grünes Fähnchen.
/// Erledigte Aufgaben lassen sich überspringen oder noch einmal machen – das Fähnchen bleibt.
/// </summary>
public partial class MainWindow
{
    FreieAufgabenKatalog freieKatalog;
    FreieStufe freieStufe;
    Aufgabe freieAufgabe;
    StackPanel freiErgebnisHost;
    Grid freiRaster;
    FrameworkElement freiListe, freiDetail;

    const double FreiZweispaltigAb = 760;

    FreieAufgabenKatalog FreieAufgaben
    {
        get
        {
            if (freieKatalog != null) return freieKatalog;
            try { freieKatalog = FreieAufgabenLader.Laden(); }
            catch (Exception ex)
            {
                Fortschritt.Protokoll(ex, "Freie Aufgaben laden");
                freieKatalog = new FreieAufgabenKatalog();
            }
            return freieKatalog;
        }
    }

    bool IstErledigt(Aufgabe a) => fortschritt.ErledigteFreieAufgaben.Contains(a.Id);

    // ===================================================== Seite

    void SeiteFreieAufgaben()
    {
        var katalog = FreieAufgaben;
        int gesamt = katalog.Alle.Count();
        int erledigt = katalog.Alle.Count(IstErledigt);
        SeitenKopf("Freie Aufgaben",
            $"Üben ohne festen Weg: Wähl eine Schwierigkeit und löse die Aufgaben in deinem Tempo. {erledigt} von {gesamt} hast du schon erledigt.",
            "\uE7C1");

        if (katalog.Stufen.Count == 0)
        {
            SeitenInhalt.Children.Add(Ui.Hinweis("Keine Aufgaben gefunden", "Die freien Aufgaben konnten nicht geladen werden. Details stehen in der Datei fehler.log.", "Gelb", "\uE7BA"));
            return;
        }

        // Zuletzt bearbeitete Aufgabe wieder aufnehmen
        if (freieStufe == null || !katalog.Stufen.Contains(freieStufe))
        {
            var letzte = katalog.Alle.FirstOrDefault(a => a.Id == fortschritt.LetzteFreieAufgabe);
            freieStufe = letzte != null ? katalog.StufeVon(letzte) : katalog.Stufen[0];
            freieAufgabe = letzte ?? FreieAufgabenStatus.Start(freieStufe, fortschritt.ErledigteFreieAufgaben);
        }
        if (freieAufgabe == null || !freieStufe.Aufgaben.Contains(freieAufgabe))
            freieAufgabe = FreieAufgabenStatus.Start(freieStufe, fortschritt.ErledigteFreieAufgaben);

        SeitenInhalt.Children.Add(StufenAuswahl(katalog));

        freiRaster = new Grid();
        freiRaster.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        freiRaster.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        freiRaster.ColumnDefinitions.Add(new ColumnDefinition());
        freiRaster.ColumnDefinitions.Add(new ColumnDefinition());
        freiRaster.ColumnDefinitions.Add(new ColumnDefinition());
        freiListe = AufgabenListe();
        freiDetail = AufgabenDetail();
        freiRaster.Children.Add(freiListe);
        freiRaster.Children.Add(freiDetail);
        freiRaster.SizeChanged += (_, e) => { if (e.WidthChanged) FreiAnordnen(); };
        SeitenInhalt.Children.Add(freiRaster);
        FreiAnordnen();
    }

    /// <summary>Breit: Liste links, Aufgabe rechts. Schmal: untereinander, damit nichts gequetscht wird.</summary>
    void FreiAnordnen()
    {
        if (freiRaster == null) return;
        double b = freiRaster.ActualWidth > 0 ? freiRaster.ActualWidth : SeitenSpalte.ActualWidth - 60;
        bool breit = b >= FreiZweispaltigAb;
        freiRaster.ColumnDefinitions[0].Width = breit ? new GridLength(290) : new GridLength(1, GridUnitType.Star);
        freiRaster.ColumnDefinitions[1].Width = breit ? new GridLength(16) : new GridLength(0);
        freiRaster.ColumnDefinitions[2].Width = breit ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
        Grid.SetRow(freiListe, 0); Grid.SetColumn(freiListe, 0);
        Grid.SetRow(freiDetail, breit ? 0 : 1); Grid.SetColumn(freiDetail, breit ? 2 : 0);
    }

    /// <summary>Drei Karten nebeneinander: Leicht, Mittel, Schwer – mit Fortschritt je Schwierigkeit.</summary>
    UIElement StufenAuswahl(FreieAufgabenKatalog katalog)
    {
        var raster = new UniformGrid { Columns = katalog.Stufen.Count, Margin = new Thickness(0, 0, -10, 6) };
        for (int i = 0; i < katalog.Stufen.Count; i++)
        {
            var s = katalog.Stufen[i];
            int fertig = FreieAufgabenStatus.Erledigt(s, fortschritt.ErledigteFreieAufgaben);
            bool alle = FreieAufgabenStatus.AlleErledigt(s, fortschritt.ErledigteFreieAufgaben);

            var inhalt = new StackPanel { Margin = new Thickness(4, 8, 4, 8) };
            var kopf = new Grid();
            kopf.ColumnDefinitions.Add(new ColumnDefinition());
            kopf.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            kopf.Children.Add(Ui.Text(s.Titel, 15, "TextHell", FontWeights.SemiBold));
            var stufenBalken = SchwierigkeitsBalken(i + 1, katalog.Stufen.Count);
            Grid.SetColumn(stufenBalken, 1);
            kopf.Children.Add(stufenBalken);
            inhalt.Children.Add(kopf);
            inhalt.Children.Add(Ui.Text(s.Beschreibung, 11.5, "TextGedimmt", rand: new Thickness(0, 4, 0, 10)));
            var zeile = new Grid();
            zeile.Children.Add(Ui.Text($"{fertig}/{s.Aufgaben.Count} erledigt", 11, alle ? "Gruen" : "TextMittel", alle ? FontWeights.SemiBold : FontWeights.Normal));
            if (alle)
            {
                var fahne = Ui.Icon("\uE7C1", 12, "Gruen");
                fahne.HorizontalAlignment = HorizontalAlignment.Right;
                zeile.Children.Add(fahne);
            }
            inhalt.Children.Add(zeile);
            inhalt.Children.Add(new ProgressBar { Maximum = Math.Max(1, s.Aufgaben.Count), Value = fertig, Height = 6, Margin = new Thickness(0, 6, 0, 0) });

            var karte = new RadioButton
            {
                Style = (Style)FindResource("PillenKnopf"),
                GroupName = "FreieStufe",
                Content = inhalt,
                IsChecked = s == freieStufe,
                Margin = new Thickness(0, 0, 10, 10),
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                ToolTip = $"{s.Titel}: {s.Aufgaben.Count} Aufgaben",
            };
            var stufe = s;
            karte.Checked += (_, _) =>
            {
                if (stufe == freieStufe) return;
                freieStufe = stufe;
                freieAufgabe = FreieAufgabenStatus.Start(stufe, fortschritt.ErledigteFreieAufgaben);
                BaueSeite("freie");
            };
            raster.Children.Add(karte);
        }
        return raster;
    }

    /// <summary>Kleine Stufen-Anzeige (1 bis 3 gefüllte Balken) statt eines weiteren Textes.</summary>
    static FrameworkElement SchwierigkeitsBalken(int stufe, int von)
    {
        var sp = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 0, 0) };
        for (int i = 1; i <= von; i++)
        {
            var balken = new Border { Width = 4, Height = 6 + i * 3, Margin = new Thickness(2, 0, 0, 0), VerticalAlignment = VerticalAlignment.Bottom };
            balken.SetResourceReference(Border.BackgroundProperty, i <= stufe ? "Akzent" : "Rand");
            sp.Children.Add(balken);
        }
        return sp;
    }

    /// <summary>Liste der Aufgaben der gewählten Schwierigkeit mit Fortschritt und grünen Fähnchen.</summary>
    FrameworkElement AufgabenListe()
    {
        var s = freieStufe;
        int fertig = FreieAufgabenStatus.Erledigt(s, fortschritt.ErledigteFreieAufgaben);
        var sp = new StackPanel();

        var kopf = new Grid { Margin = new Thickness(2, 0, 2, 6) };
        kopf.Children.Add(Ui.Text($"Aufgaben · {s.Titel}", 13.5, "TextHell", FontWeights.SemiBold));
        var zaehler = Ui.Text($"{fertig}/{s.Aufgaben.Count} erledigt", 11.5, fertig == s.Aufgaben.Count ? "Gruen" : "TextGedimmt");
        zaehler.HorizontalAlignment = HorizontalAlignment.Right;
        zaehler.VerticalAlignment = VerticalAlignment.Center;
        kopf.Children.Add(zaehler);
        sp.Children.Add(kopf);
        sp.Children.Add(new ProgressBar { Maximum = Math.Max(1, s.Aufgaben.Count), Value = fertig, Height = 6, Margin = new Thickness(2, 0, 2, 14) });

        for (int i = 0; i < s.Aufgaben.Count; i++)
        {
            var a = s.Aufgaben[i];
            bool erledigt = IstErledigt(a);
            var zeile = new Grid();
            zeile.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
            zeile.ColumnDefinitions.Add(new ColumnDefinition());
            zeile.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            var nr = Ui.Text($"{i + 1:00}", 11.5, erledigt ? "Gruen" : "TextGedimmt", FontWeights.SemiBold);
            nr.FontFamily = Ui.CodeSchrift;
            nr.VerticalAlignment = VerticalAlignment.Center;
            zeile.Children.Add(nr);
            var texte = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
            var titel = Ui.Text(a.Titel, 12.5, "TextHell", a == freieAufgabe ? FontWeights.SemiBold : FontWeights.Normal);
            titel.TextWrapping = TextWrapping.NoWrap;
            titel.TextTrimming = TextTrimming.CharacterEllipsis;
            titel.LineHeight = double.NaN;
            texte.Children.Add(titel);
            var thema = Ui.Text(a.Thema, 10.5, "TextGedimmt");
            thema.LineHeight = double.NaN;
            texte.Children.Add(thema);
            Grid.SetColumn(texte, 1);
            zeile.Children.Add(texte);
            if (erledigt)
            {
                var fahne = ErledigtFahne(klein: true);
                Grid.SetColumn(fahne, 2);
                zeile.Children.Add(fahne);
            }

            var rb = new RadioButton
            {
                Style = (Style)FindResource("PillenKnopf"),
                GroupName = "FreieAufgabe",
                Content = zeile,
                IsChecked = a == freieAufgabe,
                Margin = new Thickness(0, 0, 0, 6),
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                ToolTip = erledigt ? $"{a.Titel} – erledigt" : a.Titel,
            };
            var aufgabe = a;
            rb.Checked += (_, _) => { if (aufgabe != freieAufgabe) FreieAufgabeOeffnen(aufgabe); };
            sp.Children.Add(rb);
        }
        return Ui.Karte(sp, new Thickness(0, 0, 0, 14), new Thickness(12, 14, 12, 8));
    }

    /// <summary>Grünes Fähnchen „Erledigt“ – klein in der Liste, normal im Aufgabenkopf.</summary>
    static Border ErledigtFahne(bool klein = false)
    {
        var sp = new StackPanel { Orientation = Orientation.Horizontal };
        sp.Children.Add(new TextBlock { Text = "\uE7C1", FontFamily = Ui.Icons, FontSize = klein ? 10 : 11.5, Foreground = Ui.B("Gruen"), VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 1, 5, 0) });
        sp.Children.Add(new TextBlock { Text = "Erledigt", FontSize = klein ? 10.5 : 11.5, FontWeight = FontWeights.SemiBold, Foreground = Ui.B("Gruen"), VerticalAlignment = VerticalAlignment.Center });
        var b = new Border
        {
            CornerRadius = new CornerRadius(0),
            BorderBrush = Ui.B("Gruen"),
            BorderThickness = new Thickness(1),
            Padding = klein ? new Thickness(6, 1, 7, 2) : new Thickness(8, 3, 10, 3),
            VerticalAlignment = VerticalAlignment.Center,
            Margin = klein ? new Thickness(8, 0, 0, 0) : new Thickness(0, 3, 0, 0),
            Child = sp,
        };
        b.SetResourceReference(Border.BackgroundProperty, "ErfolgFlaeche");
        return b;
    }

    // ===================================================== Aufgabe

    void FreieAufgabeOeffnen(Aufgabe a)
    {
        freieAufgabe = a;
        BaueSeite("freie");
        // Schmales Fenster: die Aufgabe steht unter der Liste – dorthin scrollen.
        if (freiDetail != null && Grid.GetRow(freiDetail) == 1)
            Dispatcher.BeginInvoke(() => freiDetail?.BringIntoView(), System.Windows.Threading.DispatcherPriority.Loaded);
    }

    FrameworkElement AufgabenDetail()
    {
        var a = freieAufgabe;
        var s = freieStufe;
        var sp = new StackPanel();
        if (a == null)
        {
            sp.Children.Add(Ui.Text("Diese Schwierigkeit hat noch keine Aufgaben.", 12.5, "TextMittel"));
            return Ui.Karte(sp);
        }

        int index = s.Aufgaben.IndexOf(a);
        bool erledigt = IstErledigt(a);
        fortschritt.LetzteFreieAufgabe = a.Id;
        PruefenKnopf.Visibility = Visibility.Visible;
        SetzeEditorKontext(a.Id, a.Startcode, a.Tests.FirstOrDefault()?.Eingabe ?? new List<string>(), $"Freie Aufgabe · {s.Titel} {index + 1}");

        sp.Children.Add(Ui.Text($"{s.Titel}   ›   Aufgabe {index + 1} von {s.Aufgaben.Count}   ›   {a.Thema}", 11, "TextGedimmt", rand: new Thickness(0, 0, 0, 6)));
        var kopf = new WrapPanel();
        kopf.Children.Add(Ui.Text(a.Titel, 19, "TextHell", FontWeights.SemiBold, new Thickness(0, 0, 12, 10)));
        if (erledigt) kopf.Children.Add(ErledigtFahne());
        sp.Children.Add(kopf);

        if (erledigt)
        {
            var box = new StackPanel();
            var zeile = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 4) };
            zeile.Children.Add(Ui.Icon("\uE73E", 14, "Gruen"));
            zeile.Children.Add(new TextBlock { Text = "  Diese Aufgabe hast du schon gelöst", FontWeight = FontWeights.SemiBold, Foreground = Ui.B("Gruen"), FontSize = 12.5 });
            box.Children.Add(zeile);
            box.Children.Add(Ui.Text("Du kannst sie überspringen oder noch einmal von vorn lösen. Das grüne Fähnchen bleibt dir in jedem Fall erhalten.", 12, "TextMittel", rand: new Thickness(0, 0, 0, 10)));
            var knoepfe = new WrapPanel();
            knoepfe.Children.Add(Ui.Knopf("Überspringen", (_, _) => FreieAufgabeUeberspringen(), true, "\uE893"));
            knoepfe.Children.Add(Ui.Knopf("Nochmal machen", (_, _) => FreieAufgabeNochmal(), false, "\uE72C"));
            box.Children.Add(knoepfe);
            var rahmen = new Border
            {
                BorderBrush = Ui.B("Gruen"),
                BorderThickness = new Thickness(3, 0, 0, 0),
                Padding = new Thickness(14, 10, 14, 12),
                Margin = new Thickness(0, 0, 0, 16),
                Child = box,
            };
            rahmen.SetResourceReference(Border.BackgroundProperty, "EingabeHinweis");
            sp.Children.Add(rahmen);
        }

        Ui.Markdown(sp, a.Aufgabentext, null);

        var ersteEingabe = a.Tests.FirstOrDefault()?.Eingabe;
        if (ersteEingabe != null && ersteEingabe.Count > 0)
        {
            string text = $"Diese Aufgabe liest Eingaben ein. Wenn du dein Programm mit F5 ausführst, fragt es dich unten nach den Eingaben, zum Beispiel {string.Join(", ", ersteEingabe.Select(e => e.Length == 0 ? "(leere Zeile)" : e))}. Beim Prüfen testet die App {a.Tests.Count} verschiedene Eingaben.";
            sp.Children.Add(Ui.Hinweis("Eingaben", text, "Akzent", "\uE765"));
        }
        else
        {
            sp.Children.Add(Ui.Text("Schreib deinen Code rechts in den Editor. Mit F5 probierst du ihn aus, mit „Prüfen“ (F6) kontrolliert die App deine Lösung.", 12, "TextGedimmt", rand: new Thickness(0, 0, 0, 12)));
        }

        var leiste = new WrapPanel { Margin = new Thickness(0, 4, 0, 10) };
        var tippHost = new StackPanel();
        var loesungHost = new StackPanel();
        leiste.Children.Add(Abstand(Ui.Knopf("Prüfen (F6)", (_, _) => _ = Pruefen(), !erledigt, "\uE73E")));
        leiste.Children.Add(Abstand(Ui.Knopf("Tipp", (_, _) =>
        {
            tippHost.Children.Clear();
            tippHost.Children.Add(Ui.Hinweis("Tipp", a.Tipp, "Gelb", "\uEA80"));
        }, false, "\uEA80")));
        leiste.Children.Add(Abstand(Ui.Knopf("Lösung zeigen", async (_, _) =>
        {
            if (loesungHost.Children.Count > 0) return;
            if (!IstErledigt(a) && await DialogService.ShowAsync("Lösung ansehen?",
                    "Willst du dir wirklich schon die Lösung ansehen? Versuch es ruhig erst noch ein bisschen, aus Fehlern lernt man am meisten.",
                    DialogKind.Frage, "Lösung zeigen", "Selbst probieren") != DialogAntwort.Primaer) return;
            if (loesungHost.Children.Count > 0) return;
            loesungHost.Children.Add(Ui.Text("Musterlösung", 14.5, "TextHell", FontWeights.SemiBold, new Thickness(0, 6, 0, 4)));
            loesungHost.Children.Add(Ui.Text("Es gibt oft mehrere richtige Lösungen. Deine darf anders aussehen!", 11.5, "TextGedimmt", rand: new Thickness(0, 0, 0, 4)));
            loesungHost.Children.Add(Ui.CodeKarte(a.Loesung, "Lösung", CodeInEditor, "In den Editor übernehmen"));
        }, false, "\uE7B3")));
        if (!erledigt && s.Aufgaben.Count > 1)
            leiste.Children.Add(Abstand(Ui.Knopf("Überspringen", (_, _) => FreieAufgabeUeberspringen(), false, "\uE893")));
        sp.Children.Add(leiste);
        sp.Children.Add(tippHost);
        freiErgebnisHost = new StackPanel();
        sp.Children.Add(freiErgebnisHost);
        sp.Children.Add(loesungHost);
        return Ui.Karte(sp, new Thickness(0, 0, 0, 14), new Thickness(22, 18, 22, 14));
    }

    /// <summary>Knöpfe in einer WrapPanel-Leiste: auch beim Umbruch mit Luft nach unten.</summary>
    static Button Abstand(Button b)
    {
        b.Margin = new Thickness(0, 0, 8, 8);
        return b;
    }

    void FreieAufgabeUeberspringen()
    {
        if (freieStufe == null || freieAufgabe == null) return;
        var naechste = FreieAufgabenStatus.Naechste(freieStufe, freieAufgabe, fortschritt.ErledigteFreieAufgaben);
        if (naechste == null) return;
        FreieAufgabeOeffnen(naechste);
    }

    /// <summary>„Nochmal machen“: Startcode zurück in den Editor (mit Strg+Z rückgängig), das Fähnchen bleibt.</summary>
    void FreieAufgabeNochmal()
    {
        if (freieAufgabe == null) return;
        CodeInEditor(freieAufgabe.Startcode);
        freiErgebnisHost?.Children.Clear();
        KonsoleZuruecksetzen();
        Status("↺ Neuer Versuch – der Editor zeigt wieder den Startcode.", "TextMittel");
        Toast("Neuer Versuch", "Der Startcode steht wieder im Editor. Deine alte Lösung holst du mit Strg+Z zurück.", "\uE72C", "Akzent");
    }

    // ===================================================== Prüfen

    async Task PruefeFreieAufgabe()
    {
        var aufgabe = freieAufgabe;
        var stufe = freieStufe;
        if (laeuft || aufgabe == null || stufe == null) return;
        bool warErledigt = IstErledigt(aufgabe);
        SpeichereEditor();
        string code = Editor.Text;
        int nr = LaufStart();
        TabAusgabe.IsChecked = true;
        Status($"⏳ Prüfe deine Lösung mit {Math.Max(1, aufgabe.Tests.Count)} Test(s) …", "TextMittel");
        LogZeile($"Prüfe freie Aufgabe „{aufgabe.Titel}“ ({aufgabe.Id}) mit {aufgabe.Tests.Count} Test(s)");
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
                fortschritt.ErledigteFreieAufgaben.Add(aufgabe.Id);
                fortschritt.ErfolgreicheLaeufe++;
                Status("✓ Kompilierung erfolgreich · Richtig! Aufgabe erledigt. 🎉", "Gruen");
            }
            else
            {
                Status("✗ Noch nicht ganz – Details siehst du links unter der Aufgabe.", "Gelb");
            }
        }
        LogZeile($"Prüfergebnis: {(r.Bestanden ? "bestanden" : "nicht bestanden")} – {r.Titel}");
        fortschritt.Speichern();

        if (seite == "freie" && aufgabe == freieAufgabe)
        {
            if (r.Bestanden)
            {
                // Seite neu aufbauen (Fähnchen in Liste und Kopf), die Ausgabe der Prüfung bleibt stehen.
                ausgabeBehalten = true;
                try { BaueSeite("freie"); } finally { ausgabeBehalten = false; }
                ZeigeGepruefteAusgabe(r);
                Status("✓ Kompilierung erfolgreich · Richtig! Aufgabe erledigt. 🎉", "Gruen");
            }
            ZeigeFreiesErgebnis(r, aufgabe, stufe, warErledigt);
        }
        if (r.Bestanden && !warErledigt && FreieAufgabenStatus.AlleErledigt(stufe, fortschritt.ErledigteFreieAufgaben))
            Toast($"Alle Aufgaben in „{stufe.Titel}“ erledigt!", "Stark! Jede Aufgabe dieser Schwierigkeit hat jetzt ein grünes Fähnchen.", "\uE7C1", "Gruen");
        PruefeErfolge();
    }

    void ZeigeFreiesErgebnis(Pruefergebnis r, Aufgabe aufgabe, FreieStufe stufe, bool warErledigt)
    {
        if (freiErgebnisHost == null) return;
        freiErgebnisHost.Children.Clear();
        var sp = new StackPanel();
        string titel = r.Bestanden ? (warErledigt ? "Wieder geschafft! 🎉" : "Richtig! Aufgabe erledigt. 🎉") : r.Titel;
        sp.Children.Add(Ui.Text(titel, 16, r.Bestanden ? "Gruen" : "Gelb", FontWeights.Bold, new Thickness(0, 0, 0, 6)));
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
            var naechste = FreieAufgabenStatus.Naechste(stufe, aufgabe, fortschritt.ErledigteFreieAufgaben);
            if (naechste != null && !IstErledigt(naechste))
                leiste.Children.Add(Abstand(Ui.Knopf("Nächste offene Aufgabe", (_, _) => FreieAufgabeOeffnen(naechste), true, "\uE76C")));
            else if (FreieAufgabenStatus.AlleErledigt(stufe, fortschritt.ErledigteFreieAufgaben))
            {
                sp.Children.Add(Ui.Text($"Alle {stufe.Aufgaben.Count} Aufgaben in „{stufe.Titel}“ sind erledigt.", 12.5, "Gruen", rand: new Thickness(0, 10, 0, 0)));
                var schwerer = FreieAufgaben.Stufen.SkipWhile(x => x != stufe).Skip(1).FirstOrDefault(x => !FreieAufgabenStatus.AlleErledigt(x, fortschritt.ErledigteFreieAufgaben));
                if (schwerer != null)
                    leiste.Children.Add(Abstand(Ui.Knopf($"Weiter mit „{schwerer.Titel}“", (_, _) =>
                    {
                        freieStufe = schwerer;
                        freieAufgabe = FreieAufgabenStatus.Start(schwerer, fortschritt.ErledigteFreieAufgaben);
                        BaueSeite("freie");
                    }, true, "\uE76C")));
            }
        }
        else if (r.Lauf?.Ausgang == Ausgang.Kompilierfehler)
        {
            leiste.Children.Add(Abstand(Ui.Knopf("Fehler anzeigen", (_, _) => TabFehler.IsChecked = true, false, "\uEA39")));
        }
        if (leiste.Children.Count > 0) sp.Children.Add(leiste);

        var rahmen = new Border
        {
            Background = r.Bestanden ? Ui.B("EingabeHinweis") : Ui.Farbe("#221D0C"),
            BorderBrush = r.Bestanden ? Ui.B("Gruen") : Ui.Farbe("#7A6326"),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(0),
            Padding = new Thickness(16, 12, 16, 14),
            Margin = new Thickness(0, 4, 0, 14),
            Child = sp,
        };
        freiErgebnisHost.Children.Add(rahmen);
        Dispatcher.BeginInvoke(() => rahmen.BringIntoView(), System.Windows.Threading.DispatcherPriority.Loaded);
    }
}
