﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Model;

namespace FarmMaster.Models
{
    public class LifeEventIndexViewModel : ViewModelWithMessage
    {
        public IEnumerable<LifeEvent> LifeEvents { get; set; }
    }
}