Unity3D Project lister and launcher
==================
A plugin for the [Flow launcher](https://github.com/Flow-Launcher/Flow.Launcher) that let's you traverse through and open your Unity projects with the correct editor version. The context menu will also allow you to view the project in VSCode or open the projects Assets folder in explorer.

<img src="flowlauncher_unityhelper_preview.gif" alt="Preview gif of Unity Helper plugin" style="display: block; margin: 0 auto" />

Icons from [Unity branding page](https://brandguide.brandfolder.com/unity/downloadbrandassets)  and [Freepik - FlatIcon](https://www.flaticon.com/free-icons/coding).

## Installation
1. This plugin relies on the [Unity.powershell Module](https://github.com/microsoft/unitysetup.powershell). Go to the github page for [installation instructions](https://github.com/microsoft/unitysetup.powershell#installation), and install that module, first.
2. Then, place the contents of the release folder into %appdata%/FlowLauncher/Plugins folder and restart FlowLauncher.
3. In the plugin settings, click the 'Unity Project Folder' button and navigate to your Unity projects folder and click 'open'. Wait for the project folder to be indexed.

## Usage
    un <fuzzy search through your projects>
Shift+Enter on a project listing will let you open your project in Explorer, VSCode, or GitHub
