using Plugin.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RS_TSD.Basic
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class B_Menu : ContentPage
    {
        public B_Menu()
        {
            InitializeComponent();
            string UserName = CrossSettings.Current.GetValueOrDefault("BS_UserName", null);
            if (UserName != null) Lab_UserName.Text = UserName;
            else Application.Current.MainPage = new NavigationPage(new Basic.B_Authorization());
        }

        private async void Btn_Exit_Clicked(object sender, EventArgs e)
        {
            if (await DisplayAlert("Подтверждение выхода", "Вы уверены, что хотите выйти?", "да", "Нет"))
            {
                var Exit = Classes.Funfunctions.Exit();
                if (Exit.Exit) App.Current.MainPage = new NavigationPage(new Basic.B_Authorization());
                else
                {
                    switch (Exit.Reason)
                    {
                        case "False":
                            DisplayAlert("Результат выхода", "Не все смены закрыты", "Ок");
                            break;

                        case "Error":
                            DisplayAlert("Результат выхода", "Неизвестный ошибка", "Ок");
                            break;

                        case "SocketError":
                            DisplayAlert("Результат выхода", "Ошибка передачи данных", "Ок");
                            break;

                        case "ErrorServer":
                            DisplayAlert("Результат выхода", "Ошибка сервера", "Ок");
                            break;

                        default:
                            DisplayAlert("Результат выхода", "Неизвестный ошибка", "Ок");
                            break;
                    }
                }
            }
        }
        protected override bool OnBackButtonPressed() => true;

        private async void Btn_Region_Clicked(object sender, EventArgs e)
        {
            var Access = Classes.Funfunctions.GetAccessRegion((sender as Button).TabIndex);
            await DisplayAlert(Access.Input.ToString(), Access.Mode.ToString(), Access.Reason.ToString());
        }

        private void Btn_WC1_Clicked(object sender, EventArgs e)
        {
            var GetAccess = Classes.Funfunctions.GetAccessRegion(1);
            if (GetAccess.Input)
            {
                Navigation.PushAsync(new Woodcutter1.WC1_Main(GetAccess.Mode));
            }
            else
            {
                switch (GetAccess.Reason)
                {
                    case "NotAccess":
                        DisplayAlert("Выбор цеха", "Цех: \"" + (sender as Button).Text + "\" уже занят", "Ок");
                        break;

                    case "Error":
                        DisplayAlert("Выбор цеха", "Неизвестный ошибка", "Ок");
                        break;

                    case "SocketError":
                        DisplayAlert("Выбор цеха", "Ошибка передачи данных", "Ок");
                        break;

                    case "ErrorServer":
                        DisplayAlert("Выбор цеха", "Ошибка сервера", "Ок");
                        break;

                    default:
                        DisplayAlert("Выбор цеха", "Неизвестный ошибка", "Ок");
                        break;
                }
            }
        }
    }
}