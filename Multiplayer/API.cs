using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class API : MonoBehaviour
{
    public static string url = "https://mptinventarization.site/api/";

        /// <summary>
        /// Посыл запроса GET
        /// </summary>
        /// <typeparam name="Model">Класс модели</typeparam>
        /// <param name="urlController">Имя контроллера API</param>
        /// <returns>Объект класса модели</returns>
        public static Model GET<Model>(string urlController)
        {
            return Task.Run(() => GetAsync<Model>(urlController)).Result;
        }

        /// <summary>
        /// Посыл запроса GET (асинхронный метод)
        /// </summary>
        /// <typeparam name="Model">Класс модели</typeparam>
        /// <param name="urlController">Имя контроллера API</param>
        /// <returns>Объект класса модели</returns>
        private static async Task<Model> GetAsync<Model>(string urlController)
        {
            var resultArray = "";
            try
            {
                var client = new HttpClient { BaseAddress = new Uri(url) };
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var responseMessage = await client.GetAsync($"{urlController}", HttpCompletionOption.ResponseContentRead);
                resultArray = await responseMessage.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
            if (typeof(Model) == typeof(PlayerModelList)) return JsonUtility.FromJson<Model>("{\"playerModels\":" + resultArray + "}");
            return JsonUtility.FromJson<Model>(resultArray);
        }

        /// <summary>
        /// Посыл запроса POST с объектом
        /// </summary>
        /// <typeparam name="Model">Класс модели</typeparam>
        /// <param name="urlController">Имя контроллера API</param>
        /// <param name="entry">Посылаемый объект класса модели</param>
        /// <returns>Посланный объект класса модели</returns>
        public static Model POST<Model>(string urlController, Model entry)
        {
            return JsonUtility.FromJson<Model>(PostOrPutOrDelete(urlController, entry, "POST"));
        }

        /// <summary>
        /// Посыл запроса PUT с объектом
        /// </summary>
        /// <typeparam name="Model">Класс модели</typeparam>
        /// <param name="urlController">Имя контроллера API</param>
        /// <param name="entry">Посылаемый объект класса модели для изменения</param>
        /// <param name="id">Идентификатор посылаемого объекта</param>
        /// <returns>JSON ответ сервера</returns>
        public static string PUT<Model>(string urlController, Model entry, int id)
        {
            return PostOrPutOrDelete($"{urlController}/{id}", entry, "PUT");
        }

        /// <summary>
        /// Посыл запроса DELETE с объектом
        /// </summary>
        /// <typeparam name="Model">Класс модели</typeparam>
        /// <param name="urlController">Имя контроллера API</param>
        /// <param name="entry">Посылаемый объект класса модели для удаления</param>
        /// <param name="id">Идентификатор посылаемого объекта</param>
        /// <returns>JSON ответ сервера</returns>
        public static string DELETE<Model>(string urlController, Model entry, int id)
        {
            return PostOrPutOrDelete($"{urlController}/{id}", entry, "DELETE");
        }

        /// <summary>
        /// Обращение к серверу
        /// </summary>
        /// <typeparam name="Model">Класс модели</typeparam>
        /// <param name="urlController">Имя контроллера API</param>
        /// <param name="entry">Посылаемый объект класса модели</param>
        /// <param name="method">Метод запроса</param>
        /// <returns>JSON ответ сервера</returns>
        private static string PostOrPutOrDelete<Model>(string urlController, Model entry, string method)
        {
            var json = "";
            try
            {
                HttpWebRequest webRequest;

                string requestParams = JsonUtility.ToJson(entry);

                Debug.Log(requestParams);

                webRequest = (HttpWebRequest)WebRequest.Create(url + urlController);

                webRequest.Method = method;
                webRequest.ContentType = "application/json";

                byte[] byteArray = Encoding.UTF8.GetBytes(requestParams);
                webRequest.ContentLength = byteArray.Length;
                using (Stream requestStream = webRequest.GetRequestStream())
                {
                    requestStream.Write(byteArray, 0, byteArray.Length);
                }

                using (WebResponse response = webRequest.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        StreamReader rdr = new StreamReader(responseStream, Encoding.UTF8);
                        json = rdr.ReadToEnd();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
            return json;
        }
}
