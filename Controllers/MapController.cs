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

        public IActionResult GetPreviews(string mls)
        {
            //MongoClient mongoClient = new MongoClient(MongoUrl.Create("mongodb://localhost:27017"));
            //IMongoDatabase mongoDatabase = mongoClient.GetDatabase("RealEstate");

            //return Ok("{ \"type\" : \"Feature\", \"properties\" : { \"MlsNo\" : \"MlsNo\", \"Address\" : \"doc.Address\", \"Bedrooms\" : \"doc.Bedrooms\", \"Washrooms\" : \"doc.Washrooms\", \"HouseType\" : \"doc.HouseType\", \"ListingDate\" : \"doc.ListingDate\", \"ListingPrice\" : \"doc.ListingPrice\", \"ImageUrl\" : \"doc.Pictures[0].url\", \"City\" : \"doc.City\", \"District\" : \"doc.District\", \"Community\" : \"doc.Community\" }, \"geometry\" : { \"type\" : \"Point\", \"coordinates\" : [-79.0, 43.0] } }");

            return View();
        }
    }
}