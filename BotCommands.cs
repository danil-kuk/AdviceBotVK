using System;
using System.Collections.Generic;
using System.Text;

namespace AdviceBotVK
{
    static class BotCommands
    {
        public static string Start { get; } = "Чем заняться?";
        public static string Back { get; } = "Назад";
        public static string OneMore { get; } = "Другое";
        public static string Rate { get; } = "Оценить бота";
        public static string BackToMenu { get; } = "Назад в меню";
    }

    static class StringExtension
    {
        public static bool EqualsBotCommand(this string strA, string strB)
        {
            return strA.Equals(strB, StringComparison.OrdinalIgnoreCase);
        }
    }
}
