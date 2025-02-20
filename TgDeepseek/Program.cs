using Codeblaze.SemanticKernel.Connectors.Ollama;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TgDeepseek.TelegramOptions;

using var cts = new CancellationTokenSource();
var bot = new TelegramBotClient(TelegramOptions.Token, cancellationToken: cts.Token);

var builder = Kernel.CreateBuilder()
    .AddOllamaChatCompletion("deepseek-r1:1.5b",
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
        File.ReadAllText("B:\\applications\\telegram\\TgDeepseek\\TgDeepseek\\messages\\message_start.txt"));
}

async Task OnMessage(Message msg, UpdateType type)
{
    if (msg.Text == "/start")
    {
        await SendInfoMessage(msg);
    }

    var input = msg.Text;
    var response = await kernel.InvokePromptAsync(input);
    bot.SendMessage(msg.Chat.Id, response.ToString());
}


async Task OnUpdate(Update update)
{
    if (update is { CallbackQuery: { } query })
    {
        await bot.AnswerCallbackQuery(query.Id,
            $"You picked {query.Data}");
        await bot.SendMessage(query.Message!.Chat,
            $"User {query.From} clicked on {query.Data}");
    }
}

