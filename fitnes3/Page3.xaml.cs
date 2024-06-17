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
	public partial class Page3 : ContentPage
	{
        private List<DataPoint> dataPoints;
        private const double xScaleFactor = 1.8;
        public Page3 ()
        {
            InitializeComponent();
            var timerData = App.GlobalTimerData;

            if (timerData.TimerRunning)
            {
                exerciseLabel.Text = $"{timerData.ExerciseType} {timerData.RemainingTime:mm\\:ss} оставшееся";
            }
            else
            {
                exerciseLabel.Text = "Нет активных занятий";
            }
            // Инициализация списка точек данных
            dataPoints = new List<DataPoint>();

            // Пример: добавляем существующие данные
            dataPoints.Add(new DataPoint { Month = 0, Weight = 70 });
            dataPoints.Add(new DataPoint { Month = 1, Weight = 75 });

            // Обновление диаграммы
            RefreshGraph();
            LoadDataPoints();
        }
        private async void LoadDataPoints()
        {
            dataPoints = await App.Database.GetDataPointsAsync();
            RefreshGraph();
        }

        private async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            if (double.TryParse(WeightEntry.Text, out double weight) && MonthPicker.SelectedIndex >= 0)
            {
                if (weight >= 0 && weight <= 120)
                {
                    var existingPoint = dataPoints.FirstOrDefault(dp => dp.Month == MonthPicker.SelectedIndex);
                    if (existingPoint != null)
                    {
                        existingPoint.Weight = weight;
                        await App.Database.SaveDataPointAsync(existingPoint);
                    }
                    else
                    {
                        var newPoint = new DataPoint { Month = MonthPicker.SelectedIndex, Weight = weight };
                        dataPoints.Add(newPoint);
                        await App.Database.SaveDataPointAsync(newPoint);
                    }

                    RefreshGraph(); 
                }
                else
                {
                   await DisplayAlert("Ошибка", "Вес вне допустимого диапазона.", "OK");
                }
            }
            else
            {
               await DisplayAlert("Ошибка", "Некорректный ввод.", "OK");
            }
        }

        private void RefreshGraph()
        {
            // Очистка диаграммы
            GraphLayout.Children.Clear();

            // Фон диаграммы
            var background = new BoxView { Color = Color.FromHex("White") };
            AbsoluteLayout.SetLayoutBounds(background, new Rectangle(0, 0, 1, 1));
            AbsoluteLayout.SetLayoutFlags(background, AbsoluteLayoutFlags.All);
            GraphLayout.Children.Add(background);

            // Добавление точек на диаграмму
            foreach (var point in dataPoints)
            {
                double normalizedValue = point.Weight / 120.0;  // Нормализация веса
                double xPosition = (point.Month / 5.0) * xScaleFactor;  // Позиция по оси X в диапазоне от 0 до 1

                var dot = new BoxView
                {
                    Color = Color.White,
                    WidthRequest = 10,
                    HeightRequest = 10,
                    CornerRadius = 5
                };

                AbsoluteLayout.SetLayoutBounds(dot, new Rectangle(xPosition, 1 - normalizedValue, 10, 10));
                AbsoluteLayout.SetLayoutFlags(dot, AbsoluteLayoutFlags.PositionProportional);
                GraphLayout.Children.Add(dot);
            }
        }
    }
}

