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
        FormUdp formUdpSend;
        FormUdp formUdpReceive;
        Dictionary<string, Dictionary<string, string>> _columnsDictionary = new Dictionary<string, Dictionary<string, string>>
        {
            { "_dgSendUDPparams", new Dictionary<string, string> {{ "ID", "ID" }, { "Name", "Название" },{ "DataType", "Тип" }}},
            { "_dgReceiveUDPparams", new Dictionary<string, string> {{ "ID", "ID" }, { "Name", "Название" },{ "DataType", "Тип" }}},
            { "_dgReceiveIEC104params", new Dictionary<string, string> {{ "UDPparameterIDs", "Параметры Simulink" }, { "IOA", "Адрес 104" }}},
            { "_dgSendIEC104params", new Dictionary<string, string> {{ "UDPParameterID", "Параметр Simulink" }, { "IOA", "Адрес 104" }}}
        };


        private static string _configFileName = "settings.xml";
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

                foreach (var dest in _settings.IEC104Destinations)
                {
                    foreach (var ca in dest.CommonAdreses)
                    {
                        ca.SetDestination(dest);
                        foreach (var send in ca.SendIOAs)
                        {
                            send.SetCA(ca);
                        }

                        foreach (var recieve in ca.ReceiveIOAs)
                        {
                            recieve.SetCA(ca);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка десериализации " + ex.Message);
            }

            DataContext= _settings;
             
        }

        private void _save_Button_Click(object sender, RoutedEventArgs e)
        {

            XmlSerializer formatter = new XmlSerializer(typeof(Settings));

            try
            {
                using (FileStream fs = new FileStream(_configFileName, FileMode.Create))
                {
                    formatter.Serialize(fs, _settings);
                }
                MessageBox.Show("Сохранено!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message);
            }
        }

        private void _addUDPDestination_Button_Click(object sender, RoutedEventArgs e)
        {
            _settings.UDPDestinations.Add(new Destination("Новое"));
        }

        private void _deleteUDPDestination_Button_Click(object sender, RoutedEventArgs e)
        {
            _settings.UDPDestinations.Remove((Destination)_lbUDPDestinations.SelectedItem);
        }

        private void _addIEC104DestinationButton_Click(object sender, RoutedEventArgs e)
        {            
            var result = MessageBox.Show("Создать клиент? Да - создастся клиент, Нет - создастся сервер", "Клиент или сервер МЭК104?", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Cancel) return;

            IEC104Destination newDest;
            if (result == MessageBoxResult.Yes)
            {
                newDest = new IEC104Connection("Новый клиент");
            } 
            else
            {
                newDest = new IEC104Server("Новый сервер");
            }

            _settings.IEC104Destinations.Add(newDest);
            _cbIEC104Destinations.SelectedItem = newDest;
        }

        private void _deleteIEC104DestinationButton_Click(object sender, RoutedEventArgs e)
        {
            _settings.IEC104Destinations.Remove((IEC104Destination)_cbIEC104Destinations.SelectedItem);
        }



        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            _settings.IEC104Destinations.ResetBindings();
            int index = _cbIEC104Destinations.SelectedIndex;
            _cbIEC104Destinations.SelectedIndex = -1;
            _cbIEC104Destinations.SelectedIndex = index;
        }



        private void _cbIEC104Destinations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_cbIEC104Destinations.SelectedItem == null) return;

            if (_cbIEC104Destinations.SelectedItem.GetType() == typeof(IEC104Connection))
            {
                _tbIPadsress.Visibility = Visibility.Visible;
                _lIPaddress.Visibility = Visibility.Visible;
            }
            else
            {
                _tbIPadsress.Visibility = Visibility.Hidden;
                _lIPaddress.Visibility = Visibility.Hidden;
            }
        }

        private void _addCAButton_Click(object sender, RoutedEventArgs e)
        {
            var ca = new IEC104CommonAddress("Новый CA");
            ca.SetDestination((IEC104Destination)_cbIEC104Destinations.SelectedItem);

            ((IEC104Destination)_cbIEC104Destinations.SelectedItem).CommonAdreses.Add(ca);

        }

        private void _removeCAButton_Click(object sender, RoutedEventArgs e)
        {
            var param = (IEC104CommonAddress)_lbIEC104CommonAddresses.SelectedItem;

            ((IEC104Destination)_cbIEC104Destinations.SelectedItem).CommonAdreses.Remove(param);
            param.DeleteCa();

            foreach (var send in param.SendIOAs)
            {
                send.DeleteIOA();
                send.ClearUDPParameter();

            }
            foreach (var recieve in param.ReceiveIOAs)
            {

                recieve.DeleteIOA();
                recieve.ClearUDPparameters();

            }

        }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (_columnsDictionary.ContainsKey( ((Control)sender).Name ))
            {
                if (_columnsDictionary[((Control)sender).Name].ContainsKey(e.PropertyName))
                {
                    e.Column.Header = _columnsDictionary[((Control)sender).Name][e.PropertyName];
                   
                }
                else
                {
                    e.Cancel = true;
                }
            }
            
        }

        private void _dgSendIEC104params_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void _dgSendIEC104params_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            /*if (e.Column.)
            MessageBox.Show(e.Row.Item.GetType().ToString());*/
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }


        private void _tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_UDPTabs.SelectedIndex != _IEC104Tabs.SelectedIndex)
                if (((TabControl)sender).Name == "_IEC104Tabs")
                    _UDPTabs.SelectedIndex = _IEC104Tabs.SelectedIndex;
                else
                    _IEC104Tabs.SelectedIndex = _UDPTabs.SelectedIndex;
        }

        private void editIDsButton_Click(object sender, RoutedEventArgs e)
        {
            if (formUdpSend != null && !formUdpSend.IsDisposed)
            {
                formUdpSend.Focus();
                return;
            }
            formUdpSend = new FormUdp(_settings.UDPDestinations, (IEC104ReceiveParameter)((Button)sender).Tag );
            formUdpSend.Show();

        }

        private void editIDButtonButton_Click(object sender, RoutedEventArgs e)
        {
            if (formUdpReceive != null && !formUdpReceive.IsDisposed)
            {
                formUdpReceive.Focus();
                return;
            }
            formUdpReceive = new FormUdp(_settings.UDPDestinations, (IEC104SendParameter)((Button)sender).Tag);
            formUdpReceive.Show();
        }

        private void addIEC104ReceiveParameterButton_Click(object sender, RoutedEventArgs e)
        {
            var recieveP = new IEC104ReceiveParameter();
            recieveP.SetCA((IEC104CommonAddress)_lbIEC104CommonAddresses.SelectedItem);

            ((IEC104CommonAddress)_lbIEC104CommonAddresses.SelectedItem).ReceiveIOAs.Add(recieveP);
        }

        private void deleteIEC104ReceiveParameterButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = _dgReceiveIEC104params.SelectedItems;
            Array arr = new object[selected.Count];
            selected.CopyTo(arr,0);
            foreach (var item in arr)
            {
                ((IEC104CommonAddress)_lbIEC104CommonAddresses.SelectedItem).ReceiveIOAs.Remove((IEC104ReceiveParameter)item);
                ((IEC104ReceiveParameter)item).ClearUDPparameters();
                ((IEC104Parameter)item).DeleteIOA();
            }
        }



        private void addIEC104SendParameterButton_Click(object sender, RoutedEventArgs e)
        {
            var sendP = new IEC104SendParameter();
            sendP.SetCA((IEC104CommonAddress)_lbIEC104CommonAddresses.SelectedItem);
            ((IEC104CommonAddress)_lbIEC104CommonAddresses.SelectedItem).SendIOAs.Add(sendP);
        }

        

        private void deleteIEC104SendParameterButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = _dgSendIEC104params.SelectedItems;
            Array arr = new object[selected.Count];
            selected.CopyTo(arr, 0);
            foreach (var item in arr)
            {
                ((IEC104CommonAddress)_lbIEC104CommonAddresses.SelectedItem).SendIOAs.Remove((IEC104SendParameter)item);
                ((IEC104SendParameter)item).ClearUDPParameter();
                ((IEC104Parameter)item).DeleteIOA();
            }
        }

        private void addUDPSendingParameterButton_Click(object sender, RoutedEventArgs e)
        {
            ((Destination)_lbUDPDestinations.SelectedItem).SendingParameters.Add(new SendingParameter("Новый передаваемый", DataTypeEnum.Int32));
        }

        private void deleteUDPSendingParameterButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = _dgSendUDPparams.SelectedItems;
            Array arr = new object[selected.Count];
            selected.CopyTo(arr, 0);
            foreach (var item in arr)
            {
                ((Destination)_lbUDPDestinations.SelectedItem).SendingParameters.Remove((SendingParameter)item);
                ((SendingParameter)item).ClearSourceParameter();
                ParameterUniqueID.DeleteParameter((Parameter)item);
            }
        }

        private void addUDPReceivingParameterButton_Click(object sender, RoutedEventArgs e)
        {
            ((Destination)_lbUDPDestinations.SelectedItem).ReceivingParameters.Add(new ReceivingParameter("Новый получаемый", DataTypeEnum.Int32));
        }

        private void deleteUDPReceivingParameterButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = _dgReceiveUDPparams.SelectedItems;
            Array arr = new object[selected.Count];
            selected.CopyTo(arr, 0);
            foreach (var item in arr)
            {
                ((Destination)_lbUDPDestinations.SelectedItem).ReceivingParameters.Remove((ReceivingParameter)item);

                ((ReceivingParameter)item).ClearSourceParameter();
                ParameterUniqueID.DeleteParameter((Parameter)item);
            }
        }

       
    }
}
