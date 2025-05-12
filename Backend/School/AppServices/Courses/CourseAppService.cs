using System;
using System.Linq;
using AutoMapper;
using DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using Entities.Courses;
using ApplicationServices.Exceptions;
using Microsoft.AspNetCore.JsonPatch;
using System.ComponentModel.DataAnnotations;
using Dtos.Course;



namespace AppServices.Courses
{
    public class CourseAppService : ICourseAppService
    {
        private readonly IRepository<Course> _repository;
        private readonly IMapper _mapper;


        public CourseAppService(IRepository<Course> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;

        }

        public async Task<CourseResponseDto> CreateCourseAsync(CourseCreateDto dto)
        {

            // Mapear DTO a entidad usando AutoMapper
            var entity = _mapper.Map<Course>(dto);

            var result = await _repository.AddRepositoryAsync(entity);

            await _repository.SaveChangesAsync();


            // Mapear entidad a DTO de respuesta usando AutoMapper
            var response = _mapper.Map<CourseResponseDto>(result);

            return response;
        }

        public async Task DeleteCourseAsync(int id)
        {
            var entity = await _repository.GetByIdRepositoryAsync(id);
            if(entity == null)
                throw new NotFoundException("Course no encontrada.");


            await _repository.DeleteRepositoryAsync(entity);
            await _repository.SaveChangesAsync();

        }

        public async Task<List<CourseResponseDto>> GetAllCoursesAsync()
        {
            var entities = await _repository.GetAllRepositoryAsync().ToListAsync();
            // Mapear cada entidad a DTO de respuesta usando AutoMapper
            var result = _mapper.Map<List<CourseResponseDto>>(entities);
            return result;
        }


        public async Task<CourseResponseDto> GetCourseAsync(int id)
        {
            var entity = await _repository.GetByIdWithIncludesAsync(
                query =>
                {
                    query = query.Include(e => e.Classes);
                    return query;
                },
                id
            );
            
            if (entity == null)
                throw new NotFoundException("Course no encontrada.");
            var response = _mapper.Map<CourseResponseDto>(entity);
            return response;
        }


        public async Task<CourseResponseDto?> PatchCourseAsync(int id, JsonPatchDocument<CoursePatchDto> patchDoc)
        {
            var entity = await _repository.GetByIdRepositoryAsync(id);
            if(entity == null)
                return null;

            var dto = _mapper.Map<CoursePatchDto>(entity);

            // Se aplica el pacth y se manejan posibles errores
            patchDoc.ApplyTo(dto, error =>
            {
                if (error.ErrorMessage.Contains("The value '' is invalid for target location."))
                {
                    throw new BadRequestException($"El campo '{error.AffectedObject}' no puede ser nulo.");
                }
            });

            // Validar el DTO resultante
            var validationContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(dto, validationContext, validationResults, true);
            if (!isValid)
            {
                var errorMessages = string.Join("; ", validationResults.Select(r => r.ErrorMessage));
                throw new ValidationException($"Errores de validación: {errorMessages}");
            }



            // Mapear los cambios del DTO a la entidad
            _mapper.Map(dto, entity);
            await _repository.UpdateRepositoryAsync(entity);

            await _repository.SaveChangesAsync();


            return _mapper.Map<CourseResponseDto>(entity);
        }
    }
}
