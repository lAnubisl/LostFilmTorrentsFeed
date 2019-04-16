using LostFilmMonitoring.DAO.DomainModels;
using System.Linq;

namespace LostFilmMonitoring.BLL.Models
{
    public class SerialViewModel
    {
        public SerialViewModel(Serial s, SelectedFeedItem[] selectedFeedItems)
        {
            Name = s.Name;
            var selectedItem = selectedFeedItems?.FirstOrDefault(i => i.Serial == s.Name);
            Selected = selectedItem != null;
            Quantity = selectedItem?.Quality;
        }

        public bool Selected { get; }
        public string Name { get; }
        public string Quantity { get; }
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
        public string Escaped => Name.EscapePath();
    }
}