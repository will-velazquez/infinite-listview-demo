using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Foundation;

namespace Fantastical.App.Pages;

internal sealed partial class ParserLayoutTest1 : Page
{
    private long _visibilityRegistration;

    public ParserLayoutTest1()
    {
        this.InitializeComponent();

        this.PART_ButtonVisibleCheckbox.IsChecked = true;
        this.PART_FixedRowHeightNumberBox.Value = 30; 
    }

    private void PART_Grid_LayoutUpdated(object sender, object e)
    {
        
    }

    private void PART_Grid_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        this.PART_FixedRowHeightText.Text = this.PART_FixedRow.ActualHeight.ToString();
        // this.PART_AutoRowHeightText.Text = this.PART_AutoRow.ActualHeight.ToString();
    }

    private void UpdateParserPositioning()
    {
        Point p = this.PART_ShowParserButton.TransformToVisual(this.PART_Grid).TransformPoint(new Point(0, 0));

        this.PART_ButtonPositionText.Text = $"{p.X}, {p.Y}";

        this.PART_ParserTextBox.Margin = new Thickness(0, p.Y, 0, 0);
        this.PART_ParserPanel.Margin = new Thickness(0, p.Y + this.PART_ParserTextBox.ActualHeight + 10, 0, 10);
    }

    private void PART_ShowParserButton_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        this.UpdateParserPositioning();
    }

    private void PART_ShowParserButton_EffectiveViewportChanged(FrameworkElement sender, EffectiveViewportChangedEventArgs args)
    {
        this.UpdateParserPositioning();
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        this._visibilityRegistration = this.PART_ShowParserButton.RegisterPropertyChangedCallback(UIElement.VisibilityProperty, PART_ShowParserButton_VisibilityChanged);
    }

    private void Page_Unloaded(object sender, RoutedEventArgs e)
    {
        this.PART_ShowParserButton.UnregisterPropertyChangedCallback(UIElement.VisibilityProperty, this._visibilityRegistration);
        this._visibilityRegistration = 0;
    }

    private void PART_ShowParserButton_VisibilityChanged(DependencyObject d, DependencyProperty p)
    {
        // this.UpdateParserPositioning();
    }

    private void PART_FixedRowHeightNumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        double height = Math.Max(15, sender.Value);

        this.PART_FixedRow.Height = new GridLength(height, GridUnitType.Pixel);
        this.PART_FixedRow.MinHeight = this.PART_FixedRow.MaxHeight = height;
    }

    private void PART_ParserTextBox_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        this.UpdateParserPositioning();
    }
}
