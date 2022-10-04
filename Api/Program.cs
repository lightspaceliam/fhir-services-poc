using Api.MockServices;
using Api.MockServices.Abstract;
using Fhir.Service;
using Fhir.Service.Abstract;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

const string CorsPolicy = "Cors_Policy";

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        //options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/*  
 *  Entity Services Registration.
 */
builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();

/*
 * Named HTTP Client/s
 */
builder.Services.AddHttpClient<FhirHttpClient>(httpClient =>
{
    httpClient.BaseAddress = new Uri(builder.Configuration["Fhir:BaseUrl"]);
    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/fhir+json"));
}).SetHandlerLifetime(TimeSpan.FromMinutes(5));

builder.Services.AddTransient<IFhirService<Hl7.Fhir.Model.Patient, Hl7.Fhir.Model.Patient>, PatientService>();

/*
 * Authentication.
 */
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwt =>
{
    var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Secret"]);
    jwt.SaveToken = true;
    jwt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        RequireExpirationTime = false,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["Jwt:Authority"],
        ValidAudience = builder.Configuration["Jwt:Audience"]
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseHttpsRedirection();
app.UseCors(CorsPolicy);
app.UseAuthorization();

app.MapControllers();

app.Run();
