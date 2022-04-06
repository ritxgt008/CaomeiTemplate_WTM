﻿using System.Collections.Generic;

namespace Caomei.Core
{
    /// <summary>
    /// DFS
    /// </summary>
    public class DFS
    {
        /// <summary>
        /// StorageMaxConnection
        /// </summary>
        public int? StorageMaxConnection { get; set; }

        /// <summary>
        /// TrackerMaxConnection
        /// </summary>
        public int? TrackerMaxConnection { get; set; }

        /// <summary>
        /// ConnectionTimeout
        /// </summary>
        public int? ConnectionTimeout { get; set; }

        /// <summary>
        /// ConnectionLifeTime
        /// </summary>
        public int? ConnectionLifeTime { get; set; }

        /// <summary>
        /// Trackers
        /// </summary>
        public List<DFSTracker> Trackers { get; set; }
    }
}