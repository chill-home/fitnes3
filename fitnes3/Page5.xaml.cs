using fitnes3.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace fitnes3
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page5 : ContentPage
    {
        private ObservableCollection<WaterIntakePair> _waterIntakeHistory;
        private int _totalWaterIntake;
        private int _currentLabelIndex;
        private const int DailyWaterGoal = 2000;
        private int[] _waterAmounts;

        public Page5()
        {
            InitializeComponent();
            _waterIntakeHistory = new ObservableCollection<WaterIntakePair>();
            _waterAmounts = new int[4] { 0, 0, 0, 0 };
            _totalWaterIntake = 0;
            _currentLabelIndex = 0;
            LoadWaterIntakeHistory();
            StartMidnightResetTimer();
        }

        private async void LoadWaterIntakeHistory()
        {
            
            var intakeHistory = await App.Database.GetWaterIntakesAsync();
            _totalWaterIntake = intakeHistory.Sum(intake => int.Parse(intake.Amount));
            foreach (var intake in intakeHistory)
            {
                AddWaterToLabel(int.Parse(intake.Amount));
            }

            UpdateTotalWaterIntakeLabel();
        }

        private void AddWaterToLabel(int amount)
        {
            switch (_currentLabelIndex)
            {
                case 0:
                    UpdateLabel(Label1, amount);
                    break;
                case 1:
                    UpdateLabel(Label2, amount);
                    break;
                case 2:
                    UpdateLabel(Label3, amount);
                    break;
                case 3:
                    UpdateLabel(Label4, amount);
                    break;
            }

            _currentLabelIndex = (_currentLabelIndex + 1) % 4; // Переход к следующей метке
        }
        private void UpdateLabel(Label label, int amount)
        {
            var currentText = label.Text;
            var currentAmount = int.Parse(currentText.Split(':')[1].Trim().Split(' ')[0]);
            var newAmount = currentAmount + amount;
            label.Text = $"{label.Text.Split(':')[0]} : {newAmount} мл";
        }

        private async Task AddWater(int amount)
        {
            try
            {
                var intake = new WaterIntake
                {
                    Amount = amount.ToString(),
                    TimeAdded = DateTime.Now
                };

                await App.Database.SaveWaterIntakeAsync(intake);

                AddWaterToLabel(amount);

                _totalWaterIntake += amount;
                UpdateTotalWaterIntakeLabel();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                await DisplayAlert("Ошибка", "Произошла ошибка при добавлении воды.", "OK");
            }
        }

        private async void AddWaterButtonClicked(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Выберите количество воды", "Отмена", null, "100 мл", "250 мл", "500 мл", "1000 мл");
            int amount = 0;

            switch (action)
            {
                case "100 мл":
                    amount = 100;
                    break;
                case "250 мл":
                    amount = 250;
                    break;
                case "500 мл":
                    amount = 500;
                    break;
                case "1000 мл":
                    amount = 1000;
                    break;
                default:
                    return; // Если было выбрано "Отмена"
            }

            await AddWater(amount);
        }
        private void UpdateTotalWaterIntakeLabel()
        {
            TotalWaterIntakeLabel.Text = $"{_totalWaterIntake} мл";
        }

        private void StartMidnightResetTimer()
        {
            TimeSpan interval = TimeSpan.FromSeconds(86400);

            Device.StartTimer(interval, () =>
            {
                ResetWaterIntakeData();

                // Настроить следующий таймер на короткий интервал
                Device.StartTimer(interval, () =>
                {
                    ResetWaterIntakeData();
                    return true; // Возвращение true для продолжения запуска таймера
                });

                return true; // Возвращение true для продолжения запуска таймера
            });
        }


        private  void ResetWaterIntakeData()
        {
            _totalWaterIntake = 0;
            _currentLabelIndex = 0;
            for (int i = 0; i < _waterAmounts.Length; i++)
            {
                _waterAmounts[i] = 0;
            }

             Device.BeginInvokeOnMainThread(() =>
            {
                Label1.Text = "Выпито: 0 мл";
                Label2.Text = "Выпито: 0 мл";
                Label3.Text = "Выпито: 0 мл";
                Label4.Text = "Выпито: 0 мл";
                UpdateTotalWaterIntakeLabel();
                App.Database.DeleteAllWaterIntakesAsync();
            });
        }

    }
}




