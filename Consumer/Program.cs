using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddKafkaConsumer<string, string>
(
    "messaging",
    consumer =>
    {
        consumer.Config.GroupId = "poc";
        consumer.Config.EnablePartitionEof = true;
        consumer.Config.AutoOffsetReset = AutoOffsetReset.Earliest;
        consumer.Config.EnableAutoOffsetStore = true;
    }
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet
    (
        "/receive",
        IEnumerable<Message<string, string>> ([FromServices] IConsumer<string, string> consumer) =>
        {
            consumer.Subscribe("poc");

            var received = new List<Message<string, string>>();
            while (consumer.Consume() is { IsPartitionEOF: false, Message: var message })
            {
                received.Add(message);
            }

            if (received.Any())
            {
                consumer.Commit();
            }
            consumer.Unsubscribe();

            return received;
        }
    )
.WithName("receive")
.WithOpenApi();

app.Run();
