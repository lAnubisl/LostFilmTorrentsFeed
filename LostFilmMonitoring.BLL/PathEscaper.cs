namespace LostFilmMonitoring.BLL
{
    public static class PathEscaper
    {
        public static string EscapePath(this string path)
        {
            return path
                .Replace(":", "_")
                .Replace("*", "_")
                .Replace("\"", "_")
                .Replace("/", "_")
                .Replace("?", "_")
                .Replace(">", "_")
                .Replace("<", "_")
                .Replace("|", "_");
        }
    }
}
