using Business.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IO;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SL = SixLabors.ImageSharp;

namespace FarmMaster.Services
{
    public interface IServiceImageManager : IServiceEntityManager<Image>
    {
        Task<Image> UploadFromForm(IFormFile file);

        Task<string> ResizeToPhysicalFile(Image image, ushort width, ushort height);

        string GetFileNameForImageAndSize(Image image, ushort width, ushort height);
    }

    public class ServiceImageManager : IServiceImageManager
    {
        readonly FarmMasterContext _context;
        readonly IHostingEnvironment _environment;
        static RecyclableMemoryStreamManager _memoryManager = new RecyclableMemoryStreamManager();

        public ServiceImageManager(FarmMasterContext context, IHostingEnvironment env)
        {
            this._context = context;
            this._environment = env;
        }

        public async Task<Image> UploadFromForm(IFormFile file)
        {
            using(var stream = new RecyclableMemoryStream(_memoryManager, string.Empty, (int)file.Length))
            {
                await file.CopyToAsync(stream);

                // Check it's an image.
                stream.Position = 0;
                var format = SL.Image.DetectFormat(stream);
                if(format == null)
                    return null;

                // Upload it.
                stream.Position = 0;
                var data = new ImageData
                {
                    Data = stream.GetBuffer()
                };

                stream.Position = 0;
                var header = SL.Image.Identify(stream);
                var image = new Image 
                { 
                    Width = (ushort)header.Width,
                    Height = (ushort)header.Height,
                    ImageData = data
                };

                // Save changes, then untrack the data so the eventually recycled buffer isn't referenced to.
                await this._context.AddAsync(data);
                await this._context.AddAsync(image);
                await this._context.SaveChangesAsync();

                this._context.Entry(data).State = EntityState.Detached;
                data.Data = null;

                return image;
            }
        }

        public string GetFileNameForImageAndSize(Image image, ushort width, ushort height)
        {
            return $"db_cache/{image.ImageId}_{image.ImageDataId}_from_{image.Width}_{image.Height}_to_{width}_{height}.jpg";
        }

        public async Task<string> ResizeToPhysicalFile(Image image, ushort width, ushort height)
        {
            var fileName = this.GetFileNameForImageAndSize(image, width, height);
            var fileInfo = this._environment.WebRootFileProvider.GetFileInfo(fileName);

            // Download image from the database, resize it, save it to a file.
            if(!fileInfo.Exists)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fileInfo.PhysicalPath));

                var data = await this._context.ImageData.AsNoTracking().FirstOrDefaultAsync(d => d.ImageDataId == image.ImageDataId);
                if(data == null)
                    throw new InvalidOperationException("The image asked for apparently doesn't exist.");

                using(var resized = SL.Image.Load<Rgb24>(data.Data))
                {
                    resized.Mutate(ctx => ctx.Resize(width, height));
                
                    using(var file = File.Create(fileInfo.PhysicalPath))
                        resized.Save(file, new JpegEncoder{ Quality = 75 });
                }

                // Try to let the GC clean up the mess.
                // During a new instance's first run, or during a large addition of new creatures,
                // this function *devours* memory, so this should hopefully keep memory use in check in
                // exchange for some hiccups.
                data.Data = null;
                data = null;
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
            }

            return fileInfo.PhysicalPath;
        }

        public int GetIdFor(Image entity)
        {
            return entity.ImageId;
        }

        public IQueryable<Image> Query()
        {
            return this._context.Images;
        }

        public IQueryable<Image> QueryAllIncluded()
        {
            // We *very intentionally* don't .Include the ImageData, due to the sheer memory usage that'd cause.
            // In fact, that's the very reason we have a seperate table just for image data.
            return this.Query();
        }

        public void Update(Image entity)
        {
            this._context.Update(entity);
            this._context.SaveChanges();
        }
    }
}
