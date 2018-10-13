namespace LostFilmMonitoring.BLL.Models
{
    public class CaptchaViewModel
    {
        internal CaptchaViewModel(string sessionKey, byte[] captchaGif)
        {
            SessionKey = sessionKey;
            CaptchaGif = captchaGif;
        }
        public string SessionKey { get; }
        public byte[] CaptchaGif { get; }
    }
}