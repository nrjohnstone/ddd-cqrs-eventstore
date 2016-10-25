using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Restaurant.Host;
using Restaurant.Host.Documents;
using Xunit;

namespace Restaurant.Tests
{
    public class WaiterRestaurantDocumentWriterTests
    {
        public WaiterRestaurantDocumentWriter CreateSut(string documentJson)
        {
            return
                new WaiterRestaurantDocumentWriter(documentJson);
        }

        [Fact]
        public void CanCreateInstance()
        {
            var restaurantDocument = new RestaurantDocument("1");
            
            var sut = CreateSut(JsonConvert.SerializeObject(restaurantDocument));

            sut.Should().NotBeNull();
        }

        [Fact]
        public void TableNumberGetter_ShouldReturnValueFromJson()
        {
            int expectedTableNumber = 10;
            var restaurantDocument = new RestaurantDocument("1")
            {
                TableNumber = expectedTableNumber
            };

            var sut = CreateSut(JsonConvert.SerializeObject(restaurantDocument));

            sut.TableNumber.Should().Be(expectedTableNumber);
        }

        [Fact]
        public void TableNumberSetter_ShouldUpdateValueFromJson()
        {
            int expectedTableNumber = 10;
            var restaurantDocument = new RestaurantDocument("1")
            {
                TableNumber = expectedTableNumber - 1
            };

            var sut = CreateSut(JsonConvert.SerializeObject(restaurantDocument));

            sut.TableNumber = expectedTableNumber;

            sut.TableNumber.Should().Be(expectedTableNumber);
        }

        [Fact]
        public void ItemsGetter_ShouldReturnJson()
        {
            Item item = new Item("Pizza");
            var restaurantDocument = new RestaurantDocument("1");
            restaurantDocument.Items.Add(item);
           
            var sut = CreateSut(JsonConvert.SerializeObject(restaurantDocument));

            sut.Items[0].Description.Should().Be("Pizza");
        }

        [Fact]
        public void AddItems_ShouldUpdateJson()
        {
            Item item = new Item("Pizza");
            var restaurantDocument = new RestaurantDocument("1");
            restaurantDocument.Items.Add(item);

            var sut = CreateSut(JsonConvert.SerializeObject(restaurantDocument));
            sut.AddItem(new Item("Pie"));

            sut.Items[0].Description.Should().Be("Pizza");
            sut.Items[1].Description.Should().Be("Pie");
        }

        [Fact]
        public void TimeToCookMsGetter_ShouldReturnFromJson()
        {
            int expectedTimeToCook = 300;
            var restaurantDocument = new RestaurantDocument("1")
            {
                TimeToCookMs = 300
            };

            var sut = CreateSut(JsonConvert.SerializeObject(restaurantDocument));

            sut.TimeToCookMs.Should().Be(expectedTimeToCook);
        }

        [Fact]
        public void TimeToCookMsSetter_ShouldUpdateFromJson()
        {
            int expectedTimeToCook = 300;
            var restaurantDocument = new RestaurantDocument("1")
            {
                TimeToCookMs = 300
            };

            var sut = CreateSut(JsonConvert.SerializeObject(restaurantDocument));

            sut.TimeToCookMs = expectedTimeToCook;
            sut.TimeToCookMs.Should().Be(expectedTimeToCook);
        }

        [Fact]
        public void TaxGetter_ShouldReturnFromJson()
        {
            double expectedTax = 2.00;
            var restaurantDocument = new RestaurantDocument("1")
            {
                Tax = 2.00
            };

            var sut = CreateSut(JsonConvert.SerializeObject(restaurantDocument));

            sut.Tax.Should().Be(expectedTax);
        }

        [Fact]
        public void TaxSetter_ShouldUpdateUpdate()
        {
            double expectedTax = 2.00;
            var restaurantDocument = new RestaurantDocument("1")
            {
                Tax = 2.00
            };

            var sut = CreateSut(JsonConvert.SerializeObject(restaurantDocument));

            sut.Tax = expectedTax;
            sut.Tax.Should().Be(expectedTax);
        }

        [Fact]
        public void ToJsonString_ShouldReturnCorrectJson()
        {
            var originalJson = JsonConvert.SerializeObject(new RestaurantDocument("1"));
            var sut = CreateSut(originalJson);

            string expectedJson = JObject.Parse(originalJson).ToString();
            sut.ToJsonString().Should().Be(expectedJson);
        }

        [Fact]
        public void WhenPropertyModified_ToJsonString_ShouldReturnCorrectJson()
        {
            var originalJson = JsonConvert.SerializeObject(new RestaurantDocument("1")
            {
                TableNumber = 4
            });
            var sut = CreateSut(originalJson);

            sut.TableNumber = 5;
            var expectedJson = JsonConvert.SerializeObject(new RestaurantDocument("1")
            {
                TableNumber = 5
            });
            string expectedJsonString = JObject.Parse(expectedJson).ToString();
            sut.ToJsonString().Should().Be(expectedJsonString);
        }

        [Fact]
        public void WhenJsonContainsUnknownProperties_ShouldNotBeLost()
        {
            var originalJson = JsonConvert.SerializeObject(new RestaurantDocument("1")
            {
                TableNumber = 4
            });
            var jObject = JObject.Parse(originalJson);
            jObject.Property("TableNumber").AddAfterSelf(new JProperty("NewProperty", "New value"));

            var sut = CreateSut(jObject.ToString());

            string resultJsonString = sut.ToJsonString();
            resultJsonString.Should().Contain("NewProperty");
            resultJsonString.Should().Contain("New value");
        }


    }
}
