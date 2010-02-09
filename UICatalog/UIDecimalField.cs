using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;

namespace escoz
{
	public partial class UIDecimalField : UITextField
	{
		public decimal Value {
				get { return UIDecimalField.GetAmountFromString(Text); }
				set { Text = value.ToString("N2"); }
		}
		
		public UIDecimalField (Decimal currentValue): base()
		{
			Value = currentValue;
			Initialize();
		}
		
		public UIDecimalField (IntPtr ptr) : base(ptr) {
			Initialize();
		}
		
		protected void Initialize() {
			KeyboardType = UIKeyboardType.NumberPad;
			Delegate = new UIDecimalFieldDelegate();
		}
			
		private class UIDecimalFieldDelegate : UITextFieldDelegate {
			public override bool ShouldChangeCharacters (UITextField textField, NSRange range, string replacementString)
			{
				var newText = textField.Text.Remove(range.Location, range.Length);
				newText = newText.Insert(range.Location, replacementString);
						
				if (newText.Length>0){
					textField.Text = (UIDecimalField.GetAmountFromString(newText)).ToString("N2");
					
					return false;
				}
				
				return false;
			}
		
		}
		
		private static decimal GetAmountFromString(string text){
			if (text.Length==0)
				return 0;
			
				var cleanedUpText = "";
			foreach (char c in text){
				if (Char.IsDigit(c)) cleanedUpText+=c;
			}
			return (decimal.Parse(cleanedUpText))/100;
		}
	}
}
