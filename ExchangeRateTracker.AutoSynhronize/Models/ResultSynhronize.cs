﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRateTracker.AutoSynhronize.Models
{
    public class ResultSynhronize
    {
        public bool IsSuccess { get; set; } = true;

        public string Message { get; set; } = string.Empty;
    }
}
