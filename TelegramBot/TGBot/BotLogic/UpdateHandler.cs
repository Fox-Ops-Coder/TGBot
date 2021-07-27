using DataAccessLayer.Entities;
using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TGBot.BotLogic.BotTypes;
using TGBot.BotLogic.Commands;

namespace TGBot.BotLogic
{
    /// <summary>
    /// Класс получает и обрабатывает сообщения
    /// </summary>
    internal sealed class UpdateHandler
    {
        private readonly ITelegramBotClient telegramBotClient;
        private readonly TGContext tGContext;

        /// <summary>
        /// Список пользователей, которые хотят добавить новую вакансию или курс
        /// </summary>
        private readonly List<PendingAdd> pendingAdds;

        /// <summary>
        /// Список записей, ожидающих подтверждения модератором для добавления в базу данных
        /// </summary>
        private readonly List<PendingObjectAdd> pendingObjects;

        /// <summary>
        /// Пременная нужна для указания отступа при получении сообщений
        /// Без неё бот будет спамить сообщениями бесконечно
        /// </summary>
        private int offset = 0;

        public UpdateHandler(ITelegramBotClient telegramBotClient, TGContext tGContext)
        {
            this.telegramBotClient = telegramBotClient;
            this.tGContext = tGContext;

            pendingAdds = new List<PendingAdd>();
            pendingObjects = new List<PendingObjectAdd>();
        }

        /// <summary>
        /// Метод получает массив вакансий из базы данных
        /// </summary>
        /// <param name="ProfId">Id професии</param>
        /// <param name="tGContext">Контекст базы данных</param>
        /// <returns>Массив вакансий</returns>
        private static Cource[] GetCources(int ProfId, TGContext tGContext) =>
            tGContext.Cources.Select(cource => cource).Where(cource => cource.ProfessionId == ProfId).ToArray();

        /// <summary>
        /// Метод получает массив курсов из базы данных
        /// </summary>
        /// <param name="ProfId">Id професси</param>
        /// <param name="tGContext">Контекст базы данных</param>
        /// <returns>Массив курсов</returns>
        private static Vacancy[] GetVacancies(int ProfId, TGContext tGContext) =>
            tGContext.Vacancies.Select(vacancy => vacancy).Where(vacancy => vacancy.ProfessionId == ProfId).ToArray();

        /// <summary>
        /// Метод находит число в строке и парсит его
        /// </summary>
        /// <param name="source">Строка, в которой будет проводиться поиск числа</param>
        /// <returns>Число</returns>
        private static int GetNumber(string source)
        {
            StringBuilder currentNumber = new();
            foreach (char ch in source)
            {
                if (char.IsDigit(ch))
                    currentNumber.Append(ch);
            }

            int value = int.Parse(currentNumber.ToString());
            currentNumber.Clear();
            return value;
        }

        /// <summary>
        /// Метод возвращает строку, с шаблоном заполения.
        /// В случае если пользьзователь отправил не правильные данные
        /// Обычно, направильными данными считаются данные, в которых количество строк не равно 2
        /// </summary>
        /// <param name="pendingAdd">Тип добавляемого объекта</param>
        /// <returns>Строка с сообщением</returns>
        private static string ErrorMessage(Types pendingAdd) =>
            pendingAdd == Types.Cource ? BotCommands.addCourceMessage : BotCommands.addVacancyMessage;

        /// <summary>
        /// Метод создаёт новый объект на основе полученных данных
        /// </summary>
        /// <param name="pendingAdd"></param>
        /// <param name="data">Массив строк с данными</param>
        /// <returns>Новый объект для записы в базу данных</returns>
        private static PendingObjectAdd GetNewObject(PendingAdd pendingAdd, string[] data)
        {
            PendingObjectAdd newObject = new()
            {
                guid = Guid.NewGuid(),
                id = pendingAdd.id
            };

            switch (pendingAdd.type)
            {
                case Types.Cource:
                    newObject.addObject = new Cource
                    {
                        ProfessionId = pendingAdd.professionId,
                        CourceName = data[0],
                        Url = data[1]
                    };
                    newObject.type = Types.Cource;
                    break;

                case Types.Vacancy:
                    newObject.addObject = new Vacancy
                    {
                        ProfessionId = pendingAdd.professionId,
                        VacancyName = data[0],
                        Url = data[1]
                    };
                    newObject.type = Types.Vacancy;
                    break;
            }

            return newObject;
        }

