using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Windows.Foundation;

#nullable enable

namespace App1;

public sealed partial class BlankPage1 : Page
{
    private readonly ObservableCollection<IntegerGroup> IntegerGroups = new ObservableCollection<IntegerGroup>();
    private readonly Random Random = new Random();

    // How many groups we keep before/after the initial or scrolled-to date
    private readonly int _loadBufferSize = 50;
    // When we scroll, we remove this many groups from the end and add to the beginning
    private readonly int _scrollBufferSize = 10;
    private ScrollViewer? _scrollViewer;

    // private readonly DispatcherTimer _dispatcherTimer;
    private readonly Timer _timer;

    public BlankPage1()
    {
        this.InitializeComponent();
        CollectionViewSource collectionViewSource = new CollectionViewSource();
        collectionViewSource.IsSourceGrouped = true;
        collectionViewSource.ItemsPath = new PropertyPath(nameof(IntegerGroup.Collection));
        collectionViewSource.Source = IntegerGroups;

        this.GroupsList.ItemsSource = collectionViewSource.View;
        GroupsList.LayoutUpdated += GroupsList_LayoutUpdated;
        GroupsList.Loaded += GroupsList_Loaded;

        AutoResetEvent evt = new AutoResetEvent(false);
        Random addRemoveRandom = new Random();
        _timer = new Timer(state =>
        {
            int itemsCount = _loadBufferSize * 2;
            bool?[] addRemoves = new bool?[itemsCount];
            for (int i = 0; i < _loadBufferSize * 2; ++i)
            {
                if (Random.NextDouble() <= 0.50)
                {
                    if (Random.NextDouble() > 0.5)
                    {
                        addRemoves[i] = true;
                    }
                    else
                    {
                        addRemoves[i] = false;
                    }
                }
                else
                {
                    addRemoves[i] = null;
                }
            }


            if (DispatcherQueue.TryEnqueue(() =>
            {
                for (int i = 0; i < addRemoves.Length; ++i)
                {
                    if (i >= IntegerGroups.Count)
                    {
                        break;
                    }

                    bool? addRemove = addRemoves[i];
                    if (addRemove is null)
                    {
                        continue;
                    }

                    IntegerGroup group = IntegerGroups[i];

                    if (addRemove.Value)
                    {
                        group.Collection.Add(new Integer(group.Collection[group.Collection.Count - 1].Value + 1));
                    }
                    else if (group.Collection.Count > 2)
                    {
                        group.Collection.RemoveAt(group.Collection.Count - 1);
                    }
                }

                evt.Set();
            }))
            {
                evt.WaitOne();
            }
        });
        _timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(1));
        // _dispatcherTimer.Tick += DispatcherTimer_Tick;
        // _dispatcherTimer.Start();
    }

    private void DispatcherTimer_Tick(object? sender, object e)
    {

        foreach (IntegerGroup group in this.IntegerGroups)
        {
            if (Random.NextDouble() > 0.75)
            {
                if (Random.NextDouble() > 0.5)
                {
                    group.Collection.Add(new Integer(group.Collection[group.Collection.Count - 1].Value + 1));
                }
                else if (group.Collection.Count > 2)
                {
                    group.Collection.RemoveAt(group.Collection.Count - 1);
                }
            }
        }
    }

    private void GroupsList_LayoutUpdated(object? sender, object e)
    {
        GroupsList.LayoutUpdated -= GroupsList_LayoutUpdated;
        _scrollViewer = GetFirstChildOfType<ScrollViewer>(GroupsList) ?? throw new InvalidOperationException();
        GroupsList.LayoutUpdated += GroupsList_LayoutUpdated2;
        _scrollViewer.ViewChanging += ScrollViewer_ViewChanging;
        _scrollViewer.ViewChanged += ScrollViewer_ViewChanged;
    }

    private void GroupsList_LayoutUpdated2(object? sender, object e) => UpdateScrollViewer();

    private void ScrollViewer_ViewChanging(object? sender, ScrollViewerViewChangingEventArgs e) => UpdateScrollViewer();

    private void ScrollViewer_ViewChanged(object? sender, ScrollViewerViewChangedEventArgs e)
    {
        if (_scrollViewer is null)
        {
            // ???
            return;
        }

        if (e.IsIntermediate)
        {
            return;
        }

        UpdateScrollViewer();
    }

    private void UpdateScrollViewer()
    {
        if (IntegerGroups.Count <= _scrollBufferSize)
        {
            return;
        }

        FrameworkElement? topElement = null;
        for (int i = 0; i < _scrollBufferSize; ++i)
        {
            IntegerGroup group = IntegerGroups[_scrollBufferSize - 1 - i];
            for (int j = 0; j < group.Collection.Count; ++j)
            {
                if (GroupsList.ContainerFromItem(group.Collection[group.Collection.Count - 1 - j]) is FrameworkElement elt)
                {
                    topElement = elt;
                    break;
                }
            }

            if (topElement is not null)
            {
                break;
            }
        }

        FrameworkElement? bottomElement = null;
        for (int i = 0; i < _scrollBufferSize; ++i)
        {
            IntegerGroup group = IntegerGroups[IntegerGroups.Count - 1 - _scrollBufferSize + i];
            for (int j = 0; j < group.Collection.Count; ++j)
            {
                if (GroupsList.ContainerFromItem(group.Collection[j]) is FrameworkElement elt)
                {
                    bottomElement = elt;
                    break;
                }
            }

            if (bottomElement is not null)
            {
                break;
            }
        }

        // Y positions relative to the top of the scrollViewer port
        // eg -1 is above the port and not visible
        double topY = 0;
        double bottomY = double.MaxValue;

        if (topElement is not null)
        {
            // DependencyObject? topGroupContainer = GroupsList.GroupHeaderContainerFromItemContainer(topElement);
            GeneralTransform topTransform = topElement.TransformToVisual(_scrollViewer);
            Point topPoint = topTransform.TransformPoint(new Point(0, 0));
            topY = topPoint.Y;
        }

        if (bottomElement is not null)
        {
            // DependencyObject? bottomGroupContainer = GroupsList.GroupHeaderContainerFromItemContainer(bottomElement);
            GeneralTransform bottomTransform = bottomElement.TransformToVisual(_scrollViewer);
            double bottomGroupHeight = double.IsNaN(bottomElement.ActualHeight) ? 0 : bottomElement.ActualHeight;
            Point bottomPoint = bottomTransform.TransformPoint(new Point(0, bottomGroupHeight));
            bottomY = bottomPoint.Y;
        }

        // Debug.WriteLine($"Top/Bottom: {topY}/{bottomY}");
        if (0 < topY)
        {
            // We scrolled past the top elt
            for (int i = 0; i < _scrollBufferSize; ++i)
            {
                IntegerGroups.Insert(0, MakeGroup(IntegerGroups[0].GroupNumber - 1));
                IntegerGroups.RemoveAt(IntegerGroups.Count - 1);
            }
        }
        else if (bottomY <= 0)
        {
            // We scrolled past the bottom elt
            for (int i = 0; i < _scrollBufferSize; ++i)
            {
                IntegerGroups.Add(MakeGroup(IntegerGroups[IntegerGroups.Count - 1].GroupNumber + 1));
                IntegerGroups.RemoveAt(0);
            }
        }

        // int firstVisibleGroupIdx = int.MaxValue;
        // for (int i = 0; i < _scrollBufferSize; ++i)
        // {
        //     IntegerGroup group = IntegerGroups[i];
        //     if (GroupsList.ContainerFromItem(group.Collection[0]) is FrameworkElement elt && elt.IsLoaded)
        //     {
        //         firstVisibleGroupIdx = i;
        //         break;
        //     }
        // }
        // 
        // int lastVisibleGroupIdx = 0;
        // for (int i = 0; i < _scrollBufferSize; ++i)
        // {
        //     int idx = IntegerGroups.Count - 1 - i;
        //     IntegerGroup group = IntegerGroups[idx];
        //     if (GroupsList.ContainerFromItem(group.Collection[group.Collection.Count - 1]) is FrameworkElement elt && elt.IsLoaded)
        //     {
        //         lastVisibleGroupIdx = idx;
        //         break;
        //     }
        // }
        // 
        // if (firstVisibleGroupIdx < _scrollBufferSize)
        // {
        //     // We scrolled past the top elt
        //     for (int i = 0; i < _scrollBufferSize; ++i)
        //     {
        //         IntegerGroups.Insert(0, MakeGroup(IntegerGroups[0].GroupNumber - 1));
        //         IntegerGroups.RemoveAt(IntegerGroups.Count - 1);
        //     }
        // }
        // else if (lastVisibleGroupIdx >= IntegerGroups.Count - 1 - _scrollBufferSize)
        // {
        //     // We scrolled past the bottom elt
        //     for (int i = 0; i < _scrollBufferSize; ++i)
        //     {
        //         IntegerGroups.Add(MakeGroup(IntegerGroups[IntegerGroups.Count - 1].GroupNumber + 1));
        //         IntegerGroups.RemoveAt(0);
        //     }
        // }
    }

    public static T? GetFirstChildOfType<T>(DependencyObject dependencyObject) where T : DependencyObject
    {
        if (dependencyObject == null)
        {
            return null;
        }

        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
        {
            var child = VisualTreeHelper.GetChild(dependencyObject, i);

            var result = (child as T) ?? GetFirstChildOfType<T>(child);

            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    private void GroupsList_Loaded(object sender, RoutedEventArgs e) => DoScrollToGroup(0);

    private IntegerGroup MakeGroup(int groupKey)
    {
        int groupSize = Random.Next(5) + 1;

        IntegerGroup group = new IntegerGroup(groupKey);
        for (int i = 0; i < groupSize; ++i)
        {
            group.Collection.Add(new Integer(i));
        }

        return group;
    }

    private void DoScrollToGroup(int groupKey)
    {
        Debug.WriteLine($"Scrolling to group '{groupKey}'");

        // IntegerGroup firstGroup = IntegerGroups[0];
        // IntegerGroup lastGroup = IntegerGroups[IntegerGroups.Count - 1];
        // 
        // if (groupKey < firstGroup.WhateverIWant)
        // {
        //     
        //     for (int i = firstGroup.WhateverIWant; i >= groupKey; --i)
        //     {
        //     }
        // }
        IntegerGroup? group = IntegerGroups.FirstOrDefault(ig => ig.GroupNumber == groupKey);

        if (group is null)
        {
            while (IntegerGroups.Count > 0)
            {
                IntegerGroups.RemoveAt(0);
            }

            for (int i = -_loadBufferSize; i < _loadBufferSize; ++i)
            {
                IntegerGroups.Add(MakeGroup(i + groupKey));
            }

            group = IntegerGroups.FirstOrDefault(ig => ig.GroupNumber == groupKey);
        }

        if (group != null)
        {
            GroupsList.ScrollIntoView(group, ScrollIntoViewAlignment.Leading);
        }
    }

    private void MaybeDoScrollToGroup()
    {
        if (!int.TryParse(ScrollToGroupTextBox.Text, out int result))
        {
            Debug.WriteLine($"Bad number '{ScrollToGroupTextBox.Text}'");
            return;
        }

        DoScrollToGroup(result);
    }

    private void Button_Click(object sender, RoutedEventArgs e) => MaybeDoScrollToGroup();

    private void StackPanel_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            MaybeDoScrollToGroup();
            e.Handled = true;
        }
    }
}

public class IntegerGroup
{
    public ObservableCollection<Integer> Collection { get; } = new ObservableCollection<Integer>();

    public IntegerGroup(int key)
    {
        this.GroupNumber = key;
    }

    public int GroupNumber { get; private set; }

    public override string ToString()
    {
        return "Group " + GroupNumber.ToString();
    }
}

public class Integer
{
    public int Value { get; private set; }
    public Integer(int value)
    {
        Value = value;
    }
}
