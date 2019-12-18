using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace PictureAnalyser
{
    public partial class ImageEngine
    {
        private decimal _scale = 1;
        private BitmapImage _imageSource;

        #region Dependency Properties

        public static readonly DependencyProperty ImagePathProperty = DependencyProperty.Register(
            "ImagePath", typeof(string), typeof(ImageEngine), new PropertyMetadata(default(string), ImagePathChanged));

        private static void ImagePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageEngine engine && e.NewValue is string path)
                engine.ImagePathChanged(path);
        }

        #endregion

        #region Public Properties

        public string ImagePath
        {
            get => (string) GetValue(ImagePathProperty);
            set => SetValue(ImagePathProperty, value);
        }

        #endregion

        #region Constructor

        public ImageEngine()
        {
            InitializeComponent();

            PreviewMouseWheel += ImageEngine_MouseWheel;
        }

        #endregion

        #region Private Methods

        private void ImagePathChanged(string imagePath)
        {
            var uri = new Uri(imagePath, UriKind.RelativeOrAbsolute);

            _imageSource = new BitmapImage();

            _imageSource.BeginInit();
            _imageSource.UriSource = uri;
            _imageSource.EndInit();

            Image.Source = _imageSource;

            _scale = 1;
            UpdateScale();
        }

        private void ImageEngine_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                e.Handled = true;

                var delta = e.Delta;

                if (delta > 0)
                {
                    if (_scale >= 200) return;

                    _scale += GetAddedDeltaResolution(_scale);
                }
                else
                {
                    if (_scale <= 0.0001m) return;

                    _scale -= GetSubtractDeltaResolution(_scale);
                }

                UpdateScale();
            }
        }

        private static decimal GetAddedDeltaResolution(decimal resolution)
        {
            var decLog = Math.Log10((double)resolution);
            var truncate = Math.Truncate(decLog);

            if (decLog < 0 && decLog != truncate) truncate = truncate - 1;

            return (decimal)Math.Pow(10, truncate);
        }
        
        private static decimal GetSubtractDeltaResolution(decimal resolution)
        {
            var decLog = Math.Log10((double)resolution);

            var truncate = Math.Truncate(decLog);

            if (decLog < 0 && decLog != truncate) truncate = truncate - 1;

            return decLog == truncate
                ? (decimal)Math.Pow(10, truncate - 1)
                : (decimal)Math.Pow(10, truncate);
        }

        private void UpdateScale()
        {
            var width = _imageSource.PixelWidth * _scale;
            var height = _imageSource.PixelHeight * _scale;

            Host.Width =  (double) width;
            Host.Height = (double) height;
        }

        #endregion
    }
}