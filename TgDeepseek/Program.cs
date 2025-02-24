using Codeblaze.SemanticKernel.Connectors.Ollama;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TgDeepseek.TelegramOptions;

using var cts = new CancellationTokenSource();
var bot = new TelegramBotClient(TelegramOptions.Token, cancellationToken: cts.Token);
var workstate = false;
var builder = Kernel.CreateBuilder()
    .AddOllamaChatCompletion("deepseek-r1:7b",
        "http://localhost:11434");

builder.Services.AddScoped<HttpClient>();
var kernel = builder.Build();

bot.OnError += OnError;
bot.OnMessage += OnMessage;
bot.OnUpdate += OnUpdate;


while (cts.IsCancellationRequested == false)
{
    var me = await bot.GetMe();
}

async Task OnError(Exception exception, HandleErrorSource source)
{
    Console.WriteLine(exception);
}

async Task SendInfoMessage(Message msg)
{
    bot.SendMessage(msg.Chat.Id,
        File.ReadAllText("B:\\applications\\telegram\\TgDeepseek\\TgDeepseek\\messages\\message_start.txt"),
        replyMarkup: new string[] { "расскажи о себе", "возобновить", "стоп" });
}

async Task OnMessage(Message msg, UpdateType type)
{
    var message = msg.Text;

    switch (message)
    {
        case ("/start"):
        {
            workstate = true;
            await SendInfoMessage(msg);
            break;
        }
        case ("возобновить"):
        {
            workstate = true;
            await SendInfoMessage(msg);
            break;
        }
        case ("стоп"):
        {
            workstate = false;
            break;
        }
    }

    if (workstate && message != "возобновить")
    {
        bot.SendMessage(msg.Chat.Id, await GetResponse(message));
    }
}


async Task<string> GetResponse(string? input)
{
    Console.WriteLine(input);
    var response = await kernel.InvokePromptAsync(input);
    return response.ToString();
}


async Task OnUpdate(Update update)
{
    switch (update)
    {
        case { Message: { } msg }: await OnMessage(msg, update.Type); break;
        case { CallbackQuery: { } cbQuery }: await HandleCallbackQuery(cbQuery); break;
    }
}

async Task HandleCallbackQuery(CallbackQuery callbackQuery)
{
}