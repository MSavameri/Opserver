﻿using System;

namespace Opserver.Data.Redis
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class RedisInfoPropertyAttribute : Attribute
    {
        public string PropertyName { get; }
        public RedisInfoPropertyAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}
