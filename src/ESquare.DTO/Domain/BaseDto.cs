namespace ESquare.DTO.Domain
{
    /// <summary>
    /// Base class for all DTOs
    /// </summary>
    public abstract class BaseDto
    {
        public int Id { get; set; }

        public string RowVersion { get; set; }
    }
}
