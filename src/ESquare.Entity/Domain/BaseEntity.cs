using System.ComponentModel.DataAnnotations;

namespace ESquare.Entity.Domain
{
    /// <summary>
    /// Base class for all entities
    /// </summary>
    public abstract class BaseEntity
    {
        [Key]
        public virtual int Id { get; set; }

        [Timestamp]
        [ConcurrencyCheck]
        public byte[] RowVersion { get; set; }
    }
}
