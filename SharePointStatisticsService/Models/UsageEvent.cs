namespace SharePointStatisticsService.Models
{
    public class UsageEvent
    {
        public DateTime LogTime { get; set; }

        public string? UserLogin { get; set; }

        public string? SiteUrl { get; set; }

        public string? WebUrl { get; set; }

        public string? DocumentPath { get; set; }

        public string? RequestType { get; set; }

        public short? HttpStatus { get; set; }

        public string? ReferrerUrl { get; set; }

        public Guid RowId { get; set; }
    }
}