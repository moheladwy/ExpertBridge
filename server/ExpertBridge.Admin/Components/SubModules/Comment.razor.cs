using ExpertBridge.Core.Responses;
using Microsoft.AspNetCore.Components;

namespace ExpertBridge.Admin.Components.SubModules
{
  public partial class Comment : ComponentBase
  {
    [Parameter]
    public CommentResponse CommentResponse { get; set; }
  }
}
