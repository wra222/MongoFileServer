using System.IO;
namespace MonGo.Entity
{
    public class ImageHelper
    {
        private   string FileType { get; set; }
        /// <summary>
        /// 获取文件类型
        /// </summary>
        /// <param name="extend"> 文件后缀</param>
        /// <returns></returns>
        public   string GetImageType(string extend)
        {
            switch (extend)
            {
                case "png":
                    FileType = "image/png";
                    break;
                case "jpg":
                    FileType = "image/jpeg";
                    break;
                case "gif":
                    FileType = "image/gif";
                    break;
                case "mp4":
                    FileType = "video/mp4";
                    break;
                case "avi":
                    FileType = "video/avi";
                    break;
                case "doc":
                case "docx":
                    FileType = "application/msword";
                    break;
                case "xls":
                case "xlsx":
                    FileType = "application/msexcel";
                    break;
                case "ppt":
                case "pptx":
                case "pps":
                    FileType = "application/mspowerpoint";
                    break;
                default:
                    FileType = "image/jpeg";
                    break;

            };
            return FileType;

        }
        #region 缩略图
        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">生成缩略图的方式</param>    
        public static MemoryStream MakeThumbnail(Stream originalImagePath,  int width, int height, string mode)
        {

            originalImagePath.Position = 0;
            System.Drawing.Image originalImage = System.Drawing.Image.FromStream(originalImagePath);

            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            switch (mode)
            {
                case "HW":  //指定高宽缩放（可能变形）                
                    break;
                case "W":   //指定宽，高按比例                    
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case "H":   //指定高，宽按比例
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case "Cut": //指定高宽裁减（不变形）                
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }

            //新建一个bmp图片
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);

            //新建一个画板
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            //设置高质量插值法
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //清空画布并以透明背景色填充
            g.Clear(System.Drawing.Color.Transparent);

            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(originalImage, new System.Drawing.Rectangle(0, 0, towidth, toheight), new System.Drawing.Rectangle(x, y, ow, oh), System.Drawing.GraphicsUnit.Pixel);

            try
            {
                //以jpg格式保存缩略图
                //bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                MemoryStream str = new MemoryStream();
                bitmap.Save(str, System.Drawing.Imaging.ImageFormat.Jpeg);
                return str;
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }
        #endregion
    }
}
