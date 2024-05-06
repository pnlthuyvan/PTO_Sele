namespace PTO.Models.Microsoft
{
    public class JobData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LocationName { get; set; }
        public string CustomerName { get; set; }
        public string EstimatorName { get; set; }
        public string EstimatedValue { get; set; }
        public string SalesRep { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public string Address { get; set; }
    }

    public class JobDataWrapper
    {
        public JobData[] Data { get; set; }
    }
}
