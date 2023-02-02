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
        static BotView botView = BotView.Instance;
        static MyUser currentUser;

        static List<Command> commandList = new List<Command>()
        {
            new Command("/start", StartExecutorAsync),
            new Command("/time", TimeExecutorAsync),
            new Command("/allusers", AllUsersExecutorAsync)
        };

        


        static void Main(string[] args)
        {
            botView = new BotView(commandList);
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
                    botView.HandleUpdateAsync,
                    botView.HandleErrorAsync,
                    receiverOptions,
                    cancellationToken
                );
                Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);
                Console.ReadLine();
            }
        }

        static async Task StartExecutorAsync()
        {
            const string WELCOME_MESSAGE = "Добро пожаловать, ";
            const string ASK_NICKNAME = "Вас приветствует MazeBot. Поскольку вы - новый пользователь, то сначала скажите ваш никнейм";
            const string ASK_TIMEZONE = "Укажите ваш часовой пояс\nНапример, если ваш часовой пояс - GMT(UTC)+2, то в ответе укажите просто 2\nЕсли GMT(UTC)-1, то в ответе укажите -1)";

            botView.UserChat = botView.UserMessage.Chat;
            var id = botView.UserMessage.From.Id;
            bool isUserExists = false;

            using (DatabaseContext dbContext = new DatabaseContext())
            {
                foreach (var user in dbContext.Users)
                {
                    if (user.Id == id)
                    {
                        isUserExists = true;
                        currentUser = user;
                        await botView.SendMessageAsync(WELCOME_MESSAGE + user.Nickname);
                        break;
                    }
                    
                }
                if (!isUserExists)
                {
                    string nickname = await botView.AskAsync(ASK_NICKNAME);
                    string timezone = await botView.AskAsync(ASK_TIMEZONE);
                    currentUser = new MyUser(id, nickname, timezone);
                    dbContext.Users.Add(currentUser);
                    dbContext.SaveChanges();
                    await botView.SendMessageAsync(WELCOME_MESSAGE + currentUser.Nickname);
                }
            }
        }
        static async Task TimeExecutorAsync()
        {
            var userTime = DateTimeOffset.Now.ToOffset(TimeSpan.FromHours(Convert.ToInt32(currentUser.Timezone)));
            await botView.SendMessageAsync(userTime.Hour.ToString() + ':' + userTime.Minute.ToString());
        }
        static async Task AllUsersExecutorAsync()
        {
            const string ALL_USERS = "Всего зарегистрированных пользователей: ";

            using (DatabaseContext dbContext = new DatabaseContext())
            {
                await botView.SendMessageAsync(ALL_USERS + dbContext.Users.Count().ToString());
            }
        }
    }
}