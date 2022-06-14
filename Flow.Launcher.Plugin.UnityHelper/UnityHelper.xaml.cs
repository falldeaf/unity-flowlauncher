using Microsoft.Win32;
//using System;
using System.Windows;
using System.Windows.Controls;
using System.IO;

namespace Flow.Launcher.Plugin.UnityHelper {
	public partial class UnitySettings : UserControl {

		private readonly Settings _settings;

		public UnitySettings(Settings settings) {
			InitializeComponent();
			_settings = settings;
			current_path.Content = string.IsNullOrEmpty(_settings.project_path) ? "c:\\" : _settings.project_path;
		}

		private void OpenProjectPath(object sender, RoutedEventArgs e) {
			OpenFileDialog openFileDlg = new OpenFileDialog();

			string to_remove = "Folder Selection";
			openFileDlg.ValidateNames = false;
			openFileDlg.CheckFileExists = false;
			openFileDlg.CheckPathExists = true;
			openFileDlg.FileName = to_remove;
			var result = openFileDlg.ShowDialog();
			if (result == true) {
				string folder = openFileDlg.FileName;
				folder = folder.Substring(0, folder.LastIndexOf(to_remove));
				this._settings.project_path = folder;
				current_path.Content = folder;
			}
		}

		private void ViewLoaded(object sender, RoutedEventArgs re) {
		}
	}
}