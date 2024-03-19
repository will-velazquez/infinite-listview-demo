using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI;

namespace App1;


enum FreeBusyItemType
{
	FreeBusyItemTypeBusy = 0,
	FreeBusyItemTypeFree,
	FreeBusyItemTypeUnavailable,
	FreeBusyItemTypeTentative,
	FreeBusyItemTypeUserNotFound, // not found on the server
	FreeBusyItemTypeWorkingElsewhere, // Exchange only
}

internal sealed record FreeBusyGap(DateTime StartDate, DateTime EndDate);

internal sealed record FreeBusyItem(DateTime StartDate, DateTime EndDate, FreeBusyItemType Type, string Title);

internal sealed record FreeBusyAttendee(string AttendeeId, FreeBusyItem[] FreeBusyItems);

internal sealed partial class FreeBusyViewModel : ObservableObject
{
	public ObservableCollection<FreeBusyAttendee> FreeBusyAttendees { get; } = new ObservableCollection<FreeBusyAttendee>();

	public ObservableCollection<FreeBusyGap> FreeBusyGaps { get; } = new ObservableCollection<FreeBusyGap>();

	public ObservableCollection<string> Hours { get; } = new ObservableCollection<string>();

	public FreeBusyViewModel()
	{
		this.FreeBusyAttendees.Add(
			new FreeBusyAttendee("wilfredo", new FreeBusyItem[]
			{
				new FreeBusyItem(DateTime.Parse("03/13/2024 8:00"), DateTime.Parse("03/13/2024 8:30"), FreeBusyItemType.FreeBusyItemTypeWorkingElsewhere, "morning coffee"),
				new FreeBusyItem(DateTime.Parse("03/13/2024 11:30"), DateTime.Parse("03/13/2024 12:00"), FreeBusyItemType.FreeBusyItemTypeBusy, "Windows Demo"),
			}));

		this.FreeBusyAttendees.Add(
			new FreeBusyAttendee("kent", new FreeBusyItem[]
			{
				new FreeBusyItem(DateTime.Parse("03/13/2024 8:00"), DateTime.Parse("03/13/2024 9:30"), FreeBusyItemType.FreeBusyItemTypeBusy, "scheduling"),
				new FreeBusyItem(DateTime.Parse("03/13/2024 11:30"), DateTime.Parse("03/13/2024 12:00"), FreeBusyItemType.FreeBusyItemTypeTentative, "Windows Demo"),
				new FreeBusyItem(DateTime.Parse("03/13/2024 02:30 PM"), DateTime.Parse("03/13/2024 03:00 PM"), FreeBusyItemType.FreeBusyItemTypeBusy, "Afternoon Dev Meeting"),
			}));

		this.FreeBusyAttendees.Add(
			new FreeBusyAttendee("longer name after", new FreeBusyItem[]
			{
				new FreeBusyItem(DateTime.Parse("03/13/2024 0:00"), DateTime.Parse("03/14/2024 0:00"), FreeBusyItemType.FreeBusyItemTypeUserNotFound, "Unavailable"),
			}));

		this.FreeBusyGaps.Add(new FreeBusyGap(DateTime.Parse("03/13/2024 0:00"), DateTime.Parse("03/13/2024 8:00")));
		this.FreeBusyGaps.Add(new FreeBusyGap(DateTime.Parse("03/13/2024 9:30"), DateTime.Parse("03/13/2024 11:30")));
		this.FreeBusyGaps.Add(new FreeBusyGap(DateTime.Parse("03/13/2024 12:00"), DateTime.Parse("03/13/2024 2:30 PM")));
		this.FreeBusyGaps.Add(new FreeBusyGap(DateTime.Parse("03/13/2024 3:00 PM"), DateTime.Parse("03/14/2024 11:30")));
	}
}

internal sealed partial class AvailabilityGrid : UserControl
{
	public readonly static DependencyProperty DateProperty = DependencyProperty.Register(
		nameof(Date),
		typeof(int),
		typeof(AvailabilityGrid),
		new PropertyMetadata(DateOnly.FromDateTime(DateTime.Parse("03/13/2024")).DayNumber, (DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args) =>
		{
		}));

