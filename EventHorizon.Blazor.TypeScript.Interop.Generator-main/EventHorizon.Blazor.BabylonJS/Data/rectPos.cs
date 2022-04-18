namespace EventHorizon.Blazor.BabylonJS.Data
{
    public class rectPos
    {
        public double[] startPos { get; set; }

        public double[] endPos { get; set; }

        public rectPos()
        {
            startPos = new double[3] { 0, 0, 0 };
            endPos = new double[3] { 0, 0, 0 };

        }
    }
}
