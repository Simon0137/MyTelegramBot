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

        private List<Command> _commandTable;
        private ITelegramBotClient _botClient;
        private Update _update;
        private BotState _state = BotState.WaitNewDialog;
        private long _oldMessageId = 0;
        private TaskCompletionSource<string> _currentQuestionTask;
        private bool _isCommandExists;

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
                _isCommandExists = false;
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
                                _isCommandExists = true;
                                command.Execute();
                                break;
                            }
                        }
                    }
                    if (!_isCommandExists)
                    {
                        await SendMessageAsync(StandartAnswerMessage);
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

        public BotView(List<Command> commandTable, string sam = "Ваша команда некорректна. Пожалуйста, проверьте, правильно ли вы ее написали или существует ли она")
        {
            this._commandTable = commandTable;
            this.StandartAnswerMessage = sam;
        }
    }
}
