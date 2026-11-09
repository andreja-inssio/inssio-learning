using Microsoft.AspNetCore.Components;

namespace Recipes.Components;

public partial class ApiFeedback
{
    [Parameter] public string? Error { get; set; }
    [Parameter] public string? Success { get; set; }
}
