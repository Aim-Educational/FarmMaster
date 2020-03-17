<template>
    <table>
        <thead>
            <tr>
                <th v-if="selection !== 'none'"
                    style="max-width: 60px;">
                    <input type="checkbox"
                           v-model="selectAll"
                           v-if="selection === 'multiple'">
                </th>
                <th v-for="row in rows"
                    :key="row.name + row.bind"
                    :style="{ 'width': rowWidth }">
                    {{ row.name }}
                    
                    <div class="sort" v-if="row.sort" :data-name="row.name" @click="onSortRow">
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
            <tr v-for="(obj, index) in values"
                :key="index">
                <!--Display a checkbox if selections are allowed-->
                <td v-if="selection !== 'none'"
                    style="max-width: 60px;">
                    <input type="checkbox" 
                           v-model="selectedValueIndicies[index]"
                           @change="onValueChecked(index)" />
                </td>
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

/**
 * Events:
 *   'selected': {
 *      Emitted whenever a checkbox is checked. Payload is the following object.
 *      
 *      triggerValue:   Object   # The value from `props.values` that was selected. THIS IS NULL if the 'selectAll' checkbox was used. THIS IS NULL if the trigger was caused a column being sorted
 *      selectedValues: Object[] # All values from `props.values` that are currently selected, including the `triggerValue`.
 * 
 *      Use this event to keep track of selected data so you can later perform actions.
 *   },
 * 
 *   'sort': {
 *      Emitted whenever a row needs to be sorted.
 * 
 *      row: Object  # The row from `props.rows` that needs to be sorted. THIS IS NULL if the user doesn't want any sorting anymore.
 *      asc: Boolean # Whether to sort by ascending or descending order.
 * 
 *      Use this event to sort the values you pass to this component.
 *   }
 */
export default {
    data() {
        return {
            sortedRow: {
                row: null,
                asc: false
            },
            selectedValueIndicies: [], // [0] = false means values[0] is not selected, [1] = true means values[1] is.
            selectAll: false,
            ignoreNextSelectAll: false // If true, then make the watch function for selectAll return immediately. Used for UI cohesiveness
        }
    },

    props: {
        rows: {
            type: Array, // Array of RowT, see ROWT_EXAMPLE
            required: true
        },
        values: Array, // Array of user defined objects
        selection: {
            type: String,
            default: "none",
            validator: function (v) {
                return ["none", "single", "multiple"].indexOf(v) !== -1;
            }
        }
    },

    watch: {
        selectAll(newValue, old) {
            if(this.ignoreNextSelectAll) {
                this.ignoreNextSelectAll = false;
                return;
            }

            for(let i = 0; i < this.values.length; i++)
                this.selectedValueIndicies[i] = newValue;

            this.$emit("selected", { triggerValue: null, selectedValues: newValue ? this.values : [] });
        }
    },

    computed: {
        rowWidth() {
            return Math.round(100 / this.rows.length) + "%";
        },

        selectedValues() {
            return this.values.filter((v, index) => this.selectedValueIndicies[index]);
        }
    },
    
    methods: {
        isRowSorted(row) {
            return this.sortedRow.row === row;
        },

        isRowSortedAsc(row) {
            return this.isRowSorted(row) && this.sortedRow.asc;
        },

        onSortRow(event) {
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

            // If selectAll is already false, then the watcher won't trigger, so we need to deselect stuff anyway just in case.
            for(let i = 0; i < this.selectedValueIndicies.length; i++)
                this.selectedValueIndicies[i] = false;

            if(this.selectAll)
                this.ignoreNextSelectAll = true;

            this.selectAll = false;
            this.$emit("selected", { triggerValue: null, selectedValues: [] });
            this.$emit("sort",     { row: this.sortedRow.row, asc: this.sortedRow.asc });
        },

        onValueChecked(index) {
            // In 'multiple' selection, set `selectAll` is every checkbox is checked.
            if(this.selection == "multiple") {
                let isAllSelected =  this.selectedValueIndicies.length === this.values.length;
                
                // Array.every doesn't handle undefined, and Vue.js happens to fill gaps in the array with undefined, so...
                // Responsible Edge Case:
                //      Select a checkbox for the first time since the table was loaded, suddenly the "selectAll" checkbox is marked.
                //      This is due to Array.every only passing in non-undefined values, which we need to keep as they are falsey values.
                if(isAllSelected) {
                    for(let i = 0; i < this.selectedValueIndicies.length; i++){
                        if(!this.selectedValueIndicies[i]) {
                            isAllSelected = false;
                            break;
                        }
                    }
                }

                // If we're triggering a change to selectAll (so by proxy, calling its watcher function), then we need
                // to tell the watcher function not to trigger, otherwise we might unselect everything by accident.
                // Responsible Edge Case: 
                //      Have all checkboxes selected, deselect any value, suddenly all values will deselect.
                if(isAllSelected !== this.selectAll) {
                    this.ignoreNextSelectAll = true;
                    this.selectAll = isAllSelected
                }
            }

            // If we're on 'single' selection, uncheck all other buttons if another button is checked.
            if(this.selection === "single") {
                for(let i = 0; i < this.selectedValueIndicies.length; i++)
                {
                    if(i !== index)
                        this.selectedValueIndicies[i] = false;
                }
            }

            this.$emit("selected", { triggerValue: (index > 0) ? this.values[index] : null, selectedValues: this.selectedValues });
        }
    },

    // Util functions for implementors
    util: {
        /**
         * Given a value object (a direct child of `prop.values`) and a row, attempts to get
         * the value for the given row.
         * 
         * Since values can either be given flat out (e.g. { username: "Sealab" }) or given as objects with 
         * special properties (e.g. { username: { value: "Sealab", href: "/Account/User/0" } }) this function
         * can be used to painlessly support both.
         * 
         * CAN RETURN undefined if the value doesn't exist.
         */
        getRowValue(valueObject, ownerRow) {
            let value = valueObject[ownerRow.bind];
            if(value && value.value) // Value might be a special object
                value = value.value;

            return value;
        },

        sortRowValue(a, b, row) {
            return (typeof row.sort === "function")
                   ? row.sort(a, b)
                   : a < b;
        }
    }
}
</script>