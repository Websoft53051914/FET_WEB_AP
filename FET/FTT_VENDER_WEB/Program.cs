
using FTT_VENDER_WEB.Common.ConfigurationHelper;
using Microsoft.Extensions.FileProviders;
//using Hangfire;
using FTT_VENDER_WEB.Models.Handler;
using Microsoft.AspNetCore.DataProtection;



var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

// --- 關鍵修正：在 Linux 環境必須註冊編碼提供者 ---
System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
// ------

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

// �[�J Data Protection �]�w - �󥭥x�ۮe
//20260202 var dataProtectionKeysPath = Environment.OSVersion.Platform == PlatformID.Win32NT
//    ? Path.Combine(Directory.GetCurrentDirectory(), "DataProtectionKeys")
//    : "/home/wmliou75/FTT/DataProtectionKeys";

//20260202
var dataProtectionKeysPath = builder.Configuration["DataProtectionPath"]
?? Path.Combine(AppContext.BaseDirectory, "DataProtectionKeys");

// �T�O�ؿ��s�b
Directory.CreateDirectory(dataProtectionKeysPath);

builder.Services.AddDataProtection()
    .SetApplicationName("FTT_VENDER_API")
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeysPath));


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
    options.Cookie.Name = ".net.core.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(15);
    options.Cookie.IsEssential = true; //�[�]http �D https �n����

    options.Cookie.HttpOnly = true; //�[�]http �D https �n����
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; //�[�]http �D https �n����
});

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; //�[�]http �D https �n����
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddHttpClient();
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // response �^���ݩʤ��j��令 camelcase
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
//builder.Services.AddScoped<SendMailHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts(); //�[�]http �D https �n����
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// 設定 PublicStaticFile 目錄為可下載的靜態檔案
var publicStaticFilePath = Path.Combine(builder.Environment.ContentRootPath, "PublicStaticFile");
if (!Directory.Exists(publicStaticFilePath))
{
    Directory.CreateDirectory(publicStaticFilePath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(publicStaticFilePath),
    RequestPath = "/download",
    ServeUnknownFileTypes = true, // 允許提供未知檔案類型
    DefaultContentType = "application/octet-stream" // 設定預設 MIME 類型
});

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

FTT_VENDER_WEB.Common.HttpContext.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());

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