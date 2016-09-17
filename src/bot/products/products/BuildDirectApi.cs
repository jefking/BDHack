using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using products.Models;

namespace products
{
    public class BuildDirectApi
    {
        public async Task<IEnumerable<SearchProduct>>  GetProducts(string term, int? page = 1, int? pageSize = 100)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "4df1c1eabd384ddab759ca6cad9e5a14");

            // Request parameters
            queryString["page"] = page.ToString();
            queryString["pagesize"] = pageSize.ToString();
            var uri = "https://api.builddirect.io/products/?query=" + term + "&" + queryString;

            var response = await client.GetAsync(uri);
            var json = await response.Content.ReadAsStringAsync();
            var searchResult = new JavaScriptSerializer().Deserialize<SearchResult>(json);
            return searchResult.Data.Products;
        }
    }
}