namespace Service.DTO
{
    public class SemesterDto
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}
