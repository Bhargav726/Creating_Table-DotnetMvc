using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class PersonsRepository : IPersonsRepository
    {
        private readonly Entities.ApplicationDbContext _db;

        public PersonsRepository(Entities.ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<Person> AddPerson(Person person)
        {
            _db.Persons.Add(person);
            await _db.SaveChangesAsync();

            return person;
        }

        public async Task<bool> DeletePersonByPersonId(Guid personId)
        {
            _db.Persons.RemoveRange(_db.Persons.Where(temp => temp.PersonId == personId));
            int rowsDeleted=await _db.SaveChangesAsync();

            return rowsDeleted>0;
        }

        public async Task<List<Person>> GetAllPersons()
        {
            return await _db.Persons.Include("Country").ToListAsync();
        }

        public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
        {
            return await _db.Persons.Include("Country").Where(predicate).ToListAsync();
        }

        public async Task<Person?> GetPersonByPersonId(Guid personId)
        {
            return await _db.Persons.Include("Country").FirstOrDefaultAsync(temp => temp.PersonId == personId);
        }

        public async Task<Person> UpdatePerson(Person person)
        {
            Person? matchingPerson = await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonId == person.PersonId);

            if (matchingPerson == null)
                return person;

            matchingPerson.PersonId = person.PersonId;
            matchingPerson.PersonName = person.PersonName;
            matchingPerson.PersonEmail = person.PersonEmail;
            matchingPerson.PersonDOB = person.PersonDOB;
            matchingPerson.PersonGender = person.PersonGender;
            matchingPerson.CountryId = person.CountryId;
            matchingPerson.PersonAddress = person.PersonAddress;
            matchingPerson.PersonReceiveNewsLetters = person.PersonReceiveNewsLetters;

            int countryUpdated = await _db.SaveChangesAsync();

            return matchingPerson;
        }
    }
}
