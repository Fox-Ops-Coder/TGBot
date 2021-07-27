using DataAccessLayer.Model;
using System;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using TGBot.BotLogic.Commands;

namespace TGBot.BotLogic
{
    internal sealed class Bot
    {
        private readonly string botToken;
        private readonly TGContext tGContext;

        /// <summary>
        /// Переменная необходима для остановки бота
        /// </summary>
        private bool needStop;

        public Bot(TGContext tGContext, string botToken)
        {
            this.tGContext = tGContext;
            this.botToken = botToken;
        }

        /// <summary>
        /// Метод запускает бота и производит обработку поступивших сообщений
        /// </summary>
        private async void StartBot()
        {
            try
            {
                TelegramBotClient telegramBotClient = new(botToken);
                await telegramBotClient.SetWebhookAsync("");

                try
                {
                    await telegramBotClient.SendTextMessageAsync(BotCommands.myId, "Бот запущен");
                }
                catch (ApiRequestException e)
                {
                    Console.Error.WriteLine("При попытке отправить сообщение в модераторский чат произошла ошибка");
                    Console.Error.WriteLine(e);

                    needStop = true;
                }

                Console.Out.WriteLine("Бот запущен");

                UpdateHandler updateHandler = new(telegramBotClient, tGContext);

                while (!needStop)
                {
                    updateHandler.HandleMessages();
                }

                await telegramBotClient.CloseAsync();
                Console.Out.WriteLine("Бот остановлен");
            }
            catch (ApiRequestException e)
            {
                Console.Error.WriteLine(e);
            }
        }

        /// <summary>
        /// Метод производит запуск бота
        /// </summary>
        public void Start()
        {
            needStop = false;
            StartBot();

            string comand;

            do
            {
                comand = Console.In.ReadLine();

                switch (comand.ToLower())
                {
                    case "stop":
                        Console.Out.WriteLine("Бот останавливается...");
                        needStop = true;
                        break;

                    default:
                        Console.Out.WriteLine("Введите stop для остановки");
                        break;
                }
            } while (!needStop);
        }
    }
}