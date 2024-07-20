﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using static System.Net.Mime.MediaTypeNames;

namespace API.Controllers
{
    [Route("api/images")]
    [ApiController]
    public class ImageUploadsAPIController : ControllerBase
    {
        /// <summary>
        /// Lưu ảnh vào hệ thống
        /// </summary>
        /// <remarks>
        /// File hợp lệ: jpg file, png file, jpeg file
        /// </remarks>
        /// <param name="file">file</param>
        /// <response Code="400">Error</response>
        /// <returns>Tên ảnh mã hóa</returns>
        //[HttpPost("savetosystem")]
        //public async Task<ActionResult<string>> SaveImageToSystem([FromForm] IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //    {
        //        return BadRequest("không thấy tệp tin");
        //    }
        //    string filename = file.FileName;
        //    string extension = Path.GetExtension(filename);

        //    string[] allowedExtensions = { ".jpg", ".png", ".jpeg" };

        //    if (!allowedExtensions.Contains(extension))
        //    {
        //        return BadRequest("tệp tin không hợp lệ");
        //    }

        //    string newFileName = $"{Guid.NewGuid()}{extension}";
        //    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "FileUploads", newFileName);

        //    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        //    {
        //        await file.CopyToAsync(fileStream);
        //    }

        //    return newFileName;
        //}

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
        /// <response Code="400">Error</response>
        /// <returns>Tên ảnh mã hóa</returns>
        [HttpPost("name/encrypt")]
        public ActionResult<string> SaveImageToSystem(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("không thấy tệp tin");
            }
            string filename = file.FileName;
            string extension = Path.GetExtension(filename);

            string[] allowedExtensions = { ".jpg", ".png", ".jpeg" };

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest("tệp tin không hợp lệ");
            }

            string newFileName = $"{Guid.NewGuid()}{extension}";

            return newFileName;
        }
    }
}