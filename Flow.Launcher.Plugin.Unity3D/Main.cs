using System;
using System.Collections.Generic;
using Flow.Launcher.Plugin;
using System.Text.Json;

namespace Flow.Launcher.Plugin.Unity3D
{
	public class Unity3D : IPlugin
	{
		private PluginInitContext _context;

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
		}

		public List<Result> Query(Query query) {
			var results = new List<Result>();

            //var result_json = JsonDocument.Parse(test_json_string);
            string project_path = "C:\\project\\git\\";
            string str_cmd = $"Get-UnityProjectInstance -BasePath {project_path} -Recurse | ConvertTo-Json";
            var result_json = JsonDocument.Parse(RunCmd(str_cmd, true));

            foreach (var item in result_json.RootElement.EnumerateArray()) {

                string upath = item.GetProperty("Path").ToString();

                Result result = new Result
                {
                    Title = item.GetProperty("ProductName").ToString(),
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

			return results;
		}

        private static string RunCmd(string str_command, bool wait) {
            string str_output = "";
            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
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