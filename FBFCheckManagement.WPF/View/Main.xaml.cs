using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FBFCheckManagement.Application.Domain;
using FBFCheckManagement.Application.DTO;
using FBFCheckManagement.Application.Report;
using FBFCheckManagement.Application.Repository;
using FBFCheckManagement.Infrastructure.EntityFramework;
using FBFCheckManagement.Infrastructure.Repository;
using FBFCheckManagement.WPF.HelperClass;
using FBFCheckManagement.WPF.Report;

namespace FBFCheckManagement.WPF.View
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main
    {
        private readonly string _imageDirectory;
        private readonly IDatabaseType _dbType;

        public Main(){
            InitializeComponent();
            _imageDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) +
                              ConfigurationManager.AppSettings["systemimagedirectory"];

            _dbType = new EfSQLite("SQLiteDb");
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e){
            AddImageToButtons();
        }

        private void AddImageToButtons(){
            CheckImage.Source = ImageToBitmap.ConvertToBitmapImage(System.Drawing.Image.FromFile(_imageDirectory + "BankCheck.png"));
            BankImage.Source = ImageToBitmap.ConvertToBitmapImage(System.Drawing.Image.FromFile(_imageDirectory + "Bank.png"));
            CheckAmountsImage.Source = ImageToBitmap.ConvertToBitmapImage(System.Drawing.Image.FromFile(_imageDirectory + "schedule.png"));
        }

        private void CheckButton_Click(object sender, RoutedEventArgs e){
            IBankRepository bankRepository = new BankRepository(_dbType);
            ICheckRepository checkRepository = new CheckRepository(_dbType);

            CheckMaintenance m = new CheckMaintenance(bankRepository, checkRepository);
            m.ShowDialog();
        }

        private void BankButton_Click(object sender, RoutedEventArgs e)
        {
            IBankRepository bankRepository = new BankRepository(_dbType);

            BankMaintenance b = new BankMaintenance(bankRepository);
            b.ShowDialog();
        }

        private void CheckAmountsButton_Click(object sender, RoutedEventArgs e)
        {
            ICheckRepository checkRepository = new CheckRepository(_dbType);
            IBankRepository bankRepository = new BankRepository(_dbType);

            MonthChecks m = new MonthChecks(checkRepository, bankRepository);
            m.ShowDialog();
        }

        private void ReportButton_OnClick(object sender, RoutedEventArgs e){
            IDatabaseType type = new EfSQLite("SQLiteDb");
            IBankRepository bankRep = new BankRepository(type);
            ICheckRepository checkRep = new CheckRepository(type);

            ReportParameter param = new ReportParameter();
            param.Type = ReportType.Weekly;
            param.Day = new DateTime(2017, 11, 20);
            param.ShouldFilterByBank = false;
            param.BankId = 1;
            param.From = new DateTime(2017, 11, 19);
            param.To = new DateTime(2017, 11, 25);

            ReportGenerator gen = new ReportGenerator(checkRep, bankRep);
            WeekReportModel weekly = gen.GetWeekly(param);

            ReportExporter reporter = new ReportExporter();
            reporter.ExportWeeklyReport(weekly, @"C:\Users\Ruby\Desktop\NoFlag.xlsx");
        }
    }
}
