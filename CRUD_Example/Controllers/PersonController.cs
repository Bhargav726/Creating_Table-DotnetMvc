using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
namespace CRUD_Example.Controllers
{
    [Route("[controller]")]
    public class PersonController : Controller
    {
        //private fields
        private readonly IPersonService _personService;
        private readonly ICountriesService _countriesService;

        //constructor
        public PersonController(IPersonService personService, ICountriesService countriesService){
            _personService=personService;
            _countriesService=countriesService;
        }
        // GET: PersonController
        //[Route("person/index")]
        [Route("[action]")]
        [Route("/")]
        public async Task<IActionResult> Index( string searchBy, string? searchString,string sortBy=nameof(PersonResponse.PersonName), SortOrderOptions sortOrder=SortOrderOptions.ASC)
        {
            ViewBag.SearchFields = new Dictionary<string, string>(){
                {nameof(PersonResponse.PersonName),"Name"},
                {nameof(PersonResponse.PersonEmail),"Email"},
                {nameof(PersonResponse.PersonDOB),"DateOfBirth"},
                {nameof(PersonResponse.PersonGender),"Gender"},
                {nameof(PersonResponse.CountryId),"Country"},
                {nameof(PersonResponse.PersonAddress),"Address" }
            };
            List<PersonResponse> person=await _personService.GetFilteredPersons(searchBy,searchString);
            ViewBag.CurrentSearchBy=searchBy;
            ViewBag.CurrentSearchString=searchString;

            //Sort
            List<PersonResponse> sortedPersons=await _personService.GetSortedPersons(person,sortBy,sortOrder);
            ViewBag.CurrentSortBy=sortBy;
            ViewBag.CurrentSortOrder=sortOrder.ToString();
            return View(sortedPersons);  //Views/Person/Index.cshtml
        }

        //Executes when the user clicks on "Create Person" hyperlink(while opening the create view)
        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> Create(){
            /*List<CountryResponse> countries= _countriesService.GetAllCountries();
            ViewBag.Countries =countries.Select(temp => new SelectListItem{Text=temp.CountryName, Value=temp.CountryId.ToString()}).ToList();

            //new SelectListItem() {Text="Bhargav", Value="1"}
            //<option value="1">Harsha</option>*/


            //Fetch all countries from the service
            List<CountryResponse> countries = await _countriesService.GetAllCountries();

            //Convert List<CountryResponse> to List<SelectListItem>
            IEnumerable<SelectListItem> countrySelectList = countries.Select(temp => new SelectListItem()
            {
                Text = temp.CountryName,
                Value = temp.CountryId.ToString()
            }).ToList();

            //Store in ViewBag
            ViewBag.Countries = countrySelectList;
            return View();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Create(PersonAddRequest personAddRequest){
            if(!ModelState.IsValid){
                List<CountryResponse> countries= await _countriesService.GetAllCountries();
           ViewBag.Countries=countries;

           ViewBag.Errors=ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return View();
            }
            //call the service method
           PersonResponse personResponse= await _personService.AddPerson(personAddRequest);
            
            //navigate to index() action method (it makes another get request to "person/index") 
            return RedirectToAction("Index","person");
        }

        [HttpGet]
        [Route("[action]/{personId}")] //Eg: /person/edit/1
        public async Task<IActionResult> Edit(Guid personId){
            PersonResponse? personResponse=await _personService.GetPersonByPersonId(personId);
            if(personResponse==null){
                return RedirectToAction("Index");
            }
            PersonUpdateRequest personUpdateRequest=personResponse.ToPersonUpdateRequest();

            List<CountryResponse> countries=await _countriesService.GetAllCountries();
           ViewBag.Countries=countries.Select(temp => new SelectListItem(){Text=temp.CountryName, Value=temp.CountryId.ToString()});
            return View(personUpdateRequest);
        }

        [HttpPost]
        [Route("[action]/{personId}")]
        public async Task<IActionResult> Edit(PersonUpdateRequest personUpdateRequest){
            PersonResponse? personResponse=await _personService.GetPersonByPersonId(personUpdateRequest.PersonId);

            if(personResponse==null){
                return RedirectToAction("Index");
            }

            if(ModelState.IsValid){
                PersonResponse updatedPerson=await _personService.UpdatePerson(personUpdateRequest);
                return RedirectToAction("Index");
            }
            else{
                List<CountryResponse> countries= await _countriesService.GetAllCountries();
           ViewBag.Countries=countries;

           ViewBag.Errors=ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return View(personResponse.ToPersonUpdateRequest());
            }
        }
    
    [HttpGet]
    [Route("[action]/{personId}")]
    public async Task<IActionResult> Delete(Guid? personId){
        PersonResponse? personResponse=await _personService.GetPersonByPersonId(personId);
        if(personResponse==null)
            return RedirectToAction("Index");
        return View(personResponse);
    }

    [HttpPost]
    [Route("[action]/{personId}")]
    public async Task<IActionResult> Delete(PersonUpdateRequest personUpdateResult){
        PersonResponse? personResponse=await _personService.GetPersonByPersonId(personUpdateResult.PersonId);
        if(personResponse==null){
            return RedirectToAction("Index");
        }
        else
        {
            await _personService.DeletePerson(personUpdateResult.PersonId);
            return RedirectToAction("Index");
        }
    }
        [Route("PersonsPDF")]
        public async Task<IActionResult> PersonsPDF()
        {
            //Get list of persons
            List<PersonResponse> person=await _personService.GetAllPersons();

            //Return view as pdf
            return new ViewAsPdf("PersonPDF",person, ViewData)
            {
                PageMargins=new Rotativa.AspNetCore.Options.Margins()
                {
                    Top=20, Right=20, Bottom=20, Left=20
                }, 
                PageOrientation=Rotativa.AspNetCore.Options.Orientation.Landscape
            };
        }

        [Route("PersonsCSV")]
        public async Task<IActionResult> PersonsCSV()
        {
            MemoryStream memoryStream = await _personService.GetPersonsCSV();
            return File(memoryStream, "application/octet-stream", "person.csv");

        }

        [Route("PersonsExcel")]
        public async Task<IActionResult> PersonsExcel()
        {
            MemoryStream memoryStream = await _personService.GetPersonsExcel();
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "person.xlsx");

        }
    }
}
