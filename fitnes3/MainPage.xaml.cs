using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace fitnes3
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        private async void LoginClicked(object sender, EventArgs e)
        {
            var username = usernameEntry.Text;
            var password = passwordEntry.Text;

            var user = await App.Database.GetUserAsync(username, password);
            if (user != null)
            {
                await DisplayAlert("Успех", "Вы успешно вошли", "ОК");
                // Перенаправление на главную или защищенную страницу
                await Navigation.PushAsync(new MainPage());
            }
            else
            {
                await DisplayAlert("Ошибка", "Неверный логин или пароль", "ОК");
            }
            await Navigation.PushModalAsync(new Page1());
        }
        private async void RegisterClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new Page4());
        }
    }
}
