var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



//var kafkaBrokerSettings =  builder.Configuration
//                                  .GetSection(nameof(PocKafkaBrokerSettings))
//                                  .Get<Dictionary<string, PocKafkaBrokerSettings>>();


//foreach (var brokerSettings in kafkaBrokerSettings)
//{
//    var pockafka = builder
//        .Services.AddPocKafka(
//        brokerSettings.Key,
//        brokerSettings.Value.BootstrapServer!,
//        brokerSettings.Value.Username!,
//        brokerSettings.Value.Password!);

//    foreach (var topicSettings in brokerSettings.Value.Topics)
//        _ = pockafka.AddPocKafkaPubSub<string, MessageExampleProducer>(
//            KafkaTopic.ExampleProducer,
//            producerConfigAction: config =>
//            {
//                config.SetTopic(topicSettings.Key);
//            });
//}


var app = builder.Build();

if (app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.MapPost("/send", async (IPocKafkaPubSub<string, MessageExampleProducer> pubSubCartao, [FromBody] MessageExampleProducer message) =>
//{
//    try
//    {
//        var deliveryResult = await pubSubCartao.SendAsync(message);
//        if (deliveryResult.Status == PersistenceStatus.Persisted)
//        {
//            return Results.Ok(deliveryResult);
//        }
//        else
//        {
//            return Results.Problem($"Publish failed with status: {deliveryResult.Status}");
//        }

//    }
//    catch (Exception e)
//    {
//        Console.WriteLine($"Publish failed: {e.Message}.");
//        return Results.Problem($"Publish failed: {e.Message}");
//    }
//})
//.WithName("SendAsync");


//app.MapPost("/sendBatch", async (IPocKafkaPubSub<string, MessageExampleProducer> pubSubCartao, List<Message<string, MessageExampleProducer>> messages) =>
//{
//    try
//    {
//        var batchSendResult = await pubSubCartao.SendBatchAsync(messages);

//        if (batchSendResult.Failures.Count > 0)
//        {
//            foreach (var (message, exception) in batchSendResult.Failures)
//            {
//                Console.WriteLine($"Failed to send message: '{message.Value}' with key '{message.Key}'. Exception: {exception.Message}.");
//            }

//            return Results.Problem("Some messages failed to send. Check the server logs for details.");
//        }


//        return Results.Ok($"Batch of {messages.Count} messages sent successfully.");

//    }
//    catch (Exception e)
//    {
//        Console.WriteLine($"Batch publish failed: {e.Message}");
//        return Results.Problem($"Batch publish failed: {e.Message}");
//    }
//})
//.WithName("SendBatchAsync");

app.Run();

