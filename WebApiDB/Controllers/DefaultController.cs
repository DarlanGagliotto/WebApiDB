using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiDB.Models;

namespace WebApiDB.Controllers
{
    [RoutePrefix("api/meuprojeto")]
    public class DefaultController : ApiController
    {

        private string ConnectionString =
            @"Data Source = DARLANDELL; Initial Catalog = CarSaleDbNew; Persist Security Info=True;User ID = sa;Password=123456";

        [HttpGet]
        [Route("datahora/consulta")]
        public HttpResponseMessage GetDataHoraServidor()
        {
            try
            {
                var dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                return Request.CreateResponse(HttpStatusCode.OK, dataHora);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [HttpGet]
        [Route("consulta/cliente/{id:int}")]
        public HttpResponseMessage GetClientePorId(int id)
        {
            try
            {
                var clientes = new[] {
                    new { Id = 1, Nome = "Pedro", DataNascimento = new DateTime(1954, 2, 1) },
                    new { Id = 2, Nome = "Paulo", DataNascimento = new DateTime(1944, 4, 12) },
                    new { Id = 3, Nome = "Fernando", DataNascimento = new DateTime(1963, 5, 9) },
                    new { Id = 4, Nome = "Maria", DataNascimento = new DateTime(1984, 4, 30) },
                    new { Id = 5, Nome = "João", DataNascimento = new DateTime(1990, 3, 14) },
                    new { Id = 6, Nome = "Joana", DataNascimento = new DateTime(1974, 6, 19) }
                };

                var cliente = clientes.Where(x => x.Id == id).FirstOrDefault();

                if (cliente == null)
                {
                    throw new Exception("Cliente não encontrado");
                }

                return Request.CreateResponse(HttpStatusCode.OK, cliente);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [HttpGet]
        [Route("clientes/todos")]
        public HttpResponseMessage GetAll()
        {
            try
            {
                List<Cliente> listClientes = new List<Cliente>();

                using (SqlConnection connection = new SqlConnection(this.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "SELECIONAR_TODOS_CLIENTES";//"SELECT * FROM CLIENTES";

                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            Cliente cliente = new Cliente()
                            {
                                ID = reader["ID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ID"]),
                                Nome = reader["NOME"] == DBNull.Value ? string.Empty : reader["NOME"].ToString(),
                                CPF = reader["CPF"] == DBNull.Value ? string.Empty : reader["CPF"].ToString(),
                                Email = reader["EMAIL"] == DBNull.Value ? string.Empty : reader["EMAIL"].ToString(),
                                Ativo = reader["ATIVO"] == DBNull.Value ? false : Convert.ToBoolean(reader["ATIVO"].ToString()),
                                IsPremium = reader["ISPREMIUM"] == DBNull.Value ? false : Convert.ToBoolean(reader["ISPREMIUM"].ToString()),
                                Cidade = reader["CIDADE"] == DBNull.Value ? string.Empty : reader["CIDADE"].ToString(),
                            };

                            listClientes.Add(cliente);
                        }
                    }
                    connection.Close();
                }

                return Request.CreateResponse(HttpStatusCode.OK, listClientes.ToArray());
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [HttpPost]
        [Route("clientes/inserircliente")]
        public HttpResponseMessage PostCLiente(string nome, string cpf, string email, bool ativo, bool isPremium, string cidade)
        {
            bool result = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(this.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "INSERIR_CLIENTE";
                        command.Parameters.AddWithValue("@NOME", nome);
                        command.Parameters.AddWithValue("@CPF", cpf);
                        command.Parameters.AddWithValue("@EMAIL", email);
                        command.Parameters.AddWithValue("@ATIVO", ativo);
                        command.Parameters.AddWithValue("@ISPREMIUM", isPremium);
                        command.Parameters.AddWithValue("@CIDADE", cidade);
                        command.CommandType = CommandType.StoredProcedure;

                        int i = command.ExecuteNonQuery();

                        result = i > 0;
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }
    }
}
