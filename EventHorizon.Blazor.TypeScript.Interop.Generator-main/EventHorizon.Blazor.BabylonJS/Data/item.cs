namespace EventHorizon.Blazor.BabylonJS.Data
{
    public class item
    {
        public decimal X { get; set; }
        public decimal Y { get; set; }
        public decimal Z { get; set; }
        public decimal[,] orientations { get; set; }

        public item()
        {
            X = 0;

            Y = 0;

            Z = 0;

            orientations = new decimal[,] { { X, Y, Z }, 
                                            { X, Z, Y }, 
                                            { Y, X, Z }, 
                                            { Y, Z, X }, 
                                            { Z, X, Y }, 
                                            { Z, Y, X }};

        }
    }
}
