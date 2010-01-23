
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
