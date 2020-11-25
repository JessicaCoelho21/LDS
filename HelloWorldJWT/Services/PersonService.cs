using HelloWorldJWT.Configs;
using HelloWorldJWT.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloWorldJWT.Services
{
    public class PersonService
    {
        public readonly HelloWorldContext _context;

        public PersonService(HelloWorldContext context)
        {
            _context = context;
        }

        //Create
        public Person Create(Person person, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new Exception("Password is required");
            }

            if(_context.People.Any(x => x.Username == person.Username))
            {
                throw new Exception("Username \"" + person.Username + "\" is already taken");
            }

            //Hashing of the password to be saved
            byte[] pwdHash, pwdSalt;
            CreatePasswordHash(password, out pwdHash, out pwdSalt);

            person.PasswordHash = pwdHash;
            person.PasswordSalt = pwdSalt;

            _context.People.Add(person);
            _context.SaveChanges();

            return person;
        }

        //Reads
        public Person GetById(long id)
        {
            return _context.People.Find(id);
        }

        public List<Person> GetAll()
        {
            return _context.People.ToList();
        }

        //Delete
        public void Delete(long id)
        {
            var person = _context.People.Find(id);

            if(person != null)
            {
                _context.People.Remove(person);
                _context.SaveChanges();
            }
        }

        //Update
        public void Update(Person personIn, string password = null)
        {
            var person = _context.People.Find(personIn.ID);

            if(person == null)
            {
                throw new Exception("User not found");
            }

            //Update person's first name if changed
            if (!string.IsNullOrWhiteSpace(personIn.FirstName))
            {
                person.FirstName = personIn.FirstName;
            }

            //Update person's last name if changed
            if (!string.IsNullOrWhiteSpace(personIn.LastName))
            {
                person.LastName = personIn.LastName;
            }

            //Update password if provided
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                person.PasswordHash = passwordHash;
                person.PasswordSalt = passwordSalt;
            }

            _context.People.Update(person);
            _context.SaveChanges();
        }

        //Authenticate
        public Person Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            var person = _context.People.SingleOrDefault(x => x.Username == username);

            if (person == null)
            {
                return null;
            }

            if (!VerifyPasswordHash(password, person.PasswordHash, person.PasswordSalt))
            {
                return null;
            }

            return person;
        }

        //Create Hash
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Value cannot be empty or whitespace, only string.", "password");
            }

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        //Verify Hash
        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Value cannot be empty or whitespace, only string.", "password");
            }

            if (storedHash.Length != 64)
            {
                throw new ArgumentException("Invalid lenght of password hash (64 bytes expected).", "passwordHash");
            }

            if (storedSalt.Length != 128)
            {
                throw new ArgumentException("Invalid lenght of password salt (128 bytes expected).", "passwordSalt");
            }

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
