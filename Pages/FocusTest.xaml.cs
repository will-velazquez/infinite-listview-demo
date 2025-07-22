using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace Fantastical.App.Pages;

internal sealed partial class FocusTest : Page
{
    public FocusTest()
    {
        this.InitializeComponent();

        this.PART_ParserBox.Visibility = Visibility.Collapsed;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        this.PART_ParserBox.Visibility = Visibility.Visible;
        this.PART_ParserBox.Focus(FocusState.Programmatic);
    }

    private void RichEditBox_LosingFocus(UIElement sender, LosingFocusEventArgs args)
    {

    }

    private void RichEditBox_LostFocus(object sender, RoutedEventArgs e)
    {
        this.PART_ParserBox.TextDocument.GetText(TextGetOptions.None, out string sentence);

        if (string.IsNullOrWhiteSpace(sentence))
        {
            this.PART_ParserBox.Visibility = Visibility.Collapsed;
        }
    }
}
