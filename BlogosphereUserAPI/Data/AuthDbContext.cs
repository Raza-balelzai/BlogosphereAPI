using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogosphereUserAPI.Data
{
    public class AuthDbContext : IdentityDbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);
            var adminRoleId = "f1b767cc-fa88-4590-b5cf-f99d41e0ba12";
            var SuperAdminRoleId = "4f028a39-e7fa-4276-9bc2-701d6ebfb2b4";
            var userRoleId = "dce917d9-1f29-4ab4-bbf9-518570218255";
            //Seed Roles(SupeArdmin,Admin,User)
            var roles = new List<IdentityRole>()
            { 
                new IdentityRole
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "Admin",
                    ConcurrencyStamp=adminRoleId,

                },
                new IdentityRole
                {
                    Id=userRoleId,
                    Name = "User",
                    NormalizedName = "User",
                    ConcurrencyStamp = userRoleId,
                },
                new IdentityRole
                {
                    Id=SuperAdminRoleId,
                    Name="SuperAdmin",
                    NormalizedName="SuperAdmin",
                    ConcurrencyStamp=SuperAdminRoleId,
                },
            };
            builder.Entity<IdentityRole>().HasData(roles);
            //Seed SuperAdminUser
            var superAdminId = "fd649463-e2aa-4ed2-8cb7-74413b3385ce";
            var superAdminUser = new IdentityUser
            {
                UserName="superadmin@blogosphere.com",
                Email= "superadmin@blogosphere.com",
                NormalizedEmail= "superadmin@blogosphere.com".ToUpper(),
                NormalizedUserName= "superadmin@blogosphere.com".ToUpper(),
                Id=superAdminId,
            };
            superAdminUser.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(superAdminUser, "Superadmin@123");
            builder.Entity<IdentityUser>().HasData(superAdminUser);
            //Add all Roles to SuperAdminUser
            var superAdminRoles = new List<IdentityUserRole<string>>()
            {
                new IdentityUserRole<string>
                {
                    RoleId= userRoleId,
                    UserId=superAdminId,
                },
                new IdentityUserRole<string>
                {
                    RoleId= adminRoleId,
                    UserId=superAdminId,
                },
                new IdentityUserRole<string>
                {
                    RoleId= SuperAdminRoleId,
                    UserId=superAdminId,
                },
            };
            builder.Entity<IdentityUserRole<string>>().HasData(superAdminRoles);


        }
    }
}
