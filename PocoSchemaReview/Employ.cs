using Newtonsoft.Json;
namespace PocoSchemaReview
{
    class Employ
    {

        [JsonProperty(Required = Required.Always)]
        public string Forename { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Surname { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string EmailAddress { get; set; }


        public int Age { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public string MobilePhoneNumber { get; set; }
    }
}
