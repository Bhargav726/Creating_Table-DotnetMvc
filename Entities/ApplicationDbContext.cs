﻿using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Person> Persons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");

            //Seed to Countries
            string countriesJson = System.IO.File.ReadAllText("countries.json");
            List<Country> countries = System.Text.Json.JsonSerializer.Deserialize<List<Country>>(countriesJson);

            foreach (Country country in countries)
                modelBuilder.Entity<Country>().HasData(country);

            //Seed to Persons
            string personsJson = System.IO.File.ReadAllText("persons.json");
            List<Person> persons = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(personsJson);

            foreach (Person person in persons)
                modelBuilder.Entity<Person>().HasData(person);



            //Fluent API

            //modelBuilder.Entity<Person>()
             //   .HasIndex(temp => temp.TIN).IsUnique();

            //modelBuilder.Entity<Person>().HasCheckConstraint("CHK_TIN", "len([TaxIdentificationNumber]) = 8");
            

        }
        
        public List<Person> sp_GetAllPersons()
        {
            return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
        }

        public int sp_InsertPerson(Person person)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@PersonId",person.PersonId),
                new SqlParameter("@PersonName",person.PersonName),
                new SqlParameter("@PersonEmail",person.PersonEmail),
                new SqlParameter("@PersonDOB",person.PersonDOB),
                new SqlParameter("@PersonGender",person.PersonGender),
                new SqlParameter("@CountryId",person.CountryId),
                new SqlParameter("@PersonAddress",person.PersonAddress),
                new SqlParameter("@PersonReceiveNewsLetters",person.PersonReceiveNewsLetters)
            };

            return Database.ExecuteSqlRaw("EXECUTE [dbo].[InsertPerson]@PersonId,@PersonName,@PersonEmail,@PersonDOB,@PersonGender,@CountryId,@PersonAddress,@PersonReceiveNewsLetters",parameters);
        
            }
    }
}
