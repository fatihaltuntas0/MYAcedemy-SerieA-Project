using SerieA.WebUI.Extensions;
using SerieA.WebUI.Handlers;
using SerieA.WebUI.Services.Dashboard;
using SerieA.WebUI.Services.Matches;
using SerieA.WebUI.Services.MatchEvents;
using SerieA.WebUI.Services.Standings;
using SerieA.WebUI.Services.Teams;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddTransient<ApiExceptionHandler>();

builder.Services.AddCustomHttpClient<ITeamApiService, TeamApiService>();
builder.Services.AddCustomHttpClient<IMatchApiService, MatchApiService>();
builder.Services.AddCustomHttpClient<IStandingApiService, StandingApiService>();
builder.Services.AddCustomHttpClient<IMatchEventApiService, MatchEventApiService>();
builder.Services.AddCustomHttpClient<IDashboardApiService, DashboardApiService>();

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

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
