using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace UICatalog
{
		[MonoTouch.Foundation.Register("ControlsTableViewController")]
		public partial class ControlsTableViewController : UITableViewController {
			static NSString kCellIdentifier = new NSString ("MyIdentifier");
		
			public List<string> _controls = new List<string>();	
		
			//
			// Constructor invoked from the NIB loader
			//
			public ControlsTableViewController (IntPtr p) : base (p) {}
		
			
			//
			// The data source for our TableView
			//
			class DataSource : UITableViewDataSource {
				ControlsTableViewController tvc;
			
				public DataSource (ControlsTableViewController tvc)
				{
					this.tvc = tvc;
					tvc._controls.Add("UIWebImageView");
					tvc._controls.Add("RotatingViewController");
					tvc._controls.Add("UIDecimalField");
					tvc._controls.Add("LoadingHUDView");
					tvc._controls.Add("CalendarMonthView");
				}
				
				public override int RowsInSection (UITableView tableView, int section)
				{
					return tvc._controls.Count();
				}
		
				public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
				{
					var cell = tableView.DequeueReusableCell (kCellIdentifier);
					if (cell == null){
						cell = new UITableViewCell (UITableViewCellStyle.Default, kCellIdentifier);
					}
				
					cell.TextLabel.Text = tvc._controls[indexPath.Row];
					cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
					return cell;
				}
			}
		
			//
			// This class receives notifications that happen on the UITableView
			//
			class TableDelegate : UITableViewDelegate {
				ControlsTableViewController tvc;
	
				public TableDelegate (ControlsTableViewController tvc)
				{
					this.tvc = tvc;
				}
				
				public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
				{
					var selected = tvc._controls[indexPath.Row];
				
					Console.WriteLine ("UICatalog: Row selected {0}", selected);
				
					switch (selected){
						case "UIWebImageView":
							tvc.NavigationController.PushViewController(new UIWebImageViewController(), true);
							break;
					
						case "RotatingViewController":
							tvc.NavigationController.PushViewController(_createRotatingViewController(), true);
							break;
						
						case "UIDecimalField":
							tvc.NavigationController.PushViewController(
					                 new AmountEditorViewController(1234.23m, "Amount"), true);
							break;
					
						case "LoadingHUDView" :
							tvc.NavigationController.PushViewController(
					                 new LoadingHUDViewController(), true);
							break;
						case "CalendarMonthView" :
							tvc.NavigationController.PushViewController(
					                 new CalendarMonthViewController{Title="Calendar"}, true);
							break;
					}
				}
			
				public UIViewController _createRotatingViewController(){
					var rvc = new RotatingViewController();
					rvc.PortraitViewController = new UIViewController{View = new UIView{ BackgroundColor = UIColor.Blue }};
					rvc.LandscapeLeftViewController = new TestViewController{View = new UIView {BackgroundColor = UIColor.Green}};
					rvc.LandscapeRightViewController = new UIWebImageViewController(); 
					rvc.PortraitViewController.View.AddSubview(new UILabel {
									Text = "PortraitView. Rotate for more.", 
									Frame = new System.Drawing.RectangleF(10,10,300,40), 
									BackgroundColor = UIColor.Blue});
				
					rvc.LandscapeLeftViewController.View.AddSubview(new UILabel {
									Text = "LandscapeLeftView.", 
									Frame = new System.Drawing.RectangleF(10,10,300,40), 
									BackgroundColor = UIColor.Green});
					return rvc;
				}
			}
			
			public override void ViewDidLoad ()
			{
				base.ViewDidLoad ();
				Title = "Controls";
				
				TableView.Delegate = new TableDelegate (this);
				TableView.DataSource = new DataSource (this);
			}
			
	}
	
	public class TestViewController : UIViewController {
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
		
		public override bool WantsFullScreenLayout {
			get {
				return true;
			}
			set {
				base.WantsFullScreenLayout = value;
			}
		}

	}
		
}
