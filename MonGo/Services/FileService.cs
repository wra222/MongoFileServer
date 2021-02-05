using Microsoft.Extensions.Configuration;
using MonGo.Entity;
using MonGo.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.IO;
using System.Linq;


namespace MonGo.Services
{
    public class FileService
    {
        private readonly IMongoClient client;
        private readonly IMongoDatabase database;
        //private readonly IMongoCollection<BsonDocument> collection;
        private readonly IMongoCollection<UrlCache> _UrlCache;
        private  GridFSBucket bucket;
        private GridFSFileInfo fileInfo;
        //private ObjectId oid;
        private GridFSBucketOptions option;
        public FileService(IConfiguration config)
        {
            client = new MongoClient(config.GetConnectionString("FilesServerDb"));
            database = client.GetDatabase(config.GetConnectionString("DataBaseName"));
            _UrlCache = database.GetCollection<UrlCache>("UrlCache");
        }
        public string UploadFromFile(string FilePath, string FileName, GridFSUploadOptions Options = null)
        {
            
            //string fullpath = "C:/Users/Administrator/Desktop/image.png";
            string FileId = string.Empty;
            try
            {
                using (FileStream sr = new FileStream(FilePath, FileMode.Open))
                {

                    GridFSBucketOptions option = new GridFSBucketOptions()
                    {
                        BucketName = "fs",
                        ChunkSizeBytes = 2 * 1024 * 1024,//每个文件大小
                        ReadConcern = null,
                        ReadPreference = null,
                        WriteConcern = null
                    };

                    bucket = new GridFSBucket(database, option);
                    if (Options == null)
                    {
                        Options = new GridFSUploadOptions();
                    }
                    BsonDocument doc = new BsonDocument();
                    doc.Add("operator", "Administrator");
                    doc.Add("time", DateTime.Now);
                    Options.Metadata = doc; // 添加metadata数据


                    using (var upload = bucket.OpenUploadStream(FileName, Options))
                    {
                        sr.CopyTo(upload);
                        upload.Close();
                        FileId = upload.Id.ToString();
                    }
                    return FileId;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string UploadFromStream(Stream str, string FileName,string FileType, GridFSUploadOptions Options = null)
        {

            //string fullpath = "C:/Users/Administrator/Desktop/image.png";
            string FileId = string.Empty;
            try
            {
                using (str)
                {
                    StreamHelper Ihelper = new StreamHelper();
                    option = new GridFSBucketOptions()
                        {
                            BucketName = "fs",
                            ChunkSizeBytes = 2 * 1024 * 1024,//每个文件大小
                            ReadConcern = null,
                            ReadPreference = null,
                            WriteConcern = null
                        };
                    bucket = new GridFSBucket(database, option);
                    if (Options == null)
                    {
                        Options = new GridFSUploadOptions();
                    }
                    BsonDocument doc = new BsonDocument();
                    doc.Add("operator", "Administrator");
                    doc.Add("FileType", FileType);
                    doc.Add("time", DateTime.Now);
                    Options.Metadata = doc; // 添加metadata数据


                    using (var upload = bucket.OpenUploadStream(FileName, Options))
                    {
                        str.Position = 0;
                        str.CopyTo(upload);
                        upload.Close();
                        FileId = upload.Id.ToString();
                    }
                    return FileId;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                str = null;
            }
        }
        public bool CheckFileExistsById(string FileId)
        {
            var FileInfo = FindFiles(FileId);
            if (FileInfo == null)
            { return false; }
            else
            { return true; }
            
        }
        public string CheckFileExistsByMd5(string md5)
        {

            string id = string.Empty;
            bucket = new GridFSBucket(database, new GridFSBucketOptions()
            {
                BucketName = "fs",
                ChunkSizeBytes = 2 * 1024 * 1024,//每个文件大小
                ReadConcern = null,
                ReadPreference = null,
                WriteConcern = null
            });
            var filter = new { md5 = md5.ToLower() };
            var result = bucket.Find(filter.ToBsonDocument()).ToList();
            if (result.Count > 0)
            {
                fileInfo = result.FirstOrDefault();
                id = fileInfo.Id.ToString();
            }
            return id;
        }
        public Byte[] DownloadToByte(ObjectId id)
        {
            bucket = new GridFSBucket(database, new GridFSBucketOptions()
            {
                BucketName = "fs",
                ChunkSizeBytes = 2 * 1024 * 1024,//每个文件大小
                ReadConcern = null,
                ReadPreference = null,
                WriteConcern = null
            });
            var download = bucket.DownloadAsBytes(id);
            if (download.Length > 0)
            {
                return download;

            }
            else
            {
                return null;
            }

        }
        public GridFSFileInfo FindFiles(string FileId)
        {
            
            bucket = new GridFSBucket(database, new GridFSBucketOptions()
            {
                BucketName = "fs",
                ChunkSizeBytes = 2 * 1024 * 1024,//每个文件大小
                ReadConcern = null,
                ReadPreference = null,
                WriteConcern = null
            });
            //var filter = new { id = "5ce78cab36b193553cae0d24" };
            //var result = bucket.Find(filter.ToBsonDocument()).ToList();
            var filter = Builders<GridFSFileInfo>.Filter.Eq(x => x.IdAsBsonValue, BsonValue.Create("5ce65052d7f2b94f00d320da"));
            //var filter = Builders<GridFSFileInfo>.Filter.Eq(x => x.MD5, BsonValue.Create(FileId));
            //Builders<GridFSFileInfo>.Filter.Gte(x => x.UploadDateTime, new DateTime(2019, 1, 1, 0, 0, 0, DateTimeKind.Utc)));
            //Builders<GridFSFileInfo>.Filter.Lt(x => x.UploadDateTime, new DateTime(2017, 2, 1, 0, 0, 0, DateTimeKind.Utc)));
            var sort = Builders<GridFSFileInfo>.Sort.Descending(x => x.UploadDateTime);
            var options = new GridFSFindOptions
            {
                Limit = 1,
                Sort = sort
            };

            using (var cursor = bucket.Find(filter, options))
            {
                fileInfo = cursor.ToList().FirstOrDefault();
            }
            return fileInfo;
        }
        public bool UrlCacheCreate(string Uri,string originalFileId,string fileId,string Md5)
        {
            string id =  Guid.NewGuid().ToString().ToLower().Replace("-","");
           
            UrlCache uriCa = new UrlCache
            {
                //Id = id,
                url=Uri,
                fileId=fileId,
                originalFileId=originalFileId,
                createTime= ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000).ToString(),
                updateTime="",
                Md5=Md5.ToLower()
            };
            _UrlCache.InsertOne(uriCa);
            return true;
        }
        public string GetFileIdByCacheUri(string Uri)
        {
            var result = _UrlCache.Find<UrlCache>(UrlCache => UrlCache.url == Uri).FirstOrDefault();
            if (result == null)
            {
                return "";
            }
            return result.fileId;
        }
        public MemoryStream GetTrumbImage(ObjectId id,int w,int h,string extend,string uri)
        {
            ImageHelper Ihelper = new ImageHelper();
            var imgByte = DownloadToByte(id); //查询原文件
            Stream OrignImage = new MemoryStream(imgByte);
            //生成缩略图
            MemoryStream NewImage = ImageHelper.MakeThumbnail(OrignImage, w, h, "HW");
            string FileType = Ihelper.GetImageType(extend);
            NewImage.Position = 0;
            var md5 = MD5Helper.GetMD5Hash(NewImage);
            string newFileId = CheckFileExistsByMd5(md5);
            if (string.IsNullOrEmpty(newFileId))
            {
                newFileId = UploadFromStream(NewImage, uri, FileType);
            }
            //保存urlcache
            UrlCacheCreate(uri, id.ToString(), newFileId, md5);
            return NewImage;
        }
       
    }
}
