using System.Collections.Generic;

namespace Jellyfin.Plugin.Kitsu.Providers.KitsuIO.ApiClient.Models
{
    public class ApiResponse<T>
    {
        public T Data { get; set; }
        public List<Included> Included { get; set; }
        public ResponseMeta Meta { get; set; }

        public ApiResponse() { }

        public ApiResponse(T initialData)
        {
            Data = initialData;
        }
    }
}
