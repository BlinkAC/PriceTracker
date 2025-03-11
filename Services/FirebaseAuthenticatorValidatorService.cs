using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Products3.Interfaces;


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
            var payload = new
            {
                requestType = "VERIFY_EMAIL",
                idToken = idToken
            };

            var jsonPayload = System.Text.Json.JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
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
