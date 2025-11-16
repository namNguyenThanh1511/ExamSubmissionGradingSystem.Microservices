namespace Service.DTO
{
    public class RubricDto
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public long? ExamId { get; set; }
        public string? ExamTitle { get; set; }
        public List<RubricCriterionDto> RubricCriteria { get; set; } = new List<RubricCriterionDto>();
    }
}
