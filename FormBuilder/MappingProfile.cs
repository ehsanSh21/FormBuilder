using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FormBuilder.DTOs.Forms;
using FormBuilder.Models;

namespace FormBuilder
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Form, FormDetailDTO>()
                //.ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
                //.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName))
                // Add other mappings as needed

                .ForMember(dest => dest.FormGroups, opt => opt.MapFrom(src => src.FormGroups));

            CreateMap<FormGroup, FormGroupDTO>()
                // Add mappings for FormGroup properties

                .ForMember(dest => dest.FormElements, opt => opt.MapFrom(src => src.FormElements));

            CreateMap<FormElement, FormElementDTO>()
                // Add mappings for FormElement properties
                .ReverseMap(); // Add this if you also want to map from FormElementDTO to FormElement


            CreateMap<FormElement, FormElementDTO>()
           .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Answers));

            CreateMap<Answer, AnswerDTO>();

        }
    }
}
