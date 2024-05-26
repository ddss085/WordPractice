using WordPractice_MAUI.Models;
using WordPractice_MAUI.Views;

namespace WordPractice_MAUI
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();

            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            DrawManager.Instance.Initialize(mainDisplayInfo.Width / 3, mainDisplayInfo.Height / 3 - 120);
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += OnSmallDrawingBoxTapped;
            SmallDrawingBox.GestureRecognizers.Add(tapGestureRecognizer);
        }

        private async void OnSmallDrawingBoxTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DrawingPage(SmallDrawingBox));
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            //count++;

            //if (count == 1)
            //    CounterBtn.Text = $"Clicked {count} time";
            //else
            //    CounterBtn.Text = $"Clicked {count} times";

            //SemanticScreenReader.Announce(CounterBtn.Text);
        }

        private void Button_Pressed(object sender, EventArgs e)
        {
            HanziText.IsVisible = true;
            SmallDrawingBox.IsVisible = false;
        }

        private void Button_Released(object sender, EventArgs e)
        {
            HanziText.IsVisible = false;
            SmallDrawingBox.IsVisible = true;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            DrawManager.Instance.ClearLine();
            SmallDrawingBox.Invalidate();
        }
    }

}
