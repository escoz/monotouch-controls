using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace UICatalog
{

    public delegate void DateSelected(DateTime date);
    public delegate void MonthChanged();


    public class CalendarMonthViewController : UIViewController
    {

        public CalendarMonthView MonthView;

        public override void ViewDidLoad()
        {
            MonthView = new CalendarMonthView();
            View.AddSubview(MonthView);
        }

        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            return false;
        }

    }

    public class CalendarMonthView : UIView
    {
        public DateTime CurrentMonthYear = DateTime.Now;
        protected DateTime CurrentDate { get; set; }

        private readonly UIScrollView _scrollView;
        private readonly UIImageView _shadow;
        private readonly ArrayList _deck;
        private readonly MonthGridView _monthGridView;
        private UIButton _leftButton, _rightButton;

        public CalendarMonthView() : base(new RectangleF(0, 0, 320, 400))
        {
            CurrentDate = DateTime.Now.Date;

            _scrollView = new UIScrollView(new RectangleF(0, 44, 320, 460 - 44))
                  {
                      ContentSize = new SizeF(320, 260),
                      ScrollEnabled = false,
                      BackgroundColor = UIColor.FromRGBA(222/255f, 222/255f, 225/255f, 1f)
                  };

            _shadow = new UIImageView(UIImage.FromFile("images/calendar/shadow.png"));
            
            LoadButtons();

            _monthGridView = LoadInitialGrids();

            BackgroundColor = UIColor.Clear;
            AddSubview(_scrollView);
            AddSubview(_shadow);
            AddSubview(_monthGridView);
        }

        private void LoadButtons()
        {
            _leftButton = UIButton.FromType(UIButtonType.Custom);
            _leftButton.TouchUpInside += HandleLeftButtonTouchUpInside;
            _leftButton.SetImage(UIImage.FromFile("images/calendar/leftarrow.png"), UIControlState.Normal);
            AddSubview(_leftButton);
            _leftButton.Frame = new RectangleF(10, 0, 44, 42);

            _rightButton = UIButton.FromType(UIButtonType.Custom);
            _rightButton.TouchUpInside += HandleRightButtonTouchUpInside;
            _rightButton.SetImage(UIImage.FromFile("images/calendar/rightarrow.png"), UIControlState.Normal);
            AddSubview(_rightButton);
            _rightButton.Frame = new RectangleF(320 - 56, 0, 44, 42);
        }

        void HandleRightButtonTouchUpInside(object sender, EventArgs e)
        {
            MoveCalendarMonths(true, true);
        }
        void HandleLeftButtonTouchUpInside(object sender, EventArgs e)
        {
            MoveCalendarMonths(false, true);
        }

        private void MoveCalendarMonths(bool upwards, bool animated)
        {
            UserInteractionEnabled = false;
            SetNeedsDisplay();
            UserInteractionEnabled = true;
            
        }

        private MonthGridView LoadInitialGrids()
        {
            var grid = new MonthGridView(CurrentMonthYear, CurrentDate);
            var rect = _scrollView.Frame;
            rect.Size = new SizeF { Height = (grid.Lines + 1) * 44, Width = rect.Size.Width };
            _scrollView.Frame = rect;

            Frame = new RectangleF(Frame.X, Frame.Y, _scrollView.Frame.Size.Width, _scrollView.Frame.Size.Height);

            var imgRect = _shadow.Frame;
            imgRect.Y = rect.Size.Height - 132;
            _shadow.Frame = imgRect;

            //var next = new UIView(new RectangleF(0, _monthGridView.Lines * 44, 320, 20));
            //var prev = new UIView(new RectangleF(0, -20, 320, 20));
            
            //_deck.Add(prev); _deck.Add(_monthGridView); _deck.Add(next);
			grid.BuildGrid();
			grid.Frame = new RectangleF(0, 44, 320, 400);
            return grid;

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
			UIColor.DarkGray.SetColor(); // TODO  proper color needs to be set
            //UIColor.FromRGBA(75/255, 92/255, 111/255, 1).SetColor();
			var a  = CurrentMonthYear.ToString("MMMM yyyy");
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
        private readonly DateTime _currentDay;
        private DateTime _currentMonth;
        protected readonly IList<CalendarDayView> _dayTiles = new List<CalendarDayView>();
        public int Lines { get; set; }

        public IList<DateTime> Marks { get; set; }

        public MonthGridView(DateTime month, DateTime day)
        {
            _currentDay = day;
            _currentMonth = month.Date;
        }

        public void BuildGrid()
        {
            DateTime previousMonth = _currentMonth.AddMonths(-1);
            var daysInPreviousMonth = DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month);
            var daysInMonth = DateTime.DaysInMonth(_currentMonth.Year, _currentMonth.Month);
            var weekdayOfFirst = (int)_currentMonth.DayOfWeek;
            var lead = daysInPreviousMonth - (weekdayOfFirst - 2);

            // build last month's days
            for (int i = 1; i < weekdayOfFirst; i++)
            {
                var dayView = new CalendarDayView
                {
                    Frame = new RectangleF((i - 1) * 46 - 1, 0, 47, 45),
                    Text = lead.ToString(),
                    Active = false
                };
                AddSubview(dayView);
                _dayTiles.Add(dayView);
                lead++;
            }

            var position = weekdayOfFirst;
            var line = 0;

            // current month
            for (int i = 1; i < daysInMonth; i++)
            {
                var dayView = new CalendarDayView
                  {
                      Frame = new RectangleF((position - 1) * 46 - 1, line * 44, 47, 45),
                      Today = (i == _currentDay.Day),
                      Text = i.ToString(),
                      Active = true,
                      Tag = i,
                      Marked = IsDayMarked(i),
					  Selected = (i == _currentDay.AddDays(1).Day)
                  };
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
                    var dayView = new CalendarDayView
                      {
                          Frame = new RectangleF((i - 1) * 46 -1, line * 44, 47, 45),
                          Text = dayCounter.ToString(),
                          Selected = false,
                          Active = false,
                          Today = false
                      };
                    AddSubview(dayView);
                    _dayTiles.Add(dayView);
                    dayCounter++;
                }
            }

            Frame = new RectangleF(Frame.Location, new SizeF(Frame.Width, (line + 1) * 44));

            Lines = (position == 1 ? line - 1 : line);
        }

        private bool IsDayMarked(int day)
        {
            return true;//return Marks.Contains(new DateTime(_currentMonth.Year, _currentMonth.Month, day));
        }

    }

    public class CalendarDayView : UIView
    {
        public string Text { get; set; }
        public bool Active { get; set; }
        public bool Today { get; set; }
        public bool Selected { get; set; }
        public bool Marked { get; set; }

        public override void Draw(RectangleF rect)
        {
            UIImage img;
            UIColor color;

            if (!Active)
            {
                color = UIColor.Gray;
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
            }else
            {
				color = UIColor.DarkGray;
                //color = UIColor.FromRGBA(75/255, 92/255, 111/255, 1);
                img = UIImage.FromFile("images/calendar/datecell.png");
            }
            img.Draw(new PointF(0,0));
            color.SetColor();
            DrawString(Text, RectangleF.Inflate(Bounds, 4, -6),
                UIFont.BoldSystemFontOfSize(22), 
                UILineBreakMode.WordWrap, UITextAlignment.Center);


            if (Marked)
            {
                var context = UIGraphics.GetCurrentContext();
                if (Selected || Today)
                    context.SetRGBFillColor(1, 1, 1, 1);
                else
                    context.SetRGBFillColor(75/255f, 92/255f, 111/255f, 1);
                context.SetLineWidth(0);
                context.AddEllipseInRect(new RectangleF(Frame.Size.Width/2 - 2, 45-10, 4, 4));
                context.FillPath();

            }
        }
    }
}