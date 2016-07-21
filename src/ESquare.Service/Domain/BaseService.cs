using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESquare.DTO;
using ESquare.DTO.Domain;
using ESquare.Entity.Domain;
using ESquare.Repository;
using ESquare.Service.Exceptions;
using ESquare.Service.Mappers;

namespace ESquare.Service.Domain
{
    public abstract class BaseService<TDto, TEntity>
        where TDto : BaseDto
        where TEntity : BaseAggregateRoot
    {
        protected IUnitOfWork UnitOfWork;
        protected IEntityMapper<TDto, TEntity> EntityMapper;
        protected IRepository<TEntity> Repository;

        protected BaseService(IUnitOfWork unitOfWork, IEntityMapper<TDto, TEntity> entityMapper)
        {
            UnitOfWork = unitOfWork;
            EntityMapper = entityMapper;
            Repository = unitOfWork.Repository<TEntity>();
        }

        #region Get

        public TDto GetById(int id)
        {
            var entity = Repository.Find(id);
            var dto = EntityMapper.MapFromEntity(entity);

            return dto;
        }

        public async Task<TDto> GetByIdAsync(int id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var entity = await Repository.FindAsync(id, cancellationToken);
            var dto = EntityMapper.MapFromEntity(entity);

            return dto;
        }

        #endregion

        #region List

        protected PagingResultDto<TDto> List(Expression<Func<TEntity, bool>> filter = null, string sortData = null, int? pageIndex = null, int? pageSize = null)
        {
            IEnumerable<TEntity> entities;

            // filtering
            var query = filter != null ? Repository.Query(filter) : Repository.Query();

            // order by
            if (string.IsNullOrEmpty(sortData))
            {
                var orderBy = TransformOrderByClause(sortData);
                query = query.OrderBy(orderBy);
            }

            // paging
            int totalItems;
            if (pageIndex != null && pageSize != null)
            {
                entities = query.SelectPaging(pageIndex.Value, pageSize.Value, out totalItems).ToList();
            }
            else
            {
                entities = query.Select().ToList();
                totalItems = entities.Count();
            }

            // result
            var dtoList = EntityMapper.MapFromEntity(entities);
            var result = new PagingResultDto<TDto>
            {
                TotalCount = totalItems,
                Items = dtoList
            };

            return result;
        }

        protected async Task<PagingResultDto<TDto>> ListAsync(Expression<Func<TEntity, bool>> filter = null, string sortData = null, int? pageIndex = null, int? pageSize = null)
        {
            IEnumerable<TEntity> entities;

            // filtering
            var query = filter != null ? Repository.Query(filter) : Repository.Query();

            // order by
            if (string.IsNullOrEmpty(sortData))
            {
                var orderBy = TransformOrderByClause(sortData);
                query = query.OrderBy(orderBy);
            }

            // paging
            int totalItems;
            if (pageIndex != null && pageSize != null)
            {
                entities = (await query.SelectPagingAsync(pageIndex.Value, pageSize.Value)).ToList();
                totalItems = await query.CountAsync();
            }
            else
            {
                entities = (await query.SelectAsync()).ToList();
                totalItems = entities.Count();
            }

            // result
            var dtoList = EntityMapper.MapFromEntity(entities);
            var result = new PagingResultDto<TDto>
            {
                TotalCount = totalItems,
                Items = dtoList
            };

            return result;
        }

        /// <summary>
        /// Constructs the order by clause based on the provided sort data. Sort terms must match items in
        /// the OrderByFieldMappings dictionary.
        /// </summary>
        /// <param name="sortData">Sort data in the format "DTOField1,asc;DTOField2,desc;DTOField3"
        /// Default sort order is asc</param>
        /// <returns>string use in dynamic LinQ in format: "Field1 asc,Field2 desc,Field3 asc"</returns>
        protected string TransformOrderByClause(string sortData)
        {
            try
            {
                var orderByClause = new StringBuilder();
                var hasPrimaryKeyValue = OrderByFieldMappings.ContainsKey("PrimaryKey");
                var firstOrderByColumnSortOrder = string.Empty;
                var containPrimaryKeyValue = false;

                // If no sortby field is supplied, use the default.
                if (string.IsNullOrEmpty(sortData))
                {
                    return OrderByFieldMappings["default"] + " asc";
                }

                var sortItems = sortData.Split(';');

                foreach (var sortItem in sortItems)
                {
                    if (!string.IsNullOrEmpty(sortItem))
                    {
                        var splitedSortItems = sortItem.Split(',');
                        var orderByColumnName = splitedSortItems[0];

                        if (hasPrimaryKeyValue && string.Compare(OrderByFieldMappings[orderByColumnName], OrderByFieldMappings["PrimaryKey"], StringComparison.InvariantCultureIgnoreCase) == 0)
                        {
                            containPrimaryKeyValue = true;
                        }

                        orderByClause.Append(OrderByFieldMappings[orderByColumnName]);

                        if (splitedSortItems.Length == 2)
                        {
                            var sortOrder = splitedSortItems[1].Equals("desc") ? " desc," : " asc,";
                            orderByClause.Append(sortOrder);
                            if (string.IsNullOrEmpty(firstOrderByColumnSortOrder))
                            {
                                firstOrderByColumnSortOrder = sortOrder;
                            }
                        }
                        else
                        {
                            orderByClause.Append(" asc,");  // Default sort order is asc
                        }
                    }
                }

                if (hasPrimaryKeyValue && !containPrimaryKeyValue)
                {
                    orderByClause.Append(OrderByFieldMappings["PrimaryKey"]);
                    orderByClause.Append(!string.IsNullOrEmpty(firstOrderByColumnSortOrder)
                        ? firstOrderByColumnSortOrder
                        : " asc,");
                }

                return orderByClause.ToString().TrimEnd(',');
            }
            catch (Exception)
            {
                throw new InvalidSortParameterException(sortData);
            }
        }

        /// <summary>
        /// Virtual base property to be overridden by the derived service with a list of
        /// mappings between DTO fields and EF column names.
        /// 
        /// Multiple database columns can be specified for the sort separated by semi-colon e.g.
        /// 
        ///     "ProductName", "Name;Category.Name"
        /// 
        /// This would specify that when the ProductName DTO field is specified as the
        /// sort column, the results are sorted by Name and then by Category.Name
        /// </summary>
        protected virtual Dictionary<string, string> OrderByFieldMappings => new Dictionary<string, string>();

        #endregion

        #region Insert or Update

        public TDto InsertOrUpdate(TDto dto, string userId)
        {
            TEntity entity = null;
            if (dto.Id > 0)
            {
                entity = Repository.Find(dto.Id);
            }

            entity = EntityMapper.MapToEntity(dto, entity);

            InsertOrUpdate(entity, userId);

            var resultDto = EntityMapper.MapFromEntity(entity);

            return resultDto;
        }

        protected void InsertOrUpdate(TEntity entity, string userId, bool commit = true)
        {
            if (entity.Id <= 0)
            {
                entity.CreatedBy = userId;
                Repository.Insert(entity);
            }
            else
            {
                entity.ModifiedBy = userId;
                Repository.Update(entity);
            }

            if (commit)
            {
                UnitOfWork.SaveChanges();
            }
        }

        public async Task<TDto> InsertOrUpdateAsync(TDto dto, string userId)
        {
            TEntity entity = null;
            if (dto.Id > 0)
            {
                entity = await Repository.FindAsync(dto.Id);
            }

            entity = EntityMapper.MapToEntity(dto, entity);

            await InsertOrUpdateAsync(entity, userId);

            var resultDto = EntityMapper.MapFromEntity(entity);

            return resultDto;
        }

        protected async Task InsertOrUpdateAsync(TEntity entity, string userId, bool commit = true)
        {
            if (entity.Id <= 0)
            {
                entity.CreatedBy = userId;
                Repository.Insert(entity);
            }
            else
            {
                entity.ModifiedBy = userId;
                Repository.Update(entity);
            }

            if (commit)
            {
                await UnitOfWork.SaveChangesAsync();
            }
        }

        #endregion

        #region Delete

        public void Delete(int id, string userId, bool commit = true)
        {
            Repository.Delete(id);

            if (commit)
            {
                UnitOfWork.SaveChanges();
            }
        }

        public async Task DeleteAsync(int id, string userId, bool commit = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            await Repository.DeleteAsync(id, cancellationToken);

            if (commit)
            {
                await UnitOfWork.SaveChangesAsync(cancellationToken: cancellationToken);
            }
        }

        #endregion

        //protected async Task<ListResultDto<TDto>> ListAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, int? pageIndex = null, int? pageSize = null)
        //{
        //    IEnumerable<TEntity> entities;

        //    // filtering
        //    var entitiesQuery = filter != null ? Repository.Query(filter) : Repository.Query();

        //    // order by
        //    if (orderBy != null)
        //    {
        //        entitiesQuery = entitiesQuery.OrderBy(orderBy);
        //    }

        //    // paging
        //    int totalItems;
        //    if (pageIndex != null && pageSize != null)
        //    {
        //        entities = await entitiesQuery.SelectPaging(pageIndex.Value, pageSize.Value, out totalItems).ToListAsync();
        //    }
        //    else
        //    {
        //        entities = await entitiesQuery.Select().ToListAsync();
        //        totalItems = entities.Count();
        //    }

        //    // result
        //    var entityDtoList = EntityMapper.MapFromEntity(entities);
        //    var result = new ListResultDto<TDto>
        //    {
        //        TotalCount = totalItems,
        //        Items = entityDtoList
        //    };

        //    return result;
        //}

        //protected ListResultDto<TDto> List(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, int? pageIndex = null, int? pageSize = null)
        //{
        //    IEnumerable<TEntity> entities;

        //    // filtering
        //    var entitiesQuery = filter != null ? Repository.Query(filter) : Repository.Query();

        //    // order by
        //    if (orderBy != null)
        //    {
        //        entitiesQuery = entitiesQuery.OrderBy(orderBy);
        //    }

        //    // paging
        //    int totalItems;
        //    if (pageIndex != null && pageSize != null)
        //    {
        //        entities = entitiesQuery.SelectPaging(pageIndex.Value, pageSize.Value, out totalItems).ToList();
        //    }
        //    else
        //    {
        //        entities = entitiesQuery.Select().ToList();
        //        totalItems = entities.Count();
        //    }

        //    // result
        //    var dtoList = EntityMapper.MapFromEntity(entities);
        //    var result = new ListResultDto<TDto>
        //    {
        //        TotalCount = totalItems,
        //        Items = dtoList
        //    };

        //    return result;
        //}

        //public virtual string InsertRange(IEnumerable<TDto> dtoList, int userId)
        //{
        //    var entities = EntityMapper.ConvertEntityDtoListToEntityList(dtoList).ToList();
        //    Repository.InsertRange(entities);
        //    Repository.UnitOfWork.SaveChanges();

        //    return null;
        //}

        //public virtual async Task<string> InsertRangeAsync(IEnumerable<TDto> dtoList, int userId)
        //{
        //    var entities = EntityMapper.ConvertEntityDtoListToEntityList(dtoList).ToList();
        //    Repository.InsertRange(entities);
        //    await Repository.UnitOfWork.SaveChangesAsync();

        //    return null;
        //}
    }
}
