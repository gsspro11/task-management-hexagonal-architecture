using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TaskManagement.HexagonalArchitecture.Domain.Entities.v1
{
    public class User : IdentityUser<Guid>
    {
        public User()
        {
        }

        public User(string firstName, string lastName, string email
                       /*, IUsuarioUnico usuarioUnico*/)
        {
            //if (usuarioUnico != null && !usuarioUnico.Unico(email, cpf))
            //    throw new TaskManagementDomainException("E-mail ou CPF já cadastrados.");

            Id = Guid.NewGuid();

            FirstName = firstName;
            LastName = lastName;

            Email = email;

            UserName = email;
            NormalizedUserName = email.ToUpper();

            SecurityStamp = Guid.NewGuid().ToString();

            //Active = true;
            CreatedDate = DateTime.Now;
        }

        //public bool Active { get; private set; }
        [MaxLength(256)]
        public string FirstName { get; private set; }
        [MaxLength(256)]
        public string LastName { get; private set; }
        public DateTime UpdatedDate => DateTime.Now;
        public DateTime CreatedDate { get; private set; }

        public List<Comment> Comments { get; private set; }
        public List<Assignment> Tasks { get; private set; }

        //public void AtivarDesativar(bool value)
        //{
        //    Active = value;
        //    UpdateDate = DateTime.Now;
        //}
        public void Alterar(string name, string email /*, IUsuarioUnico usuarioUnico*/)
        {
            //if (usuarioUnico != null && !usuarioUnico.Unico(email, cpf, Id))
            //    throw TaskManagementException.Of(ProcessReturn.ERROR_EMAIL_EXISTS);

            Email = email;
            UserName = name;
            NormalizedUserName = name.ToUpper();

            SecurityStamp = Guid.NewGuid().ToString();

            //UpdateDate = DateTime.Now;
        }
    }
}