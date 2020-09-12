using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VkNet;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;

namespace AdviceBotVK
{
    class VKApiBot
    {
        public VkApi VK { get; private set; } = new VkApi();
        public ulong MyGroupId { get; private set; }
        public Dictionary<long, UserSession> AllUserSessions { get; private set; } = new Dictionary<long, UserSession>();
        public SpreadsheetBot Spreadsheet { get; set; }

        /// <summary>
        /// Создание бота для группы
        /// </summary>
        /// <param name="config">Настройки бота</param>
        public VKApiBot(VkBotConfig config, SpreadsheetBot spreadsheetBot = null)
        {
            MyGroupId = config.GroupId;
            Authorize(config.Token);
            Spreadsheet = spreadsheetBot;
        }

        /// <summary>
        /// Авторизация бота через LongPollApi
        /// </summary>
        /// <param name="token">Токен</param>
        private void Authorize(string token)
        {
            VK.Authorize(new ApiAuthParams() { AccessToken = token });
        }

        /// <summary>
        /// Получить все беседы без ответа
        /// </summary>
        /// <returns>Беседы, в которых нет ответа на последнее сообщение</returns>
        private GetConversationsResult GetConversations()
        {
            return VK.Messages.GetConversations(new GetConversationsParams
            {
                GroupId = MyGroupId,
                Count = 100,
                Filter = GetConversationFilter.Unanswered
            });
        }

        /// <summary>
        /// Отслеживание ботом всех событий сообщества
        /// </summary>
        public void Listen()
        {
            while (true)
            {
                var conversations = GetConversations();
                foreach (var message in conversations.Items.Select(c => c.LastMessage))
                {
                    var userId = message.FromId.Value;
                    var timeDelta = DateTime.UtcNow - message.Date;
                    // Бот реагирует только на сообщения полученные в течение последних 10 минут
                    // Иначе, он просто помечает их как прочитанные и отвеченные
                    if (timeDelta < TimeSpan.FromMinutes(15))
                    {
                        if (!AllUserSessions.ContainsKey(userId))
                            AllUserSessions.Add(userId, new UserSession(userId, this));
                        MessageResponse(AllUserSessions[userId], message);
                    }
                    else
                    {
                        VK.Messages.MarkAsAnsweredConversation(userId, true, MyGroupId);
                        VK.Messages.MarkAsRead(userId.ToString(), message.Id, (long)MyGroupId);
                    }
                }
            }
        }

        /// <summary>
        /// Реакция бота на сообщение
        /// </summary>
        /// <param name="userSession">Текущая сессия пользователя</param>
        /// <param name="message">Сообщение пользователя, на которое должен ответить бот</param>
        private void MessageResponse(UserSession userSession, Message message)
        {
            switch (userSession.CurrentDialogLayout)
            {
                case DialogLayout.Hello:
                    {
                        HelloActions(userSession, message);
                        break;
                    }
                case DialogLayout.RecommendationsMenu:
                    {
                        RecommendationMenuActions(userSession, message);
                        break;
                    }
                case DialogLayout.GenreSelection:
                    {
                        GenreSelectionActions(userSession, message);
                        break;
                    }
                case DialogLayout.AfterRecommendationPage:
                    {
                        AfterRecommendationActions(userSession, message);
                        break;
                    }
                case DialogLayout.Rating:
                    {
                        RatingActions(userSession, message);
                        break;
                    }
                case DialogLayout.WriteReview:
                    {
                        ReviewActions(userSession, message);
                        break;
                    }
            }
        }

        private void HelloActions(UserSession userSession, Message message)
        {
            var userId = userSession.SessionOwnerId;
            var command = message.Text;
            if (command.EqualsBotCommand(BotCommands.Start))
            {
                RecommendationMenuActions(userSession, message);
            }
            else
            {
                SendMessageToUser(userId, ResponsePhrases.HelloMessage, BotKeyboardsSchemes.StartKeyboard);
                userSession.SetDialogLayout(DialogLayout.Hello);
            }
        }

