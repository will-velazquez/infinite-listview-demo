using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using Windows.UI;

namespace Fantastical.App.Pages;

public sealed partial class SegmentedCrashTest : Page
{
	public List<int> Numbers { get; } = [1, 2, 3];

	public Color Color { get; } = Colors.Red;

	public SegmentedCrashTest()
	{
		this.InitializeComponent();


	}
}
