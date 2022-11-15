/**************************************************************************************************
File Summary:
This is the enterprise controller for the task management web API.
**************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Library.TaskManagementApp.Models;

namespace TaskManagementAPI.Controllers.Enterprise {
  public class TaskManagementEC {
    // Returns all items in the database as a list.
    public List<Item> Get(DataContext db) {
      var items = new List<Item>();
      items.AddRange(db.Tasks.ToList());

      // Adds all attendees in AttendeesStr to the Attendees collection.
      foreach (var appt in db.Appointments.ToList()) {
        string[] attendeesTemp = appt.AttendeesStr.Split(',');

        foreach (var a in attendeesTemp) {
          // Attendees is an observable collection and lacks AddRange method.
          appt.Attendees.Add(a);
        }
        items.Add(appt);
      }

      return items;
    }

    // Adds an item to the database if the item's id is 0, otherwise the item is updated.
    public Item AddOrEdit(DataContext db, Item item) {
      db.SaveChanges();
      if (item.Id == 0) {
        var itemTemp = db.Add(item);   // Loads a new item into the database.
        db.SaveChanges();
        item.Id = itemTemp.Entity.Id;  // Sets the id of the item to the id given by the database.
      } else {
        db.Update(item);
        db.SaveChanges();
      }
      return item;
    }

    // Removes an item from the database based on its id.
    public Item Delete(DataContext db, int id) {
      // Creates a list of all items in the database.
      var items = new List<Item>();
      items.AddRange(db.Tasks.ToList());
      items.AddRange(db.Appointments.ToList());

      // Finds the item the matches the id and removes it from the database.
      var itemToDelete = items.FirstOrDefault(i => i.Id.Equals(id));
      db.Remove(itemToDelete);
      db.SaveChanges();
      return itemToDelete;
    }
  }
}
