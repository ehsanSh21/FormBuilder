using FormBuilder.Data;
using FormBuilder.DTOs.Forms;
using FormBuilder.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace FormBuilder.Controllers
{
    [ApiController]
    [Route("api/forms")]
    public class FormsController : Controller
    {
        private readonly FormBuilderAPIDbContext dbContext;
        private readonly IMapper mapper;

        public FormsController(FormBuilderAPIDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }


        [HttpGet]
        public IActionResult GetForms()
        {

            var forms = dbContext.Forms
                        .Include(f => f.User)
                        .Select(f => new FormIndexDTO
                        {
                            Title = f.Title,
                            Type = f.Type,
                            UserName = f.User != null ? f.User.FullName : "N/A" 
                        })
                             .ToList();

            return Ok(forms);


        }

        [HttpPost]
        public async Task<IActionResult> AddForm(AddFormRequest addFormRequest)
        {

            var form = new Form
            {
                UserId = addFormRequest.UserId,
                Uuid = Guid.NewGuid(),
                Type = addFormRequest.Type,
                Title = addFormRequest.Title,

            };
            dbContext.Forms.Add(form);
            await dbContext.SaveChangesAsync();


            foreach (var formGroupRequest in addFormRequest.FormGroups)
            {
                var formGroup = new FormGroup
                {
                    FormId = form.Id,
                    Title = formGroupRequest.Title,
                    Data = formGroupRequest.Data,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                dbContext.FormGroups.Add(formGroup);

                await dbContext.SaveChangesAsync(); 

                if (formGroupRequest.Ratio.HasValue)
                {
                    var formGroupRatioMeta = new Meta
                    {
                        Key = "ratio",
                        Value = formGroupRequest.Ratio.Value.ToString(), // Assuming Ratio is a numeric value
                        IsJson = false,
                        RelatableType = "FormGroup",
                        RelatableId = formGroup.Id
                    };
                    dbContext.Metas.Add(formGroupRatioMeta);
                }




                foreach (var formElementRequest in formGroupRequest.Elements)
                {
                    var formElement = new FormElement
                    {

                        FormId = form.Id,
                        GroupId = formGroup.Id,
                        Uuid = Guid.NewGuid(),
                        Title = formElementRequest.Title,
                        Type = formElementRequest.Type,
                        Ordering = (int)formElementRequest.Ordering,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    dbContext.FormElements.Add(formElement);
                    await dbContext.SaveChangesAsync();

                    if (formElementRequest.Ratio.HasValue)
                    {
                        var formElementRatioMeta = new Meta
                        {
                            Key = "ratio",
                            Value = formElementRequest.Ratio.Value.ToString(), 
                            IsJson = false,
                            RelatableType = "FormElement",
                            RelatableId = formElement.Id
                        };
                        dbContext.Metas.Add(formElementRatioMeta);
                    }

                    if (formElementRequest.ReverseGrading.HasValue)
                    {
                        var formElementReverseGradingMeta = new Meta
                        {
                            Key = "reverse_grading",
                            Value = formElementRequest.ReverseGrading.Value.ToString(), 
                            IsJson = false,
                            RelatableType = "FormElement",
                            RelatableId = formElement.Id
                        };
                        dbContext.Metas.Add(formElementReverseGradingMeta);
                    }

                }
            }

        
            await dbContext.SaveChangesAsync();

          
            var newForm = dbContext.Forms
                      .Include(f => f.User) 
                      .Include(f => f.FormGroups)
                          .ThenInclude(fg => fg.FormElements)
                      .FirstOrDefault(f => f.Id == form.Id);


            var formDetailDto = mapper.Map<FormDetailDTO>(newForm);

            return Ok(formDetailDto);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateForm(Guid id, UpdateFormRequest updateFormRequest)
        {
            var existingForm = await dbContext.Forms.FindAsync(id);

            if (existingForm == null)
            {
                return NotFound();
            }

            existingForm.Type = updateFormRequest.Type;

            await dbContext.SaveChangesAsync();

            return Ok(existingForm);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetForm(int id)
        {


            var form = dbContext.Forms
                        .Include(f => f.User) 
                        .Include(f => f.FormGroups)
                            .ThenInclude(fg => fg.FormElements)
                        .FirstOrDefault(f => f.Id == id);



            if (form == null)
            {
                return NotFound(); 
            }
             
            var formDetailDto = mapper.Map<FormDetailDTO>(form);

            return Ok(formDetailDto);
        }


        [HttpPost("submit-answers/{formId}")]
        public IActionResult SubmitAnswer(int formId, [FromBody] SubmitAnswerRequest submitAnswerRequest)
        {
            // Validate submitAnswerRequest and formId

            var form = dbContext.Forms
                .Include(f => f.FormGroups)
                    .ThenInclude(fg => fg.FormElements)
                .FirstOrDefault(f => f.Id == formId);

            if (form == null)
            {
                return NotFound("Form not found");
            }

            foreach (var answerDto in submitAnswerRequest.Answers)
            {
                var formElement = form.FormGroups
                    .SelectMany(fg => fg.FormElements)
                    .FirstOrDefault(fe => fe.Id == answerDto.FormElementId);

                if (formElement == null)
                {
                    continue;
                }

                // Create Answer entity
                var answer = new Answer
                {
                    UserId = submitAnswerRequest.UserId,
                    FormElementId = answerDto.FormElementId,
                    AnswerText = answerDto.Answer,
                    EvaluatedUserId = submitAnswerRequest.EvaluatedUserId
                };

                dbContext.Answers.Add(answer);
            }

            dbContext.SaveChanges();


            var newForm = dbContext.Forms
                    .Include(f => f.User)
                    .Include(f => f.FormGroups)
                    .ThenInclude(fg => fg.FormElements)
                    .FirstOrDefault(f => f.Id == formId);

            if (newForm != null)
            {
                foreach (var formGroup in newForm.FormGroups)
                {
                    foreach (var formElement in formGroup.FormElements)
                    {
                        // Load answers for the specified UserId
                        dbContext.Entry(formElement)
                            .Collection(fe => fe.Answers)
                            .Query()
                            .Where(a => a.UserId == submitAnswerRequest.UserId)
                            .Load();
                    }
                }
            }


            var formDetailDto = mapper.Map<FormDetailDTO>(newForm);

            return Ok(formDetailDto);

            //return Ok("Answers submitted successfully");
        }



        [HttpGet("{formId}/evaluated/{evaluatedUserId}")]
        public IActionResult GetFormAndEvaluatedUser(int formId, Guid evaluatedUserId)
        {
            bool exists = dbContext.FormElementResults
                        .Any(f => f.FormElement.FormId == formId &&
                        f.User.Id == evaluatedUserId);
            if (!exists)
            {
                Console.WriteLine("asdas");
            }
            else
            {
                Console.WriteLine("Asdasd");
            }

            var df = dbContext.FormElementResults.All(f => f.OverallPoint > 10);

            Console.WriteLine(exists);

            var formElementIds = dbContext.Forms
                .Where(f => f.Id == formId)
                .SelectMany(f => f.FormElements.Select(fe => fe.Id))
                .ToList();

            foreach (var formElementId in formElementIds)
            {
                var ElementId = formElementId;
                var userId = evaluatedUserId;


                // Query to get data from the main tables
                var mainQuery = dbContext.FormElements
            .Join(
                dbContext.Answers.Where(a => a.EvaluatedUserId == userId),
                fe => fe.Id,
                a => a.FormElementId,
                (fe, a) => new { FormElement = fe, Answer = a }
            )
            .Join(
                dbContext.Metas.Where(m => m.RelatableId == ElementId && m.RelatableType == "FormElement" && m.Key == "ratio"),
                x => x.FormElement.Id,
                m => m.RelatableId,
                (x, m) => new { x.FormElement, x.Answer, RatioMeta = m }
            )
            .ToList(); 

                var reverseMetas = dbContext.Metas
                    .Where(m => m.RelatableType == "FormElement" && m.Key == "reverse_grading" && m.Value == "true")
                    .ToList(); 

                var result = mainQuery
                    .Where(x => x.FormElement.Id == ElementId)
                    .GroupBy(x => new
                    {
                        x.FormElement.Id,
                        x.Answer.EvaluatedUserId,
                        ReverseMetaKey = reverseMetas.FirstOrDefault(rm => rm.RelatableId == x.FormElement.Id)?.Key
                    })
                    .Select(g => new
                    {
                        Id = g.Key.Id,
                        EvaluatedUserId = g.Key.EvaluatedUserId,
                        ReverseMetaKey = g.Key.ReverseMetaKey,
                        OverallPoint = g.Key.ReverseMetaKey == "reverse_grading" ?
                5 - g.Select(a => decimal.TryParse(a.Answer.AnswerText, out var parsed) ? parsed : 0m).Average() + 1 :
                g.Select(a => decimal.TryParse(a.Answer.AnswerText, out var parsed) ? parsed : 0m).Sum() / g.Count()
                    })
                    .FirstOrDefault();


                var formElementResult = new FormElementResult
                {
                    FormElementId = result.Id,
                    UserId = result.EvaluatedUserId,
                    OverallPoint = result.OverallPoint
                };

                dbContext.FormElementResults.Add(formElementResult);
                dbContext.SaveChanges();

            }
           


            return Ok($"Form ID: {formId}, Evaluated User ID: {evaluatedUserId}");
        }



    }
}
