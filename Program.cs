var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

var app = builder.Build();
app.UseStaticFiles();
app.MapControllers();
app.MapGet("/", () => Results.Redirect("/index.html"));
app.MapGet("/admin", () => Results.Redirect("/admin.html"));

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://0.0.0.0:{port}");
app.Run();
