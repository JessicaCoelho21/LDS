using HelloWorldJWT.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloWorldJWT.Configs
{
    public class HelloWorldContext: DbContext
    {
        public HelloWorldContext(DbContextOptions<HelloWorldContext> options) : base(options) { }
        public DbSet<Person> People { get; set; }
    }
}
