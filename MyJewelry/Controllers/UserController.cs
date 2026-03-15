using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyJewelry.Interfaces;
using MyJewelry.Models;
using MyJewelry.Services;

namespace MyJewelry.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize] // דורש התחברות לכל הפעולות בבקר
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IActiveUser _activeUserService;

        public UserController(IUserService userService, IActiveUser activeUserService)
        {
            _userService = userService;
            _activeUserService = activeUserService;
        }

        // לקבלת רשימת כל המשתמשים - רק לאדמין מותר
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult<IEnumerable<User>> Get()
        {
            return Ok(_userService.Get());
        }

        // לקבלת פרטי המשתמש המחובר כרגע (פרופיל אישי)
        [HttpGet("me")]
        public ActionResult<User> GetMyProfile()
        {
            var currentUser = _activeUserService.ActiveUser;
            if (currentUser == null)
                return Unauthorized();

            var user = _userService.Get(currentUser.Id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        // לקבלת משתמש לפי ID ספציפי
        [HttpGet("{id}")]
        public ActionResult<User> Get(int id)
        {
            var currentUser = _activeUserService.ActiveUser;

            // בדיקה: אם הוא לא אדמין, מותר לו לראות רק את ה-ID של עצמו
            if (!User.IsInRole("Admin") && currentUser.Id != id)
            {
                return Forbid("אינך מורשה לצפות בפרטים של משתמש אחר");
            }

            var user = _userService.Get(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        // יצירת משתמש חדש - בדרך כלל רק אדמין או הרשמה (Register)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(User newUser)
        {
            var userId = _userService.Create(newUser);
            return CreatedAtAction(nameof(Get), new { id = userId }, newUser);
        }

        // עדכון פרטי משתמש
        [HttpPut("{id}")]
        public ActionResult Update(int id, User newUser)
        {
            var currentUser = _activeUserService.ActiveUser;

            // מותר לעדכן רק אם אתה אדמין או שאתה מעדכן את עצמך
            if (!User.IsInRole("Admin") && currentUser.Id != id)
            {
                return Forbid();
            }

            if (id != newUser.Id)
                return BadRequest("ה-ID בנתיב אינו תואם ל-ID של האובייקט");

            var success = _userService.Update(newUser);
            if (!success)
                return NotFound();

            return NoContent();
        }

        // מחיקת משתמש - רק לאדמין מותר
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            var success = _userService.Delete(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}

// using System.Collections.Generic;
// using System.Linq;
// using System.Security.Claims;
// using System.Security.Cryptography.X509Certificates;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using MyJewelry.Interfaces;
// using MyJewelry.Models;
// using MyJewelry.Services;

// namespace MyJewelry.Controllers;

// [ApiController]
// [Route("[controller]")]
// public class UserController : ControllerBase
// {
//     IUserService userService;

//     public UserController(IUserService userService)
//     {
//         this.userService = userService;
//     }

//     [HttpGet()]
//     [Authorize(Roles = "Admin")]
//     public ActionResult<IEnumerable<User>> Get()
//     {
//         return userService.Get();
//     }

//     [HttpGet("{id}")]
//     [Authorize(Roles = "Admin,User")]
//     public ActionResult<User> Get(int id)
//     {
//         var user = userService.Get(id);
//         if (user == null)
//             return NotFound();
//         return user;
//     }

//     [HttpPost]
//      [Authorize(Roles = "Admin")]
//     public ActionResult Create(User newUser)
//     {
//         var userId = userService.Create(newUser);
//         return CreatedAtAction(nameof(Get), new { id = userId }, newUser);
//     }

//     [HttpPut("{id=}")]
//     public ActionResult Update(int id, User newUser)
//     {
//         if (id != newUser.Id)
//             return BadRequest();

//         var success = userService.Update(newUser);

//         if (!success)
//             return NotFound();

//         return NoContent();
//     }

//     [HttpDelete("{id}")]
//     public ActionResult Delete(int id)
//     {
//         var user = userService.Get(id);

//         if (user == null)
//             return NotFound();
//         userService.Delete(id);

//         return NoContent();
//     }
// }
