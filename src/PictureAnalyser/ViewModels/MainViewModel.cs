using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ImageViewer;
using Microsoft.Win32;
using Prism.Commands;

namespace PictureAnalyser
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
                Filter = "Pictures |*.jpg|All Files (*.*)|*.*"
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

        #endregion
    }
}