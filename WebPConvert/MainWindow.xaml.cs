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
                var btm = new MyBitmapImage(file, 100, 100);
                im.Source = btm.ResetReducedImage();
                lst.Children.Add(im);
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
                        p.StartInfo.Arguments = $"\"{file}\" -o \"{path}\\{i}.webp\" -q {(int)q.Value} -preset {(preset.SelectedItem as ComboBoxItem).Content.ToString()}";
                    }
                    p.Start();
                    //p.WaitForExit();
                }
            }
        }


    }
    public class MyBitmapImage
    {
        public int DecodeWidth { set; get; }
        public int DecodeHeight { set; get; }
        public string UriSource;
        public BitmapImage ReducedImage
        {
            get
            {
                return ResetReducedImage();
            }
            set
            {
                ReducedImage = value;
            }
        }
        public MyBitmapImage(string uri, int w, int h)
        {
            DecodeWidth = w;
            DecodeHeight = h;
            UriSource = uri;
        }
        public BitmapImage ResetReducedImage()
        {
            #region 
            BitmapImage _reducedImage = null;
            MemoryStream mem;
            // Only load thumbnails
            byte[] buffer = File.ReadAllBytes(UriSource);
            mem = new MemoryStream(buffer);
            _reducedImage = new BitmapImage();
            _reducedImage.BeginInit();
            _reducedImage.CacheOption = BitmapCacheOption.None;
            _reducedImage.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
            _reducedImage.DecodePixelWidth = DecodeWidth;
            _reducedImage.DecodePixelHeight = DecodeHeight;
            _reducedImage.StreamSource = mem;
            _reducedImage.Rotation = Rotation.Rotate0;
            _reducedImage.EndInit();
            buffer = null;
            return _reducedImage;
            #endregion
        }
    }
}
