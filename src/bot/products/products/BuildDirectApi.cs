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
        private const string ApiKey = "4df1c1eabd384ddab759ca6cad9e5a14";

        public async Task<IEnumerable<SearchProduct>> GetProducts(string term, int? page = 1, int? pageSize = 100)
        {
            var searchResult = await ProductSearch(term, page, pageSize);
            return searchResult.Data.Products;
        }

        public async Task<SearchData> GetFullProductSearch(string term, int? page = 1, int? pageSize = 100)
        {
            var searchResult = await ProductSearch(term, page, pageSize);
            return searchResult.Data;
        }

        public async Task<ProductData> GetProductDetails(string sku)
        {
            var product = await GetProduct(sku);
            return product.Data;
        }

        private static async Task<SearchResult> ProductSearch(string term, int? page = 1, int? pageSize = 100)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ApiKey);

            // Request parameters
            queryString["page"] = page.ToString();
            queryString["pagesize"] = pageSize.ToString();
            var uri = "https://api.builddirect.io/products/?query=" + term + "&" + queryString;

            var response = await client.GetAsync(uri);
            var json = await response.Content.ReadAsStringAsync();
            var searchResult = new JavaScriptSerializer().Deserialize<SearchResult>(json);
            return searchResult;
        }

        private static async Task<ProductResult> GetProduct(string sku)
        {
            var client = new HttpClient();
            
            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ApiKey);

            // Request parameters
            var uri = "https://api.builddirect.io/products/" + sku;

            var response = await client.GetAsync(uri);
            var json = await response.Content.ReadAsStringAsync();
            var productResult = new JavaScriptSerializer().Deserialize<ProductResult>(json);
            return productResult;
        }
    }
}