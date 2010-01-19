using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;


namespace UICatalog
{
	
	public partial class LoadingHUDViewController : UIViewController
	{
		
		LoadingHUDView _hud = new LoadingHUDView("Loading", "Wait for 5 more seconds and " +
			"this   text will automatically disappear");
		
		
		LoadingHUDView _hudSmall = new LoadingHUDView("Loading");
		
		public override void ViewWillAppear (bool animated)
		{
			View.AddSubview(_hud);
			View.AddSubview(_hudSmall);
			
			Title = "LoadingHUDView";
			
			UIButton btnDone = UIButton.FromType (UIButtonType.RoundedRect);
			btnDone.SetTitle("Large Message", UIControlState.Normal);
			btnDone.Frame = new RectangleF(20,300, 280, 40);
			btnDone.TouchUpInside += HandleButtonTouchUpInside;
			View.AddSubview(btnDone);
			
			UIButton btnSmall = UIButton.FromType (UIButtonType.RoundedRect);
			btnSmall.SetTitle("Small Message", UIControlState.Normal);
			btnSmall.Frame = new RectangleF(20,350, 280, 40);
			btnSmall.TouchUpInside += HandleBtnSmallTouchUpInside;;
			View.AddSubview(btnSmall);
			
			View.AddSubview(new UILabel{Text="Click to show the HUD View",
				Frame = new RectangleF(20, 100, 300, 40)});
			
		}

		void HandleBtnSmallTouchUpInside (object sender, EventArgs e)
		{
			_hudSmall.StartAnimating();
			NSTimer.CreateScheduledTimer(TimeSpan.FromSeconds(5), ()=>_hudSmall.StopAnimating());
		}

		void HandleButtonTouchUpInside (object sender, EventArgs e)
		{
			_hud.StartAnimating();
			NSTimer.CreateScheduledTimer(TimeSpan.FromSeconds(5), ()=>_hud.StopAnimating());
		}
	}
}
