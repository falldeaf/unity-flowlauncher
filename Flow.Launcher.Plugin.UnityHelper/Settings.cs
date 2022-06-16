using System;
using System.Collections.Generic;
using System.Linq;

namespace Flow.Launcher.Plugin.UnityHelper {
    public class Settings {
        public string project_path { get; set; }
        public string cached_project_json { get; set; }
        public DateTime last_index_time { get; set; }
    }
}