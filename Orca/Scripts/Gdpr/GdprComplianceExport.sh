#!/bin/bash

# 1. To run first execute commands 'chmod +x ExportEventData.sh', 'chmod +x ExportStudentDetails.sh' and 'chmod +x GdprComplianceExport.sh'.
# 2. Execute script by running './GdprComplianceExport.sh [user_id]' where [user_id] is the student ID of the student the report is being generated for.
# 3. Change localhost to url of remote server if exporting from remote server.


PGSQL="psql"

if [ -z $1 ] ; then
  echo "Student ID required! Usage: $0 <student_id>" && exit 1;
fi

read -p "Enter IP address or hostname of the PostgreSQL server: " HOSTNAME

read -p "Enter the port: " PORT

read -p "Enter PostgreSQL username: " USERNAME

read -s -p "Enter password for PostgreSQL user: " PASSWORD

read -p "Enter the PostgreSQL database name: " DBNAME


./ExportStudentDetails.sh $1 ${PGSQL} ${HOSTNAME} ${PORT} ${USERNAME} ${PASSWORD} ${DBNAME} && ./ExportEventData.sh $1 ${PGSQL} ${HOSTNAME} ${PORT} ${USERNAME} ${PASSWORD} ${DBNAME}
