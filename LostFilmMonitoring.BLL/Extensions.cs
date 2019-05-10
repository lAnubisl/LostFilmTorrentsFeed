using LostFilmMonitoring.BLL.Implementations;
using LostFilmMonitoring.DAO.DomainModels;
using System;

namespace LostFilmMonitoring.BLL
{
    internal static class Extensions
    {
        internal static Serial ParseSerial(this FeedItem feedItem)
        {
            var serial = new Serial()
            {
                Name = GetSerialName(feedItem.Title),
                LastEpisodeName = GetEpisodeName(feedItem.Title),
                LastEpisode = feedItem.PublishDateParsed
            };

            var quality = ParseQuality(feedItem);
            switch (quality)
            {
                case Quality.h1080:
                    serial.LastEpisodeTorrentLink1080 = feedItem.Link;
                    break;
                case Quality.h720:
                    serial.LastEpisodeTorrentLinkMP4 = feedItem.Link;
                    break;
                case Quality.SD:
                    serial.LastEpisodeTorrentLinkSD = feedItem.Link;
                    break;
            }

            return serial;
        }

        internal static string ParseQuality(this FeedItem feedItem)
        {
            var startIndex = feedItem.Title.LastIndexOf('[');
            var endIndex = feedItem.Title.LastIndexOf(']');
            var quality = feedItem.Title.Substring(startIndex + 1, endIndex - startIndex - 1);
            if (quality == Quality.h1080 + "p")
            {
                quality = Quality.h1080;
            }

            return quality;
        }

        internal static string GetTorrentId(this Serial serial, string quality)
        {
            switch (quality)
            {
                case Quality.SD: return ParseId(serial.LastEpisodeTorrentLinkSD);
                case Quality.h1080: return ParseId(serial.LastEpisodeTorrentLink1080);
                case Quality.h720: return ParseId(serial.LastEpisodeTorrentLinkMP4);
                default: throw new InvalidOperationException("Quality not supported");
            }
        }

        private static string GetSerialName(string feedItemTitle)
        {
            return feedItemTitle.Substring(0, feedItemTitle.IndexOf(").") + 2);
        }

        private static string GetEpisodeName(string feedItemTitle)
        {
            return feedItemTitle.Substring(0, feedItemTitle.IndexOf("["));
        }

        internal static string GenerateTorrentLink(Guid userId, string torrentId)
        {
            return $"https://lostfilmfeed.petproject.by/Rss/{userId}/{torrentId}";
        }

        public static string ParseId(this FeedItem feedItem)
        {
            return ParseId(feedItem.Link);
        }

        internal static string ParseId(string link)
        {
            //http://tracktor.in/rssdownloader.php?id=33572
            if(link == null || link.IndexOf("rssdownloader.php") < 0)
            {
                return null;
            }

            return link.Substring(link.IndexOf("=") + 1);
        }

        internal static bool HasUpdatesComparedTo(this Serial newOne, Serial oldOne)
        {
            return (newOne.LastEpisodeTorrentLink1080 != null && oldOne.LastEpisodeTorrentLink1080 == null) ||
                   (newOne.LastEpisodeTorrentLinkMP4 != null && oldOne.LastEpisodeTorrentLinkMP4 == null) ||
                   (newOne.LastEpisodeTorrentLinkSD != null && oldOne.LastEpisodeTorrentLinkSD == null);
        }

        internal static void Merge(this Serial to, Serial from)
        {
            if (from.LastEpisodeName != to.LastEpisodeName || from.LastEpisodeTorrentLink1080 != null) to.LastEpisodeTorrentLink1080 = from.LastEpisodeTorrentLink1080;
            if (from.LastEpisodeName != to.LastEpisodeName || from.LastEpisodeTorrentLinkMP4 != null) to.LastEpisodeTorrentLinkMP4 = from.LastEpisodeTorrentLinkMP4;
            if (from.LastEpisodeName != to.LastEpisodeName || from.LastEpisodeTorrentLinkSD != null) to.LastEpisodeTorrentLinkSD = from.LastEpisodeTorrentLinkSD;
            to.LastEpisodeName = from.LastEpisodeName;
            to.LastEpisode = from.LastEpisode;
        }
    }
}