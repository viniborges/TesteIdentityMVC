using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace ByteBank.Forum.App_Start.Identity
{
    public class SenhaValidador : IIdentityValidator<string>
    {
        public int TamanhaRequerido { get; set; }

        public bool CaracteresEspeciais { get; set; }

        public bool CaractereLowerCase { get; set; }

        public bool CaractereUpperCase { get; set; }
        public bool Digitos { get; set; }

        public async Task<IdentityResult> ValidateAsync(string item)
        {

            var erros = new List<string>();
            if (CaracteresEspeciais && !VerificaCaaracteresEspeciais(item))
                erros.Add("A senha deve conter  caracteres especiais!");

            if (!VerificaTamanhoRequerido(item))
                erros.Add($"A senha deve conter nominimo {TamanhaRequerido}  caracteres!");

            if (CaractereLowerCase && !VerificaLowerCase(item))
                erros.Add("A senha deve conter no minimo uma letra minusculo");

            if (CaractereUpperCase && !VerificaUpperCase(item))
                erros.Add("A senha deve conter no minimo uma letra maiuscula");

            if (Digitos && !VerificaDigito(item))
                erros.Add("A senha deve conter no minimo um digito");

            if (erros.Any())
                return IdentityResult.Failed(erros.ToArray());
            else
                return IdentityResult.Success;
        }

        private bool VerificaTamanhoRequerido(string senha) =>
            senha?.Length >= TamanhaRequerido;

        private bool VerificaCaaracteresEspeciais(string senha) =>
                Regex.IsMatch(senha, "//[^a-zA-Z0-9]+//g");

        private bool VerificaLowerCase(string senha) =>
            senha.Any(char.IsLower);

        private bool VerificaUpperCase(string senha) =>
            senha.Any(char.IsUpper);

        private bool VerificaDigito(string senha) =>
            senha.Any(char.IsDigit);
    }
}