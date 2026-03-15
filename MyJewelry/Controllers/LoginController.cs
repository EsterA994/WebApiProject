 using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyJewelry.Interfaces;
using MyJewelry.Models;
using MyJewelry.Services;

 [ApiController]
  [Route("api/[controller]")]
 public class LoginController : ControllerBase
{
    IUserService userService;

    public LoginController(IUserService userService)
    {
        this.userService = userService;
    }
 
    [HttpPost("login")]
    public ActionResult<string> Login( User loginInfo)
    {
        // 1. חיפוש המשתמש ב-UserService (שבודק ב-JSON)
        var list = userService
            .Get().ToList().FirstOrDefault(u => u.Name == loginInfo.Name && u.Id == loginInfo.Id);
            //.FirstOrDefault(u => u.Name == loginInfo.Name && u.Id == loginInfo.Id);

        // var user = list.FirstOrDefault(u => u.Name == loginInfo.Name && u.Id == loginInfo.Id);
        if (list == null)
        {
            return Unauthorized("aaa User not found or password incorrect:  "+list);
        }

        // 2. קביעת תפקיד (אפשר להשאיר את הלוגיקה שלך או להוסיף שדה Role למודל User)
        loginInfo.Role = (loginInfo.Name == "Esty" && loginInfo.Id == 1272) ? "Admin" : "User";

        // 3. יצירת Claims עם ה-ID האמיתי מה-JSON
        var claims = new List<Claim>
        {
            new Claim("Id", loginInfo.Id.ToString()), // חשוב שזה יהיה ה-ID מה-JSON!
            new Claim(ClaimTypes.Role, loginInfo.Role),
            new Claim("Name", loginInfo.Name),
        };

        var token = JewelryTokenService.GetToken(claims);
        return Ok(JewelryTokenService.WriteToken(token));
    }
}