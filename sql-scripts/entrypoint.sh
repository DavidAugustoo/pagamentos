#!/bin/bash

echo "🚀 Starting SQL Server with custom initialization..."

/usr/src/app/configure-db.sh &

/opt/mssql/bin/sqlservr 