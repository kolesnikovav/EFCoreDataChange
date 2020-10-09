using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EfCoreDataChange
{
    internal interface ITrackable
    {
        EntityState State {get;set;}

        DateTime Date {get;set;}
    }

}