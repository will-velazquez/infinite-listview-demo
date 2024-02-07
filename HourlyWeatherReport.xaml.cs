using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;

namespace App1;

internal sealed partial class HourlyWeatherReport : UserControl
{
    private static readonly CanvasTextFormat TimeTextFormat = new CanvasTextFormat()
    {
        FontSize = 12,
        VerticalAlignment = CanvasVerticalAlignment.Top,
        HorizontalAlignment = CanvasHorizontalAlignment.Left,
        WordWrapping = CanvasWordWrapping.NoWrap,
        TrimmingGranularity = CanvasTextTrimmingGranularity.Character,
        Options = CanvasDrawTextOptions.Clip
    };

    private static readonly CanvasTextFormat TemperatureTextFormat = new CanvasTextFormat()
    {
        FontSize = 12,
        VerticalAlignment = CanvasVerticalAlignment.Bottom,
        HorizontalAlignment = CanvasHorizontalAlignment.Left,
        WordWrapping = CanvasWordWrapping.NoWrap,
        TrimmingGranularity = CanvasTextTrimmingGranularity.Character,
        Options = CanvasDrawTextOptions.Clip
    };

    public readonly static DependencyProperty CurrentTimeProperty =
        DependencyProperty.Register(
            nameof(CurrentTime),
            typeof(DateTime),
            typeof(HourlyWeatherReport),
            new PropertyMetadata(default(DateTime), (DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs) =>
            {
                // (dependencyObject as HourlyWeatherReport ?? throw new InvalidOperationException()).UpdateTime();
            }));

    public DateTime CurrentTime
    {
        get => (DateTime)this.GetValue(CurrentTimeProperty);
        set => this.SetValue(CurrentTimeProperty, value);
    }

    public readonly static DependencyProperty WeatherReportPointsProperty =
        DependencyProperty.Register(
            nameof(WeatherReportPoints),
            typeof(WeatherReportPoint[]),
            typeof(HourlyWeatherReport),
            new PropertyMetadata(new WeatherReportPoint[] { }, (DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs) =>
            {
                (dependencyObject as HourlyWeatherReport ?? throw new InvalidOperationException()).UpdateTemperaturePoints();
            }));

    public WeatherReportPoint[]? WeatherReportPoints
    {
        get => (WeatherReportPoint[]?)this.GetValue(WeatherReportPointsProperty);
        set => this.SetValue(WeatherReportPointsProperty, value);
    }

    public readonly static DependencyProperty PointRadiusProperty =
        DependencyProperty.Register(
            nameof(PointRadius),
            typeof(double),
            typeof(HourlyWeatherReport),
            new PropertyMetadata(3.0, (DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs) =>
            {
                (dependencyObject as HourlyWeatherReport ?? throw new InvalidOperationException()).CanvasControl.Invalidate();
            }));

    public double PointRadius
    {
        get => (double)this.GetValue(PointRadiusProperty);
        set => this.SetValue(PointRadiusProperty, value);
    }

    public readonly static DependencyProperty GraphColorProperty =
        DependencyProperty.Register(
            nameof(GraphColor),
            typeof(Color),
            typeof(HourlyWeatherReport),
            new PropertyMetadata(Colors.Red, (DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs) =>
            {
                (dependencyObject as HourlyWeatherReport ?? throw new InvalidOperationException()).CanvasControl.Invalidate();
            }));

    public Color GraphColor
    {
        get => (Color)this.GetValue(GraphColorProperty);
        set => this.SetValue(GraphColorProperty, value);
    }

    private readonly Dictionary<WeatherIcon, CanvasBitmap> WeatherBitmaps = new Dictionary<WeatherIcon, CanvasBitmap>();

    public HourlyWeatherReport()
    {
        this.InitializeComponent();
    }

