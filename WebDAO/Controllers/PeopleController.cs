using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebDAO.DataAccess;
using WebDAO.Interfaces;
using WebDAO.Models;

namespace WebDAO.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PeopleController : ControllerBase
    {
        private IConnection _connection;

        public PeopleController(IConnection connection)
        {
            _connection = new Connection();
            _connection.Fetch();
        }

        //CRUD
        //Create
        [HttpPost]
        public IActionResult Create(Person person)
        {
            IDAO<Person> personDAO = new PersonDAO(_connection);
            personDAO.Create(person);

            return CreatedAtRoute("Get Person", new { id = person.Id }, person);
        }

        //Read
        [HttpGet("{id}", Name = "Get Person")]
        public ActionResult<Person> GetByID(long id)
        {
            PersonDAO personDAO = new PersonDAO(_connection);
            var person = personDAO.FindById(id);

            if (person == null)
            {
                return NotFound(); //404 not found
            }

            return person;
        }

        [HttpGet]
        public ActionResult<List<Person>> GetAll()
        {
            PersonDAO personDAO = new PersonDAO(_connection);
            return personDAO.GetAll().ToList();
        }

        //Update
        [HttpPut("{id}")]
        public IActionResult Update(long id, Person person)
        {
            PersonDAO personDAO = new PersonDAO(_connection);
            var pOld = personDAO.FindById(id);

            if (pOld == null)
            {
                return NotFound();
            }

            pOld.FirstName = person.FirstName;
            pOld.LastName = person.LastName;
            pOld.Age = person.Age;
            pOld.Email = person.Email;

            personDAO.Update(pOld);

            return NoContent();
        }

        //Delete
        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            PersonDAO personDAO = new PersonDAO(_connection);
            var toDel = personDAO.FindById(id);

            if (toDel == null)
            {
                NotFound();
            }

            personDAO.Delete(toDel);

            return NoContent();
        }
    }
}

