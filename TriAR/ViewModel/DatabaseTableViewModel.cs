using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TriAR.Database;
using TriAR.Model;

namespace TriAR.ViewModel
{
    public class DatabaseTableViewModel
    {
        public DatabaseTableViewModel()
        {
            RefreshData();
        }

        private void RefreshData()
        {
            _restList = GetAllRest();
            if (_restList.Count() == 0)
                ReadSaveAndSetData();

            _restCount = SQLiteCommandExecution.GetRecordCountFromTable("RESTS"); // оставил ради прямого запроса 
        }

        private List<Rest> _restList;
        public List<Rest> RestList
        { 
            get => _restList;
            set => _restList = value;
        }

        private int _restCount;
        public int RestCount
        {
            get => _restCount;
            set => _restCount = value;
        }
        public void ReadSaveAndSetData()
        {
            ParseFile parseFile = new ParseFile();
            List<Rest> restsForSave = new List<Rest>();

            var restsFromStock = parseFile.ParseFirsFile();
            restsForSave.AddRange(restsFromStock);

            var beerRests = parseFile.ParseSecondFile();
            restsForSave.AddRange(beerRests);

            new SaveRestToDataBase().SaveRestToDataBaseMethod(restsForSave);
            _restList = restsForSave;
        }

        public static List<Rest> GetAllRest()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var rest = db.Rests.ToList();
                var product = db.Products.ToList();
                var producer = db.Producers.ToList();
                return rest;
            }
        }

        private class ParseFile
        {
            private static XNamespace nPref = "http://fsrar.ru/WEGAIS/ProductRef_v2";
            private static XNamespace nNs = "http://fsrar.ru/WEGAIS/WB_DOC_SINGLE_01";
            private static XNamespace nOref = "http://fsrar.ru/WEGAIS/ClientRef_v2";

            private static XDocument xdoc = XDocument.Load(@"D:\Visual Studio\Projects\Files\ReplyRests_v2-010xD40F23FA-E703-4D3E-8D2D-DB0525A2161E.xml");
            private static XDocument ydoc = XDocument.Load(@"D:\Visual Studio\Projects\Files\ReplyRestsShop_0e21bc71-ed87-4994-8e6b-d84d74bc364b.xml");

            private static XNamespace nRst = "http://fsrar.ru/WEGAIS/ReplyRests_v2";
            private static XNamespace bRst = "http://fsrar.ru/WEGAIS/ReplyRestsShop_v2";

            private string xReplyDocument = "ReplyRests_v2";
            private string yReplyDocument = "ReplyRestsShop_v2";

            private string xPosition = "StockPosition";
            private string yPosition = "ShopPosition";

            public List<Rest> ParseFirsFile() => ParseXmlFromFolder(nRst, xdoc, xReplyDocument, xPosition);
            public List<Rest> ParseSecondFile() => ParseXmlFromFolder(bRst, ydoc, yReplyDocument, yPosition);

            private List<Rest> ParseXmlFromFolder(XNamespace rst, XDocument loadXMLDocument, string replyDocument, string positions)
            {
                List<Rest> rests = new List<Rest>();

                foreach (XElement element in loadXMLDocument.Elements(nNs + "Documents").Elements(nNs + "Document")
                .Elements(nNs + replyDocument).Elements(rst + "Products").Elements(rst + positions))
                {
                    var rest = new Rest();
                    rest.Quantity = element.Descendants(rst + "Quantity").FirstOrDefault().Value.ToString();
                    rest.InformF1RegId = element.Descendants(rst + "InformF1RegId").FirstOrDefault()?.Value.ToString();
                    rest.InformF2RegId = element.Descendants(rst + "InformF2RegId").FirstOrDefault()?.Value.ToString();
                    var product = new Product();
                    product.FullName = element.Descendants(nPref + "FullName").FirstOrDefault().Value.ToString();
                    product.AlcCode = element.Descendants(nPref + "AlcCode").FirstOrDefault().Value.ToString();
                    product.Capacity = element.Descendants(nPref + "Capacity").FirstOrDefault().Value.ToString();
                    product.AlcVolume = element.Descendants(nPref + "AlcVolume").FirstOrDefault().Value.ToString();
                    product.ProductVCode = element.Descendants(nPref + "ProductVCode").FirstOrDefault().Value.ToString();
                    var producer = new Producer();
                    producer.Organization = element.Descendants(nOref + "ShortName").FirstOrDefault()?.Value.ToString();
                    producer.ClientRegId = element.Descendants(nOref + "ClientRegId").FirstOrDefault()?.Value.ToString();
                    rest.Product = product;
                    rest.Product.Producer = producer;
                    rests.Add(rest);
                }
                return rests;
            }
        }

        private class SQLiteCommandExecution
        {
            public static int GetRecordCountFromTable(string tableName)
            {
                using (var connection = new SqliteConnection(@"Data Source=C:\sqlite\TZ.db; "))
                {
                    connection.Open();
                    SqliteCommand command = new SqliteCommand("SELECT COUNT(*) FROM " + tableName, connection);
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        private class SaveRestToDataBase
        {
            public void SaveRestToDataBaseMethod(IEnumerable<Rest> rests)
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    foreach (Rest rest in rests)
                    {
                        try
                        {
                            var producerId = GetProducerIdByFsrarId(rest.Product.Producer, db); rest.Product.ProducerId = producerId;
                            rest.Product.Producer = null;
                            var productId = GetProductIdByAlcCode(rest.Product, db);
                            rest.ProductId = productId;
                            rest.Product = null;
                            db.Rests.Add(rest);
                        }
                        catch { }
                        db.SaveChanges();
                    }
                }
            }

            private int GetProducerIdByFsrarId(Producer producer, ApplicationContext dbContext)
            {
                var producerFromDbByClientRegId = dbContext.Producers.FirstOrDefault(x => x.ClientRegId == producer.ClientRegId);
                if (producerFromDbByClientRegId is null)
                {
                    dbContext.Producers.Add(producer);
                    dbContext.SaveChanges();
                    producerFromDbByClientRegId = producer;
                }
                return producerFromDbByClientRegId.Id;
            }

            private int GetProductIdByAlcCode(Product product, ApplicationContext dbContext)
            {
                var productFromDbByAlcCode = dbContext.Products.FirstOrDefault(x => x.AlcCode == product.AlcCode);
                if (productFromDbByAlcCode is null)
                {
                    dbContext.Products.Add(product);
                    dbContext.SaveChanges();
                    productFromDbByAlcCode = product;
                }
                return productFromDbByAlcCode.Id;
            }
        }
    }
}

