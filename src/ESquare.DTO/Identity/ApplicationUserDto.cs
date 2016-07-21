namespace ESquare.DTO.Identity
{
    public class ApplicationUserDto
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string PasswordHash { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public string Avatar { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string PhoneNumber { get; set; }

        //public ICollection<string> Roles { get; set; }

        //public ICollection<Claim> Claims { get; set; }

        ///// <summary>
        ///// A random value that should change whenever a users credentials have changed (password changed, login removed)
        ///// 
        ///// </summary>
        //public string SecurityStamp { get; set; }

        ///// <summary>
        ///// True if the phone number is confirmed, default is false
        ///// 
        ///// </summary>
        //public bool PhoneNumberConfirmed { get; set; }

        ///// <summary>
        ///// Is two factor enabled for the user
        ///// 
        ///// </summary>
        //public bool TwoFactorEnabled { get; set; }

        ///// <summary>
        ///// DateTime in UTC when lockout ends, any time in the past is considered not locked out.
        ///// 
        ///// </summary>
        //public DateTime? LockoutEndDateUtc { get; set; }

        ///// <summary>
        ///// Is lockout enabled for this user
        ///// 
        ///// </summary>
        //public bool LockoutEnabled { get; set; }

        ///// <summary>
        ///// Used to record failures for the purposes of lockout
        ///// 
        ///// </summary>
        //public int AccessFailedCount { get; set; }

        ///// <summary>
        ///// Navigation property for user logins
        ///// 
        ///// </summary>
        //public ICollection<UserLoginInfo> Logins { get; set; }
    }
}
