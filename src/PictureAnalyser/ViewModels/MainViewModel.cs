using System;
using Microsoft.Win32;
using Prism.Commands;

namespace PictureAnalyser
{
    internal class MainViewModel : BaseViewModel, IMainViewModel
    {
        #region Public Properties

        public Uri ImagePath { get; set; }

        #endregion

        #region Commands

        public DelegateCommand OpenImageCommand { get; }

        #endregion

        #region Constructor

        public MainViewModel()
        {
            OpenImageCommand = new DelegateCommand(OpenImage);
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

        #endregion
    }
}