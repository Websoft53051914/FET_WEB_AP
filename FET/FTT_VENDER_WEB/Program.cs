
using FTT_VENDER_WEB.Common.ConfigurationHelper;
using Microsoft.Extensions.FileProviders;

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
    options.Cookie.Name = ".FET_RM_Vender.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(15);
    options.Cookie.IsEssential = true;

    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddAntiforgery(options =>
{

    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

#region Localization
//app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
app.UseRequestLocalization(localizationoptions);
#endregion

app.UseRouting();


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
FTT_VENDER_WEB.Common.HttpContext.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());

app.Run();