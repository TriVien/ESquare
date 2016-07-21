using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESquare.DTO.Domain;

namespace ESquare.DTO
{
    public class PagingResultDto<TDto> where TDto : BaseDto
    {
        /// <summary>
        /// Items in page
        /// </summary>
        public IEnumerable<TDto> Items { get; set; }

        /// <summary>
        /// Total count
        /// </summary>
        public int TotalCount { get; set; }
    }
}
