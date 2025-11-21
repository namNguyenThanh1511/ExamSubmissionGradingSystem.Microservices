using AutoMapper;
using Repository.Models;
using Service.DTO;

namespace Service.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Subject mappings
            CreateMap<Subject, SubjectDto>();
            CreateMap<SubjectDto, Subject>();
            CreateMap<CreateSubjectDto, Subject>();
            
            // Semester mappings
            CreateMap<Semester, SemesterDto>();
            CreateMap<SemesterDto, Semester>();
            CreateMap<CreateSemesterDto, Semester>();
            
            // Exam mappings
            CreateMap<Exam, ExamDto>()
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject != null ? src.Subject.Name : null))
                .ForMember(dest => dest.SubjectCode, opt => opt.MapFrom(src => src.Subject != null ? src.Subject.Code : null))
                .ForMember(dest => dest.SemesterName, opt => opt.MapFrom(src => src.Semester != null ? src.Semester.Name : null));
            CreateMap<ExamDto, Exam>();
            CreateMap<CreateExamDto, Exam>();
            
            // RubricCriterion mappings
            CreateMap<RubricCriterion, RubricCriterionDto>();
            CreateMap<RubricCriterionDto, RubricCriterion>();
            CreateMap<CreateRubricCriterionDto, RubricCriterion>();
            
            // Rubric mappings
            CreateMap<Rubric, RubricDto>()
                .ForMember(dest => dest.ExamTitle, opt => opt.MapFrom(src => src.Exam != null ? src.Exam.Title : null));
            CreateMap<RubricDto, Rubric>();
            CreateMap<CreateRubricDto, Rubric>();
            
            // Examiner mappings
            CreateMap<Examiner, ExaminerDto>();
            CreateMap<ExaminerDto, Examiner>();
            CreateMap<CreateExaminerDto, Examiner>();
            
            // Submission mappings
            CreateMap<Submission, SubmissionDto>()
                .ForMember(dest => dest.ExaminerName, opt => opt.MapFrom(src => src.Examiner != null ? src.Examiner.FullName : null));
            CreateMap<SubmissionDto, Submission>();
            CreateMap<CreateSubmissionDto, Submission>();
            
            // Submission Detail mappings
            CreateMap<Submission, SubmissionDetailDto>()
                .ForMember(dest => dest.ExaminerName, opt => opt.MapFrom(src => src.Examiner != null ? src.Examiner.FullName : null))
                .ForMember(dest => dest.StudentCode, opt => opt.MapFrom(src => src.StudentCode ?? string.Empty))
                .ForMember(dest => dest.CriterionScores, opt => opt.MapFrom(src => src.CriterionScores));
            
            CreateMap<CriterionScore, CriterionScoreDetailDto>()
                .ForMember(dest => dest.CriterionId, opt => opt.MapFrom(src => src.RubricCriterionId))
                .ForMember(dest => dest.CriterionName, opt => opt.MapFrom(src => src.RubricCriterion != null ? src.RubricCriterion.CriterionName : null))
                .ForMember(dest => dest.MaxScore, opt => opt.MapFrom(src => src.RubricCriterion != null ? src.RubricCriterion.MaxScore : null));
            
            // Violation mappings
            CreateMap<Violation, ViolationDto>()
                .ForMember(dest => dest.ExamTitle, opt => opt.MapFrom(src => src.Submission != null && src.Submission.Exam != null ? src.Submission.Exam.Title : null));
            CreateMap<ViolationDto, Violation>();
            CreateMap<CreateViolationDto, Violation>();
        }
    }
}
