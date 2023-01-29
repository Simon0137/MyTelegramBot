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
        public Chat UserChat { get; set; }

        private List<RnA> _rnaTable; //Возможно удалю из-за узкого круга использования
        private List<Command> _commandTable;
        private ITelegramBotClient _botClient;
        private Update _update;
        private bool _isDialogComplete = true;

        public async void Initialize()
        {
            if (_update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                UserMessage = _update.Message;
                foreach (var command in _commandTable)
                {
                    if (UserMessage.Text == command.Text)
                    {
                        command.Execute();
                        break;
                    }
                }
                foreach (var item in _rnaTable)
                {
                    if (UserMessage.Text == item.Request)
                    {
                        SendMessage(item.Answer);
                        break;
                    }
                }
                SendMessage(StandartAnswerMessage);
            }
        }

        public async void SendMessage(string message)
        {
            await _botClient.SendTextMessageAsync(UserChat, message);
        }

        public async void Ask(string message)
        {
            SendMessage(message);
            var oldMessId = UserMessage.MessageId;
            //Выйти из метода лишь тогда, когда пользователь напишет новое сообщение, или же ID сообщения изменится
        }

        public BotView(ITelegramBotClient botClient, Update update, List<RnA> rnaTable, List<Command> commandTable, string sam = "Я не знаю ответ на ваш вопрос. Пожалуйста, выразитесь конкретнее")
        {
            this._botClient = botClient;
            this._update = update;
            this._rnaTable = rnaTable;
            this._commandTable = commandTable;
            this.StandartAnswerMessage = sam;
        }
    }
}
