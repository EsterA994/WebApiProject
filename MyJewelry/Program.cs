
// using System.IdentityModel.Tokens.Jwt;
// using MyJewelry.Services;
// using MyJewelry.Interfaces;
// using Microsoft.OpenApi.Models;

// var builder = WebApplication.CreateBuilder(args);

// // 1. הוספת שירותי Controllers
// builder.Services.AddControllers();

// // 2. רישום השירותים שלך באמצעות ה-Extensions שכתבת בקבצים
// builder.Services.AddUserService();      // מתוך UserService.cs
// builder.Services.AddJewelryService();   // מתוך JewelryService.cs
// builder.Services.AddActiveUser();        // מתוך ActiveUserService.cs

// // 3. חובה עבור ActiveUserService - גישה ל-HttpContext
// builder.Services.AddHttpContextAccessor();

// // 4. הגדרת אימות (Authentication) עם JWT
// builder.Services.AddAuthentication("Bearer")
//     .AddJwtBearer("Bearer", options =>
//     {
//         options.TokenValidationParameters = JewelryTokenService.GetTokenValidationParameters();
//     });

// // 5. הגדרת Swagger/OpenAPI
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen(c =>
// {
//     c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyJewelry API", Version = "v1" });
// });

// var app = builder.Build();

// // --- הגדרת צינור הטיפול בבקשות (Middleware Pipeline) ---

// // א. הגשת קבצים סטטיים (חייב לבוא לפני הניתוב והאימות)
// app.UseDefaultFiles(); // מאפשר להריץ את index.html אוטומטית
// app.UseStaticFiles();  // מגיש קבצים מתיקיית wwwroot

// app.UseRouting();

// // ב. אימות והרשאות
// app.UseAuthentication();
// app.UseMyLogMiddleware(); // הלוגר שלך יראה עכשיו את המשתמש המחובר
// app.UseAuthorization();

// // ג. הגדרת Swagger בסביבת פיתוח
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// app.MapControllers();

// app.Run();




using System.IdentityModel.Tokens.Jwt; // להוסיף למעלה
using MyJewelry;
using MyJewelry.Interfaces;
using MyJewelry.Services;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

///
var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

builder.Services.AddUserService();
builder.Services.AddJewelryService();

builder.Services.AddHttpContextAccessor();
builder.Services.AddActiveUser();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddOpenApi();

builder
    .Services.AddAuthentication("Bearer")
    .AddJwtBearer(
        "Bearer",
        options =>
        {
            options.TokenValidationParameters = JewelryTokenService.GetTokenValidationParameters();
        }
    );

var app = builder.Build();

/* js */
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();
app.UseMyLogMiddleware();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}




app.MapControllers();

app.Run();

