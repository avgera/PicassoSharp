﻿using System;
using MonoTouch.UIKit;

namespace PicassoSharp
{
    public class UIImageViewAction : Action<UIImage, UIImage>
	{
		public UIImageViewAction(
            Picasso picasso, 
            UIImageView target, 
            Request<UIImage> data,
            bool skipCache,
            string key,
            UIImage errorImage,
            System.Action onSuccessListener,
            System.Action onFailureListener,
            System.Action onFinishListener)
            : base(picasso, target, data, skipCache, FadeMode.Never, key, errorImage, onSuccessListener, onFailureListener, onFinishListener)
		{
		}

		#region implemented abstract members of Action

		protected override void OnComplete(UIImage bitmap, LoadedFrom loadedFrom)
		{
			if (bitmap == null) {
				throw new Exception(String.Format("Attempted to complete action with no result!\n{0}", this));
			}

			var target = this.Target as UIImageView;
			if (target == null)
				return;

		    target.Image = bitmap;
		}

	    protected override void OnError()
	    {
            var target = this.Target as UIImageView;
            if (target == null)
                return;

	        if (ErrorImage != null)
	        {
	            target.Image = ErrorImage;
	        }
	    }

	    #endregion
	}
}

