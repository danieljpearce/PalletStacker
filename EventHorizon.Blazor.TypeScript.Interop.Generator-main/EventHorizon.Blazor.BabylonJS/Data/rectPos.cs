namespace EventHorizon.Blazor.BabylonJS.Data
{
    public class rectPos
    {
        public decimal[] startPos { get; set; }

        public decimal[] endPos { get; set; }

        public rectPos()
        {
            startPos = new decimal[3] { 0, 0, 0 };
            endPos = new decimal[3] { 0, 0, 0 };

        }
    }
}
