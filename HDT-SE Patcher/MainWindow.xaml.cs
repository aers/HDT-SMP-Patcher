using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
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
using HDT_SE_Patcher.config;
using Microsoft.Win32;

namespace HDT_SE_Patcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Offsets config;
        public MainWindow()
        {
            InitializeComponent();

            Loaded += OnWindowLoaded;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (!File.Exists("offsets.json"))
            {
                MessageBox.Show("offsets.json not found, please place it in the same directory as the app");
                this.Close();
            }
            else
            {
                config = JsonSerializer.Deserialize<Offsets>(File.ReadAllText("offsets.json"));

                Loaded -= OnWindowLoaded;
            }
        }

        private void FrameworkButton_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = "hdtSSEFramework.dll|hdtSSEFramework.dll|DLL files (*.dll)|*.dll|All files (*.*)|*.*"
            };
            if (fileDialog.ShowDialog() == true)
            {
                TbFrameworkFile.Text = fileDialog.FileName;
                using FileStream stream = File.OpenRead(fileDialog.FileName);
                var sha = new SHA256Managed();
                var hash = BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", String.Empty);
                bool valid = false;
                foreach (var entry in config.Files["hdtSSEFramework"].Versions)
                {
                    if (entry.Value.Equals(hash))
                    {
                        LbFrameworkReady.Content = $"Valid - {entry.Key}";
                        LbFrameworkReady.Foreground = Brushes.Green;
                        valid = true;
                        break;
                    }
                }

                if (!valid)
                {
                    LbFrameworkReady.Content = "Invalid File";
                    LbFrameworkReady.Foreground = Brushes.Red;
                }
            }
        }

        private void PhysicsButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void HighHeelsButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
