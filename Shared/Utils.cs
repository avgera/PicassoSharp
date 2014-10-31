using System.IO;
using System.Text;
using Android.Content;

namespace PicassoSharp
{
	internal sealed class Utils
	{
	    public const string ThreadPrefix = "Picasso-";
        public const string ThreadIdleName = ThreadPrefix + "Idle";
        public const int DefaultConnectTimeout = 15 * 1000;
        public const int DefaultReadTimeout = 20 * 1000;

	    private static readonly StringBuilder MainThreadKeyBuilder = new StringBuilder();

	    public static string CreateKey(Request request)
	    {
	        string key = CreateKey(request, MainThreadKeyBuilder);
	        MainThreadKeyBuilder.Length = 0;
            return key;
		}

	    public static string CreateKey(Request request, StringBuilder builder)
	    {
	        if (request.Uri != null)
	        {
	            string path = request.Uri.ToString();
	            builder.EnsureCapacity(path.Length + 50);
	            builder.Append(path);
	        }
	        else
	        {
                builder.EnsureCapacity(50);
                builder.Append(request.ResourceId);
	        }

	        builder.Append(';');

	        if (request.TargetWidth != 0)
	        {
	            builder.Append("resize:").Append(request.TargetWidth).Append('x').Append(request.TargetHeight);
	            builder.Append(';');
	        }

	        if (request.CenterCrop)
	        {
	            builder.Append("centercrop;");
	        }

	        if (request.CenterInside)
	        {
	            builder.Append("centerinside;");
	        }

            if (request.Transformations != null)
            {
                for (int i = 0; i < request.Transformations.Count; i++)
                {
                    builder.Append(request.Transformations[i].Key);
                    builder.Append(';');
                }
            }

            return builder.ToString();
	    }

	    public static void CloseQuietly(Stream stream)
	    {
	        if (stream == null)
	            return;
	        try
	        {
	            stream.Close();
	        }
	        catch (IOException)
	        {
	        }
	    }

	    public static byte[] ToByteArray(Stream stream)
	    {
	        using (var ms = new MemoryStream())
	        {
	            byte[] buffer = new byte[1024*4];
	            int bytesRead;
	            while (0 != (bytesRead = stream.Read(buffer, 0, buffer.Length)))
	            {
                    ms.Write(buffer, 0, bytesRead);
	            }
	            return ms.ToArray();
	        }
	    }

	    public static IDownloader CreateDefaultDownloader(Context context)
	    {
            // For now this just returns a UrlConnectionDownloader
            return new UrlConnectionDownloader(context);
	    }
	}
}

