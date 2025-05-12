using System;
using Entities;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using Moq;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit.Abstractions;

namespace CRUD_Tests;

public class PersonServiceTest
{
    //private fields
    private readonly IPersonService _personService;
    private readonly ICountriesService _countriesService;
    private readonly Mock<IPersonsRepository> _personRepositoryMock;
    private readonly IPersonsRepository _personsRepository;
    private readonly ITestOutputHelper _testOutputHelper; 

    //Constructor
    public PersonServiceTest(ITestOutputHelper testOutputHelper){

        _personRepositoryMock = new Mock<IPersonsRepository>();
        _personsRepository = _personRepositoryMock.Object;


        var countriesInitialData = new List<Country>() { };
        var personsInitialData = new List<Person>() { };

        DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);

        ApplicationDbContext dbContext = dbContextMock.Object;
        dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
        dbContextMock.CreateDbSetMock(temp => temp.Persons, personsInitialData);

        _countriesService = new CountriesService(null);

        _personService = new PersonService(_personsRepository);

        _testOutputHelper = testOutputHelper;


        //_countriesService = new CountriesService(new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().Options));
        //_personService=new PersonService(new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().Options),_countriesService);
        //_testOutputHelper=testOutputHelper;
    }
#region AddPerson

    //When we supply null value as PersonAddRequest, it should throw ArgumentNullException
    [Fact]
    public async Task AddPerson_NullPerson(){
        PersonAddRequest? personAddRequest=null;
        await Assert.ThrowsAsync<ArgumentNullException>(async ()=>{
            await _personService.AddPerson(personAddRequest);
        });
    }

    //When we supply null value as PersonName, it should throw ArgumentException
    [Fact]
    public async Task AddPerson_PersonNameIsNull(){
        PersonAddRequest? personAddRequest=new PersonAddRequest(){PersonName=null};
        await Assert.ThrowsAsync<ArgumentException>(async ()=>{
            await _personService.AddPerson(personAddRequest);
        });
    }

    //When we supply proper details,it insert the person into the persons list; and it sgould return an object of PersonResponse, which includes with the newly generated person id
    [Fact]
    public async Task AddPerson_FullPersonDetails_ToBeSuccessful(){
            //Arrange
            PersonAddRequest? personAddRequest=new PersonAddRequest(){PersonName="Bhargav",PersonEmail="Bhargav@gmail.com", PersonAddress="Hyderabad", PersonGender=GenderOptions.Male, CountryId=Guid.NewGuid(), PersonDOB=DateTime.Parse("2000-07-26"),PersonReceiveNewsLetters=true};

        Person person = personAddRequest.ToPerson();
        PersonResponse person_response_expected = person.ToPersonResponse();

        //If we supply any argument value to the AddPerson method,It should return the same return value
        _personRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);
            
        
        //Act
            PersonResponse person_response_from_add=await _personService.AddPerson(personAddRequest);
        List<PersonResponse> persons_list=await _personService.GetAllPersons();
        person_response_expected.PersonId = person_response_from_add.PersonId;
        //Assert

        //person_response_from_add.PersonId.Should().NotBe(Guid.Empty);
        //person_response_from_add.Should().Be(person_response_expected);

            Assert.True
           (person_response_from_add.PersonId!=Guid.Empty);
           Assert.Contains(person_response_from_add,persons_list);
    }
#endregion
#region GetPersonByPersonId
    //PersonResponse
    [Fact]
    public async Task GetPersonByPersonId_NullPersonId(){
        //Arrange
        Guid? personId=null;

        //Act
        PersonResponse? person_response_from_get=await _personService.GetPersonByPersonId(personId);

        //Assert
        Assert.Null(person_response_from_get);
    }

    //If we supply a valid person id, it should return the valid person details as personResponse object
    //person details as PersonResponse object
    [Fact]
    public async Task GetPersonByPersonId_WithPersonId(){
        //Arrange
        CountryAddRequest country_request=new CountryAddRequest(){CountryName="India"};
        CountryResponse country_response=await _countriesService.AddCountry(country_request);

        //Act
        PersonAddRequest person_request=new PersonAddRequest(){PersonName="Bhargav",PersonEmail="Bhargav@gmail.com", PersonAddress="Hyderabad", PersonGender=GenderOptions.Male, CountryId=Guid.NewGuid(), PersonDOB=DateTime.Parse("2000-07-26"),PersonReceiveNewsLetters=true};
        PersonResponse person_response_from_add=await _personService.AddPerson(person_request);
        PersonResponse? person_response_from_get=await _personService.GetPersonByPersonId(person_response_from_add.PersonId);

        //Assert
        Assert.Equal(person_response_from_add,person_response_from_get);
    }
    #endregion
