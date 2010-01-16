using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;


namespace UICatalog
{
	
	public partial class AmountEditorViewController : UIViewController
	{
		decimal _amount;
		string _title;
		
		public AmountEditorViewController (Decimal amount, string title) : base()
		{
			_amount = amount;
			_title = title;
		}
		
		private UIDecimalField _totalField;
		
		public override void ViewDidLoad ()
		{
			Title = _title;
			
			View.AddSubview(new UILabel {
				Frame = new RectangleF(20,10,280,40),
				TextAlignment = UITextAlignment.Center,
				Font = UIFont.SystemFontOfSize(20),
				TextColor = UIColor.Gray,
				Text = "Type Amount"
			}); 
			
			_totalField = new UIDecimalField (_amount) {
				Frame = new RectangleF(50,50,220,40),
				TextAlignment = UITextAlignment.Center,
				Font = UIFont.BoldSystemFontOfSize(30),
				Text = _amount.ToString("N2"),
				EnablesReturnKeyAutomatically = true,
				BorderStyle = UITextBorderStyle.RoundedRect,
			};
			
			View.AddSubview(_totalField);
			
			_totalField.BecomeFirstResponder();
		}
	}
}
