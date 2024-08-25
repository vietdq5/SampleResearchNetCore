using BaseProject.Hubs;
using BaseProject.MiddlewaresCustom;
using BaseProject.Settings;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services
    .AddConfig(builder.Configuration)
    .AddMyDBContext(builder.Configuration)
    .AddMyDependencyGroup();
//
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder
        .WithOrigins("http://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});
//
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseWebSockets();

app.MapHub<ChatHub>("/chatHub");

//app.UseMiddleware<WebSocketMiddleware>();

app.MapControllers();

app.Run();