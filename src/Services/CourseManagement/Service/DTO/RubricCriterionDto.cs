namespace Service.DTO
{
    public class RubricCriterionDto
    {
        public long Id { get; set; }
        public long? RubricId { get; set; }
        public string? CriterionName { get; set; }
        public double? MaxScore { get; set; }
    }
}
