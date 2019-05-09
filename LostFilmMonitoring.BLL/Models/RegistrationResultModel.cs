namespace LostFilmMonitoring.BLL.Models
{
    public class RegistrationResultModel
    {
        internal RegistrationResultModel() { }

        internal RegistrationResultModel(string cookie, string usess, string uid)
        {
            Cookie = cookie;
            Usess = usess;
            Uid = uid;
        }

        public string Cookie { get; }
        public string Usess { get; }
        public string Uid { get; }
        public string Error { get; set; }
        public bool Success => Cookie != null;
    }
}