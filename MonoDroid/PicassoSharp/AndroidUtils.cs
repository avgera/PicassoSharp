using Android;
using Android.Content;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Java.IO;
using Java.Lang;
using Java.Util.Concurrent;

namespace PicassoSharp
{
    public class AndroidUtils
    {
        private const long MinDiskCacheSize = 5 * 1024 * 1024;
        private const long MaxDiskCacheSize = 50 * 1024 * 1024;
        private const string PicassoCache = "picasso-cache";

        public static int CalculateMemoryCacheSize(Context context)
        {
            var am = (ActivityManager) context.GetSystemService(Context.ActivityService);
            bool largeHeap = (context.ApplicationInfo.Flags & Android.Content.PM.ApplicationInfoFlags.LargeHeap) != 0;
            int memoryClass = am.MemoryClass;
            if (largeHeap && Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb)
            {
                memoryClass = am.LargeMemoryClass;
            }

            return 1024*1024*memoryClass/7;
        }

        internal class PicassoSharpThreadFactory : Object, IThreadFactory
        {
            public Thread NewThread(IRunnable r)
            {
                return new PicassoSharpThread(r);
            }
        }

        internal class PicassoSharpThread : Thread
        {
            public PicassoSharpThread(IRunnable runnable)
                : base(runnable)
            {
            }

            public override void Run()
            {
                Android.OS.Process.SetThreadPriority(ThreadPriority.Background);
                base.Run();
            }
        }

        public static bool HasPermission(Context context, string permission)
        {
            return context.CheckCallingOrSelfPermission(Manifest.Permission.AccessNetworkState) ==
                   Android.Content.PM.Permission.Granted;
        }

        public static bool IsAirplaneModeOn(Context context)
        {
            ContentResolver contentResolver = context.ContentResolver;
            return Settings.System.GetInt(contentResolver, Settings.System.AirplaneModeOn, 0) != 0;
        }

        public static int SizeOfBitmap(Bitmap bitmap)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.HoneycombMr1)
            {
                return bitmap.ByteCount;
            }
            return bitmap.RowBytes*bitmap.Height;
        }

        public static File CreateDefaultCacheDir(Context context)
        {
            var cache = new File(context.ApplicationContext.CacheDir, PicassoCache);
            if (!cache.Exists())
            {
                cache.Mkdirs();
            }
            return cache;
        }

        public static long CalculateDiskCacheSize(File cacheDir)
        {
            long size = MinDiskCacheSize;

            try
            {
                var statFs = new StatFs(cacheDir.AbsolutePath);
                long available = ((long) statFs.BlockCount)*statFs.BlockSize;
                size = available/50;
            }
            catch (IllegalArgumentException) { }

            return Math.Max(Math.Min(size, MaxDiskCacheSize), MinDiskCacheSize);
        }
    }
}

