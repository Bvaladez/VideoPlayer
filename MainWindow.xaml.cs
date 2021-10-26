using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.IO;
using System.Diagnostics;
using System.Text;
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
		private bool mediaPlayerIsPlaying = false;
		private bool userIsDraggingSlider = false;
		private string mFileName = null;
		private string mBlkFileName = null;
		private List<string> mBlkTimes;


		public MainWindow()
		{
			InitializeComponent();
			DispatcherTimer timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromSeconds(1);
			timer.Tick += timerTick;
			timer.Start();
		}


		private void timerTick(object sender, EventArgs e)
		{
			if ((Media.Source != null)
				&& (Media.NaturalDuration.HasTimeSpan)
				&& (true//needs to be sliderbar is not sliding 
				)) {

				SliderPrgBar.Minimum = 0;
				SliderPrgBar.Maximum = Media.NaturalDuration.TimeSpan.TotalSeconds;
				SliderPrgBar.Value = Media.Position.TotalSeconds;
			}
		}

		private void LoadButton_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog();
			dialog.Title = "Choose Media";
			if (dialog.ShowDialog() == true)
			{
				Media.Source = new Uri(dialog.FileName);
				string FileName = getFileNameFromPath(dialog.FileName);
				FileTextDisplay.Text = FileName;
				string BlkFileName = getBlkFileName(FileName);
				// we dont know if a blk file exists yet update it when we are certain 
				BlockFileTextDisplay.Text = "";
				mFileName = FileName;
				mBlkFileName = BlkFileName;
				FileStream fin = openBlkFile(BlkFileName, createBlkFilePath(BlkFileName, dialog.FileName));
				// blk file was found in same dir as media
				if (fin != null)
				{
					byte[] buf = new byte[1024];
					int c;
					// Read file 1 line at a time 
					while ((c = fin.Read(buf, 0, buf.Length)) > 0)
					{
						Debug.WriteLine(Encoding.UTF8.GetString(buf, 0, c));
					}
				}
			}

		}

		private string getFileNameFromPath(string path)
		{
			int startFileIdx = 0;
			int extensionIdx = path.IndexOf(".");
			for (int i = extensionIdx; i > 0; i--)
			{
				if (path[i] == '\\') {
					startFileIdx = i + 1;
					break;
				}
			}
			return path.Substring(startFileIdx, path.Length - startFileIdx);
		}

		private string getPathFromFile(string path)
		{
			int startFileIdx = 0;
			int extensionIdx = path.IndexOf(".");
			for (int i = extensionIdx; i > 0; i--)
			{
				if (path[i] == '\\') {
					startFileIdx = i + 1;
					break;
				}
			}
			return path.Substring(0, path.Length - getFileNameFromPath(path).Length );
		}

		private string getBlkFileName(string FileName)
		{
			int extensionIdx = FileName.IndexOf(".");
			string blkFile = "";
			for (int i = 0; i < FileName.Length; i++)
			{
				if (i == extensionIdx)
				{
					break;
				}
				blkFile += FileName[i];
			}
			blkFile += ".blk";
			return blkFile;
		}
		// Returns path to blk file 

		private string createBlkFilePath(string FileName, string Path)
		{
			string blkFileName = getBlkFileName(FileName);
			string pathToDir = getPathFromFile(Path);
			string blkFilePath = pathToDir + blkFileName;
			return blkFilePath;
		}

		// Safely open a blk file contained in the same dir as media	
		private FileStream openBlkFile(string FileName, string path)
		{
			string blkFilePath = createBlkFilePath(FileName, path);
			if (File.Exists(blkFilePath))
			{
				BlockFileTextDisplay.Text = mBlkFileName;
				FileStream fin = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
				return fin;
			}
			return null;
		}

		private void PlayButton_Click(object sender, RoutedEventArgs e)
		{
			if (Media.Source != null && mediaPlayerIsPlaying == false){
				Media.Play();
				mediaPlayerIsPlaying = true;
				PlayPauseButton.Content = "pause";
			}
			else
			{
				Media.Pause();
				mediaPlayerIsPlaying = false;
				PlayPauseButton.Content = "Play";
			}

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
		private void Speed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			Media.SpeedRatio = SpeedSlider.Value;
		}
		
		private void SliderPrgBar_DragStarted(object sender, DragStartedEventArgs e)
		{
			userIsDraggingSlider = true;
		}
		private void SliderPrgBar_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			userIsDraggingSlider = false;
			Media.Position = TimeSpan.FromSeconds(SliderPrgBar.Value);
		}
 
		private void SliderPrgBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			TimeDisplay.Text = TimeSpan.FromSeconds(SliderPrgBar.Value).ToString(@"hh\:mm\:ss");
		}

	}
}
