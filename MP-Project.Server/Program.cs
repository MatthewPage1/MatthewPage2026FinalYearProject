using Microsoft.EntityFrameworkCore;
using MP_Project.BlazorClient.Services;
using MP_Project.Server.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<PersonaService>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddScoped(sp => new HttpClient
{
	BaseAddress = new Uri("https://localhost:7190/")
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseMySql(
		connectionString,
		ServerVersion.AutoDetect(connectionString)
	)
);

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

app.UseAuthorization();

app.MapControllers();

app.Run();
