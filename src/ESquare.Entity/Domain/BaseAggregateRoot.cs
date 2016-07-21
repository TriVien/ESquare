using System;
using System.ComponentModel.DataAnnotations;

namespace ESquare.Entity.Domain
{
    /// <summary>
    /// Base class for all aggregate roots
    /// </summary>
    public abstract class BaseAggregateRoot : BaseEntity
    {
        [Required]
        public virtual string CreatedBy { get; set; }

        [Required]
        public virtual DateTime CreatedDate { get; set; }

        public virtual string ModifiedBy { get; set; }

        public virtual DateTime? ModifiedDate { get; set; }
    }
}
