using ClienteMovil.Models;
using ClienteMovil.Services;
using ClienteMovil.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;

namespace ClienteMovil.ViewModel
{
    public class VuelosViewModel:INotifyPropertyChanged
    {
        public ICommand VistaAgregarCommand { get; set; }
        public ICommand GuardarCommand { get; set; }
       public ICommand EditarCommand { get; set; }
        public ICommand ConfirmarCommand { get; set; }
        public ICommand ActualizarCommand { get; set; }
        public Vuelos Vuelo { get; set; } = null;
        public string Errores { get; set; } = "";
        
        public VuelosViewModel()
        {
            ActualizarCommand = new Command(VerVuelos);
            ConfirmarCommand = new Command<Vuelos>(ConfirmarAsync);
            EditarCommand = new Command<Vuelos>(Editar);
            GuardarCommand = new Command(Guardar);
            VistaAgregarCommand = new Command(NuevoVuelo);
           
            VerVuelos();
            service.Error += Service_Error;
        }

        private void Service_Error(List<string> obj)
        {
            Errores = "";
            obj.ForEach(x=>Errores+=x+"\n");
            Actualizar(nameof(Errores));
        }

        private void NuevoVuelo()
        {
            Vuelo = new Vuelos();
            AgregarView agregar = new AgregarView() { BindingContext = this };
            Application.Current.MainPage.Navigation.PushAsync(agregar);
            Errores = "";
            Actualizar(nameof(Errores));
        }
        private async void ConfirmarAsync(Vuelos obj)
        {
            bool option = await Application.Current.MainPage.DisplayAlert("Eliminar", "Seguro de Cancelar este vuelo?", "Si", "No");
            if (option == true)
            {
                Vuelo = obj;
                Eliminar();
            }
        }
      

        public ObservableCollection<Vuelos> Vuelos { get; set; }= new ObservableCollection<Vuelos>();

        readonly VuelosService service = new VuelosService();

        public event PropertyChangedEventHandler PropertyChanged;

        async void VerVuelos()
        {
            Vuelos.Clear();
            var datos = await service.GetVuelos();
            datos.ForEach(v => Vuelos.Add(v));
        }

        private TimeSpan time;
        public TimeSpan Time
        {
            get { return time; }
            set { time = value; Actualizar(nameof(Time)); }
        }
        private DateTime date=DateTime.Now;

        public DateTime Date
        {
            get { return date; }
            set { date = value; Actualizar(nameof(Date)); }
        }
     
        async void Guardar()
        {
            Errores = "";
            Vuelo.HorarioSalida = new DateTime(date.Year, date.Month, date.Day, Time.Hours, Time.Minutes, Time.Seconds);
            if (Vuelo != null)
            {

                if (Validar())
                {


                    if (Vuelo.Id == 0)
                    {
                        if (await service.Insert(Vuelo))
                        {
                            await Application.Current.MainPage.Navigation.PopAsync();
                        }
                    }

                    else
                    {
                        if (await service.Update(Vuelo))
                        {
                            await Application.Current.MainPage.Navigation.PopAsync();
                        }
                    }
                }
                VerVuelos();
              
               
                Actualizar(nameof(Errores));

            }
        }

        bool Validar()
        {
            if (string.IsNullOrWhiteSpace(Vuelo.Destino))
            {
                Errores+= "El destino no puede estar vacio "+"\n";

            }

            if (Vuelo.HorarioSalida < DateTime.Now.AddMinutes(4))
            {
                Errores += "Se debe planear el vuelo con 4 minutos de anticipación " + "\n";
            }
            if (string.IsNullOrWhiteSpace(Vuelo.CodigoVuelo))
            {
                Errores += "El codigo de vuelo no puede estar vacio " + "\n";
             

            }
            if (Vuelo.PuertaSalida == null)
            {
                Errores+="Selecione una puerta de salida " + "\n";
               
            }



            return Errores=="";
        }
      
        void Editar(Vuelos v)
        {
            Errores = "";
            Actualizar(nameof(Errores));
            Vuelo = new Vuelos
            {
                Id = v.Id,
                CodigoVuelo = v.CodigoVuelo,
                Destino = v.Destino,
                PuertaSalida = v.PuertaSalida,
                HorarioSalida = v.HorarioSalida
            };
            date = new DateTime(Vuelo.HorarioSalida.Year, Vuelo.HorarioSalida.Month, Vuelo.HorarioSalida.Day);
            time = new TimeSpan(Vuelo.HorarioSalida.Hour, Vuelo.HorarioSalida.Minute, Vuelo.HorarioSalida.Second);
            EditarView editar = new EditarView() { BindingContext = this };
            Application.Current.MainPage.Navigation.PushAsync(editar);
        }

        async void Eliminar()
        {
            if (Vuelo!=null)
            {
                Vuelo.Estado = "Cancelado";
                if (await service.Update(Vuelo))
                {
                    VerVuelos();

                }
            }
        }
        public void Actualizar(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }




    }
}
