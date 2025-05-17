using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertBridge.Core.Requests
{
    public class PostsCursorRequest
    {
        public int PageSize { get; set; } = 10;
        public double? LastDistanceCursor { get; set; }
        public string? LastIdCursor { get; set; }
    }
}
