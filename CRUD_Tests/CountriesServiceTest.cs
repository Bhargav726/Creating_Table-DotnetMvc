using System;
using System.Runtime.CompilerServices;
using Entities;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace CRUD_Tests;

public class CountriesServiceTest
{
    private readonly ICountriesService _countriesService;
    //constructor
    public CountriesServiceTest(){
        var countriesInitialData = new List<Country>() { };

        DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);

        ApplicationDbContext dbContext = dbContextMock.Object;
        dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);

        _countriesService = new CountriesService(null);

        //var dbContext = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().Options);
        //_countriesService =new CountriesService(dbContext);
    }

    #region  AddCountry

    //When CountryAddRequest is null, it should throw ArgumentNullException
    [Fact]
    public async Task  AddCountry_NullCountry(){
        //Arrange
        CountryAddRequest? request=null;

        //Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async ()=>{
             //Act
        await _countriesService.AddCountry(request);
        });
    }


    //When the CountryName is null, it should throw ArgumentException
    [Fact]
    public async Task AddCountry_CountryNameIsNull(){
        //Arrange
        CountryAddRequest? request=new CountryAddRequest(){CountryName=null};

        //Assert
        await Assert.ThrowsAsync<ArgumentException>(async ()=>{
             //Act
        await _countriesService.AddCountry(request);
        });
    }

    //When the CountryName is duplicate, it should throw ArgumentException
    [Fact]
    public async Task AddCountry_DulicateCountryName(){
        //Arrange
        CountryAddRequest? request1=new CountryAddRequest(){CountryName="India"};
         CountryAddRequest? request2=new CountryAddRequest(){CountryName="India"};

        //Assert
        await Assert.ThrowsAsync<ArgumentException>(async ()=>{
             //Act
        await _countriesService.AddCountry(request1);
        await _countriesService.AddCountry(request2);
        });
    }

    //When you supply proper country name, it should insert (add) the country to the existing list of countries
    [Fact]
    public async Task  AddCountry_ProperCountryDetails(){
        //Arrange
        CountryAddRequest? request=new CountryAddRequest(){CountryName="USA"};

        //Act
        CountryResponse response=await _countriesService.AddCountry(request);
        List<CountryResponse> countries_from_GetAllCountries=await _countriesService.GetAllCountries();

        //Assert
        Assert.True(response.CountryId!=Guid.Empty);
        Assert.Contains(response,countries_from_GetAllCountries);
    }
    #endregion

#region GetAllCountries
[Fact]
public async Task GetAllCountries_EmptyList(){
List<CountryResponse>actual_country_response_list=await _countriesService.GetAllCountries();
Assert.Empty(actual_country_response_list);
}

[Fact]
public async Task GetAllCountries_AddFewCountries(){
    List<CountryAddRequest> country_request_list=new List<CountryAddRequest>(){
        new CountryAddRequest() {CountryName="India"},
        new CountryAddRequest(){CountryName="USA"}
    };
    List<CountryResponse> country_list_from_add_country=new List<CountryResponse>();
    foreach(CountryAddRequest country_request in country_request_list){
        country_list_from_add_country.Add(await _countriesService.AddCountry(country_request));
    }
    List<CountryResponse>actualCountryResponseList=await _countriesService.GetAllCountries();

    //read each element from countries_list_from_add_country
    foreach(CountryResponse expected_country in country_list_from_add_country){
        Assert.Contains(expected_country,actualCountryResponseList);
    }
}
#endregion

#region GetCountryByCountryId
[Fact]
public async Task GetCountryByCountryId_NullCountryId(){
    //Arrange
    Guid? countryId=null;

    //Act
    CountryResponse? country_response_from_get_method=await _countriesService.GetCountryByCountryId(countryId);

    //Assert
    Assert.Null(country_response_from_get_method);
    }

    [Fact]
    //If we supply a valid country id, it should return the matching country details as CountryResponse Object

    public async Task GetCountryByCountryId_ValidCountryId(){
        //Arrange
        CountryAddRequest? country_add_request=new CountryAddRequest(){CountryName="India"};
        CountryResponse country_response_from_add=await _countriesService.AddCountry(country_add_request);

        //Act
        CountryResponse? country_response_from_get=await _countriesService.GetCountryByCountryId(country_response_from_add.CountryId);

        //Assert
        Assert.Equal(country_response_from_add,country_response_from_get);


    }
    #endregion
}
