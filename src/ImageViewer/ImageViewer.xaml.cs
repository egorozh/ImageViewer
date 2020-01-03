using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ImageViewer
{
    public partial class ImageViewer
    {
        #region Private Fields

        private BitmapImage _imageSource;

        /// <summary>
        /// Предыдущее значение позиции указателя мыши
        /// </summary>
        private Point _prevPoint;

        private bool _isTranslate;

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty ImagePathProperty = DependencyProperty.Register(
            nameof(ImagePath), typeof(Uri), typeof(ImageViewer),
            new PropertyMetadata(default(Uri), ImagePathChanged));

        private static void ImagePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageViewer viewer && e.NewValue is Uri path)
                viewer.ImagePathChanged(path);
        }

        public static readonly DependencyProperty MinimumScaleProperty = DependencyProperty.Register(
            nameof(MinimumScale), typeof(decimal), typeof(ImageViewer), new PropertyMetadata(1m));

        public static readonly DependencyProperty MaximumScaleProperty = DependencyProperty.Register(
            nameof(MaximumScale), typeof(decimal), typeof(ImageViewer), new PropertyMetadata(5000m));

        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register(
            nameof(Scale), typeof(decimal), typeof(ImageViewer), new PropertyMetadata(100m, ScaleChanged));

        private static void ScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageViewer viewer && e.NewValue is decimal scale)
                viewer.ScaleChanged(scale);
        }

        #endregion

        #region Public Properties

        public decimal Scale
        {
            get => (decimal) GetValue(ScaleProperty);
            private set => SetValue(ScaleProperty, value);
        }

        public Uri ImagePath
        {
            get => (Uri) GetValue(ImagePathProperty);
            set => SetValue(ImagePathProperty, value);
        }

        public decimal MinimumScale
        {
            get => (decimal) GetValue(MinimumScaleProperty);
            set => SetValue(MinimumScaleProperty, value);
        }

        public decimal MaximumScale
        {
            get => (decimal) GetValue(MaximumScaleProperty);
            set => SetValue(MaximumScaleProperty, value);
        }

        #endregion

        #region Constructor

        public ImageViewer()
        {
            InitializeComponent();

            PreviewMouseWheel += ImageEngine_MouseWheel;

            ScrollViewer.PreviewMouseLeftButtonDown += ImageEngine_MouseLeftButtonDown;
            PreviewMouseMove += ImageEngine_MouseMove;
            PreviewMouseLeftButtonUp += ImageEngine_MouseLeftButtonUp;
        }

        #endregion

        #region Private Methods

        private void ImagePathChanged(Uri source)
        {
            _imageSource = new BitmapImage(source);

            Image.Source = _imageSource;

            ScaleChanged(Scale);
        }

        private void ImageEngine_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _prevPoint = e.GetPosition(this);

            if (e.OriginalSource == Canvas || e.OriginalSource == ScrollViewer)
            {
                Mouse.Capture(this);
                _isTranslate = true;
            }
        }

        private void ImageEngine_MouseMove(object sender, MouseEventArgs e)
        {
#if DEBUG
            //var offsets = GetMousePositionOffsets();

            //if (offsets.HasValue)
            //    IoC.Instance.Logger.ShowMessage($"X-Offset: {offsets.Value.Item1}%; Y-Offset: {offsets.Value.Item2}%");
            //else
            //    IoC.Instance.Logger.ShowMessage(string.Empty);

#endif


            if (_isTranslate)
            {
                var mousePos = e.GetPosition(this);

                ScrollViewer.ScrollToHorizontalOffset(ScrollViewer.HorizontalOffset - (mousePos.X - _prevPoint.X));
                ScrollViewer.ScrollToVerticalOffset(ScrollViewer.VerticalOffset - (mousePos.Y - _prevPoint.Y));

                _prevPoint = mousePos;

                Mouse.SetCursor(Cursors.Hand);
            }
        }

        private void ImageEngine_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.SetCursor(Cursors.Arrow);
            Mouse.Capture(null);

            _isTranslate = false;
        }

        #region Scalling

        private void ScaleChanged(decimal scale)
        {
            var width = _imageSource.PixelWidth * scale / 100m;
            var height = _imageSource.PixelHeight * scale / 100m;

            Host.Width = (double)width;
            Host.Height = (double)height;
        }

        private void ImageEngine_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                var delta = e.Delta;

                if (delta > 0)
                {
                    if (Scale >= MaximumScale) return;

                    Scale += GetAddedDeltaResolution(Scale / 100m) * 100m;
                }
                else
                {
                    if (Scale <= MinimumScale) return;

                    Scale -= GetSubtractDeltaResolution(Scale / 100m) * 100m;
                }
                
                ScrollViewer.ScrollToHorizontalOffset(ScrollViewer.ScrollableWidth / 2);
                ScrollViewer.ScrollToVerticalOffset(ScrollViewer.ScrollableHeight / 2);
            }
            else
            {
                ScrollViewer.ScrollToVerticalOffset(ScrollViewer.VerticalOffset - e.Delta);
            }
        }

        private static decimal GetAddedDeltaResolution(decimal resolution)
        {
            var decLog = Math.Log10((double) resolution);
            var truncate = Math.Truncate(decLog);

            if (decLog < 0 && decLog != truncate)
                truncate -= 1;

            return (decimal) Math.Pow(10, truncate);
        }

        private static decimal GetSubtractDeltaResolution(decimal resolution)
        {
            var decLog = Math.Log10((double) resolution);

            var truncate = Math.Truncate(decLog);

            if (decLog < 0 && decLog != truncate)
                truncate -= 1;

            return decLog == truncate
                ? (decimal) Math.Pow(10, truncate - 1)
                : (decimal) Math.Pow(10, truncate);
        }
        
        #endregion

        /// <summary>
        /// Получение положения указателя мыши относительно холста в процентах
        /// </summary>
        /// <returns></returns>
        private (double, double)? GetMousePositionOffsets()
        {
            var position = Mouse.GetPosition(Host);

            if (position.X > 0 && position.Y > 0 && position.X < Host.ActualWidth && position.Y < Host.ActualHeight)
            {
                var xOffset = position.X / Host.ActualWidth * 100.0;
                var yOffset = position.Y / Host.ActualHeight * 100.0;

                return (xOffset, yOffset);
            }

            return null;
        }

        #endregion
    }
}