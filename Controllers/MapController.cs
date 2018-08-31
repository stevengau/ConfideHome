using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConfideHome.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace ConfideHome.Controllers
{
    public class MapController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ForLease(string searchCity, string searchDays, string searchBeds, string searchBaths, 
            string searchPriceStart, string searchPriceEnd, string searchSouth,  string searchWest, string searchNorth, string searchEast, string searchZoom)
        {
            if (searchCity == null)
            {
                return View("Index");
            }
            else
            {
                MongoClient mongoClient = new MongoClient(MongoUrl.Create("mongodb://localhost:27017"));
                IMongoDatabase mongoDatabase = mongoClient.GetDatabase("RealEstate");
                var builder = Builders<BsonDocument>.Filter;
                var filter = builder.GeoWithinBox("Location", Double.Parse(searchSouth), Double.Parse(searchWest), Double.Parse(searchNorth), Double.Parse(searchEast));

                long recordCount = mongoDatabase.GetCollection<BsonDocument>("ForLeaseMap").Count(filter);
                if (recordCount == 0)
                {
                    return Json("{}");
                }
                if (recordCount <= 30000)
                {
                    var data = mongoDatabase.GetCollection<BsonDocument>("ForLeaseMap").Find<BsonDocument>(filter).ToList();
                    return Json(data.ToJson());
                }
            }
            return View("Index");
        }
        
        public IActionResult GetMapPopup(string mls)
        {
            MongoClient mongoClient = new MongoClient(MongoUrl.Create("mongodb://localhost:27017"));
            IMongoDatabase mongoDatabase = mongoClient.GetDatabase("RealEstate");
            var collection = mongoDatabase.GetCollection<PropertyDetailByJaJa>("ForLease");

            string[] inCriterias = mls.Split(",");

            List<PropertyDetailByJaJa> result = collection.Find(c => inCriterias.Contains(c.MlsNo)).ToList<PropertyDetailByJaJa>();

            return Json(result);
        }
    }
}