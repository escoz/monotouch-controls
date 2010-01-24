
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
			MonthView.OnDateSelected += (date) => {
				Console.WriteLine(String.Format("Selected {0}", date.ToShortDateString()));
			};
			MonthView.OnFinishedDateSelection += (date) => {
				Console.WriteLine(String.Format("Finished selecting {0}", date.ToShortDateString()));
			};
			MonthView.IsDayMarkedDelegate += (date) => {
				return (date.Day % 2==0) ? true : false;
			};
            View.AddSubview(MonthView);
        }
		
        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            return false;
        }

    }
}
