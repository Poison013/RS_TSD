using Newtonsoft.Json;
using Plugin.Settings;
using Plugin.Toast;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RS_TSD.Basic
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class B_Settings : ContentPage
    {
        public B_Settings()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            string IP = CrossSettings.Current.GetValueOrDefault("BS_IP", null);
            string Port = CrossSettings.Current.GetValueOrDefault("BS_Port", null);
            if (IP != null) Entry_IP.Text = IP;
            if (Port != null) Entry_Port.Text = Port;
        }

        private void Btn_Back_Clicked(object sender, EventArgs e) => Navigation.PopAsync();
        protected override bool OnBackButtonPressed() => true;

        private async void Btn_TestConnect_Clicked(object sender, EventArgs e)
        {
            if (Entry_IP.Text != null && Entry_IP.Text != "" &&
                Entry_Port.Text != null && Entry_Port.Text != "")
            {
                try
                {
                    TcpClient client = new TcpClient(Entry_IP.Text, Convert.ToInt32(Entry_Port.Text));
                    NetworkStream stream = client.GetStream();
                    // отправляем сообщение
                    StreamWriter writer = new StreamWriter(stream);

                    string Request = JsonConvert.SerializeObject(new Classes.Request() { Program = "TestConnect" });

                    writer.WriteLine(Request);
                    writer.Flush();

                    StreamReader reader = new StreamReader(stream);
                    string Result = reader.ReadLine();

                    writer.Close();
                    reader.Close();
                    stream.Close();
                    if (Result == "TestConnect")  await DisplayAlert("Результат проверки соединения", "Всё работает", "Ок");
                    else await DisplayAlert("Результат проверки соединения", "Данные не соответствуют", "Ок");
                }
                catch (SocketException) { await DisplayAlert("Проверка соединения", "Произошла ошибка сокета.", "Ок"); }
                catch (Exception) { await DisplayAlert("Проверка соединения", "Произошла ошибка.", "Ок"); }
            }
            else await DisplayAlert("Проверка соединения", "Заполните все данные.", "Ок");
        }

        private async void Btn_Save_Clicked(object sender, EventArgs e)
        {
            if (Entry_IP.Text != null && Entry_IP.Text != "" &&
                Entry_Port.Text != null && Entry_Port.Text != "")
            {
                if (await DisplayAlert("Подтверждение сохранения", "Вы уверены, что хотите сохранить настройки", "Да", "Нет"))
                {
                    if (await DisplayPromptAsync("Подтверждение сохранения", "Введите пароль, чтобы сохранить", "Ок", null, maxLength: 4, keyboard: Keyboard.Numeric) == "8888")
                    {
                        CrossSettings.Current.AddOrUpdateValue("BS_IP", Entry_IP.Text);
                        CrossSettings.Current.AddOrUpdateValue("BS_Port", Entry_Port.Text);
                        CrossToastPopUp.Current.ShowToastMessage("Успешно", Plugin.Toast.Abstractions.ToastLength.Short);
                    }
                    else await DisplayAlert("Ошибка сохранения", "Неверный пароль.", "Ок");
                }
                else CrossToastPopUp.Current.ShowToastMessage("Действие отменено", Plugin.Toast.Abstractions.ToastLength.Short);
            }
            else await DisplayAlert("Ошибка сохранения", "Не все данные заполнены.", "Ок");
        }
    }
}