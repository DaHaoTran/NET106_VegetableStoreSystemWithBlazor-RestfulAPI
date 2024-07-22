using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace API.Controllers
{
    [Route("api/images")]
    [ApiController]
    public class ImageUploadsAPIController : ControllerBase
    {
        /// <summary>
        /// Lưu ảnh vào hệ thống theo đường dẫn cung cấp
        /// </summary>
        /// <remarks>
        /// File hợp lệ: jpg file, png file, jpeg file
        /// </remarks>
        /// <param name = "file"> file </param>
        /// <param name="filePath">đường dẫn copy ảnh</param>
        /// <param name="encryptFileName">tên ảnh mã hóa</param>
        /// <response Code="400">Error</response>
        [HttpPost("savetosystem/{encryptFileName}/{filePath}")]
        public async Task<ActionResult<string>> SaveImageToSystem(IFormFile file, string encryptFileName, string filePath)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Không thấy tệp tin");
            }
            string filename = file.FileName;
            string extension = Path.GetExtension(filename);

            string[] allowedExtensions = { ".jpg", ".png", ".jpeg" };

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest("Tệp tin không hợp lệ");
            }

            string pathCombine = Path.Combine(filePath, encryptFileName);

            using (var fileStream = new FileStream(pathCombine, FileMode.Create, FileAccess.Write))
            {
                await file.CopyToAsync(fileStream);
            }

            return Ok($"{pathCombine}");
        }

        /// <summary>
        /// Lấy hình ảnh bằng tên ảnh
        /// </summary>
        /// <remarks>
        /// {
        ///     "imageName" : "092304i2349ff-42099f...jpg"
        /// }
        /// </remarks>
        /// <param name="name">imageName</param>
        /// <returns>Hình ảnh</returns>
        //[HttpGet("{name}")]
        //public ActionResult<FileStreamResult> GetImageByName(string name)
        //{
        //    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "FileUploads", name);

        //    if (!System.IO.File.Exists(imagePath))
        //    {
        //        return NotFound();
        //    }

        //    var imageFileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
        //    var image = new FileStreamResult(imageFileStream, "image/png");

        //    return image;
        //} 

        /// <summary>
        /// mã hóa tên ảnh
        /// </summary>
        /// <remarks>
        /// File hợp lệ: jpg file, png file, jpeg file
        /// </remarks>
        /// <response Code = "400" > Error </response>
        /// <returns> Tên ảnh mã hóa</returns>
        [HttpPost("name/encrypt")]
        public ActionResult<string> SaveImageToSystem(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Không thấy tệp tin");
            }
            string filename = file.FileName;
            string extension = Path.GetExtension(filename);

            string[] allowedExtensions = { ".jpg", ".png", ".jpeg" };

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest("Tệp tin không hợp lệ");
            }

            string newFileName = $"{Guid.NewGuid()}{extension}";

            return newFileName;
        }
    }
}