        /// <summary>
        /// Метод обрабатывает текстовые сообщения
        /// </summary>
        /// <param name="message">Сообщение</param>
        private void HandleTextMessage(Message message)
        {
            /// <summary>
            /// Лямбда, обрабатывающая сообщения, содержащиеся в списке pendingAdds
            /// </summary>
            void pendingMessage(PendingAdd pendingAdd)
            {
                string[] data = message.Text.Split('\n');

                if (data.Length == BotCommands.dataLenght)
                {
                    PendingObjectAdd newObjcet = GetNewObject(pendingAdd, data);
                    InlineKeyboardMarkup moderatorButtons = BotCommands.GetModeratorComands(newObjcet.guid);

                    pendingObjects.Add(newObjcet);

                    switch (pendingAdd.type)
                    {
                        case Types.Cource:
                            telegramBotClient.SendTextMessageAsync(BotCommands.myId, "Запрос на добавление курса\n" + message.Text, replyMarkup: moderatorButtons);
                            telegramBotClient.SendTextMessageAsync(message.Chat.Id, "Ваша заявка на добавление курса отправлена модератору", replyMarkup: BotCommands.whoButtons);
                            break;

                        case Types.Vacancy:
                            telegramBotClient.SendTextMessageAsync(BotCommands.myId, "Запрос на добавление вакансии\n" + message.Text, replyMarkup: moderatorButtons);
                            telegramBotClient.SendTextMessageAsync(message.Chat.Id, "Ваша заявка на добавление вакансии отправлена модератору", replyMarkup: BotCommands.whoButtons);
                            break;
                    }

                    pendingAdds.Remove(pendingAdd);
                }
                else telegramBotClient.SendTextMessageAsync(message.Chat.Id, ErrorMessage(pendingAdd.type), replyMarkup: BotCommands.backButton);
            }

            /// <summary>
            /// Лямбда, обрабатывающая стандартные сообщения
            /// Сообщение является стандартным если его нет в списке pendingAdds
            /// </summary>
            void standartMessage()
            {
                switch (message.Text)
                {
                    case BotCommands.StartCommand:
                        telegramBotClient.SendTextMessageAsync(message.Chat.Id, "Добрый день", replyMarkup: BotCommands.commandsKeyboard);
                        break;

                    case BotCommands.WhoCommand:
                        telegramBotClient.SendTextMessageAsync(message.Chat.Id, "Кто вы?", replyMarkup: BotCommands.whoButtons);
                        break;

                    default:
                        telegramBotClient.SendTextMessageAsync(message.Chat.Id, "Не узнаю команду:(", replyMarkup: BotCommands.whoButtons);
                        break;
                }
            }

            switch (message.Type)
            {
                case MessageType.Text:
                    PendingAdd pendingAdd = pendingAdds.Find(pendingAdd => pendingAdd.id == message.Chat.Id);

                    if (pendingAdd != null) pendingMessage(pendingAdd);
                    else standartMessage();

                    break;

                default:
                    Console.Error.WriteLine("Содержание сообщения неизвестно: " + message.Type);
                    telegramBotClient.SendTextMessageAsync(message.Chat.Id, "Непонятно:(", replyMarkup: BotCommands.whoButtons);
                    break;
            }
        }

        /// <summary>
        /// Метод просто оборачивает введённую строку в скобки ""
        /// </summary>
        /// <param name="source">Строка, которую нужно обернуть</param>
        /// <returns>"Строка"</returns>
        private static string GetBrace(string source) => '"' + source + '"';

