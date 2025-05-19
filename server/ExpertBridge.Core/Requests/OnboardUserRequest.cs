using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertBridge.Core.Requests
{
    public class OnboardUserRequest
    {
        public List<string> TagIds { get; set; }
    }

    public class OnboardUserRequestV2
    {
        public List<string> Tags { get; set; }
    }
}
