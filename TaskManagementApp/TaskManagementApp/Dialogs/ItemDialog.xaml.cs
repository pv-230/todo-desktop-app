/**************************************************************************************************
File Summary:
This is the code-behind for the add/edit item dialog.
**************************************************************************************************/

using Library.TaskManagementApp.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace TaskManagementApp.Dialogs {
  public sealed partial class ItemDialog : ContentDialog {

    /************
     *  Fields  *
     ************/

    private readonly IList<Item> _items;  // Stores reference to items list from MainViewModel.

    /****************
     *  Properties  *
     ****************/

    public string SelectedAttendee { get; set; }

    /******************
     *  Constructors  *
     ******************/

    // Constructor used for adding new items.
    public ItemDialog(IList<Item> items) {
      InitializeComponent();
      DataContext = new Task();  // Task is selected by default.
      TaskRb.IsChecked = true;
      _items = items;
      SelectedAttendee = string.Empty;
    }

    // Constructor used for editing existing items.
    public ItemDialog(IList<Item> items, Item itemToEdit) {
      InitializeComponent();

      // Changes the look of the dialog for editing.
      Title = "Edit Item";
      PrimaryButtonText = "Edit";
      TaskRb.Visibility = Visibility.Collapsed;
      ApptRb.Visibility = Visibility.Collapsed;
      IsPrimaryButtonEnabled = true;
      IsCompletedCheckbox.Visibility = Visibility.Visible;

      // Changes the data context to the item to edit and displays the appropriate controls.
      if (itemToEdit is Task taskItem) {
        DataContext = new Task(taskItem);
        TaskDate.Visibility = Visibility.Visible;
        ApptStartDate.Visibility = Visibility.Collapsed;
      } else if (itemToEdit is Appointment apptItem) {
        DataContext = new Appointment(apptItem);
        TaskDate.Visibility = Visibility.Collapsed;
        ApptStartDate.Visibility = Visibility.Visible;
        AttendeesList.Visibility = Visibility.Visible;
      }

      _items = items;
      SelectedAttendee = string.Empty;
    }

    /********************
     *  Event Handlers  *
     ********************/

    // Performs the relevant action of adding or editing when the primary button is clicked.
    private async void ContentDialog_PrimaryButtonClick(ContentDialog sender,
                                                  ContentDialogButtonClickEventArgs args) {
      Item currentItem = null;

      // Adds or edits the item on the server side.
      if (PrimaryButtonText == "Add") {
        currentItem = JsonConvert.DeserializeObject<Item>(
          await new WebRequestHandler().Post("http://localhost/TaskManagementAPI/Items/AddOrEdit",
            DataContext as Item));
      } else if (PrimaryButtonText == "Edit") {
        currentItem = JsonConvert.DeserializeObject<Item>(
          await new WebRequestHandler().Post("http://localhost/TaskManagementAPI/Items/AddOrEdit",
            DataContext as Item));
      }

      // Adds or edits the item on the client side.
      if (currentItem != null) {
        var index = _items.IndexOf(_items.FirstOrDefault(i => i.Id.Equals(currentItem.Id)));
        if (index < 0) {
          _items.Add(currentItem);
        } else {
          _items.RemoveAt(index);
          _items.Insert(index, currentItem);
        }
      }
    }

    // Cancel button.
    private void ContentDialog_SecondaryButtonClick(ContentDialog sender,
                                                    ContentDialogButtonClickEventArgs args) {
    }

    // Controls the data context based on two radio buttons that indicate the item type.
    private void Rb_ItemTypeSelected(object sender, RoutedEventArgs e) {
      if ((bool)TaskRb.IsChecked && !(DataContext is Task)) {
        DataContext = new Task();
        TaskDate.Visibility = Visibility.Visible;
        ApptStartDate.Visibility = Visibility.Collapsed;
      } else if ((bool)ApptRb.IsChecked && !(DataContext is Appointment)) {
        DataContext = new Appointment();
        TaskDate.Visibility = Visibility.Collapsed;
        ApptStartDate.Visibility = Visibility.Visible;
      }
    }

    // Disables the primary dialog button if the name textbox is empty.
    private void NameTextbox_TextChanged(object sender, TextChangedEventArgs e) {
      if (IsPrimaryButtonEnabled && NameTextbox.Text.Length == 0) {
        IsPrimaryButtonEnabled = false;
      } else if (!IsPrimaryButtonEnabled) {
        IsPrimaryButtonEnabled = true;
      }
    }

    // Allows user to add attendees to the attendees list.
    private void AttendeesTextBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e) {
      if ((DataContext is Appointment appt) && e.Key == Windows.System.VirtualKey.Enter) {
        string contents = AttendeesTextBox.Text.Trim();

        if (contents.Length > 0) {
          appt.Attendees.Add(contents);
          AttendeesTextBox.Text = string.Empty;

          // Shows the attendees list when it is populated.
          if (AttendeesList.Visibility == Visibility.Collapsed) {
            AttendeesList.Visibility = Visibility.Visible;
          }
        }
      }
    }

    // Allows user to remove attendees by selecting a name from the list.
    private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      if (SelectedAttendee == null) { return; }

      if (DataContext is Appointment appt) {
        appt.Attendees.Remove(SelectedAttendee);

        // Hides the attendees list when it is empty.
        if (appt.Attendees.Count == 0 && AttendeesList.Visibility == Visibility.Visible) {
          AttendeesList.Visibility = Visibility.Collapsed;
        }
      }
    }
  }
}
