using System;
using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using System.Windows.Input;
using ImageViewer;
using Microsoft.Win32;
using Prism.Commands;

namespace ImageViewerSample
{
    internal class MainViewModel : BaseViewModel, IMainViewModel
    {
        #region Public Properties

        public Uri ImagePath { get; set; }
        public ILogger Logger { get; }

        public ImageViewerController Controller { get; }

        public ObservableCollection<KeyBinding> KeyBindings { get; } = new ObservableCollection<KeyBinding>();

        #endregion

        #region Commands

        public DelegateCommand OpenImageCommand { get; }

        #endregion

        #region Constructor

        public MainViewModel(ILogger logger)
        {
            Controller = new ImageViewerController();

            Logger = logger;
            OpenImageCommand = new DelegateCommand(OpenImage);

            KeyBindings.Add(new KeyBinding(new DelegateCommand(IncreaseScale), Key.Add, ModifierKeys.None));
            KeyBindings.Add(new KeyBinding(new DelegateCommand(DecreaseScale), Key.Subtract, ModifierKeys.None));
        }

        #endregion

        #region Command Methods

        private void OpenImage()
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Выберите изображение",
                Filter = GenerateImageFilter()
            };

            if (openFileDialog.ShowDialog() != true)
                return;

            var path = openFileDialog.FileName;

            ImagePath = new Uri(path, UriKind.RelativeOrAbsolute);
        }

        private void DecreaseScale()
        {
            Controller.DecreaseScale();
        }

        private void IncreaseScale()
        {
            Controller.IncreaseScale();
        }

        private static string GenerateImageFilter()
        {
            var filter = "All Files (*.*)|*.*|";

            var codecs = ImageCodecInfo.GetImageEncoders();
            var sep = string.Empty;

            foreach (var c in codecs)
            {
                var codecName = c.CodecName.Substring(8).Replace("Codec", "Files").Trim();
                filter = $"{filter}{sep}{codecName} ({c.FilenameExtension})|{c.FilenameExtension}";
                sep = "|";
            }

            filter = $"{filter}{sep}ICO Files (*.ICO)|*.ico";

            return filter;
        }

        #endregion
    }
}