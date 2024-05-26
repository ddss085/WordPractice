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
    }
}