using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using VkNet.Model;

namespace AdviceBotVK
{
    class UserSession
    {
        public long SessionOwnerId { get; }
        public Timer SessionTime { get; private set; }
        public VKApiBot VKBot { get; }
        public DialogLayout CurrentDialogLayout { get; private set; }
        public UploadServerInfo UploadServer { get; }
        public Func<string, Task<RecommendationData>> PrevAction { get; set; }
        public Category SelectedCategory { get; private set; }
        public string SelectedGenreApiString { get; private set; }
        public int BotRating { get; set; }

        public UserSession(long userId, VKApiBot bot)
        {
            SessionOwnerId = userId;
            VKBot = bot;
            CurrentDialogLayout = DialogLayout.Hello;
            UploadServer = bot.VK.Photo.GetMessagesUploadServer(userId);
            SetTimer();
        }

        public void SetDialogLayout(DialogLayout newDialogLayout)
        {
            CurrentDialogLayout = newDialogLayout;
        }

        public void SetSelectedCategory(Category category)
        {
            SelectedCategory = category;
        }

        public void SetSelectedGenreApiString(string genre)
        {
            SelectedGenreApiString = genre;
        }

        private void SetTimer()
        {
            SessionTime = new Timer(15 * 1000 * 60); // Длина сессии 15 минут
            SessionTime.Elapsed += OnTimedEvent;
            SessionTime.AutoReset = true;
            SessionTime.Enabled = true;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            SessionTime.Stop();
            if (BotRating > 0)
            {
                VKBot.SendBotRating(this);
            }
            VKBot.AllUserSessions.Remove(SessionOwnerId);
        }
    }
}
