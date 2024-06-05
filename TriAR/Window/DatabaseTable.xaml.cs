using TriAR.ViewModel;

namespace TriAR.Window
{
    /// <summary>
    /// Логика взаимодействия для DatabaseTable.xaml
    /// </summary>
    public partial class DatabaseTable : System.Windows.Window
    {
        public DatabaseTable()
        {
            InitializeComponent();
            DataContext = new DatabaseTableViewModel();

        }
    }
}
