namespace ImageViewer
{
    public class ImageViewerController
    {
        private ImageViewer _imageViewer;

        internal void Init(ImageViewer imageViewer)
        {
            _imageViewer = imageViewer;
        }

        public void IncreaseScale()
        {
            _imageViewer?.ToScale(ScaleType.Increase);
        }

        public void DecreaseScale()
        {
            _imageViewer?.ToScale(ScaleType.Decrease);
        }
    }
}