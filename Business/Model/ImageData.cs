using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Business.Model
{
    public class ImageData
    {
        [Key]
        public int ImageDataId { get; set; }

        [MaxLength(1024 * 1024 * 5)] // 5MB
        public byte[] Data { get; set; }
    }
}
