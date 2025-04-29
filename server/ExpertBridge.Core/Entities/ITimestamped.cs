using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertBridge.Core.Entities
{
    public interface ITimestamped
    {
        DateTime? CreatedAt { get; set; }
        DateTime? LastModified { get; set; }
    }
}
