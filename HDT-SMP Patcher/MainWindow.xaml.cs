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
using HDT_SMP_Patcher.config;
using Microsoft.Win32;

namespace HDT_SMP_Patcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<string, HDT_SMP_Patcher.config.Version> versions;
        public MainWindow()
        {
            InitializeComponent();

            Loaded += OnWindowLoaded;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (!System.IO.File.Exists("offsets/versions.json"))
            {
                MessageBox.Show("versions.json not found, please make sure the offsets directory has files in it");
                this.Close();
            }
            else
            {
                versions = JsonSerializer.Deserialize<Dictionary<string, HDT_SMP_Patcher.config.Version>>(System.IO.File.ReadAllText("offsets/versions.json"));

                foreach(var value in versions.Values)
                {
                    CbVersion.Items.Add(value.Name);
                }

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
                using FileStream stream = System.IO.File.OpenRead(fileDialog.FileName);
                var sha = new SHA256Managed();
                var hash = BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", String.Empty);
                bool valid = false;
                foreach (var entry in versions)
                {
                    if (entry.Value.Hashes["hdtSSEFramework.dll"].Equals(hash))
                    {
                        TbFrameworkFile.Text = fileDialog.FileName;
                        LbFrameworkReady.Content = $"{entry.Value.Name}";
                        LbFrameworkReady.Foreground = Brushes.Green;
                        valid = true;
                        break;
                    }
                }

                if (!valid)
                {
                    LbFrameworkReady.Content = "Unknown File";
                    LbFrameworkReady.Foreground = Brushes.Red;
                }
            }
        }

        private void PhysicsButton_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = "hdtSSEPhysics.dll|hdtSSEPhysics.dll|DLL files (*.dll)|*.dll|All files (*.*)|*.*"
            };
            if (fileDialog.ShowDialog() == true)
            {
                using FileStream stream = System.IO.File.OpenRead(fileDialog.FileName);
                var sha = new SHA256Managed();
                var hash = BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", String.Empty);
                bool valid = false;
                foreach (var entry in versions)
                {
                    if (entry.Value.Hashes["hdtSSEPhysics.dll"].Equals(hash))
                    {
                        TbPhysicsFile.Text = fileDialog.FileName;
                        LbPhysicsReady.Content = $"{entry.Value.Name}";
                        LbPhysicsReady.Foreground = Brushes.Green;
                        valid = true;
                        break;
                    }
                }

                if (!valid)
                {
                    LbPhysicsReady.Content = "Unknown File";
                    LbPhysicsReady.Foreground = Brushes.Red;
                }
            }
        }

        private void PatchButton_Click(object sender, RoutedEventArgs e)
        {
            if (CbVersion.SelectedItem == null)
            {
                MessageBox.Show("select a version to patch to");
                return;
            }

            if (!File.Exists($"offsets/{versions[CbVersion.SelectedItem.ToString()].OffsetFile}"))
            {
                MessageBox.Show("offset file not found for selected version");
                return;
            }

            var offsets = JsonSerializer.Deserialize<Dictionary<string, List<Offset>>>(System.IO.File.ReadAllText($"offsets/{versions[CbVersion.SelectedItem.ToString()].OffsetFile}"));

            var fileCount = 0;

            if (LbFrameworkReady.Foreground == Brushes.Green)
            {
                PatchFile("hdtSSEFramework.dll", TbFrameworkFile.Text, offsets["hdtSSEFramework.dll"], LbFrameworkReady.Content.ToString());
                fileCount++;
                LbFrameworkReady.Content = $"Patched {CbVersion.SelectedItem.ToString()}";
            }

            if (LbPhysicsReady.Foreground == Brushes.Green)
            {
                PatchFile("hdtSSEPhysics.dll", TbPhysicsFile.Text, offsets["hdtSSEPhysics.dll"], LbPhysicsReady.Content.ToString());
                fileCount++;
                LbPhysicsReady.Content = $"Patched {CbVersion.SelectedItem.ToString()}";
            }

            MessageBox.Show($"Patched {fileCount} files");
        }

        private void PatchFile(string name, string path, List<Offset> offsets, string version)
        {
            // create backup
            System.IO.File.Copy(path, path + $".backup_{version}", true);

            using (FileStream stream = File.OpenWrite(path))
            {
                foreach (var offset in offsets)
                {
                    stream.Seek(uint.Parse(offset.Address, System.Globalization.NumberStyles.AllowHexSpecifier), SeekOrigin.Begin);
                    stream.Write(BitConverter.GetBytes(uint.Parse(offset.Patch, System.Globalization.NumberStyles.AllowHexSpecifier)));
                }
            }
        }
    }
}
