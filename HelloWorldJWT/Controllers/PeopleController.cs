using AutoMapper;
using HelloWorldJWT.Configs;
using HelloWorldJWT.Entities;
using HelloWorldJWT.Models.People;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace HelloWorldJWT.Services.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PeopleController : ControllerBase
    {
        private readonly PersonService _personService;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;

        //Constructor
        public PeopleController(PersonService personService, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            _personService = personService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        //CRUD
        //Create (register)
        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Create([FromBody] RegisterModel regModel)
        {
            var person = _mapper.Map<Person>(regModel);

            try
            {
                _personService.Create(person, regModel.Password);
                return Ok();
            }

            catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //Reads
        [HttpGet("{id}", Name = "Get Person")]
        public IActionResult GetById(long id)
        {
            var person = _personService.GetById(id);

            if(person == null)
            {
                return NotFound();
            }

            var personModel = _mapper.Map<List<PersonModel>>(person);

            return Ok(personModel);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var people = _personService.GetAll();
            var personModel = _mapper.Map<List<PersonModel>>(people);

            return Ok(personModel);
        }

        //Update
        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody]UpdateModel updateModel)
        {
            var personOld = _personService.GetById(id);

            if(personOld == null)
            {
                return NotFound();
            }

            var person = _mapper.Map<Person>(updateModel);
            person.ID = id;

            try
            {
                _personService.Update(person, updateModel.Password);
                return Ok();
            }

            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //Delete
        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            var toDel = _personService.GetById(id);

            if (toDel == null)
            {
                return NotFound();
            }

            _personService.Delete(id);

            return Ok();
        }

        //Authenticate
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AuthenticationModel authModel)
        {  
            var person = _personService.Authenticate(authModel.Username, authModel.Password);

            if (person == null)
            {
                return BadRequest(new { message = "Username or password is incorrect." });
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, person.ID.ToString())
                }),

                //Token expires in 7 days
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                Id = person.ID,
                FirstName = person.FirstName,
                LastName = person.LastName,
                Username = person.Username,
                Token = tokenString
            });
        }
    }
}


