using AspNetCoreRateLimit;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using RateLimitedApi.Models;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var myOptions = new MyRateLimitOptions();
builder.Configuration.GetSection(MyRateLimitOptions.MyRateLimit).Bind(myOptions);
var policyName = "LoginPolicy";

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Configure rate limiting options
builder.Services.AddMemoryCache();

//builder.Services.AddRateLimiter(_ =>
//{
//    _
//    .AddFixedWindowLimiter(policyName: policyName, options =>
//    {
//        options.PermitLimit = 1;
//        options.Window = TimeSpan.FromSeconds(50);
//        //options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
//        //options.QueueLimit = 2;
//    });
//    _.RejectionStatusCode = 429;
//});

builder.Services.AddRateLimiter(_ =>
    {
        _.AddSlidingWindowLimiter(policyName: policyName, options =>
        {
            options.PermitLimit = myOptions.PermitLimit;
            options.Window = TimeSpan.FromSeconds(myOptions.Window);
            options.SegmentsPerWindow = myOptions.SegmentsPerWindow;
            //options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            //options.QueueLimit = myOptions.QueueLimit;
        });
        _.RejectionStatusCode = 429;
    });


//builder.Services.AddRateLimiter(_ =>
//{
//    _
//    .AddTokenBucketLimiter(policyName: policyName, options =>
//    {
//        options.TokenLimit = myOptions.TokenLimit;
//        //options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
//        //options.QueueLimit = myOptions.QueueLimit;
//        options.ReplenishmentPeriod = TimeSpan.FromSeconds(myOptions.ReplenishmentPeriod);
//        options.TokensPerPeriod = myOptions.TokensPerPeriod;
//        options.AutoReplenishment = myOptions.AutoReplenishment;
//    });
//    _.RejectionStatusCode = 429;
//});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});


app.UseHttpsRedirection();

app.UseAuthorization();

app.UseRateLimiter();
app.MapControllers();

app.Run();
