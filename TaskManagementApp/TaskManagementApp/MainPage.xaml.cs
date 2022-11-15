/**************************************************************************************************
File Summary:
This is the code-behind for the main page UI.
**************************************************************************************************/

using System;
using TaskManagementApp.Dialogs;
using TaskManagementApp.ViewModels;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace TaskManagementApp {
  public sealed partial class MainPage : Page {

    /******************
     *  Constructors  *
     ******************/

    public MainPage() {
      InitializeComponent();

      // Sets up preferred window size upon launch.
      ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(500, 500));
      ApplicationView.PreferredLaunchViewSize = new Size(750, 750);
      ApplicationView.PreferredLaunchWindowingMode =
        ApplicationViewWindowingMode.PreferredLaunchViewSize;

      DataContext = new MainViewModel();
      ViewToggleSwitch.IsOn = true;  // Showing only active items by default.
    }

    /********************
     *  Event Handlers  *
     ********************/

    // Displays a dialog for adding a new item.
    private async void Add_Click(object sender, RoutedEventArgs e) {
      var diag = new ItemDialog((DataContext as MainViewModel).Items);
      await diag.ShowAsync();
    }

    // Displays a dialog for editing an existing item.
    private async void Edit_Click(object sender, RoutedEventArgs e) {
      var mainViewModel = DataContext as MainViewModel;
      var diag = new ItemDialog(mainViewModel.Items, mainViewModel.SelectedItem);
      await diag.ShowAsync();
    }

    // Removes a selected item from the list.
    private void Delete_Click(object sender, RoutedEventArgs e) {
      (DataContext as MainViewModel).Remove();
    }

    // Switches between displaying all items or only active (non-completed) items.
    private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e) {
      if (ViewToggleSwitch.IsOn) {
        (DataContext as MainViewModel).FilterItems(MainViewModel.Filters.Active);
      } else {
        (DataContext as MainViewModel).FilterItems(MainViewModel.Filters.All);
      }
    }

    // Displays the items matching the search criteria given in the text box.
    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) {
      var mainViewModel = DataContext as MainViewModel;

      if (SearchBox.Text.Length == 0) {
        // Ensures the displayed list is sorted when emptying out the search box.
        mainViewModel.Sort(MainViewModel.Sorts.Current);
      } else {
        mainViewModel.SearchItems(SearchBox.Text.Trim());
      }
    }

    // Sets how the list should be sorted.
    private void SortButton_Click(object sender, RoutedEventArgs e) {
      var mainViewModel = DataContext as MainViewModel;
      var sortButton = sender as MenuFlyoutItem;

      if (sortButton.Text == "Name") {
        mainViewModel.Sort(MainViewModel.Sorts.ByName);
      } else if (sortButton.Text == "Date") {
        mainViewModel.Sort(MainViewModel.Sorts.ByDate);
      } else if (sortButton.Text == "Priority") {
        mainViewModel.Sort(MainViewModel.Sorts.ByPriority);
      }
    }

    //// Opens the relevant file picker when the save/load button is clicked and allows a user to
    //// save their task/appointment list. Default location for file saving is in Documents. Code
    //// pattern was taken from the following URLs:
    //// https://docs.microsoft.com/en-us/windows/uwp/files/quickstart-save-a-file-with-a-picker
    //// https://docs.microsoft.com/en-us/windows/uwp/files/quickstart-using-file-and-folder-pickers
    //private async void FileButton_Click(object sender, RoutedEventArgs e) {
    //  var mainViewModel = DataContext as MainViewModel;
    //  var fileButton = sender as MenuFlyoutItem;

    //  if (fileButton.Text == "Save") {
    //    // Sets up the save file picker.
    //    var picker = new Windows.Storage.Pickers.FileSavePicker();
    //    picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
    //    picker.FileTypeChoices.Add("JSON File (*.json)", new List<string> { ".json" });

    //    // Creates the save file.
    //    StorageFile saveFile = await picker.PickSaveFileAsync();

    //    // Writes to the save file.
    //    if (saveFile != null) {
    //      CachedFileManager.DeferUpdates(saveFile);
    //      await FileIO.WriteTextAsync(saveFile, mainViewModel.SerializeItems());
    //      await CachedFileManager.CompleteUpdatesAsync(saveFile);
    //    }
    //  } else if (fileButton.Text == "Load") {
    //    // Sets up the load file picker.
    //    var picker = new Windows.Storage.Pickers.FileOpenPicker();
    //    picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
    //    picker.FileTypeFilter.Add(".json");

    //    // Loads the save file.
    //    StorageFile saveFile = await picker.PickSingleFileAsync();

    //    // Reads from the selected save file.
    //    if (saveFile != null) {
    //      mainViewModel.DeserializeItems(await FileIO.ReadTextAsync(saveFile));
    //    }
    //  }
    //}
  }
}
