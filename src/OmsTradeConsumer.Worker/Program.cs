using OmsTradeConsumer.Configuration;
using OmsTradeConsumer.Worker.BackgroundServices;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();
builder.Services.AddConfigurations(builder.Configuration);

var host = builder.Build();
host.Run();
