using System.Collections.Generic;

namespace products.Models
{
    public class SearchResult
    {
        public SearchData Data { get; set; }
    }

    public class SearchData
    {
        public int TotalRecordCount { get; set; }
        public string Query { get; set; }
        public string OriginalQuery { get; set; }
        public PageInfo PageInfo { get; set; }
        public IEnumerable<Navigation> AvailableNavigations { get; set; }
        public IEnumerable<SearchProduct> Products { get; set; }
    }

    public class PageInfo
    {
        public int RecordStart { get; set; }
        public int RecordEnd { get; set; }
        public int TotalRecordCount { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }

    public class Navigation
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public IEnumerable<Refinement> Refinements { get; set; }
    }

    public class Refinement
    {
        public string Type { get; set; }
        public int Count { get; set; }
        public string Value { get; set; }
        public bool HasResults { get; set; }
        public string HashedValue { get; set; }
    }

    public class SearchProduct
    {
        public Brand Brand { get; set; }
        public Category Category { get; set; }
        public float ComparablePrice { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
    }

    public class Brand
    {
        public string Name { get; set; }
    }

    public class Category
    {
        public string Name { get; set; }
    }
}