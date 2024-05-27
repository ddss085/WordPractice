using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordPractice_MAUI.Models
{
    public class DrawManager
    {
        private static DrawManager _instance;
        public static DrawManager Instance
        {
            get => _instance ??= new DrawManager();
        }

        private readonly List<List<PointF>> _pointsList = [];
        public IDrawable BigDrawable { get; set; }
        public IDrawable SmallDrawable { get; set; }

        public void Initialize(double width, double height)
        {
            BigDrawable = new CustomDrawable(_pointsList);
            SmallDrawable = new CustomResizedDrawable(_pointsList, (float)width, (float)height);
        }

        public void NewLine(PointF point)
        {
            _pointsList.Add(new List<PointF> { point });
        }

        public void AddLine(PointF point)
        {
            _pointsList.Last()?.Add(point);
        }

        public void ClearLine()
        {
            _pointsList.Clear();
        }
    }
    public class CustomDrawable : IDrawable
    {
        private readonly List<List<PointF>> _pointsList;

        public CustomDrawable(List<List<PointF>> pointsList)
        {
            _pointsList = pointsList;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.StrokeColor = Colors.White;
            canvas.StrokeSize = 2;

            if (_pointsList.Count > 0)
            {
                foreach (var points in _pointsList)
                {
                    for (int i = 0; i < points.Count - 1; i++)
                    {
                        var point1 = points[i];
                        var point2 = points[i + 1];
                        canvas.DrawLine(point1.X, point1.Y, point2.X, point2.Y);
                    }
                }
            }
        }
    }
    public class CustomResizedDrawable : IDrawable
    {
        private readonly List<List<PointF>> _pointsList;
        private readonly float _originalWidth;
        private readonly float _originalHeight;
        private float _scaleX = 0.0F;
        private float _scaleY = 0.0F;

        public float OriginalWidth => _originalWidth;
        public float OriginalHeight => _originalHeight;
        public float ScaleX => _scaleX;
        public float ScaleY => _scaleY;


        public CustomResizedDrawable(List<List<PointF>> points, float originalWidth, float originalHeight)
        {
            _pointsList = points;
            _originalWidth = originalWidth;
            _originalHeight = originalHeight;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.StrokeColor = Colors.White;
            canvas.StrokeSize = 2;

            if (_pointsList.Count > 0)
            {
                float scaleX = dirtyRect.Width / _originalHeight;
                float scaleY = dirtyRect.Height / _originalWidth;
                _scaleX = scaleX;
                _scaleY = scaleY;

                foreach (var points in _pointsList)
                {
                    for (int i = 0; i < points.Count - 1; i++)
                    {
                        var point1 = new PointF(points[i].Y * scaleX, (_originalWidth - points[i].X) * scaleY);
                        var point2 = new PointF(points[i + 1].Y * scaleX, (_originalWidth - points[i + 1].X) * scaleY);
                        canvas.DrawLine(point1.X, point1.Y, point2.X, point2.Y);
                    }
                }
            }
        }
    }
}
