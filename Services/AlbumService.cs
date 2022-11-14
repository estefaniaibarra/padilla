using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace FotoAlbum.Services
{
    public class AlbumService
    {
        HttpListener server= new HttpListener();
        public void Start()
        {
            if (!server.IsListening)
            {
                server.Prefixes.Add("http://*:9876/album/");
                server.Start();
                Escuchar();
              
            }
        }
        void Escuchar()
        {
            Thread peticion= new Thread(new ThreadStart(Atender));
            peticion.IsBackground = true;
            peticion.Start();
        }
        public event Action<string> ImagenRecibida;
        void Atender()
        {
            var context = server.GetContext();
            Escuchar();

            //Atender
            if (context.Request.Url.LocalPath == "/album/")
            {
                byte[] buffer = File.ReadAllBytes("Assets/HTMLPage1.html");
                context.Response.ContentType = "text/html";
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
             
                context.Response.StatusCode = 200;
            }
            else if (context.Request.Url.LocalPath=="/album/imagen" && context.Request.HttpMethod=="POST")
            {
                var stream = new StreamReader(context.Request.InputStream);
                var data = HttpUtility.UrlDecode(stream.ReadToEnd());
                //Tomar solo lo que este despues del MIME type

                var imageb64 = data.Substring(data.IndexOf(',')+1);
               byte[] buffer = Convert.FromBase64String(imageb64);
                File.WriteAllBytes($"assets/imagen_{DateTime.Now.ToString("ddMMyyyyhhmmss")}.png",buffer);

                string ruta = $"assets/imagen_{DateTime.Now.ToString("ddMMyyyyhhmmss")}.png";
                File.WriteAllBytes(ruta, buffer);
                ImagenRecibida?.Invoke(ruta);

                context.Response.StatusCode = 200;//estado de proceso OK
                context.Response.Redirect("/album/");//redirecciona a la pagina principal
            }
            else
            {
                context.Response.StatusCode = 404;
            }
            context.Response.Close();




        }
    }
}
