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
		private string mMediaFileName = "";
		private string mMediaDirPath = null;
		private string mBlkFileName = null;
		private string mBlkFilePath = "";
		private List<string> mBlkStringTimes = new List<string>();
		private List<int> mBlkIntTimes = new List<int>();


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
				// we only apply time filters if they exist and there is a start AND stop time for each timeSpan
				if(mBlkIntTimes.Count != 0 && mBlkIntTimes.Count % 2 == 0)
				{
					applyTimeFilters();
				}
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
				BlockFileTextDisplay.Text = BlkFileName;

				mMediaFileName = FileName;
				mMediaDirPath = FileName;

				mBlkFileName = BlkFileName;
				mBlkFilePath = createBlkFilePath(BlkFileName, dialog.FileName);

				FileStream fin = openBlkFile(mBlkFilePath);
				if (fin != null)
				{
					// blk file was found in same dir as media
					captureBlkTimes(fin);
					convertBlkStrings();
				}
				else
				{
					writeBlkFile();
				}
				fin.Close();
			}

		}

		private void applyTimeFilters()
		{
			for (int t = 0; t < mBlkIntTimes.Count; t += 2)
			{
				double startTime = mBlkIntTimes[t];
				double endTime = mBlkIntTimes[t+1];
				if (Media.Position.TotalSeconds >= startTime && Media.Position.TotalSeconds <= endTime)
				{
					Media.Position = TimeSpan.FromSeconds(endTime);
				}
			}
		}

		private void captureBlkTimes(FileStream fin)
		{
			// Clear so if a new file is loaded we dont use the wrong blk times
			mBlkStringTimes.Clear();
			byte[] buf = new byte[1024];
			int c;
				// if the file has contents read line by line saving each time to mBlkTimes 
			while ((c = fin.Read(buf, 0, buf.Length)) > 0)
			{
				mBlkStringTimes.Add(Encoding.UTF8.GetString(buf, 0, c));
			}

		}

		private void convertBlkStrings()
		{
			// Clear so if a new file is loaded we dont use the wrong blk times
			mBlkIntTimes.Clear();
			// Format for string times is --> hh:mm:ss-hh:mm:ss \n
			for (int s = 0; s < mBlkStringTimes.Count; s++)
			{
				string timeString = mBlkStringTimes[s];
				char[] delimiterChars = { ':', ':', '-', ':', ':' };
				string[] words = timeString.Split(delimiterChars);
				// If the time input was formated wrong we dont want to try and convert it, UGLY.
				if(words.Length == 6)
				{
					int hs = Int32.Parse(words[0]) * 60 * 60;
					int ms = Int32.Parse(words[1]) * 60;
					int ss = Int32.Parse(words[2]);
					int StartTimeSeconds = hs + ms + ss;
					mBlkIntTimes.Add(StartTimeSeconds);

					int hf = Int32.Parse(words[3]) * 60 * 60;
					int mf = Int32.Parse(words[4]) * 60;
					int sf = Int32.Parse(words[5]);
					int EndTimeSeconds = hf + mf + sf;
					mBlkIntTimes.Add(EndTimeSeconds);

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
		private FileStream openBlkFile(string blkPath)
		{
			if (File.Exists(blkPath))
			{
				BlockFileTextDisplay.Text = mBlkFileName;
				FileStream fin = File.Open(mBlkFilePath, FileMode.Open, FileAccess.Read, FileShare.None);
				return fin;
			}
			BlockFileTextDisplay.Text = "NO FILE";
			return null;
		}

		private void writeBlkFile()
		{
    		File.Create(mBlkFilePath);
		}

		private void AddBlkTimes_Clicked(object sender, RoutedEventArgs e)
		{
			// We arnt checking to make sure both start time and end time are populated before adding the times, this could lead to problems from user error.
			if (File.Exists(mBlkFilePath))
			{
				if ((StartTimeInput.Text != "" && StartTimeInput.Text != "ERROR!" ) && (EndTimeInput.Text != "" && EndTimeInput.Text != "ERROR!"))
				{
					string s = "-";
	    			string nl = "\n";
        			FileStream fin = File.Open(mBlkFilePath, FileMode.Append, FileAccess.Write, FileShare.None);
		  			fin.Write(Encoding.UTF8.GetBytes(StartTimeInput.Text, 0, StartTimeInput.Text.Length));
					fin.Write(Encoding.UTF8.GetBytes(s, 0, s.Length));
        			fin.Write(Encoding.UTF8.GetBytes(EndTimeInput.Text, 0, EndTimeInput.Text.Length));
        			fin.Write(Encoding.UTF8.GetBytes(nl, 0, nl.Length));
        			fin.Close();
        			StartTimeInput.Text = "";
        			EndTimeInput.Text = "";
				}
				// Atleast one of the fields is not filled out
				// we can try to convert the times here and if it fails also throw an error so formatting is correct
				else
				{
					if (StartTimeInput.Text == "" )
					{
						StartTimeInput.Text = "ERROR!";
					}
					else if(EndTimeInput.Text == "" )
					{
						EndTimeInput.Text = "ERROR!";
					}
				}
		}
			else
			{
				// If we want to add blk times to a blk file but the blk file doesnt exist we can ask if they want to write the file 
				// temp solution Do nothing on nonexistent blk files.
			}

		}

		private void PlayButton_Click(object sender, RoutedEventArgs e)
		{
			if (Media.Source != null && mediaPlayerIsPlaying == false){
				Media.Play();
				mediaPlayerIsPlaying = true;
				PlayPauseButton.Content = "Pause";
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
			{
				Media.Stop();
				mediaPlayerIsPlaying = false;
			}
			if (PlayPauseButton.Content.ToString() == "Pause")
			{
				PlayPauseButton.Content = "Play";
			}
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
