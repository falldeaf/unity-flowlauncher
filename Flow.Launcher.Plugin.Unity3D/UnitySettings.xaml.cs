using System.Windows;
using System.Windows.Controls;
//using Flow.Launcher.Plugin.Unity3D;
using Microsoft.Win32;

namespace Flow.Launcher.Plugin.Unity3D
{
    public partial class UnitySettings : UserControl
    {
        public Settings settings { get; }


        public UnitySettings(Settings settings)
        {
            this.settings = settings;
        }

        private void View_Loaded(object sender, RoutedEventArgs re)
        {
            /*
            UseLocationAsWorkingDir.IsChecked = settings.UseLocationAsWorkingDir;

            UseLocationAsWorkingDir.Checked += (o, e) =>
            {
                settings.UseLocationAsWorkingDir = true;
            };

            UseLocationAsWorkingDir.Unchecked += (o, e) =>
            {
                settings.UseLocationAsWorkingDir = false;
            };

            LaunchHidden.IsChecked = settings.LaunchHidden;

            LaunchHidden.Checked += (o, e) =>
            {
                settings.LaunchHidden = true;
            };

            LaunchHidden.Unchecked += (o, e) =>
            {
                settings.LaunchHidden = false;
            };

            ShowWindowsContextMenu.IsChecked = settings.ShowWindowsContextMenu;

            ShowWindowsContextMenu.Checked += (o, e) =>
            {
	            settings.ShowWindowsContextMenu = true;
            };

            ShowWindowsContextMenu.Unchecked += (o, e) =>
            {
	            settings.ShowWindowsContextMenu = false;
            };*/

            //project_path.Content = settings.project_path;
        }

        private void ProjectPathClicked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Executable File(*.exe)| *.exe";
            if (!string.IsNullOrEmpty(settings.project_path))
                openFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(settings.project_path);

            if (openFileDialog.ShowDialog() == true)
            {
                settings.project_path = openFileDialog.FileName;
            }

            //project_path.Content = settings.project_path;
        }


        private void onSelectionChange(object sender, SelectionChangedEventArgs e)
        {
            /*
            // on load, tbFastSortWarning control will not have been loaded yet
            if (tbFastSortWarning is not null)
            {
                tbFastSortWarning.Visibility = vm.FastSortWarningVisibility;
                tbFastSortWarning.Text = vm.GetSortOptionWarningMessage;
            }
            */
        }
    }
}
