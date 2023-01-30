using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

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
        private BotState _state = BotState.WaitNewDialog;
        private long _oldMessageId = 0;

        

        public async Task Initialize()
        {
            if (_update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                UserMessage = _update.Message;
                if (_state == BotState.WaitAnswer)
                {
                    if (UserMessage.MessageId != _oldMessageId)
                    {
                        _state = BotState.WaitNewDialog;
                    }
                }
                else
                {
                    if (UserMessage.Text != null)
                    {
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
                                await SendMessageAsync(item.Answer);
                                break;
                            }
                        }
                        if (UserChat != null)
                        {
                            await SendMessageAsync(StandartAnswerMessage);
                        }
                        else
                        {
                            _commandTable.First().Execute();
                        }
                    }
                }
            }
        }

        public async Task SendMessageAsync(string message)
        {
            await _botClient.SendTextMessageAsync(UserChat, message);
        }

        public async Task<string> AskAsync(string message)
        {
            _oldMessageId = UserMessage.MessageId;
            await SendMessageAsync(message);
            _state = BotState.WaitAnswer;
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
            Task<string> task = tcs.Task;
            await Task.Factory.StartNew(() =>
            {
                if (_state == BotState.WaitNewDialog)
                {
                    tcs.SetResult(UserMessage.Text);
                }
            });
            Stopwatch sw = Stopwatch.StartNew();
            var result = task.Result;
            sw.Stop();
            return result;
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
