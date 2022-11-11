using Microsoft.AspNetCore.Mvc;
using OpenSalt.Authenticator_api;
using OpenSalt.Authenticator_core.Backend.Services;
using OpenSalt.Authenticator_core.Backend.Utils;
using OpenSalt.Authenticator_infrastructure.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUtilService, UtilService>(); 

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("api/encryptPassword", (string password, IAuthService _authSC) =>
{
    return _authSC.PasswordEncrypt(password);
});

//this data is comming from DB from user record
app.MapGet("api/decryptPassword", (string hashedPassword, string salt, IAuthService _authSC) =>
{
    return _authSC.PasswordDecrypt(hashedPassword, salt);
});

app.MapPost("api/getBearerAuthInformation", ([FromHeader] string clientKey, IAuthService _authSC) =>
{

    return ExceptionHandler.ExecuteAndCatching(() =>
    {
        return _authSC.GetBearerAuthInformation(clientKey);
    });

});

app.MapPost("api/generateToken", (List<KeyValuePair<string, string>> tokenInfo, IAuthService _authSC) =>
{
    // Func<TokenResponseModel> generateToken = () =>
    //{
    //    return AuthService.GenerateToken(tokenInfo);
    //};

    return ExceptionHandler.ExecuteAndCatching(() =>
    {
        return  _authSC.GenerateToken(tokenInfo);
    });
});

app.Run();


