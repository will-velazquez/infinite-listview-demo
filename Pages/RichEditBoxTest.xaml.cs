using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Text.RegularExpressions;
using Windows.UI;

namespace Fantastical.App.Pages;

internal sealed partial class RichEditBoxTest : Page
{
    private static readonly Regex _urlRegex = new Regex("(http(s)?:\\/\\/.)?(www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{2,256}\\.[a-z]{2,6}([-a-zA-Z0-9@:%_\\+.~#?&//=]*)");
    private bool _isContentChanging;
    private bool _needsLinkify;

    public static DependencyProperty AccentTextFillColorProperty = DependencyProperty.Register(
        nameof(AccentTextFillColor),
        typeof(Color?),
        typeof(RichEditBoxTest),
        new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        {
            ((RichEditBoxTest)d).AccentTextFillColorUpdated();
        }));

    public Color? AccentTextFillColor
    {
        get => (Color?)this.GetValue(AccentTextFillColorProperty);
        set => this.SetValue(AccentTextFillColorProperty, value);
    }

    public RichEditBoxTest()
    {
        this.InitializeComponent();

        this.UpdateStatusText();
    }

    private void AccentTextFillColorUpdated()
    {
        this.UpdateLinkTextColors();
    }

    private void UpdateLinkTextColors()
    {
        RichEditTextDocument richEditTextDocument = this.PART_RichEditBox.Document;
        ITextRange caret = richEditTextDocument.GetRange(0, 0);

        caret.Move(TextRangeUnit.Link, 1);

        while (caret.GetIndex(TextRangeUnit.Link) != -1)
        {
            caret.Expand(TextRangeUnit.Link);

            caret.CharacterFormat.ForegroundColor = this.AccentTextFillColor ?? Colors.Transparent;

            caret.Collapse(false);

            int unitsMoved = caret.Move(TextRangeUnit.Link, 1);

            if (unitsMoved == 0)
            {
                break;
            }
        }
    }

    private void UpdateStatusText()
    {
        RichEditTextDocument richEditTextDocument = this.PART_RichEditBox.TextDocument;
        ITextRange selection = richEditTextDocument.Selection;

        this.PART_Status.Text = $"Position: {selection.StartPosition} / {selection.EndPosition}\n"
            + $"Format: {selection.CharacterFormat.Name} {selection.CharacterFormat.Position} {selection.CharacterFormat.LinkType}\n"
            + $"        {selection.CharacterFormat.ForegroundColor}";

        richEditTextDocument.GetText(TextGetOptions.None, out string bareText);

        this.PART_BareText.Text = bareText;
    }

    private void UnlinkifyText()
    {
        RichEditTextDocument richEditTextDocument = this.PART_RichEditBox.Document;
        ITextRange caret = richEditTextDocument.GetRange(0, 0);

        caret.Move(TextRangeUnit.Link, 1);

        while (caret.GetIndex(TextRangeUnit.Link) != -1)
        {
            caret.Expand(TextRangeUnit.Link);

            if (!string.IsNullOrEmpty(caret.Link))
            {
                caret.Link = null;
            }

            caret.Collapse(false);

            int unitsMoved = caret.Move(TextRangeUnit.Link, 1);

            if (unitsMoved == 0)
            {
                break;
            }
        }
    }

    private void UpdateLinkifiedText()
    {
        RichEditTextDocument richEditTextDocument = this.PART_RichEditBox.TextDocument;
        richEditTextDocument.GetText(TextGetOptions.NoHidden | TextGetOptions.UseLf, out string plainText);

        Match nextMatch = _urlRegex.Match(plainText);

        if (nextMatch.Success)
        {
            richEditTextDocument.BeginUndoGroup();

            richEditTextDocument.SetText(TextSetOptions.None, string.Empty);

            ITextRange caret = richEditTextDocument.GetRange(0, 0);
            int prevMatchEnd = 0;

            while (nextMatch.Success)
            {
                int textBeforeEnd = nextMatch.Index;
                string textBefore = plainText.Substring(prevMatchEnd, textBeforeEnd - prevMatchEnd);

                caret.SetText(TextSetOptions.None, textBefore);
                caret.Collapse(false);

                Group fullMatch = nextMatch.Groups[0];
                string fullMatchText = fullMatch.Value;

                caret.SetText(TextSetOptions.None, fullMatchText);

                if (Uri.TryCreate(fullMatchText, UriKind.Absolute, out Uri? uri)
                    || Uri.TryCreate(Uri.EscapeDataString(fullMatchText), UriKind.Absolute, out uri)
                    || Uri.TryCreate(string.Concat("http://", Uri.EscapeDataString(fullMatchText)), UriKind.Absolute, out uri))
                {
                    caret.Link = $"\"{uri.AbsoluteUri}\"";
                }

                caret.Collapse(false);

                prevMatchEnd = nextMatch.Index + nextMatch.Length;
                nextMatch = nextMatch.NextMatch();
            }

            string leftoverText = plainText.Substring(prevMatchEnd, plainText.Length - prevMatchEnd);

            caret.SetText(TextSetOptions.None, leftoverText);
            caret.Collapse(false);

            richEditTextDocument.Selection.SetRange(caret.StartPosition, caret.EndPosition);

            richEditTextDocument.EndUndoGroup();
        }
    }

    private void PART_URLifyButton_Click(object sender, RoutedEventArgs e)
    {
        this.UpdateLinkifiedText();
    }

    private void PART_RichEditBox_SelectionChanging(RichEditBox sender, RichEditBoxSelectionChangingEventArgs e)
    {
        this.PART_StatusBrush.Color = Colors.Red;
    }

    private void PART_RichEditBox_SelectionChanged(object sender, RoutedEventArgs e)
    {
        this.PART_StatusBrush.Color = Colors.Black;

        this.UpdateStatusText();
    }

    private void PART_RichEditBox_TextChanging(RichEditBox sender, RichEditBoxTextChangingEventArgs e)
    {
        RichEditTextDocument richEditTextDocument = this.PART_RichEditBox.TextDocument;
        richEditTextDocument.GetText(TextGetOptions.NoHidden | TextGetOptions.UseLf, out string plainText);

        this._isContentChanging = e.IsContentChanging;
    }

    private void PART_RichEditBox_TextChanged(object sender, RoutedEventArgs e)
    {
        this.UpdateStatusText();

        if (this._isContentChanging)
        {
            this._isContentChanging = false;

            if (this.PART_RichEditBox.FocusState is not FocusState.Unfocused)
            {
                if (!this._needsLinkify)
                {
                    this._needsLinkify = true;

                    this.UnlinkifyText();
                }
            }

            this.UpdateLinkTextColors();
        }
    }

    private void PART_RichEditBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (this._needsLinkify)
        {
            this._needsLinkify = false;
            this.UpdateLinkifiedText();
            this.UpdateLinkTextColors();
        }
    }
}
