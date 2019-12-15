using Finder.Helpers;
using Finder.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Finder
{
    class Program
    {
        // Urls de consulta
        private static string GoogleUrlQuery = "https://www.google.com/search?q=";
        private static string YahooUrlQuery = "https://search.yahoo.com/search?p=";

        // Expresiones Regulares
        private static string GoogleRegex = "<div id=\"resultStats\">(.*?)<nobr>";
        private static string YahooRegex = @">([,\d]+) results</span>";

        // Existirán dos tipos de busquedas, las que van encerradas entre comillas y las que no.
        private static string FindTypeRegex = "^\".*\"$";

        static List<FindResponse> GetResultadosBusqueda( IEnumerable<string> PalabrasParaBuscar)
        {
            List<FindResponse> ResultadoBusquedas = new List<FindResponse>();
            foreach (string Palabra in PalabrasParaBuscar)
            {
                // Se guardará dentro de una lista el resultado de busqueda de cada palabra
                // realizado en dos motores de busqueda
                var Modelo = new FindResponse
                {
                    Keyword = Palabra,
                    GoogleResults = 0,
                    YahooResults = 0
                };


                // Busqueda de Google
                Modelo.GoogleResults = GetResult(Palabra, "GOOGLE");

                // Busqueda de Yahoo
                Modelo.YahooResults = GetResult(Palabra, "YAHOO");

                ResultadoBusquedas.Add(Modelo);
            }

            return ResultadoBusquedas;
        }
        static void Main(string[] args)
        {
            
            bool Retry = false;
            do
            {
                Console.WriteLine("BUSQUEDA DE RESULTADOS...");

                Console.Write("Ingresa lo que deseas buscar: ");

                // Primero leemos lo que desea buscar el usuario.
                string texto = Console.ReadLine();

                // Luego de esto, extraemos el contenido previo a buscar
                var ArrayPalabras = GetPalabrasArray(texto);

                // Con el Array de palabras, procedemos a realizar la busqueda en Google y Yahoo.
                var Resultado = GetResultadosBusqueda(ArrayPalabras);

                if (Resultado.Count > 0)
                {
                    PrintResults(Resultado);
                    PrintFinalWinner(Resultado);
                }
                else
                    Console.WriteLine("NO SE ENCONTRARON RESULTADOS..");
                

                Console.Write("¿Deseas realizar otra busqueda? [Y/n]: ");

                ConsoleKeyInfo CKey = Console.ReadKey();
                if (CKey.Key == ConsoleKey.Y)
                {
                    Console.Clear();
                    Retry = true;
                }
                else if (CKey.Key == ConsoleKey.N)
                {
                    Console.Clear();
                    Console.WriteLine("GRACIAS POR UTILIZAR By Cristopher Castillo C. :D.");

                    Thread.Sleep(2000);
                    Retry = false;
                }
                else
                {
                    Console.Clear();
                    Retry = true;
                }

            }
            while (Retry);
        }

        static void PrintResults( List<FindResponse> Resultado)
        {
            foreach (FindResponse Model in Resultado)
            {
                Console.WriteLine();
                Console.WriteLine("*********************************");
                Console.WriteLine("PALABRA: " + Model.Keyword);
                Console.WriteLine("*********RESULTADOS*********");
                Console.WriteLine("GOOGLE: " + Model.GoogleResults);
                Console.WriteLine("YAHOO: " + Model.YahooResults);

                string Winner = "EMPATE";
                if (Model.YahooResults > Model.GoogleResults)
                    Winner = "YAHOO";

                else if (Model.GoogleResults > Model.YahooResults)
                    Winner = "GOOGLE";

                Console.WriteLine("*********************************");

                Console.WriteLine("GANADOR: " + Winner);

                Console.WriteLine("*********************************");
            }
        }
        static List<string> GetPalabrasArray( string texto )
        {
            List<string> PalabrasParaBuscar = new List<string>();

            // El primer método de búsqueda refiere al texto que ingresamos a consola y está encerrado en comillas
            // por ejemplo: "Vehiculos baratos y economicos en Perú"

            if (Regex.IsMatch(texto, FindTypeRegex))
            {
                // Removemos las decimales del contenido previo a buscar 
                // y lo añadimos al array de búsqueda
                string NuevoContenido = Utils.RemoveChar(texto, '\"');
                PalabrasParaBuscar.Add(NuevoContenido);
            }
            else
            {
                // El segundo método de busqueda, es hacerlo con cada palabra. Para ello nos basamos en los espacios
                // para separar cada palabra
                PalabrasParaBuscar = texto.Split(' ').TakeWhile(x => !string.IsNullOrWhiteSpace(x)).ToList();
            }

            return PalabrasParaBuscar;
        }
        static void PrintFinalWinner( List<FindResponse> Resultado)
        {
            // Este mensaje sólo se mostraría en caso de que se esté realizando la busqueda de dos o más palabras.
            if (Resultado.Count <= 1)
                return;

            string FinalWinner = "NINGUNO";

            var GanadorGoogle = Resultado.OrderByDescending(x => x.GoogleResults).FirstOrDefault();
            var GanadorYahoo = Resultado.OrderByDescending(x => x.YahooResults).FirstOrDefault();

            if (GanadorGoogle.GoogleResults > GanadorYahoo.YahooResults)
                FinalWinner = GanadorGoogle.Keyword;

            else if (GanadorGoogle.GoogleResults < GanadorYahoo.YahooResults)
                FinalWinner = GanadorYahoo.Keyword;

            Console.WriteLine();
            Console.WriteLine("GANADOR FINAL: " + FinalWinner);
            Console.WriteLine();
        }
        static long GetResult( string Keyword, string SearchEngine)
        {
            
            // se crean dos variables para verificar con que motor se trabajará
            // y a partir de allí, elegir que Url y qué expresion regular utilizar.

            string QueryUrl = "";
            // REGular EXpresion
            string RegEx = "";
            
            // Haciendo esto por separado causaria código espaguetti
            if (SearchEngine == "GOOGLE")
            {
                QueryUrl = GoogleUrlQuery;
                RegEx = GoogleRegex;
            }
                
            else if (SearchEngine == "YAHOO")
            {
                QueryUrl = YahooUrlQuery;
                RegEx = YahooRegex;
            }
                
            // Realizamos una simple peticion HTTP
            string Url = string.Concat(QueryUrl, Keyword);
            string HtmlCode = HttpQuery.Get(Url);

            // de la cual, a través de la expresión regular extraemos la etiqueta
            // que nos provee de la información de cuantos resultados genera esa busqueda.

            string Informacion = ExpresionesRegulares.ExtractValue(HtmlCode, RegEx);
            
            // debido a que esa información contiene caracteres y letras, solo dejamos los numeros.
            long NumberResult = ExpresionesRegulares.ExtractNumbers(Informacion);
            return NumberResult;
        }
    }
}
