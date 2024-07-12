using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics;

namespace Fantastical.App;

sealed partial class WindowedContentDialog : UserControl
{
    public static readonly DependencyProperty PrimaryButtonTextProperty = DependencyProperty.Register(
        nameof(PrimaryButtonText),
        typeof(string),
        typeof(WindowedContentDialog),
        new PropertyMetadata(string.Empty, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        {
            ((WindowedContentDialog)d).UpdateButtonsVisibilityStates();
        }));

    public string PrimaryButtonText
    {
        get => (string)this.GetValue(PrimaryButtonTextProperty);
        set => this.SetValue(PrimaryButtonTextProperty, value);
    }

    public static readonly DependencyProperty IsPrimaryButtonEnabledProperty = DependencyProperty.Register(
        nameof(IsPrimaryButtonEnabled),
        typeof(bool),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(bool)));

    public bool IsPrimaryButtonEnabled
    {
        get => (bool)this.GetValue(IsPrimaryButtonEnabledProperty);
        set => this.SetValue(IsPrimaryButtonEnabledProperty, value);
    }

    public static readonly DependencyProperty PrimaryButtonStyleProperty = DependencyProperty.Register(
        nameof(PrimaryButtonStyle),
        typeof(Style),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(Style)));

    public Style PrimaryButtonStyle
    {
        get => (Style)this.GetValue(PrimaryButtonStyleProperty);
        set => this.SetValue(PrimaryButtonStyleProperty, value);
    }

    public static readonly DependencyProperty SecondaryButtonTextProperty = DependencyProperty.Register(
        nameof(SecondaryButtonText),
        typeof(string),
        typeof(WindowedContentDialog),
        new PropertyMetadata(string.Empty, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        {
            ((WindowedContentDialog)d).UpdateButtonsVisibilityStates();
        }));

    public string SecondaryButtonText
    {
        get => (string)this.GetValue(SecondaryButtonTextProperty);
        set => this.SetValue(SecondaryButtonTextProperty, value);
    }

    public static readonly DependencyProperty IsSecondaryButtonEnabledProperty = DependencyProperty.Register(
        nameof(IsSecondaryButtonEnabled),
        typeof(bool),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(bool)));

    public bool IsSecondaryButtonEnabled
    {
        get => (bool)this.GetValue(IsSecondaryButtonEnabledProperty);
        set => this.SetValue(IsSecondaryButtonEnabledProperty, value);
    }

    public static readonly DependencyProperty SecondaryButtonStyleProperty = DependencyProperty.Register(
        nameof(SecondaryButtonStyle),
        typeof(Style),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(Style)));

    public Style SecondaryButtonStyle
    {
        get => (Style)this.GetValue(SecondaryButtonStyleProperty);
        set => this.SetValue(SecondaryButtonStyleProperty, value);
    }

    public static readonly DependencyProperty CloseButtonTextProperty = DependencyProperty.Register(
        nameof(CloseButtonText),
        typeof(string),
        typeof(WindowedContentDialog),
        new PropertyMetadata(string.Empty, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        {
            ((WindowedContentDialog)d).UpdateButtonsVisibilityStates();
        }));

    public string CloseButtonText
    {
        get => (string)this.GetValue(CloseButtonTextProperty);
        set => this.SetValue(CloseButtonTextProperty, value);
    }

    public static readonly DependencyProperty CloseButtonStyleProperty = DependencyProperty.Register(
        nameof(CloseButtonStyle),
        typeof(Style),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(Style)));

    public Style CloseButtonStyle
    {
        get => (Style)this.GetValue(CloseButtonStyleProperty);
        set => this.SetValue(CloseButtonStyleProperty, value);
    }

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title),
        typeof(object),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(object)));

    public object Title
    {
        get => (object)this.GetValue(TitleProperty);
        set => this.SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty TitleTemplateProperty = DependencyProperty.Register(
        nameof(TitleTemplate),
        typeof(DataTemplate),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(DataTemplate)));

    public DataTemplate TitleTemplate
    {
        get => (DataTemplate)this.GetValue(TitleTemplateProperty);
        set => this.SetValue(TitleTemplateProperty, value);
    }

    public static readonly DependencyProperty ContentTemplateProperty = DependencyProperty.Register(
        nameof(ContentTemplate),
        typeof(DataTemplate),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(DataTemplate)));

    public DataTemplate ContentTemplate
    {
        get => (DataTemplate)this.GetValue(ContentTemplateProperty);
        set => this.SetValue(ContentTemplateProperty, value);
    }

    public static readonly DependencyProperty DialogContentProperty = DependencyProperty.Register(
        nameof(DialogContent),
        typeof(object),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(object)));

    public object DialogContent
    {
        get => (object)this.GetValue(DialogContentProperty);
        set => this.SetValue(DialogContentProperty, value);
    }

    public static readonly DependencyProperty DefaultButtonProperty = DependencyProperty.Register(
        nameof(DefaultButton),
        typeof(ContentDialogButton),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(ContentDialogButton), (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        {
            ((WindowedContentDialog)d).UpdateDefaultButtonStates();
        }));

    public ContentDialogButton DefaultButton
    {
        get => (ContentDialogButton)this.GetValue(DefaultButtonProperty);
        set => this.SetValue(DefaultButtonProperty, value);
    }

    public WindowedContentDialog()
    {
        this.InitializeComponent();

        this.UpdateButtonsVisibilityStates();
        this.UpdateDefaultButtonStates();
    }

    private static void SetWindowOwner(Window window, WindowId ownerWindowId)
    {
        IntPtr windowHwnd = Win32Interop.GetWindowFromWindowId(window.AppWindow.Id);
        IntPtr ownerHwnd = Win32Interop.GetWindowFromWindowId(ownerWindowId);

        SetWindowLong(windowHwnd, GWL_HWNDPARENT, ownerHwnd);
    }

    private static OverlappedPresenter CreatePresenter()
    {
        OverlappedPresenter overlappedPresenter = OverlappedPresenter.Create();

        overlappedPresenter.SetBorderAndTitleBar(false, false);
        overlappedPresenter.IsAlwaysOnTop = false;
        overlappedPresenter.IsMaximizable = false;
        overlappedPresenter.IsMinimizable = false;
        overlappedPresenter.IsModal = false;
        overlappedPresenter.IsResizable = false;

        return overlappedPresenter;
    }

    private TaskCompletionSource<ContentDialogResult>? _taskCompletionSource;
    private Window? _window;

    private async Task<ContentDialogResult> ShowInternalAsync(WindowId? ownerWindowId, Point origin)
    {
        if (this._taskCompletionSource is not null)
        {
            throw new InvalidOperationException($"Already showing this dialog");
        }

        this._taskCompletionSource = new TaskCompletionSource<ContentDialogResult>();

        this._window = new Window();

        this._window.AppWindow.SetPresenter(CreatePresenter());

        if (ownerWindowId.HasValue)
        {
            SetWindowOwner(this._window, ownerWindowId.Value);
        }

        // this._window.SystemBackdrop = new DesktopAcrylicBackdrop(); // new MicaBackdrop() { Kind = Microsoft.UI.Composition.SystemBackdrops.MicaKind.Base };
        this._window.AppWindow.Move(new PointInt32((int)origin.X, (int)origin.Y));

        this.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        this.InvalidateArrange();

        this._window.Content = this;

        this._window.SizeChanged += this.Window_SizeChanged;
        this._window.Closed += this.Window_Closed;

        this._window.SystemBackdrop = new DesktopAcrylicBackdrop();

        this._window.Activate();

        ContentDialogResult contentDialogResult = await this._taskCompletionSource.Task;

        this._taskCompletionSource = null;

        this._window.SizeChanged -= this.Window_SizeChanged;
        this._window.Closed -= this.Window_Closed;
        this._window.Close();

        return contentDialogResult;
    }

    protected override void OnKeyDown(KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Escape)
        {
            e.Handled = true;
            this.Hide();

            return;
        }

        ContentDialogButton defaultButton = this.DefaultButton;

        if (defaultButton != ContentDialogButton.None
            && e.Key is Windows.System.VirtualKey.Enter or Windows.System.VirtualKey.Space)
        {
            if (defaultButton == ContentDialogButton.Primary && this.IsPrimaryButtonEnabled)
            {
                e.Handled = true;
                this._taskCompletionSource.SetResult(ContentDialogResult.Primary);

                return;
            }

            if (defaultButton == ContentDialogButton.Secondary && this.IsSecondaryButtonEnabled)
            {
                e.Handled = true;
                this._taskCompletionSource.SetResult(ContentDialogResult.Secondary);

                return;
            }

            if (defaultButton == ContentDialogButton.Close)
            {
                e.Handled = true;
                this._taskCompletionSource.SetResult(ContentDialogResult.None);

                return;
            }
        }

        base.OnKeyDown(e);
    }

    public async Task<ContentDialogResult> ShowAsync()
    {
        RectInt32 outerBounds = DisplayArea.Primary.OuterBounds;
        Point origin = new Point(outerBounds.X + outerBounds.Width / 2, outerBounds.Y + outerBounds.Height / 2);

        return await this.ShowInternalAsync(null, origin).ConfigureAwait(false);
    }

    public async Task<ContentDialogResult> ShowAsync(AppWindow appWindow)
    {
        WindowId ownerWindowId = appWindow.Id;
        PointInt32 position = appWindow.Position;
        SizeInt32 sizeInt32 = appWindow.Size;
        Point origin = new Point(position.X + sizeInt32.Width / 2, position.Y + sizeInt32.Height / 2);

        return await this.ShowInternalAsync(ownerWindowId, origin).ConfigureAwait(false);
    }

    public async Task<ContentDialogResult> ShowAsync(WindowId ownerWindowId)
    {
        AppWindow appWindow = AppWindow.GetFromWindowId(ownerWindowId);

        return await this.ShowAsync(appWindow).ConfigureAwait(false);
    }

    public async Task<ContentDialogResult> ShowAsync(Window window)
    {
        return await this.ShowAsync(window.AppWindow).ConfigureAwait(false);
    }

    public void Hide()
    {
        this._taskCompletionSource?.SetResult(ContentDialogResult.None);
    }

    private void UpdateButtonsVisibilityStates()
    {
        bool primary = !string.IsNullOrEmpty(this.PrimaryButtonText);
        bool secondary = !string.IsNullOrEmpty(this.SecondaryButtonText);
        bool close = !string.IsNullOrEmpty(this.CloseButtonText);
        string state;

        if (primary && secondary && close)
        {
            state = nameof(this.AllVisible);
        }
        else if (primary && close)
        {
            state = nameof(this.PrimaryAndCloseVisible);
        }
        else if (secondary && close)
        {
            state = nameof(this.SecondaryAndCloseVisible);
        }
        else if (primary && secondary)
        {
            state = nameof(this.PrimaryAndSecondaryVisible);
        }
        else if (primary)
        {
            state = nameof(this.PrimaryVisible);
        }
        else if (secondary)
        {
            state = nameof(this.SecondaryVisible);
        }
        else if (close)
        {
            state = nameof(this.CloseVisible);
        }
        else
        {
            state = nameof(this.NoneVisible);
        }

        VisualStateManager.GoToState(this, state, true);
    }

    private void UpdateDefaultButtonStates()
    {
        ContentDialogButton defaultButton = this.DefaultButton;
        string state;

        if (defaultButton == ContentDialogButton.Primary)
        {
            state = nameof(this.PrimaryAsDefaultButton);
        }
        else if (defaultButton == ContentDialogButton.Secondary)
        {
            state = nameof(this.SecondaryAsDefaultButton);
        }
        else if (defaultButton == ContentDialogButton.Close)
        {
            state = nameof(this.CloseAsDefaultButton);
        }
        else
        {
            state = nameof(this.NoDefaultButton);
        }

        VisualStateManager.GoToState(this, state, true);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        Size desiredSize = base.MeasureOverride(new Size(double.PositiveInfinity, double.PositiveInfinity));

        if (this._window is { AppWindow: AppWindow appWindow })
        {
            Size previousDesiredSize = this.DesiredSize;
            double deltaX = (desiredSize.Width - previousDesiredSize.Width) / 2;
            double deltaY = (desiredSize.Height - previousDesiredSize.Height) / 2;
            PointInt32 position = appWindow.Position;
            PointInt32 shiftedPosition = new PointInt32((int)(position.X - deltaX / 2), (int)(position.Y - deltaY / 2));

            appWindow.Move(shiftedPosition);
            appWindow.ResizeClient(new SizeInt32((int)desiredSize.Width, (int)desiredSize.Height));
        }

        return desiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        Size size = base.ArrangeOverride(finalSize);

        return size;
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        this.UpdateButtonsVisibilityStates();

        // NOTE WV: There seems to be a bug in loading, where
        //          visual styles aren't applied or something
        //          so we hack around it
        VisualStateManager.GoToState(this, nameof(NoDefaultButton), false);
        this.UpdateDefaultButtonStates();

        ContentDialogButton defaultButton = this.DefaultButton;

        if (defaultButton == ContentDialogButton.Primary)
        {
            this.PrimaryButton.Focus(FocusState.Programmatic);
        }
        else if (defaultButton == ContentDialogButton.Secondary)
        {
            this.SecondaryButton.Focus(FocusState.Programmatic);
        }
        else if (defaultButton == ContentDialogButton.Close)
        {
            this.CloseButton.Focus(FocusState.Programmatic);
        }
    }

    private void Window_SizeChanged(object sender, WindowSizeChangedEventArgs args)
    {
    }

    private void Window_Closed(object sender, WindowEventArgs args)
    {
        if (this._taskCompletionSource is not null)
        {
            this._taskCompletionSource.SetResult(ContentDialogResult.None);
        }
    }

    private void PrimaryButton_Click(object sender, RoutedEventArgs e)
    {
        this._taskCompletionSource.SetResult(ContentDialogResult.Primary);
    }

    private void SecondaryButton_Click(object sender, RoutedEventArgs e)
    {
        this._taskCompletionSource.SetResult(ContentDialogResult.Secondary);
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        this._taskCompletionSource.SetResult(ContentDialogResult.None);
    }

    [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

    private const int GWL_HWNDPARENT = (-8);

    private static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
    {
        if (IntPtr.Size == 4)
        {
            return SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
        }
        return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
    }

    [DllImport("User32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLong")]
    private static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [DllImport("User32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLongPtr")]
    public static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
}
