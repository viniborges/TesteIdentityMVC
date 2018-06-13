using ByteBank.Forum.Models;
using ByteBank.Forum.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ByteBank.Forum.Controllers
{
    public class ContaController : Controller
    {
        private UserManager<UsuarioAplicacao> _userManager;
        public UserManager<UsuarioAplicacao> UserManger
        {
            get
            {
                if (_userManager == null)
                {
                    var contextOwin = HttpContext.GetOwinContext();
                    _userManager = contextOwin.GetUserManager<UserManager<UsuarioAplicacao>>();
                }
                return _userManager;
            }
            set { _userManager = value; }
        }

        public ActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Registrar(ContaRegistrarViewModel modelo)
        {
            if (ModelState.IsValid)
            {
                //incluir usuário
                //var dbContext = new IdentityDbContext<UsuarioAplicacao>("DefaultConnection");
                //var userStorie = new UserStore<UsuarioAplicacao>(dbContext);
                //var userManager = new UserManager<UsuarioAplicacao>(userStorie);

                var newUser = new UsuarioAplicacao
                {
                    Email = modelo.Email,
                    UserName = modelo.UserName,
                    NomeCompleto = modelo.NomeCompleto
                };

                var usuario = await _userManager.FindByEmailAsync(modelo.Email);

                if (usuario != null)
                {
                    return RedirectToAction("AguardandoConfirmacao");
                }

                var result = await UserManger.CreateAsync(newUser, modelo.Senha);
                //dbContext.Users.Add(newUser);
                //dbContext.SaveChanges();

                if (result.Succeeded)
                {
                    await EnviarEmailConfirmacaoAsync(newUser);
                    return RedirectToAction("AguardandoConfirmacao");
                }
                else
                {
                    AdicionaErros(result);
                }
            }


            return View(modelo);

        }

        private async Task EnviarEmailConfirmacaoAsync(UsuarioAplicacao usuario)
        {
            var token = await UserManger.GenerateEmailConfirmationTokenAsync(usuario.Id);

            var linkCallback = Url.Action(
                "ConfirmacaoEmail",
                "Coanta",
                new { usuarioId = usuario.Id, token = token },
                Request.Url.Scheme);


            await UserManger.SendEmailAsync(
                usuario.Id,
                "Confirmação de e-mail",
                $"Bem vindo. Clique aqui {token}  para confirmar seu e-mail.");

        }

        public async Task<ActionResult> ConfirmacaoEmail(string usuarioId, string token)
        {
            if (usuarioId == null || token == null)
                return View("Error");

            var result = await _userManager.ConfirmEmailAsync(usuarioId, token);

            if(result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
                return View("Error");
        }

        private void AdicionaErros(IdentityResult result)
        {
            foreach (var erro in result.Errors)
            {
                ModelState.AddModelError("", erro);
            }
        }
    }
}