        /// <summary>
        /// Метод обрабатывает нажатия на кнопки
        /// </summary>
        /// <param name="callbackQuery">Данные по нажатой кнопке</param>
        private void HandleCallBackMessage(CallbackQuery callbackQuery)
        {
            /// <summary>
            /// Лямбда возвращает макет ответа для пользователя
            /// Что-то типа такого
            /// FRONTEND-Разработчик
            /// BACKEND-Разработчик
            /// ...
            /// </summary>
            InlineKeyboardMarkup getProfessions(string user)
            {
                Profession[] professions = tGContext.Professions.ToArray();
                int count = professions.Length;
                int index = 0;

                InlineKeyboardButton[][] inlineKeyboardButtons = new InlineKeyboardButton[count][];

                foreach (Profession profession in professions)
                {
                    inlineKeyboardButtons[index] = new InlineKeyboardButton[]
                    {
                       new InlineKeyboardButton
                       {
                           Text = profession.ProfessionName,
                           CallbackData = user +"_" + profession.ProfessionId.ToString()
                       }
                    };

                    ++index;
                }

                return new InlineKeyboardMarkup(inlineKeyboardButtons);
            }

            /// <summary>
            /// Лямбда обрабаытвае команды модератора
            /// </summary>
            void moderatorCommand(ModCommands modCommand, string[] data)
            {
                /// <summary>
                /// Лямбда добавлет указанный объект в базу данныех и возращает сообщение для пользователя
                /// </summary>
                string addToDb(object addObject, Types type)
                {
                    string userMessage = "";

                    switch (type)
                    {
                        case Types.Cource:
                            Cource cource = (Cource)addObject;
                            tGContext.Cources.Add(cource);
                            tGContext.SaveChangesAsync();
                            userMessage = "Ваш курс " + GetBrace(cource.CourceName) + " добавлен";
                            break;

                        case Types.Vacancy:
                            Vacancy vacancy = (Vacancy)addObject;
                            tGContext.Vacancies.Add(vacancy);
                            tGContext.SaveChangesAsync();
                            userMessage = "Ваша вакансия " + GetBrace(vacancy.VacancyName) + " добавлена";
                            break;
                    }

                    return userMessage;
                }

                /// <summary>
                /// Лямбда обрабатывает отказ модератора добавлять объект в бд и возращает сообщение об этом для пользователя
                /// </summary>
                string deny(object addObject, Types type)
                {
                    string userMessage = "";

                    switch (type)
                    {
                        case Types.Cource:
                            Cource cource = (Cource)addObject;
                            userMessage = "Ваш курс " + GetBrace(cource.CourceName) + " отклонён";
                            break;

                        case Types.Vacancy:
                            Vacancy vacancy = (Vacancy)addObject;
                            userMessage = "Ваша вакансия " + GetBrace(vacancy.VacancyName) + " отклонена";
                            break;
                    }

                    return userMessage;
                }

                // Данные необходиом разделять символом _, так как обычно они приходят в таком виде: команда_доп данные
                data = callbackQuery.Data.Split('_');
                Guid guid = Guid.Parse(data[1]);

                PendingObjectAdd PendingObjectAdd = pendingObjects.Find(addObject => addObject.guid.Equals(guid));

                if (PendingObjectAdd != null)
                {
                    switch (modCommand)
                    {
                        case ModCommands.Yes:
                            string userMessage = addToDb(PendingObjectAdd.addObject, PendingObjectAdd.type);

                            telegramBotClient.SendTextMessageAsync(BotCommands.myId, "Добавлено");
                            telegramBotClient.SendTextMessageAsync(PendingObjectAdd.id, userMessage);
                            Console.Out.WriteLineAsync("Объект добавлен");
                            break;

                        case ModCommands.No:
                            userMessage = deny(PendingObjectAdd.addObject, PendingObjectAdd.type);

                            telegramBotClient.SendTextMessageAsync(BotCommands.myId, "Отклонено");
                            telegramBotClient.SendTextMessageAsync(PendingObjectAdd.id, userMessage);
                            Console.Out.WriteLine("Объект не добавлен");
                            break;
                    }

                    pendingObjects.Remove(PendingObjectAdd);
                }
            }

            /// <summary>
            /// Лямбда обрабатывает нажатия на кнопки с необычными данными типа команда_данные
            /// </summary>
            void unusualCommand()
            {
                string[] data = callbackQuery.Data.Split("_");

                switch (data[0])
                {
                    case BotCommands.Stud:
                        int ProfId = GetNumber(callbackQuery.Data);
                        telegramBotClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id,
                            "Что вас интересует?", replyMarkup: BotCommands.GetInteresButtons(ProfId));
                        break;

                    case BotCommands.Tea:
                        InlineKeyboardMarkup addButton = new(new InlineKeyboardButton[][]
                        {
                            new InlineKeyboardButton[]
                            {
                                new InlineKeyboardButton
                                {
                                    Text = "Добавит курс",
                                    CallbackData = BotCommands.AddCource + "_" + GetNumber(callbackQuery.Data).ToString()
                                }}
                        });

                        telegramBotClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Что вас интересует?", replyMarkup: addButton);
                        break;

                    case BotCommands.HR:
                        addButton = new(new InlineKeyboardButton[][]
                        {
                            new InlineKeyboardButton[]
                            {
                                new InlineKeyboardButton
                                {
                                    Text = "Добавить вакансию",
                                    CallbackData = BotCommands.AddVacancy + "_" + GetNumber(callbackQuery.Data).ToString()
                                }
                            }
                        });

                        telegramBotClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Что вас интересует?", replyMarkup: addButton);
                        break;

                    case BotCommands.AddCource:
                        pendingAdds.Add(new PendingAdd
                        {
                            id = callbackQuery.Message.Chat.Id,
                            professionId = GetNumber(callbackQuery.Data),
                            type = Types.Cource
                        });

                        Console.Out.WriteLine("Ожидается заявка на добавление курса " + callbackQuery.Message.Chat.Username + " " + callbackQuery.Message.Chat.Id);
                        telegramBotClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, BotCommands.addCourceMessage, replyMarkup: BotCommands.backButton);
                        break;

                    case BotCommands.AddVacancy:
                        pendingAdds.Add(new PendingAdd
                        {
                            id = callbackQuery.Message.Chat.Id,
                            professionId = GetNumber(callbackQuery.Data),
                            type = Types.Vacancy
                        });

                        Console.Out.WriteLine("Ожидается заявка на добавление вакансии " + callbackQuery.Message.Chat.Username + " " + callbackQuery.Message.Chat.Id);
                        telegramBotClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, BotCommands.addVacancyMessage, replyMarkup: BotCommands.backButton);
                        break;

