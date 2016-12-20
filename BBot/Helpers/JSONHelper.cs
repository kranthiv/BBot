using BBot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BBot.Helpers
{
    public static class JSONHelper
    {
        public static async Task<T> DeSerializeJSON<T>(string json)
        {
            T response = await Task.Factory.StartNew<T>(()=>
            {
                return JsonConvert.DeserializeObject<T>(json);
            });

            return response;

        }
    }
}