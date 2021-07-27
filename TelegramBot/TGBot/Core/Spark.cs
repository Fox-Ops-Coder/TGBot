using DataAccessLayer.Data;
using DataAccessLayer.Model;
using System;
using TGBot.BotLogic;

namespace TGBot.Core
{
    internal sealed class Spark
    {
        private static void Bootstrap(string pgConnection, string botToken)
        {
            Console.Out.WriteLine("Выполняется подулючение к базе данных...");
            TGContext tGContext = new(pgConnection);

            if (DataFill.CreateDb(tGContext))
            {
                Console.Out.WriteLine("Подключение установлено\nБот запускается...");

                Bot bot = new(tGContext, botToken);
                bot.Start();
            }
            else Console.Error.WriteLine("Ошибка при подключении");
        }

        /// <summary>
        /// Точка входа в проложение
        /// </summary>
        /// <param name="args">Две строки: 1. Строка подключения к бд 2. Токен бота</param>
        public static void Main(string[] args)
        {
            switch (args.Length)
            {
                case 2:
                    Bootstrap(args[0], args[1]);
                    break;

                case 1:
                    Console.Error.WriteLine("Недостаточно параметров");
                    break;

                case 0:
                    Console.Error.WriteLine("Для работы нужно ввести строку подлючения к серверу и токен бота");
                    break;

                default:
                    Console.Error.WriteLine("Получено больше двух параметров!");
                    break;
            }
        }
    }
}