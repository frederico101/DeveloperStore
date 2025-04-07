# Use the official SQL Server 2019 image as the base image
FROM mcr.microsoft.com/mssql/server:2019-latest

# Expose the SQL Server port
EXPOSE 1433

# Start SQL Server
CMD ["/opt/mssql/bin/sqlservr"]