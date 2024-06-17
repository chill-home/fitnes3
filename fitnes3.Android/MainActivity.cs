using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Hardware;
using Xamarin.Forms;
using Xamarin.Essentials;
using Android.Support.V4.Content;
using Android.Support.V4.App;
using Android;
using fitnes3.Services;



namespace fitnes3.Droid
{   
    
    [Activity(Label = "fitnes", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    

    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        const int RequestActivityRecognitionId = 1000;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            RequestActivityRecognitionPermission();

            LoadApplication(new App());
        }

        void RequestActivityRecognitionPermission()
        {
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ActivityRecognition) != (int)Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.ActivityRecognition }, RequestActivityRecognitionId);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == RequestActivityRecognitionId)
            {
                if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                {
                    // Permission granted
                }
            }

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
    
}