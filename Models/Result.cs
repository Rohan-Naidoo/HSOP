namespace HSOP.Models
{
    class Result
    {
        private int[] route;
        private double objectiveFunctionValue;

        public Result(int[] route, double objectiveFunctionValue)
        {
            this.route = route;
            this.objectiveFunctionValue = objectiveFunctionValue;
        }

        public int[] getRoute()
        {
            return route;
        }  

        public double getObjectiveFunctionValue()
        {
            return objectiveFunctionValue;
        }
    }
}
