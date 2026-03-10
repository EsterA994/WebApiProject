using MyJewelry.Interfaces;
using MyJewelry.Services;
using MyJewelry;

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

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = JewelryTokenService.GetTokenValidationParameters();
    });

var app = builder.Build();

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

/* js */
app.UseDefaultFiles();
app.UseStaticFiles();
/* remove "launchUrl" from Properties\launchSettings.json */

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// using MyJewelry.Interfaces;
// using MyJewelry.Services;
// using MyJewelry;
// // using Microsoft.OpenApi;

// var builder = WebApplication.CreateBuilder(args);

// // Add services to the container.
// builder.Services.AddUserService();
// builder.Services.AddJewelryService();
// builder.Services.AddControllers();

// builder.Services.AddHttpContextAccessor();
// builder.Services.AddActiveUser();

// builder.Logging.ClearProviders();
// builder.Logging.AddConsole(); 


// builder.Services.AddOpenApi();

// var app = builder.Build();

// app.UseMyLogMiddleware();


// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
//     app.UseSwaggerUI(options =>
//     {
//         options.SwaggerEndpoint("/openapi/v1.json", "v1");
//     });
// }

// /*js*/
// app.UseDefaultFiles();
// app.UseStaticFiles();
// /*js (remove "launchUrl" from Properties\launchSettings.json*/

// app.UseHttpsRedirection();

// app.UseAuthorization();
// builder.Services.AddAuthentication("Bearer")
//     .AddJwtBearer("Bearer", options =>
//     {
//         options.TokenValidationParameters = JewelryTokenService.GetTokenValidationParameters();
//     });

// app.MapControllers();

// app.Run();
