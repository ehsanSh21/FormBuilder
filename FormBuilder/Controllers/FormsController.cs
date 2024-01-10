﻿using FormBuilder.Data;
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
                            UserName = f.User != null ? f.User.FullName : "N/A" // Use the user's name if available
                        })
                             .ToList();

            return Ok(forms);


        }

        [HttpPost]
        public async Task<IActionResult> AddForm(AddFormRequest addFormRequest)
        {

            // Create and add the Form entity
            var form = new Form
            {
                UserId = addFormRequest.UserId,
                Uuid = Guid.NewGuid(),
                Type = addFormRequest.Type,
                Title = addFormRequest.Title,
                //Point = addFormRequest.Point,
                //MinPoint = addFormRequest.MinPoint
                // Set other properties as needed
            };
            dbContext.Forms.Add(form);
            await dbContext.SaveChangesAsync();


            // Loop through FormGroups in the request and create FormGroups and FormElements
            foreach (var formGroupRequest in addFormRequest.FormGroups)
            {
                var formGroup = new FormGroup
                {
                    FormId = form.Id,
                    Title = formGroupRequest.Title,
                    Data = formGroupRequest.Data,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                    // Set other properties as needed
                };
                dbContext.FormGroups.Add(formGroup);

                await dbContext.SaveChangesAsync(); // Save to get the formGroup Id

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


                //Console.WriteLine(formGroupRequest.Elements);


                foreach (var formElementRequest in formGroupRequest.Elements)
                {
                    var formElement = new FormElement
                    {

                        FormId = form.Id,
                        GroupId = formGroup.Id,
                        Uuid = Guid.NewGuid(),
                        Title = formElementRequest.Title,
                        Type = formElementRequest.Type,
                        Ordering = (int)formElementRequest.Ordering, // Adjust based on your requirements
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                        // Set other properties as needed
                    };
                    dbContext.FormElements.Add(formElement);
                    await dbContext.SaveChangesAsync();

                    if (formElementRequest.Ratio.HasValue)
                    {
                        var formElementRatioMeta = new Meta
                        {
                            Key = "ratio",
                            Value = formElementRequest.Ratio.Value.ToString(), // Assuming Ratio is a numeric value
                            IsJson = false,
                            RelatableType = "FormElement",
                            RelatableId = formElement.Id
                        };
                        dbContext.Metas.Add(formElementRatioMeta);
                    }

                    // Store reverse_grading for FormElement if not null
                    if (formElementRequest.ReverseGrading.HasValue)
                    {
                        var formElementReverseGradingMeta = new Meta
                        {
                            Key = "reverse_grading",
                            Value = formElementRequest.ReverseGrading.Value.ToString(), // Assuming ReverseGrading is a boolean
                            IsJson = false,
                            RelatableType = "FormElement",
                            RelatableId = formElement.Id
                        };
                        dbContext.Metas.Add(formElementReverseGradingMeta);
                    }

                }
            }

            // Save changes to the database
            await dbContext.SaveChangesAsync();

            // Return the ID of the created Form entity
            //return Ok(new { FormId = form.Id });

            var newForm = dbContext.Forms
                      .Include(f => f.User) // Include User details if needed
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
                return NotFound(); // Form not found
            }

            // Update form properties
            existingForm.Type = updateFormRequest.Type;
            // Update other properties as needed

            // Save changes to the database
            await dbContext.SaveChangesAsync();

            return Ok(existingForm);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetForm(int id)
        {

            //var form = await dbContext.Forms.FindAsync(id);

            var form = dbContext.Forms
                        .Include(f => f.User) // Include User details if needed
                        .Include(f => f.FormGroups)
                            .ThenInclude(fg => fg.FormElements)
                        .FirstOrDefault(f => f.Id == id);



            if (form == null)
            {
                return NotFound(); // Form not found
            }

            //var formDetailDTO = new FormDetailDTO
            //{
            //    Id = form.Id,
            //    Uuid = form.Uuid,
            //    Title = form.Title,
            //    Type = form.Type,
            //    UserFullName = form.User?.FullName,
            //    // Map other properties
            //    //UserName = User != null ? User.FullName : "N/A" // Use the user's name if available

            //    //User = new UserDTO
            //    //{
            //    //    // Map user properties
            //    //},

            //    FormGroups = form.FormGroups.Select(fg => new FormGroupDTO
            //    {
            //        Id = fg.Id,
            //        Title = fg.Title,
            //        Data = fg.Data,
            //        // Map other properties

            //        FormElements = fg.FormElements.Select(fe => new FormElementDTO
            //        {
            //            Title = fe.Title,
            //            Type = fe.Type,
            //            Ordering = fe.Ordering
            //            // Map other properties
            //        }).ToList()
            //    }).ToList()
            //};

            //return Ok(formDetailDTO);

            // Use AutoMapper to map Form to FormDetailDTO
            var formDetailDto = mapper.Map<FormDetailDTO>(form);

            return Ok(formDetailDto);
        }


        //[HttpPost("{formId}/submit")]
        //public async Task<IActionResult> SubmitAnswer(int formId, [FromBody] SubmitAnswerRequest submitAnswerRequest)
        //{
        //    // Check if the form with the given id exists
        //    var form = await dbContext.Forms.FindAsync(formId);

        //    if (form == null)
        //    {
        //        return NotFound($"Form with ID {formId} not found.");
        //    }

        //    // Validate the request data as needed
        //    // (You may want to create a separate validation method or use attributes on your request model)

        //    // Create and add the answer entity
        //    var answer = new Answer
        //    {
        //        UserId = submitAnswerRequest.UserId,
        //        FormElementId = submitAnswerRequest.FormElementId,
        //        AnswerText = submitAnswerRequest.Answer,
        //        EvaluatedUserId = submitAnswerRequest.EvaluatedUserId,
        //        // Set other properties as needed
        //    };

        //    dbContext.Answers.Add(answer);

        //    // Save changes to the database
        //    await dbContext.SaveChangesAsync();

        //    return Ok(answer);
        //}

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
            //var userId = new Guid("63dd9c13-22b6-44aa-b9ad-b52c889c594d");
            //var ElementId = 28;


            bool exists = dbContext.FormElementResults
                    .Any(f => f.FormElement.FormId == formId &&
                        f.User.Id == evaluatedUserId);

            Console.WriteLine(exists);

            var formElementIds = dbContext.Forms
                .Where(f => f.Id == formId)
                .SelectMany(f => f.FormElements.Select(fe => fe.Id))
                .ToList();

            //var 
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
            .ToList(); // Fetch data from the database

                // Query to get data for LEFT JOIN
                var reverseMetas = dbContext.Metas
                    .Where(m => m.RelatableType == "FormElement" && m.Key == "reverse_grading" && m.Value == "true")
                    .ToList(); // Fetch data from the database

                // Combine the results
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


        private async Task<decimal?> CalculateOverallPoint(int formElementId, Guid evaluatedUserId)
        {

            Console.WriteLine(formElementId);
            // Your raw SQL query using Entity Framework
            string sqlQuery = @"
        SELECT 
            CASE 
                WHEN metas_reverse.[key] = 'reverse_grading' THEN 5 - AVG(CAST(answers.AnswerText AS DECIMAL)) + 1 
                ELSE SUM(CAST(answers.AnswerText AS DECIMAL)) / COUNT(*) 
            END AS overall_point
        FROM 
            FormElements
        JOIN 
            answers ON FormElements.id = Answers.FormElementId
            AND answers.EvaluatedUserId = @EvaluatedUserId
        JOIN 
            metas ON FormElementId = metas.RelatableId
            AND metas.RelatableType = 'FormElement'
            AND metas.[key] = 'ratio'
        LEFT JOIN 
            metas AS metas_reverse ON FormElements.id = metas_reverse.RelatableId
            AND metas_reverse.RelatableType = 'FormElement'
            AND metas_reverse.[key] = 'reverse_grading'
            AND metas_reverse.[value] = 'true'
        WHERE 
            FormElements.id = @FormElementId
        GROUP BY 
            FormElements.id,
            answers.EvaluatedUserId,
            metas_reverse.[key];
    ";

            // Execute the query using Entity Framework
            var result = await dbContext.Query<OverallPointDTO>()
                .FromSqlRaw(sqlQuery, new SqlParameter("@EvaluatedUserId", evaluatedUserId), new SqlParameter("@FormElementId", formElementId))
                .FirstOrDefaultAsync();

            Console.WriteLine(result);

            // Return the overall point
            return result?.OverallPoint;
        }

        public class OverallPointDTO
        {
            public decimal? OverallPoint { get; set; }
        }



    }
}
