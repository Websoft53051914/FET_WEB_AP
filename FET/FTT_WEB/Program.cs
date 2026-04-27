
using FTT_WEB.Common.ConfigurationHelper;
using Microsoft.Extensions.FileProviders;
//using Hangfire;
using FTT_WEB.Models.Handler;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides; // 記得在檔案最上方加入 using


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


// 加入 Data Protection 設定 - 跨平台相容
//20260202 var dataProtectionKeysPath = Environment.OSVersion.Platform == PlatformID.Win32NT
//    ? Path.Combine(Directory.GetCurrentDirectory(), "DataProtectionKeys")
//    : "/home/wmliou75/FTT/DataProtectionKeys";
//20260202
var dataProtectionKeysPath = builder.Configuration["DataProtectionPath"]
?? Path.Combine(AppContext.BaseDirectory, "DataProtectionKeys");

// 確保目錄存在
Directory.CreateDirectory(dataProtectionKeysPath);

builder.Services.AddDataProtection()
    .SetApplicationName("FTT_API")
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeysPath));


builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme;
})
 .AddCookie(options =>
 {
     options.Cookie.HttpOnly = true;
     //20260203 options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
     options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
 });

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".net.core.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(15);
    options.Cookie.IsEssential = true; //架設http 非 https 要註解

    options.Cookie.HttpOnly = true; //架設http 非 https 要註解
    //20260203 options.Cookie.SecurePolicy = CookieSecurePolicy.Always; //架設http 非 https 要註解
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

builder.Services.AddAntiforgery(options =>
{
    //20260203 options.Cookie.SecurePolicy = CookieSecurePolicy.Always; //架設http 非 https 要註解
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
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

//20260203 var app = builder.Build();
var app = builder.Build();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
// ----------------------------------------

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts(); //架設http 非 https 要註解
}

//20260203 app.UseHttpsRedirection();
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