using denunciadosWeb.Data;
using denunciadosWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace denunciadosWeb.Controllers;

public class ComplaintController : Controller
{
    private readonly ComplaintRepository _complaintRepository;

    public ComplaintController(ComplaintRepository complaintRepository)
    {
        _complaintRepository = complaintRepository;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View("~/Views/Home/Index.cshtml", new List<Complaint>());
    }

    [HttpPost]
    public async Task<IActionResult> Search(
        ComplaintFilterViewModel complaintFilterViewModel)
    {
        var complaints = await _complaintRepository
            .GetComplaintsByFiltersAsync(complaintFilterViewModel);

        var crimeTypes = await _complaintRepository.GetTypeOfCrime();
        var locationTypes = await _complaintRepository.GetLocationTypeAsync();
        var complaintStatuses = await _complaintRepository.GetComplaintStatusAsync();
        var municipalities = await _complaintRepository.GetMunicipalityAsync();

        ViewBag.CrimeTypes = crimeTypes;
        ViewBag.LocationTypes = locationTypes;
        ViewBag.ComplaintStatuses = complaintStatuses;
        ViewBag.Municipalities = municipalities;

        return View("~/Views/Home/Index.cshtml", complaints);
    }
}