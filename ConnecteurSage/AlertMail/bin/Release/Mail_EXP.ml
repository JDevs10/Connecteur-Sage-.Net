<?xml version="1.0"?>
<CustomMailRecap xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <MailType>Mail_EXP</MailType>
  <Client>TABLEWEAR</Client>
  <Subject>Erreur d'export des documents</Subject>
  <DateTimeCreated>12-02-2020 11:30</DateTimeCreated>
  <DateTimeModified />
  <Lines>
    <LineNumber>0</LineNumber>
    <DocumentReference />
    <DocumentErrorMessage>ERROR [42000] [Microsoft][ODBC Driver 13 for SQL Server][SQL Server]Échec de la connexion pour l'utilisateur 'CFCI\Administrateur'. Raison : le serveur est en mode mono-utilisateur. Seul un administrateur peut se connecter à ce moment-là.
ERROR [01S00] [Microsoft][ODBC Driver 13 for SQL Server]Attribut de chaîne de connexion non valide
ERROR [42000] [Microsoft][ODBC Driver 13 for SQL Server][SQL Server]Échec de la connexion pour l'utilisateur 'CFCI\Administrateur'. Raison : le serveur est en mode mono-utilisateur. Seul un administrateur peut se connecter à ce moment-là.
ERROR [01S00] [Microsoft][ODBC Driver 13 for SQL Server]Attribut de chaîne de connexion non valide</DocumentErrorMessage>
    <DocumentErrorStackTrace>   à System.Data.Odbc.OdbcConnection.HandleError(OdbcHandle hrHandle, RetCode retcode)
   à System.Data.Odbc.OdbcConnectionHandle..ctor(OdbcConnection connection, OdbcConnectionString constr, OdbcEnvironmentHandle environmentHandle)
   à System.Data.Odbc.OdbcConnectionOpen..ctor(OdbcConnection outerConnection, OdbcConnectionString connectionOptions)
   à System.Data.Odbc.OdbcConnectionFactory.CreateConnection(DbConnectionOptions options, DbConnectionPoolKey poolKey, Object poolGroupProviderInfo, DbConnectionPool pool, DbConnection owningObject)
   à System.Data.ProviderBase.DbConnectionFactory.CreateConnection(DbConnectionOptions options, DbConnectionPoolKey poolKey, Object poolGroupProviderInfo, DbConnectionPool pool, DbConnection owningConnection, DbConnectionOptions userOptions)
   à System.Data.ProviderBase.DbConnectionFactory.CreateNonPooledConnection(DbConnection owningConnection, DbConnectionPoolGroup poolGroup, DbConnectionOptions userOptions)
   à System.Data.ProviderBase.DbConnectionFactory.TryGetConnection(DbConnection owningConnection, TaskCompletionSource`1 retry, DbConnectionOptions userOptions, DbConnectionInternal oldConnection, DbConnectionInternal&amp; connection)
   à System.Data.ProviderBase.DbConnectionInternal.TryOpenConnectionInternal(DbConnection outerConnection, DbConnectionFactory connectionFactory, TaskCompletionSource`1 retry, DbConnectionOptions userOptions)
   à System.Data.ProviderBase.DbConnectionClosed.TryOpenConnection(DbConnection outerConnection, DbConnectionFactory connectionFactory, TaskCompletionSource`1 retry, DbConnectionOptions userOptions)
   à System.Data.ProviderBase.DbConnectionInternal.OpenConnection(DbConnection outerConnection, DbConnectionFactory connectionFactory)
   à System.Data.Odbc.OdbcConnection.Open()
   à importPlanifier.Classes.ExportCommandes.ExportCommande(List`1 recapLinesList_new)</DocumentErrorStackTrace>
    <FileName />
    <FilePath>D:\EBP\EDI\CONNECTEUR_TB\LOG\LOG_Export\COMMANDE\LOG_Export_Commande_12-02-2020 11.30.03.txt</FilePath>
    <Increment>1</Increment>
  </Lines>
</CustomMailRecap>