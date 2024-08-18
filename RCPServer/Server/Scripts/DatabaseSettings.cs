using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Supported RCPKit database types
/// </summary>
public enum DatabaseType
{
    SQLite,
    MySQL
}

/// <summary>
/// Static class containing current database configuration. Configure this class for your database server.
/// </summary>
static class DatabaseSettings
{
    /// <summary>
    /// Current database type. Change this to fit your server. SQLite is recommended for smaller servers, MySQL is essential for 
    /// larger or multi noded servers.
    /// </summary>
    public const DatabaseType DatabaseProvider = DatabaseType.MySQL;

    /// <summary>
    /// SQLite Accounts Database location
    /// </summary>
    public const String SQLiteAccountsFile = @"Data/Server Data/Accounts/AccountDatabase.s3db";

    /// <summary>
    /// MySQL Server IP
    /// </summary>
    public const String MySQLServerAddress = @"127.0.0.1;";

    /// <summary>
    /// MySQL Accounts Database name
    /// </summary>
    public const String MySQLDatabaseNameAccounts = @"RCPKitAccounts;";

    /// <summary>
    /// MySQL username
    /// </summary>
    public const String MySQLUserName = @"User;";

    /// <summary>
    /// MySQL password
    /// </summary>
    public const String MySQLPassword = @"Pass;";

    /// <summary>
    /// MySQL Port (Default 3306)
    /// </summary>
    public const String MySQLPort = @"3306;";

    /// <summary>
    /// Enable account server polling. This will increase the load on the database server, but it allows you to make real time
    /// changes to accounts.
    /// </summary>
    public static bool AccountUpdatePollingEnabled = false;

    /// <summary>
    /// How many entries to update in between account update polls. Each poll will sequentially read more of the database.
    /// </summary>
    /// 
    public const int AccountUpdatesPerQuery = 100;

    /// <summary>
    /// Database updating delay in seconds.
    /// </summary>
    public const int AccountsPollingDelay = 60;
}