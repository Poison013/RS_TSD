using Android.Content;
using Plugin.Toast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RS_TSD.Woodcutter1
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WC1_Edit : ContentPage
    {
        static readonly int RegionId = 1;
        private const String SCANNER_ACTION_START = "android.intent.action.M3SCANNER_BUTTON_DOWN";
        private const String SCANNER_ACTION_CANCEL = "android.intent.action.M3SCANNER_BUTTON_UP";
        static string Time = "";
        public WC1_Edit()
        {
            InitializeComponent();
            var Setting = Classes.Funfunctions.GetSettingsRegion(RegionId);
            if (Setting.Get)
            {
                Grid_Time.IsVisible = true;
                TP_Time.Time = DateTime.Parse(Setting.settings.TimeStart).TimeOfDay;
                ShowWorkers(Grid_PL);
            }
            else
            {
                switch (Setting.Reason)
                {
                    case "Error":
                        DisplayAlert("Ошибка сохранения рабочего", "Неизвестный ошибка", "Ок");
                        break;

                    case "SocketError":
                        DisplayAlert("Ошибка сохранения рабочего", "Ошибка передачи данных", "Ок");
                        break;

                    case "ServerError":
                        DisplayAlert("Ошибка сохранения рабочего", "Ошибка сервера", "Ок");
                        break;
                }
                Navigation.PopAsync();
            }
        }
        #region End
        private void Btn_Esc_Clicked(object sender, EventArgs e) => Navigation.PopAsync();
        protected override bool OnBackButtonPressed() => true;
        #endregion

        private void Btn_Menu_Clicked(object sender, EventArgs e)
        {
            Btn_PL.Margin = Btn_KR.Margin = Btn_PR.Margin = Btn_OS.Margin = Btn_BK.Margin = new Thickness(0);
            SV_PL.IsVisible = SV_KR.IsVisible = SV_PR.IsVisible = SV_OS.IsVisible = SV_BK.IsVisible = false;

            (sender as Button).Margin = new Thickness(2);

            switch ((sender as Button).TabIndex)
            {
                case 1:
                    ShowWorkers(Grid_PL);
                    SV_PL.IsVisible = true;
                    break;

                case 2:
                    ShowWorkers(Grid_KR);
                    SV_KR.IsVisible = true;
                    break;

                case 3:
                    ShowWorkers(Grid_PR);
                    SV_PR.IsVisible = true;
                    break;

                case 4:
                    ShowWorkers(Grid_OS);
                    SV_OS.IsVisible = true;
                    break;

                case 5:
                    ShowWorkers(Grid_BK);
                    SV_BK.IsVisible = true;
                    break;
            }
        }

        private async void Btn_Scan_Clicked(object sender, EventArgs e)
        {
            string Worker = "";
            bool Scan = true;
            bool Save = true;
            DateTime time = DateTime.Now;
            await Clipboard.SetTextAsync("Null");
            Android.App.Application.Context.SendBroadcast(new Intent(SCANNER_ACTION_START));
            Label label = ((sender as Button).Parent as Grid).Children[0] as Label;

            while (await Clipboard.GetTextAsync() == "Null")
            {
                if ((DateTime.Now - time).TotalSeconds > 5)
                {
                    Android.App.Application.Context.SendBroadcast(new Intent(SCANNER_ACTION_CANCEL));
                    Scan = false;
                    break;
                }
            }
            if (Scan)
            {

                string Barcode = await Clipboard.GetTextAsync();
                if (Barcode != "777")
                {
                    if (label.Text == "Рабочий не выбран")
                    {
                        var GetWorker = Classes.Funfunctions.GetWorker(Barcode);
                        if (GetWorker.Get) { label.Text = GetWorker.Worker; }
                        else
                        {
                            switch (GetWorker.Reason)
                            {
                                case "Error":
                                    await DisplayAlert("Ошибка сохранения рабочего", "Неизвестный ошибка", "Ок");
                                    break;

                                case "SocketError":
                                    await DisplayAlert("Ошибка сохранения рабочего", "Ошибка передачи данных", "Ок");
                                    break;

                                case "ServerError":
                                    await DisplayAlert("Ошибка сохранения рабочего", "Ошибка сервера", "Ок");
                                    break;

                                case "False":
                                    await DisplayAlert("Ошибка сохранения рабочего", "Данные не сохранены, попробуйте еще раз", "Ок");
                                    break;
                            }
                        }

                        var SetWorker = Classes.Funfunctions.SetWorkerEdit(RegionId, (label.Parent as Grid).TabIndex, label.Text, Time);
                        if (SetWorker.Set) { CrossToastPopUp.Current.ShowToastMessage("Успешно", Plugin.Toast.Abstractions.ToastLength.Short); }
                        else
                        {
                            switch (SetWorker.Reason)
                            {
                                case "Error":
                                    await DisplayAlert("Ошибка сохранения рабочего", "Неизвестный ошибка", "Ок");
                                    break;

                                case "SocketError":
                                    await DisplayAlert("Ошибка сохранения рабочего", "Ошибка передачи данных", "Ок");
                                    break;

                                case "ServerError":
                                    await DisplayAlert("Ошибка сохранения рабочего", "Ошибка сервера", "Ок");
                                    break;

                                case "False":
                                    await DisplayAlert("Ошибка сохранения рабочего", "Данные не сохранены, попробуйте еще раз", "Ок");
                                    break;
                            }
                        }
                    }
                    else
                    {
                        string OldNameWorker = label.Text;
                        var GetWorker = Classes.Funfunctions.GetWorker(Barcode);
                        if (GetWorker.Get) { label.Text = GetWorker.Worker; }
                        else
                        {
                            switch (GetWorker.Reason)
                            {
                                case "Error":
                                    await DisplayAlert("Ошибка сохранения рабочего", "Неизвестный ошибка", "Ок");
                                    break;

                                case "SocketError":
                                    await DisplayAlert("Ошибка сохранения рабочего", "Ошибка передачи данных", "Ок");
                                    break;

                                case "ServerError":
                                    await DisplayAlert("Ошибка сохранения рабочего", "Ошибка сервера", "Ок");
                                    break;

                                case "False":
                                    await DisplayAlert("Ошибка сохранения рабочего", "Данные не сохранены, попробуйте еще раз", "Ок");
                                    break;
                            }
                        }

                        var SetWorker = Classes.Funfunctions.ReplaceWorkerEdit(RegionId, label.TabIndex, label.Text, OldNameWorker, Time);
                        if (SetWorker.Replace) { CrossToastPopUp.Current.ShowToastMessage("Успешно", Plugin.Toast.Abstractions.ToastLength.Short); }
                        else
                        {
                            switch (SetWorker.Reason)
                            {
                                case "Error":
                                    await DisplayAlert("Ошибка сохранения рабочего", "Неизвестный ошибка", "Ок");
                                    break;

                                case "SocketError":
                                    await DisplayAlert("Ошибка сохранения рабочего", "Ошибка передачи данных", "Ок");
                                    break;

                                case "ServerError":
                                    await DisplayAlert("Ошибка сохранения рабочего", "Ошибка сервера", "Ок");
                                    break;

                                case "False":
                                    await DisplayAlert("Ошибка сохранения рабочего", "Данные не сохранены, попробуйте еще раз", "Ок");
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    if (label.Text == "Рабочий не выбран") { EngineeringMenu(label, false); }
                    else { EngineeringMenu(label, true); }
                }
            }
        }
        async void EngineeringMenu(Label Lab, bool Select)
        {
            switch (Select)
            {
                case true:
                    {
                        switch (await DisplayActionSheet("Инженерное меню", "Отмена", null, "Выбор из списка", "Ручной ввод", "Отчистить"))
                        {
                            case "Выбор из списка":
                                {
                                    string OldNameWorker = Lab.Text;
                                    var GetWorkers = Classes.Funfunctions.GetWorkers();
                                    if (GetWorkers.Get) 
                                    {
                                        Lab.Text = await DisplayActionSheet("Выбор из списка", "Отмена", null, GetWorkers.Workers.Split('/'));

                                        var ReplaceWorker = Classes.Funfunctions.ReplaceWorkerEdit(RegionId, (Lab.Parent as Grid).TabIndex, Lab.Text, OldNameWorker, Time);
                                        if (ReplaceWorker.Replace) { CrossToastPopUp.Current.ShowToastMessage("Успешно", Plugin.Toast.Abstractions.ToastLength.Short); }
                                        else
                                        {
                                            switch (ReplaceWorker.Reason)
                                            {
                                                case "Error":
                                                    await DisplayAlert("Ошибка сохранения рабочего", "Неизвестный ошибка", "Ок");
                                                    break;

                                                case "SocketError":
                                                    await DisplayAlert("Ошибка сохранения рабочего", "Ошибка передачи данных", "Ок");
                                                    break;

                                                case "ServerError":
                                                    await DisplayAlert("Ошибка сохранения рабочего", "Ошибка сервера", "Ок");
                                                    break;

                                                case "False":
                                                    await DisplayAlert("Ошибка сохранения рабочего", "Данные не сохранены, попробуйте еще раз", "Ок");
                                                    break;
                                            }
                                        }
                                    }
                                    else { ShowError(GetWorkers.Reason); }
                                }
                                break;

                            case "Ручной ввод":
                                {
                                    string OldNameWorker = Lab.Text;
                                    Lab.Text = await DisplayPromptAsync("Выбор из списка", "Введите данные рабочего", "Ок");

                                    var ReplaceWorker = Classes.Funfunctions.ReplaceWorkerEdit(RegionId, (Lab.Parent as Grid).TabIndex, Lab.Text, OldNameWorker, Time);
                                    if (ReplaceWorker.Replace) { CrossToastPopUp.Current.ShowToastMessage("Успешно", Plugin.Toast.Abstractions.ToastLength.Short); }
                                    else { ShowError(ReplaceWorker.Reason); }
                                }
                                break;

                            case "Отчистить":
                                {
                                    var RemoveWorker = Classes.Funfunctions.RemoveWorkerEdit(RegionId, (Lab.Parent as Grid).TabIndex, Lab.Text, Time);
                                    if (RemoveWorker.Remove)
                                    {
                                        Lab.Text = "Рабочий не выбран";
                                        CrossToastPopUp.Current.ShowToastMessage("Успешно", Plugin.Toast.Abstractions.ToastLength.Short);
                                    }
                                    else { ShowError(RemoveWorker.Reason); }
                                }
                                break;
                        }
                    }
                    break;

                case false:
                    switch (await DisplayActionSheet("Инженерное меню", "Отмена", null, "Выбор из списка", "Ручной ввод"))
                    {
                        case "Выбор из списка":
                            {
                                var GetWorkers = Classes.Funfunctions.GetWorkers();
                                if (GetWorkers.Get)
                                {
                                    Lab.Text = await DisplayActionSheet("Выбор из списка", "Отмена", null, GetWorkers.Workers.Split('/'));
                                    var SetWorker = Classes.Funfunctions.SetWorkerEdit(RegionId, (Lab.Parent as Grid).TabIndex, Lab.Text, Time);
                                    if (SetWorker.Set) { CrossToastPopUp.Current.ShowToastMessage("Успешно", Plugin.Toast.Abstractions.ToastLength.Short); }
                                    else { ShowError(SetWorker.Reason); }
                                }
                                else { ShowError(GetWorkers.Reason); }
                            }
                            break;

                        case "Ручной ввод":
                            { 
                                Lab.Text = await DisplayPromptAsync("Выбор из списка", "Введите данные рабочего", "Ок");
                                var SetWorker = Classes.Funfunctions.SetWorkerEdit(RegionId, (Lab.Parent as Grid).TabIndex, Lab.Text, Time);
                                if (SetWorker.Set) { CrossToastPopUp.Current.ShowToastMessage("Успешно", Plugin.Toast.Abstractions.ToastLength.Short); }
                                else { ShowError(SetWorker.Reason); }
                            }
                            break;
                        case "Отмена": ShowError("Cancel"); break;
                        default: ShowError("Error"); break;
                    }
                    break;
            }
        }
        void ShowWorkers(Grid grid)
        {
            var GetWorkers = Classes.Funfunctions.GetWorkersRegion(RegionId);

            if (GetWorkers.Get)
            {
                string[] Worker = GetWorkers.Workers.Split('/');
                foreach (var item in grid.Children)
                {
                    if (item is Grid && item.TabIndex > 0)
                    {
                        if (Worker[item.TabIndex - 1] != "") ((item as Grid).Children[0] as Label).Text = Worker[item.TabIndex - 1];
                        else ((item as Grid).Children[0] as Label).Text = "Рабочий не выбран";
                    }
                }
            }
            else
            {
                Navigation.PopAsync();
                ShowError(GetWorkers.Reason);
            }
        }

        async void ShowError(string Error)
        {
            switch (Error)
            {
                case "Cancel":
                    CrossToastPopUp.Current.ShowToastMessage("Действие отменено", Plugin.Toast.Abstractions.ToastLength.Short);
                    break;

                case "NoScan":
                    CrossToastPopUp.Current.ShowToastMessage("Штрихкод не отсканирован", Plugin.Toast.Abstractions.ToastLength.Short);
                    break;

                case "MinLength":
                    await DisplayAlert("Ошибка", "Данные отсутствуют", "Ок");
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

        private async void Btn_Next_Clicked(object sender, EventArgs e)
        {
            var GetSettings = Classes.Funfunctions.GetSettingsRegion(RegionId);
            if (GetSettings.Get)
            {
                if ((TP_Time.Time - DateTime.Parse(GetSettings.settings.TimeStart).TimeOfDay).TotalMinutes > 1)
                {
                    Time = TP_Time.Time.ToString().Split(':')[0] + ":" + TP_Time.Time.ToString().Split(':')[1];
                    Grid_Time.IsVisible = false;
                }
                else
                {
                    DisplayAlert("Ошибка", "Не правильно выбрано время", "Ок");
                }
            }
            else
            {
                switch (GetSettings.Reason)
                {
                    case "Error":
                        await DisplayAlert("Ошибка сохранения рабочего", "Неизвестный ошибка", "Ок");
                        break;

                    case "SocketError":
                        await DisplayAlert("Ошибка сохранения рабочего", "Ошибка передачи данных", "Ок");
                        break;

                    case "ServerError":
                        await DisplayAlert("Ошибка сохранения рабочего", "Ошибка сервера", "Ок");
                        break;

                    case "False":
                        await DisplayAlert("Ошибка сохранения рабочего", "Данные не сохранены, попробуйте еще раз", "Ок");
                        break;
                }
            }
        }
    }
}