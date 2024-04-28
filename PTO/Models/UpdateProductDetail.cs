﻿namespace PTO.Models
{
    public class UpdateProductDetail
    {
        public Guid JobId { get; set; }
        public Guid ProductId { get; set; }
        public int EstimatingSectionId { get; set; }
        public int? EstimatingUseId { get; set; } 
        public int NewEstimatingSectionId { get; set; }
        public string NewEstimatingSectionName { get; set; }
        public int NewEstimatingUseId { get; set; }
        public string NewEstimatingUseName { get; set; }
        public string NewColorCode { get; set; }
    }
}