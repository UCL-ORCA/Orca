#!/bin/bash
TABLE="event"
mkdir -p ./Record_data/Student_Events

EXPORT_PATH=./Record_data/Student_Events/$1"_EVENTS_GDPR_RECORD".csv
echo "Downloading student $1 data from $7 in pgsql..."
select_events="select * from ${TABLE} where student_id = '$1'"
PGPASSWORD=$6 $2 -h $3 -p $4 -U $5 -d $7 -c "\copy (${select_events}) TO ${EXPORT_PATH} CSV HEADER"

#echo the infomation
echo "Convert $7 into $1_EVENTS_GDPR_RECORD.csv."
echo "Done successfully! Please check the file!"
