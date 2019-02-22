using HtmlAgilityPack;
using PetitChef.Models;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PetitChef
{
    class Program
    {
        public static List<Receita> listaReceitas = new List<Receita>();

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var link = "https://pt.petitchef.com/receitas/rapida";
            Pagina(link);

        }

        public static Receita CriarNovaReceita(HtmlNode linha)
        {
            Receita receita = new Receita();
            
            var propriedades = linha.SelectNodes("./div[@class='prop']/span");

            foreach (var procuraProp in propriedades)
            {
                var text = procuraProp.GetAttributeValue("title", string.Empty);

                if (text.Equals(string.Empty))
                {
                    throw new Exception("Não foi possivel capturar as propriedades da receita!");
                }

                var texto= Regex.Match(text, @"(.+): (.+)");
           

                switch (texto.Groups[1].Value)
                {
                    case "Tipo de receita":
                        {
                            receita.Tipo = texto.Groups[2].Value;
                            break;
                        }
                    case "Dificuldade":
                        {
                            receita.Dificuldade = texto.Groups[2].Value;
                            break;
                        }
                    case "Pronto em":
                        {
                            receita.Tempo = texto.Groups[2].Value;
                            break;
                        }
                    case "Calorias:":
                        {
                            receita.Calorias = texto.Groups[2].Value;
                            break;
                        }
                    case "Cozedura":
                        {
                            receita.Cozedura = texto.Groups[2].Value;
                            break;
                        }

                    default:
                        throw new Exception("Existe uma nova propriedade não mapeada.");

                }

            }




            //HtmlNode gluten;
            //HtmlNode titulo = linha.SelectSingleNode("./h2/a");
            //HtmlNode nota = linha.SelectSingleNode("");
            //HtmlNode votos = linha.SelectSingleNode("");
            //HtmlNode comentarios = linha.SelectSingleNode("");
            //HtmlNode ameis = linha.SelectSingleNode("");
            //HtmlNode link = linha.SelectSingleNode("./span[6]");
            //HtmlNode ingredientes = linha.SelectSingleNode("./div[@class ='i - right']/div[@class='ingredients']");

            return receita;
        }

        public static HtmlNode GetHtmlNode(string href)
        {
            var web = new HtmlWeb();
            HtmlDocument htmldoc = web.Load(href);
            HtmlNode htmlNode = htmldoc.DocumentNode;

            return htmlNode;
        }

        public static void AddReceitaNaLista(HtmlNode link)
        {
            var linhas = link.SelectNodes("//div[@class ='i-right']");
            foreach (var linha in linhas)
            {
                Receita novaReceita = CriarNovaReceita(linha);
                
                listaReceitas.Add(novaReceita);
            }

        }
        private static void Pagina(string link)
        {
            var doc = GetHtmlNode(link);

            AddReceitaNaLista(doc);

            var nodeNext = doc.SelectSingleNode("//a[@rel='next']");

            if (nodeNext != null)
            {
                var att = nodeNext.GetAttributeValue("href", string.Empty);
                string attLink = "https://www.worldwildlife.org" + att;

                Pagina(attLink);
            }
        }

    }


}

