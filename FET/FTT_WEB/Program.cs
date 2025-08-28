
using FTT_WEB.Common.ConfigurationHelper;
using Microsoft.Extensions.FileProviders;
//using Hangfire;
using FTT_WEB.Models.Handler;


var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
#region Localization
var localizationoptions = new RequestLocalizationOptions();
var supportedCultures = new List<System.Globalization.CultureInfo> {
        new System.Globalization.CultureInfo("zh-TW"),
        new System.Globalization.CultureInfo("en-US")
    };
localizationoptions.SupportedCultures = supportedCultures;
localizationoptions.SupportedUICultures = supportedCultures;
localizationoptions.SetDefaultCulture("zh-TW");
localizationoptions.ApplyCurrentCultureToResponseHeaders = true;

#endregion

// Add services to the container.
builder.Services.AddRazorPages();
 

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme;
})
 .AddCookie(options =>
 {
     options.Cookie.HttpOnly = true;
     options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
 });

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".FTT_WEB.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(15);
    //options.Cookie.IsEssential = true; //架設http 非 https 要註解

    //options.Cookie.HttpOnly = true; //架設http 非 https 要註解
    //options.Cookie.SecurePolicy = CookieSecurePolicy.Always; //架設http 非 https 要註解
});

builder.Services.AddAntiforgery(options =>
{

    //options.Cookie.SecurePolicy = CookieSecurePolicy.Always; //架設http 非 https 要註解
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddHttpClient();
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // response 回傳屬性不強制改成 camelcase
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile("message.json", optional: true, reloadOnChange: true);


builder.Services.AddSingleton<ConfigurationHelper>();

//builder.Services.AddHangfire(config =>
//              config.UseInMemoryStorage(new()
//              {
//                  MaxExpirationTime = TimeSpan.FromHours(1)
//              })
//          );
//builder.Services.AddHangfireServer();
//builder.Services.AddSingleton<SendMailHandler>();
builder.Services.AddScoped<SendMailHandler>(); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts(); //架設http 非 https 要註解
}

app.UseHttpsRedirection();
app.UseStaticFiles();

#region Localization
//app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
app.UseRequestLocalization(localizationoptions);
#endregion

app.UseRouting();
//app.UseHangfireDashboard();


//app.UseCors();


app.UseAuthorization();

app.UseSession();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=login}/{action=Index}/{id?}");
//pattern: "triptest/{controller=Home}/{action=Index}/{id?}");

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "PublicStaticFile")
    ),
    RequestPath = "/download"
});

//// 專案啟動時載入
//var container = new Unity.UnityContainer();
//Business.BusinessFactory.Register(container);
FTT_WEB.Common.HttpContext.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());

//RecurringJob.AddOrUpdate<SendMailHandler>(
//    nameof(SendMailHandler.Send),
//    (job) => job.Send(),
//    "* * * * *",
//    new RecurringJobOptions
//    {
//        TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time")
//    }
//);

app.Run();