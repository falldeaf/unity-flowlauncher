using System;
using System.Collections.Generic;
using System.Text.Json;
using fuzzy.match;
using System.Linq;
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

    public class UnityHelper : IPlugin, ISettingProvider//, IPluginI18n, IContextMenu
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
            //var project_path = string.IsNullOrEmpty(_settings.project_path) ? "notepad.exe" : _settings.project_path;

            string project_path = "C:\\project\\git\\";
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
                    Title = name + " : " + out1.ToString(),
                    SubTitle = upath,
                    //IcoPath = "",
                    ContextData = item.GetProperty("Path").ToString(),
                    Action = _ =>
                    {
                        RunCmd($"Start-UnityEditor -Project {upath}", false);

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