using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace YPS.Parts2y.Parts2y_SQLITE
{
    public interface ISqlite
    {
        SQLiteConnection GetConnection();

    }
}
