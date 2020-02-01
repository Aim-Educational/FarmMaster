using FarmMaster.Filters;
using FarmMaster.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Controllers
{
    // This is for non-static image files.
    // i.e. anything to do with user-uploaded stuff, since we have special functionality for those.
    public class ImageController
    {
        readonly IServiceImageManager _images;
        readonly IHostingEnvironment _environment;

        public ImageController(IServiceImageManager images, IHostingEnvironment environment)
        {
            this._images = images;
            this._environment = environment;
        }

        [HttpGet]
        [FarmAuthorise]
        public async Task<IActionResult> Get(int? imageId, ushort? width, ushort? height)
        {
            if(imageId == null || width == null || height == null)
                return new BadRequestResult();

            // TODO: Put these magic numbers into a IConfig thing.
            if(width > 1920 || height > 1080)
                return new BadRequestResult();

            var image = await this._images.Query().FirstOrDefaultAsync(i => i.ImageId == imageId);
            if(image == null)
            {
                return new PhysicalFileResult(
                    this._environment.WebRootFileProvider.GetFileInfo("/images/icons/default.png").PhysicalPath, 
                    "image/jpeg"
                );
            }

            var path = await this._images.ResizeToPhysicalFile(image, width ?? 0, height ?? 0);
            if(path == null)
                return new BadRequestResult();

            return new PhysicalFileResult(path, "image/jpeg");
        }
    }
}
