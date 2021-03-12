using Plugin.Settings;
using Plugin.Toast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RS_TSD.Woodcutter1
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WC1_Main : ContentPage
    {
        static readonly int RegionId = 1;
        static string UserId = CrossSettings.Current.GetValueOrDefault("BS_UserId", null);
        static int _Work;
        public WC1_Main(int Work)
        {
            InitializeComponent();
            if (Work != 2)
            {
                var SetAccess = Classes.Funfunctions.SetAccessRegion(RegionId, UserId, 1);
                if (!SetAccess.Set) { ShowError(SetAccess.Reason); }
            }
            else
            {
                Btn_Select.IsVisible = Btn_Start.IsVisible = Btn_Settings.IsVisible = false;
                Btn_Edit.IsVisible = Btn_End.IsVisible = Btn_History.IsVisible = true;
            }
            _Work = Work;
        }
        protected override void OnAppearing()
        {
            var GetSettings = Classes.Funfunctions.GetSettingsRegion(RegionId);
            var GetWorkers = Classes.Funfunctions.GetWorkersRegion(RegionId);

            if (GetSettings.Get && GetWorkers.Get)
            {
                Lab_Data.Text = "Смена: " + GetSettings.settings.Date + " Время: " + GetSettings.settings.TimeStart;
                string[] Worker = GetWorkers.Workers.Split('/');
                foreach (var item in Grid_Workers.Children)
                {
                    if (item is Label && item.TabIndex > 0)
                    {
                        if (Worker[item.TabIndex - 1] != "") (item as Label).Text = Worker[item.TabIndex - 1];
                        else (item as Label).Text = "Рабочий не выбран";
                    }
                }
            }
            else
            {
                if (GetSettings.Reason != null) { ShowError(GetSettings.Reason); }
                else { ShowError(GetWorkers.Reason); }
                Navigation.PopAsync();
            }
        }

        #region Esc
        private void Btn_Esc_Clicked(object sender, EventArgs e)
        {
            if (_Work != 2)
            {
                var SetAccess = Classes.Funfunctions.SetAccessRegion(RegionId, null, null);
                if (SetAccess.Set) {  Navigation.PopAsync(); }
                else { ShowError(SetAccess.Reason); }
            }
            else { Navigation.PopAsync(); }
        }
        protected override bool OnBackButtonPressed() => true;
        #endregion

        #region Btn_Navigation
        private void Btn_Settings_Clicked(object sender, EventArgs e) => Navigation.PushAsync(new General.G_Settings(RegionId));
        private void Btn_History_Clicked(object sender, EventArgs e) => Navigation.PushAsync(new General.G_History(RegionId));
        private void Btn_Select_Clicked(object sender, EventArgs e) => Navigation.PushAsync(new WC1_Select());
        private void Btn_Edit_Clicked(object sender, EventArgs e) => Navigation.PushAsync(new WC1_Edit());
        #endregion

        #region Start
        private async void Btn_Start_Clicked(object sender, EventArgs e)
        {
            var GetSettings = Classes.Funfunctions.GetSettingsRegion(RegionId);
            if (GetSettings.Get)
            {
                if (GetSettings.settings.Date != null && GetSettings.settings.TimeStart != null && GetSettings.settings.Mode != null)
                {
                    Lab_Start_Date.Text = GetSettings.settings.Date;
                    Lab_Start_Time.Text = GetSettings.settings.TimeStart;
                    Lab_Start_Mode.Text = GetSettings.settings.Mode;
                    Grid_Start.IsVisible = true;
                }
                else { await DisplayAlert("Ошибка начала смены", "Заполните все настройки смены", "Ок"); }
            }
            else { ShowError(GetSettings.Reason); }        
        }
        private void Btn_Start_No_Clicked(object sender, EventArgs e) => Grid_Start.IsVisible = false;
        private void Btn_Start_Yes_Clicked(object sender, EventArgs e)
        {
            var StartWork = Classes.Funfunctions.StartWork(RegionId);
            if (StartWork.Start)
            {
                Grid_Start.IsVisible = Btn_Select.IsVisible = Btn_Start.IsVisible = Btn_Settings.IsVisible = false;
                Btn_Edit.IsVisible = Btn_End.IsVisible = Btn_History.IsVisible = true;
                _Work = 2;
                Classes.Funfunctions.SetAccessRegion(RegionId, UserId, 2);
            }
            else {
                DisplayAlert("1223", StartWork.Reason, "0");
                ShowError(StartWork.Reason); }
        }
        #endregion

        #region End
        private void Btn_End_Clicked(object sender, EventArgs e)
        {
            Grid_End.IsVisible = true;
            var GetSettings = Classes.Funfunctions.GetSettingsRegion(RegionId);
            if (GetSettings.Get)
            {
                if (GetSettings.settings.TimeEnd != null) { TP_End_Time.Time = DateTime.Parse(GetSettings.settings.TimeEnd).TimeOfDay; }
                else { TP_End_Time.Time = DateTime.Parse(GetSettings.settings.TimeStart).TimeOfDay; }
            }
            else { ShowError(GetSettings.Reason); }
        }
        private void Btn_End_No_Clicked(object sender, EventArgs e) => Grid_End.IsVisible = false;
        private void Btn_End_Yes_Clicked(object sender, EventArgs e)
        {
            var GetSettings = Classes.Funfunctions.GetSettingsRegion(RegionId);
            if (GetSettings.Get)
            {
                if (GetSettings.settings.TimeEnd != null)
                {
                    if ((TP_End_Time.Time - DateTime.Parse(GetSettings.settings.TimeEnd).TimeOfDay).TotalMinutes > 1)
                    {
                        string Time = TP_End_Time.Time.ToString();

                        var EndWork = Classes.Funfunctions.EndWork(RegionId, Time.Split(':')[0] + ":" + Time.Split(':')[1]);
                        if (EndWork.End) { Classes.Funfunctions.SetAccessRegion(RegionId, UserId, 1); }
                        else { ShowError(EndWork.Reason); }
                    }
                    else { DisplayAlert("Ошибка", "Не правильно выбрано время", "Ок"); }
                }
                else
                {
                    if ((TP_End_Time.Time - DateTime.Parse(GetSettings.settings.TimeEnd).TimeOfDay).TotalMinutes > 1)
                    {
                        string Time = TP_End_Time.Time.ToString();

                        var EndWork = Classes.Funfunctions.EndWork(RegionId, Time.Split(':')[0] + ":" + Time.Split(':')[1]);
                        if (EndWork.End) { Classes.Funfunctions.SetAccessRegion(RegionId, UserId, 1); }
                        else { ShowError(EndWork.Reason); }
                    }
                    else { DisplayAlert("Ошибка", "Не правильно выбрано время", "Ок"); }
                }
            }
            else { DisplayAlert("Ошибка", "Не правильно выбрано время", "Ок"); }           
        }

        private void Btn_End_New_Clicked(object sender, EventArgs e)
        {
            var GetSettings = Classes.Funfunctions.GetSettingsRegion(RegionId);
            if (GetSettings.Get)
            {
                if (GetSettings.settings.TimeEnd != null)
                {
                    if ((TP_End_Time.Time - DateTime.Parse(GetSettings.settings.TimeEnd).TimeOfDay).TotalMinutes > 1)
                    {
                        string Time = TP_End_Time.Time.ToString();

                        var NewWork = Classes.Funfunctions.NewWork(RegionId, Time.Split(':')[0] + ":" + Time.Split(':')[1]);
                        if (NewWork.End) { Classes.Funfunctions.SetAccessRegion(RegionId, UserId, 1); }
                        else { ShowError(NewWork.Reason); }
                    }
                    else { DisplayAlert("Ошибка", "Не правильно выбрано время", "Ок"); }
                }
                else
                {
                    if ((TP_End_Time.Time - DateTime.Parse(GetSettings.settings.TimeEnd).TimeOfDay).TotalMinutes > 1)
                    {
                        string Time = TP_End_Time.Time.ToString();

                        var NewWork = Classes.Funfunctions.NewWork(RegionId, Time.Split(':')[0] + ":" + Time.Split(':')[1]);
                        if (NewWork.End) { Classes.Funfunctions.SetAccessRegion(RegionId, UserId, 1); }
                        else { ShowError(NewWork.Reason); }
                    }
                    else { DisplayAlert("Ошибка", "Не правильно выбрано время", "Ок"); }
                }
            }
            else { DisplayAlert("Ошибка", "Не правильно выбрано время", "Ок"); }
        }
        #endregion

        async void ShowError(string Error)
        {
            switch (Error)
            {
                /* case "Cancel":
                     CrossToastPopUp.Current.ShowToastMessage("Действие отменено", Plugin.Toast.Abstractions.ToastLength.Short);
                     break;

                 case "NoScan":
                     CrossToastPopUp.Current.ShowToastMessage("Штрихкод не отсканирован", Plugin.Toast.Abstractions.ToastLength.Short);
                     break;

                 case "MinLength":
                     await DisplayAlert("Ошибка", "Данные отсутствуют", "Ок");
                     break;*/
                case "StartNo":
                    await DisplayAlert("Ошибка", "Смена не начата", "Ок");
                    break;

                case "Error":
                    await DisplayAlert("Ошибка", "Неизвестный ошибка", "Ок");
                    break;

                case "SocketError":
                    await DisplayAlert("Ошибка", "Ошибка передачи данных", "Ок");
                    break;

                case "ServerError":
                    await DisplayAlert("Ошибка", "Ошибка сервера", "Ок");
                    break;
            }
        }
    }
}