using Financiera.Data;
using Financiera.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financiera.BusinessLogic
{
    public class PrestamoBL
    {
        private readonly PrestamoRepositorio prestamoDB;
        private readonly CuotasPrestamoRepositorio cuotaDB;
        private readonly ClienteRespositorio clienteDB;
        private readonly TipoClienteRepositorio tipoClienteDB;

        public PrestamoBL(IConfiguration config)
        {
            prestamoDB = new PrestamoRepositorio(config);
            cuotaDB = new CuotasPrestamoRepositorio(config);
            clienteDB= new ClienteRespositorio(config);
            tipoClienteDB= new TipoClienteRepositorio(config);
        }

        public List<Prestamo> Listar()
        {
            return prestamoDB.Listar();
        }

        public Prestamo ObtenerPorID(int id)
        {
            return prestamoDB.ObtenerPorID(id);
        }

        public int Registrar(Prestamo prestamo)
        {   //  CREAR EXCEPCIONES PERSONALIZADAS
            //EL PLAZO MINIMO DEBE SER DE 6 MESES
            if (prestamo.Plazo < 6)
            {   //propagar una excepcion 
                throw new Exception("El plazo minimo es de 6 meses");
            }

            //BUSCAR AL CLIENTE
            Cliente cliente=clienteDB.ObtenerPorID(prestamo.ClienteID);
            if (cliente == null)
            { 
               throw new Exception("El cliente no existe");
            }
            
            //BUSCAR EL TIPO DE CLIENTE
            TipoCliente tipoCliente=tipoClienteDB.ObtenerPorID(cliente.TipoClienteID);
            if (tipoCliente == null)
            {
                throw new Exception("El tipo de cliente no existe");
            }


            // SI EL CLIENTE ES INDIVIDUAL EL PLAZO MINIMO PARA UN PRESTAMO ES DE 24 MESES
            if (tipoCliente.Nombre.Contains("INDIVIDUAL")&& prestamo.Plazo<24)
            {
                throw new Exception("El plazo minimo para el tipo de cliente asociado es de 24 meses");
            }

            //si el cliente es coorporativo y el prestamo es de tipo mi negocio,entonces se le asigna un 3% menos adcional



            //  REGISTRO DEL PRESTAMO
            int nuevoID = prestamoDB.Registrar(prestamo);

            //REGISTRO DE LAS CUOTAS
            //CUOTA FIJA MENSUAL
            decimal cuotaMensual = prestamo.Importe / prestamo.Plazo;
            decimal porcentajeInteres = prestamo.Tasa / 100;
            decimal importeInteres= cuotaMensual * porcentajeInteres;
            CuotaPrestamo cuota;
            for (int idx = 0; idx <= prestamo.Plazo; idx++)
            {
                cuota = new CuotaPrestamo()
                {
                    PrestamoID = nuevoID,
                    NumeroCuota=idx,
                    Importe=cuotaMensual,   
                    ImporteInteres=importeInteres,
                    Estado="P",
                    FechaPago=prestamo.FechaDeposito.AddMonths(idx)
                };
                cuotaDB.Registrar(cuota);
            }


            return nuevoID;
        }
    }
}
