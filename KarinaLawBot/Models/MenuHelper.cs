using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace KarinaLawBot.Models
{
    public static class MenuHelper
    {
        public static List<InlineKeyboardButton[]> GetMainMenuButtons()
        {
            return new List<InlineKeyboardButton[]>
            {
                new[] { InlineKeyboardButton.WithCallbackData("Брак и супружество") },
                new[] { InlineKeyboardButton.WithCallbackData("Развод") },
                new[] { InlineKeyboardButton.WithCallbackData("Дети") },
                new[] { InlineKeyboardButton.WithCallbackData("Родительские права") },
                new[] { InlineKeyboardButton.WithCallbackData("Имущественные вопросы") },
                new[] { InlineKeyboardButton.WithCallbackData("Юридическая консультация") },
                new[] { InlineKeyboardButton.WithCallbackData("Поиск информации") },
                new[] { InlineKeyboardButton.WithCallbackData("Шаблоны документов") },
                new[] { InlineKeyboardButton.WithCallbackData("О нас") }
            };
        }
    }
}
