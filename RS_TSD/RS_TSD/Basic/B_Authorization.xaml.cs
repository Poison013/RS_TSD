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
    public partial class B_Authorization : ContentPage
    {
        public B_Authorization()
        {
            InitializeComponent();
        }

        private void Btn_Settings_Clicked(object sender, EventArgs e) => Navigation.PushAsync(new Basic.B_Settings());

        private void Btn_Authorization_Clicked(object sender, EventArgs e)
        {
            Lab_Error.Text = "";
            string IP = CrossSettings.Current.GetValueOrDefault("BS_IP", null);
            string Port = CrossSettings.Current.GetValueOrDefault("BS_Port", null);
            if (IP != null && Port != null)
            {
                if (Entry_Login.Text != null && Entry_Login.Text != "" &&
                    Entry_Password.Text != null && Entry_Password.Text != "")
                {
                    var Authorization = Classes.Funfunctions.Authorization(Entry_Login.Text, Entry_Password.Text);
                    if (Authorization.Auth) Application.Current.MainPage = new NavigationPage(new Basic.B_Menu());
                    else
                    {
                        switch (Authorization.Reason)
                        {
                            case "NotAccess":
                                Lab_Error.Text = "Нет доступа";
                                break;

                            case "NotUser":
                                Lab_Error.Text = "Неверный логин или пароль";
                                break;

                            case "LotUser":
                                Lab_Error.Text = "Данные подходят к нескольким пользователям";
                                break;

                            case "Error":
                                DisplayAlert("Результат авторизации", "Неизвестный ошибка", "Ок");
                                break;

                            case "SocketError":
                                DisplayAlert("Результат авторизации", "Ошибка передачи данных", "Ок");
                                break;

                            case "ErrorServer":
                                DisplayAlert("Результат авторизации", "Ошибка сервера", "Ок");
                                break;

                            default:
                                DisplayAlert("Результат авторизации", "Неизвестный ошибка", "Ок");
                                break;
                        }
                    }
                }
                else Lab_Error.Text = "Введите логин и пароль";
            }
            else DisplayAlert("Ошибка действия", "Заполните настройки.", "Ок");
        }
    }
}