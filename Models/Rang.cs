namespace HSOP.Models
{
    class Rang
    {
        public int index { set; get; }
        public double value { set; get; }

        public Rang(int index, double value)
        {
            this.index = index;
            this.value = value;
        }
    }
}
