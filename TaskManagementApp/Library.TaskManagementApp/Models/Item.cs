/**************************************************************************************************
File Summary:
The abstract Item class is the base class for tasks and appointments.
**************************************************************************************************/

using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.TaskManagementApp.Models {
  [JsonConverter(typeof(ItemJsonConverter))]
  public abstract class Item {

    /****************
     *  Properties  *
     ****************/

    public abstract string Name { get; set; }

    public abstract string Description { get; set; }

    public abstract bool IsCompleted { get; set; }

    public abstract int Priority { get; set; }

    [Key]
    public abstract int Id { get; set; }

    [NotMapped]
    public abstract DateTimeOffset SortableDate { get; }
  }
}
