using Xamarin.Forms;
using fitnes3.Services;
using System.ComponentModel;
using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Microcharts;
using SkiaSharp;
using System.Linq;
using Microcharts.Forms;
using Xamarin.Essentials;

namespace fitnes3
{
    public partial class Page1 : ContentPage, INotifyPropertyChanged
    {
        private IDispatcherTimerService dispatcherTimerService;
        public new event PropertyChangedEventHandler PropertyChanged;
        private IStepCounterService stepCounterService;
        private Random random = new Random();
        public static int colories { get; set; }
        public ObservableCollection<string> TimeZones { get; private set; }
        public ObservableCollection<int> WeeklyCalories { get; private set; }

        private int stepCount;
        private int stepOffset;
        private int calories;
        private int[] dailyCalories = new int[7]; // массив для хранения калорий за каждый день недели

        public int StepCount
        {
            get => stepCount - stepOffset;
            private set
            {
                if (stepCount != value)
                {
                    stepCount = value;
                    OnPropertyChanged(nameof(StepCount));
                    OnPropertyChanged(nameof(Calories));
                }
            }
        }

        private const double CaloriesPerStep = 0.05;
        public int Calories => (int)(StepCount * CaloriesPerStep);

        public Page1()
        {
            InitializeComponent();
            TimeZones = new ObservableCollection<string>();
            WeeklyCalories = new ObservableCollection<int>(new int[7]);

            ListAllTimeZones();

            stepCounterService = DependencyService.Get<IStepCounterService>();
            stepCounterService.StepCountChanged += OnStepCountChanged;
            stepCounterService.Start();

            dispatcherTimerService = DependencyService.Get<IDispatcherTimerService>();
            StartMidnightResetTimer();
            StartHeartbeatSimulation();
            UpdateChart();
        }

        public static void UpdateCalories(int calories)
        {
            colories = calories;
        }

        private async void StartHeartbeatSimulation()
        {
            while (true)
            {
                int heartbeat = random.Next(60, 80);
                HeartbeatLabel.Text = heartbeat.ToString();
                await Task.Delay(1000);
            }
        }
        private void LoadSavedData()
        {
            stepOffset = Preferences.Get("StepOffset", 0);
            stepCount = Preferences.Get("StepCount", 0);
            calories = Preferences.Get("Calories", 0);

            OnPropertyChanged(nameof(StepCount));
            OnPropertyChanged(nameof(Calories));

            MainThread.BeginInvokeOnMainThread(() =>
            {
                stepCountLabel.Text = stepCount.ToString();
                caloriesLabel.Text = calories.ToString();
            });

            UpdateChart();
        }
        private void OnStepCountChanged(object sender, StepCountChangedEventArgs e)
        {
            stepCount = e.StepCount;
            calories = CalculateCalories(stepCount - stepOffset); // Предполагая, что у вас есть метод CalculateCalories

            // Сохранение данных в Preferences
            Preferences.Set("StepCount", stepCount);
            Preferences.Set("Calories", calories);

            OnPropertyChanged(nameof(StepCount));
            OnPropertyChanged(nameof(Calories));
            stepCountLabel.Text = StepCount.ToString();
            caloriesLabel.Text = Calories.ToString();
        }
        private int CalculateCalories(int steps)
        {
            return steps / 20; // Допустим, что на каждые 20 шагов сжигается 1 калория
        }

        private void StartMidnightResetTimer()
        {
            TimeSpan interval = TimeSpan.FromMinutes(1);

            dispatcherTimerService.StartTimer(interval, () =>
            {
                CheckAndResetAtMidnight();
                return true; // Продолжать выполнение таймера
            });
        }

        private DateTime GetCurrentVolgogradTime()
        {
            TimeZoneInfo volgogradTimeZone;
            try
            {
                volgogradTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Volgograd");
            }
            catch (TimeZoneNotFoundException)
            {
                Debug.WriteLine("The Volgograd time zone is not found on the local system.");
                throw;
            }
            catch (InvalidTimeZoneException)
            {
                Debug.WriteLine("The Volgograd time zone contains invalid or missing data.");
                throw;
            }

            DateTime utcNow = DateTime.UtcNow;
            DateTime volgogradTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, volgogradTimeZone);
            return volgogradTime;
        }

        private void CheckAndResetAtMidnight()
        {
            DateTime now = GetCurrentVolgogradTime();
            if (now.Hour == 16 && now.Minute == 25)
            {
                UpdateDailyCalories(now.DayOfWeek, Calories);
                ResetStepCountAndCalories();
                UpdateChart();
            }
        }

        private void UpdateDailyCalories(DayOfWeek day, int calories)
        {
            int dayIndex = (int)day;
            dailyCalories[dayIndex] = calories; // Обновление массива калорий за неделю
                                                // Обновление ObservableCollection WeeklyCalories
            MainThread.BeginInvokeOnMainThread(() =>
            {
                WeeklyCalories[dayIndex] = calories;
            });
        }

        private void ResetStepCountAndCalories()
        {
            stepOffset = stepCount;

            // Сброс значений шагов и калорий
            stepCount = 0;
            calories = 0;

            // Сохранение данных в Preferences
            Preferences.Set("StepOffset", stepOffset);
            Preferences.Set("StepCount", stepCount);
            Preferences.Set("Calories", calories);

            OnPropertyChanged(nameof(StepCount));
            OnPropertyChanged(nameof(Calories));

            MainThread.BeginInvokeOnMainThread(() =>
            {
                stepCountLabel.Text = stepCount.ToString();
                caloriesLabel.Text = calories.ToString();
            });

            UpdateChart();
        }

        private async void UpdateChart()
        {
            var dailyCaloriesList = await App.Database.GetDailyCaloriesAsync();
            var currentWeek = GetCurrentWeekDays();

            for (int i = 0; i < currentWeek.Length; i++)
            {
                var day = currentWeek[i];
                var calories = dailyCaloriesList.FirstOrDefault(c => c.DayOfWeek == day)?.CaloriesBurned ?? 0;
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    WeeklyCalories[i] = calories;
                });

            }

            

            
        }

        private string[] GetCurrentWeekDays()
        {
            var days = new[]
            {
            DayOfWeek.Monday.ToString(),
            DayOfWeek.Tuesday.ToString(),
            DayOfWeek.Wednesday.ToString(),
            DayOfWeek.Thursday.ToString(),
            DayOfWeek.Friday.ToString(),
            DayOfWeek.Saturday.ToString(),
            DayOfWeek.Sunday.ToString()
        };
            return days;
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            base.OnPropertyChanged(propertyName);
        }

        private void ListAllTimeZones()
        {
            foreach (var timezone in TimeZoneInfo.GetSystemTimeZones())
            {
                Debug.WriteLine(timezone.Id);
            }
        }
        public async void WorkOutClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new Page2());
        }
        public async void AllClicked(object sender, EventArgs  e)
        {
            await Navigation.PushModalAsync(new Page3());
            
        }
        public async void WaterClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new Page5());

        }

    }
}







