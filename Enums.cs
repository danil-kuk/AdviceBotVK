using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace AdviceBotVK
{
    /// <summary>
    /// Основные диалоговые окна бота. Нужно для удобства, чтобы понимать, на каком окне сейчас находится пользователь.
    /// </summary>
    enum DialogLayout
    {
        Hello, // Пользователь находится на экране приветсвия бота
        RecommendationsMenu, // Пользователь выбирает категорию
        GenreSelection, // Пользователь выбирает жанр
        AfterRecommendationPage, // Пользователь получил рекомендацию
        Rating, // Пользователь ставит оценку боту
        WriteReview // Пользователь пишет отзыв о боте
    }

    enum BotRating
    {
        [EnumData("😍")]
        A = 5,
        [EnumData("😀")]
        B = 4,
        [EnumData("😐")]
        C = 3,
        [EnumData("😕")]
        D = 2,
        [EnumData("😟")]
        F = 1
    }

    /// <summary>
    /// Категории
    /// </summary>
    enum Category
    {
        None,
        [GenreData("Фильм")] Movie,
        [GenreData("Сериал")] Series,
        [GenreData("Книга")] Book,
        [GenreData("Игра")] Game
    }

    enum MovieGenre
    {
        [GenreData("Случайный фильм")]
        Random,
        [GenreData("Боевик", "with_genres=28")]
        Action,
        [GenreData("Мультфильм", "with_genres=16")]
        Cartoon,
        [GenreData("Комедия", "with_genres=35")]
        Comedy,
        [GenreData("Драма", "with_genres=18")]
        Drama,
        [GenreData("Хоррор", "with_genres=27")]
        Horror,
        [GenreData("Триллер", "with_genres=53")]
        Thriller,
        [GenreData("Фантастика", "878")]
        Fantasy,
        [GenreData("Детектив", "with_genres=9648")]
        Detective,
        [GenreData("Документальный", "with_genres=99")]
        Documentary
    }

    enum SeriesGenre
    {
        [GenreData("Случайный сериал")]
        Random,
        [GenreData("Комедийные", "genre=9")]
        Comedy,
        [GenreData("Драмы", "genre=3")]
        Drama,
        [GenreData("Аниме", "genre=39")]
        Anime,
        [GenreData("Хорроры", "genre=19")]
        Horror,
        [GenreData("Документальные", "genre=11")]
        Documentary,
        [GenreData("Криминальные", "genre=10")]
        Criminal,
        [GenreData("Фэнтези", "genre=13")]
        Fantasy,
        [GenreData("Триллеры", "genre=32")]
        Thriller,
        [GenreData("Экшены", "genre=5")]
        Action
    }

    enum BookGenre
    {
        [GenreData("Случайная книга")]
        Random,
        [GenreData("Бизнес", "niches=1")]
        Buisness,
        [GenreData("Фантастика", "niches=2")]
        Fantastic,
        [GenreData("Фэнтези", "niches=22")]
        Fantasy,
        [GenreData("Психология", "niches=24")]
        Psycology,
        [GenreData("Классика", "niches=31")]
        Classics,
        [GenreData("Мистика", "niches=164")]
        Mistery,
        [GenreData("История", "niches=230")]
        History,
        [GenreData("Юмор", "niches=13")]
        Humor,
        [GenreData("Приключения", "niches=5")]
        Adventure
    }

    enum GameGenre
    {
        [GenreData("Случайная игра")]
        Random,
        [GenreData("Экшен", "genre=1")]
        Action,
        [GenreData("Казуальные", "genre=4")]
        Casual,
        [GenreData("Стратегия", "genre=2")]
        Strategy,
        [GenreData("Спорт", "genre=18")]
        Sport,
        [GenreData("Симулятор", "genre=28")]
        Simulation,
        [GenreData("RPG", "genre=3")]
        RPG,
        [GenreData("Инди", "genre=23")]
        Indie,
        [GenreData("Приключение", "genre=25")]
        Adventure,
        [GenreData("Бесплатные", "genre=37")]
        Free
    }

    public class EnumData : Attribute
    {
        public string Description { get; }
        public EnumData(string description)
        {
            Description = description;
        }
    }

    public class GenreData : EnumData
    {
        public string ApiString { get; }

        public GenreData(string description, string apiString = null) :
            base(description)
        {
            ApiString = apiString;
        }
    }

    static class EnumExtension
    {
        /// <summary>
        /// Given an enum value, if a <see cref="EnumData"/> attribute has been defined on it, then return that.
        /// Otherwise return the enum name.
        /// </summary>
        /// <typeparam name="T">Enum type to look in</typeparam>
        /// <typeparam name="U">Output data type</typeparam>
        /// <param name="value">Enum value</param>
        /// <returns>Description or name</returns>
        public static U GetEnumCustomAttribute<T, U>(this T value) where T : struct where U : EnumData
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("Type is not an Enum");
            }
            var fieldName = Enum.GetName(typeof(T), value);
            if (fieldName == null)
            {
                return null;
            }
            var fieldInfo = typeof(T).GetField(fieldName, BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static);
            if (fieldInfo == null)
            {
                return null;
            }
            var descriptionAttribute = (U)fieldInfo.GetCustomAttributes(typeof(U), false).FirstOrDefault();
            if (descriptionAttribute == null)
            {
                return null;
            }
            return descriptionAttribute;
        }

        public static bool TryParseEnumByDescription<T, U>(string description, out T value) where U : EnumData
        {
            MemberInfo[] fields = typeof(T).GetFields();

            foreach (var field in fields)
            {
                U[] attributes = (U[])field.GetCustomAttributes(typeof(U), false);

                if (attributes != null && attributes.Length > 0 && attributes[0].Description.ToLower() == description.ToLower())
                {
                    value = (T)Enum.Parse(typeof(T), field.Name);
                    return true;
                }
            }
            value = default;
            return false;
        }
    }
}
