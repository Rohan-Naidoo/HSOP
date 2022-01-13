namespace HSOP.Models
{
    public struct Coordinate
    {
        public double x { set; get; }
        public double y { set; get; }
        public double weight { set; get; }

        public Coordinate(double x, double y)
        {
            this.x = x;
            this.y = y;
            this.weight = 0;
        }

        public Coordinate(double x, double y, double weight)
        {
            this.x = x;
            this.y = y;
            this.weight = weight;
        }
    }
}
