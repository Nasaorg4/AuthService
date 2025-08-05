using AuthService.Model;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//swagger

builder.Services.Configure<JwtSettings>(
	builder.Configuration.GetSection("JwtSettings"));


var app = builder.Build();




//swagger
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
//swagger

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();





app.UseAuthorization();

app.MapControllers();

app.Run();
