using System;
using System.IO;

namespace YPS.Droid.Dependencies
{
    public static class SQLite_Android
    {
        public static readonly string DbFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "yps.db");
    }
}