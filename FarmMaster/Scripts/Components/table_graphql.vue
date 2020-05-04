<template>
    <fm-table :rows="rows"
              :selection="selection"
              :values="values"
              @sort="onSort"
              v-on="$listeners"
              ref="table">
        <template v-slot:tfoot>
            <slot name="tfoot"></slot>
        </template>
    </fm-table>
</template>

<script>
import FarmTableBase from "./table_base";

/**
 */
export default {
    props: {
        rows: FarmTableBase.props.rows,
        selection: FarmTableBase.props.selection,
        queryRoot: String,
        itemsQuery: String
    },

    data() {
        return {
            pageInfo: { startCursor: 0, endCursor: 0 },
            values: []
        }
    },

    computed: {
        fullQuery() {
            return `
            query GetItems($after:String!, $order: String) {
                ${this.queryRoot}(after: $after, order: $order) {
                    items {
                        ${this.itemsQuery}
                    }
                    pageInfo {
                        endCursor
                        hasNextPage
                        hasPreviousPage
                        startCursor
                    }
                }
            }`;
        }
    },

    methods: {
        onSort(event) {
            libs.Ajax
                .graphql(this.fullQuery,
                { 
                    after: ""+this.pageInfo.startCursor, 
                    order: (!event.row) ? null : (event.asc) ? event.row.bind : event.row.bind + "_desc"
                })
                .then(json => {
                    var data = json[this.queryRoot];
                    this.values = data.items;
                    this.pageInfo = data.pageInfo;
                })
                .catch(e => console.log(e));
        }
    },

    created() {
        this.onSort({});
    },

    components: {
        'fm-table': FarmTableBase
    }
}
</script>