using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PictureAnalyser
{
    public partial class ImageEngine
    {
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
        }

        #endregion

        #region Private Methods

        private void ImagePathChanged(string imagePath)
        {
            var uri = new Uri(imagePath, UriKind.RelativeOrAbsolute);

            var imageSource = new BitmapImage();

            imageSource.BeginInit();
            imageSource.UriSource = uri;
            imageSource.EndInit();

            Image.Source = imageSource;
        }

        #endregion
    }
}