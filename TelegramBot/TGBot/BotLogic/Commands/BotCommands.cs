using System;
using Telegram.Bot.Types.ReplyMarkups;

namespace TGBot.BotLogic.Commands
{
    /// <summary>
    /// Класс содержит команды и шаблоны ответов для пользователей, которые не надо создавать динамически,
    /// а так же функции которые создают такие шаблоны
    /// </summary>
    internal sealed class BotCommands
    {
        public const long myId = 0;

        public const int dataLenght = 2;

        public const string StartCommand = "/start";
        public const string WhoCommand = "Кто я?";

        public const string AddCource = "AddCource";
        public const string AddVacancy = "AddVacancy";
        public const string Cancel = "Cancel";

        public const string Yes = "yes";
        public const string No = "no";

        public const string Stud = "Stud";
        public const string Tea = "Tea";
        public const string HR = "HR";

        public const string SelectedStudent = "SelectedStudent";
        public const string SelectedTecher = "SelectedTecher";
        public const string SelectedHR = "SelectedHR";

        public const string interesCource = "InteresCource";
        public const string interesVacancy = "InteresVacancy";

        public const string addCourceMessage = "Введите информацию о курсе в таком виде\nНазвание курса\nСсылка на курс";
        public const string addVacancyMessage = "Введите информацию о вакансии в таком виде\nНазвание вакансии\nСсылка";

        public static readonly ReplyKeyboardMarkup commandsKeyboard = new()
        {
            Keyboard = new KeyboardButton[][]
            {
                new KeyboardButton[]
                {
                    new KeyboardButton { Text = WhoCommand }
                }
            },
            ResizeKeyboard = true
        };

        public static readonly InlineKeyboardMarkup whoButtons = new(new InlineKeyboardButton[][]
        {
            new InlineKeyboardButton[]
            {
                new InlineKeyboardButton
                {
                    Text = "Студент",
                    CallbackData = SelectedStudent
                },

                new InlineKeyboardButton
                {
                    Text = "Преподаватель",
                    CallbackData = SelectedTecher
                }
            },

            new InlineKeyboardButton[]
            {
                new InlineKeyboardButton
                {
                    Text = "HR-Менеджер",
                    CallbackData = SelectedHR
                }
            }
        });

        public static InlineKeyboardMarkup GetInteresButtons(int arg)
        {
            InlineKeyboardMarkup inlineKeyboardMarkup = new(new InlineKeyboardButton[][]
            {
                new InlineKeyboardButton[]
                {
                    new InlineKeyboardButton
                    {
                        Text = "Курсы",
                        CallbackData = interesCource + "_" + arg.ToString()
                    }
                },

                new InlineKeyboardButton[]
                {
                    new InlineKeyboardButton
                    {
                        Text = "Вакансии",
                        CallbackData = interesVacancy + "_" + arg.ToString()
                    }
                }
            });

            return inlineKeyboardMarkup;
        }

        public static InlineKeyboardMarkup GetModeratorComands(Guid guid)
        {
            return new InlineKeyboardMarkup(new InlineKeyboardButton[][]
            {
                new InlineKeyboardButton[]
                {
                    new InlineKeyboardButton
                    {
                        Text = "Да",
                        CallbackData = Yes + "_" + guid.ToString()
                    },

                    new InlineKeyboardButton
                    {
                        Text = "Нет",
                        CallbackData = No + "_" + guid.ToString()
                    }
                }
            });
        }

        public static readonly InlineKeyboardMarkup backButton = new(new InlineKeyboardButton[][]
        {
            new InlineKeyboardButton[]
            {
                new InlineKeyboardButton
                {
                    Text = "Отмена",
                    CallbackData = Cancel
                }
            }
        });
    }
}