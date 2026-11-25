using System.Windows.Controls;

namespace CSharpLernen;

public sealed class NavReiter : RadioButton
{
    public event EventHandler Geschlossen;

    protected override void OnToggle()
    {
        if (IsChecked != true)
        {
            base.OnToggle();
            return;
        }

        SetCurrentValue(IsCheckedProperty, false);
        Geschlossen?.Invoke(this, EventArgs.Empty);
    }
}
