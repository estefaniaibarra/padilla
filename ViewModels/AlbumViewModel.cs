using FotoAlbum.Services;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.IO;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Threading;

namespace FotoAlbum.ViewModels
{
    internal class AlbumViewModel:INotifyPropertyChanged
    {
        public ObservableCollection<BitmapImage> Imagenes { get; set; }= new ObservableCollection<BitmapImage>();
        AlbumService server = new AlbumService();

        public event PropertyChangedEventHandler? PropertyChanged;
        public BitmapImage? Seleccionada { get; set; }
        int indice = 0;
        DispatcherTimer timer = new DispatcherTimer();
        Dispatcher principal= Dispatcher.CurrentDispatcher;
        public ICommand IniciarCommand { get; set; }
        public AlbumViewModel()
        {
            var ruta = Environment.CurrentDirectory;
            var files = Directory.GetFiles("assets", "*.png");
            
            foreach (var item in files)
            {
                BitmapImage image = new BitmapImage(new Uri(ruta + "/" + item));
                Imagenes.Add(image);

            }
            Seleccionada = Imagenes.FirstOrDefault();
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Tick += Timer_Tick;
            timer.Start();
            server.ImagenRecibida += Server_ImagenRecibida;

            IniciarCommand = new RelayCommand(() => server.Start());
        }

        private void Server_ImagenRecibida(string obj)
        {
            principal.Invoke(() =>
            {
                var ruta = Environment.CurrentDirectory;
                BitmapImage image = new BitmapImage(new Uri(ruta + "/" + obj));
                Imagenes.Add(image);

                Seleccionada = image;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Seleccionada)));

                timer.Stop();
                timer.Start();
            });
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            indice++;
            if (indice==Imagenes.Count)
            {
                indice = 0;
            }
            Seleccionada = Imagenes.ElementAtOrDefault(indice);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Seleccionada)));
        }
    }
}
