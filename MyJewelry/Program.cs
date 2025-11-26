
using MyJewelry.Interfaces;
using MyJewelry.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.addJewelryService();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

/*js*/
app.UseDefaultFiles();
app.UseStaticFiles();
/*js (remove "launchUrl" from Properties\launchSettings.json*/

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
