namespace LostFilmMonitoring.BLL.Models
{
    public class RegistrationResultModel
    {
        internal RegistrationResultModel(string cookie)
        {
            Cookie = cookie;
        }

        public string Cookie { get; }
        public string Error { get; set; }
        public bool Success => Cookie != null;
    }
}