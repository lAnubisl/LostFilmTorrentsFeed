using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace LostFilmMonitoring.BLL.Interfaces.Models
{
    public class RssItemViewModel
    {
        public RssItemViewModel(HttpResponseMessage httpResponseMessage)
        {
            Body = httpResponseMessage.Content.ReadAsStreamAsync();
            httpResponseMessage.Content.Headers.TryGetValues("Content-Disposition", out IEnumerable<string> cd);
            FileName = cd?.FirstOrDefault()?.Substring("attachment;filename=\"".Length + 1);
            FileName = FileName.Substring(0, FileName.Length - 1);
            ContentType = "application/x-bittorrent";
        }

        public Task<Stream> Body { get; }
        public string FileName { get; }
        public string ContentType { get; }
    }
}