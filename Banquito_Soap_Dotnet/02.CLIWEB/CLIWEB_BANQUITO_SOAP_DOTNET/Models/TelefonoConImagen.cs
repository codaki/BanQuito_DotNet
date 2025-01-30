namespace CLIWEB_BANQUITO_SOAP_DOTNET.Models
{
    public class TelefonoConImagen
    {
        public int COD_TEL { get; set; }
        public string NOMBRE { get; set; }
        public decimal PRECIO { get; set; }
        public string FOTO { get; set; }
        public string FotoImage { get; set; }

        // Constructor que mapea un objeto de tipo Telefono a TelefonoConImagen
        public TelefonoConImagen(WSBANQUITO.Telefono telefono)
        {
            // Mapea las propiedades de Telefono a TelefonoConImagen
            COD_TEL = telefono.COD_TEL;
            NOMBRE = telefono.NOMBRE;
            PRECIO = telefono.PRECIO;
            FOTO = telefono.FOTO; // Asegúrate de que FOTO es el campo correcto
            FotoImage = GetImageSourceFromBase64(telefono.FOTO); // Convierte la imagen Base64 en Data URI
        }

        // Convierte Base64 en un URI de imagen
        private string GetImageSourceFromBase64(string fotoBase64)
        {
            if (!string.IsNullOrEmpty(fotoBase64))
            {
                return $"data:image/jpeg;base64,{fotoBase64}";
            }
            return string.Empty;
        }
    }
}
