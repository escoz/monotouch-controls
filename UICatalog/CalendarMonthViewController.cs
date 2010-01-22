
using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
namespace UICatalog
{
	public class CalendarMonthViewController : UIViewController
    {

        public CalendarMonthView MonthView;

        public override void ViewDidLoad()
        {
            MonthView = new CalendarMonthView();
			MonthView.OnDateSelected += DateSelectedHandler;
            View.AddSubview(MonthView);
        }
		
		private void DateSelectedHandler(DateTime date){
			Console.WriteLine(String.Format("Date selected: {0}", date.ToShortDateString()));
		}

        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            return false;
        }

    }
}
