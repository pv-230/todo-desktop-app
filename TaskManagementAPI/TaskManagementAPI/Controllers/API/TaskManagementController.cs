/**************************************************************************************************
File Summary:
This is the API controller for the task management web API.
**************************************************************************************************/

using Library.TaskManagementApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Controllers.Enterprise;

namespace TaskManagementAPI.Controllers.API {
  [ApiController]
  [Route("Items")]
  public class TaskManagementController : ControllerBase {
    private readonly DataContext _db;

    public TaskManagementController(DataContext db) {
      _db = db;
    }

    [HttpGet("Test")]
    public string Test() {
      return "Hello!";
    }

    [HttpGet("GetAll")]
    public ActionResult<List<Item>> Get() {
      return Ok(new TaskManagementEC().Get(_db));
    }

    [HttpPost("AddOrEdit")]
    public ActionResult<Item> AddOrEdit([FromBody]Item item) {
      if (item == null) {
        return BadRequest();
      } else {
        return Ok(new TaskManagementEC().AddOrEdit(_db, item));
      }
    }

    [HttpPost("Delete")]
    public ActionResult<Item> Delete([FromBody]int id) {
      return Ok(new TaskManagementEC().Delete(_db, id));
    }
  }
}
