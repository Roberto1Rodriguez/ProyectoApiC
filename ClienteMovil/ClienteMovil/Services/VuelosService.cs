using ClienteMovil.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ClienteMovil.Services
{
   public class VuelosService
    {      
           HttpClient cliente = new HttpClient
            {
                BaseAddress = new Uri("https://equipo9.sistemas19.com/")
            };
        
        public event Action<List<string>> Error;

        public async Task<bool> Insert(Vuelos v)
        {
            var json = JsonConvert.SerializeObject(v);
            var response = await cliente.PostAsync("api/Vuelos", new StringContent(json, Encoding.UTF8, "application/json"));
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest) 
            {
                var errores = await response.Content.ReadAsStringAsync();
                LanzarErrorJson(errores);
                return false;
            }
            return true;
        }
        public async Task<bool> Update(Vuelos v)
        {
            var json = JsonConvert.SerializeObject(v);
            var response = await cliente.PutAsync("api/vuelos", new StringContent(json, Encoding.UTF8,
                "application/json"));
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var errores = await response.Content.ReadAsStringAsync();
                LanzarErrorJson(errores);
                return false;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                LanzarError("No se encontro el vuelo");
            }
            return true;
        }

        public async Task<List<Vuelos>> GetVuelos()
        {
            List<Vuelos> vuelos = null;
            var response = await cliente.GetAsync("api/vuelos");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                vuelos=JsonConvert.DeserializeObject<List<Vuelos>>(json);
            }
            if (vuelos!=null)
            {
                return vuelos;
            }
            else
            {
                return new List<Vuelos> ();
            }
        }
        void LanzarError(string mensaje)
        {
            Error?.Invoke(new List<string> { mensaje });
        }
        void LanzarErrorJson(string json)
        {
            List<string> obj = JsonConvert.DeserializeObject<List<string>>(json);
            if (obj != null)
            {
                Error?.Invoke(obj);
            }
        }
    }
}