#region GetAllPersons

    //The GetAllPersons() should return an empty list by default
    [Fact]
    public async Task GetAllPersons_EmptyList(){
        //Act
        List<PersonResponse> persons_from_get=await _personService.GetAllPersons();

        //Assert
        Assert.Empty(persons_from_get);
    }

    [Fact]
    public async Task GetAllPersons_AddFewPersons(){
        CountryAddRequest country_request_1=new CountryAddRequest(){CountryName="India"};
        CountryAddRequest country_request_2=new CountryAddRequest(){CountryName="Usa"};

        CountryResponse country_response_1=await _countriesService.AddCountry(country_request_1);
        CountryResponse country_response_2=await _countriesService.AddCountry(country_request_2);

        PersonAddRequest person_request_1=new PersonAddRequest(){PersonName="Bhargav",PersonEmail="Bhargav@gmail.com", PersonAddress="Hyderabad", PersonGender=GenderOptions.Male, CountryId=country_response_1.CountryId, PersonDOB=DateTime.Parse("2000-07-26"),PersonReceiveNewsLetters=true};
        PersonAddRequest person_request_2=new PersonAddRequest(){PersonName="Pavan",PersonEmail="Pavan@gmail.com", PersonAddress="Hyderabad", PersonGender=GenderOptions.Male, CountryId=country_response_1.CountryId, PersonDOB=DateTime.Parse("2001-08-26"),PersonReceiveNewsLetters=false};
        PersonAddRequest person_request_3=new PersonAddRequest(){PersonName="Chinni",PersonEmail="Chinni@gmail.com", PersonAddress="Hyderabad", PersonGender=GenderOptions.Female, CountryId=country_response_1.CountryId, PersonDOB=DateTime.Parse("2002-10-26"),PersonReceiveNewsLetters=true};

        List<PersonAddRequest> person_requests=new List<PersonAddRequest>(){person_request_1,person_request_2,person_request_3};
        List<PersonResponse> person_response_list_from_add=new List<PersonResponse>();

        foreach(PersonAddRequest person_request in person_requests){
            PersonResponse person_response=await _personService.AddPerson(person_request);
            person_response_list_from_add.Add(person_response);
        }

        //print person_response_list_from_add
        _testOutputHelper.WriteLine("Expected:");
        foreach(PersonResponse person_response_from_add in person_response_list_from_add){
            _testOutputHelper.WriteLine(person_response_from_add.ToString());
        }

        //Act
        List<PersonResponse> persons_list_from_get=await _personService.GetAllPersons();

        //print persons_list_from_get
        _testOutputHelper.WriteLine("Actual:");
        foreach(PersonResponse person_response_from_get in persons_list_from_get){
            _testOutputHelper.WriteLine(person_response_from_get.ToString());
        }

        //Assert
        foreach(PersonResponse person_response_from_add in person_response_list_from_add){
            Assert.Contains(person_response_from_add,persons_list_from_get);
        }

    }
    #endregion
    #region GetFilteredPersons
    [Fact]
    public async Task GetFilteredPersonst(){
        //Act
        List<PersonResponse> persons_from_get=await _personService.GetAllPersons();

        //Assert
        Assert.Empty(persons_from_get);
    }

    //If the search text is empty and search by is "PersonName", it should return all persons
    [Fact]
    public async Task GetFilteredPersons_EmptySearchText(){
        //Arrange
        CountryAddRequest country_request_1=new CountryAddRequest(){CountryName="India"};
        CountryAddRequest country_request_2=new CountryAddRequest(){CountryName="Usa"};

        CountryResponse country_response_1=await _countriesService.AddCountry(country_request_1);
        CountryResponse country_response_2=await _countriesService.AddCountry(country_request_2);

        PersonAddRequest person_request_1=new PersonAddRequest(){PersonName="Bhargav",PersonEmail="Bhargav@gmail.com", PersonAddress="Hyderabad", PersonGender=GenderOptions.Male, CountryId=country_response_1.CountryId, PersonDOB=DateTime.Parse("2000-07-26"),PersonReceiveNewsLetters=true};
        PersonAddRequest person_request_2=new PersonAddRequest(){PersonName="Pavan",PersonEmail="Pavan@gmail.com", PersonAddress="Hyderabad", PersonGender=GenderOptions.Male, CountryId=country_response_1.CountryId, PersonDOB=DateTime.Parse("2001-08-26"),PersonReceiveNewsLetters=false};
        PersonAddRequest person_request_3=new PersonAddRequest(){PersonName="Chinni",PersonEmail="Chinni@gmail.com", PersonAddress="Hyderabad", PersonGender=GenderOptions.Female, CountryId=country_response_1.CountryId, PersonDOB=DateTime.Parse("2002-10-26"),PersonReceiveNewsLetters=true};

        List<PersonAddRequest> person_requests=new List<PersonAddRequest>(){person_request_1,person_request_2,person_request_3};
        List<PersonResponse> person_response_list_from_add=new List<PersonResponse>();

        foreach(PersonAddRequest person_request in person_requests){
            PersonResponse person_response=await _personService.AddPerson(person_request);
            person_response_list_from_add.Add(person_response);
        }

        //print person_response_list_from_add
        _testOutputHelper.WriteLine("Expected:");
        foreach(PersonResponse person_response_from_add in person_response_list_from_add){
            _testOutputHelper.WriteLine(person_response_from_add.ToString());
        }

        //Act
        List<PersonResponse> persons_list_from_search=await _personService.GetFilteredPersons(nameof(Person.PersonName),"");

        //print persons_list_from_get
        _testOutputHelper.WriteLine("Actual:");
        foreach(PersonResponse person_response_from_get in persons_list_from_search){
            _testOutputHelper.WriteLine(person_response_from_get.ToString());
        }

        //Assert
        foreach(PersonResponse person_response_from_add in person_response_list_from_add){
            Assert.Contains(person_response_from_add,persons_list_from_search);
        }

    }

    //First we will add few persons; and then we will search based on person name with some search string. It should return the matching persons
    [Fact]
    public async Task GetFilteredPersons_SearchByPersonName(){
        CountryAddRequest country_request_1=new CountryAddRequest(){CountryName="India"};
        CountryAddRequest country_request_2=new CountryAddRequest(){CountryName="Usa"};

        CountryResponse country_response_1=await _countriesService.AddCountry(country_request_1);
        CountryResponse country_response_2=await _countriesService.AddCountry(country_request_2);

        PersonAddRequest person_request_1=new PersonAddRequest(){PersonName="Bhargav",PersonEmail="Bhargav@gmail.com", PersonAddress="Hyderabad", PersonGender=GenderOptions.Male,CountryId=country_response_1.CountryId, PersonDOB=DateTime.Parse("2000-07-26"),PersonReceiveNewsLetters=true};
        PersonAddRequest person_request_2=new PersonAddRequest(){PersonName="Pavan",PersonEmail="Pavan@gmail.com", PersonAddress="Hyderabad", PersonGender=GenderOptions.Male,CountryId=country_response_1.CountryId, PersonDOB=DateTime.Parse("2001-08-26"),PersonReceiveNewsLetters=false};
        PersonAddRequest person_request_3=new PersonAddRequest(){PersonName="Chinni",PersonEmail="Chinni@gmail.com", PersonAddress="Hyderabad", PersonGender=GenderOptions.Female,CountryId=country_response_1.CountryId, PersonDOB=DateTime.Parse("2002-10-26"),PersonReceiveNewsLetters=true};

        List<PersonAddRequest> person_requests=new List<PersonAddRequest>(){person_request_1,person_request_2,person_request_3};
        List<PersonResponse> person_response_list_from_add=new List<PersonResponse>();

        foreach(PersonAddRequest person_request in person_requests){
            PersonResponse person_response=await _personService.AddPerson(person_request);
            person_response_list_from_add.Add(person_response);
        }

        //print person_response_list_from_add
        _testOutputHelper.WriteLine("Expected:");
        foreach(PersonResponse person_response_from_add in person_response_list_from_add){
            _testOutputHelper.WriteLine(person_response_from_add.ToString());
        }

        //Act
        List<PersonResponse> persons_list_from_search=await _personService.GetFilteredPersons(nameof(Person.PersonName),"ma");

        //print persons_list_from_get
        _testOutputHelper.WriteLine("Actual:");
        foreach(PersonResponse person_response_from_get in persons_list_from_search){
            _testOutputHelper.WriteLine(person_response_from_get.ToString());
        }

        //Assert
        foreach(PersonResponse person_response_from_add in person_response_list_from_add){
            if(person_response_from_add.PersonName!=null){
            if(person_response_from_add.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase)){
                Assert.Contains(person_response_from_add,persons_list_from_search);
            }
            }
        }

    }
