using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Platform;
using WordPractice_MAUI.Models;

namespace WordPractice_MAUI.Views
{
    public partial class DrawingPage : ContentPage
    {
        private readonly GraphicsView _smallDrawingBox;
        private Image _drawingImage;

        public DrawingPage(GraphicsView smallDrawingBox)
        {
            InitializeComponent();
            _smallDrawingBox = smallDrawingBox;

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += OnClearButtonTapped;
            ClearButton.GestureRecognizers.Add(tapGestureRecognizer);

            //_drawable = new CustomDrawable(_pointsList);
            //DrawingCanvas.Drawable = _drawable;
        }

        private void OnLargeDrawingBoxTapped(object sender, EventArgs e)
        {
            // Implement drawing logic here
            // For the sake of simplicity, we'll just simulate a drawing
            //var graphicsView = new GraphicsView
            //{
            //    Drawable = new CustomDrawable()
            //};
            //LargeDrawingBox.Content = graphicsView;
        }

        private async void OnConfirmButtonClicked(object sender, EventArgs e)
        {
            // Capture the drawing from LargeDrawingBox
            //var image = await CaptureViewAsImage(LargeDrawingBox.Content);
            //_smallDrawingBox.Content = new Image { Source = image };

            await Navigation.PopAsync();
        }

        private void OnStartInteraction(object sender, TouchEventArgs e)
        {
            if (e.Touches.Length > 0)
            {
                DrawManager.Instance.NewLine(e.Touches[0]);
                DrawingCanvas.Invalidate();
            }
        }

        private void OnDragInteraction(object sender, TouchEventArgs e)
        {
            if (e.Touches.Length > 0)
            {
                DrawManager.Instance.AddLine(e.Touches[0]);
                DrawingCanvas.Invalidate();
            }
        }

        private void OnEndInteraction(object sender, TouchEventArgs e)
        {
            // Optional: Implement any logic needed when touch ends
        }

        private void OnClearButtonTapped(object sender, EventArgs e)
        {
            DrawManager.Instance.ClearLine();
            DrawingCanvas.Invalidate();
        }

        private void ContentPage_Loaded(object sender, EventArgs e)
        {
            DrawingCanvas.Drawable = DrawManager.Instance.BigDrawable;
            //var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
            //activity.RequestedOrientation = ScreenOrientation.Landscape;
        }

        private void ContentPage_Unloaded(object sender, EventArgs e)
        {
            _smallDrawingBox.Drawable = DrawManager.Instance.SmallDrawable;
        }


        //private async Task<ImageSource> CaptureViewAsImage(View view)
        //{
        //    var nativeView = view.ToPlatform(this.Handler.MauiContext);
        //    //var bitmap = nativeView.CaptureViewAsBitmap();

        //    using var stream = new MemoryStream();
        //    bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);
        //    stream.Seek(0, SeekOrigin.Begin);
        //    return ImageSource.FromStream(() => stream);
        //}
    }
}