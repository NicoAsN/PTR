using System;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using PTR.Models;
using System.Data.SqlClient;
using System.Data;

namespace PTR.Controllers
{
    public class AccesoController : Controller
    {

        static string cadena = "Data Source=NICO-NOTE\\SQLEXPRESS;Initial Catalog=DB_PTR;Integrated Security=true"; //acople

        // GET: Acceso
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registrar(Usuario oUsuario)
        {
            bool registrado;
            string mensaje;

            if (oUsuario.primer_nom == null)
            {
                ViewData["mensaje"] = "Ingrese el primer nombre";
                return View();
            }
            else
            {
                if (oUsuario.segundo_nom == null)
                {
                    ViewData["mensaje"] = "Ingrese el segundo nombre";
                    return View();
                }
                else
                {
                    if (oUsuario.primer_ape == null)
                    {
                        ViewData["mensaje"] = "Ingrese el primer apellido";
                        return View();
                    }
                    else
                    {
                        if (oUsuario.contrasenia == null)
                        {
                            ViewData["mensaje"] = "Ingrese la contrasenia";
                            return View();
                        }
                        else
                        {
                            if (oUsuario.contrasenia == oUsuario.confirmar_contrasenia)
                            {
                                oUsuario.contrasenia = ConvertirSha256(oUsuario.contrasenia);

                            }
                            else
                            {
                                ViewData["mensaje"] = "Las contraseñas no coinciden";
                                return View();
                            }
                        }
                    }

                }
            }

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("pa_registrar_usuario", cn);
                cmd.Parameters.AddWithValue("primer_nom", oUsuario.primer_nom);
                cmd.Parameters.AddWithValue("segundo_nom", oUsuario.segundo_nom);
                cmd.Parameters.AddWithValue("primer_ape", oUsuario.primer_ape);
                cmd.Parameters.AddWithValue("segundo_ape", oUsuario.segundo_ape);
                cmd.Parameters.AddWithValue("telefono", oUsuario.telefono);
                cmd.Parameters.AddWithValue("rut", oUsuario.rut);
                cmd.Parameters.AddWithValue("DV", oUsuario.DV);
                cmd.Parameters.AddWithValue("correo", oUsuario.correo);
                cmd.Parameters.AddWithValue("contrasenia", oUsuario.contrasenia);
                cmd.Parameters.Add("registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("mensaje", SqlDbType.VarChar,100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                cmd.ExecuteNonQuery(); 

                registrado = Convert.ToBoolean(cmd.Parameters["registrado"].Value);
                mensaje = cmd.Parameters["mensaje"].Value.ToString();
            }

            ViewData["mensaje"] = mensaje;

            if (registrado)
            {
                return RedirectToAction("Login", "Acceso");
            }
            else
            {
                return View();
            }

        }

        [HttpPost]
        public ActionResult Login(Usuario oUsuario)
        {
            oUsuario.contrasenia = ConvertirSha256(oUsuario.contrasenia);

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("pa_validar_usuario", cn);                
                cmd.Parameters.AddWithValue("Correo", oUsuario.correo);
                cmd.Parameters.AddWithValue("Contrasenia", oUsuario.contrasenia);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                oUsuario.id_usuario = Convert.ToInt32(cmd.ExecuteScalar().ToString());
            }

            if(oUsuario.id_usuario != 0)
            {
                Session["usuario"] = oUsuario;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewData["mensaje"] = "Usuario no encontrado";
                return View();
            }

        }

        public static string ConvertirSha256(string texto)
        {
            //using System.Text;
            //Usar la referencia dem "System.Security.Cryptography"

            StringBuilder Sb = new StringBuilder();
            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(texto));

                foreach(byte b in result)
                    Sb.Append(b.ToString("x2"));
            }
            return Sb.ToString();
        }
    }
}