using Android.Content;
using Android.Media;
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
    public partial class WC1_Select : ContentPage
    {
        static readonly int RegionId = 1;
        private const String SCANNER_ACTION_START = "android.intent.action.M3SCANNER_BUTTON_DOWN";
        private const String SCANNER_ACTION_CANCEL = "android.intent.action.M3SCANNER_BUTTON_UP";

        public WC1_Select()
        {
            InitializeComponent();
            ShowWorkers(Grid_PL);
        }

        #region End
        private void Btn_Esc_Clicked(object sender, EventArgs e) => Navigation.PopAsync();
        protected override bool OnBackButtonPressed() => true;
        #endregion
        private void Btn_Briafing_Clicked(object sender, EventArgs e) => DisplayAlert("Брифинг", "Пока не сделано", "Ок");        

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
                    var GetWorker = Classes.Funfunctions.GetWorker(Barcode);
                    if (GetWorker.Get)  
                    { 
                        Worker = GetWorker.Worker;
                        label.Text = Worker;
                    }
                    else
                    {
                        Save = false;
                        ShowError(GetWorker.Reason);
                    }              
                }
                else 
                {
                    (bool EM, string Worker, string Reason) EM;
                    if (label.Text == "Рабочий не выбран") { EM  = await EngineeringMenu(false); }
                    else { EM = await EngineeringMenu(true); }

                    if (EM.EM) 
                    {
                        if (EM.Worker != "0")
                        {
                            Worker = EM.Worker;
                            label.Text = Worker;

                        }
                        else
                        {
                            Worker = "0";
                            label.Text = "Рабочий не выбран";
                        }
                    }
                    else
                    {
                        ShowError(EM.Reason);
                        Save = false;
                    }
                }

                if (Save)
                {
                    var SetWorker = Classes.Funfunctions.SetWorkerSelect((label.Parent as Grid).TabIndex, Worker);
                    if (SetWorker.Set) { CrossToastPopUp.Current.ShowToastMessage("Успешно", Plugin.Toast.Abstractions.ToastLength.Short); }
                    else { ShowError(SetWorker.Reason); }
                }

            } else { ShowError("NoScan"); }
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
        async Task<(bool EM, string Worker, string Reason)> EngineeringMenu(bool SelectWorker)
        {
            string[] Item = SelectWorker ?
                (new string[] { "Выбор из списка", "Ручной ввод", "Отчистить" }) :
                (new string[] { "Выбор из списка", "Ручной ввод" });

            switch (await DisplayActionSheet("Инженерное меню", "Отмена", null, Item))
            {
                case "Выбор из списка":
                    {
                        var GetWorkers = Classes.Funfunctions.GetWorkers();
                        if (GetWorkers.Get)
                        {
                            string Worker = await DisplayActionSheet("Выбор из списка", "Отмена", null, GetWorkers.Workers.Split('/'));
                            if (Worker != "Отмена") { return (true, Worker, null); }
                            else { return (false, null, "Cancel"); }
                        }
                        else  {  return (false, null, GetWorkers.Reason);  }
                    }

                case "Ручной ввод":
                    {
                        string Worker = await DisplayPromptAsync("Ручной ввод", "Введите ФИО рабочего", "Ок", "Отмена");
                        if (Worker.Length != 0)
                        {
                            if (Worker != "Отмена") { return (true, Worker, null); }
                            else { return (false, null, "Cancel"); }
                        }
                        else { return (false, null, "MinLength"); }
                    }

                case "Отчистить": return (true, "0", null);
                case "Отмена": return (false, null, "Cancel");
                default: return (false, null, "Error");
            }
        }

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
    }
}