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
    // Step 1: log out of Auth0, then return to /easy-auth-logout
    var auth0Domain = "https://slejco.eu.auth0.com";
    var clientId = "qAfeGEKBODAI2yi6O6DoIPCaF2qZ6p4F";
    var returnTo = "https://javdapp1.azurewebsites.net/easy-auth-logout";

    var url =
        $"{auth0Domain}/v2/logout" +
        $"?client_id={Uri.EscapeDataString(clientId)}" +
        $"&returnTo={Uri.EscapeDataString(returnTo)}";

    return Results.Redirect(url);
});

app.MapGet("/easy-auth-logout", () =>
{
    // Step 2: clear Easy Auth cookie. No redirect params (since they don't work for you).
    return Results.Redirect("/.auth/logout");
});

app.Run();
