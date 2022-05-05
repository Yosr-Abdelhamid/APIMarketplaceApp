﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace APIMarketplaceApp.Models
{
    public class Vendeur
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Nom")]
        public string Nom { get; set; }

        [BsonElement("Prenom")]
        public string Prenom { get; set; }

        [BsonElement("Email")]
        public string Email { get; set; }

        [BsonElement("Adresse")]
        public string Adresse { get; set; }

        [BsonElement("Num_Telephone")]
        public string Num_Telephone { get; set; }

        [BsonElement("MotDePasse")]
        public string MotDePasse { get; set; }
    }
}