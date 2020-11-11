using HelloWorld.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloWorld.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PeopleController : ControllerBase
    {
        private readonly HelloWorldContext _context;

        public PeopleController (HelloWorldContext context)
        {
            _context = context;
        }

        //CRUD
        //Create
        [HttpPost]
        public IActionResult Create (Person person)
        {
            _context.People.Add(person);
            _context.SaveChanges();

            return CreatedAtRoute("Get Person", new { id = person.ID }, person);
        }

        //Read
        [HttpGet ("{id}", Name = "Get Person")]
        public ActionResult<Person> GetByID(long id)
        {
            var person = _context.People.Find(id);

            if(person == null)
            {
                return NotFound(); //404 not found
            }

            return person;
        }

        [HttpGet]
        public ActionResult<List<Person>> GetAll()
        {
            return _context.People.ToList();
        }

        //Update
        [HttpPut ("{id}")]
        public IActionResult Update(long id, Person person)
        {
            var pOld = _context.People.Find(id);

            if(pOld == null)
            {
                return NotFound();
            }

            pOld.FirstName = person.FirstName;
            pOld.LastName = person.LastName;
            pOld.Email = person.Email;

            _context.People.Update(pOld);
            _context.SaveChanges();

            return NoContent();
        }

        //Delete
        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            var toDel = _context.People.Find(id);

            if(toDel == null)
            {
                NotFound();
            }

            _context.People.Remove(toDel);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
