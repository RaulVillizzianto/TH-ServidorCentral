using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmaApi.Comunicacion
{
    public sealed class Arduino
    {
        private const string PUERTO_SERVIDOR = "COM5";
        public  SerialPort? _serialPort;
        public bool trabajando = true;
        public List<string> respuestasArduino = new List<string>();
        public const int ARDUINO_RESPONSE_TIMEOUT = 30;

        private Arduino()
        {
            List<string> puertos = ObtenerPuertos();
            try
            {
                _serialPort = InicializarPuerto(PUERTO_SERVIDOR);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            if (_serialPort.IsOpen)
            {
                _serialPort.DiscardInBuffer();
                _serialPort.DiscardOutBuffer();
                Thread readThread = new Thread(ReadSerialPort);
                readThread.Start();
            }
        }
        private static Arduino _instance;
        public static Arduino GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Arduino();
            }
            return _instance;
        }

        /* * <codigomensaje>|<nombredispositivo>,<parametria> */

        private void ReadSerialPort()
        {
            while(trabajando)
            {
                string linea = _serialPort.ReadLine();
                linea = RemoveSpecialCharacters(linea);
                if (String.IsNullOrEmpty(linea) || String.IsNullOrWhiteSpace(linea))
                {
                    continue;
                }
                Console.WriteLine($"{DateTime.Now.ToString()} Recibido => " + linea);

                if (linea.Contains('|') && !linea.Contains("DEBUG"))
                {
                    string informacion = linea;
                    var data = informacion.Split('|');
                    if (data.First() != null)
                    {
                        int codigoMensaje = Int32.Parse(data.First());
                        var parametros = data.ElementAt(1).Split(',');
                        string idDispositivo = parametros.First();
                        if (!String.IsNullOrEmpty(idDispositivo))
                        {
                            switch (codigoMensaje)
                            {
                                case (int)Procesamiento.Mensajes.Eventos.HEARTBEAT:
                                    {
                                        Procesamiento.Mensajes.HeartBeat(idDispositivo);
                                        break;
                                    }
                                case (int)Procesamiento.Mensajes.Eventos.SENSOR_MOVIMIENTO_ACTIVADO:
                                    {
                                        Procesamiento.Mensajes.SensorMovimientoActivado(idDispositivo);
                                        break;
                                    }
                                case (int)Procesamiento.Mensajes.Eventos.OBTENER_ESTADO_SENSOR_MOVIMIENTO:
                                    {
                                        Procesamiento.Mensajes.ActualizarEstadoSensorMovimiento(idDispositivo, Int32.Parse(parametros.ElementAt(1)));
                                        break;
                                    }
                            }
                        }
                    }
                }
                else if(linea.Contains("DEBUG:"))
                {
                    Console.WriteLine($"{DateTime.Now.ToString()} Desde Arduino: {linea}");
                }
                else if(linea.ToUpper().CompareTo("TRUE") == 0 || linea.ToUpper().CompareTo("FALSE") == 0)  respuestasArduino.Add(linea);
                Thread.Sleep(1000);
            }
        }

        public string BloquearHastaObtenerRespuesta()
        {
            int cantidadRespuestasArduino = respuestasArduino.Count() + 1;
            Int32 timeout = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds + ARDUINO_RESPONSE_TIMEOUT;
            
            while (respuestasArduino.Count() < cantidadRespuestasArduino)
            {
                if((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds > timeout)
                {
                    Console.WriteLine("timeout arduino");
                    return "false";
                }
                else Thread.Sleep(100);
            }
            var respuesta = respuestasArduino.Last().ToString();
            respuestasArduino.RemoveAt(respuestasArduino.Count() - 1);
            return respuesta;
        }

        public void HeartBeat(Comunicacion.Models.Dispositivo dispositivo)
        {
            Procesamiento.Comandos.HeartBeat(this, dispositivo);
        }
        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == ',' || c == ':' || c == '_' || c == '|')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        private static List<string> ObtenerPuertos()
        {
            return SerialPort.GetPortNames().ToList();
        }
        private SerialPort InicializarPuerto(string id)
        {
            _serialPort = new SerialPort();
            _serialPort.PortName = id;
            _serialPort.BaudRate = 9600;
          //  _serialPort.NewLine = "\n";
            _serialPort.ReceivedBytesThreshold = 500000;
            _serialPort.ReadBufferSize = 1048576;
            _serialPort.Open();
            return _serialPort;
        }
    }
}
