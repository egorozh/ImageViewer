using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ImageViewer
{
    [TemplatePart(Name = PART_Host, Type = typeof(Grid))]
    [TemplatePart(Name = PART_ScrollViewer, Type = typeof(ScrollViewer))]
    [TemplatePart(Name = PART_Image, Type = typeof(Image))]
    [TemplatePart(Name = PART_Canvas, Type = typeof(Canvas))]
    [TemplatePart(Name = PART_PreviewViewer, Type = typeof(Border))]
    [TemplatePart(Name = PART_PreviewImage, Type = typeof(Image))]
    [TemplatePart(Name = PART_PreviewCanvas, Type = typeof(Canvas))]
    [TemplatePart(Name = PART_PreviewRectangle, Type = typeof(Rectangle))]
    public class ImageViewer : Control
    {
        #region Private Fields

        private const string PART_Host = "PART_Host";
        private const string PART_ScrollViewer = "PART_ScrollViewer";
        private const string PART_Image = "PART_Image";
        private const string PART_Canvas = "PART_Canvas";
        private const string PART_PreviewViewer = "PART_PreviewViewer";
        private const string PART_PreviewImage = "PART_PreviewImage";
        private const string PART_PreviewCanvas = "PART_PreviewCanvas";
        private const string PART_PreviewRectangle = "PART_PreviewRectangle";

        private Grid _host;
        private ScrollViewer _scrollViewer;
        private Image _image;
        private Canvas _canvas;
        private Border _previewViewer;
        private Image _previewImage;
        private Canvas _previewCanvas;
        private Rectangle _previewRectangle;

        private BitmapImage _imageSource;

        private ImageViewerController _controller;

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

        public static readonly DependencyProperty ControllerProperty = DependencyProperty.Register(
            nameof(Controller), typeof(ImageViewerController), typeof(ImageViewer),
            new PropertyMetadata(default(ImageViewerController), ControllerChanged));

        private static void ControllerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageViewer viewer && e.NewValue is ImageViewerController controller)
                viewer.ControllerChanged(controller);
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

        public ImageViewerController Controller
        {
            get => (ImageViewerController) GetValue(ControllerProperty);
            set => SetValue(ControllerProperty, value);
        }

        #endregion

        #region Constructor

        static ImageViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageViewer),
                new FrameworkPropertyMetadata(typeof(ImageViewer)));
        }

        public ImageViewer()
        {
            PreviewMouseWheel += ImageEngine_MouseWheel;
            PreviewMouseMove += ImageEngine_MouseMove;
            PreviewMouseLeftButtonUp += ImageEngine_MouseLeftButtonUp;
        }

        #endregion

        public override void OnApplyTemplate()
        {
            _host = (Grid) GetTemplateChild(PART_Host);
            _scrollViewer = (ScrollViewer) GetTemplateChild(PART_ScrollViewer);
            _image = (Image) GetTemplateChild(PART_Image);
            _canvas = (Canvas) GetTemplateChild(PART_Canvas);
            _previewViewer = (Border) GetTemplateChild(PART_PreviewViewer);
            _previewImage = (Image) GetTemplateChild(PART_PreviewImage);
            _previewCanvas = (Canvas) GetTemplateChild(PART_PreviewCanvas);
            _previewRectangle = (Rectangle) GetTemplateChild(PART_PreviewRectangle);

            _scrollViewer.PreviewMouseLeftButtonDown += ImageEngine_MouseLeftButtonDown;
        }

        #region Internal Methods

        internal void ToScale(ScaleType type)
        {
            if (type == ScaleType.Increase)
            {
                if (Scale >= MaximumScale) return;

                Scale += GetAddedDeltaResolution(Scale);
            }
            else
            {
                if (Scale <= MinimumScale) return;

                Scale -= GetSubtractDeltaResolution(Scale);
            }
        }

        #endregion

        #region Private Methods

        private void ImagePathChanged(Uri source)
        {
            _imageSource = new BitmapImage(source);

            _image.Source = _imageSource;
            _previewImage.Source = _imageSource;

            ScaleChanged(Scale);
        }

        private void ControllerChanged(ImageViewerController controller)
        {
            _controller = controller;

            _controller?.Init(this);
        }

        private void ImageEngine_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _prevPoint = e.GetPosition(this);

            if (e.OriginalSource == _canvas || e.OriginalSource == _scrollViewer)
            {
                Mouse.Capture(this);
                _isTranslate = true;
            }
        }

        private void ImageEngine_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isTranslate)
            {
                var mousePos = e.GetPosition(this);

                _scrollViewer.ScrollToHorizontalOffset(_scrollViewer.HorizontalOffset - (mousePos.X - _prevPoint.X));
                _scrollViewer.ScrollToVerticalOffset(_scrollViewer.VerticalOffset - (mousePos.Y - _prevPoint.Y));

                _prevPoint = mousePos;

                DrawPreviewRectangle();

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
            var width = (double) (_imageSource.PixelWidth * scale / 100m);
            var height = (double) (_imageSource.PixelHeight * scale / 100m);

            _host.Width = width;
            _host.Height = height;

            _host.UpdateLayout();
            _scrollViewer.UpdateLayout();

            // TODO: Необходимо задавать offset таким образом, чтобы 
            // при изменении масштаба центральная точка изображения остававалась прежней
            _scrollViewer.ScrollToHorizontalOffset(_scrollViewer.ExtentWidth / 2);
            _scrollViewer.ScrollToVerticalOffset(_scrollViewer.ExtentHeight / 2);

            _host.UpdateLayout();
            _scrollViewer.UpdateLayout();

            if (width > _scrollViewer.ViewportWidth || height > _scrollViewer.ViewportHeight)
            {
                _previewViewer.Visibility = Visibility.Visible;

                _previewViewer.UpdateLayout();

                DrawPreviewRectangle();
            }
            else
            {
                _previewViewer.Visibility = Visibility.Collapsed;
            }
        }

        private void DrawPreviewRectangle()
        {
            var left = _previewCanvas.ActualWidth * _scrollViewer.HorizontalOffset / _host.Width;
            var top = _previewCanvas.ActualHeight * _scrollViewer.VerticalOffset / _host.Height;

            var right = _previewCanvas.ActualWidth *
                (_scrollViewer.HorizontalOffset + _scrollViewer.ViewportWidth) / _host.Width;
            var bottom = _previewCanvas.ActualHeight *
                (_scrollViewer.VerticalOffset + _scrollViewer.ViewportHeight) / _host.Height;

            _previewRectangle.Width = right - left;
            _previewRectangle.Height = bottom - top;

            Canvas.SetLeft(_previewRectangle, left);
            Canvas.SetTop(_previewRectangle, top);
        }

        private void ImageEngine_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                ToScale(e.Delta > 0 ? ScaleType.Increase : ScaleType.Decrease);
            }
            else if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                _scrollViewer.ScrollToHorizontalOffset(_scrollViewer.HorizontalOffset - e.Delta);

                DrawPreviewRectangle();
            }
            else
            {
                _scrollViewer.ScrollToVerticalOffset(_scrollViewer.VerticalOffset - e.Delta);

                DrawPreviewRectangle();
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

        #endregion
    }
}