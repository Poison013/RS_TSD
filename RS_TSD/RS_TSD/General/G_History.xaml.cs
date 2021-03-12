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
    public partial class G_History : ContentPage
    {
        int RegionId;
        public G_History(int RegionId)
        {
            InitializeComponent();
            this.RegionId = RegionId;
        }
        protected override void OnAppearing()
        {
            int CountRow = 1;
            var GetHistory = Classes.Funfunctions.GetHistory(RegionId);
            if (GetHistory.Get)
            {
                if (GetHistory.History != null)
                {
                    foreach (string item in GetHistory.History.Split('/'))
                    {
                        string[] Data = item.Split('*');

                        RowDefinition HistoryRow = new RowDefinition
                        { Height = 40 };

                        Label Lab_Job = new Label
                        {
                            BackgroundColor = Color.White,
                            TextColor = Color.FromHex("#333"),
                            VerticalTextAlignment = TextAlignment.Center,
                            HorizontalTextAlignment = TextAlignment.Center,
                            FontAttributes = FontAttributes.Bold,
                            FontSize = 15,
                            Margin = new Thickness(2, -4, 0, 0)
                        };
                        Label Lab_Worker = new Label
                        {
                            BackgroundColor = Color.White,
                            TextColor = Color.FromHex("#333"),
                            VerticalTextAlignment = TextAlignment.Center,
                            HorizontalTextAlignment = TextAlignment.Center,
                            FontAttributes = FontAttributes.Bold,
                            FontSize = 15,
                            Margin = new Thickness(-4, -4, 0, 0),
                            Padding = new Thickness(5, 0, 5, 0)
                        };
                        Label Lab_TimeStart = new Label
                        {
                            BackgroundColor = Color.White,
                            TextColor = Color.FromHex("#333"),
                            VerticalTextAlignment = TextAlignment.Center,
                            HorizontalTextAlignment = TextAlignment.Center,
                            FontAttributes = FontAttributes.Bold,
                            FontSize = 15,
                            Margin = new Thickness(-4, -4, 0, 0)
                        };
                        Label Lab_TimeEnd = new Label
                        {
                            BackgroundColor = Color.White,
                            TextColor = Color.FromHex("#333"),
                            VerticalTextAlignment = TextAlignment.Center,
                            HorizontalTextAlignment = TextAlignment.Center,
                            FontAttributes = FontAttributes.Bold,
                            FontSize = 15,
                            Margin = new Thickness(-4, -4, 2, 0)
                        };

                        Lab_Job.Text = Data[0] == ""? "-" : Data[0];
                        Lab_Worker.Text = Data[1] == "" ? "-" : Data[1];
                        Lab_TimeStart.Text = Data[2] == "" ? "-" : Data[2];
                        Lab_TimeEnd.Text = Data[3] == "" ? "-" : Data[3];

                        Grid_History.RowDefinitions.Add(HistoryRow);

                        Grid.SetRow(Lab_Job, CountRow);
                        Grid.SetRow(Lab_Worker, CountRow);
                        Grid.SetRow(Lab_TimeStart, CountRow);
                        Grid.SetRow(Lab_TimeEnd, CountRow++);

                        Grid.SetColumn(Lab_Job, 0);
                        Grid.SetColumn(Lab_Worker, 1);
                        Grid.SetColumn(Lab_TimeStart, 2);
                        Grid.SetColumn(Lab_TimeEnd, 3);

                        Grid_History.Children.Add(Lab_Job);
                        Grid_History.Children.Add(Lab_Worker);
                        Grid_History.Children.Add(Lab_TimeStart);
                        Grid_History.Children.Add(Lab_TimeEnd);
                    }

                    RowDefinition BVRow = new RowDefinition();
                    BoxView boxView = new BoxView
                    {
                        BackgroundColor = Color.White,
                        Margin = new Thickness(2, -4, 2, 2)
                    };

                    Grid_History.RowDefinitions.Add(BVRow);
                    Grid.SetRow(boxView, CountRow);
                    Grid.SetColumnSpan(boxView, 4);
                    Grid_History.Children.Add(boxView);
                }
                else
                {
                    RowDefinition BVRow = new RowDefinition();
                    BoxView boxView = new BoxView
                    {
                        BackgroundColor = Color.White,
                        Margin = new Thickness(2, -4, 2, 2)
                    };
                    Grid_History.RowDefinitions.Add(BVRow);
                    Grid.SetRow(boxView, CountRow);
                    Grid.SetColumnSpan(boxView, 4);
                    Grid_History.Children.Add(boxView);
                }
            }
            else
            {

            }
            
        }

        private void Btn_Esc_Clicked(object sender, EventArgs e) => Navigation.PopAsync();
        protected override bool OnBackButtonPressed() => true;
    }
}