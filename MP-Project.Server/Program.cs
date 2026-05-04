using Microsoft.EntityFrameworkCore;
using MP_Project.BlazorClient.Services;
using MP_Project.Server.Data;
///////////////////////////////////////////
//using MP_Project.Server.Middleware;
///////////////////////////////////////////

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<PersonaService>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddScoped(sp => new HttpClient
{
	BaseAddress = new Uri("https://localhost:7190/")
});

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});


builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<AppDbContext>(options =>
{
	var conn = builder.Configuration.GetConnectionString("DefaultConnection");
	options.UseMySql(conn, ServerVersion.AutoDetect(conn));
});

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowClient",
		policy =>
		{
			policy
				.AllowAnyOrigin()
				.AllowAnyHeader()
				.AllowAnyMethod();
		});
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowClient");

app.UseSession();

app.UseAuthorization();

app.MapControllers();

app.Run();
