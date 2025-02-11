
var builder = WebApplication.CreateBuilder(args);

const string CredentialsPath = "./client_secret.json";
const string TokenPath = "./token.json";
const string ApplicationName = "Todo Api with Google Tasks";

const string serviceAccountEmail = "todogoogle@todo-1608676657801.iam.gserviceaccount.com";
const string serviceAccountName = "TodoGoogle";
const string serviceAccountI = "todogoogle";


var googleOptions = new GoogleOptions()
{
    ApplicationName = ApplicationName,
    CredentialsPath = CredentialsPath,
    TokenPath = TokenPath
};

// Configure auth
//builder.AddAuthentication();
//builder.Services.AddAuthorizationBuilder().AddCurrentUserHandler();

// Add the service to generate JWT tokens
//builder.Services.AddTokenService();

builder.Services.AddSingleton<GoogleOptions>(googleOptions);
builder.Services.AddGoogleTasksService();

// Configure the database
//var connectionString = builder.Configuration.GetConnectionString("Todos") ?? "Data Source=.db/Todos.db";
//builder.Services.AddSqlite<TodoDbContext>(connectionString);

// Configure identity
//builder.Services.AddIdentityCore<TodoUser>()
//                .AddEntityFrameworkStores<TodoDbContext>();

// State that represents the current user from the database *and* the request
//builder.Services.AddCurrentUser();

// Configure Open API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o => o.InferSecuritySchemes());

// Configure rate limiting
//builder.Services.AddRateLimiting();

// Configure OpenTelemetry
//builder.AddOpenTelemetry();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseRateLimiter();

app.Map("/", () => Results.Redirect("/swagger"));

// Configure the APIs
app.MapTodos();
//app.MapUsers();

// Configure the prometheus endpoint for scraping metrics
//app.MapPrometheusScrapingEndpoint();
// NOTE: This should only be exposed on an internal port!
// .RequireHost("*:9100");

app.Run();
