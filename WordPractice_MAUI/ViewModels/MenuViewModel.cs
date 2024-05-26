using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordPractice_MAUI.Models;

namespace WordPractice_MAUI.ViewModels
{
    public class MenuViewModel
    {
        public ObservableCollection<QuestionBunch> QuestionBunches { get; set; } = XmlManager.Instance.QuestionBunches;
    }
}
