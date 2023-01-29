using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using SimonUtils.JsonUtil;
using Microsoft.Data.Sqlite;

namespace MyTelegramBot
{
    class Program
    {
        static ITelegramBotClient bot;

        static List<RnA> requestAnswerList = new List<RnA>()
        {
            new RnA("Привет", "Привет!")
        };
        static List<Command> commandList = new List<Command>()
        {
            new Command("/start", StartExecutor),
            new Command("/time", TimeExecutor)
        };

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Console.WriteLine(update.ToJson());
            BotView.Instance = new BotView(botClient, update, requestAnswerList, commandList);
            BotView.Instance.Initialize();
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(exception.ToJson());
        }


        static void Main(string[] args)
        {
            var cfg = JsonConverter.FromJsonFile<Config>("config.json");
            if (cfg == null)
            {
                Console.WriteLine("Конфигурационный файл config.json не найден или некорректен");
            }
            else if (cfg.Token == null)
            {
                Console.WriteLine("Токен отсутствует в конфигурационном файле");
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

        static void StartExecutor()
        {
            const string WELCOME_MESSAGE = "Добро пожаловать, ";
            const string ASK_NICKNAME = "Добро пожаловать, ";
            const string ASK_TIMEZONE = "Добро пожаловать, ";

            var bv = BotView.Instance;
            var id = bv.UserMessage.From.Id;
            bool isUserExists = false;

            using (DatabaseContext dbContext = new DatabaseContext())
            {
                foreach (var user in dbContext.Users)
                {
                    if (user.Id == id)
                    {
                        bv.Answer(WELCOME_MESSAGE + user.Nickname);
                        isUserExists = true;
                        break;
                    }
                    
                }
                if (!isUserExists)
                {
                    string nickname = bv.Ask(ASK_NICKNAME);
                    string timezone = bv.Ask(ASK_TIMEZONE);
                    var user = new MyUser(id, nickname, timezone);
                    dbContext.Users.Add(user);
                    bv.Answer(WELCOME_MESSAGE + user.Nickname);
                }
            }
        }
        static void TimeExecutor()
        {
            BotView.Instance.Answer(DateTimeOffset.Now.ToString("HH:mm"));
        }
    }
}