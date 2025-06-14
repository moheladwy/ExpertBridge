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
        public PageInfoResponse PageInfo { get; set; }
    }
}
