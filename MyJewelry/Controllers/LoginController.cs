// using System;
// using System.Collections.Generic;
// using System.Security.Claims;
// using Microsoft.AspNetCore.Mvc;
// using MyJewelry.Models;
// using MyJewelry.Services;

// namespace MyJewelry.Controllers;

// [ApiController]
// [Route("[controller]")]
// public class LoginController : ControllerBase
// {
//     public LoginController() { }

//     [HttpPost]
//     public ActionResult<String> Login(User User)
//     {
//         var dt = DateTime.Now;

//          var query = $"select * from users where idnumber = @idnumber";
//         if (User.Name != "Esty"
//         || User.Id != 1272)
//         {
//             return Unauthorized();
//         }

//         var claims = new List<Claim>
//             {
//                 new Claim("username", User.Name),
//                 new Claim("type", "User"),
//             };

//         var token = JewelryTokenService.GetToken(claims);

//         return new OkObjectResult(JewelryTokenService.WriteToken(token));
//     }
// }