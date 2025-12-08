
using FTT_API.Common.ConfigurationHelper;
using Microsoft.Extensions.FileProviders;
using Hangfire;
using FTT_API.Models.Handler;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Const.VO;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;


IConfiguration Config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

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
//builder.Services.AddRazorPages();
 

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FTT_API", Version = "v1" });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".net.core.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(15);
    //options.Cookie.IsEssential = true; //架設http 非 https 要註解

    //options.Cookie.HttpOnly = true; //架設http 非 https 要註解
    //options.Cookie.SecurePolicy = CookieSecurePolicy.Always; //架設http 非 https 要註解
});

builder.Services.AddAntiforgery(options =>
{
    // 設置 token 的名稱（可選）
    options.HeaderName = "X-CSRF-TOKEN";
    // 設置 cookie 的名稱（可選）
    options.Cookie.Name = "CSRF-COOKIE";

    // 🌟 錯誤點 2：跨域必須設置為 None
    options.Cookie.SameSite = SameSiteMode.None;

    // 🌟 錯誤點 1：設置為 None 時，Secure 必須為 Always
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
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

var secret = builder.Configuration["JwtConfig:Secret"];
var key = Encoding.UTF8.GetBytes(secret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(jwt =>
{
    jwt.SaveToken = true;


    jwt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JwtConfig:Issuer"],

        ValidateAudience = false,

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),

        ValidateLifetime = true
    };
});
//JOB
builder.Services.AddSingleton<FETTaskService>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<FETTaskService>());


builder.Services.AddSingleton<ConfigurationHelper>();

builder.Services.AddHangfire(config =>
              config.UseInMemoryStorage(new()
              {
                  MaxExpirationTime = TimeSpan.FromHours(1)
              })
          );
builder.Services.AddHangfireServer();
//builder.Services.AddSingleton<SendMailHandler>();
builder.Services.AddScoped<SendMailHandler>();
//builder.Services.AddScoped<CheckVenderPWLoginTimeHandler>();

// 註冊 CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost7234",
        policy =>
        {
            policy.WithOrigins("https://localhost:50102") // 允許的來源
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials()
              .WithExposedHeaders("Content-Disposition"); // <- 重要;
        });
}); 

builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365); // 1年
});

var app = builder.Build();

app.UseSwagger();
if (Config.GetValue<string>("EnableSwaggerUI") == "Y")
{
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "LADSWeb API V1");
    });
}


// Configure the HTTP request pipeline.
app.UseExceptionHandler("/Home/Error");

// 只有 HTTPS 才啟用 HSTS
app.Use(async (context, next) =>
{
    if (context.Request.IsHttps)
    {
        app.UseHsts();
    }

    await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

#region Localization
//app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
app.UseRequestLocalization(localizationoptions);
#endregion

app.UseHangfireDashboard();

app.UseRouting();

// 使用 CORS
app.UseCors("AllowLocalhost7234");

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

//app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "swagger/index.html");
//pattern: "triptest/{controller=Home}/{action=Index}/{id?}");

//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(
//        Path.Combine(builder.Environment.ContentRootPath, "PublicStaticFile")
//    ),
//    RequestPath = "/download"
//});

//// 專案啟動時載入
//var container = new Unity.UnityContainer();
//Business.BusinessFactory.Register(container);
FTT_API.Common.HttpContext.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());

RecurringJob.AddOrUpdate<SendMailHandler>(
    nameof(SendMailHandler.Send),
    (job) => job.Send(),
    "* * * * *",
    new RecurringJobOptions
    {
        TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time")
    }
);

RecurringJob.AddOrUpdate<CheckVenderPWLoginTimeHandler>(
        nameof(CheckVenderPWLoginTimeHandler.CheckPWChangeTime),
        (job) => job.CheckPWChangeTime(),
          builder.Configuration["HangFireScheduledTime:CheckVendorLastChangePW"],         
         new RecurringJobOptions
         {
             TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time")
         }
    );

RecurringJob.AddOrUpdate<CheckVenderPWLoginTimeHandler>(
        nameof(CheckVenderPWLoginTimeHandler.CheckLastLoginTime),
        (job) => job.CheckLastLoginTime(),
         builder.Configuration["HangFireScheduledTime:CheckVendorLastLogin"],
         new RecurringJobOptions
         {
             TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time")
         }
    );

app.Run();