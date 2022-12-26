using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;

namespace MyTelegramBot
{
    class Program
    {
        static string token = "5576822711:AAFTxUGk493hb8_-Voc0wbY0-9oSP-RoEf8";
        static ITelegramBotClient bot = new TelegramBotClient(token);

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            BotEditor.Instance = new BotEditor(botClient, update);
            BotEditor.Instance.AddRnA("/start", "Бот проснулся");
            BotEditor.Instance.AddRnA("Привет", "Привет!");
            BotEditor.Instance.AddRnA("Который час?", DateTimeOffset.Now.ToString("HH:mm"));
            BotEditor.Instance.Initialize();
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.ReadLine();
        }
    }
}