using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ESquare.WebAPI.Models.Account
{
    public class CreateRoleBindingModel
    {
        [Required]
        [StringLength(256, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string Name { get; set; }
    }

    public class UsersInRoleModel
    {
        public string Id { get; set; }
        public List<string> EnrolledUserIds { get; set; }
        public List<string> RemovedUserIds { get; set; }
    }
}
