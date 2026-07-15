#!/bin/bash
set -euo pipefail

# The MVC app's Serilog MSSqlServer sink logs to a separate "Serilog" database.
# It auto-creates the Logs table but NOT the database, and EF migrations don't
# create it either, so the app fails to start without this.
/opt/mssql-tools18/bin/sqlcmd -S sqlserver -U sa -P "$MSSQL_SA_PASSWORD" -C \
  -Q "IF DB_ID('Serilog') IS NULL CREATE DATABASE [Serilog];"

echo "Init complete: Serilog database ensured."
