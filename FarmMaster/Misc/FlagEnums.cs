﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace FarmMaster.Misc
{
    public enum SaveChanges
    {
        Yes,
        No
    }

    public enum CouldDelete
    {
        Yes,
        No
    }

    public enum IsUnique
    {
        Yes,
        No
    }

    public enum AlsoDelete
    {
        Yes,
        No
    }

    public enum TokenActionResult
    {
        Success,
        Expired,
        Failed
    }
}
