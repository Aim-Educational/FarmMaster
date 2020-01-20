using Microsoft.EntityFrameworkCore.Migrations;

namespace Business.Migrations
{
    public partial class GroupScriptStoredProcedures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Write and test in PGAdmin first, before making migrations for these things.
            migrationBuilder.Sql(
                @"
CREATE OR REPLACE FUNCTION SP_AnimalGroupScriptFilter(
	filterCode TEXT
)
RETURNS SETOF ""Animals""
AS $$
DECLARE
	query TEXT;
	logicalStack TEXT[]; -- Stack for whether we're using AND or OR at the moment.
	filterLines TEXT[];
	line TEXT;
	
	firstQueryConcat BOOLEAN; -- Controls whether a leading AND or OR is appended.
	toConcat TEXT;
	
	dateParam TIMESTAMP; -- Used for things like BORN AFTER
BEGIN
	filterLines := STRING_TO_ARRAY(filterCode, '~');
	query := _sp_AnimalGroupScriptBaseQueryString();
	
	FOREACH line IN ARRAY filterLines LOOP
		IF line = 'AND' OR line = 'OR' THEN
			IF NOT (logicalStack = '{}') AND NOT firstQueryConcat THEN
				query := CONCAT(query, line);
			END IF;
		
			logicalStack := ARRAY_APPEND(logicalStack, line);
			query := CONCAT(query, '(');
			firstQueryConcat := TRUE;
		ELSIF line = 'END' THEN
			IF logicalStack = '{}' THEN
				RAISE EXCEPTION 'Unbalanced amount of AND/ORs, and ENDs. Stray END found.';
			END IF;
		
			logicalStack := logicalStack[1:ARRAY_UPPER(logicalStack, 1) - 1];
			query := CONCAT(query, ')');
			firstQueryConcat := FALSE;
		ELSE
			IF line = '' THEN CONTINUE; 
			ELSIF logicalStack = '{}' THEN
				RAISE EXCEPTION 'End of logical stack, cannot process command: %', line;
			ELSIF line ILIKE 'BORN_AFTER%' THEN
				dateParam := SPLIT_PART(line, ' ', 2)::timestamp;
				toConcat := _SP_AnimalGroupScriptBornQueryString(dateParam, 'AFTER');
			ELSIF line ILIKE 'BORN_BEFORE%' THEN
				dateParam := SPLIT_PART(line, ' ', 2)::timestamp;
				toConcat := _SP_AnimalGroupScriptBornQueryString(dateParam, 'BEFORE');
			END IF;
			
			IF NOT firstQueryConcat THEN
				query := CONCAT(query, logicalStack[ARRAY_UPPER(logicalStack, 1)], toConcat);
			ELSE
				query := CONCAT(query, toConcat, ' ');
				firstQueryConcat = FALSE;
			END IF;
		END IF;
	END LOOP;
	
	RETURN QUERY EXECUTE query;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION _SP_AnimalGroupScriptBornQueryString(
	dateParam TIMESTAMP,
	beforeOrAfter TEXT
)
RETURNS TEXT
IMMUTABLE
AS $$
DECLARE
	op TEXT;
BEGIN
	IF beforeOrAfter = 'AFTER' THEN
		op := '>';
	ELSE
		op := '<';
	END IF;

	RETURN FORMAT('
	(   ""LifeEvents"".""Name"" = ''Born'' 
        AND ""LifeEvents"".""Flags"" & 2 = 2 
        AND ""LifeEventEntries"".""LifeEventId"" = ""LifeEvents"".""LifeEventId""
        AND ""LifeEventDynamicFieldValues"".""LifeEventEntryId"" = ""LifeEventEntries"".""LifeEventEntryId""
        AND (""LifeEventDynamicFieldValues"".""Value""::json->>''v'')::timestamp with time zone %s %L::timestamp
 	)', op, dateParam);
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION _SP_AnimalGroupScriptBaseQueryString()
RETURNS TEXT
IMMUTABLE 
AS $$
BEGIN
	RETURN 
	'SELECT ""Animals"".*
		FROM ""Animals""
		INNER JOIN ""MapLifeEventEntryToAnimals""
			ON ""MapLifeEventEntryToAnimals"".""AnimalId"" = ""Animals"".""AnimalId""
			INNER JOIN ""LifeEventEntries""
				ON ""LifeEventEntries"".""LifeEventEntryId"" = ""MapLifeEventEntryToAnimals"".""LifeEventEntryId""
				INNER JOIN ""LifeEvents""
					ON ""LifeEvents"".""LifeEventId"" = ""LifeEventEntries"".""LifeEventId""
				INNER JOIN ""LifeEventDynamicFieldValues"" 
					ON ""LifeEventDynamicFieldValues"".""LifeEventEntryId"" = ""LifeEventEntries"".""LifeEventEntryId""
					INNER JOIN ""LifeEventDynamicFieldInfo""
						ON ""LifeEventDynamicFieldInfo"".""LifeEventDynamicFieldInfoId"" = ""LifeEventDynamicFieldValues"".""LifeEventDynamicFieldInfoId""
	WHERE';
END
$$ LANGUAGE plpgsql;"    
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP FUNCTION _SP_AnimalGroupScriptBaseQueryString();
                DROP FUNCTION _SP_AnimalGroupScriptBornQueryString();
                DROP FUNCTION SP_AnimalGroupScriptFilter();
            ");
        }
    }
}
