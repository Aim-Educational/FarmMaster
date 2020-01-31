using Microsoft.EntityFrameworkCore.Migrations;

namespace Business.Migrations
{
    public partial class GroupScript_NOT_and_BREEDIS : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // oops. Oh well.
            migrationBuilder.InsertData(
                table: "EnumRolePermissions",
                columns: new[] { "EnumRolePermissionId", "Description", "InternalName" },
                values: new object[] { 31, "Use (create/edit) Group Scripts", "use_group_script" });

            migrationBuilder.Sql(@"
CREATE OR REPLACE FUNCTION public.sp_animalgroupscriptfilter(
	filtercode text)
    RETURNS SETOF ""Animals"" 
    LANGUAGE 'plpgsql'

    COST 100
    VOLATILE 
    ROWS 1000
AS $BODY$
DECLARE
	query TEXT;
	logicalStack TEXT[]; -- Stack for whether we're using AND, or OR at the moment.
	filterLines TEXT[];
	line TEXT;
	
	usingNOT BOOLEAN; -- Whether the next statement should be inverted.
	firstQueryConcat BOOLEAN; -- Controls whether a leading AND or OR is appended.
	toConcat TEXT;
	
	dateParam TIMESTAMP; -- Used for things like BORN AFTER
	intParam INTEGER;    -- SPECIES_IS
BEGIN
	filterLines := STRING_TO_ARRAY(filterCode, '~');
	query := _sp_AnimalGroupScriptBaseQueryString();
	
	FOREACH line IN ARRAY filterLines LOOP
		IF line = 'AND' OR line = 'OR' THEN
			-- Prevents the root ""WHERE"" from becoming, e.g. ""WHERE AND"" which doesn't work.
			-- Also prevents ""(AND ...)"" from happening.
			IF NOT (logicalStack = '{}') AND NOT firstQueryConcat THEN
				query := CONCAT(query, line);
			END IF;
		
			logicalStack := ARRAY_APPEND(logicalStack, line);
			query := CONCAT(query, '(');
			firstQueryConcat := TRUE;
		ELSIF line = 'NOT' THEN
			usingNOT := TRUE;
		ELSIF line = 'END' THEN
			IF logicalStack = '{}' THEN
				RAISE EXCEPTION 'Unbalanced amount of AND/ORs, and ENDs. Stray END found.';
			END IF;
		
			logicalStack := logicalStack[1:ARRAY_UPPER(logicalStack, 1) - 1];
			query := CONCAT(query, ')');
			firstQueryConcat := FALSE;
		ELSE
			IF line = '' THEN CONTINUE; END IF;
				
			IF logicalStack = '{}' THEN
				RAISE EXCEPTION 'End of logical stack, cannot process command: %', line;
			ELSIF line ILIKE 'BORN_AFTER%' THEN
				dateParam := SPLIT_PART(line, ' ', 2)::timestamp;
				toConcat := _SP_AnimalGroupScriptBornQueryString(dateParam, 'AFTER');
			ELSIF line ILIKE 'BORN_BEFORE%' THEN
				dateParam := SPLIT_PART(line, ' ', 2)::timestamp;
				toConcat := _SP_AnimalGroupScriptBornQueryString(dateParam, 'BEFORE');
			ELSIF line ILIKE 'SPECIES_IS%' THEN
				intParam := SPLIT_PART(line, ' ', 2)::integer;
				toConcat := _SP_AnimalGroupScriptSpeciesIsQueryString(intParam);
			ELSIF line ILIKE 'BREED_IS%' THEN
				intParam := SPLIT_PART(line, ' ', 2)::integer;
				toConcat := _SP_AnimalGroupScriptBreedIsQueryString(intParam);
			END IF;
			
			IF NOT firstQueryConcat THEN
				query := CONCAT(query, logicalStack[ARRAY_UPPER(logicalStack, 1)]);
			ELSE
				firstQueryConcat = FALSE;
			END IF;
			
			IF usingNOT THEN
				query := CONCAT(query, ' NOT ');
				usingNOT := FALSE;
			END IF;
			
			query := CONCAT(query, toConcat, ' ');
		END IF;
	END LOOP;
	
	RETURN QUERY EXECUTE query;
END;
$BODY$;

CREATE OR REPLACE FUNCTION public._sp_animalgroupscriptbasequerystring(
	)
    RETURNS text
    LANGUAGE 'plpgsql'

    COST 100
    IMMUTABLE 
AS $BODY$
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
		INNER JOIN ""MapBreedToAnimals""
			ON ""MapBreedToAnimals"".""AnimalId"" = ""Animals"".""AnimalId""
	WHERE';
END
$BODY$;

CREATE OR REPLACE FUNCTION public._SP_AnimalGroupScriptBreedIsQueryString(
	intparam integer)
    RETURNS text
    LANGUAGE 'plpgsql'

    COST 100
    IMMUTABLE 
AS $BODY$
BEGIN
	RETURN FORMAT('
	(   ""MapBreedToAnimals"".""BreedId"" = %s
 	)', intParam);
END;
$BODY$;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EnumRolePermissions",
                keyColumn: "EnumRolePermissionId",
                keyValue: 31);
        }
    }
}
