namespace LostFilmMonitoring.BLL.Models
{
    public class RegistrationResponseModel
    {
        public int error { get; set; }
        public string name { get; set; }
        public int uid { get; set; }
        public string code { get; set; }
        public bool success { get; set; }
        public string result { get; set; }
    }
}