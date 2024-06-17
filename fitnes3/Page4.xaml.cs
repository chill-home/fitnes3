using CloudKit;
using fitnes3.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace fitnes3
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Page4 : ContentPage
	{
        private Database _database;
        public Page4()
        {
            InitializeComponent();
            CreateButton.Clicked += OnCreateButtonClicked;
        }

        private async void OnCreateButtonClicked(object sender, EventArgs e)
        {
            var user = new User
            {
                Username = UsernameEntry.Text,
                Password = PasswordEntry.Text,
                Email = EmailEntry.Text
            };

            try
            {
                var existingUser = await App.Database.GetUserAsync(user.Username, user.Password);

                if (existingUser == null)
                {
                    await App.Database.SaveUserAsync(user);
                    await DisplayAlert("Success", "User registered", "OK");
                }
                else
                {
                    await DisplayAlert("Error", "User already exists", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }
    }
}

