<<<<<<< HEAD
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
=======
﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
>>>>>>> e40425a (Add .gitignore and remove ignored files from repo)

namespace QuickCommerce.Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string UserId { get; set; }
        public List<OrderItem> Items { get; set; }
        public double TotalPrice { get; set; }
        public Location DeliveryLocation { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
<<<<<<< HEAD

=======
>>>>>>> e40425a (Add .gitignore and remove ignored files from repo)
    public class OrderItem
    {
        public string ItemId { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
    }

    public class Location
    {
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
