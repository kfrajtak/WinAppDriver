using Nancy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace WinAppDriver.Server
{
    public class JsonNetSerializer : ISerializer
    {
        private readonly JsonSerializer _serializer;

        public JsonNetSerializer()
        {
            var settings = new JsonSerializerSettings
            {
                //ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            _serializer = JsonSerializer.Create(settings);
        }

        IEnumerable<string> ISerializer.Extensions => throw new NotImplementedException();

        bool ISerializer.CanSerialize(string contentType)
        {
            return contentType == "application/json";
        }

        void ISerializer.Serialize<TModel>(string contentType, TModel model, Stream outputStream)
        {
            using (var writer = new JsonTextWriter(new StreamWriter(outputStream)))
            {
                _serializer.Serialize(writer, model);
                writer.Flush();
            }
        }
    }
}
