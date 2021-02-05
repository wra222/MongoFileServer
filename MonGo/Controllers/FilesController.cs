using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MonGo.Entity;
using MonGo.Services;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.IO;

namespace MonGo.Controllers
{
    [Route("[controller]")]
    [ApiController]

    public class FileController : ControllerBase
    {
        private readonly FileService _fileService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public FileController(FileService fileService, IWebHostEnvironment hostingEnvironment)
        {
            _fileService = fileService;
            _hostingEnvironment = hostingEnvironment;
        }
        /// <summary>
        /// 普通文件获取
        /// </summary>
        /// <param name="id">文件id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            try
            {
                string[] files = id.Split('.');
                if (files.Length < 2)
                {
                    return NotFound();
                }
                ImageHelper Ihelper = new ImageHelper();
                var imgByte = _fileService.DownloadToByte(ObjectId.Parse(files[0]));
                string FileType = Ihelper.GetImageType(files[files.Length - 1]);
                if (imgByte == null)
                {
                    return NotFound();
                }
                var response = File(imgByte, FileType);
                return response;
            }
            catch(Exception ex)
            {
                LogHelper.Error($"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName} - {DateTime.Now} 获取文件:id={id}",ex); // 日志记录
                return NotFound();
            }
        }
       /// <summary>
       /// 缩略图获取
       /// </summary>
       /// <param name="Thumb">缩略图参数</param>
       /// <param name="id">文件id</param>
       /// <returns></returns>
        [HttpGet("{Thumb}/{id}", Name = "GetThumb")]
        public IActionResult Get(string Thumb, string id)
        {
            //thumb_650_300
            LogHelper.Info($"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName} - {DateTime.Now} 获取文件:id={id}"); // 日志记录
            string[] Thumbs = Thumb.Split('_');
            IActionResult response;
            ImageHelper Ihelper = new ImageHelper();
            string[] files = id.Split('.');
            if (files.Length < 2)
            {
                return NotFound();
            }
            if (Thumbs[0].Contains("Thumb") == false&& Thumbs[0].Contains("thumb") == false)
            {
                response = Content("您的查询接口参数不正确！");
                return response;
            }
            else
            {
                string newFileId = string.Empty;
               
                if (Thumbs.Length < 3) //返回原图
                {
                    var imgByte = _fileService.DownloadToByte(ObjectId.Parse(files[0]));
                    string FileType = Ihelper.GetImageType(files[files.Length - 1]);
                    if (imgByte == null)
                    {
                        return NotFound();
                    }
                    response = File(imgByte, FileType);
                    return response;
                }
                else
                {
                    newFileId = _fileService.GetFileIdByCacheUri("\\" + Thumb + "\\" + id); //通过缩略图url 查找缓存文件id
                    if (!string.IsNullOrEmpty(newFileId)) //存在缓存返回缓存文件
                    {
                        var imgByte = _fileService.DownloadToByte(ObjectId.Parse(newFileId));
                        string FileType = Ihelper.GetImageType(files[files.Length - 1]);
                        if (imgByte == null)
                        {
                            return NotFound();
                        }
                        response = File(imgByte, FileType);
                        return response;
                    }
                    else //不存在则生成缩略图并保存到缓存中
                    {
                        MemoryStream NewImage =_fileService.GetTrumbImage(ObjectId.Parse(files[0]), int.Parse(Thumbs[Thumbs.Length - 2]), int.Parse(Thumbs[Thumbs.Length - 1]), files[files.Length-1], "\\" + Thumb + "\\" + id);
                        var newByte = NewImage.ToArray();
                        if (newByte == null)
                        {
                            return NotFound();
                        }
                        string FileType = Ihelper.GetImageType(files[files.Length-1]);
                        response = File(newByte, FileType);
                        return response;
                    }
                }
            }
 
        }
        /// <summary>
        /// 文件上传，通过form 上传文件
        /// </summary>
        /// <returns></returns>
        [Route("uploadFile")]
        [HttpPost(Name = "uploadFile")]
        
        public ApiResponse uploadFile()
        {
            string State = "success";
            try
            {
                Console.WriteLine("Uploading...");
                string FileType;
                var files = Request.Form.Files;

                string FileId = string.Empty;
                Content content = new Content();
                ImageHelper Ihelper = new ImageHelper();
                List<Content> list = new List<Content>();
                try
                {

                    foreach (var file in files)
                    {
                        var fileName = file.FileName;
                        string[] fileType = fileName.Split('.');
                        FileType = Ihelper.GetImageType(fileType[fileType.Length - 1]);
                        //fileName = Path.Combine(_hostingEnvironment.WebRootPath + $"/UploadFile/", fileName);
                        using (MemoryStream ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            //fs.Flush();
                            ms.Position = 0;
                            //通过MD5检验文件是否在库，
                            var md5 = MD5Helper.GetMD5Hash(ms);
                            FileId = _fileService.CheckFileExistsByMd5(md5);
                            if (string.IsNullOrEmpty(FileId))
                            {
                                FileId += _fileService.UploadFromStream(ms, file.FileName, FileType);
                            }
                            content.Id = FileId;
                            content.Hash = md5;
                            content.FileName = file.FileName;
                            content.ContentType = FileType;
                            list.Add(content);
                        }
                        //filenames+= _fileService.UploadFromFile(fileName, file.FileName);
                    }
                    return new ApiResponse() {  State = State,Content = list };
                }
                catch(Exception ex)
                {
                    State = "fail";
                    foreach (var file in files)
                    {
                        content.Id = FileId;
                        string[] fileType = file.FileName.Split('.');
                        FileType = Ihelper.GetImageType(fileType[fileType.Length - 1]);
                        content.FileName = file.FileName;
                        content.ContentType = FileType;
                    }
                    list.Add(content);
                    return new ApiResponse() { State = State, Content = list };
                }
            }
            catch
            {
                State = "fail";
                return new ApiResponse() { State = State, Content = null };
            }
        }
    }
}