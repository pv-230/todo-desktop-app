/**************************************************************************************************
File Summary:
This is DbContext for the entity framework setup with the task management web API.
**************************************************************************************************/

using Library.TaskManagementApp.Models;
using Microsoft.EntityFrameworkCore;

namespace TaskManagementAPI {
  public partial class DataContext : DbContext {
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public virtual DbSet<Task> Tasks { get; set; }
    public virtual DbSet<Appointment> Appointments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
      modelBuilder.Entity<Item>().ToTable("Items");
    }
  }
}
