using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VkNet.Model.Attachments;

namespace AdviceBotVK
{
    static class Helpers
    {
        public static async Task<IEnumerable<Photo>> GetVkPhotoAttachmentFromImageUrl(UserSession userSession, string url, string extension = "jpg")
        {
            // Загрузить картинку на сервер VK.
            var response = await UploadFile(userSession.UploadServer.UploadUrl,
                url, extension);

            // Сохранить загруженный файл
            return userSession.VKBot.VK.Photo.SaveMessagesPhoto(response);
        }

        public static async Task<string> UploadFile(string serverUrl, string file, string fileExtension)
        {
            // Получение массива байтов из файла
            var data = GetBytes(file);

            // Создание запроса на загрузку файла на сервер
            using (var client = new HttpClient())
            {
                var requestContent = new MultipartFormDataContent();
                var content = new ByteArrayContent(data);
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                requestContent.Add(content, "file", $"file.{fileExtension}");

                var response = client.PostAsync(serverUrl, requestContent).Result;
                return Encoding.Default.GetString(await response.Content.ReadAsByteArrayAsync());
            }
        }

        public static byte[] GetBytes(string fileUrl)
        {
            using (var webClient = new WebClient())
            {
                return webClient.DownloadData(fileUrl);
            }
        }
    }

    static class ListExtension
    {
        /// <summary>
        /// Получить случайный элемент из списка
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">Список</param>
        /// <returns>Элемент из списка</returns>
        public static T GetRandomItem<T>(this List<T> list)
        {
            var rnd = new Random();
            var listLength = list.Count;
            return list[rnd.Next(listLength)];
        }
    }

    static class ArrayExtension
    {
        /// <summary>
        /// Получить случайный элемент из массива
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">Список</param>
        /// <returns>Элемент из списка</returns>
        public static T GetRandomItem<T>(this T[] array)
        {
            var rnd = new Random();
            var listLength = array.Length;
            return array[rnd.Next(listLength)];
        }
    }
}
