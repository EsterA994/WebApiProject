using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using MyJewelry.Services;
using MyJewelry.Models;
using MyJewelry.Interfaces;

namespace MyJewelry.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    IUserService userService;

    public UserController(IUserService userService)
    {
        this.userService = userService;
    }


    [HttpGet()]
    public ActionResult<IEnumerable<User>> Get()
    {
        return userService.Get();
    }

    [HttpGet("{id}")]
    public ActionResult<User> Get(int id)
    {
        var user = userService.Get(id);
        if (user == null)
            return NotFound();
        return user;
    }

    [HttpPost]
    public ActionResult Create(User newUser)
    {
        var postedUser = userService.Create(newUser);
        return CreatedAtAction(nameof(Create), new { id = postedUser.Id });
    }

    [HttpPut("{id}")]
    public ActionResult Update(int id, User newUser)
    {
        var user = userService.find(id);
        if (user == null)
            return NotFound();
        if (user.Id != newUser.Id)
            return BadRequest();
        userService.Update(id,newUser);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var user = userService.find(id);
        
        if (user == null)
            return NotFound();
        userService.Delete(id);

        return NoContent();
    }

}
