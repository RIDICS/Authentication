﻿using System;

namespace Ridics.Authentication.Database.Migrations.Migrations
{
    public class MigrationException : Exception
    {
        public MigrationException()
        {
        }

        public MigrationException(string message) : base(message)
        {
        }

        public MigrationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}