using GymManager.BL.BE;
using GymManager.DL.DALC;
using Microsoft.AspNetCore.Identity;

namespace GymManager.BL.BC
{
    public class UsuarioBC
    {
        private readonly UsuarioDALC _usuarioDALC;
        private readonly PasswordHasher<UsuarioBE> _passwordHasher;

        public UsuarioBC(string connectionString)
        {
            _usuarioDALC = new UsuarioDALC(connectionString);
            _passwordHasher = new PasswordHasher<UsuarioBE>();
        }

        public async Task<(UsuarioBE? Usuario, string? Error)> ValidarCredenciales(
            string email, string password)
        {
            // 1. ¿Existe el usuario?
            var usuario = await _usuarioDALC.ObtenerPorEmail(email);
            if (usuario == null)
                return (null, "El correo electrónico no está registrado.");

            if (!usuario.activo)
                return (null, "Tu cuenta ha sido desactivada. Contacta al administrador.");

            var resultado = _passwordHasher.VerifyHashedPassword(
                usuario, usuario.password_hash, password);

            if (resultado != PasswordVerificationResult.Success)
                return (null, "Contraseña incorrecta.");

            return (usuario, null);
        }


    }
}