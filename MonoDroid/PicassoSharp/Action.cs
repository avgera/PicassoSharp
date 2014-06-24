using System;
using Android.Graphics;
using Android.Graphics.Drawables;
using Java.Lang.Ref;
using Object = Java.Lang.Object;
using WeakReference = Java.Lang.Ref.WeakReference;

namespace PicassoSharp
{
	public abstract class Action : Java.Lang.Object
    {
        internal class RequestWeakReference<T> : WeakReference where T : Java.Lang.Object
	    {
	        private readonly Action m_Action;

	        public RequestWeakReference(Action action, T referent, ReferenceQueue referenceQueue) 
                : base(referent, referenceQueue)
	        {
	            m_Action = action;
	        }

            public Action Action
            {
                get { return m_Action; }
            }
	    }

        private readonly RequestWeakReference<Java.Lang.Object> m_Target;
		private readonly Picasso m_Picasso;
		private readonly Request m_Data;
		private readonly bool m_SkipCache;
		private readonly bool m_NoFade;
		private readonly string m_Key;
		private readonly Drawable m_ErrorDrawable;
	    private readonly System.Action m_OnSuccessListener;
	    private readonly System.Action m_OnFailureListener;
	    private readonly System.Action m_OnFinishListener;

	    protected Action(Picasso picasso, Object target, Request data, bool skipCache, bool noFade, string key, Drawable errorDrawable, System.Action onSuccessListener, System.Action onFailureListener, System.Action onFinishListener)
        {
            m_Target = new RequestWeakReference<Java.Lang.Object>(this, target, picasso.ReferenceQueue);
			m_Picasso = picasso;
			m_Data = data;
			m_Key = key;
			m_SkipCache = skipCache;
			m_NoFade = noFade;
			m_ErrorDrawable = errorDrawable;
	        m_OnSuccessListener = onSuccessListener;
	        m_OnFailureListener = onFailureListener;
	        m_OnFinishListener = onFinishListener;
        }

		public Picasso Picasso
		{
			get
			{
				return m_Picasso;
			}
		}

        public Java.Lang.Object Target
        {
            get
			{
				return m_Target.Get();
            }
        }

        public Request Data
        {
            get
			{
				return m_Data;
			}
        }

        public string Key
        {
            get
			{
				return m_Key;
			}
        }

        public bool SkipCache
        {
            get
			{
				return m_SkipCache;
			}
        }

        public bool Cancelled
        {
            get;
            private set;
        }

		public Drawable ErrorDrawable
		{
			get
			{
				return m_ErrorDrawable;
			}
		}

		public bool NoFade
		{
			get
			{
				return m_NoFade;
			}
		}

	    protected System.Action OnFinishListener
	    {
	        get { return m_OnFinishListener; }
	    }

	    public void Complete(Bitmap bitmap, LoadedFrom loadedFrom)
	    {
	        OnComplete(bitmap, loadedFrom);

	        if (m_OnSuccessListener != null)
	        {
	            m_OnSuccessListener();
	        }

	        Finish();
	    }

	    protected abstract void OnComplete(Bitmap bitmap, LoadedFrom loadedFrom);

	    public void Error()
	    {
	        OnError();

	        if (m_OnFailureListener != null)
	        {
	            m_OnFailureListener();
	        }

	        Finish();
	    }

	    private void Finish()
	    {
	        if (m_OnFinishListener != null)
	        {
	            m_OnFinishListener();
	        }
	    }

	    protected abstract void OnError();

        public void Cancel()
        {
            Cancelled = true;
        }
	}
}

