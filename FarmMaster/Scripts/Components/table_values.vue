<template>
    <fm-table :rows="rows"
              :selection="selection"
              :values="sortedValues"
              @sort="onSort"
              v-on="$listeners">
        <template v-slot:tfoot>
            <slot name="tfoot"></slot>
        </template>
    </fm-table>
</template>

<script>
import FarmTableBase from "./table_base";

/**
 * This is the simplest table to use, it simply works off of the values you pass to it.
 * 
 * All events from the base component are forwarded.
 * 
 * No new events exist.
 * 
 * All properties for the base component are forwarded.
 * 
 * The reason you'd probably want to use something more specialised like the graphql table, is due to how
 * sorting is handled.
 * 
 * This table will create a shallow copy of your values, and then sort them in memory.
 * 
 * The GraphQL table however doesn't have access to the entire dataset, so it'd instead instruct the server
 * to order things for us.
 */
export default {
    props: {
        rows: FarmTableBase.props.rows,
        selection: FarmTableBase.props.selection,
        values: Array,   // Array of objects.
    },

    data: function() {
        return {
            sortedValues: []
        }
    },

    methods: {
        onSort(event) {
            this.sortedValues = this.values.slice();
            if(!event.row)
                return;

            this.sortedValues.sort((a, b) => {
                let aValue = FarmTableBase.util.getRowValue(a, event.row);
                let bValue = FarmTableBase.util.getRowValue(b, event.row);

                return FarmTableBase.util.sortRowValue(aValue, bValue, event.row);
            });

            if(!event.asc)
                this.sortedValues.reverse();
        }
    },

    created() {
        this.sortedValues = this.values;
    },

    components: {
        'fm-table': FarmTableBase
    }
}
</script>