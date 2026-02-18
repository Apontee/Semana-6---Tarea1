using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Backend.Data;
using Backend.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAntiforgery _antiforgery;
        private readonly ApplicationDbContext _contexto;

        public AuthController(IAntiforgery antiforgery, ApplicationDbContext dataContext)
        {
            _antiforgery = antiforgery;
            _contexto = dataContext;
        }

        // Endpoint para obtener el token de seguridad (CSRF)
        [HttpGet("csrf/token")]
        public IActionResult ObtenerTokenCsrf()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            return Ok(new { token = tokens.RequestToken });
        }

        // Endpoint para registrar un nuevo usuario
        [HttpPost("register")]
        public async Task<IActionResult> RegistrarUsuario([FromBody] LoginDto datos)
        {
            if (await _contexto.Usuarios.AnyAsync(u => u.Email == datos.Email))
            {
                return BadRequest(new { message = "El usuario ya existe" });
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(datos.Password);
            var nuevoUsuario = new Backend.Models.Usuario
            {
                Email = datos.Email,
                Password = passwordHash
            };

            _contexto.Usuarios.Add(nuevoUsuario);
            await _contexto.SaveChangesAsync();

            return Ok(new { message = "Usuario registrado exitosamente" });
        }

        // Endpoint para iniciar sesión
        [HttpPost("login")]
        public async Task<IActionResult> IniciarSesion([FromBody] LoginDto datos)
        {
            // Buscamos el usuario en la base de datos
            var usuario = await _contexto.Usuarios.FirstOrDefaultAsync(u => u.Email == datos.Email);

            // Verificamos si existe y si la contraseña es correcta
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(datos.Password, usuario.Password))
            {
                return Unauthorized(new { message = "Credenciales incorrectas" });
            }

            // Creamos la "Claims" (información del usuario)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email)
            };

            var identidad = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var propiedades = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(20)
            };

            // Iniciamos sesión creando la cookie
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identidad),
                propiedades);

            return Ok(new { message = "Login exitoso", user = usuario.Email });
        }

        // Endpoint para cerrar sesión
        [HttpPost("logout")]
        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { message = "Logout exitoso" });
        }

        // Verificar si el usuario sigue logueado
        [HttpGet("check-session")]
        public IActionResult VerificarSesion()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                // Devolvemos también el nombre/email del usuario para que el frontend pueda sincronizar
                // si localStorage se borró pero la cookie sigue viva (opcional, pero útil)
                var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? User.Identity.Name;
                return Ok(new { authenticated = true, user = userEmail });
            }
            return Unauthorized(new { authenticated = false });
        }
    }
}
