using System;
using System.ComponentModel.DataAnnotations;

namespace EventHorizon.Blazor.BabylonJS.Pages;

class boxDimensions
{
    [Required(ErrorMessage = "Box Width Required")]
    [RegularExpression(@"^\d+(\.\d{1,2})?$")]
    public decimal boxX { get; set; }

    [Required(ErrorMessage = "Box Height Required")]
    [RegularExpression(@"^\d+(\.\d{1,2})?$")]
    public decimal boxY { get; set; }

    [Required(ErrorMessage = "Box Width Required")]
    [RegularExpression(@"^\d+(\.\d{1,2})?$")]
    public decimal boxZ { get; set; }

    public bool useStaircase { get; set; } 
}
