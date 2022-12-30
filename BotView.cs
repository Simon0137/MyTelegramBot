﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

namespace MyTelegramBot
{
    public class BotView
    {
        public static BotView Instance { get; set; }
        public string StandartAnswerMessage { get; set; }

        private List<RnA> _rnaTable;
        private ITelegramBotClient _botClient;
        private Update _update;

        public async void Initialize()
        {
            if (_update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = _update.Message;
                foreach (var item in _rnaTable)
                {
                    if (message.Text == item.Request)
                    {
                        await _botClient.SendTextMessageAsync(message.Chat, item.Answer);
                        return;
                    }
                }
                await _botClient.SendTextMessageAsync(message.Chat, StandartAnswerMessage);
            }
        }

        public BotView(ITelegramBotClient botClient, Update update, List<RnA> rnaTable, string sam = "Я не знаю ответ на ваш вопрос. Пожалуйста, выразитесь конкретнее")
        {
            this._botClient = botClient;
            this._update = update;
            this._rnaTable = rnaTable;
            this.StandartAnswerMessage = sam;
        }
    }
}
