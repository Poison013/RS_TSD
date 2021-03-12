using Plugin.Toast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RS_TSD.General
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class G_Settings : ContentPage
    {
        string[] Item1 = new string[] { "Тонкий лес", "Толстый лес", "Тонкий брак", "Тослый брак", "Лиственница", "Ручной ввод"};
        string[] Item2 = new string[] { "Тонкий лес", "Толстый лес", "Ручной ввод"};
        string[] Item3 = new string[] { "Вагон", "Платформа", "Машина", "Ручной ввод"};
        string[] Item4 = new string[] { "Сосна 4м", "Сосна 3м", "Лиственница", "Ручной ввод"};
        string[] Item5 = new string[] { "Сосна 4м", "Сосна 6м", "Лиственница 4м", "Лиственница 6м", "Ручной ввод"};
        string[] Item6 = new string[] { "4м", "6м", "4,5м", "Ручной ввод"};
        string[] Item7 = new string[] { "Тонкий лес", "Тонкий брак", "Лиственница", "Баланс", "Ручной ввод"};
        string[] Items;
        int RegionId;
        public G_Settings(int RegionId)
        {
            InitializeComponent();

            this.RegionId = RegionId;
            switch (RegionId)
            {
                case 1:
                    Items = Item1;
                    break;

                case 2:
                    Items = Item2;
                    break;

                case 3:
                    Items = Item3;
                    break;

                case 4:
                    Items = Item4;
                    break;

                case 5:
                    Items = Item5;
                    break;

                case 6:
                    Items = Item6;
                    break;

                case 7:
                    Items = Item7;
                    break;
            }

            foreach (string Item in Items) Picker_Mode.Items.Add(Item);

            var Settings = Classes.Funfunctions.GetSettingsRegion(RegionId);
            if (Settings.Get)
            {
                Lab_Date.Text = Settings.settings.Date != null ? Settings.settings.Date : DateTime.Now.ToShortDateString();//дата
                if (Settings.settings.TimeStart != null)
                {
                    string[] Time = Settings.settings.TimeStart.Split(':');
                    Picker_Time1.SelectedItem = Time[0];
                    Picker_Time2.SelectedItem = Time[1];
                }//время
                else
                {
                    Picker_Time1.SelectedItem = "08";
                    Picker_Time2.SelectedItem = "00";
                }

                if (Settings.settings.Mode != null)
                {
                    int Count = (from t in Items where t == Settings.settings.Mode select t).Count();
                    if (Count == 0) { Picker_Mode.Items.Add(Settings.settings.Mode); }
                    Picker_Mode.SelectedItem = Settings.settings.Mode;
                }//режим пиления
            }
            else
            {
                switch (Settings.Reason)
                {
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

        #region Esc
        private void Btn_Esc_Clicked(object sender, EventArgs e) => Navigation.PopAsync();
        protected override bool OnBackButtonPressed() => true;
        #endregion

        #region EditData
        private void Btn_EditDate_Clicked(object sender, EventArgs e) => Lab_Date.Text = Convert.ToDateTime(Lab_Date.Text).AddDays((sender as Button).TabIndex).ToShortDateString();       
        private async void Picker_Mode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((string)(sender as Picker).SelectedItem == "Ручной ввод")
            {
                string Mode = await DisplayPromptAsync("Режим", "Введите режим", "Ок", null, null);
                Picker_Mode.Items.Add(Mode);
                Picker_Mode.SelectedItem = Mode;
            }
        }
        #endregion

        private void Btn_Save_Clicked(object sender, EventArgs e)
        {
            if (Picker_Time1.SelectedIndex != -1 && Picker_Time2.SelectedIndex != -1 && Picker_Mode.SelectedIndex != -1)
            {
              Classes.Settings settings = new Classes.Settings()
                {
                    Date = Lab_Date.Text,
                    TimeStart = Picker_Time1.SelectedItem.ToString() + ":" + Picker_Time2.SelectedItem.ToString(),
                    Mode = Picker_Mode.SelectedItem.ToString()
                };

                var Save = Classes.Funfunctions.SetSettingsRegion(RegionId, settings);

                if (Save.Set) { CrossToastPopUp.Current.ShowToastMessage("Успешно", Plugin.Toast.Abstractions.ToastLength.Short); }
                else
                {
                    switch (Save.Reason)
                    {
                        case "False":
                            DisplayAlert("Сохранение настроек", "Что-то пошло не так", "Ок");
                            break;

                        case "Error":
                            DisplayAlert("Сохранение настроек", "Неизвестный ошибка", "Ок");
                            break;

                        case "SocketError":
                            DisplayAlert("Сохранение настроек", "Ошибка передачи данных", "Ок");
                            break;

                        case "ErrorServer":
                            DisplayAlert("Сохранение настроек", "Ошибка сервера", "Ок");
                            break;
                    }
                }
            }
            else { DisplayAlert("Сохранение настроек", "Не все данные заполнены", "Ок"); }
        }
    }
}