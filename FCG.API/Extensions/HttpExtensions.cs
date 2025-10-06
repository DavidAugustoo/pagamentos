using FCG.API.Models;
using System.Text.Json;

namespace FCG.API.Extensions
{
    public static class HttpExtensions
    {
        //Extensão para colocar as informações no Header da aplicação, invocada pela Controller.
        public static void AddPaginationHeader(this HttpResponse response, PaginationHeader header)
        {
            var jsonOptions = new JsonSerializerOptions {  PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            response.Headers.Add("Pagination", JsonSerializer.Serialize(header, jsonOptions));
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");

        }
    }
}