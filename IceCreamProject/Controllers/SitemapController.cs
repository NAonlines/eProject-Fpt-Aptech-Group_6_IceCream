using IceCreamProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using System.Xml.Xsl;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using IceCreamProject.Libary;
using System.Xml;

namespace IceCreamProject.Controllers
{
    public class SitemapController : Controller
    {
        private readonly ShopContext _db;

        public SitemapController(ShopContext db)
        {
            _db = db;
        }

        // Endpoint trả về sitemap XML
        [HttpGet("/sitemap.xml")]
        public async Task<IActionResult> SitemapXml()
        {
            var sitemapXml = await GenerateSitemapXml();

            return Content(sitemapXml, "application/xml");
        }

        // Tạo sitemap XML từ cơ sở dữ liệu
        private async Task<string> GenerateSitemapXml()
        {
            // Tạo danh sách các URL tĩnh
            var urls = new List<string>
            {
                Url.Action("Index", "Home", null, Request.Scheme), // Trang chủ
                Url.Action("AboutUs", "Home", null, Request.Scheme), // Giới thiệu
                Url.Action("ContactUs", "Home", null, Request.Scheme), // Liên hệ
                Url.Action("FreeRecipes", "Home", null, Request.Scheme), // Công thức miễn phí
                Url.RouteUrl("Membership", null, Request.Scheme)
            };

            // Thêm URL cho các sản phẩm (Books)
            var books = await _db.Books.Where(b => b.IsActive).ToListAsync();
            foreach (var book in books)
            {
                var productUrl = Url.Action("ProductDetails", "Home", new { id = book.BookId, name = UrlHelper.seoweb(book.Title) }, Request.Scheme);
                urls.Add(productUrl);
            }

            // Thêm URL cho các công thức (Recipes)
            var recipes = await _db.Recipes.ToListAsync();
            foreach (var recipe in recipes)
            {
                var recipeUrl = Url.Action("RecipeDetails", "Home", new { id = recipe.RecipeId, name = UrlHelper.seoweb(recipe.Name) }, Request.Scheme);
                urls.Add(recipeUrl);
            }

            // Định nghĩa namespace
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

            // Tạo sitemap XML với namespace chỉ định một lần
            var sitemap = new XElement(ns + "urlset",
                urls.Select(url => new XElement(ns + "url",
                    new XElement(ns + "loc", url),
                    new XElement(ns + "lastmod", DateTime.UtcNow.ToString("yyyy-MM-dd")),
                    new XElement(ns + "changefreq", "daily"),
                    new XElement(ns + "priority", "0.8")
                ))
            );

            // Thêm khai báo XSL vào đầu tệp XML
            var xslDeclaration = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
            <?xml-stylesheet type=""text/xsl"" href=""/sitemap.xsl""?>";

            // Kết hợp khai báo XSL và nội dung sitemap
            return $"{xslDeclaration}\n{sitemap}";
        }

        // Endpoint để hiển thị sitemap dưới dạng HTML (có sử dụng XSLT)
        //[HttpGet("/sitemap-view", Name = "SitemapView")]
        //public async Task<IActionResult> SitemapView()
        //{
        //    var sitemapXml = await GenerateSitemapXml(); // Tạo XML sitemap từ cơ sở dữ liệu

        //    // Đường dẫn đến file XSLT trong thư mục wwwroot
        //    var xsltFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "sitemap.xsl");

        //    // Tạo một XmlDocument từ XML sitemap
        //    var xmlDoc = new XmlDocument();
        //    xmlDoc.LoadXml(sitemapXml);

        //    // Tạo XslCompiledTransform để xử lý XSLT
        //    var xslt = new XslCompiledTransform();
        //    xslt.Load(xsltFilePath); // Load file XSLT từ wwwroot

        //    // Tạo StringWriter để lưu kết quả HTML
        //    using (var writer = new StringWriter())
        //    {
        //        // Áp dụng XSLT transformation vào XML sitemap
        //        xslt.Transform(xmlDoc, null, writer);

        //        // Trả về kết quả HTML
        //        return Content(writer.ToString(), "text/html");
        //    }
        //}

    }
}
