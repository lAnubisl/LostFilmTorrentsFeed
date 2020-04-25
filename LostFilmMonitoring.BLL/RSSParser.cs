using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;

namespace LostFilmMonitoring.BLL
{
    public class RSSParser
    {
        public static XDocument Parse(string rssString)
        {
            //string pattern = "(?<start>>)(?<content>.+?(?<!>))(?<end><)|(?<start>\")(?<content>.+?)(?<end>\")";
            //string result = Regex.Replace(rssString, pattern, m =>
            //            m.Groups["start"].Value +
            //            HttpUtility.HtmlEncode(HttpUtility.HtmlDecode(m.Groups["content"].Value)) +
            //            m.Groups["end"].Value);
            return XDocument.Parse(rssString);
        }
    }
}