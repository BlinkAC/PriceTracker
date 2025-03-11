using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Products3.Interfaces;

namespace Products3.Services
{
    public class UserNotificationService : IUserNotification
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        public async Task HandleToastNavigationAsync(string message, ToastDuration duration, string navigationPage)
        {
            try
            {
                // Evitar concurrencia usando SemaphoreSlim
                await _semaphore.WaitAsync();

                // Mostrar Toast en el hilo adecuado
                if (!string.IsNullOrEmpty(message))
                {
                    var toast = Toast.Make(message, duration);
                    await toast.Show();
                }

                // Redirigir a la página especificada
                if (!string.IsNullOrEmpty(navigationPage))
                {
                    await Shell.Current.GoToAsync(navigationPage);
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores (log o reintento)
                Console.WriteLine($"Error en la navegación: {ex.Message}");
            }
            finally
            {
                // Liberar el SemaphoreSlim
                _semaphore.Release();
            }

        }
    }
}
