using System;
using System.Collections.Generic;

namespace HubSpot.NET.Core
{
    /// <summary>
    /// Options used when querying for lists of items.
    /// </summary>
    public class ListRequestOptionsV3
    {
        private int _limit = 20;
        private readonly int _upperLimit;

        public ListRequestOptionsV3(int limit = 20)
        {
            _limit = limit;
        }

        /// <summary>
        /// Gets or sets the number of items to return.
        /// </summary>
        /// <remarks>
        /// Defaults to 20 which is also the HubSpot API default.
        /// </remarks>
        /// <value>
        /// The number of items to return.
        /// </value>
        public virtual int Limit
        {
            get => _limit;
            set
            {
                _limit = value;
            }
        }

        /// <summary>
        /// Get or set the continuation after string when calling list many times to enumerate all your items
        /// </summary>
        /// <remarks>
        /// </remarks>
        public string After { get; set; }

        public virtual List<string> PropertiesToInclude { get; set; } = new List<string>();
    }
}
