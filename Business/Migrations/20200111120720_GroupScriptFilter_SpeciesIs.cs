using Microsoft.EntityFrameworkCore.Migrations;

namespace Business.Migrations
{
    public partial class GroupScriptFilter_SpeciesIs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"CREATE OR REPLACE FUNCTION public.sp_animalgroupscriptfilter(
	filtercode text)
    RETURNS SETOF ""Animals"" 
    LANGUAGE 'plpgsql'

    COST 100
    VOLATILE 
    ROWS 1000
AS $BODY$
DECLARE
	query TEXT;
	logicalStack TEXT[]; -- Stack for whether we're using AND or OR at the moment.
	filterLines TEXT[];
	line TEXT;
	
	firstQueryConcat BOOLEAN; -- Controls whether a leading AND or OR is appended.
	toConcat TEXT;
	
	dateParam TIMESTAMP; -- Used for things like BORN AFTER
	intParam INTEGER;    -- SPECIES_IS
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
			ELSEIF line ILIKE 'SPECIES_IS%' THEN
				intParam := SPLIT_PART(line, ' ', 2)::integer;
				toConcat := _SP_AnimalGroupScriptSpeciesIsQueryString(intParam);
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
$BODY$;

CREATE OR REPLACE FUNCTION public._SP_AnimalGroupScriptSpeciesIsQueryString(
	intParam INTEGER)
    RETURNS text
    LANGUAGE 'plpgsql'

    COST 100
    IMMUTABLE 
AS $BODY$
BEGIN
	RETURN FORMAT('
	(   ""Animals"".""SpeciesId"" = %s
 	)', intParam);
END;
$BODY$;"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP FUNCTION _SP_AnimalGroupScriptSpeciesIsQueryString();
                DROP FUNCTION SP_AnimalGroupScriptFilter();
            ");
        }
    }
}
