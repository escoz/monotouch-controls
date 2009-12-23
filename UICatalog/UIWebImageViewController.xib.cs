
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace UICatalog
{
	public partial class UIWebImageViewController : UIViewController
	{
		#region Constructors
		
		List<String> urls = new List<String>();
		
		int current = 0;

		// The IntPtr and NSCoder constructors are required for controllers that need 
		// to be able to be created from a xib rather than from managed code

		public UIWebImageViewController (IntPtr handle) : base(handle)
		{
			Initialize ();
			
			
		}

		[Export("initWithCoder:")]
		public UIWebImageViewController (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public UIWebImageViewController () : base("UIWebImageViewController", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
			Title = "UIWebImageView";
			urls.Add("http://media-cdn.tripadvisor.com/media/photo-s/01/09/28/ee/lagoa-dalla-barca.jpg");
			urls.Add("http://media-cdn.tripadvisor.com/media/photo-s/01/0d/e4/cc/nossa-senhora-da-lapa.jpg");
			urls.Add("http://media-cdn.tripadvisor.com/media/photo-s/01/0d/e4/c7/view-restaurant-nostradamus.jpg");
		}
		
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			uiWebImageView.DownloadImage(urls[current]);
			
		}
		
		partial void onNextImage (UIButton sender)
		{
			current+=1;
			if (current>=urls.Count())
				current = 0;
			uiWebImageView.DownloadImage(urls[current]);
		}


		
		#endregion
		
		
		
	}
}