                    case BotCommands.interesCource:
                        ProfId = GetNumber(callbackQuery.Data);
                        Cource[] cources = GetCources(ProfId, tGContext);

                        if (cources.Length != 0)
                        {
                            new Task(() =>
                            {
                                Cource cource;

                                for (int index = 0; index < cources.Length - 1; ++index)
                                {
                                    cource = cources[index];
                                    telegramBotClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, cource.CourceName + "\n" + cource.Url);
                                    Thread.Sleep(200);
                                }

                                InlineKeyboardMarkup keyboardMarkup = new(new InlineKeyboardButton[][]
                                {
                                    new InlineKeyboardButton[]
                                    {
                                        new InlineKeyboardButton
                                        {
                                            Text = "Вакансии",
                                            CallbackData = BotCommands.interesVacancy + "_" + ProfId.ToString()
                                        }
                                    }
                                });

                                cource = cources[^1];
                                telegramBotClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, cource.CourceName + "\n" + cource.Url, replyMarkup: keyboardMarkup);
                            }).Start();
                        }
                        else telegramBotClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Курсов нет", replyMarkup: BotCommands.whoButtons);
                        break;

                    case BotCommands.interesVacancy:
                        ProfId = GetNumber(callbackQuery.Data);
                        Vacancy[] vacancies = GetVacancies(ProfId, tGContext);

                        if (vacancies.Length != 0)
                        {
                            new Task(() =>
                            {
                                Vacancy vacancy;

                                for (int index = 0; index < vacancies.Length - 1; ++index)
                                {
                                    vacancy = vacancies[index];
                                    telegramBotClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, vacancy.VacancyName + "\n" + vacancy.Url);
                                    Thread.Sleep(200);
                                }

                                InlineKeyboardMarkup keyboardMarkup = new(new InlineKeyboardButton[][]
                                {
                                    new InlineKeyboardButton[]
                                    {
                                        new InlineKeyboardButton
                                        {
                                            Text = "Курсы",
                                            CallbackData = BotCommands.interesCource + "_" + ProfId.ToString()
                                        }
                                    }
                                });

                                vacancy = vacancies[^1];
                                telegramBotClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, vacancy.VacancyName + "\n" + vacancy.Url, replyMarkup: keyboardMarkup);
                            }).Start();
                        }
                        else telegramBotClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Вакансий нет", replyMarkup: BotCommands.whoButtons);
                        break;

                    case BotCommands.Yes:
                        moderatorCommand(ModCommands.Yes, data);
                        break;

                    case BotCommands.No:
                        moderatorCommand(ModCommands.No, data);
                        break;
                }
            }

            PendingAdd pendingAdd = pendingAdds.Find(pending => pending.id == callbackQuery.Message.Chat.Id);

            if (pendingAdd == null)
            {
                switch (callbackQuery.Data)
                {
                    case BotCommands.SelectedStudent:
                        telegramBotClient
                            .SendTextMessageAsync(callbackQuery.Message.Chat.Id,
                            "Выберите изучаемую профессию", replyMarkup: getProfessions(BotCommands.Stud));
                        break;

                    case BotCommands.SelectedTecher:
                        telegramBotClient
                            .SendTextMessageAsync(callbackQuery.Message.Chat.Id,
                            "Выберите изучаемую профессию", replyMarkup: getProfessions(BotCommands.Tea));
                        break;

                    case BotCommands.SelectedHR:
                        telegramBotClient
                            .SendTextMessageAsync(callbackQuery.Message.Chat.Id,
                            "Выберите изучаемую профессию", replyMarkup: getProfessions(BotCommands.HR));
                        break;

                    default:
                        unusualCommand();
                        break;
                }
            }
            else if (callbackQuery.Data == BotCommands.Cancel)
            {
                pendingAdds.Remove(pendingAdd);
                telegramBotClient.SendTextMessageAsync(pendingAdd.id, "Заявка отменена", replyMarkup: BotCommands.whoButtons);
            }
            else if (BotCommands.myId != callbackQuery.Message.Chat.Id)
            {
                switch (pendingAdd.type)
                {
                    case Types.Cource:
                        telegramBotClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, BotCommands.addCourceMessage, replyMarkup: BotCommands.backButton);
                        break;

                    case Types.Vacancy:
                        telegramBotClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, BotCommands.addVacancyMessage, replyMarkup: BotCommands.backButton);
                        break;
                }
            }
        }

        /// <summary>
        /// Обрабатывает сообщение
        /// </summary>
        /// <param name="update">Сообщение</param>
        private void HandleUpdate(Update update)
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    Console.Out.WriteLine(update.Message.From.Username + " " + update.Message.Chat.Id + ": " + update.Message.Text);
                    HandleTextMessage(update.Message);
                    break;

                case UpdateType.CallbackQuery:
                    Console.Out.WriteLine(update.CallbackQuery.From.Username + " " + update.CallbackQuery.From.Id + ": " + update.CallbackQuery.Data);
                    HandleCallBackMessage(update.CallbackQuery);
                    break;

                default:
                    Console.Error.WriteLine("Пришло сообщение неизвестного содеражения: " + update.Type);
                    break;
            }
        }

        /// <summary>
        /// Получает новые сообщения
        /// </summary>
        public void HandleMessages()
        {
            Task<Update[]> updateTask = telegramBotClient.GetUpdatesAsync(offset);
            updateTask.Wait();

            Update[] updates = updateTask.Result;

            if (updates.Length != 0)
            {
                foreach (Update update in updates)
                {
                    try
                    {
                        HandleUpdate(update);
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine("Исключение\n" + e);
                    }
                }

                offset = updates[^1].Id + 1;
            }
        }
    }
}