using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace AdviceBotVK
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new VkBotConfig
            {
                GroupId = 0, // Id группы,где будет работать бот.
                Token = "" // Ключ API для бота.
            };
            var sheetBot = new SpreadsheetBot(
                "google-credentials.json", // Путь к файлу с данными для интеграции с API Google Таблиц.
                "" // Id таблицы, с которой бот будет работать.
                );
            Console.WriteLine("SpreadsheetBot запущен");
            var vkBot = new VKApiBot(config, sheetBot);
            Console.WriteLine("AdviseBotVK запущен");
            vkBot.Listen();
        }
    }
}
