using Currency.API.Middlewares;
using Currency.API.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.OperationFilter<AuthenticationHeadersFilter>();
});

builder.Services.AddHttpClient();
builder.Services.AddSingleton<ExceptionCatchMiddleware>();

Currency.API.Application.Register.RegisterMediator(builder.Services);
Currency.API.Application.Register.RegisterScopedServices(builder.Services);
Currency.API.Infrastructure.Register.RegisterScopedRepository(builder.Services);
Currency.API.Infrastructure.Register.RegisterScopedDBConnection(builder.Services, builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseMiddleware<ExceptionCatchMiddleware>();

app.Run();
