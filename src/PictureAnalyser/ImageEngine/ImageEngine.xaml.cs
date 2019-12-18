using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace PictureAnalyser
{
    public partial class ImageEngine
    {
        #region Private Fields

        private decimal _resolution = 1;
        private BitmapImage _imageSource;

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty ImagePathProperty = DependencyProperty.Register(
            nameof(ImagePath), typeof(string), typeof(ImageEngine),
            new PropertyMetadata(default(string), ImagePathChanged));

        private static void ImagePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageEngine engine && e.NewValue is string path)
                engine.ImagePathChanged(path);
        }

        public static readonly DependencyProperty MinimumResolutionProperty = DependencyProperty.Register(
            nameof(MinimumResolution), typeof(decimal), typeof(ImageEngine), new PropertyMetadata(0.01m));

        public static readonly DependencyProperty MaximumResolutionProperty = DependencyProperty.Register(
            nameof(MaximumResolution), typeof(decimal), typeof(ImageEngine), new PropertyMetadata(50m));

        #endregion

        #region Public Properties

        public string ImagePath
        {
            get => (string) GetValue(ImagePathProperty);
            set => SetValue(ImagePathProperty, value);
        }

        public decimal MinimumResolution
        {
            get => (decimal) GetValue(MinimumResolutionProperty);
            set => SetValue(MinimumResolutionProperty, value);
        }

        public decimal MaximumResolution
        {
            get => (decimal) GetValue(MaximumResolutionProperty);
            set => SetValue(MaximumResolutionProperty, value);
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

            _resolution = 1;
            UpdateScale();
        }

        #region Scalling

        private void ImageEngine_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                e.Handled = true;

                var delta = e.Delta;

                if (delta > 0)
                {
                    if (_resolution >= MaximumResolution) return;

                    _resolution += GetAddedDeltaResolution(_resolution);
                }
                else
                {
                    if (_resolution <= MinimumResolution) return;

                    _resolution -= GetSubtractDeltaResolution(_resolution);
                }

                UpdateScale();
            }
        }

        private static decimal GetAddedDeltaResolution(decimal resolution)
        {
            var decLog = Math.Log10((double) resolution);
            var truncate = Math.Truncate(decLog);

            if (decLog < 0 && decLog != truncate) truncate = truncate - 1;

            return (decimal) Math.Pow(10, truncate);
        }

        private static decimal GetSubtractDeltaResolution(decimal resolution)
        {
            var decLog = Math.Log10((double) resolution);

            var truncate = Math.Truncate(decLog);

            if (decLog < 0 && decLog != truncate) truncate = truncate - 1;

            return decLog == truncate
                ? (decimal) Math.Pow(10, truncate - 1)
                : (decimal) Math.Pow(10, truncate);
        }

        private void UpdateScale()
        {
            var width = _imageSource.PixelWidth * _resolution;
            var height = _imageSource.PixelHeight * _resolution;

            Host.Width = (double) width;
            Host.Height = (double) height;
        }

        #endregion

        #endregion
    }
}