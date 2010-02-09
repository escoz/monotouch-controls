
using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace escoz
{


	public class ImageListViewController : UIViewController
	{

		public override void ViewDidLoad ()
		{
			View.AddSubview(new ImageListView{
				Frame = View.Frame
			});
		}		
		
        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            return false;
        }
		
	}
	
	public class ImageListView : UIScrollView {
		
	}
}
