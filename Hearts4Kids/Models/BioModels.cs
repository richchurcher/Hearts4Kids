﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hearts4Kids.Models
{
    public class BioModels
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Bio { get; set; }
        public string ProfilePicAddress { get; set; }
    }
}