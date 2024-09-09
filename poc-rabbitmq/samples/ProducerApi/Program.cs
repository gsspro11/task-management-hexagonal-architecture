using Poc.RabbitMQ;
using Poc.RabbitMQ.Extensions;
using Poc.RabbitMQ.PubSub;
using Microsoft.AspNetCore.Mvc;
using Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var rabbitMQBrokerSettings = builder.Configuration.GetSection(nameof(PocRabbitMQBrokerConfig)).Get<Dictionary<string, PocRabbitMQBrokerConfig>>();
foreach (var brokerSettings in rabbitMQBrokerSettings)
{
    var pocRabbitMQ = builder
       .Services.AddPocRabbitMQ<RabbitQueue>(brokerSettings.Key, brokerSettings.Value);

    foreach (var queueSettings in brokerSettings.Value.Queues)
        _ = pocRabbitMQ.AddPocRabbitPubSub<CartaoMessage>(
            RabbitQueue.CartaoQueue,
            configAction: config =>
            {

            }
        );
}


var app = builder.Build();

if (app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHealthChecks("/healthz");

app.UseHttpsRedirection();

app.MapPost("/send", async (IPocRabbitMQPubSub<CartaoMessage> pubSubCartao, [FromBody] CartaoMessage message) =>
{
    try
    {
        await pubSubCartao.PublishAsync(new CartaoMessage() { Cvv = "958", Number = "5984 4568 6485 1556" },
               null, true);

        return Results.Ok();

    }
    catch (Exception e)
    {
        Console.WriteLine($"Publish failed: {e.Message}.");
        return Results.Problem($"Publish failed: {e.Message}");
    }
})
.WithName("SendAsync");


app.MapPost("/sendBatch", async (IPocRabbitMQPubSub<CartaoMessage> pubSubCartao, List<CartaoMessage> messages) =>
{
    try
    {
        await pubSubCartao.BatchPublishAsync(messages, null, true);

        return Results.Ok($"Batch of {messages.Count} messages sent successfully.");

    }
    catch (Exception e)
    {
        Console.WriteLine($"Batch publish failed: {e.Message}");
        return Results.Problem($"Batch publish failed: {e.Message}");
    }
})
.WithName("SendBatchAsync");

app.Run();

