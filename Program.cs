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
    // 1) Log out of Auth0, return to /easy-auth-logout
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
    // 2) Clear Easy Auth cookie in a hidden iframe, then send user home
    const string html = """
<!doctype html>
<html>
<head>
  <meta charset="utf-8" />
  <title>Signing out…</title>
  <meta name="viewport" content="width=device-width, initial-scale=1" />
</head>
<body>
  <p>Signing out…</p>

  <iframe src="/.auth/logout" style="display:none" aria-hidden="true"></iframe>

  <script>
    setTimeout(function () { window.location.replace("/"); }, 800);
  </script>
</body>
</html>
""";

    return Results.Content(html, "text/html; charset=utf-8");
});

app.Run();
