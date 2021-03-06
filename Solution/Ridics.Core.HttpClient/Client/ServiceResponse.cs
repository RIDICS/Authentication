﻿namespace Ridics.Core.HttpClient.Client
{
    public class ServiceResponse<T> where T : class
    {
        public string ContentType { get; set; }

        public string StatusCode { get; set; }

        public T Value { get; set; }
    }
}