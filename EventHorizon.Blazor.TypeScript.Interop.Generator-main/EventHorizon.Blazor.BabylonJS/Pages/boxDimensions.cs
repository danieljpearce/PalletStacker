using System;
using System.ComponentModel.DataAnnotations;

namespace EventHorizon.Blazor.BabylonJS.Pages;

class boxDimensions
{
    [Required(ErrorMessage = "Box Width Required")]
    [Range(0.1, 1.2, ErrorMessage = "Value must be between 0.1 and the pallet width")]
    public decimal boxX { get; set; } = 0.12m;

    [Required(ErrorMessage = "Box Height Required")]
    [Range(0.1, 1, ErrorMessage = "Value must be between 0.1 and the pallet height")]
    public decimal boxY { get; set; } = 0.25m;

    [Required(ErrorMessage = "Box Width Required")]
    [Range(0.1, 1, ErrorMessage = "Value must be between 0.1 and the pallet length")]
    public decimal boxZ { get; set; } = 0.17m;

    public bool useStaircase { get; set; } 
}
