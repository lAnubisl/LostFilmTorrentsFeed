using System;
using System.IO;
using LostFilmMonitoring.BLL;
using Microsoft.AspNetCore.Mvc;

namespace LostFilmMonitoring.Web.Controllers
{
    public class ImagesController : Controller
    {
        [Route("images/serials/{filename}")]
        public ActionResult Serials(string fileName)
        {
            Stream stream;
            try
            {
                stream = System.IO.File.OpenRead(Path.Combine(Configuration.GetImagesPath(), fileName));
            }
            catch(Exception)
            {
                return new NotFoundResult();
            }
            
            return new FileStreamResult(stream, "image/jpeg");
        }
    }
}
