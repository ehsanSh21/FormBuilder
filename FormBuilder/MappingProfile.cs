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

                .ForMember(dest => dest.FormGroups, opt => opt.MapFrom(src => src.FormGroups));

            CreateMap<FormGroup, FormGroupDTO>()
                .ForMember(dest => dest.FormElements, opt => opt.MapFrom(src => src.FormElements));

            CreateMap<FormElement, FormElementDTO>()
                .ReverseMap(); 


            CreateMap<FormElement, FormElementDTO>()
            .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Answers));

            CreateMap<Answer, AnswerDTO>();

            CreateMap<FormElement, FormElementDTO>()
            .ForMember(dest => dest.FormElementResults, opt => opt.MapFrom(src => src.FormElementResults));

            CreateMap<FormElementResult, FormElementResultDTO>();

        }
    }
}
