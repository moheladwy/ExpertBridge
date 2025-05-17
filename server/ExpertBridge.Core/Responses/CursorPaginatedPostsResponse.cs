using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertBridge.Core.Responses
{
    public class CursorPaginatedPostsResponse
    {
        public List<PostResponse> Posts { get; set; }
        public string? NextIdCursor { get; set; }
        public double? NextDistanceCursor { get; set; }
        public bool HasNextPage { get; set; } 
    }
}
