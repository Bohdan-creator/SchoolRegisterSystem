using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolRegister.BLL.Entities;
using SchoolRegisterSystem.ViewModel.DTos;
using SchoolRegisterSystem.ViewModel.VMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolRegisterSystem.Configuration
{
    public static class AutoMapperConfig
    {
        public static IMapperConfigurationExpression Mapping(this IMapperConfigurationExpression configurationExpression)
        {
            Mapper.Initialize(mapper =>
            {
                mapper.CreateMap<GroupVm, SelectListItem>()
                .ForMember(x => x.Text, y => y.MapFrom(z => z.Name))
                .ForMember(x => x.Value, y => y.MapFrom(z => z.Id));

                mapper.CreateMap<StudentVm, SelectListItem>()
                .ForMember(x => x.Text, y => y.MapFrom(z => z.UserName))
                .ForMember(x => x.Value, y => y.MapFrom(z => z.Id));
                mapper.CreateMap<Student, AttachDetachStudentToGroup>();
                mapper.CreateMap<AddOrUpdateGroupDto, Group>();
                mapper.CreateMap<Group, GroupVm>();
                mapper.CreateMap<Student, StudentVm>()
                 .ForMember(dest => dest.GroupName, x => x.MapFrom(src => src.Group.Name))
                 .ForMember(dest => dest.ParentName,
                     x => x.MapFrom(src => $"{src.Parent.FirstName}"));
                mapper.CreateMap<AddOrUpdateSubjectDto, Subject>();
                mapper.CreateMap<SubjectVm, AddOrUpdateSubjectDto>();
                mapper.CreateMap<Subject, SubjectVm>()
                .ForMember(dest => dest.TeacherName, x => x.MapFrom(src => $"{src.Teacher.FirstName}{src.Teacher.SecondName}"))
                .ForMember(dest => dest.Groups, x => x.MapFrom(src => src.SubjectGroups.Select(y => y.Group)));

                mapper.CreateMap<AddGradeToStudentDto, Grade>();
                mapper.CreateMap<Grade, GradeVm>();
                mapper.CreateMap<Student, GradeReportVm>()
                .ForMember(dest => dest.StudentFirstName, y => y.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.StudentLastName, y => y.MapFrom(src => src.SecondName))
                .ForMember(dest => dest.GroupName, y => y.MapFrom(src => src.Group.Name))
                .ForMember(dest => dest.ParentName, y => y.MapFrom(src => $"{src.Parent.FirstName} {src.Parent.SecondName}"))
                  .ForMember(dest => dest.StudentGradesPerSubject, y => y.MapFrom(src => src.Grades
                        .GroupBy(g => g.Subject.Name)
                        .Select(g => new { SubjectName = g.Key, Grades = g.Select(gl => gl.GradeValue).ToList() })
                        .ToDictionary(x => x.SubjectName, x => x.Grades)));
                mapper.CreateMap<Teacher, TeacherVm>();

            });
            return configurationExpression;
        }
    }
}
