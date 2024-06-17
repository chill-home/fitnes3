using fitnes3.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace fitnes3
{
    public partial class App : Application
    {
        static Database database;
        public static Database Database
        {
            get
            {
                if (database == null)
                {
                    // Используем один и тот же путь для создания экземпляра базы данных
                    var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Database.db3");
                    database = new Database(dbPath);
                    database = new Database(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WaterIntake.db3"));
                }
                return database;
            }
        }

        public static TimerData GlobalTimerData { get; set; } = new TimerData();

        public App()
        {
            InitializeComponent();

            // Запрашиваем разрешения на датчики и активность
            RequestPermissions();

            MainPage = new NavigationPage(new Page1());


            Device.StartTimer(TimeSpan.FromMinutes(1), CheckForMidnight);
        }
        private bool CheckForMidnight()
        {
            var currentTime = DateTime.UtcNow.AddHours(4); // Волгоградское время (UTC+4)
            if (currentTime.Hour == 12 && currentTime.Minute == 00)
            {
                SaveDailyCaloriesAsync();
            }
            return true;
        }

        private async void SaveDailyCaloriesAsync()
        {
            var today = DateTime.UtcNow.AddHours(4).DayOfWeek.ToString();
            var caloriesBurned = Page1.colories;  // Используем статическое свойство для получения значения сожжённых калорий за день
            var dailyCalories = new DailyCalories { DayOfWeek = today, CaloriesBurned = caloriesBurned };

            await Database.SaveDailyCaloriesAsync(dailyCalories);
        }

        private async void RequestPermissions()
        {
            var status = await Permissions.RequestAsync<Permissions.Sensors>();
            if (status != PermissionStatus.Granted)
            {
                // Обработка сценария, когда разрешение не было предоставлено
            }
        }

        protected override void OnStart() { }
        protected override void OnSleep() { }
        protected override void OnResume() { }
    }
}
