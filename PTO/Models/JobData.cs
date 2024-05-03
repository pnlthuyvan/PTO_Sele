namespace PTO.Models
{
    public class JobData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public DateTime DueDate { get; set; }
        public string EstimatorName { get; set; }
        public string EstimatedValue { get; set; }
        public string SalesRep { get; set; }
        public DateTime LastOpenedDate { get; set; }
        public string OriginalName { get; set; }
        public string FileSize { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public string Address { get; set; }
        public string EstimatorId { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class JobDataWrapper
    {
        public JobData[] Data { get; set; }
    }
}
