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