using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertBridge.Core.Responses
{
    public class PostsCursorPaginatedResponse
    {
        public List<PostResponse> Posts { get; set; }
        public string? NextIdCursor { get; set; }
        public double? EndCursor { get; set; }
        public bool HasNextPage { get; set; } 
    }
}
