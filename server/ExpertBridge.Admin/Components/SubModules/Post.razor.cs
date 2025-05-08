using ExpertBridge.Core.Responses;
using Microsoft.AspNetCore.Components;

namespace ExpertBridge.Admin.Components.SubModules
{
  /// <summary>
  /// This is the code-behind file for the Post component.
  /// </summary>
  public partial class Post : ComponentBase
  {
    /// <summary>
    ///   The post-data to be displayed in the component.
    /// </summary>
    [Parameter]
    public PostResponse? PostResponse { get; set; }
  }
}
