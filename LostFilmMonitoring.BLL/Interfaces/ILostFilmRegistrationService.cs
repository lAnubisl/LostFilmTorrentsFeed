using LostFilmMonitoring.BLL.Models;
using System.Threading.Tasks;

namespace LostFilmMonitoring.BLL.Interfaces
{
    public interface ILostFilmRegistrationService
    {
        Task<CaptchaViewModel> GetNewCaptcha();

        Task<RegistrationResultModel> Register(string captchaCookie, string captcha);
    }
}