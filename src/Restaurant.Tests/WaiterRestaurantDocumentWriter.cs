using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Restaurant.Tests
{
    public class WaiterRestaurantDocumentWriter
    {
        private readonly string _documentJson;
        private readonly JObject _documentJObject;

        public WaiterRestaurantDocumentWriter(string documentJson)
        {
            _documentJson = documentJson;
            _documentJObject = JObject.Parse(documentJson);
        }

        public int TableNumber
        {
            get { return (int) _documentJObject[nameof(RestaurantDocument.TableNumber)]; }

            set { _documentJObject[nameof(RestaurantDocument.TableNumber)] = value; }
        }

        public int TimeToCookMs
        {
            get { return (int)_documentJObject[nameof(RestaurantDocument.TimeToCookMs)]; }

            set { _documentJObject[nameof(RestaurantDocument.TimeToCookMs)] = value; }
        }

        public double Tax
        {
            get { return (double)_documentJObject[nameof(RestaurantDocument.Tax)]; }

            set { _documentJObject[nameof(RestaurantDocument.Tax)] = value; }
        }

        public Item[] Items
        {
            get { return _documentJObject[nameof(RestaurantDocument.Items)].ToObject<Item[]>(); }
        }

        public void AddItem(Item item)
        {
            var items = (JArray) _documentJObject[nameof(RestaurantDocument.Items)];
            items.Add(JObject.Parse(JsonConvert.SerializeObject(item)));
        }

        public string ToJsonString()
        {
            return _documentJObject.ToString();
        }
    }
}