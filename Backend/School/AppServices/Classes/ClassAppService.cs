using System;
using System.Linq;
using AutoMapper;
using DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using DTOs.Classes;
using Entities.Classes;
using ApplicationServices.Exceptions;
using Microsoft.AspNetCore.JsonPatch;
using System.ComponentModel.DataAnnotations;

using Entities.Courses;
using Entities.Teachers;
using AutoMapper.QueryableExtensions;


namespace AppServices.Classes
{
    public class ClassAppService : IClassAppService
    {
        private readonly IRepository<Class> _repository;
        private readonly IMapper _mapper;
        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<Teacher> _teacherRepository;

        public ClassAppService(IRepository<Class> repository, IMapper mapper, IRepository<Course> courseRepository, IRepository<Teacher> teacherRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _courseRepository = courseRepository;
            _teacherRepository = teacherRepository;
        }

        public async Task CreateManyClasses()
        {

            using (var transaction = await _repository.BeginTransactionAsync())
            {
                try
                {

                    var modeClass = new Class
                    {
                        Schedule = "Sabados y domingos",
                        CourseId = 2,
                        TeacherId = 1,
                        Room = "Entidad 1"

                    };

                    var modeClass1 = new Class
                    {
                        Schedule = "Sabados y domingos",
                        CourseId = 2,
                        TeacherId = 1,
                        Room = "Entidad 2"

                    };

                    var modeClass2 = new Class
                    {
                        Schedule = "Sabados y domingos",
                        CourseId = 2,
                        TeacherId = 1,
                        Room = "Entidad 3"

                    };

                    await _repository.AddRepositoryAsync(modeClass);
                    await _repository.AddRepositoryAsync(modeClass1);
                    await _repository.AddRepositoryAsync(modeClass2);


                    await _repository.SaveChangesAsync();

                    transaction.Commit();

                }
                catch (Exception)
                {
                    // Revierte la transacción en caso de error.
                    transaction.RollbackAsync();
                    throw;
                }
            }


        }



        public async Task<ClassResponseDto> CreateClassAsync(ClassCreateDto dto, int userId)
        {
            var foreignCourse = await _courseRepository.GetByIdRepositoryAsync(dto.CourseId);


            if (foreignCourse == null)
                throw new BadRequestException("La llave foránea Course proporcionada no existe.");

            var foreignTeacher = await _teacherRepository.GetAllRepositoryAsync().Where(e => e.UserId == userId).FirstOrDefaultAsync();

            if (foreignTeacher == null)
            {
                throw new BadRequestException("El usuario logueado no es un maestro o el usuario no se encuentra en la base de datos");

            }

            var teacherId = foreignTeacher.Id;

            // Mapear DTO a entidad usando AutoMapper
            var entity = _mapper.Map<Class>(dto);

            entity.TeacherId = teacherId;

            var result = await _repository.AddRepositoryAsync(entity);

            await _repository.SaveChangesAsync();


            // Mapear entidad a DTO de respuesta usando AutoMapper
            var response = _mapper.Map<ClassResponseDto>(result);

            return response;
        }

        public async Task DeleteClassAsync(int id)
        {
            var entity = await _repository.GetByIdRepositoryAsync(id);
            if (entity == null)
                throw new NotFoundException("Class no encontrada.");


            await _repository.DeleteRepositoryAsync(entity);
            await _repository.SaveChangesAsync();

        }

        public async Task<List<ClassResponseDto>> GetAllClassesAsync()
        {
            var result = await _repository.GetAllRepositoryAsync().ProjectTo<ClassResponseDto>(_mapper.ConfigurationProvider).ToListAsync();

            //var students = await _context.Students.Where(s => s.Age > 18).ToListAsync();
            //var students = await _context.Students.Include(s => s.Courses).ToListAsync();
            //var studentNames = await _context.Students.Select(s => s.Name).ToListAsync();
            //var count = await _context.Students.CountAsync(s => s.Age > 18);
            //var exists = await _context.Students.AnyAsync(s => s.Name == "John");

            return result;
        }


        public async Task<ClassResponseDto> GetClassAsync(int id)
        {
            var entity = await _repository.GetByIdWithIncludesAsync(
                query =>
                {
                    query = query.Include(e => e.Course);
                    query = query.Include(e => e.Teacher);
                    return query;
                },
                id
            );
            if (entity == null)
                throw new NotFoundException("Class no encontrada.");

            var response = _mapper.Map<ClassResponseDto>(entity);
            return response;
        }


        public async Task<ClassResponseDto?> PatchClassAsync(int id, JsonPatchDocument<ClassPatchDto> patchDoc)
        {
            var entity = await _repository.GetByIdRepositoryAsync(id);
            if (entity == null)
                return null;

            var dto = _mapper.Map<ClassPatchDto>(entity);

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

            // Validación de llave foránea Course (requerida) en patch
            if (dto.CourseId != entity.CourseId)
            {
                var foreignCourse = await _courseRepository.GetByIdRepositoryAsync(dto.CourseId);
                if (foreignCourse == null)
                    throw new BadRequestException("La llave foránea Course proporcionada no existe.");
            }

            // Mapear los cambios del DTO a la entidad
            _mapper.Map(dto, entity);
            await _repository.UpdateRepositoryAsync(entity);

            await _repository.SaveChangesAsync();


            return _mapper.Map<ClassResponseDto>(entity);
        }


    }
}