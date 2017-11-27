using System.Configuration;
using System.Reflection;
using System.Windows;
using FBFCheckManagement.Application.Repository;
using FBFCheckManagement.Infrastructure.EntityFramework;
using FBFCheckManagement.Infrastructure.Repository;
using FBFCheckManagement.WPF.HelperClass;

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
            IDepartmentRepository deptRepository = new DepartmentRepository(_dbType);
            IBankRepository bankRepository = new BankRepository(_dbType);
            ICheckRepository checkRepository = new CheckRepository(_dbType);

            CheckMaintenance m = new CheckMaintenance(deptRepository, bankRepository, checkRepository);
            m.ShowDialog();
        }

        private void BankButton_Click(object sender, RoutedEventArgs e)
        {
            IBankRepository bankRepository = new BankRepository(_dbType);
            IDepartmentRepository deptRepository = new DepartmentRepository(_dbType);

            BankMaintenance b = new BankMaintenance(deptRepository, bankRepository);
            b.ShowDialog();
        }

        private void CheckAmountsButton_Click(object sender, RoutedEventArgs e)
        {
            ICheckRepository checkRepository = new CheckRepository(_dbType);
            IDepartmentRepository deptRepository = new DepartmentRepository(_dbType);
            IBankRepository bankRepository = new BankRepository(_dbType);

            MonthChecks m = new MonthChecks(checkRepository, deptRepository, bankRepository);
            m.ShowDialog();
        }
    }
}
