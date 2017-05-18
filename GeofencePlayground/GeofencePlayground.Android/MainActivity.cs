using System.Linq;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Util;
using GeofencePlayground.Droid.Helpers;
using Splat;
using SQLite;

namespace GeofencePlayground.Droid
{
	[Activity (Label = "GeofencePlayground.Android", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : ListActivity
	{
	    private SQLiteConnection _db;

        protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

		    _db = Locator.CurrentMutable.GetService<IDatabaseService>().DefaultConnection;

		    SetAdapter();

            var logger = (SqliteLogger)Locator.CurrentMutable.GetService<ILogger>();
		    logger.OnLogEntry += entry => SetAdapter();
		}

	    private void SetAdapter()
	    {
	        var logEntries = _db.Table<LogEntry>()
	            .OrderByDescending(x => x.Time)
	            .ToList();

	        var messages = logEntries.Select(x => $"{x.Time}\n{x.Message}");

	        RunOnUiThread(() =>
	        {
	            ListAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, messages.ToList());
            });
        }
	}
}


