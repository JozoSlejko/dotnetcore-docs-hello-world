using dotnetcoresample.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Needed to read the current user's Cookie header in Blazor Server
builder.Services.AddHttpContextAccessor();

// Needed for IHttpClientFactory
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapGet("/logout", () =>
{
    // 1) clear Easy Auth cookie, then come back to /logged-out
    return Results.Redirect("/.auth/logout?post_logout_redirect_uri=/logged-out");
});

app.MapGet("/logged-out", () =>
{
    // 2) clear Auth0 SSO session, then return to the app home page
    var auth0Domain = "https://slejco.eu.auth0.com";
    var clientId = "qAfeGEKBODAI2yi6O6DoIPCaF2qZ6p4F";
    var returnTo = "https://javdapp1.azurewebsites.net/";

    var url =
        $"{auth0Domain}/v2/logout" +
        $"?client_id={Uri.EscapeDataString(clientId)}" +
        $"&returnTo={Uri.EscapeDataString(returnTo)}";

    return Results.Redirect(url);
});

app.Run();
