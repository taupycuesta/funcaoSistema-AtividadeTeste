using FI.AtividadeEntrevista.BLL;
using WebAtividadeEntrevista.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FI.AtividadeEntrevista.DML;

namespace WebAtividadeEntrevista.Controllers
{
    public class ClienteController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Incluir()
        {
            return View();
        }


        private bool validaCpf(string CPF = "")
        {
            CPF = String.Join("", System.Text.RegularExpressions.Regex.Split(CPF, @"[^\d]"));
            string cpfAux;
            int i , soma = 0;

            if (CPF != "" && CPF != null && CPF.Length == 11 )
            {
                cpfAux = CPF.Substring(0, 9); 

                if (CPF.Equals("00000000000") ||!CPF.Equals("11111111111") ||!CPF.Equals("22222222222") || !CPF.Equals("33333333333") || !CPF.Equals("44444444444") || !CPF.Equals("55555555555") ||!CPF.Equals("66666666666") || !CPF.Equals("77777777777") ||
                !CPF.Equals("88888888888") || !CPF.Equals("99999999999"))
                {

                    //Primeiro dígito
                    for (i = 10; i >= 2; i--)
                    {
                        soma += (Int32.Parse(cpfAux[10 - i].ToString()) * i);
                    }
                    cpfAux += retornaDigito(soma);

                    soma = 0;

                    //Segundo dígito
                    for (i = 11; i >= 2; i--)
                    {
                        soma += (Int32.Parse(CPF[11 - i].ToString()) * i);
                    }
                    cpfAux += retornaDigito(soma);

                    //Comparação Digitos calculados e CPF digitados
                    if (cpfAux.Equals(CPF))
                    {
                        return true;
                    }
                }
            }

            return false;

            
        }


        private string retornaDigito(int soma)
        {
            int digito, resto;

            resto = soma % 11;

            if (resto < 2)
            {
                digito = 0;
            }
            else
            {
                digito = (11 - resto);
            }

            return digito.ToString();
        }

        [HttpPost]
        public JsonResult Incluir(ClienteModel model)
        {
            BoCliente bo = new BoCliente();
            
            if (!this.ModelState.IsValid && validaCpf(model.CPF))
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                
                model.Id = bo.Incluir(new Cliente()
                {                    
                    CEP = model.CEP,
                    CPF = model.CPF,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    Telefone = model.Telefone
                });

           
                return Json("Cadastro efetuado com sucesso");
            }
        }

        [HttpPost]
        public JsonResult Alterar(ClienteModel model)
        {
            BoCliente bo = new BoCliente();
       
            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                bo.Alterar(new Cliente()
                {
                    Id = model.Id,
                    CEP = model.CEP,
                    CPF = model.CPF,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    Telefone = model.Telefone
                });
                               
                return Json("Cadastro alterado com sucesso");
            }
        }

        [HttpGet]
        public ActionResult Alterar(long id)
        {
            BoCliente bo = new BoCliente();
            Cliente cliente = bo.Consultar(id);
            Models.ClienteModel model = null;

            if (cliente != null)
            {
                model = new ClienteModel()
                {
                    Id = cliente.Id,
                    CEP = cliente.CEP,
                    CPF = model.CPF,
                    Cidade = cliente.Cidade,
                    Email = cliente.Email,
                    Estado = cliente.Estado,
                    Logradouro = cliente.Logradouro,
                    Nacionalidade = cliente.Nacionalidade,
                    Nome = cliente.Nome,
                    Sobrenome = cliente.Sobrenome,
                    Telefone = cliente.Telefone
                };

            
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult ClienteList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int qtd = 0;
                string campo = string.Empty;
                string crescente = string.Empty;
                string[] array = jtSorting.Split(' ');

                if (array.Length > 0)
                    campo = array[0];

                if (array.Length > 1)
                    crescente = array[1];

                List<Cliente> clientes = new BoCliente().Pesquisa(jtStartIndex, jtPageSize, campo, crescente.Equals("ASC", StringComparison.InvariantCultureIgnoreCase), out qtd);

                //Return result to jTable
                return Json(new { Result = "OK", Records = clientes, TotalRecordCount = qtd });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
    }
}