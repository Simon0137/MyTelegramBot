using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using SimonUtils;

namespace MyTelegramBot
{
    class Program
    {
        static ITelegramBotClient bot;

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            BotView.Instance = new BotView(botClient, update, new List<RnA> { 
                new RnA("/start", "Бот проснулся"),
                new RnA("Привет", "Привет!"),
                new RnA("Который час?", DateTimeOffset.Now.ToString("HH:mm"))
            });
            BotView.Instance.Initialize();
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }


        static void Main(string[] args)
        {
            var cfg = JsonConverter.FromJsonFile<Config>("config.json");
            if (cfg == null)
            {
                Console.WriteLine("Конфигурационный файл по заданному пути не найден");
            }
            else
            {
                bot = new TelegramBotClient(cfg.Token);

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
                Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);
                Console.ReadLine();
            }
        }

        private static void AddRnA(string request, string answer)
        {

        }
    }
}