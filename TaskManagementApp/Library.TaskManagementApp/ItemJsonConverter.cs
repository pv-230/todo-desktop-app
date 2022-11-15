using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Library.TaskManagementApp.Models;
using Newtonsoft.Json.Linq;

namespace Library.TaskManagementApp {
  public class ItemJsonConverter : JsonCreationConverter<Item> {
    protected override Item Create(Type objectType, JObject jObject) {
      if (jObject == null) throw new ArgumentNullException("jObject");

      if (jObject["deadlineDate"] != null || jObject["DeadlineDate"] != null) {
        return new Task();
      } else if (jObject["startDate"] != null || jObject["StartDate"] != null) {
        return new Appointment();
      } else {
        return null;
      }
    }
  }
}
