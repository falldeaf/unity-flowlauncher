using Microsoft.Win32;
//using System;
using System.Windows;
using System.Windows.Controls;

namespace Flow.Launcher.Plugin.UnityHelper {
	public partial class UnitySettings : UserControl {

		private readonly Settings settings;

		public UnitySettings(Settings settings) {
			InitializeComponent();
			settings = settings;
		}

		private void OpenProjectPath(object sender, RoutedEventArgs e) {
			OpenFileDialog openFileDlg = new OpenFileDialog();

			var result = openFileDlg.ShowDialog();
			if (result == true) {
				this.settings.project_path = openFileDlg.FileName;
			}
		}

		private void ViewLoaded(object sender, RoutedEventArgs re) {
		}
	}
}