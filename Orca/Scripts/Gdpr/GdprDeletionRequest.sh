#!/bin/bash

# 1. To run, first execute command 'chmod +x GdprDeletionRequest.sh'
# 2. Execute script by running './GdprDeletionRequest.sh [user_id]' where [user_id] is the student ID of the student the report is being generated for.
# 3. Change localhost to url of remote server if exporting from remote server.


PGSQL="psql"

if [ -z $1 ] ; then
  echo "Student ID required! Usage: $0 <student_id>" && exit 1;
fi

read -p "Enter IP address or hostname of the PostgreSQL server: " HOSTNAME

read -p "Enter the port:" PORT

read -p "Enter PostgreSQL username: " USERNAME

read -s -p "Enter password for PostgreSQL user: " PASSWORD

read -p "Enter the PostgreSQL database name: " DBNAME

TABLE="event"

delete_events="delete from ${TABLE} where student_id = '$1'"

TABLE="student"
delete_details="delete from ${TABLE} where id = '$1'"

PGPASSWORD=$PASSWORD ${PGSQL} -h ${HOSTNAME} -p ${PORT} -U ${USERNAME} -d ${DBNAME} -c "${delete_events}"
PGPASSWORD=$PASSWORD ${PGSQL} -h ${HOSTNAME} -p ${PORT} -U ${USERNAME} -d ${DBNAME} -c "${delete_details}"

echo "Data for Student with ID $1 has been deleted from the ORCA Database."