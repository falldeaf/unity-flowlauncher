using System;
using System.Collections.Generic;
using System.Text.Json;
using fuzzy.match;
using System.Linq;
using System.Threading.Tasks;
using Control = System.Windows.Controls.Control;

namespace Flow.Launcher.Plugin.UnityHelper {
    class StringCompare : IComparer<Result> {
        public int Compare(Result x, Result y) {
            if (x == null || y == null) return 0;
            return x.Title.ToString().ToLower().CompareTo(y.Title.ToString().ToLower());
        }
    }

    class FuzzyCompare : IComparer<Result> {
        private string query = "";

        public FuzzyCompare(string query) {
            this.query = query;
        }
        public int Compare(Result x, Result y) {
            
            if (x == null || y == null) return 0;
            int out1;
            int out2;
            FuzzyMatcher.FuzzyMatch(x.Title.ToString().ToLower(), this.query, out out1);
            FuzzyMatcher.FuzzyMatch(y.Title.ToString().ToLower(), this.query, out out2);
            return out2.CompareTo(out1);
        }
    }

    public class UnityHelper : IPlugin, ISettingProvider, IContextMenu //IPluginI18n
    {
		private PluginInitContext _context;
        private Settings _settings;

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
                ""ProductName"":  ""ParticleTest""
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
        }

		public List<Result> Query(Query query) {
			var results = new List<Result>();
            string project_path = string.IsNullOrEmpty(_settings.project_path) ? "c:\\" : _settings.project_path;

            //string project_path = "C:\\project\\git\\";
            string str_cmd = $"Get-UnityProjectInstance -BasePath {project_path} -Recurse | ConvertTo-Json";
            //var result_json = JsonDocument.Parse(RunCmd(str_cmd, true));
            var result_json = JsonDocument.Parse(test_json_string);

            foreach (var item in result_json.RootElement.EnumerateArray()) {

                string name = item.GetProperty("ProductName").ToString();
                string upath = item.GetProperty("Path").ToString();

                int out1;
                FuzzyMatcher.FuzzyMatch(name, query.Search, out out1);
                Result result = new Result
                {
                    Title = name,
                    SubTitle = upath, // + " : " + project_path,
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
                //StringCompare sc = new();
                //results.Sort(sc);
                results = results.OrderBy(o => o.Title).ToList();
            } else {
                FuzzyCompare fc = new(query.Search);
                results.Sort(fc);
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
	}
}