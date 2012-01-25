﻿using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace MyFrameWork.NHib.Mapping.Conventions
{
    public class PrimaryKeyConvention : IIdConvention
    {
        public void Apply(IIdentityInstance instance)
        {
            instance.Column("Id");
            
            instance.UnsavedValue("0");
            
            // instance.GeneratedBy.HiLo("1000");
        }
    }
}