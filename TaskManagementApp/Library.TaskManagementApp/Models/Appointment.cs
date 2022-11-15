/**************************************************************************************************
File Summary:
This class represents an appointment item.
**************************************************************************************************/

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Specialized;

namespace Library.TaskManagementApp.Models {
  public class Appointment : Item, INotifyPropertyChanged {

    /************
     *  Events  *
     ************/

    public event PropertyChangedEventHandler PropertyChanged;

    /************
     *  Fields  *
     ************/

    private string _name;
    private string _description;
    private DateTimeOffset _startDate;
    private TimeSpan _startTime;
    private DateTimeOffset _stopDate;
    private TimeSpan _stopTime;
    private bool _isCompleted;
    private int _priority;
    private int _id;

    /****************
     *  Properties  *
     ****************/

    [NotMapped]
    public ObservableCollection<string> Attendees { get; set; }

    public string AttendeesStr { get; set; }

    public override string Name {
      get {
        return _name;
      }

      set {
        _name = value;
        NotifyPropertyChanged();
      }
    }

    public override string Description {
      get {
        return _description;
      }

      set {
        _description = value;
        NotifyPropertyChanged();
      }
    }

    public DateTimeOffset StartDate {
      get {
        return _startDate;
      }

      set {
        _startDate = value;
        NotifyPropertyChanged();
      }
    }

    public TimeSpan StartTime {
      get {
        return _startTime;
      }

      set {
        _startTime = value;
        NotifyPropertyChanged();
      }
    }

    public DateTimeOffset StopDate {
      get {
        return _stopDate;
      }

      set {
        _stopDate = value;
        NotifyPropertyChanged();
      }
    }

    public TimeSpan StopTime {
      get {
        return _stopTime;
      }

      set {
        _stopTime = value;
        NotifyPropertyChanged();
      }
    }

    public override bool IsCompleted {
      get {
        return _isCompleted;
      }

      set {
        _isCompleted = value;
        NotifyPropertyChanged();
      }
    }

    public override int Priority {
      get {
        return _priority;
      }

      set {
        _priority = value;
        NotifyPropertyChanged();
      }
    }

    [Key]
    public override int Id {
      get {
        return _id;
      }

      set {
        _id = value;
        NotifyPropertyChanged();
      }
    }

    [NotMapped]
    public override DateTimeOffset SortableDate => StartDate.Date.Add(StartTime);

    /******************
     *  Constructors  *
     ******************/

    public Appointment() {
      Name = string.Empty;
      Description = string.Empty;
      StartDate = DateTimeOffset.Now.Date;
      StartTime = TimeSpan.Zero;
      StopDate = DateTimeOffset.Now.Date;
      StopTime = TimeSpan.Zero;
      IsCompleted = false;
      Priority = 1;
      Attendees = new ObservableCollection<string>();
      AttendeesStr = string.Empty;
      Attendees.CollectionChanged += Attendees_CollectionChanged;
    }

    

    public Appointment(Appointment appt) {
      Name = appt.Name;
      Description = appt.Description;
      StartDate = appt.StartDate;
      StartTime = appt.StartTime;
      StopDate = appt.StopDate;
      StopTime = appt.StopTime;
      IsCompleted = appt.IsCompleted;
      Priority = appt.Priority;
      Attendees = new ObservableCollection<string>(appt.Attendees);
      AttendeesStr = appt.AttendeesStr;
      Id = appt.Id;
      Attendees.CollectionChanged += Attendees_CollectionChanged;
    }

    /*************
     *  Methods  *
     *************/

    // Displays all the properties of an appointment on separate lines.
    public override string ToString() {
      string str = $"Appointment priority: {Priority}\n";
      str += $"Name: {Name}\n";
      str += $"Description: {Description}\n";
      str += $"Start time: {StartDate:D}";
      str += $" at {DateTime.Parse(StartTime.ToString()).ToShortTimeString()}\n";
      str += $"Stop time: {StopDate:D}";
      str += $" at {DateTime.Parse(StopTime.ToString()).ToShortTimeString()}\n";
      str += "Attendees: " + string.Join(", ", Attendees) + "\n";
      str += "Appointment has " + (IsCompleted ? "finished." : "not finished.");
      return str;
    }

    /********************
     *  Event Handlers  *
     ********************/

    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Any changes to the Attendees collection also updates the AttendeeStr property.
    private void Attendees_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
      AttendeesStr = string.Empty;
      foreach (var attendee in Attendees) {
        AttendeesStr += attendee + ",";
      }
      AttendeesStr = AttendeesStr.TrimEnd(',');
    }
  }
}
