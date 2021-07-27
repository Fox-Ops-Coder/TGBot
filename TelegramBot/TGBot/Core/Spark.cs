using DataAccessLayer.Data;
using DataAccessLayer.Model;
using System;
using TGBot.BotLogic;

namespace TGBot.Core
{
    internal sealed class Spark
    {
        private static void Bootstrap(string pgConnection, string botToken, long chanelId)
        {
            Console.Out.WriteLine("Выполняется подулючение к базе данных...");
            TGContext tGContext = new(pgConnection);

            if (DataFill.CreateDb(tGContext))
            {
                Console.Out.WriteLine("Подключение установлено\nБот запускается...");

                BotLogic.Commands.BotCommands.myId = chanelId;

                Bot bot = new(tGContext, botToken);
                bot.Start();
            }
            else Console.Error.WriteLine("Ошибка при подключении");
        }

        /// <summary>
        /// Точка входа в проложение
        /// </summary>
        /// <param name="args">Три строки: 1. Строка подключения к бд 2. Токен бота 3. Id канала</param>
        public static void Main(string[] args)
        {
            const int argsCount = 3;

            switch (args.Length)
            {
                case argsCount:
                    Bootstrap(args[0], args[1], long.Parse(args[2]));
                    break;

                default:
                    if (args.Length == 0) Console.Error.WriteLine("Для работы нужно ввести строку подлючения к серверу, токен бота и id канала");
                    else if (args.Length == 1 || args.Length == 2) Console.Error.WriteLine("Недостаточно параметров");
                    else if (args.Length > argsCount) Console.Error.WriteLine("Получено больше трёх параметров!");
                    break;
            }
        }
    }
}