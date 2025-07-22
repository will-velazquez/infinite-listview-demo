using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;

namespace Fantastical.App.Pages;

internal sealed class Person
{
    public string? Name { get; set; }
}

internal sealed partial class ItemsRepeaterBugTest : Page
{
    public static void SyncToCollection<TDst, TSrc>(ObservableCollection<TDst> observableCollection, IList<TSrc> orderedItems, Func<TDst, TSrc, bool> comparer, Action<ObservableCollection<TDst>, int, TSrc> updater, Func<TSrc, TDst> constructor)
    {
        // Shuffle around and add new ones
        for (int i = 0; i < orderedItems.Count; ++i)
        {
            TSrc newItem = orderedItems[i];
            int existingIdx = -1;

            for (int j = 0; j < observableCollection.Count; ++j)
            {
                if (comparer(observableCollection[j], newItem))
                {
                    existingIdx = j;
                    break;
                }
            }

            if (existingIdx >= 0)
            {
                // Shuffle it around
                if (existingIdx != i)
                {
                    observableCollection.Move(existingIdx, i);
                }

                // Sync its new status
                updater(observableCollection, i, newItem);
            }
            else
            {
                TDst newValue = constructor(newItem);

                observableCollection.Insert(i, newValue);
            }
        }

        // Remove any leftovers
        for (int i = observableCollection.Count - 1; i >= orderedItems.Count; --i)
        {
            observableCollection.RemoveAt(i);
        }
    }

    private ObservableCollection<Person> People { get; set; } = [];
    public ItemsRepeaterBugTest()
    {
        this.InitializeComponent();

        this.People.Insert(0, new Person
        {
            Name = "Foo",
        });

        this.People.Insert(1, new Person
        {
            Name = "Bar",
        });
    }

    private void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        string[] names;

        if (People.Count == 3)
        {
            names = ["Foo", "Bar"];
        }
        else
        {
            names = ["Organizer", "Foo", "Bar"];
        }

        SyncToCollection(this.People, names, (Person p, string name) => p.Name == name, (ObservableCollection<Person> collection, int index, string value) => { }, (string name) => new Person { Name = name });
    }
}
