﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using JsonUtil;

namespace MyTelegramBot
{
    class Program
    {
        static ITelegramBotClient bot;

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
            var cfg = JsonConverter.FromJson<Config>("C:/Projects/C#/MyTelegramBot/config.json");
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
}