        private void RecommendationMenuActions(UserSession userSession, Message message)
        {
            var userId = userSession.SessionOwnerId;
            var command = message.Text;
            if (EnumExtension.TryParseEnumByDescription<Category, GenreData>(command, out Category selectedCategory))
            {
                userSession.SetSelectedCategory(selectedCategory);
                SendMessageToUser(userId, ResponsePhrases.SelectGenre, BotKeyboardsSchemes.GetGenreSelectionKeyboard(selectedCategory));
                userSession.SetDialogLayout(DialogLayout.GenreSelection);
            }
            else if (command.EqualsBotCommand(BotCommands.Back))
            {
                HelloActions(userSession, message);
            }
            else if (command.EqualsBotCommand(BotCommands.Rate))
            {
                RatingActions(userSession, message);
            }
            else
            {
                SendMessageToUser(userId, ResponsePhrases.RecommendationsMenuMessage, BotKeyboardsSchemes.RecommendationsMenu);
                userSession.SetDialogLayout(DialogLayout.RecommendationsMenu);
            }
        }

        private void GenreSelectionActions(UserSession userSession, Message message)
        {
            var userId = userSession.SessionOwnerId;
            var command = message.Text;
            if (command.EqualsBotCommand(BotCommands.Back))
            {
                SendMessageToUser(userId, ResponsePhrases.RecommendationsMenuMessage, BotKeyboardsSchemes.RecommendationsMenu);
                userSession.SetDialogLayout(DialogLayout.RecommendationsMenu);
                userSession.SetSelectedCategory(Category.None);
                return;
            }
            switch (userSession.SelectedCategory)
            {
                case Category.Movie:
                    GiveRecommendation<MovieGenre>(userSession, message, Recommendations.GetMovieAsync);
                    break;
                case Category.Series:
                    GiveRecommendation<SeriesGenre>(userSession, message, Recommendations.GetSeriesAsync);
                    break;
                case Category.Book:
                    GiveRecommendation<BookGenre>(userSession, message, Recommendations.GetBookAsync);
                    break;
                case Category.Game:
                    GiveRecommendation<GameGenre>(userSession, message, Recommendations.GetGameAsync);
                    break;
                default:
                    break;
            }
        }

        private async void GiveRecommendation<T>(UserSession userSession, Message message, Func<string, Task<RecommendationData>> recommender) where T : struct
        {
            var command = userSession.SelectedGenreApiString ?? message.Text;
            if (EnumExtension.TryParseEnumByDescription<T, GenreData>(command, out T genre))
            {
                userSession.SetSelectedGenreApiString(genre.GetEnumCustomAttribute<T, GenreData>().ApiString);
                await SendRecommendation(recommender, userSession,
                    ResponsePhrases.RecommendationMessage + userSession.SelectedCategory.GetEnumCustomAttribute<Category, GenreData>().Description.ToLower(),
                    userSession.SelectedGenreApiString);
            }
            else
            {
                await SendMessageToUser(userSession.SessionOwnerId, ResponsePhrases.SelectGenre, BotKeyboardsSchemes.GetGenreSelectionKeyboard(userSession.SelectedCategory));
            }
        }

        private async void AfterRecommendationActions(UserSession userSession, Message message)
        {
            var userId = userSession.SessionOwnerId;
            var command = message.Text;
            if (command.EqualsBotCommand(BotCommands.OneMore))
            {
                await SendRecommendation(userSession.PrevAction, userSession,
                    ResponsePhrases.RecommendationMessage + userSession.SelectedCategory.GetEnumCustomAttribute<Category, GenreData>().Description.ToLower(),
                    userSession.SelectedGenreApiString);
            }
            else if (command.EqualsBotCommand(BotCommands.Back))
            {
                await SendMessageToUser(userId, ResponsePhrases.RecommendationsMenuMessage, BotKeyboardsSchemes.RecommendationsMenu);
                userSession.SetDialogLayout(DialogLayout.RecommendationsMenu);
                userSession.SetSelectedGenreApiString(null);
            }
            else
            {
                await SendMessageToUser(userId, ResponsePhrases.Unknown, BotKeyboardsSchemes.AfterRecommendationMenu);
            }
        }

