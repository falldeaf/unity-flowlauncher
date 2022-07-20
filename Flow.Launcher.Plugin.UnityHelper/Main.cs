using System;
using System.Collections.Generic;
using System.Text.Json;
using fuzzy.match;
using System.Linq;
using System.Threading.Tasks;
using Control = System.Windows.Controls.Control;

namespace Flow.Launcher.Plugin.UnityHelper {
    public class UnityHelper : IPlugin, ISettingProvider, IContextMenu //IPluginI18n 
    {
        internal static PluginInitContext _context;
        internal static Settings _settings;
        private static string _current_cache;
        private bool isNewIndexOfProjectsRequired => _settings.last_index_time.AddDays(7) < DateTime.Today;

        private string test_json_string = @"
		[
            {
                ""Version"":  {
                                ""Major"":  2020,
                                ""Minor"":  3,
                                ""Revision"":  4,
                                ""Release"":  ""f"",
                                ""Build"":  1,
                                ""Suffix"":  null
                            },
                ""Path"":  ""C:\\project\\git\\NetworkSim2\\"",
                ""ProductName"":  ""NetworkSim+""
            },
            {
                ""Version"":  {
                    ""Major"":  2019,
                                    ""Minor"":  3,
                                    ""Revision"":  15,
                                    ""Release"":  ""f"",
                                    ""Build"":  1,
                                    ""Suffix"":  null
                                },
                ""Path"":  ""C:\\project\\git\\New Unity Project\\"",
                ""ProductName"":  ""New Unity Project""
            },
            {
                ""Version"":  {
                    ""Major"":  2019,
                                    ""Minor"":  3,
                                    ""Revision"":  15,
                                    ""Release"":  ""f"",
                                    ""Build"":  1,
                                    ""Suffix"":  null
                                },
                ""Path"":  ""C:\\project\\git\\ParticleTest\\"",
                ""ProductName"":  ""ParticleTEST""
            },
            {
                ""Version"":  {
                    ""Major"":  2019,
                                    ""Minor"":  3,
                                    ""Revision"":  14,
                                    ""Release"":  ""f"",
                                    ""Build"":  1,
                                    ""Suffix"":  null
                                },
                ""Path"":  ""C:\\project\\git\\sctesim\\"",
                ""ProductName"":  ""SCTE Training""
            },
            {
                ""Version"":  {
                    ""Major"":  2019,
                                    ""Minor"":  3,
                                    ""Revision"":  15,
                                    ""Release"":  ""f"",
                                    ""Build"":  1,
                                    ""Suffix"":  null
                                },
                ""Path"":  ""C:\\project\\git\\SCTE_3D_SIMS\\"",
                ""ProductName"":  ""3D_SIM_SCTE""
            },
            {
                ""Version"":  {
                    ""Major"":  2019,
                    ""Minor"":  3,
                    ""Revision"":  15,
                    ""Release"":  ""f"",
                    ""Build"":  1,
                    ""Suffix"":  null
                },
                ""Path"":  ""C:\\project\\git\\WiFi Radiation Pattern\\"",
                ""ProductName"":  ""WiFi Radiation Pattern""
            }
        ]
		";

		public void Init(PluginInitContext context) {
			_context = context;
            _settings = context.API.LoadSettingJsonStorage<Settings>();
            _current_cache = _settings.cached_project_json;
        }

        private static string getProjectsCmd() {
            string project_path = string.IsNullOrEmpty(_settings.project_path) ? "c:\\" : _settings.project_path;
            return $"$warningPreference = 'SilentlyContinue'; Get-UnityProjectInstance -BasePath {project_path} -Recurse -ErrorAction SilentlyContinue | ConvertTo-Json";
        }