    private void LoadBitmaps(object themeToken)
    {
        DispatcherQueue.GetForCurrentThread().TryEnqueue(DispatcherQueuePriority.Low, async () =>
        {
            this.WeatherBitmaps[WeatherIcon.ClearDay] = await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), new Uri("ms-appx:///Assets/Weather/clear-day-light.svg.png"));
            this.WeatherBitmaps[WeatherIcon.Cloudy] = await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), new Uri("ms-appx:///Assets/Weather/cloudy-light.svg.png"));
            this.WeatherBitmaps[WeatherIcon.Hot] = await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), new Uri("ms-appx:///Assets/Weather/hot-light.svg.png"));
            this.WeatherBitmaps[WeatherIcon.PartlyCloudyDay] = await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), new Uri("ms-appx:///Assets/Weather/partly-cloudy-light.svg.png"));
            this.WeatherBitmaps[WeatherIcon.Rain] = await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), new Uri("ms-appx:///Assets/Weather/rain-light.svg.png"));
            this.CanvasControl.Invalidate();
        });
    }

    private void UpdateTemperaturePoints()
    {
        if (this.IsLoaded)
        {
            this.CanvasControl.Invalidate();
        }
    }

    private void CanvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
    {
        CanvasDrawingSession canvasDrawingSession = args.DrawingSession;
        CanvasPathBuilder canvasPathBuilder = new CanvasPathBuilder(canvasDrawingSession);

        if (this.WeatherReportPoints is null || this.WeatherReportPoints.Length < 2)
        {
            // TODO - we could just display a single point..
            return;
        }


        double vertSpacing = 5;
        double contentX = 0;
        double contentY = 0;
        double contentWidth = sender.RenderSize.Width;
        double contentHeight = sender.RenderSize.Height;

        // Horizontally place the points
        double graphX = contentX + (this.PointRadius + 20);
        double graphWidth = contentWidth - graphX - (this.PointRadius + 17);

        double pointWidth = graphWidth / (this.WeatherReportPoints.Length - 1);
        double[] pointsX = new double[this.WeatherReportPoints.Length];
        for (int i = 0; i < pointsX.Length; ++i)
        {
            pointsX[i] = graphX + (pointWidth * i);
        }

        // Time text
        {
            double partHeight = 0;
            for (int i = 0; i < this.WeatherReportPoints.Length; ++i)
            {
                DateTime time = this.WeatherReportPoints[i].Time;

                string timeText;
                Color timeTextColor;
                if (i == 0)
                {
                    // TODO WV  i8n
                    timeText = "NOW";
                    timeTextColor = Colors.Red;
                }
                else if (time.Minute == 0)
                {
                    // TODO WV  Replace w/ new time formatting
                    string shortestFormat = CultureInfo.CurrentUICulture.DateTimeFormat.ShortTimePattern.Replace(":", string.Empty).Replace("m", string.Empty);
                    // TODO WV  Timezones
                    timeText = time.ToLocalTime().ToString(shortestFormat, CultureInfo.CurrentUICulture);
                    timeTextColor = Colors.Gray;
                }
                else
                {
                    timeText = time.ToLocalTime().ToString("t");
                    timeTextColor = Colors.Gray;
                }

                double x = pointsX[i];
                CanvasTextLayout timeLayout = new CanvasTextLayout(sender, timeText, TimeTextFormat, (float)pointWidth, (float)contentHeight);
                partHeight = Math.Max(partHeight, timeLayout.LayoutBounds.Height);

                canvasDrawingSession.DrawTextLayout(timeLayout, (float)(x - (timeLayout.LayoutBounds.Width / 2)), (float)contentY, timeTextColor);
            }

            if (partHeight > 0)
            {
                contentY += partHeight + 2;
                contentHeight -= partHeight + 2;
            }
        }

        // Weather icons
        {
            int iconSize = 24;
            double partHeight = iconSize;
            for (int i = 0; i < this.WeatherReportPoints.Length; ++i)
            {
                WeatherIcon forecastKind = this.WeatherReportPoints[i].Forecast;
                if (!this.WeatherBitmaps.TryGetValue(forecastKind, out CanvasBitmap? canvasBitmap))
                {
                    continue;
                }

                double x = pointsX[i] - iconSize / 2;
                double y = contentY;

                canvasDrawingSession.DrawImage(canvasBitmap, new Rect(x, y, iconSize, iconSize));
            }

            if (partHeight > 0)
            {
                contentY += partHeight + 2;
                contentHeight -= partHeight + 2;
            }
        }

        // footer
        {
            double partHeight = 0;
            for (int i = 0; i < this.WeatherReportPoints.Length; ++i)
            {
                double temperature = this.WeatherReportPoints[i].Temperature;
                double x = pointsX[i];
                CanvasTextLayout tempLayout = new CanvasTextLayout(sender, $"{temperature:0}º", TemperatureTextFormat, (float)pointWidth, (float)contentHeight);
                partHeight = Math.Max(partHeight, tempLayout.LayoutBounds.Height);

                canvasDrawingSession.DrawTextLayout(tempLayout, (float)(x - (tempLayout.LayoutBounds.Width / 2)), (float)contentY, Colors.Black);
            }

            if (partHeight > 0)
            {
                contentHeight -= partHeight + vertSpacing;
            }
        }

        // Remaining space to graph
        {
            Rect graphRect = new Rect(contentX, contentY, contentWidth, contentHeight);

            double min = double.PositiveInfinity;
            double max = double.NegativeInfinity;
            for (int i = 0; i < this.WeatherReportPoints.Length; ++i)
            {
                double temperaturePoint = this.WeatherReportPoints[i].Temperature;
                min = Math.Min(min, temperaturePoint);
                max = Math.Max(max, temperaturePoint);
            }

            double range = max - min;
            if (range == 0)
            {
                range = 1;
            }

            Vector2[] plotPoints = new Vector2[this.WeatherReportPoints.Length];
            for (int i = 0; i < this.WeatherReportPoints.Length; ++i)
            {
                double x = pointsX[i];
                double y = graphRect.Top + CalculateY(this.WeatherReportPoints[i].Temperature, min, range, graphRect.Height);
                plotPoints[i] = new Vector2((float)x, (float)y);
            }

            Vector2[] tangents = new Vector2[plotPoints.Length];

            tangents[0] = Normalize(plotPoints[1] - plotPoints[0]);

            for (int i = 1; i < plotPoints.Length - 1; ++i)
            {
                tangents[i] = Normalize(plotPoints[i + 1] - plotPoints[i - 1]);
            }

            tangents[plotPoints.Length - 1] = Normalize(plotPoints[plotPoints.Length - 1] - plotPoints[plotPoints.Length - 2]);

            Vector2 prevPoint = plotPoints[0];
            canvasPathBuilder.BeginFigure(prevPoint);
            for (int i = 1; i < plotPoints.Length; ++i)
            {
                Vector2 currPoint = plotPoints[i];
                float thirdDist = (currPoint - prevPoint).Length() / 3;

                Vector2 ctrl1 = prevPoint + tangents[i - 1] * thirdDist;
                Vector2 ctrl2 = currPoint - tangents[i + 0] * thirdDist;

                canvasPathBuilder.AddCubicBezier(
                    ctrl1,
                    ctrl2,
                    currPoint);

                prevPoint = currPoint;
            }

            canvasPathBuilder.EndFigure(CanvasFigureLoop.Open);

            CanvasGeometry pathGeometry = CanvasGeometry.CreatePath(canvasPathBuilder);

            for (int i = 0; i < plotPoints.Length; ++i)
            {
                float x = plotPoints[i].X;
                canvasDrawingSession.DrawLine(x, (float)graphRect.Top, x, (float)graphRect.Bottom, Colors.LightGray);
            }

            canvasDrawingSession.DrawGeometry(pathGeometry, this.GraphColor);

            for (int i = 0; i < plotPoints.Length; ++i)
            {
                canvasDrawingSession.FillCircle(plotPoints[i], (float)this.PointRadius, this.GraphColor);
            }

            static double CalculateY(double temperature, double min, double range, double height)
            {
                double perc = (temperature - min) / range;
                return height * (1.0 - (perc * 0.5));
            }

            static Vector2 Normalize(Vector2 v)
            {
                float length = v.Length();
                return new Vector2(v.X / length, v.Y / length);
            }
        }
    }
}

