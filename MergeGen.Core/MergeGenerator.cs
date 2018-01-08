using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MergeGen.Core
{
    public class MergeGenerator
    {
        //list of values
        public List<TableValue> Values {get;set;}
        public List<string> FieldsToUpdate {get;set;}
        
        //Name of the target table
        public string TargetName {get;set;}
        public MergeGenerator() { 
            Values = new List<TableValue>();
            FieldsToUpdate = new List<string>();
        }

        public string BuildMergeStatement()
        {
            StringBuilder statementBuilder = new StringBuilder();
            StringBuilder sourceBuilder = new StringBuilder();
            StringBuilder onTargetBuilder = new StringBuilder();
            StringBuilder updateBuilder = new StringBuilder();
            StringBuilder insertBuilder = new StringBuilder();

            foreach(TableValue val in Values)
            {
                if( sourceBuilder.Length > 0 )
                    sourceBuilder.AppendFormat(format: ",@{0}", arg0: val.FieldName);
                else
                    sourceBuilder.AppendFormat(format: "@{0}", arg0: val.FieldName);                
            }

            statementBuilder.AppendFormat(format: "MERGE {0} as Target USING (select {1} ) as Source({2})",
                arg0: TargetName, arg1: sourceBuilder.ToString(),
                arg2: sourceBuilder.ToString().Replace("@",""));
            statementBuilder.AppendLine();

            //Préparation de la condition
            statementBuilder.AppendLine("ON ");
            
            //Préparation de la requête d'update
            statementBuilder.AppendLine("WHEN MATCHED THEN");
            foreach(var fieldName in FieldsToUpdate)
            {
                if(updateBuilder.Length > 0)
                    updateBuilder.AppendFormat(format: ", {0}=source.{0} ", arg0: fieldName);
                else
                    updateBuilder.AppendFormat(format: " {0}=source.{0} ", arg0: fieldName);
            }
            statementBuilder.AppendLine(updateBuilder.ToString());  

            //Préparation de la requête d'insertion          
            statementBuilder.AppendLine("WHEN NOT MATCHED");
            statementBuilder.AppendFormat(" INSERT INTO {0}({1}) VALUES({2})", 
                arg0: TargetName,
                arg1: sourceBuilder.ToString().Replace("@",""),
                arg2: sourceBuilder.ToString().Replace("@","source.") );

            return statementBuilder.ToString();
        }
        public void AddValue(string fieldName, object value) => Values.Add(new TableValue { FieldName = fieldName, Value = value });

        public void AddValue(string fieldName, object value, bool toUpdate)
        {
            if(Values.Where((v) => v.FieldName.Equals(fieldName)).Count() > 0)
                throw new Exception(string.Format(format: "This field is already on values: {0} .",
                                                    arg0: fieldName));

            AddValue(fieldName, value);
            if(toUpdate)
                FieldsToUpdate.Add(fieldName);
        }

        public void AddFieldToUpdate(string fieldName)
        {
            if(Values.Where((x)=> x.FieldName.Equals(fieldName)).Count() > 0)
                FieldsToUpdate.Add(fieldName);
            else
                throw new Exception(message: "This field is not in the values");
        }
    }
}