		public List<Result> Query(Query query) {
			var results = new List<Result>();

            JsonDocument result_json;
            if (string.IsNullOrEmpty(_current_cache) || isNewIndexOfProjectsRequired) {
                //var result_string = RunCmd(getProjectsCmd(), true);
                //result_json = JsonDocument.Parse(result_string);
                //_settings.cached_project_json = result_string;
                result_json = JsonDocument.Parse(IndexProjects());
            } else {
                result_json = JsonDocument.Parse(_current_cache);
            }

            //var result_json = JsonDocument.Parse(test_json_string);

            foreach (var item in result_json.RootElement.EnumerateArray()) {

                string name = item.GetProperty("ProductName").ToString();
                string upath = item.GetProperty("Path").ToString();
                string version = item.GetProperty("Version").GetProperty("Major").ToString() + "." +
                                    item.GetProperty("Version").GetProperty("Minor").ToString() + "." +
                                    item.GetProperty("Version").GetProperty("Revision").ToString() + "." +
                                    item.GetProperty("Version").GetProperty("Release").ToString() + "." +
                                    item.GetProperty("Version").GetProperty("Build").ToString();

                Result result = new Result {
                    Title = name + " : " + version,
                    SubTitle = upath,
                    IcoPath = "Images/unitylogo.png",
                    ContextData = item.GetProperty("Path").ToString(),
                    Action = _ => {
                        Task.Run(() => RunCmd($"Start-UnityEditor -Project {upath}", false));

                        return true;
                    }

                };
                results.Add(result);

            }

            if (string.IsNullOrEmpty(query.Search)) {
                results = results.OrderBy(result => result.Title).ToList();
            } else {
                results = results.OrderByDescending(result => FuzzyMatcher.FuzzyMatch(result.Title.ToString().ToLower(), query.Search, out int out1)).ToList();
            }

            return results;
        }

        public Control CreateSettingPanel() {
            return new UnitySettings(_settings);
        }

        public List<Result> LoadContextMenus(Result selected_result) {
            var resultlist = new List<Result> {
                new Result {
                    Title = "Reveal path in explorer",
                    SubTitle = selected_result.SubTitle,
                    Action = _ => {
                        Task.Run(() => RunCmd("Start " + selected_result.SubTitle + "Assets\\", false));
                        return true;
                    },
                    IcoPath = "Images/folder.png"
                },
                new Result {
                    Title = "Open project in Visual Studio Code (Code .)",
                    SubTitle = selected_result.SubTitle,
                    Action = _ => {
                        Task.Run(() => RunCmd("Code " + selected_result.SubTitle, false));
                        return true;
                    },
                    IcoPath = "Images/code.png"
                },
                new Result {
                    Title = "Open project in GitHub Desktop App (github .)",
                    SubTitle = selected_result.SubTitle,
                    Action = _ => {
                        Task.Run(() => RunCmd("github " + selected_result.SubTitle, false));
                        return true;
                    },
                    IcoPath = "Images/git.png"
                },
                new Result {
                    Title = "Open project in Unity3D",
                    SubTitle = selected_result.SubTitle,
                    Action = _ => {
                        Task.Run(() => RunCmd($"Start-UnityEditor -Project {selected_result.SubTitle}", false));
                        return true;
                    },
                    IcoPath = "Images/editor.png"
                }
            };

            return resultlist;
        }

        private static string RunCmd(string str_command, bool wait) {
            string str_output = "";
            System.Diagnostics.Process pProcess = new();
            pProcess.StartInfo.FileName = "powershell.exe";
            pProcess.StartInfo.Arguments = str_command;
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.CreateNoWindow = true;
            pProcess.Start();
            if(wait) {
                str_output = pProcess.StandardOutput.ReadToEnd();
                pProcess.WaitForExit();
            }

            return str_output;
        }

        public static string IndexProjects() {
            //Run powershell script and get results in JSON
            var result_string = RunCmd(getProjectsCmd(), true);

            //If there's only one project, the returned json will be an object and it always needs to be an array
            // -AsArray would handle it but that's only available in Powershell 6
            if (result_string.TrimStart().StartsWith("{")) {
                //This is kind of a cludge... Maybe better to check for object or array on returned object?
                //I probably also need to try/catch this parse and pass an empty array if the json fails, and maybe log it
                //in flowlaunchers logfile?
                result_string = "[" + result_string + "\n]";
            }

            _settings.cached_project_json = result_string;
            _current_cache = result_string;
            _settings.last_index_time = DateTime.Today;

            return result_string;
        }

        public static async Task IndexProjectsAsync() {
            //var t1 = Task.Run(IndexWin32Programs);
            //var t2 = Task.Run(IndexUwpPrograms);
            //await Task.WhenAll(t1).ConfigureAwait(false);
            //ResetCache();
            await Task.Run(() =>
            {
                var result_string = RunCmd(getProjectsCmd(), true);

                if (result_string.TrimStart().StartsWith("{")) {
                    result_string = "[" + result_string + "\n]";
                }

                _settings.cached_project_json = result_string;
                _current_cache = result_string;
                _settings.last_index_time = DateTime.Today;

            }).ConfigureAwait(true);

        }
    }
}