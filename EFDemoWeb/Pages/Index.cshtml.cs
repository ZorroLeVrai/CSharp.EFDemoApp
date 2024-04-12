using EFDataAccessLibrary.DataAccess;
using EFDataAccessLibrary.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EFDemoWeb.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly PeopleContext _db;

    public IndexModel(ILogger<IndexModel> logger, PeopleContext db)
    {
        _logger = logger;
        _db = db;
    }

    public void OnGet()
    {
        //called everytime we load the index page
        LoadSampleData();

        //get the People data with all addresses and email addresses
        var people = _db.People
            .Include(a => a.Addresses)
            .Include(e => e.EmailAddresses)
            //This will run in C#
            //.Where(x => ApprovedAge(x.Age))
            //This will run on SQL Server
            .Where(x => x.Age >= 18 && x.Age <= 65)
            //the ToList is actually where you execute the query
            .ToList();
    }

    private bool ApprovedAge(int age)
    {
        return (age >= 18 && age <= 65);
    }

    private void LoadSampleData()
    {
        if (_db.People.Count() == 0)
        {
            //if there is no data
            string file = System.IO.File.ReadAllText("generated.json");
            var people = JsonSerializer.Deserialize<List<Person>>(file);
            if (people == null)
                return;

            //add data in the database
            _db.AddRange(people);
            _db.SaveChanges();
        }
    }
}