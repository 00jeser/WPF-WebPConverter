using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WebPConvert
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OpenFileDialog dlg;

        public MainWindow()
        {
            InitializeComponent();
            dlg = new OpenFileDialog
            {
                Multiselect = true,
                Title = "Выберите файлы",
                Filter = "png|*.png|jpg|*.jpg"
            };
        }


        private void preset_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var a = (preset.SelectedItem as ComboBoxItem).Content;
            if (a.ToString() == "")
            {
                m.IsEnabled = true;
                z.IsEnabled = true;
                alpha_q.IsEnabled = true;
            }
            else
            {
                m.IsEnabled = false;
                z.IsEnabled = false;
                alpha_q.IsEnabled = false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            dlg.ShowDialog();

            if (dlg.FileName == String.Empty)
                return;

            (sender as Button).Visibility = Visibility.Hidden;
            exp.IsEnabled = true;
            foreach (string file in dlg.FileNames)
            {
                var im = new Image();
                im.Height = 100;
                im.Source = new BitmapImage(new Uri(file, UriKind.Absolute));
                lst.Items.Add(im);
            }
        }

        private void exp_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (dialog.ShowDialog(this).GetValueOrDefault())
            {
                var path = dialog.SelectedPath;
                int i = -1;
                foreach (string file in dlg.FileNames)
                {
                    i++;
                    System.Diagnostics.Process p = new System.Diagnostics.Process();
                    p.StartInfo.FileName = "cwebp.exe";
                    var tmp = $"\"{file}\" -o \"{path}\\{i}.webp\" -q {(int)q.Value} -z {(int)z.Value} -alpha_q {(int)alpha_q.Value} -m {(int)m.Value}";
                    if ((preset.SelectedItem as ComboBoxItem).Content.ToString() == "")
                    {
                        p.StartInfo.Arguments = $"\"{file}\" -o \"{path}\\{i}.webp\" -q {(int)q.Value} -z {(int)z.Value} -alpha_q {(int)alpha_q.Value} -m {(int)m.Value}";
                    }
                    else
                    {
                        p.StartInfo.Arguments = $"-q {(int)q.Value} -preset {(preset.SelectedItem as ComboBoxItem).Content.ToString()}";
                    }
                    p.Start();
                    //p.WaitForExit();
                }
            }
        }
    }
}
