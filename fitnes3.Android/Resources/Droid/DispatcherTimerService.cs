using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using fitnes3.Droid.Resources.Droid;
using fitnes3.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
[assembly: Dependency(typeof(DispatcherTimerService))]
namespace fitnes3.Droid.Resources.Droid
{
    public class DispatcherTimerService : IDispatcherTimerService
    {
        public void StartTimer(TimeSpan interval, Func<bool> callback)
        {
            Device.StartTimer(interval, callback);
        }
    }
}