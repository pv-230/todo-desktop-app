/**************************************************************************************************
File Summary:
This is the main view model for the task management app.
**************************************************************************************************/

using Library.TaskManagementApp.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System;

namespace TaskManagementApp.ViewModels {
  public class MainViewModel : INotifyPropertyChanged {

    /******************
     *  Enumerations  *
     ******************/

    public enum Filters {
      All,
      Active
    }

    public enum Sorts {
      Current,
      ByName,
      ByDate,
      ByPriority
    }

    /************
     *  Events  *
     ************/

    public event PropertyChangedEventHandler PropertyChanged;

    /************
     *  Fields  *
     ************/

    private Item _selectedItem;
    private bool _hasSelection;
    private Filters _currentFilter;
    private Sorts _currentSort;

    /****************
     *  Properties  *
     ****************/

    // Contains all the items that the user has added/loaded.
    public ObservableCollection<Item> Items { get; set; }

    // Binds to the main list display in the main page UI.
    public ObservableCollection<Item> FilteredItems { get; set; }

    // Holds the currently selected item.
    public Item SelectedItem {
      get => _selectedItem;
      set {
        HasSelection = value != null;
        _selectedItem = value;
      }
    }

    // Returns true if there is an item that is currently selected.
    public bool HasSelection {
      get => _hasSelection;
      set {
        _hasSelection = value;
        NotifyPropertyChanged();
      }
    }

    /******************
     *  Constructors  *
     ******************/

    public MainViewModel() {
      Items = new ObservableCollection<Item>();
      Items.CollectionChanged += Items_CollectionChanged;
      FilteredItems = new ObservableCollection<Item>();
      SelectedItem = null;  // Also sets HasSelection to false.
      _currentFilter = Filters.Active;
      _currentSort = Sorts.ByPriority;

      // Retrieves items using the API.
      var tempItems = JsonConvert.DeserializeObject<List<Item>>(
        new WebRequestHandler().Get("http://localhost/TaskManagementAPI/Items/GetAll").Result);
      tempItems.ForEach(Items.Add);
    }

    /*************
     *  Methods  *
     *************/

    // Removes the selected item from the items list.
    public async void Remove() {
      var itemToDelete = JsonConvert.DeserializeObject<Item>(
        await new WebRequestHandler().Post("http://localhost/TaskManagementAPI/Items/Delete",
          SelectedItem.Id));
      Items.Remove(Items.FirstOrDefault(i => i.Id.Equals(itemToDelete.Id)));
    }

    // Filters the items based on the type of filter.
    public void FilterItems(Filters filter) {
      // Sets the current filter if needed.
      if (_currentFilter != filter) {
        _currentFilter = filter;
      }

      // Clears and repopulates the filtered items list according to the current filter.
      FilteredItems.Clear();
      if (_currentFilter == Filters.All) {
        foreach (var item in Items) {
          FilteredItems.Add(item);
        }
      } else if (filter == Filters.Active) {
        foreach (var item in Items.Where(i => !i.IsCompleted)) {
          FilteredItems.Add(item);
        }
      }
    }

    // Populates the filtered items list with items that match the search term. This method is
    // based on a method found in the following URL:
    // https://docs.microsoft.com/en-us/windows/apps/design/controls/listview-filtering
    public void SearchItems(string term) {
      List<Item> tempList = null;

      // Creates a temporary list of items that match the current filter.
      if (_currentFilter == Filters.All) {
        // tempList will contain all items that match the search term.
        tempList = Items.Where(i => i.Name.Contains(term, (StringComparison)3) ||
                                    i.Description.Contains(term, (StringComparison)3) ||
                                    (i is Appointment appt && appt.Attendees.Any(
                                      a => a.Contains(term, (StringComparison)3)))).ToList();
      } else if (_currentFilter == Filters.Active) {
        // tempList will contain only active items that match the search term.
        tempList = Items.Where(i => (i.Name.Contains(term, (StringComparison)3) ||
                                     i.Description.Contains(term, (StringComparison)3) ||
                                     (i is Appointment appt && appt.Attendees.Any(
                                       a => a.Contains(term, (StringComparison)3)))) &&
                                     i.IsCompleted == false).ToList();
      }

      if (tempList == null) { return; }

      // Removes any items from the filtered items list that are not present in the temp list.
      for (int i = 0; i < FilteredItems.Count; i++) {
        var item = FilteredItems[i];
        if (!tempList.Contains(item)) {
          FilteredItems.Remove(item);
        }
      }

      // Adds back any items that are in the temp list but not in the filtered items list.
      foreach (var item in tempList) {
        if (!FilteredItems.Contains(item)) {
          FilteredItems.Add(item);
        }
      }
    }

    // Sorts the list according the to type of sort passed in.
    public void Sort(Sorts sort) {
      IOrderedEnumerable<Item> sortedItems = null;
      List<Item> tempList = new List<Item>(Items);

      if (sort != Sorts.Current && _currentSort != sort) {
        _currentSort = sort;
      }

      if (_currentSort == Sorts.ByName) {
        sortedItems = tempList.OrderBy(i => i.Name);
      } else if (_currentSort == Sorts.ByDate) {
        sortedItems = tempList.OrderBy(i => i.SortableDate);
      } else if (_currentSort == Sorts.ByPriority) {
        sortedItems = tempList.OrderBy(i => i.Priority);
      }

      if (sortedItems != null) {
        Items.CollectionChanged -= Items_CollectionChanged;  // Prevents excessive event handling.
        Items.Clear();
        foreach (Item item in sortedItems) {
          Items.Add(item);  // All items are added back to the main items list in the proper order.
        }
        Items.CollectionChanged += Items_CollectionChanged;
        FilterItems(_currentFilter);
      }
    }

    // Serializes the items collection and returns the json string result. Code pattern obtained
    // from the following URL:
    // https://www.newtonsoft.com/json/help/html/SerializeTypeNameHandling.htm
    public string SerializeItems() {
      return JsonConvert.SerializeObject(Items, new JsonSerializerSettings {
        TypeNameHandling = TypeNameHandling.Auto
      });
    }

    // Deserializes a json string into a new observable collection that will become the new items
    // collection. Code pattern obtained from the following URL:
    // https://www.newtonsoft.com/json/help/html/SerializeTypeNameHandling.htm
    public void DeserializeItems(string saveFileContents) {
      Items = JsonConvert.DeserializeObject<ObservableCollection<Item>>(saveFileContents,
        new JsonSerializerSettings {
          TypeNameHandling = TypeNameHandling.Auto
        });
      Items.CollectionChanged += Items_CollectionChanged;
      Sort(_currentSort);
    }

    /********************
     *  Event Handlers  *
     ********************/

    // Updates the filtered items list each time the main items list is changed.
    private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
      Sort(_currentSort);
    }

    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
