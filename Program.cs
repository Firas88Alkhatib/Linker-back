using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.OData;
using Linker.Models;
using Linker.Data;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;

static IEdmModel GetEdmModel()
{
    ODataConventionModelBuilder builder = new();

    builder.EntitySet<ShortLink>("Links").EntityType.Expand().Filter().Count().Select().OrderBy();

    builder.EnableLowerCamelCase();
    return builder.GetEdmModel();
};

FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromJson(Configuration["GoogleCredential"])
});


// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
}).AddOData((options) =>
{
    options.AddRouteComponents("Odata", GetEdmModel())
    .EnableNoDollarQueryOptions = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDb>();
builder.Services.AddSingleton<RandomService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(opt =>
{
    opt.Authority = Configuration["Jwt:Firebase:ValidIssuer"];
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = Configuration["Jwt:Firebase:ValidIssuer"],
        ValidAudience = Configuration["Jwt:Firebase:ValidAudience"]
    };
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}
app.UseCors(options =>
{
    options.AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod();
});

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