#endregion
    #region GetSortedPerson
    [Fact]
    public async Task GetSortedPersons(){
        CountryAddRequest country_request_1=new CountryAddRequest(){CountryName="India"};
        CountryAddRequest country_request_2=new CountryAddRequest(){CountryName="Usa"};

        CountryResponse country_response_1=await _countriesService.AddCountry(country_request_1);
        CountryResponse country_response_2=await _countriesService.AddCountry(country_request_2);

        PersonAddRequest person_request_1=new PersonAddRequest(){PersonName="Bhargav",PersonEmail="Bhargav@gmail.com", PersonAddress="Hyderabad", PersonGender=GenderOptions.Male, CountryId=country_response_1.CountryId, PersonDOB=DateTime.Parse("2000-07-26"),PersonReceiveNewsLetters=true};
        PersonAddRequest person_request_2=new PersonAddRequest(){PersonName="Pavan",PersonEmail="Pavan@gmail.com", PersonAddress="Hyderabad", PersonGender=GenderOptions.Male, CountryId=country_response_1.CountryId, PersonDOB=DateTime.Parse("2001-08-26"),PersonReceiveNewsLetters=false};
        PersonAddRequest person_request_3=new PersonAddRequest(){PersonName="Chinni",PersonEmail="Chinni@gmail.com", PersonAddress="Hyderabad", PersonGender=GenderOptions.Female, CountryId=country_response_1.CountryId, PersonDOB=DateTime.Parse("2002-10-26"),PersonReceiveNewsLetters=true};

        List<PersonAddRequest> person_requests=new List<PersonAddRequest>(){person_request_1,person_request_2,person_request_3};
        List<PersonResponse> person_response_list_from_add=new List<PersonResponse>();

        foreach(PersonAddRequest person_request in person_requests){
            PersonResponse person_response=await _personService.AddPerson(person_request);
            person_response_list_from_add.Add(person_response);
        }

        //print person_response_list_from_add
        _testOutputHelper.WriteLine("Expected:");
        foreach(PersonResponse person_response_from_add in person_response_list_from_add){
            _testOutputHelper.WriteLine(person_response_from_add.ToString());
        }

        List<PersonResponse> allPersons=await _personService.GetAllPersons();

        //Act
        List<PersonResponse> persons_list_from_sort=await _personService.GetSortedPersons(allPersons,nameof(Person.PersonName),SortOrderOptions.DESC);

        //print persons_list_from_get
        _testOutputHelper.WriteLine("Actual:");
        foreach(PersonResponse person_response_from_get in persons_list_from_sort){
            _testOutputHelper.WriteLine(person_response_from_get.ToString());
        }
        person_response_list_from_add= person_response_list_from_add.OrderByDescending(temp => temp.PersonName).ToList();

        //Assert
        for(int i=0;i<person_response_list_from_add.Count;i++){
           Assert.Equal(person_response_list_from_add[i], persons_list_from_sort[i]);
        }
    }
    #endregion
    #region UpdatePerson
    //When we supply null as PersonUpdateRequest, it should throw ArgumentNullException

    [Fact]
    public async Task UpdatePerson_NullPerson(){
        //Arrange
        PersonUpdateRequest? person_update_request=null;
        //Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async ()=>{
            //Act
            await _personService.UpdatePerson(person_update_request);
        });
    }

    //When we supply invalid person id, it should throw ArgumentException
    [Fact]
    public async Task UpdatePerson_InvalidPersonId(){
        //Arrange
        PersonUpdateRequest? person_update_request=new PersonUpdateRequest(){PersonId=Guid.NewGuid()};
        //Assert
        await Assert.ThrowsAsync<ArgumentException>(async ()=>{
            //Act
            await _personService.UpdatePerson(person_update_request);
        });
    }

    //When PersonName is null, it should throw ArgumentException
    [Fact]
    public async Task UpdatePerson_PersonNameIsNull()
    {

        CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "India" };
        CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

        PersonAddRequest person_add_request = new PersonAddRequest() { PersonName = "Bhargav", CountryId = country_response_from_add.CountryId, PersonEmail = "Bhargav@gmail.com", PersonGender = GenderOptions.Male };
        PersonResponse person_response_from_add = await _personService.AddPerson(person_add_request);
        PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
        person_update_request.PersonName = null;

        //Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _personService.UpdatePerson(person_update_request);
        });

    }
    //First, add a new person and try to update the person name and email
    [Fact]
    public async Task UpdatePerson_PersonFullDetailsUpdation(){
        CountryAddRequest country_add_request=new CountryAddRequest(){CountryName="India"};
        CountryResponse country_response_from_add=await _countriesService.AddCountry(country_add_request);

        PersonAddRequest person_add_request=new PersonAddRequest(){PersonName="Bhargav",CountryId=country_response_from_add.CountryId,PersonEmail="Bhargav@gmail.com",PersonGender=GenderOptions.Male};
        PersonResponse person_response_from_add=await _personService.AddPerson(person_add_request);
        PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
        person_update_request.PersonName="Sai";
        person_update_request.PersonEmail="saireddy@gmail.com";

        //Act
        PersonResponse person_response_from_update=await _personService.UpdatePerson(person_update_request);
        PersonResponse?person_response_from_get=await _personService.GetPersonByPersonId(person_response_from_update.PersonId);
        //Assert
        Assert.Equal(person_response_from_get,person_response_from_update);

    }      
    

    #endregion
    #region DeletePerson

    //if you supply an valid PersonId, it should return true
    [Fact]
    public async Task  DeletePerson_validPersonId(){
        CountryAddRequest country_add_request=new CountryAddRequest(){CountryName="USA"};
        CountryResponse country_response_from_add=await _countriesService.AddCountry(country_add_request);
        PersonAddRequest person_add_request=new PersonAddRequest(){
            PersonName="Pavan", PersonAddress="Khammam", CountryId=country_response_from_add.CountryId, PersonDOB=Convert.ToDateTime("2010-01-26"), PersonEmail="Pavan@gmail.com", PersonGender=GenderOptions.Male, PersonReceiveNewsLetters=true
        };

        PersonResponse person_response_from_add=await _personService.AddPerson(person_add_request);

        //Act
        bool isDeleted=await _personService.DeletePerson(person_response_from_add.PersonId);

        //Assert
        Assert.True(isDeleted);
    } 

    //if you supply an Invalid PersonId, it should return false
    [Fact]
    public async Task  DeletePerson_InvalidPersonId(){
        //Act
        bool isDeleted=await _personService.DeletePerson(Guid.NewGuid());

        //Assert
        Assert.False(isDeleted);
    } 
    #endregion
}