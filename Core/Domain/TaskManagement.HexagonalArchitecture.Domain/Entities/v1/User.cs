using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TaskManagement.HexagonalArchitecture.Domain.Entities.v1
{
    public class User : IdentityUser<Guid>
    {
        public User()
        {
        }

        public User(string firstName, string lastName, string email)
        {
            Id = Guid.NewGuid();

            FirstName = firstName;
            LastName = lastName;

            Email = email;
            UserName = email;
            NormalizedUserName = email.ToUpper();

            SecurityStamp = Guid.NewGuid().ToString();
            CreatedDate = DateTime.Now;
        }

        [MaxLength(256)]
        public string FirstName { get; private set; }
        [MaxLength(256)]
        public string LastName { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime? UpdatedDate { get; private set; }

        public List<Comment> Comments { get; private set; }
        public List<Assignment> Tasks { get; private set; }

        public void Update(string firstName, string lastName, string email)
        {
            FirstName = firstName;
            LastName = lastName;

            Email = email;
            UserName = email;
            NormalizedUserName = email.ToUpper();

            SecurityStamp = Guid.NewGuid().ToString();

            UpdatedDate = DateTime.Now;
        }
    }
}