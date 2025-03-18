using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Products3.Interfaces;
using Products3.Models.Authentication;


namespace Products3.Services
{
    public class FirebaseAuthenticatorValidatorService : IFirebaseAuthenticatorValidatorService
    {
        private readonly HttpClient _httpClient;
        private readonly string? _firebaseAppKey;
        public FirebaseAuthenticatorValidatorService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _firebaseAppKey = configuration["Config:FirebaseApiKey"];

        }
        public async Task SendVerificationEmail(string idToken)
        {

            var bodyContent = new MailVerificationModel()
            {
                IdToken = idToken,
                RequestType = "VERIFY_EMAIL"
            };
            var objAsJson = JsonConvert.SerializeObject(bodyContent);
            var content = new StringContent(objAsJson, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_httpClient.BaseAddress + $"?key={_firebaseAppKey}", content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Correo de verificación enviado exitosamente.");
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error al enviar el correo: {error}");
            }
        }
    }
}
