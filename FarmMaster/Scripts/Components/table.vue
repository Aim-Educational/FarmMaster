<template>
    <table>
        <thead>
            <tr>
                <th v-for="row in rows"
                    :key="row.name + row.bind"
                    :style="{ 'width': rowWidth }">
                    {{ row.name }}
                    
                    <div class="sort" v-if="row.sort" :data-name="row.name" @click="sortRow">
                        <i class="las"
                           :class="{
                               'la-minus':      !isRowSorted(row),
                               'la-arrow-down': isRowSorted(row) && !isRowSortedAsc(row),
                               'la-arrow-up':   isRowSorted(row) && isRowSortedAsc(row)
                           }">
                           </i>
                    </div>
                </th>
            </tr>
        </thead>
        <tbody>
            <tr v-for="(obj, index) in sortedValues"
                :key="index">
                <td v-for="row in rows"
                    :key="row.name + index + row.bind">
                    <!--Allow values to contain links.-->
                    <a v-if="obj[row.bind] && obj[row.bind].href"
                       :href="obj[row.bind].href">
                        {{ obj[row.bind].value }}
                    </a>
                    <template v-else>
                        {{ obj[row.bind] }}
                    </template>
                </td>
            </tr>
        </tbody>
        <tfoot>
            <tr>
                <slot name="tfoot"></slot>
            </tr>
        </tfoot>
    </table>
</template>

<script>
const ROWT_EXAMPLE = {
    name: "Username",
    bind: "username", // e.g. This row will bind to "values.username", where "values" is the "values" prop passed to the table.
    sort: true, // Use 'true' for a default sort, pass a function for a custom sort. (a, b) => a > b
};

export default {
    data() {
        return {
            sortedRow: {
                row: null,
                asc: false
            }
        }
    },

    props: {
        rows: {
            type: Array, // Array of RowT, see ROWT_EXAMPLE
            required: true
        },
        values: Array, // Array of user defined objects
    },

    computed: {
        rowWidth() {
            return Math.round(100 / this.rows.length) + "%";
        },

        sortedValues() {
            if(!this.sortedRow.row)
                return this.values;

            const sorted = this.values.slice(); // Copy
            sorted.sort((a, b) => {
                let aValue = a[this.sortedRow.row.bind];
                let bValue = b[this.sortedRow.row.bind];

                // Handle values that might be objects
                if(aValue.value)
                    aValue = aValue.value;
                if(bValue.value)
                    bValue = bValue.value;

                return (typeof this.sortedRow.row.sort === "function")
                       ? this.sortedRow.row.sort(aValue, bValue)
                       : aValue > bValue;
            });
            return (this.sortedRow.asc) ? sorted.reverse() : sorted;
        }
    },
    
    methods: {
        isRowSorted(row) {
            return this.sortedRow.row === row;
        },

        isRowSortedAsc(row) {
            return this.isRowSorted(row) && this.sortedRow.asc;
        },

        sortRow(event) {
            let rowSortIcon = event.target;
            if(!rowSortIcon.dataset.name) // If they click directly on the icon, then event.target will be the <i>, so we need the parent instead
                rowSortIcon = rowSortIcon.parentNode;

            const rowName = rowSortIcon.dataset.name;
            const prevRow = this.sortedRow.row;
            this.sortedRow.row = this.rows[this.rows.findIndex(r => r.name === rowName)];
            
            if(prevRow !== this.sortedRow.row)
                this.sortedRow.asc = false;
            else if(!this.sortedRow.asc)
                this.sortedRow.asc = true;
            else
                this.sortedRow.row = null;
        }
    }
}
</script>