	public int Date
	{
		get => (int)this.GetValue(DateProperty);
		set => this.SetValue(DateProperty, value);
	}

	public readonly static DependencyProperty StartHourProperty = DependencyProperty.Register(
		nameof(StartHour),
		typeof(int),
		typeof(AvailabilityGrid),
		new PropertyMetadata((int)0, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
		{
			((AvailabilityGrid)d).SyncStartEndTimes();
		}));

	public int StartHour
	{
		get => (int)this.GetValue(StartHourProperty);
		set => this.SetValue(StartHourProperty, value);
	}

	public readonly static DependencyProperty EndHourProperty = DependencyProperty.Register(
		nameof(EndHour),
		typeof(int),
		typeof(AvailabilityGrid),
		new PropertyMetadata((int)24, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
		{
			((AvailabilityGrid)d).SyncStartEndTimes();
		}));

	public int EndHour
	{
		get => (int)this.GetValue(EndHourProperty);
		set => this.SetValue(EndHourProperty, value);
	}

	public readonly static DependencyProperty EventStartDateProperty = DependencyProperty.Register(
		nameof(EventStartDate),
		typeof(DateTime),
		typeof(AvailabilityGrid),
		new PropertyMetadata(DateTime.Parse("03/13/2024").Date.AddHours(11).ToUniversalTime(), (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
		{
			((AvailabilityGrid)d).SyncAvailabilityBlockPositions();
		}));

	public DateTime EventStartDate
	{
		get => (DateTime)this.GetValue(EventStartDateProperty);
		set => this.SetValue(EventStartDateProperty, value);
	}

	public readonly static DependencyProperty EventEndDateProperty = DependencyProperty.Register(
		nameof(EventEndDate),
		typeof(DateTime),
		typeof(AvailabilityGrid),
		new PropertyMetadata(DateTime.Parse("03/13/2024").Date.AddHours(12).ToUniversalTime(), (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
		{
			((AvailabilityGrid)d).SyncAvailabilityBlockPositions();
		}));

	public DateTime EventEndDate
	{
		get => (DateTime)this.GetValue(EventEndDateProperty);
		set => this.SetValue(EventEndDateProperty, value);
	}

	public readonly static DependencyProperty EventColorProperty = DependencyProperty.Register(
		nameof(EventColor),
		typeof(Color),
		typeof(AvailabilityGrid),
		new PropertyMetadata(Colors.Black, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
		{
			((AvailabilityGrid)d).SyncEventColor();
		}));

	public Color EventColor
	{
		get => (Color)this.GetValue(EventColorProperty);
		set
		{
			if (this.EventColor != value)
			{
				this.SetValue(EventColorProperty, value);
			}
		}
	}

	public readonly static DependencyProperty UsesFixedHourWidthProperty = DependencyProperty.Register(
		nameof(UsesFixedHourWidth),
		typeof(bool),
		typeof(AvailabilityGrid),
		new PropertyMetadata(default(bool)));

	public bool UsesFixedHourWidth
	{
		get => (bool)this.GetValue(UsesFixedHourWidthProperty);
		set => this.SetValue(UsesFixedHourWidthProperty, value);
	}

	public readonly static DependencyProperty FixedHourWidthProperty = DependencyProperty.Register(
		nameof(FixedHourWidth),
		typeof(double),
		typeof(AvailabilityGrid),
		new PropertyMetadata((double)50));

	public double FixedHourWidth
	{
		get => (double)this.GetValue(FixedHourWidthProperty);
		set => this.SetValue(FixedHourWidthProperty, value);
	}

	private readonly static DependencyProperty ViewModelProperty = DependencyProperty.Register(
		nameof(ViewModel),
		typeof(FreeBusyViewModel),
		typeof(AvailabilityGrid),
		new PropertyMetadata(default(FreeBusyViewModel), (DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args) =>
		{
		}));

	private FreeBusyViewModel ViewModel
	{
		get => (FreeBusyViewModel)this.GetValue(ViewModelProperty);
		set => this.SetValue(ViewModelProperty, value);
	}

	private const int _freeBusyGapZIndex = 0;
	private const int _organizerFreeBusyZIndex = 100;
	private const int _eventZIndex = 200;
	private const int _attendeesFreeBusyZIndex = 300;

	private readonly Rectangle _organizerEventRectangle;
	private readonly Rectangle _attendeesEventRectangle;
	private readonly List<FrameworkElement> _availabilityBlocks = new List<FrameworkElement>();

	private readonly Color _lightLabelTextColorLight = Color.FromArgb(
		(byte)Math.Clamp(Math.Truncate(1.0 * 255), 0, 255),
		(byte)Math.Clamp(Math.Truncate(0.494 * 255), 0, 255),
		(byte)Math.Clamp(Math.Truncate(0.498 * 255), 0, 255),
		(byte)Math.Clamp(Math.Truncate(0.502 * 255), 0, 255));

	private readonly Color _lightLabelTextColorDark = Color.FromArgb(
		(byte)Math.Clamp(Math.Truncate(1.0 * 255), 0, 255),
		(byte)Math.Clamp(Math.Truncate(0.694 * 255), 0, 255),
		(byte)Math.Clamp(Math.Truncate(0.694 * 255), 0, 255),
		(byte)Math.Clamp(Math.Truncate(0.702 * 255), 0, 255));


	public AvailabilityGrid()
	{
		this.InitializeComponent();

		this.ViewModel = new FreeBusyViewModel();

		this._organizerEventRectangle = new Rectangle()
		{
			RadiusX = 2,
			RadiusY = 2,
		};

		Canvas.SetZIndex(this._organizerEventRectangle, _eventZIndex);
		this.PART_AvailabilityCanvas.Children.Add(this._organizerEventRectangle);

		this._attendeesEventRectangle = new Rectangle()
		{
			RadiusX = 2,
			RadiusY = 2,
		};

		Canvas.SetZIndex(_attendeesEventRectangle, _eventZIndex);
		this.PART_AvailabilityCanvas.Children.Add(this._attendeesEventRectangle);

		this.SyncEventColor();
		this.SyncStartEndTimes();
		this.SyncAvailabilityBlockPositions();
	}

	private int CalcNumHours(int startHour, int endHour) => Math.Max(5, endHour - startHour);

	private double GetGridColumnWidth(bool usesFixedHourWidth, int startHour, int endHour, double fixedHourWidth, double viewportWidth, Thickness margin)
	{
		if (usesFixedHourWidth)
		{
			int numHours = this.CalcNumHours(startHour, endHour);
			
			// The 0.5 here accounts for our left offset to align the first hour
			// with the left edge of the grid
			double requiredWidth = fixedHourWidth * (numHours + 0.5);

			return requiredWidth;
		}
		else
		{
			return viewportWidth - margin.Left - margin.Right;
		}
	}

	private GridLength GetHourHalfWidth(bool usesFixedHourWidth, int startHour, int endHour, double fixedHourWidth, double viewportWidth, Thickness margin)
	{
		double hourWidth;

		if (usesFixedHourWidth)
		{
			hourWidth = fixedHourWidth;
		}
		else
		{
			int numHours = this.CalcNumHours(startHour, endHour);
			hourWidth = (viewportWidth - margin.Left - margin.Right) / (numHours + 0.5);
		}

		return new GridLength(hourWidth / 2, GridUnitType.Pixel);
	}

	private string GetFinalHourText(int startHour, int endHour)
	{
		(DateTime _, DateTime endDateTime) = this.GridStartEndTimes();

		// TODO i8n
		// TODO 24h
		if (endDateTime.Hour == 11)
		{
			return "noon";
		}

		return endDateTime.ToLocalTime().ToString("%h");
	}

	private Brush GetUnavailableBrush()
	{
		Color color = this.ActualTheme == ElementTheme.Dark ? this._lightLabelTextColorLight : this._lightLabelTextColorDark;
		Color colorWithAlpha = Color.FromArgb((byte)(0.7 * 255), color.R, color.G, color.B);

		return MakeStripePatternBrush(colorWithAlpha);
	}

	private Brush GetNotFoundBrush() => MakeStripePatternBrush(Color.FromArgb(0x66, 0xFF, 0xAA, 0x33));

	private Brush MakeStripePatternBrush(Color stripeColor)
	{
		// Do transparent / color / transparent
		// This way when we .Reflect it, we get a repeating pattern.
		// .Repeat appears to have bugs where other brushes on the page
		// appear lighter/greyed out for some reason
		double imageSize = 5;
		LinearGradientBrush brush = new()
		{
			StartPoint = new Windows.Foundation.Point(0, 0),
			EndPoint = new Windows.Foundation.Point(imageSize, imageSize),
			SpreadMethod = GradientSpreadMethod.Reflect,
			MappingMode = BrushMappingMode.Absolute
		};

		GradientStopCollection stops = new GradientStopCollection();

		double percColor = 3.0 / 20;

		stops.Add(new GradientStop()
		{
			Color = Colors.Transparent,
			Offset = 0.0,
		});

		stops.Add(new GradientStop()
		{
			Color = Colors.Transparent,
			Offset = 0.5 - percColor,
		});

		stops.Add(new GradientStop()
		{
			Color = stripeColor,
			Offset = 0.5 - (percColor / 2),
		});

		stops.Add(new GradientStop()
		{
			Color = stripeColor,
			Offset = 0.5,
		});

		stops.Add(new GradientStop()
		{
			Color = stripeColor,
			Offset = 0.5 + (percColor / 2),
		});

		stops.Add(new GradientStop()
		{
			Color = Colors.Transparent,
			Offset = 0.5 + percColor,
		});

		stops.Add(new GradientStop()
		{
			Color = Colors.Transparent,
			Offset = 1,
		});

		brush.GradientStops = stops;

		return brush;
	}

	private (DateTime StartTime, DateTime EndTime) GridStartEndTimes()
	{
		DateTime date = DateOnly.FromDayNumber(this.Date).ToDateTime(TimeOnly.MinValue, DateTimeKind.Local).ToUniversalTime();
		int startHour = this.StartHour;
		DateTime startTime = date.AddHours(startHour);

		int duration = this.EndHour - startHour;

		if (duration < 5)
		{
			duration = 5;
		}

		DateTime endTime = startTime.AddHours(duration);

		return (startTime, endTime);
	}

	private (double Start, double Width) CalcTimeStartWidth(DateTime blockStartDate, DateTime blockEndDate)
	{
		(DateTime gridStartTime, DateTime gridEndTime) = this.GridStartEndTimes();

		DateTime cappedBlockStartDate = gridEndTime < blockStartDate
			? gridEndTime
			: blockStartDate < gridStartTime
			? gridStartTime
			: blockStartDate;

		DateTime clampedBlockEndDate = gridEndTime < blockEndDate
			? gridEndTime
			: blockEndDate < gridStartTime
			? gridStartTime
			: blockEndDate;

		TimeSpan blockDuration = clampedBlockEndDate - cappedBlockStartDate;

		if (blockDuration < TimeSpan.Zero)
		{
			blockDuration = TimeSpan.Zero;
		}

		TimeSpan relStartDate = cappedBlockStartDate - gridStartTime;

		double columnWidth = this.PART_AvailabilityCanvas.ActualWidth / (this.CalcNumHours(this.StartHour, this.EndHour));
		double xPos = relStartDate.TotalHours * columnWidth;
		double width = blockDuration.TotalHours * columnWidth;

		return (xPos, width);
	}

	private void SyncEventColor()
	{
		this._organizerEventRectangle.Fill = new SolidColorBrush(this.EventColor);
		this._attendeesEventRectangle.Fill = new SolidColorBrush(this.EventColor) { Opacity = 0.25 };

		foreach (FrameworkElement frameworkElement in this._availabilityBlocks)
		{
			if (frameworkElement.Tag is FreeBusyGap && frameworkElement is Rectangle r)
			{
				r.Stroke = new SolidColorBrush(this.EventColor) { Opacity = 1 };
				r.Fill = new SolidColorBrush(this.EventColor) { Opacity = 0.1 };
			}
		}
	}

	private void SyncStartEndTimes()
	{
		this.ViewModel.Hours.Clear();

		(DateTime gridStartTime, DateTime gridEndTime) = this.GridStartEndTimes();

		for (DateTime currentHour = gridStartTime; currentHour < gridEndTime; currentHour = currentHour.AddHours(1))
		{
			int hourNumber = currentHour.ToLocalTime().Hour;

			if (hourNumber == 12)
			{
				// TODO i8n
				this.ViewModel.Hours.Add("noon");
			}
			else if (hourNumber == 0)
			{
				// TODO 24h
				this.ViewModel.Hours.Add("12");
			}
			else
			{
				// TODO 24h
				this.ViewModel.Hours.Add((hourNumber % 12).ToString());
			}
		}

		this.SyncAvailabilityBlockPositions();
	}

	private void SyncAvailabilityBlockPositions()
	{
		DateOnly date = DateOnly.FromDayNumber(this.Date);
		double rowHeight = this.PART_AvailabilityCanvas.ActualHeight / this.ViewModel.FreeBusyAttendees.Count;

		(double xPosOfEvent, double widthOfEvent) = this.CalcTimeStartWidth(this.EventStartDate.ToUniversalTime(), this.EventEndDate.ToUniversalTime());

		foreach (FrameworkElement availabilityBlock in _availabilityBlocks)
		{
			DateTime startDate;
			DateTime endDate;
			double yPos;
			double height;
			RectangleGeometry? rectangleGeometry = null;

			if (availabilityBlock.Tag is FreeBusyItem freeBusyItem)
			{
				startDate = freeBusyItem.StartDate.ToUniversalTime();
				endDate = freeBusyItem.EndDate.ToUniversalTime();

				int row = this.ViewModel.FreeBusyAttendees.Select((fba, i) => new { fba, i }).Where(s => s.fba.FreeBusyItems.Any(fbi => object.ReferenceEquals(fbi, freeBusyItem))).Select(s => s.i).First();

				yPos = row * rowHeight + 1;
				height = Math.Max(0, rowHeight - 1);

				rectangleGeometry = ((availabilityBlock as Border)?.Child?.Clip);
			}
			else if (availabilityBlock.Tag is FreeBusyGap freeBusyGap)
			{
				startDate = freeBusyGap.StartDate.ToUniversalTime();
				endDate = freeBusyGap.EndDate.ToUniversalTime();
				yPos = 0;
				height = this.PART_AvailabilityCanvas.ActualHeight;
			}
			else
			{
				throw new InvalidOperationException();
			}

			(double xPos, double width) = this.CalcTimeStartWidth(startDate, endDate);

			Canvas.SetLeft(availabilityBlock, xPos);
			availabilityBlock.Width = width;
			Canvas.SetTop(availabilityBlock, yPos);
			availabilityBlock.Height = height;

			if (rectangleGeometry is not null)
			{
				rectangleGeometry.Rect = new Windows.Foundation.Rect(xPosOfEvent - xPos, 0, widthOfEvent, height);
			}
		}

		Canvas.SetLeft(_organizerEventRectangle, xPosOfEvent);
		this._organizerEventRectangle.Width = widthOfEvent;
		Canvas.SetTop(this._organizerEventRectangle, 0);
		this._organizerEventRectangle.Height = rowHeight;

		Canvas.SetLeft(this._attendeesEventRectangle, xPosOfEvent);
		this._attendeesEventRectangle.Width = widthOfEvent;
		Canvas.SetTop(this._attendeesEventRectangle, rowHeight);
		this._attendeesEventRectangle.Height = this.PART_AvailabilityCanvas.ActualHeight - rowHeight;
	}

	private void SyncAvailabilityBlocks()
	{
		List<FrameworkElement> toRemove = new List<FrameworkElement>();

		foreach (FrameworkElement rectangle in _availabilityBlocks)
		{
			if (rectangle.Tag is FreeBusyItem freeBusyItem)
			{
				if (!this.ViewModel.FreeBusyAttendees.Any(fba => fba.FreeBusyItems.Contains(freeBusyItem)))
				{
					toRemove.Add(rectangle);
				}
			}
			else if (rectangle.Tag is FreeBusyGap freeBusyGap)
			{
				if (!this.ViewModel.FreeBusyGaps.Contains(freeBusyGap))
				{
					toRemove.Add(rectangle);
				}
			}
			else
			{
				throw new InvalidOperationException();
			}
		}

		foreach (Rectangle rectangle in toRemove)
		{
			this.PART_AvailabilityCanvas.Children.Remove(rectangle);
			this._availabilityBlocks.Remove(rectangle);
		}

		List<FrameworkElement> toAdd = new List<FrameworkElement>();

		for (int i = 0; i < this.ViewModel.FreeBusyAttendees.Count; ++i)
		{
			FreeBusyAttendee freeBusyAttendee = this.ViewModel.FreeBusyAttendees[i];

			foreach (FreeBusyItem freeBusyItem in freeBusyAttendee.FreeBusyItems)
			{
				if (!this._availabilityBlocks.Any(r => r.Tag is FreeBusyItem fbi && object.ReferenceEquals(fbi, freeBusyItem)))
				{
					Color color = this.ActualTheme == ElementTheme.Light ? this._lightLabelTextColorLight : this._lightLabelTextColorDark;

					Border border = new Border()
					{
						Tag = freeBusyItem,
						Background = new SolidColorBrush(color),
					};

					ToolTipService.SetToolTip(border, $"{freeBusyAttendee.AttendeeId} - {freeBusyItem.Title} ({freeBusyItem.Type})");

					if (freeBusyItem.Type == FreeBusyItemType.FreeBusyItemTypeUserNotFound)
					{
						border.Background = this.GetNotFoundBrush();
					}
					else if (freeBusyItem.Type == FreeBusyItemType.FreeBusyItemTypeUnavailable)
					{
						border.Background = this.GetUnavailableBrush();
					}
					else
					{
						if (freeBusyItem.Type == FreeBusyItemType.FreeBusyItemTypeTentative)
						{
							border.Opacity = 0.65;
						}
						else if (freeBusyItem.Type == FreeBusyItemType.FreeBusyItemTypeWorkingElsewhere)
						{
							border.Background = this.GetUnavailableBrush();
							border.Opacity = 0.5;
						}

						border.CornerRadius = new CornerRadius(2);
					}

					if (i != 0 && freeBusyItem.Type == FreeBusyItemType.FreeBusyItemTypeBusy)
					{
						border.Child = new Border()
						{
							Background = new SolidColorBrush(Colors.Red),
							Clip = new RectangleGeometry()
						};
					}

					Canvas.SetZIndex(border, i == 0 ? _organizerFreeBusyZIndex : _attendeesFreeBusyZIndex);

					toAdd.Add(border);
				}
			}
		}

		foreach (FreeBusyGap freeBusyGap in this.ViewModel.FreeBusyGaps)
		{
			if (!this._availabilityBlocks.Any(r => r.Tag is FreeBusyGap fbg && object.ReferenceEquals(fbg, freeBusyGap)))
			{
				Rectangle rectangle = new Rectangle()
				{
					StrokeDashArray = new DoubleCollection() { 0.5, 1.0, 0.3 },
					StrokeThickness = 1,
					RadiusX = 2,
					RadiusY = 2,
					Stroke = new SolidColorBrush(this.EventColor) { Opacity = 1 },
					Fill = new SolidColorBrush(this.EventColor) { Opacity = 0.1 },
					Tag = freeBusyGap
				};

				Canvas.SetZIndex(rectangle, _freeBusyGapZIndex);

				toAdd.Add(rectangle);
			}
		}

		foreach (FrameworkElement rectangle in toAdd)
		{
			this._availabilityBlocks.Add(rectangle);
			this.PART_AvailabilityCanvas.Children.Add(rectangle);
		}

		this.SyncAvailabilityBlockPositions();
	}

	private void UserControl_Loaded(object sender, RoutedEventArgs e)
	{
		this.SyncAvailabilityBlocks();
	}

	private void UserControl_Unloaded(object sender, RoutedEventArgs e)
	{
	}

	private void PART_AvailabilityCanvas_SizeChanged(object? sender, SizeChangedEventArgs e)
	{
		this.SyncAvailabilityBlockPositions();
	}

	private void PART_AttendeeNames_SizeChanged(object? sender, SizeChangedEventArgs e)
	{
		this.PART_AvailabilityGridContainer.Margin = new Thickness(e.NewSize.Width, 0, 0, 0);
		this.PART_AvailabilityCanvasRow.Height = new GridLength(e.NewSize.Height, GridUnitType.Pixel);
	}
}
