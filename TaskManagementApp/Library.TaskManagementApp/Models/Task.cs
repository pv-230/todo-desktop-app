/**************************************************************************************************
File Summary:
This class represents a task item.
**************************************************************************************************/

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.TaskManagementApp.Models {
  public class Task : Item, INotifyPropertyChanged {

    /************
     *  Events  *
     ************/

    public event PropertyChangedEventHandler PropertyChanged;

    /************
     *  Fields  *
     ************/

    private string _name;
    private string _description;
    private DateTimeOffset _deadlineDate;
    private TimeSpan _deadlineTime;
    private bool _isCompleted;
    private int _priority;
    private int _id;

    /****************
     *  Properties  *
     ****************/

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

    public DateTimeOffset DeadlineDate {
      get {
        return _deadlineDate;
      }

      set {
        _deadlineDate = value;
        NotifyPropertyChanged();
      }
    }

    public TimeSpan DeadlineTime {
      get {
        return _deadlineTime;
      }

      set {
        _deadlineTime = value;
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
    public override DateTimeOffset SortableDate => DeadlineDate.Date.Add(DeadlineTime);

    /******************
     *  Constructors  *
     ******************/

    public Task() {
      Name = string.Empty;
      Description = string.Empty;
      DeadlineDate = DateTimeOffset.Now.Date;
      DeadlineTime = TimeSpan.Zero;
      IsCompleted = false;
      Priority = 1;
    }

    public Task(Task task) {
      Name = task.Name;
      Description = task.Description;
      DeadlineDate = task.DeadlineDate;
      DeadlineTime = task.DeadlineTime;
      IsCompleted = task.IsCompleted;
      Priority = task.Priority;
      Id = task.Id;
    }

    /*************
     *  Methods  *
     *************/

    // Displays all the properties of a task on separate lines.
    public override string ToString() {
      string str = $"Task priority: {Priority}\n";
      str += $"Name: {Name}\n";
      str += $"Description: {Description}\n";
      str += $"Deadline: {DeadlineDate:D}";
      str += $" at {DateTime.Parse(DeadlineTime.ToString()).ToShortTimeString()}\n";
      str += "Task is " + (IsCompleted ? "completed." : "not completed.");
      return str;
    }

    /********************
     *  Event Handlers  *
     ********************/

    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
