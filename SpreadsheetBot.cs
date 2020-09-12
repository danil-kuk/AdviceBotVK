using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.IO;

namespace AdviceBotVK
{
    class SpreadsheetBot
    {
        static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static SheetsService service;

        public readonly string SpreadsheetId; // Id таблицы, с которой работаем
        public string Sheet { get; set; } // Cтраница таблицы, например "Лист 1"
        public string Range { get; private set; } // Диапазон

        /// <summary>
        /// Создание бота для Google Таблиц
        /// </summary>
        /// <param name="spreadsheetId">Id таблицы с которой нужно работать</param>
        public SpreadsheetBot(string credentialPath, string spreadsheetId)
            : this(credentialPath, spreadsheetId, "Лист1") { }
        public SpreadsheetBot(string credentialPath, string spreadsheetId, string sheetPage)
            : this(credentialPath, spreadsheetId, sheetPage, "A:Z") { }
        public SpreadsheetBot(string credentialPath, string spreadsheetId, string sheetPage, string range)
        {
            SpreadsheetId = spreadsheetId;
            Sheet = sheetPage;
            Range = $"{Sheet}!{range}";
            Connect(credentialPath);
        }

        /// <summary>
        /// Подключение к таблице
        /// </summary>
        private void Connect(string credentialPath)
        {
            GoogleCredential credential;
            using (var stream = new FileStream(credentialPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(Scopes);
            }

            // Создание Google Sheets API сервиса.
            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "SpreadsheetBot",
            });
        }

        public void AppendLineValue(IList<object> values)
        {
            AppendRangeValue(new List<IList<object>>
            {
                values
            });
        }

        /// <summary>
        /// Добавление новых значений в конец таблицы
        /// </summary>
        /// <param name="values">Новые значение, которые нужно добавить</param>
        public void AppendRangeValue(List<IList<object>> values)
        {
            var valueRange = new ValueRange
            {
                Values = values
            };
            AppendRequest(valueRange);
        }

        /// <summary>
        /// Запрос на добавление новых данных в конец таблицы
        /// </summary>
        /// <param name="range"></param>
        /// <param name="valueRange"></param>
        private void AppendRequest(ValueRange valueRange)
        {
            var appendRequest = service.Spreadsheets.Values.Append(valueRange, SpreadsheetId, Range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            try
            {
                appendRequest.ExecuteAsync();
            }
            catch (Exception)
            {
                //Если введен неверный id таблицы
                throw new ArgumentException("Не удается получить доступ к таблице с указанным id");
            }
        }
    }
}
