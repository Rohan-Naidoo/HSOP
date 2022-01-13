namespace HSOP.Models
{
    class Instance
    {
        public double[,] distanceMatrix { set; get; }
        public Coordinate[] coordinates { set; get; }
        public double[] points { set; get; }
        public double maxDistance { set; get; }
        public int noOfFirstVertex { set; get; }
        public int noOfLastVertex { set; get; }

        public Instance(Coordinate[] coordinates, double[,] distanceMatrix, double[] points, double maxDistance, int noOfFirstVertex, int noOfLastVertex)
        {
            this.distanceMatrix = distanceMatrix;
            this.coordinates = coordinates;
            this.points = points;
            this.maxDistance = maxDistance;
            this.noOfFirstVertex = noOfFirstVertex;
            this.noOfLastVertex = noOfLastVertex;
        }
    }
}
