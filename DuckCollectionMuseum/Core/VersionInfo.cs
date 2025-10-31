using System;
using System.Collections.Generic;
using System.Text;

namespace DuckCollectionMuseum.Core
{
    public static class VersionInfo
    {
        public const string Name = "DuckCollectionMuseum";
        public const string Version = "1.0.0";

        public static string Tag => $"[{Name} v{Version}]";
    }
}