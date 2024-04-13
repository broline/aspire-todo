using Todo.Data;
using Todo.Common;
using Todo.Api;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Startup classes are obsolete as of .net 6 and all the registration code should be in program.cs, BUT
// TestServer in our integration tests do not yet support a single program file, only a startup file
// WebApplicationFactory does support a program file, BUT then we cannot use collection based fixtures (only class based)
// meaning each test class would have to create its own server and that would be expensive
// Doing this for now until one of those two issues are fixed
var startup = new ApiStartup(builder.Environment, builder.Configuration);

startup.ConfigureServices(builder.Services);

if (builder.Environment.EnvironmentName != Constants.EnvironmentNames.OpenApi)
    builder.AddAspireDb();

var app = builder.Build();

startup.Configure(app);

var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger(Constants.AppNames.Api);

logger.LogInformation($"{Constants.AppNames.Api} Started!");

await app.RunAsync();

logger.LogInformation($"{Constants.AppNames.Api} Stopped!");