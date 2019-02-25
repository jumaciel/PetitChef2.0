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
            var link = "https://pt.petitchef.com/receitas/rapida";
            Paginacao(link);
        }

        private static void Paginacao(string link)
        {
            var doc = GetHtmlNode(link);

            AddReceitaNaLista(doc);

            var nodeNext = doc.SelectSingleNode("//div[@class= 'pages']/span/following-sibling::a[1]");

            if (nodeNext != null)
            {
                var att = nodeNext.GetAttributeValue("href", string.Empty);
                string attLink = "https://pt.petitchef.com" + att;

                Paginacao(attLink);
            }
        }

        public static bool IsReceita(HtmlNode linha)
        {
            var propOrReceita = linha.SelectSingleNode("./p | ./fieldset");

            return propOrReceita == null;
        }

        public static HtmlNode GetHtmlNode(string url)
        {
            var web = new HtmlWeb();
            HtmlDocument htmldoc = web.Load(url);
            HtmlNode htmlNode = htmldoc.DocumentNode;

            return htmlNode;
        }

        public static void AddReceitaNaLista(HtmlNode link)
        {
            var linhas = link.SelectNodes("//div[@class ='i-right']");
            foreach (var linha in linhas)
            {
                if (IsReceita(linha))
                {
                    Receita novaReceita = CriarNovaReceita(linha);
                    listaReceitas.Add(novaReceita);
                }
            }
        }

        public static Receita CriarNovaReceita(HtmlNode linha)
        {
            Receita receita = new Receita();

            GetTitulo(receita, linha);
            GetContemGluten(receita, linha);
            GetAvaliacaoVotos(receita, linha);
            GetIngredientes(receita, linha);
            GetUrl(receita, linha);
            GetPropriedades(receita, linha);
            GetAmeis(receita, linha);
            GetComentarios(receita, linha);

            return receita;
        }

        private static void GetTitulo(Receita receita, HtmlNode linha)
        {
            HtmlNode titulo = linha.SelectSingleNode("./h2/a");
            receita.Titulo = titulo.InnerText;
        }

        private static void GetContemGluten(Receita receita, HtmlNode linha)
        {
            var gluten = linha.SelectSingleNode("./div/img[@title='sem glúten']");

            if (gluten == null) receita.Gluten = true;

            else receita.Gluten = false;
        }

        private static void GetAvaliacaoVotos(Receita receita, HtmlNode linha)
        {
            HtmlNode nota = linha.SelectSingleNode("./div/i[contains(@class, 'note-fa')]");

            if (nota != null)
            {
                var notaVotos = nota.GetAttributeValue("title", string.Empty);
                if (notaVotos.Equals(string.Empty))
                {
                    throw new Exception("Não foi possivel capturar os votos e/ou notas da receita!");
                }

                var texto = Regex.Match(notaVotos, @"(.+)/5 \((.+)( votos)\)");
                receita.Nota = texto.Groups[1].Value;
                receita.Votos = texto.Groups[2].Value;
            }
        }

        private static void GetAmeis(Receita receita, HtmlNode linha)
        {
            var likes = linha.SelectSingleNode("./div[contains(@class,'ir-vote')]/i[contains(@class, 'fa-heart')]/following-sibling::text()");

            receita.Ameis = likes.InnerText;
        }

        private static void GetComentarios(Receita receita, HtmlNode linha)
        {
            var comentarios = linha.SelectSingleNode("./div[contains(@class,'ir-vote')]/i[contains(@class, 'fa-comments')]/following-sibling::text()");
            var rgComentarios = Regex.Match(comentarios.InnerText, @"(\(\d+\))");
            receita.Comentarios = rgComentarios.Value;
        }

        private static void GetIngredientes(Receita receita, HtmlNode linha)
        {
            HtmlNode ingredientes = linha.SelectSingleNode("./div[@class='ingredients']");

            receita.Ingredientes = ingredientes.InnerText;
        }

        private static void GetUrl(Receita receita, HtmlNode linha)
        {
            HtmlNode url = linha.SelectSingleNode("./h2[@class='ir-title']/a");
            var href = url.GetAttributeValue("href", string.Empty);

            if (href.Equals(string.Empty))
            {
                throw new Exception("Não foi possivel capturar o link da receita!");
            }

            receita.Link = href;
        }

        private static void GetPropriedades(Receita receita, HtmlNode linha)
        {
            var propriedades = linha.SelectNodes("./div[@class='prop']/span");

            foreach (var procuraProp in propriedades)
            {
                var text = procuraProp.GetAttributeValue("title", string.Empty);

                if (text.Equals(string.Empty))
                {
                    throw new Exception("Não foi possivel capturar as propriedades da receita!");
                }

                var texto = Regex.Match(text, @"(.+): (.+)");

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

                    case "Calorias":
                        {
                            receita.Calorias = texto.Groups[2].Value;
                            break;
                        }

                    case "Cozedura":
                        {
                            receita.Cozedura = texto.Groups[2].Value;
                            break;
                        }

                    case "Preparação":
                        {
                            receita.Tempo = texto.Groups[2].Value;
                            break;
                        }

                    default:
                        throw new Exception("Existe uma nova propriedade não mapeada.");
                }
            }
        }
    }
}