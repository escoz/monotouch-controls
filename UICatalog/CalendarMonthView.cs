//
//  CalendarMonthView.cs
//
//  Converted to MonoTouch on 1/22/09 - Eduardo Scoz || http://escoz.com
//  Originally reated by Devin Ross on 7/28/09  - tapku.com || http://github.com/devinross/tapkulibrary
//
/*
 
 Permission is hereby granted, free of charge, to any person
 obtaining a copy of this software and associated documentation
 files (the "Software"), to deal in the Software without
 restriction, including without limitation the rights to use,
 copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the
 Software is furnished to do so, subject to the following
 conditions:
 
 The above copyright notice and this permission notice shall be
 included in all copies or substantial portions of the Software.
 
 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 OTHER DEALINGS IN THE SOFTWARE.
 
 */

using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace escoz
{

    public delegate void DateSelected(DateTime date);
    public delegate void MonthChanged(DateTime monthSelected);

    public class CalendarMonthView : UIView
    {
		public Action<DateTime> OnDateSelected;
		public Action<DateTime> OnFinishedDateSelection;
		public Func<DateTime, bool> IsDayMarkedDelegate;
		public Func<DateTime, bool> IsDateAvailable;
		
        public DateTime CurrentMonthYear;
        protected DateTime CurrentDate { get; set; }

        private UIScrollView _scrollView;
        private UIImageView _shadow;
        private bool calendarIsLoaded;
		
		private MonthGridView _monthGridView;
        private UIButton _leftButton, _rightButton;

        public CalendarMonthView() : base(new RectangleF(0, 0, 320, 400))
        {
            CurrentDate = DateTime.Now.Date;
			CurrentMonthYear = new DateTime(CurrentDate.Year, CurrentDate.Month, 1);
		}
		
		public override void SetNeedsDisplay ()
		{
			base.SetNeedsDisplay();
			if (_monthGridView!=null)
				_monthGridView.Update();
		}
		
		public override void LayoutSubviews ()
		{
			if (calendarIsLoaded) return;
			
			_scrollView = new UIScrollView(new RectangleF(0, 44, 320, 460 - 44))
                  {
                      ContentSize = new SizeF(320, 260),
                      ScrollEnabled = false,
					  Frame = new RectangleF(0, 44, 320, 460-44),
                      BackgroundColor = UIColor.FromRGBA(222/255f, 222/255f, 225/255f, 1f)
                  };

            _shadow = new UIImageView(UIImage.FromFile("images/calendar/shadow.png"));
            
            LoadButtons();

            LoadInitialGrids();

            BackgroundColor = UIColor.Clear;
            AddSubview(_scrollView);
            AddSubview(_shadow);
			_scrollView.AddSubview(_monthGridView);
			
			calendarIsLoaded = true;
        }
		
		public void DeselectDate(){
			if (_monthGridView!=null)
				_monthGridView.DeselectDayView();
		}

        private void LoadButtons()
        {
            _leftButton = UIButton.FromType(UIButtonType.Custom);
            _leftButton.TouchUpInside += HandlePreviousMonthTouch;
            _leftButton.SetImage(UIImage.FromFile("images/calendar/leftarrow.png"), UIControlState.Normal);
            AddSubview(_leftButton);
            _leftButton.Frame = new RectangleF(10, 0, 44, 42);

            _rightButton = UIButton.FromType(UIButtonType.Custom);
            _rightButton.TouchUpInside += HandleNextMonthTouch;
            _rightButton.SetImage(UIImage.FromFile("images/calendar/rightarrow.png"), UIControlState.Normal);
            AddSubview(_rightButton);
            _rightButton.Frame = new RectangleF(320 - 56, 0, 44, 42);
        }

        private void HandlePreviousMonthTouch(object sender, EventArgs e)
        {
            MoveCalendarMonths(false, true);
        }
        private void HandleNextMonthTouch(object sender, EventArgs e)
        {
            MoveCalendarMonths(true, true);
        }

        public void MoveCalendarMonths(bool upwards, bool animated)
        {
			CurrentMonthYear = CurrentMonthYear.AddMonths(upwards? 1 : -1);
			UserInteractionEnabled = false;
			
			var gridToMove = CreateNewGrid(CurrentMonthYear);
			var pointsToMove = (upwards? 0 + _monthGridView.Lines : 0 - _monthGridView.Lines) * 44;
			
			if (upwards && gridToMove.weekdayOfFirst==0)
				pointsToMove += 44;
			if (!upwards && _monthGridView.weekdayOfFirst==0)
				pointsToMove -= 44;
			
			gridToMove.Frame = new RectangleF(new PointF(0, pointsToMove), gridToMove.Frame.Size);
			
			_scrollView.AddSubview(gridToMove);
			
			if (animated){
				UIView.BeginAnimations("changeMonth");
				UIView.SetAnimationDuration(0.4);
				UIView.SetAnimationDelay(0.1);
				UIView.SetAnimationCurve(UIViewAnimationCurve.EaseInOut);
			}
			
			_monthGridView.Center = new PointF(_monthGridView.Center.X, _monthGridView.Center.Y - pointsToMove);
			gridToMove.Center = new PointF(gridToMove.Center.X, gridToMove.Center.Y - pointsToMove);
			
			_monthGridView.Alpha = 0;
			
			_shadow.Frame = new RectangleF(new PointF(0, gridToMove.Lines*44-88), _shadow.Frame.Size);
			
            _scrollView.Frame = new RectangleF(
			               _scrollView.Frame.Location,
			               new SizeF(_scrollView.Frame.Width, (gridToMove.Lines + 1) * 44));
			
			_scrollView.ContentSize = _scrollView.Frame.Size;
			SetNeedsDisplay();
			
			if (animated)
				UIView.CommitAnimations();
			
			_monthGridView = gridToMove;
			
            UserInteractionEnabled = true;
        }
		
		private MonthGridView CreateNewGrid(DateTime date){
			var grid = new MonthGridView(this, date);
			grid.CurrentDate = CurrentDate;
			grid.BuildGrid();
			grid.Frame = new RectangleF(0, 0, 320, 400);
			return grid;
		}
		
        private void LoadInitialGrids()
        {
            _monthGridView = CreateNewGrid(CurrentMonthYear);
			
            var rect = _scrollView.Frame;
            rect.Size = new SizeF { Height = (_monthGridView.Lines + 1) * 44, Width = rect.Size.Width };
            _scrollView.Frame = rect;

            Frame = new RectangleF(Frame.X, Frame.Y, _scrollView.Frame.Size.Width, _scrollView.Frame.Size.Height+44);

            var imgRect = _shadow.Frame;
            imgRect.Y = rect.Size.Height - 132;
            _shadow.Frame = imgRect;
        }

        public override void Draw(RectangleF rect)
        {
            UIImage.FromFile("images/calendar/topbar.png").Draw(new PointF(0,0));
            DrawDayLabels(rect);
            DrawMonthLabel(rect);
        }

        private void DrawMonthLabel(RectangleF rect)
        {
            var r = new RectangleF(new PointF(0, 5), new SizeF {Width = 320, Height = 42});
			UIColor.DarkGray.SetColor();
            DrawString(CurrentMonthYear.ToString("MMMM yyyy"), 
                r, UIFont.BoldSystemFontOfSize(20),
                UILineBreakMode.WordWrap, UITextAlignment.Center);
        }

        private void DrawDayLabels(RectangleF rect)
        {
            var font = UIFont.BoldSystemFontOfSize(10);
            UIColor.DarkGray.SetColor();
            var context = UIGraphics.GetCurrentContext();
            context.SaveState();
            context.SetShadowWithColor(new SizeF(0, -1), 0.5f, UIColor.White.CGColor);
            var i = 0;
            foreach (var d in Enum.GetNames(typeof(DayOfWeek)))
            {
                DrawString(d.Substring(0, 3), new RectangleF(i*46, 44 - 12, 45, 10), font,
                           UILineBreakMode.WordWrap, UITextAlignment.Center);
                i++;
            }
            context.RestoreState();
        }
    }

    public class MonthGridView : UIView
    {
		private CalendarMonthView _calendarMonthView;
		
        public DateTime CurrentDate {get;set;}
        private DateTime _currentMonth;
        protected readonly IList<CalendarDayView> _dayTiles = new List<CalendarDayView>();
        public int Lines { get; set; }
		protected CalendarDayView SelectedDayView {get;set;}
		public int weekdayOfFirst;
        public IList<DateTime> Marks { get; set; }

        public MonthGridView(CalendarMonthView calendarMonthView, DateTime month)
        {
			_calendarMonthView = calendarMonthView;
            _currentMonth = month.Date;
        }
		
		public void Update(){
			foreach (var v in _dayTiles)
				updateDayView(v);
			
			this.SetNeedsDisplay();
		}
		
		public void updateDayView(CalendarDayView dayView){
			dayView.Marked = _calendarMonthView.IsDayMarkedDelegate == null ? 
							false : _calendarMonthView.IsDayMarkedDelegate(dayView.Date);
			dayView.Available = _calendarMonthView.IsDateAvailable == null ? 
							true : _calendarMonthView.IsDateAvailable(dayView.Date);
		}

        public void BuildGrid()
        {
            DateTime previousMonth = _currentMonth.AddMonths(-1);
            var daysInPreviousMonth = DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month);
            var daysInMonth = DateTime.DaysInMonth(_currentMonth.Year, _currentMonth.Month);
            weekdayOfFirst = (int)_currentMonth.DayOfWeek;
            var lead = daysInPreviousMonth - (weekdayOfFirst - 1);
			
            // build last month's days
            for (int i = 1; i <= weekdayOfFirst; i++)
            {
				var viewDay = new DateTime(_currentMonth.Year, _currentMonth.Month, i);
                var dayView = new CalendarDayView();
				dayView.Frame = new RectangleF((i - 1) * 46 - 1, 0, 47, 45);
            	dayView.Date = viewDay;
				dayView.Text = lead.ToString();
				
                AddSubview(dayView);
                _dayTiles.Add(dayView);
                lead++;
            }

            var position = weekdayOfFirst+1;
            var line = 0;

            // current month
            for (int i = 1; i <= daysInMonth; i++)
            {
				var viewDay = new DateTime(_currentMonth.Year, _currentMonth.Month, i);
                var dayView = new CalendarDayView
                  {
                      Frame = new RectangleF((position - 1) * 46 - 1, line * 44, 47, 45),
                      Today = (CurrentDate.Date==viewDay.Date),
                      Text = i.ToString(),
					
                      Active = true,
                      Tag = i,
					  Selected = (i == CurrentDate.AddDays(1).Day )
                  };
				dayView.Date = viewDay;
				updateDayView(dayView);
				
				if (dayView.Selected)
					SelectedDayView = dayView;
				
                AddSubview(dayView);
                _dayTiles.Add(dayView);

                position++;
                if (position > 7)
                {
                    position = 1;
                    line++;
                }
            }

            //next month
            if (position != 1)
            {
                int dayCounter = 1;
                for (int i = position; i < 8; i++)
                {
					var viewDay = new DateTime(_currentMonth.Year, _currentMonth.Month, i);
                    var dayView = new CalendarDayView
                      {
                          Frame = new RectangleF((i - 1) * 46 -1, line * 44, 47, 45),
                          Text = dayCounter.ToString(),
                      };
					dayView.Date = viewDay;
					updateDayView(dayView);
					
                    AddSubview(dayView);
                    _dayTiles.Add(dayView);
                    dayCounter++;
                }
            }

            Frame = new RectangleF(Frame.Location, new SizeF(Frame.Width, (line + 1) * 44));

            Lines = (position == 1 ? line - 1 : line);
			
			if (SelectedDayView!=null)
				this.BringSubviewToFront(SelectedDayView);
        }
		
		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			base.TouchesBegan (touches, evt);
			if (SelectDayView((UITouch)touches.AnyObject)&& _calendarMonthView.OnDateSelected!=null)
				_calendarMonthView.OnDateSelected(new DateTime(_currentMonth.Year, _currentMonth.Month, SelectedDayView.Tag));
		}
		
		public override void TouchesMoved (NSSet touches, UIEvent evt)
		{
			base.TouchesMoved (touches, evt);
			if (SelectDayView((UITouch)touches.AnyObject)&& _calendarMonthView.OnDateSelected!=null)
				_calendarMonthView.OnDateSelected(new DateTime(_currentMonth.Year, _currentMonth.Month, SelectedDayView.Tag));
		}
		
		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			base.TouchesEnded (touches, evt);
			if (_calendarMonthView.OnFinishedDateSelection==null) return;
			var date = new DateTime(_currentMonth.Year, _currentMonth.Month, SelectedDayView.Tag);
			if (_calendarMonthView.IsDateAvailable == null || _calendarMonthView.IsDateAvailable(date))
				_calendarMonthView.OnFinishedDateSelection(date);
		}

		private bool SelectDayView(UITouch touch){
			var p = touch.LocationInView(this);
			
			int index = ((int)p.Y / 44) * 7 + ((int)p.X / 46);
			if(index<0 || index >= _dayTiles.Count) return false;
			
			var newSelectedDayView = _dayTiles[index];
			if (newSelectedDayView == SelectedDayView) 
				return false;
			
			if (!newSelectedDayView.Active && touch.Phase!=UITouchPhase.Moved){
				var day = int.Parse(newSelectedDayView.Text);
				if (day > 15)
					_calendarMonthView.MoveCalendarMonths(false, true);
				else
					_calendarMonthView.MoveCalendarMonths(true, true);
				return false;
			} else if (!newSelectedDayView.Active && !newSelectedDayView.Available){
				return false;
			}
			
			if (SelectedDayView!=null)
				SelectedDayView.Selected = false;
			
			this.BringSubviewToFront(newSelectedDayView);
			newSelectedDayView.Selected = true;
			
			SelectedDayView = newSelectedDayView;
			SetNeedsDisplay();
			return true;
		}
		
		public void DeselectDayView(){
			if (SelectedDayView==null) return;
			SelectedDayView.Selected= false;
			SelectedDayView = null;
			SetNeedsDisplay();
		}
    }

    public class CalendarDayView : UIView
    {
		string _text;
		public DateTime Date {get;set;}
        bool _active, _today, _selected, _marked, _available;
		public bool Available {get {return _available; } set {_available = value; SetNeedsDisplay(); }}
		public string Text {get { return _text; } set { _text = value; SetNeedsDisplay(); } }
        public bool Active {get { return _active; } set { _active = value; SetNeedsDisplay();  } }
        public bool Today {get { return _today; } set { _today = value; SetNeedsDisplay(); } }
        public bool Selected {get { return _selected; } set { _selected = value; SetNeedsDisplay(); } }
        public bool Marked {get { return _marked; } set { _marked = value; SetNeedsDisplay(); }  }
		
        public override void Draw(RectangleF rect)
        {
            UIImage img;
            UIColor color;

            if (!Active || !Available)
            {
                color = UIColor.FromRGBA(0.576f, 0.608f, 0.647f, 1f);
                img = UIImage.FromFile("images/calendar/datecell.png");
            } else if (Today && Selected)
            {
                color = UIColor.White;
                img = UIImage.FromFile("images/calendar/todayselected.png");
            } else if (Today)
            {
                color = UIColor.White;
                img = UIImage.FromFile("images/calendar/today.png");
            } else if (Selected)
            {
                color = UIColor.White;
                img = UIImage.FromFile("images/calendar/datecellselected.png");
            } else if (Marked)
            {
                color = UIColor.White;
                img = UIImage.FromFile("images/calendar/datecellmarked.png");
            }else
            {
				//color = UIColor.DarkTextColor;
                color = UIColor.FromRGBA(0.275f, 0.341f, 0.412f, 1f);
                img = UIImage.FromFile("images/calendar/datecell.png");
            }
            img.Draw(new PointF(0, 0));
            color.SetColor();
            DrawString(Text, RectangleF.Inflate(Bounds, 4, -8),
                UIFont.BoldSystemFontOfSize(22), 
                UILineBreakMode.WordWrap, UITextAlignment.Center);

//            if (Marked)
//            {
//                var context = UIGraphics.GetCurrentContext();
//                if (Selected || Today)
//                    context.SetRGBFillColor(1, 1, 1, 1);
//                else if (!Active || !Available)
//					UIColor.LightGray.SetColor();
//				else
//                    context.SetRGBFillColor(75/255f, 92/255f, 111/255f, 1);
//                context.SetLineWidth(0);
//                context.AddEllipseInRect(new RectangleF(Frame.Size.Width/2 - 2, 45-10, 4, 4));
//                context.FillPath();
//
//            }
        }
    }
}