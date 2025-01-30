using CLIESC_EUREKABANK.Modelo;
using CLIESC_EUREKABANK.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLIESC_EUREKABANK.Controlador
{
    public class LoginController
    {
        BanquitoServices client;
        UserModel model;
        public LoginController()
        {
            client = new BanquitoServices();
            model = new UserModel();
        }

        public async Task<bool> validarCredenciales(UserModel user)
        {
            model = user;
            bool succesfull = false;
            if (model != null)
            {
                succesfull = await client.loginService(model);
            }

            return succesfull;

        }

    }
}
