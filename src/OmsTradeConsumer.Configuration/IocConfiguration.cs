using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OmsTradeConsumer.Application.Services;
using OmsTradeConsumer.Application.Validators;
using OmsTradeConsumer.Domain.Interfaces.Services;
using OmsTradeConsumer.Domain.Models;
using OmsTradeConsumer.Infrastructure.FileTransfer;
using OmsTradeConsumer.Messaging.Configurations;
using OmsTradeConsumer.Messaging.Interfaces;
using OmsTradeConsumer.Messaging.Services;

namespace OmsTradeConsumer.Configuration;

public static class IocConfiguration
{
    public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .ConfigureApplicationServices()
            .ConfigureInfrastructure()
            .ConfigureMessaging()
            .ConfigureFluentValidation();

        return services;
    }

    private static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<ITradeService, TradeService>();

        return services;
    }

    private static IServiceCollection ConfigureMessaging(this IServiceCollection services)
    {
        services.AddSingleton<QueueConfiguration>();
        services.AddSingleton<IQueueListener, QueueListener>();

        return services;
    }

    private static IServiceCollection ConfigureInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IFileTransferService, FileTransferService>();

        return services;
    }

    private static IServiceCollection ConfigureFluentValidation(this IServiceCollection services)
    {
        services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<TradeModelValidator>());
        services.AddTransient<IValidator<TradeModel>, TradeModelValidator>();

        return services;
    }
}
