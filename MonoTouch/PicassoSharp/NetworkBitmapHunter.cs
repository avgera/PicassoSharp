using System.IO;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace PicassoSharp
{
    internal class NetworkBitmapHunter : BitmapHunter
    {
        private readonly IDownloader m_Downloader;

        internal NetworkBitmapHunter(Picasso picasso, Action action, Dispatcher dispatcher, ICache<UIImage> cache,
            IDownloader downloader)
            : base(picasso, action, dispatcher, cache)
        {
            m_Downloader = downloader;
        }
        
        protected override UIImage Decode(Request data)
        {
            LoadedFrom = LoadedFrom.Network;

            Response response = m_Downloader.Load(data.Uri);
            if (response == null)
                return null;

            Stream stream = response.BitmapStream;
            if (stream == null)
                return null;

            try
            {
                return DecodeStream(stream);
            }
            finally
            {
                Utils.CloseQuietly(stream);
            }
        }

        private UIImage DecodeStream(Stream stream)
        {
            return UIImage.LoadFromData(NSData.FromStream(stream));
        }
    }
}
