using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace MediaElementDemo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timerTick;
            timer.Start();
        }

        private double PrettyTime()
        {
            var seconds = Media.NaturalDuration.TimeSpan.TotalSeconds;
            return seconds;
        }

        private void timerTick(object sender, EventArgs e)
        {
            if ((Media.Source != null) 
                && (Media.NaturalDuration.HasTimeSpan)
                && (true//needs to be sliderbar is not sliding 
                )){

                SliderPrgBar.Minimum = 0;
                SliderPrgBar.Maximum = Media.NaturalDuration.TimeSpan.TotalSeconds;
                SliderPrgBar.Value = Media.Position.TotalSeconds;

                TimeDisplay.Text = PrettyTime().ToString("0.#####");

            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "Choose Media";
            if (dialog.ShowDialog() == true)
            {
                Media.Source = new Uri(dialog.FileName);
                //MediaName.Text = dialog.FileName;
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (Media.Source != null)
                Media.Play();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (Media.CanPause)
                Media.Pause();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (Media.Source != null)
                Media.Stop();
        }

        private void MuteButton_Click(object sender, RoutedEventArgs e)
        {
            Media.IsMuted = !Media.IsMuted;
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Media.Volume = VolumeSlider.Value;
        }

        private void Balance_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Media.Balance = BalanceSlider.Value;
        }

        private void Speed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Media.SpeedRatio = SpeedSlider.Value;
        }
    }
}