        private void RatingActions(UserSession userSession, Message message)
        {
            var userId = userSession.SessionOwnerId;
            var command = message.Text;
            if (EnumExtension.TryParseEnumByDescription<BotRating, EnumData>(command, out BotRating rating))
            {
                var mark = (int)rating;
                userSession.BotRating = mark;
                SendMessageToUser(userId, "Если у тебя есть время, то напиши свой отзыв:", BotKeyboardsSchemes.BackKeyboard);
                userSession.SetDialogLayout(DialogLayout.WriteReview);
            }
            else if (command.EqualsBotCommand(BotCommands.Back))
            {
                SendMessageToUser(userId, ResponsePhrases.RecommendationsMenuMessage, BotKeyboardsSchemes.RecommendationsMenu);
                userSession.SetDialogLayout(DialogLayout.RecommendationsMenu);
            }
            else
            {
                SendMessageToUser(userId, "Выбери, как ты можешь оценить мою работу:", BotKeyboardsSchemes.Rating);
                userSession.SetDialogLayout(DialogLayout.Rating);
            }
        }

        private void ReviewActions(UserSession userSession, Message message)
        {
            var userId = userSession.SessionOwnerId;
            var command = message.Text;
            if (command.EqualsBotCommand(BotCommands.BackToMenu))
            {
                SendMessageToUser(userId, "Спасибо, что поставил оценку!");
            }
            else
            {
                SendMessageToUser(userId, "Спасибо за оценку и отзыв!");
            }
            SendBotRating(userSession, command);
            RecommendationMenuActions(userSession, message);
        }

        private async Task SendRecommendation(Func<string, Task<RecommendationData>> recommender,
            UserSession userSession, string responsePhrase, string args = null)
        {
            var userId = userSession.SessionOwnerId;
            userSession.PrevAction = recommender;
            try
            {
                await SendMessageToUser(userId, ResponsePhrases.Search);
                var recommedation = await recommender(args);
                var responseMsg = $"{responsePhrase} \"{recommedation.Title}\"\n" +
                    $"Описание: {recommedation.Description}";
                var attachment = recommedation.ImageUrl != "" ? await Helpers.GetVkPhotoAttachmentFromImageUrl(userSession, recommedation.ImageUrl) : null;
                await SendMessageToUser(userId, responseMsg, BotKeyboardsSchemes.AfterRecommendationMenu, attachment);
            }
            catch (Exception)
            {
                await SendMessageToUser(userId, ResponsePhrases.Error, BotKeyboardsSchemes.AfterRecommendationMenu);
            }
            finally
            {
                userSession.SetDialogLayout(DialogLayout.AfterRecommendationPage);
            }
        }

        public Task SendBotRating(UserSession userSession, string review = null)
        {
            var userId = userSession.SessionOwnerId;
            var sheetData = new List<object>
            {
                DateTime.UtcNow.AddHours(5).ToString("dd.MM.yyyy"),
                $"=HYPERLINK(\"https://vk.com/id{userId}\";\"{userId}\")",
                userSession.BotRating,
                review
            };
            Spreadsheet.AppendLineValue(sheetData);
            userSession.BotRating = 0; // Для предотвращения повторной отправки оценки в конце сессии
            return Task.CompletedTask;
        }

        /// <summary>
        /// Отправить сообщение пользователю
        /// </summary>
        /// <param name="userId">Id получателя</param>
        /// <param name="message">Текст сообщения. Сообщение не может быть пустым!</param>
        /// <param name="keyboard">Клавиатура бота. По-умолчанию клавиатура отключена</param>
        /// <param name="attachments">Вложения для сообщения. Этот метод принимает только вложения из класса <see cref="MediaAttachment"/></param>
        private Task SendMessageToUser(long userId, string message,
            MessageKeyboard keyboard = null, IEnumerable<MediaAttachment> attachments = null)
        {
            var messageSendParams = new MessagesSendParams
            {
                UserId = userId,
                Message = message,
                Keyboard = keyboard,
                Attachments = attachments,
                RandomId = DateTime.Now.Millisecond
            };
            VK.Messages.Send(messageSendParams);
            return Task.CompletedTask;
        }
    }
}
