using System.Collections.Generic;

namespace products.Models
{
    public class ProductResult
    {
        public ProductData Data { get; set; }
    }

    public class ProductData
    {
        public string ProductItemName { get; set; }
        public ImageInfo MainImage { get; set; }
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string Features { get; set; }
        public int SkuNumber { get; set; }
        public Brand Brand { get; set; }
        public IEnumerable<ImageInfo> Images { get; set; }
        public SellUnitDetails SellUnitDetails { get; set; }
        public string ProductName { get; set; }
        public string ProductUrl { get; set; }
    }

    public class ImageInfo
    {
        public string Url { get; set; }
    }

    public class SellUnitDetails
    {
        public float Weight { get; set; }
        public float Height { get; set; }
        public float Width { get; set; }
        public int SellPerShipUnit { get; set; }
        public float Length { get; set; }
    }
}