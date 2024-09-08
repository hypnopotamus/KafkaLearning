using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddKafkaProducer<string, string>("messaging");

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
app.MapPost
(
"/send",
        async Task<DeliveryResult<string, string>>([FromServices] IProducer<string, string> producer, string to, string message) =>
        await producer.ProduceAsync
        (
            "poc",
            new Message<string, string> { Key = to, Value = message }
        )
    )
    .WithName("send")
    .WithOpenApi();

app.Run();