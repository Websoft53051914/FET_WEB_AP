
using FTT_VENDER_API.Common.ConfigurationHelper;
using Microsoft.Extensions.FileProviders;
using FTT_VENDER_API.Models.Handler;
using Microsoft.OpenApi.Models;
using System.Reflection;


IConfiguration Config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();

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
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FTT_VENDER_API", Version = "v1" });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});

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
    //options.Cookie.IsEssential = true; //�[�]http �D https �n����

    //options.Cookie.HttpOnly = true; //�[�]http �D https �n����
    //options.Cookie.SecurePolicy = CookieSecurePolicy.Always; //�[�]http �D https �n����
});

builder.Services.AddAntiforgery(options =>
{

    //options.Cookie.SecurePolicy = CookieSecurePolicy.Always; //�[�]http �D https �n����
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

// ���U CORS
#if DEBUG
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost7234",
        policy =>
        {
            policy.WithOrigins("https://localhost:7236") // ���\���ӷ�
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials()
              .WithExposedHeaders("Content-Disposition"); // <- ���n;
        });
});
#else

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost7234",
        policy =>
        {
            policy.WithOrigins("http://192.168.1.107:8004") // ���\���ӷ�
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

#endif

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowLocalhost7234", policy =>
//    {
//        policy
//            .AllowAnyOrigin()
//            .AllowAnyHeader()
//            .AllowAnyMethod();
//    });
//});

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
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts(); //�[�]http �D https �n����
}

app.UseHttpsRedirection();
app.UseStaticFiles();

#region Localization
//app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
app.UseRequestLocalization(localizationoptions);
#endregion

app.UseRouting();

// �ϥ� CORS
app.UseCors("AllowLocalhost7234");


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

//// �M�ױҰʮɸ��J
//var container = new Unity.UnityContainer();
//Business.BusinessFactory.Register(container);
FTT_VENDER_API.Common.HttpContext.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());

app.Run();