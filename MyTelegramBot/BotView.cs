using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using SimonUtils.JsonUtil;

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
        private TaskCompletionSource<string> _currentQuestionTask;

        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(exception.ToJson());
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Console.WriteLine(update.ToJson());
            _botClient = botClient;
            _update = update;
            if (_update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                UserMessage = _update.Message;
                if (_state == BotState.WaitAnswer)
                {
                    if (UserMessage.MessageId != _oldMessageId)
                    {
                        _currentQuestionTask.SetResult(UserMessage.Text);
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
        public async Task SendMessageAsync(string message, CancellationToken cancellationToken = default)
        {
            await _botClient.SendTextMessageAsync(UserChat, message, cancellationToken: cancellationToken);
        }

        public async Task<string> AskAsync(string message, CancellationToken cancellationToken = default)
        {
            _oldMessageId = UserMessage.MessageId;
            await SendMessageAsync(message, cancellationToken);
            _state = BotState.WaitAnswer;
            _currentQuestionTask = new TaskCompletionSource<string>();
            cancellationToken.Register(() => _currentQuestionTask.SetCanceled());
            return await _currentQuestionTask.Task;
        }

        public BotView(List<RnA> rnaTable, List<Command> commandTable, string sam = "Я не знаю ответ на ваш вопрос. Пожалуйста, выразитесь конкретнее")
        {
            this._rnaTable = rnaTable;
            this._commandTable = commandTable;
            this.StandartAnswerMessage = sam;
        }
    }
}
