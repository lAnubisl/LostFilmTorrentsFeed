namespace LostFilmMonitoring.BLL.Models
{
    public class SerialViewModel
    {
        public string Name { get; set; }
        public string Title
        {
            get
            {
                var index = Name.IndexOf('(');
                if (index > 0)
                {
                    return Name.Substring(0, index);
                }

                return Name;
            }
        }
        public string Escaped { get; set; }
    }
}