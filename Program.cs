using MyApp.Namespace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<RedisService>();        // Register RedisService
builder.Services.AddScoped<RabbitMQService>();
builder.Services.AddScoped<ITaskInterface,TaskRepository>();

builder.Services.AddScoped<IRegisterInterface, RegisterRepository>();
builder.Services.AddScoped<ILoginInterface, LoginRepository>();
builder.Services.AddScoped<IUserInterface, UserRepository>();
builder.Services.AddControllersWithViews();


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Set session timeout duration
    options.Cookie.HttpOnly = true;  // Makes the cookie accessible only to the server
    options.Cookie.IsEssential = true;  // Mark session cookie as essential
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();

app.MapDefaultControllerRoute();

app.UseAuthorization();

app.MapStaticAssets();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
