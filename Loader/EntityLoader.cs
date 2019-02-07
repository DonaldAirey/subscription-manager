// <copyright file="EntityLoader.cs" company="Gamma Four">
//    Copyright © 2018 - Gamma Four.  All rights reserved.
// </copyright>
// <author>Donald Airey</author>
namespace GammaFour.UnderWriter.Loader
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Loads an entity from a JSON file and sends it to the API.
    /// </summary>
    public static class EntityLoader
    {
        /// <summary>
        /// Vector of functions for handling the different verbs in the script.
        /// </summary>
        private static Dictionary<Verb, Func<HttpClient, string, string, string, Task>> verbActions =
            new Dictionary<Verb, Func<HttpClient, string, string, string, Task>>
        {
            { Verb.Delete, HandleDelete },
            { Verb.Put, HandlePut },
            { Verb.Post, HandlePost }
        };

        /// <summary>
        /// Loads a collection of entities into the REST Api from a file.
        /// </summary>
        /// <param name="baseUri">The base URI of the REST API.</param>
        /// <param name="api">The path to the resource.</param>
        /// <param name="path">The path to the data file.</param>
        /// <param name="keyProperty">The key property name.</param>
        /// <param name="verb">The verb used to modify the resource.</param>
        /// <returns>Task used for waiting.</returns>
        public static async Task LoadAsync(Uri baseUri, string api, string path, string keyProperty, Verb verb)
        {
            // Use this client to connect to the Web API.
            using (var httpClient = new HttpClient())
            {
                // The base address for the REST API.
                httpClient.BaseAddress = baseUri;

                try
                {
                    // Call a function to handle the verb.
                    await EntityLoader.verbActions[verb](httpClient, api, path, keyProperty).ConfigureAwait(true);
                }
                catch (FileNotFoundException fileNotFoundException)
                {
                    Console.WriteLine(fileNotFoundException.Message);
                }
                catch (HttpRequestException httpRequestException)
                {
                    Console.WriteLine(httpRequestException.Message);
                }
            }
        }

        /// <summary>
        /// Handles the Delete verb.
        /// </summary>
        /// <param name="httpClient">The client for the REST API.</param>
        /// <param name="api">The REST API endpoint for this verb.</param>
        /// <param name="path">A path to an optional file containing parameters.</param>
        /// <param name="keyProperty">An optional property to be used for the location of the resource.</param>
        /// <returns>A task.</returns>
        private static async Task HandleDelete(HttpClient httpClient, string api, string path, string keyProperty)
        {
            // Attempt to delete all resources identified by the api parameter.
            HttpResponseMessage response = await httpClient.DeleteAsync(new Uri($"{api}", UriKind.Relative)).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"{(int)response.StatusCode}: {response.ReasonPhrase}");
            }
        }

        /// <summary>
        /// Handles the Post verb.
        /// </summary>
        /// <param name="httpClient">The client for the REST API.</param>
        /// <param name="api">The REST API endpoint for this verb.</param>
        /// <param name="path">A path to an optional file containing parameters.</param>
        /// <param name="keyProperty">An optional property to be used for the location of the resource.</param>
        /// <returns>A task.</returns>
        private static async Task HandlePost(HttpClient httpClient, string api, string path, string keyProperty)
        {
            // Execute each of the parameters against the REST API for the given resource.
            foreach (JToken jToken in ReadJson(path))
            {
                JObject jObject = jToken as JObject;
                if (jObject != null)
                {
                    StringContent stringContent = new StringContent(JsonConvert.SerializeObject(jObject), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await httpClient.PostAsync(new Uri($"{api}", UriKind.Relative), stringContent).ConfigureAwait(false);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException($"{(int)response.StatusCode}: {response.ReasonPhrase}");
                    }
                }

                JArray jArray = jToken as JArray;
                if (jArray != null)
                {
                    StringContent stringContent = new StringContent(JsonConvert.SerializeObject(jArray), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await httpClient.PostAsync(new Uri($"{api}", UriKind.Relative), stringContent).ConfigureAwait(false);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException($"{(int)response.StatusCode}: {response.ReasonPhrase}");
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Put verb.
        /// </summary>
        /// <param name="httpClient">The client for the REST API.</param>
        /// <param name="api">The REST API endpoint for this verb.</param>
        /// <param name="path">A path to an optional file containing parameters.</param>
        /// <param name="keyProperties">An optional property to be used for the location of the resource.</param>
        /// <returns>A task.</returns>
        private static async Task HandlePut(HttpClient httpClient, string api, string path, string keyProperties)
        {
            // Execute each of the parameters against the REST API for the given resource.
            foreach (JToken jToken in ReadJson(path))
            {
                JObject jObject = jToken as JObject;
                if (jObject != null)
                {
                    // The parameter to uniquely identify the resource is extracted from the parameters and supplied as part of the URL.
                    string key = string.Empty;
                    foreach (string keyProperty in keyProperties.Split(','))
                    {
                        if (!string.IsNullOrEmpty(key))
                        {
                            key += "/";
                        }

                        key += jObject.Value<string>(keyProperty.Trim());
                    }

                    StringContent stringContent = new StringContent(JsonConvert.SerializeObject(jObject), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await httpClient.PutAsync(new Uri($"{api}/{key}", UriKind.Relative), stringContent).ConfigureAwait(false);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException($"{(int)response.StatusCode}: {response.ReasonPhrase}");
                    }
                }
            }
        }

        /// <summary>
        /// Read the JSon file from the given path.
        /// </summary>
        /// <param name="path">The path of the JSON file.</param>
        /// <returns>A list of <see cref="JObject"/> records read from the JSON file.</returns>
        private static List<JToken> ReadJson(string path)
        {
            // Read an anonymous list of tokens from the JSON file.  We really don't care about the structure at this point as we're just going to
            // pass these parameters in the body of the REST API call.
            List<JToken> tokens = new List<JToken>();
            using (StreamReader streamReader = new StreamReader(path))
            {
                tokens = JsonConvert.DeserializeObject<List<JToken>>(streamReader.ReadToEnd());
            }

            // This is the list of parameters for the REST API call.
            return tokens;
        }
    }
}