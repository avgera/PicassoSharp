using System;
using System.IO;
using Android.Content;
using Android.Graphics;
using Java.IO;

namespace PicassoSharp
{
    class ContentStreamRequestHandler : RequestHandler
    {
        private readonly Context m_Context;

        internal ContentStreamRequestHandler(Context context)
        {
            m_Context = context;
        }

        public override bool CanHandleRequest(Request data)
        {
            return ContentResolver.SchemeContent.Equals(data.Uri.Scheme);
        }

        public override Result Load(Request data)
        {
            return new Result(DecodeContentStream(data), LoadedFrom.Disk);
        }

        protected Bitmap DecodeContentStream(Request data)
        {
            var uri = Android.Net.Uri.Parse(data.Uri.AbsolutePath);
            ContentResolver contentResolver = m_Context.ContentResolver;
            BitmapFactory.Options bitmapOptions = CreateBitmapOptions(data);
            Stream stream = null;
            if (RequiresInSampleSize(bitmapOptions))
            {
                try
                {
                    stream = contentResolver.OpenInputStream(uri);
                    BitmapFactory.DecodeStream(stream, null, bitmapOptions);
                }
                finally
                {
                    Utils.CloseQuietly(stream);
                }
                CalculateInSampleSize(data.TargetWidth, data.TargetHeight, bitmapOptions, data);
            }
            stream = contentResolver.OpenInputStream(uri);
            try
            {
                return BitmapFactory.DecodeStream(stream, null, bitmapOptions);
            }
            finally
            {
                Utils.CloseQuietly(stream);
            }
        }
    }
}