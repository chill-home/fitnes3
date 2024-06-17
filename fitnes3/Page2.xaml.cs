using fitnes3.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace fitnes3
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page2 : ContentPage
    {
        private DateTime startTime;
        private bool timerRunning = false;
        private TimerData _timerData = new TimerData();
        
        public Page2()
        {
            InitializeComponent();

            Work.CheckedChanged += RadioButton_CheckedChanged;
            Yoga.CheckedChanged += RadioButton_CheckedChanged;
            Run.CheckedChanged += RadioButton_CheckedChanged;

            // Запускаем асинхронный метод без ожидания
            
        }
        private async Task LoadTimerDataAsync()
        {
            var timerDataList = await App.Database.GetTimerDataAsync();
            if (timerDataList.Count > 0)
            {
                var timerData = timerDataList[0];
                App.GlobalTimerData.ExerciseType = timerData.ExerciseType;
                App.GlobalTimerData.StartTime = timerData.StartTime;
                App.GlobalTimerData.TimerRunning = timerData.TimerRunning;

                if (App.GlobalTimerData.TimerRunning)
                {
                    StartTimerUpdatingUI();
                }
            }
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadTimerDataAsync();
            if (App.GlobalTimerData.TimerRunning)
            {
                StartTimerUpdatingUI();
            }
        }
        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            await SaveTimerDataAsync();
        }
        private async void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;

            if (e.Value)
            {
                if (App.GlobalTimerData.TimerRunning)
                {
                    StopTimer();
                }

                if (radioButton != null)
                {
                    StartTimer(radioButton);
                    await SaveTimerDataAsync();
                }
            }
        }

        private void StartTimer(RadioButton radioButton)
        {
            var exerciseType = radioButton.Content as string;
            if (exerciseType != null)
            {
                App.GlobalTimerData.ExerciseType = exerciseType;
                App.GlobalTimerData.StartTime = DateTime.Now;
                App.GlobalTimerData.TimerRunning = true;

                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    App.GlobalTimerData.RemainingTime = TimeSpan.FromMinutes(60) - (DateTime.Now - App.GlobalTimerData.StartTime);
                    if (App.GlobalTimerData.RemainingTime <= TimeSpan.Zero)
                    {
                        StopTimer();
                        return false;
                    }
                    else
                    {
                        timerLabel.Text = $"{App.GlobalTimerData.ExerciseType} {App.GlobalTimerData.RemainingTime:mm\\:ss}";
                        return true;
                    }
                });
            }
            else
            {
                Console.WriteLine("RadioButton content is null or not a string.");
            }
        }


        private void StopTimer()
        {
            App.GlobalTimerData.TimerRunning = false;
            timerLabel.Text = "";
        }
        private async Task SaveTimerDataAsync()
        {
            var timerData = new TimerData
            {
                ExerciseType = App.GlobalTimerData.ExerciseType,
                StartTime = App.GlobalTimerData.StartTime,
                TimerRunning = App.GlobalTimerData.TimerRunning
            };
            await App.Database.SaveTimerDataAsync(timerData);
        }

        private void StartTimerUpdatingUI()
        {
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                App.GlobalTimerData.RemainingTime = TimeSpan.FromMinutes(60) - (DateTime.Now - App.GlobalTimerData.StartTime);
                if (App.GlobalTimerData.RemainingTime <= TimeSpan.Zero)
                {
                    StopTimer();
                    return false;
                }
                else
                {
                    timerLabel.Text = $"{App.GlobalTimerData.ExerciseType} {App.GlobalTimerData.RemainingTime:mm\\:ss}";
                    return true;
                }
            });
        }
        private void StartTimerAfterLoading()
        {
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                App.GlobalTimerData.RemainingTime = TimeSpan.FromMinutes(60) - (DateTime.Now - App.GlobalTimerData.StartTime);
                if (App.GlobalTimerData.RemainingTime <= TimeSpan.Zero)
                {
                    StopTimer();
                    return false;
                }
                else
                {
                    timerLabel.Text = $"{App.GlobalTimerData.ExerciseType} {App.GlobalTimerData.RemainingTime:mm\\:ss}";
                    return true;
                }
            });
        }
    }
}

