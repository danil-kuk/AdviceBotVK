using System;
using System.Collections.Generic;
using System.Text;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.Keyboard;

namespace AdviceBotVK
{
    static class BotKeyboardsSchemes
    {
        /// <summary>
        /// Стартовая клавиатура бота
        /// </summary>
        public static readonly MessageKeyboard StartKeyboard = new KeyboardBuilder()
            .AddButton(BotCommands.Start, "", KeyboardButtonColor.Primary)
            .SetInline(false).SetOneTime()
            .Build();

        /// <summary>
        /// Клавиатура с одной кнопкой назад
        /// </summary>
        public static readonly MessageKeyboard BackKeyboard = new KeyboardBuilder()
            .AddButton(BotCommands.BackToMenu, "")
            .SetInline(false).SetOneTime()
            .Build();

        /// <summary>
        /// Клавиатура бота, которая появляется после рекомендации
        /// </summary>
        public static readonly MessageKeyboard AfterRecommendationMenu = new KeyboardBuilder()
            .AddButton(BotCommands.OneMore, "", KeyboardButtonColor.Primary)
            .AddButton(BotCommands.Back, "")
            .SetInline(false).SetOneTime()
            .Build();

        /// <summary>
        /// Клавиатура с выбором категории для рекомендации
        /// </summary>
        public static readonly MessageKeyboard RecommendationsMenu = new KeyboardBuilder()
            .AddButton(Category.Movie.GetEnumCustomAttribute<Category, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddButton(Category.Series.GetEnumCustomAttribute<Category, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddButton(Category.Book.GetEnumCustomAttribute<Category, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddButton(Category.Game.GetEnumCustomAttribute<Category, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddLine()
            .AddButton(BotCommands.Back, "")
            .AddButton(BotCommands.Rate, "")
            .SetInline(false).SetOneTime()
            .Build();

        /// <summary>
        /// Клавиатура с оценкой бота
        /// </summary>
        public static readonly MessageKeyboard Rating = new KeyboardBuilder()
            .AddButton(BotRating.A.GetEnumCustomAttribute<BotRating, EnumData>().Description, "")
            .AddButton(BotRating.B.GetEnumCustomAttribute<BotRating, EnumData>().Description, "")
            .AddButton(BotRating.C.GetEnumCustomAttribute<BotRating, EnumData>().Description, "")
            .AddButton(BotRating.D.GetEnumCustomAttribute<BotRating, EnumData>().Description, "")
            .AddButton(BotRating.F.GetEnumCustomAttribute<BotRating, EnumData>().Description, "")
            .AddLine()
            .AddButton(BotCommands.Back, "")
            .SetInline(false).SetOneTime()
            .Build();

        // Клавиатуры выбора жанра в категории
        public static MessageKeyboard GetGenreSelectionKeyboard(Category category)
        {
            MessageKeyboard result = null;
            switch (category)
            {
                case Category.None:
                    break;
                case Category.Movie:
                    result = MovieGenres;
                    break;
                case Category.Series:
                    result = SeriesGenres;
                    break;
                case Category.Book:
                    result = BooksGenres;
                    break;
                case Category.Game:
                    result = GamesGenres;
                    break;
                default:
                    break;
            }
            return result;
        }

        private static readonly MessageKeyboard MovieGenres = new KeyboardBuilder()
            .AddButton(MovieGenre.Action.GetEnumCustomAttribute<MovieGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddButton(MovieGenre.Comedy.GetEnumCustomAttribute<MovieGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddButton(MovieGenre.Drama.GetEnumCustomAttribute<MovieGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddLine()
            .AddButton(MovieGenre.Horror.GetEnumCustomAttribute<MovieGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddButton(MovieGenre.Fantasy.GetEnumCustomAttribute<MovieGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddButton(MovieGenre.Detective.GetEnumCustomAttribute<MovieGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddLine()
            .AddButton(MovieGenre.Thriller.GetEnumCustomAttribute<MovieGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddButton(MovieGenre.Cartoon.GetEnumCustomAttribute<MovieGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddButton(MovieGenre.Documentary.GetEnumCustomAttribute<MovieGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddLine()
            .AddButton(MovieGenre.Random.GetEnumCustomAttribute<MovieGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddButton(BotCommands.Back, "")
            .SetInline(false).SetOneTime()
            .Build();

        private static readonly MessageKeyboard SeriesGenres = new KeyboardBuilder()
            .AddButton(SeriesGenre.Comedy.GetEnumCustomAttribute<SeriesGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddButton(SeriesGenre.Drama.GetEnumCustomAttribute<SeriesGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddButton(SeriesGenre.Thriller.GetEnumCustomAttribute<SeriesGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddLine()
            .AddButton(SeriesGenre.Anime.GetEnumCustomAttribute<SeriesGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddButton(SeriesGenre.Horror.GetEnumCustomAttribute<SeriesGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddButton(SeriesGenre.Action.GetEnumCustomAttribute<SeriesGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddLine()
            .AddButton(SeriesGenre.Horror.GetEnumCustomAttribute<SeriesGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddButton(SeriesGenre.Fantasy.GetEnumCustomAttribute<SeriesGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddButton(SeriesGenre.Criminal.GetEnumCustomAttribute<SeriesGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddLine()
            .AddButton(SeriesGenre.Random.GetEnumCustomAttribute<SeriesGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddButton(BotCommands.Back, "")
            .SetInline(false).SetOneTime()
            .Build();

        private static readonly MessageKeyboard BooksGenres = new KeyboardBuilder()
            .AddButton(BookGenre.Classics.GetEnumCustomAttribute<BookGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddButton(BookGenre.Buisness.GetEnumCustomAttribute<BookGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddButton(BookGenre.Fantasy.GetEnumCustomAttribute<BookGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddLine()
            .AddButton(BookGenre.Fantastic.GetEnumCustomAttribute<BookGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddButton(BookGenre.Adventure.GetEnumCustomAttribute<BookGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddButton(BookGenre.Psycology.GetEnumCustomAttribute<BookGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddLine()
            .AddButton(BookGenre.Mistery.GetEnumCustomAttribute<BookGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddButton(BookGenre.History.GetEnumCustomAttribute<BookGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddButton(BookGenre.Humor.GetEnumCustomAttribute<BookGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddLine()
            .AddButton(BookGenre.Random.GetEnumCustomAttribute<BookGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddButton(BotCommands.Back, "")
            .SetInline(false).SetOneTime()
            .Build();

        private static readonly MessageKeyboard GamesGenres = new KeyboardBuilder()
            .AddButton(GameGenre.Action.GetEnumCustomAttribute<GameGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddButton(GameGenre.RPG.GetEnumCustomAttribute<GameGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddButton(GameGenre.Strategy.GetEnumCustomAttribute<GameGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddLine()
            .AddButton(GameGenre.Indie.GetEnumCustomAttribute<GameGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddButton(GameGenre.Adventure.GetEnumCustomAttribute<GameGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddButton(GameGenre.Casual.GetEnumCustomAttribute<GameGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddLine()
            .AddButton(GameGenre.Simulation.GetEnumCustomAttribute<GameGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddButton(GameGenre.Sport.GetEnumCustomAttribute<GameGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddButton(GameGenre.Free.GetEnumCustomAttribute<GameGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddLine()
            .AddButton(GameGenre.Random.GetEnumCustomAttribute<GameGenre, GenreData>().Description, "", KeyboardButtonColor.Primary)
            .AddButton(BotCommands.Back, "")
            .SetInline(false).SetOneTime()
            .Build();
    }
}
