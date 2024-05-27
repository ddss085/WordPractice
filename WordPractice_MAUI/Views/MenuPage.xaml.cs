using WordPractice_MAUI.Models;

namespace WordPractice_MAUI.Views;

public partial class MenuPage : ContentPage
{
    public MenuPage()
    {
        InitializeComponent();
    }

    private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        XmlManager.Instance.OnUseChanged();
        if (BunchList.ItemsSource != null)
        {
            int i = 0;
            foreach (var item in BunchList.ItemsSource)
            {
                if (item is QuestionBunch bunch)
                {
                    XmlManager.Instance.XmlRoot.QuestionBunches[i++].Use = bunch.Use;
                }
            }
        }

        XmlManager.Instance.SaveXmlFile();
    }
}