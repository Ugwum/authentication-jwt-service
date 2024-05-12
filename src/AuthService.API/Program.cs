using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using AuthService.Core.DataAccess;
using AuthService.Core.DataAccess.Abstractions;
using AuthService.Core.DataAccess.Repository;
using AuthService.Core.Infrastructure;
using AuthService.Core.Infrastructure.Abstractions;
using AuthService.Core.Model.Configuration;
using AuthService.Core.Model.Validators;
using AuthService.Core.Service.Abstractions;
using AuthService.Core.Service;
using AuthService.Core.Utils;
using StackExchange.Redis;
using AuthService.Core.Middleware;
using Serilog;
using AuthService.Core;

var builder = WebApplication.CreateBuilder(args);


builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection("ConnectionStrings"));

builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JWTSettings"));

builder.Services.AddDbContext<DBContext>(options =>
{
    var connectionstring = builder.Configuration.GetSection("ConnectionStrings").Get<ConnectionStrings>();
    options.UseMySql(connectionstring.DefaultConnection, new MySqlServerVersion(new Version(8, 0, 21)));
    options.EnableSensitiveDataLogging();
});

builder.Services.AddScoped<ICustomValidator, CustomValidator>();

builder.Services.AddScoped<IJWTService, JWTService>();
builder.Services.AddScoped<IJWTManager, JWTManager>();
builder.Services.AddScoped<IAuthClientManager, AuthClientManager>();
builder.Services.AddScoped<IKeyPairStore, RSAKeyPairStore>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISecurityService, SecurityService>();

builder.Services.AddScoped(typeof(IRepositoryAsync<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddScoped<AuthClientRepository>();
builder.Services.AddScoped<CustomUserRepository>();

var redisConnection = builder.Configuration.GetSection("RedisConnection").Get<RedisConnection>();

builder.Services.AddSingleton<IConnectionMultiplexer>(x => ConnectionMultiplexer.Connect(new ConfigurationOptions
{
    AbortOnConnectFail = false,
    Ssl = redisConnection.UseSSL,
    ConnectRetry = 5,
    ConnectTimeout = 5000,
    SyncTimeout = 5000,
    DefaultDatabase = 0,
    EndPoints = { { redisConnection.Host, Convert.ToInt32(redisConnection.Port) } },
    Password = redisConnection.Password,
}));

builder.Services.AddSingleton<IGuard, Guard>();
builder.Services.AddSingleton<ICacheProvider, RedisCacheProvider>();
//builder.Services.AddSingleton(typeof(ILogger<>), typeof(ProspaLogger<>)) ;
builder.Services.AddScoped(typeof(ICacheExtensionDataManager<>), typeof(CacheExtensionDataManager<>));



builder.Services.AddControllers();

builder.Services.AddFluentValidation(conf =>
{
    conf.RegisterValidatorsFromAssembly(typeof(Program).Assembly);
    conf.AutomaticValidationEnabled = false;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Prospa Auth Token Service ",
        Version = "v1",
        Description = "REST Service for Token Authentication and Authorization"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header \r\n\r\n 
                      Enter your token in the text input below.
                      \r\n\r\nExample: '12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,

            },
            new List<string>()
        }
    });
});

builder.Host.UseSerilog(ProspaLoggerExtension.ConfigureLogger);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<AuthClientRequestValidator>();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
