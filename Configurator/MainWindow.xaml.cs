using SimulinkIEC104;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace Configurator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private static string _configFileName = "database.xml";
        private static Settings _settings;
        public MainWindow()
        {
            InitializeComponent();

            try
            {
                XmlSerializer formatter = new XmlSerializer(typeof(Settings));
                using (FileStream fs = new FileStream(_configFileName, FileMode.Open))
                {
                    _settings = (Settings)formatter.Deserialize(fs);
                }

                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка десериализации "+ ex.Message);
            }
        }
    }
}
