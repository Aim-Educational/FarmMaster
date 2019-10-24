using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Business.Model
{
    public class Image
    {
        [Key]
        public int ImageId { get; set; }

        [Required]
        public int ImageDataId { get; set; }
        public ImageData ImageData { get; set; }

        [Required]
        public ushort Width { get; set; }

        [Required]
        public ushort Height { get; set; }
    }
}
