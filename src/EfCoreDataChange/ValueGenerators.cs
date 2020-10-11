using System;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EfCoreDataChange
{
    internal class ValueGeneratorDataNow : ValueGenerator
    {
        public override bool GeneratesTemporaryValues => false;
        protected override object NextValue(EntityEntry entry)
        {
            return DateTime.UtcNow;
        }
    }
}