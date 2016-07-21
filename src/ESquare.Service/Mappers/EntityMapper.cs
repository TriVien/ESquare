using System.Collections.Generic;
using AutoMapper;
using ESquare.DTO.Domain;
using ESquare.Entity.Domain;

namespace ESquare.Service.Mappers
{
    public abstract class EntityMapper<TDto, TEntity> : IEntityMapper<TDto, TEntity>, ICreateMapping
        where TDto : BaseDto
        where TEntity : BaseEntity, new()
    {
        private readonly IMapper _mapper;

        protected EntityMapper(IMapper mapper)
        {
            _mapper = mapper;
        }

        public virtual TEntity MapToEntity(TDto dto, TEntity entity = null)
        {
            return entity == null ? _mapper.Map<TEntity>(dto) : _mapper.Map(dto, entity);
        }

        public virtual TDto MapFromEntity(TEntity entity)
        {
            return _mapper.Map<TDto>(entity);
        }

        public virtual IEnumerable<TDto> MapFromEntity(IEnumerable<TEntity> entities)
        {
            return _mapper.Map<IEnumerable<TDto>>(entities);
        }

        public abstract void CreateMapping(Profile profile);
    }
}
