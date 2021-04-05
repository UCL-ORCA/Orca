#!/bin/bash
TABLE="student"
mkdir -p ./Record_data/Student_information

EXPORT_PATH=./Record_data/Student_information/$1"_DETAILS_GDPR_RECORD".csv
echo "Downloading student $1 data from $7 in pgsql..."
select_details="select * from ${TABLE} where id = '$1'"
PGPASSWORD=$6 $2 -h $3 -p $4 -U $5 -d $7 -c "\copy (${select_details}) TO ${EXPORT_PATH} CSV HEADER"

#echo the infomation
echo "Convert $7 into $1_DETAILS_GDPR_RECORD.csv."
echo "Done successfully! Please check the file!"
