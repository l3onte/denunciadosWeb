using System.Security.Claims;
using denunciadosWeb.Data;
using denunciadosWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Authentication.Cookies;
using static System.Net.WebRequestMethods;
using Microsoft.AspNetCore.Authentication;

namespace denunciadosWeb.Controllers;

public class AccountController : Controller
{
    private readonly UserRepository _userRepository;

    public AccountController(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> LoginValidation(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Login", model);
        }

        var user = await _userRepository.LoginUserAsync(
            model.Name,
            model.Password
        );

        if (user == null)
        {
            ModelState.AddModelError(
                string.Empty,
                "Usuario o contrasenia incorrectas."
            );

            return View("Login", model);
        }

        var claims = new List<Claim>
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()
            ),

            new Claim(
                ClaimTypes.Name,
                user.Name
            ),

            new Claim(
                ClaimTypes.Role,
                user.Role?.Name ?? String.Empty
            )
        };

        var identify = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        var principal = new ClaimsPrincipal(identify);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal
        );

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        return RedirectToAction(nameof(Login));
    }
}
