using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MonGo.Models
{
    public class UrlCache
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("url")]
        public string url { get; set; }

        [BsonElement("fileId")]
        public string fileId { get; set; }

        [BsonElement("originalFileId")]
        public string originalFileId { get; set; }

        [BsonElement("createTime")]
        public string createTime { get; set; }
        [BsonElement("updateTime")]
        public string updateTime { get; set; }
        [BsonElement("md5")]
        public string Md5 { get; set; }
    }
}
