using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using MyJewelry.Services;
using MyJewelry.Models;
using MyJewelry.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

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

    [HttpPost]
    [Route("[action]")]
    public ActionResult<string> Login([FromBody] LoginRequest user)
    {
        if (user.Name != "Esty" || user.Id != 1272)
        return Unauthorized();

    var claims = new List<Claim>
    {
        new Claim("Id", user.Id.ToString()),
        new Claim(ClaimTypes.Role, "Admin"),
        new Claim("Name", user.Name)
    };

    var token = JewelryTokenService.GetToken(claims);

    return Ok(JewelryTokenService.WriteToken(token));
    }

    [HttpGet()]
    [Authorize(Roles = "Admin")]
    public ActionResult<IEnumerable<User>> Get()
    {
        return userService.Get();
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
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
        var userId = userService.Create(newUser);
        return CreatedAtAction(nameof(Get), new { id = userId }, newUser);
    }

    [HttpPut("{id}")]
    public ActionResult Update(int id, User newUser)
    {
        if (id != newUser.Id)
            return BadRequest();

        var success = userService.Update(newUser);

        if (!success)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var user = userService.Get(id);

        if (user == null)
            return NotFound();
        userService.Delete(id);

        return NoContent();
    }

}
