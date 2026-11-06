using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SerieA.WebUI.Extensions;
using SerieA.WebUI.Handlers;
using SerieA.WebUI.Services.Auth;
using SerieA.WebUI.Services.Dashboard;
using SerieA.WebUI.Services.Matches;
using SerieA.WebUI.Services.MatchEvents;
using SerieA.WebUI.Services.Standings;
using SerieA.WebUI.Services.Teams;
using SerieA.WebUI.Services.Users;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
    {
        options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddAntiforgery(o => o.HeaderName = "X-CSRF-TOKEN");

builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<AuthHeaderHandler>();

// WebUI Cookie Authentication Infrastructure
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = ".SerieA.Auth";
        options.LoginPath = "/Account/Login";               
        options.LogoutPath = "/Account/Logout";             
        options.AccessDeniedPath = "/Account/AccessDenied"; 
        options.ExpireTimeSpan = TimeSpan.FromHours(4);    
        options.SlidingExpiration = false;                 
        options.Cookie.HttpOnly = true;                   
        options.Cookie.SameSite = SameSiteMode.Lax;

        // Her sayfa isteğinde JWT token süresini kontrol et; dolmuşsa oturumu kapatıp Login'e yönlendir
        options.Events = new CookieAuthenticationEvents
        {
            OnValidatePrincipal = async context =>
            {
                var token = context.Properties.GetTokenValue("access_token");
                if (JwtHelper.IsTokenExpired(token))
                {
                    context.RejectPrincipal();
                    await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                }
            }
        };
    });

builder.Services.AddTransient<ApiExceptionHandler>();
builder.Services.AddCustomHttpClient<ITeamApiService, TeamApiService>();
builder.Services.AddCustomHttpClient<IMatchApiService, MatchApiService>();
builder.Services.AddCustomHttpClient<IMatchEventApiService, MatchEventApiService>();
builder.Services.AddCustomHttpClient<IStandingApiService, StandingApiService>();
builder.Services.AddCustomHttpClient<IDashboardApiService, DashboardApiService>();
builder.Services.AddCustomHttpClient<IUserApiService, UserApiService>();
builder.Services.AddCustomHttpClient<IAuthApiService, AuthApiService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}
app.UseStatusCodePagesWithReExecute("/Home/StatusCodeError", "?code={0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

