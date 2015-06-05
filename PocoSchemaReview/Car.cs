using Newtonsoft.Json;
using GenerateJsonSchema.NiceCars;
using System.Collections.Generic;


namespace PocoSchemaReview
{
    class Car
    {
        [JsonProperty(Required = Required.Always)]
        public string Model { get; set; }

        public int Year { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Color { get; set; }

        [JsonProperty(Required = Required.Always)]
        public int[] Price { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string[] Cities { get; set; }

        
        public char CharTest { get; set; }

        [JsonProperty(Required = Required.Always)]
        public char[] CharArrayTest { get; set; }


        public float FloatTest { get; set; }

        [JsonProperty(Required = Required.Always)]
        public float[] FloatArrayTest { get; set; }

        [JsonProperty(Required = Required.Always)]
        public NiceCar NamespaceTest { get; set; }

        [JsonProperty(Required = Required.Always)]
        public List<Buyers> BuyersInfo { get; set; }
    }

    public class Buyers
    {
        public string name { get; set; }
        public int number { get; set; }
    }

}

namespace GenerateJsonSchema.NiceCars
{
    class NiceCar
    {
        public string Description { get; set; }
    }
}