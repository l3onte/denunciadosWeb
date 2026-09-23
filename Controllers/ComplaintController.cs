using System.Security.Claims;
using denunciadosWeb.Data;
using denunciadosWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace denunciadosWeb.Controllers;

public class ComplaintController : Controller
{
    private readonly ComplaintRepository _complaintRepository;

    public ComplaintController(
        ComplaintRepository complaintRepository)
    {
        _complaintRepository = complaintRepository;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(
            "~/Views/Home/Index.cshtml",
            new List<Complaint>()
        );
    }

    [HttpPost]
    public async Task<IActionResult> Search(
        ComplaintFilterViewModel complaintFilterViewModel)
    {
        var complaints =
            await _complaintRepository.GetComplaintsByFiltersAsync(
                complaintFilterViewModel
            );

        await LoadFilterDataAsync();

        ViewBag.Filters = complaintFilterViewModel;

        return View(
            "~/Views/Home/Index.cshtml",
            complaints
        );
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadCreateDataAsync();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreateComplaintViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadCreateDataAsync();

            return View(model);
        }

        var userIdClaim = User.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        try
        {
            var complaintId =
                await _complaintRepository.CreateComplaintAsync(
                    model,
                    userId
                );

            TempData["SuccessMessage"] =
                $"La denuncia #{complaintId:D6} fue registrada correctamente.";

            return RedirectToAction(
                "Index",
                "Home"
            );
        }
        catch
        {
            ModelState.AddModelError(
                string.Empty,
                "Ocurrió un error al registrar la denuncia."
            );

            await LoadCreateDataAsync();

            return View(model);
        }
    }

    private async Task LoadCreateDataAsync()
    {
        ViewBag.CrimeTypes =
            await _complaintRepository.GetTypeOfCrime();

        ViewBag.LocationTypes =
            await _complaintRepository.GetLocationTypeAsync();

        ViewBag.ComplaintStatuses =
            await _complaintRepository.GetComplaintStatusAsync();

        ViewBag.Locations =
            await _complaintRepository.GetLocationsAsync();
    }

    private async Task LoadFilterDataAsync()
    {
        ViewBag.CrimeTypes =
            await _complaintRepository.GetTypeOfCrime();

        ViewBag.LocationTypes =
            await _complaintRepository.GetLocationTypeAsync();

        ViewBag.ComplaintStatuses =
            await _complaintRepository.GetComplaintStatusAsync();

        ViewBag.Municipalities =
            await _complaintRepository.GetMunicipalityAsync();
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var complaint =
            await _complaintRepository.GetComplaintDetailsAsync(id);

        if (complaint == null)
        {
            return NotFound();
        }

        return View(complaint);
    }
}