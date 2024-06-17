using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Android.Hardware;
using Xamarin.Forms;
using fitnes3.Droid;
using fitnes3.Services;
using Application = Android.App.Application;

[assembly: Dependency(typeof(StepCounterService))]

namespace fitnes3.Droid
{


    public class StepCounterService : Java.Lang.Object, ISensorEventListener, IStepCounterService
    {
        private SensorManager sensorManager;
        private Sensor stepCounterSensor;
        private int stepCount;

        public event EventHandler<StepCountChangedEventArgs> StepCountChanged;

        public void Start()
        {
            sensorManager = (SensorManager)Application.Context.GetSystemService(Context.SensorService);
            stepCounterSensor = sensorManager.GetDefaultSensor(SensorType.StepCounter);
            sensorManager.RegisterListener(this, stepCounterSensor, SensorDelay.Normal);
        }

        public void Stop()
        {
            sensorManager.UnregisterListener(this, stepCounterSensor);
        }

        public void OnSensorChanged(SensorEvent e)
        {
            if (e.Sensor.Type == SensorType.StepCounter)
            {
                stepCount = (int)e.Values[0];
                StepCountChanged?.Invoke(this, new StepCountChangedEventArgs(stepCount));
            }
        }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            // Необязательная реализация
        }
    }
}