using CLICON_EUREKABANK.Modelo;
using CLICON_EUREKABANK.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CLICON_EUREKABANK.Controlador
{
    internal class LoginController
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
