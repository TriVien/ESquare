using System.Collections.Generic;
using ESquare.DTO.Domain;
using ESquare.Entity.Domain;

namespace ESquare.Service.Mappers
{
    /// <summary>
    /// Maps between entity and DTO
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public interface IEntityMapper<TDto, TEntity> where TDto : BaseDto where TEntity : BaseEntity
    {
        /// <summary>
        /// Maps DTO to entity.
        /// If entity is not provided, creates a new entity, otherwise, maps to provided entity
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        TEntity MapToEntity(TDto dto, TEntity entity = null);

        /// <summary>
        /// Maps entity to DTO
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        TDto MapFromEntity(TEntity entity);

        /// <summary>
        /// Maps entities to DTOs
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        IEnumerable<TDto> MapFromEntity(IEnumerable<TEntity> entities);
    }
}
