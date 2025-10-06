#!/bin/bash

echo "⏳ Waiting for SQL Server to start..."

TRIES=60
DBSTATUS=1
ERRCODE=1
i=0

while [[ $DBSTATUS -ne 0 ]] && [[ $i -lt $TRIES ]]; do
    i=$((i+1))
    
    # Verificar se todas as databases estão online
    DBSTATUS=$(/opt/mssql-tools/bin/sqlcmd -h -1 -t 1 -U sa -P $MSSQL_SA_PASSWORD -Q "SET NOCOUNT ON; SELECT COALESCE(SUM(state), 0) FROM sys.databases" 2>/dev/null) || DBSTATUS=1
    
    if [[ $DBSTATUS -ne 0 ]]; then
        echo "⏳ SQL Server not ready yet... (attempt $i/$TRIES)"
        sleep 2s
    fi
done

if [ $DBSTATUS -ne 0 ]; then 
    echo "❌ SQL Server took more than $((TRIES * 2)) seconds to start up or databases are not in ONLINE state"
    exit 1
fi

echo "✅ SQL Server is ready! Running initialization script..."

/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $MSSQL_SA_PASSWORD -d master -i /usr/src/app/init-database.sql

if [ $? -eq 0 ]; then
    echo "🎉 Database initialization completed successfully!"
else
    echo "❌ Database initialization failed!"
    exit 1
fi 