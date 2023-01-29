using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Microsoft.Data.Sqlite;

namespace MyTelegramBot
{
    public class BotView
    {
        public static BotView Instance { get; set; }
        public string StandartAnswerMessage { get; set; }
        public Message UserMessage { get; set; }

        private List<RnA> _rnaTable; //Возможно удалю из-за узкого круга использования
        private List<Command> _commandTable;
        private ITelegramBotClient _botClient;
        private Update _update;
        private bool _isDialogComplete = true;

        public async void Initialize()
        {
            if (_update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = _update.Message;
                foreach (var command in _commandTable)
                {
                    if (message.Text == command.Text)
                    {
                        command.Execute();
                        break;
                    }
                }
                foreach (var item in _rnaTable)
                {
                    if (message.Text == item.Request)
                    {
                        await _botClient.SendTextMessageAsync(message.Chat, item.Answer);
                        break;
                    }
                }
                await _botClient.SendTextMessageAsync(message.Chat, StandartAnswerMessage);
            }
        }

        /*
         * Нужно придумать, как перехватить следующее сообщение от пользователя и вернуть его как результат
         * Также надо придумать, как получить чат с пользователем
         */
        public string Ask(string question)
        {
            return "";
        }

        //Нужно придумать, как получить чат с пользователем
        public async void Answer(string text)
        {
            
        }

        public BotView(ITelegramBotClient botClient, Update update, List<RnA> rnaTable, List<Command> commandTable, string sam = "Я не знаю ответ на ваш вопрос. Пожалуйста, выразитесь конкретнее")
        {
            this._botClient = botClient;
            this._update = update;
            this._rnaTable = rnaTable;
            this.StandartAnswerMessage = sam;
        }
    }
}
