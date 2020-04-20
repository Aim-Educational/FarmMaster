var controls=function(e){var t={};function s(n){if(t[n])return t[n].exports;var o=t[n]={i:n,l:!1,exports:{}};return e[n].call(o.exports,o,o.exports,s),o.l=!0,o.exports}return s.m=e,s.c=t,s.d=function(e,t,n){s.o(e,t)||Object.defineProperty(e,t,{enumerable:!0,get:n})},s.r=function(e){"undefined"!=typeof Symbol&&Symbol.toStringTag&&Object.defineProperty(e,Symbol.toStringTag,{value:"Module"}),Object.defineProperty(e,"__esModule",{value:!0})},s.t=function(e,t){if(1&t&&(e=s(e)),8&t)return e;if(4&t&&"object"==typeof e&&e&&e.__esModule)return e;var n=Object.create(null);if(s.r(n),Object.defineProperty(n,"default",{enumerable:!0,value:e}),2&t&&"string"!=typeof e)for(var o in e)s.d(n,o,function(t){return e[t]}.bind(null,o));return n},s.n=function(e){var t=e&&e.__esModule?function(){return e.default}:function(){return e};return s.d(t,"a",t),t},s.o=function(e,t){return Object.prototype.hasOwnProperty.call(e,t)},s.p="",s(s.s=11)}({11:function(e,t,s){"use strict";s.r(t);var n=function(){var e=this.$createElement,t=this._self._c||e;return t("div",{staticClass:"control-window"},[t("header",[t("h1",[this._v(this._s(this.title))])]),this._v(" "),t("div",{staticClass:"content"},[this._t("default")],2)])};function o(e,t,s,n,o,i,r,l){var a,c="function"==typeof e?e.options:e;if(t&&(c.render=t,c.staticRenderFns=s,c._compiled=!0),n&&(c.functional=!0),i&&(c._scopeId="data-v-"+i),r?(a=function(e){(e=e||this.$vnode&&this.$vnode.ssrContext||this.parent&&this.parent.$vnode&&this.parent.$vnode.ssrContext)||"undefined"==typeof __VUE_SSR_CONTEXT__||(e=__VUE_SSR_CONTEXT__),o&&o.call(this,e),e&&e._registeredComponents&&e._registeredComponents.add(r)},c._ssrRegister=a):o&&(a=l?function(){o.call(this,this.$root.$options.shadowRoot)}:o),a)if(c.functional){c._injectStyles=a;var d=c.render;c.render=function(e,t){return a.call(t),d(e,t)}}else{var u=c.beforeCreate;c.beforeCreate=u?[].concat(u,a):[a]}return{exports:e,options:c}}n._withStripped=!0;var i=o({props:{title:String}},n,[],!1,null,null,null);i.options.__file="Scripts/Components/window.vue";var r=i.exports,l=function(){var e=this,t=e.$createElement,s=e._self._c||t;return"edit"===e.mode?s("input",{staticClass:"farm-master",attrs:{name:e.name,type:e.type,placeholder:e.placeholder},domProps:{value:e.value},on:{input:function(t){return e.$emit("input",t.target.value)}}}):s("div",[e._v("\n    "+e._s(this.value)+"\n")])};l._withStripped=!0;var a={vueProps:{viewOrEdit:{type:String,default:"edit",validator:function(e){return-1!==["edit","view"].indexOf(e)}}}},c=o({props:{type:String,name:String,placeholder:String,value:String,mode:a.vueProps.viewOrEdit}},l,[],!1,null,null,null);c.options.__file="Scripts/Components/textbox.vue";var d=c.exports,u=function(){var e,t=this.$createElement,s=this._self._c||t;return s("div",{staticClass:"alert",class:(e={},e[this.type]=!0,e.show=!this.closed,e)},[s("div",{staticClass:"content"},[this._t("default")],2),this._v(" "),s("div",{staticClass:"close",on:{click:this.toggle}},[s("i",{staticClass:"las la-times"})])])};u._withStripped=!0;var h=o({data:function(){return{closed:!1}},props:{type:{type:String,required:!0,validator:function(e){return-1!==["error","info"].indexOf(e)}},show:{type:Boolean,default:!1}},methods:{toggle(){this.closed=!this.closed}},created(){this.closed=!this.show}},u,[],!1,null,null,null);h.options.__file="Scripts/Components/alert.vue";var p=h.exports,m=function(){var e=this,t=e.$createElement;return(e._self._c||t)("fm-table",e._g({ref:"table",attrs:{rows:e.rows,selection:e.selection,values:e.sortedValues},on:{sort:e.onSort},scopedSlots:e._u([{key:"tfoot",fn:function(){return[e._t("tfoot")]},proxy:!0}],null,!0)},e.$listeners))};m._withStripped=!0;var v=function(){var e=this,t=e.$createElement,s=e._self._c||t;return s("table",[s("thead",[s("tr",["none"!==e.selection?s("th",{staticStyle:{"max-width":"60px"}},["multiple"===e.selection?s("input",{directives:[{name:"model",rawName:"v-model",value:e.selectAll,expression:"selectAll"}],attrs:{type:"checkbox"},domProps:{checked:Array.isArray(e.selectAll)?e._i(e.selectAll,null)>-1:e.selectAll},on:{change:function(t){var s=e.selectAll,n=t.target,o=!!n.checked;if(Array.isArray(s)){var i=e._i(s,null);n.checked?i<0&&(e.selectAll=s.concat([null])):i>-1&&(e.selectAll=s.slice(0,i).concat(s.slice(i+1)))}else e.selectAll=o}}}):e._e()]):e._e(),e._v(" "),e._l(e.rows,(function(t){return s("th",{key:t.name+t.bind,style:{width:e.rowWidth}},[e._v("\n                "+e._s(t.name)+"\n                \n                "),t.sort?s("div",{staticClass:"sort",attrs:{"data-name":t.name},on:{click:e.onSortRow}},[s("i",{staticClass:"las",class:{"la-minus":!e.isRowSorted(t),"la-arrow-down":e.isRowSorted(t)&&!e.isRowSortedAsc(t),"la-arrow-up":e.isRowSorted(t)&&e.isRowSortedAsc(t)}})]):e._e()])}))],2)]),e._v(" "),s("tbody",e._l(e.values,(function(t,n){return s("tr",{key:n},["none"!==e.selection?s("td",{staticStyle:{"max-width":"60px"}},[s("input",{directives:[{name:"model",rawName:"v-model",value:e.selectedValueIndicies[n],expression:"selectedValueIndicies[index]"}],attrs:{type:"checkbox"},domProps:{checked:Array.isArray(e.selectedValueIndicies[n])?e._i(e.selectedValueIndicies[n],null)>-1:e.selectedValueIndicies[n]},on:{change:[function(t){var s=e.selectedValueIndicies[n],o=t.target,i=!!o.checked;if(Array.isArray(s)){var r=e._i(s,null);o.checked?r<0&&e.$set(e.selectedValueIndicies,n,s.concat([null])):r>-1&&e.$set(e.selectedValueIndicies,n,s.slice(0,r).concat(s.slice(r+1)))}else e.$set(e.selectedValueIndicies,n,i)},function(t){return e.onValueChecked(n)}]}})]):e._e(),e._v(" "),e._l(e.rows,(function(o){return s("td",{key:o.name+n+o.bind},[t[o.bind]&&t[o.bind].href?s("a",{attrs:{href:t[o.bind].href}},[e._v("\n                    "+e._s(t[o.bind].value)+"\n                ")]):[e._v("\n                    "+e._s(t[o.bind])+"\n                ")]],2)}))],2)})),0),e._v(" "),s("tfoot",[s("tr",[e._t("tfoot")],2)])])};v._withStripped=!0;var f=o({data:()=>({sortedRow:{row:null,asc:!1},selectedValueIndicies:[],selectAll:!1,ignoreNextSelectAll:!1}),props:{rows:{type:Array,required:!0},values:Array,selection:{type:String,default:"none",validator:function(e){return-1!==["none","single","multiple"].indexOf(e)}}},created(){for(const e of this.rows)if(e.sortByDefault){this.sortedRow.row=e,this.sortedRow.asc="asc"===e.sortByDefault,this.$emit("sort",{row:e,asc:this.sortedRow.asc});break}},watch:{selectAll(e,t){if(this.ignoreNextSelectAll)this.ignoreNextSelectAll=!1;else{for(let t=0;t<this.values.length;t++)this.selectedValueIndicies[t]=e;this.$emit("selected",{triggerValue:null,selectedValues:e?this.values:[]})}},values(){this.clearSelection(),this.selectAll=!1}},computed:{rowWidth(){return Math.round(100/this.rows.length)+"%"},selectedValues(){return this.values.filter((e,t)=>this.selectedValueIndicies[t])}},methods:{isRowSorted(e){return this.sortedRow.row===e},isRowSortedAsc(e){return this.isRowSorted(e)&&this.sortedRow.asc},onSortRow(e){let t=e.target;t.dataset.name||(t=t.parentNode);const s=t.dataset.name,n=this.sortedRow.row;this.sortedRow.row=this.rows[this.rows.findIndex(e=>e.name===s)],n!==this.sortedRow.row?this.sortedRow.asc=!1:this.sortedRow.asc?this.sortedRow.row=null:this.sortedRow.asc=!0,this.clearSelection(),this.selectAll&&(this.ignoreNextSelectAll=!0),this.selectAll=!1,this.$emit("sort",{row:this.sortedRow.row,asc:this.sortedRow.asc})},onValueChecked(e){if("multiple"==this.selection){let e=this.selectedValueIndicies.length===this.values.length;if(e)for(let t=0;t<this.selectedValueIndicies.length;t++)if(!this.selectedValueIndicies[t]){e=!1;break}e!==this.selectAll&&(this.ignoreNextSelectAll=!0,this.selectAll=e)}if("single"===this.selection)for(let t=0;t<this.selectedValueIndicies.length;t++)t!==e&&(this.selectedValueIndicies[t]=!1);this.$emit("selected",{triggerValue:e>0?this.values[e]:null,selectedValues:this.selectedValues})},clearSelection(){for(let e=0;e<this.selectedValueIndicies.length;e++)this.selectedValueIndicies[e]=!1;this.$emit("selected",{triggerValue:null,selectedValues:[]})}},util:{getRowValue(e,t){let s=e[t.bind];return s&&s.value&&(s=s.value),s},sortRowValue:(e,t,s)=>"function"==typeof s.sort?s.sort(e,t):e<t}},v,[],!1,null,null,null);f.options.__file="Scripts/Components/table_base.vue";var _=f.exports,g=o({props:{rows:_.props.rows,selection:_.props.selection,values:Array},data:function(){return{sortEvent:null}},methods:{onSort(e){this.sortEvent=e}},computed:{sortedValues(){if(!this.sortEvent||!this.sortEvent.row)return this.values;const e=this.values.slice();return e.sort((e,t)=>{let s=_.util.getRowValue(e,this.sortEvent.row),n=_.util.getRowValue(t,this.sortEvent.row);return _.util.sortRowValue(s,n,this.sortEvent.row)}),this.sortEvent.asc||e.reverse(),e}},components:{"fm-table":_}},m,[],!1,null,null,null);g.options.__file="Scripts/Components/table_values.vue";var y=g.exports,w=function(){var e=this,t=e.$createElement,s=e._self._c||t;return s("div",{staticClass:"editor-permissions"},[s("alert",{staticClass:"bottom margined",class:{show:e.errors.length>0},attrs:{type:"error"}},[s("header",[e._v("Error")]),e._v(" "),s("ul",e._l(e.errors,(function(t){return s("li",{key:t},[e._v("\n                   "+e._s(t)+"\n                ")])})),0)]),e._v(" "),s("alert",{attrs:{type:"info",show:""}},[s("header",[e._v("Note")]),e._v(" "),s("p",[e._v("\n            Users will have to sign out, then log back in before their new permissions take affect.\n        ")])]),e._v(" "),s("alert",{staticClass:"bottom margined",attrs:{type:"info",show:""}},[s("header",[e._v("Note")]),e._v(" "),s("p",[e._v("\n            If a permission is unticked, then suddenly becomes ticked again, this means that the user's role is providing them\n            that permission.\n        ")])]),e._v(" "),e._l(e.allPermissions,(function(t){return s("div",{key:t,staticClass:"perm"},[s("input",{directives:[{name:"model",rawName:"v-model",value:e.myPermissions,expression:"myPermissions"}],attrs:{type:"checkbox",disabled:"view"===e.mode},domProps:{value:t,checked:Array.isArray(e.myPermissions)?e._i(e.myPermissions,t)>-1:e.myPermissions},on:{change:function(s){var n=e.myPermissions,o=s.target,i=!!o.checked;if(Array.isArray(n)){var r=t,l=e._i(n,r);o.checked?l<0&&(e.myPermissions=n.concat([r])):l>-1&&(e.myPermissions=n.slice(0,l).concat(n.slice(l+1)))}else e.myPermissions=i}}}),e._v(" "),s("label",[e._v(e._s(t))])])})),e._v(" "),"edit"==e.mode?s("button",{staticClass:"top margined blue fluid",on:{click:e.updatePerms}},[e._v("\n        Update permissions\n    ")]):e._e()],2)};w._withStripped=!0;var b=o({props:{mode:a.vueProps.viewOrEdit,username:{type:String,required:!0}},data:function(){return{allPermissions:[],myPermissions:[],errors:[]}},created(){libs.Ajax.graphql("\n        query($username: String!) {\n            permissions\n            user(username:$username) {\n                permissions\n            }\n        }",{username:this.username}).then(e=>{this.allPermissions=e.permissions.sort(),this.myPermissions=e.user.permissions}).catch(e=>this.errors=e.errors)},methods:{updatePerms(){const e=this.allPermissions.filter(e=>-1!==this.myPermissions.indexOf(e)),t=this.allPermissions.filter(e=>-1===this.myPermissions.indexOf(e));libs.Ajax.graphql("\n            mutation($username: String!, $permsToAdd: [String]!, $permsToRemove: [String]!) {\n                user(username:$username) {\n                    grantPermissions(permissions:$permsToAdd)\n                    revokePermissions(permissions:$permsToRemove)\n                }\n            }",{username:this.username,permsToAdd:e,permsToRemove:t}).then(e=>(this.$emit("on-valid-update"),e)).catch(e=>{console.log(e),this.errors=e&&e.errors||[e.message]})}},components:{alert:p}},w,[],!1,null,null,null);b.options.__file="Scripts/Components/editor_permissions.vue";var S=b.exports,x=function(){var e=this.$createElement;return(this._self._c||e)("button",{on:{click:this.onClick,mouseout:this.onMouseOut}},[this._v("\n        "+this._s(this.buttonText)+"\n    ")])};x._withStripped=!0;var C=o({data:()=>({text:"",showConfirm:!1}),props:{prompt:String,confirm:String},computed:{buttonText(){return this.showConfirm?this.confirm:this.prompt}},methods:{onClick(e){this.showConfirm?(this.showConfirm=!1,this.$emit("click",e)):this.showConfirm=!0},onMouseOut(){this.showConfirm=!1}}},x,[],!1,null,null,null);C.options.__file="Scripts/Components/confirm_button.vue";var $=C.exports,k=function(){var e=this.$createElement;return(this._self._c||e)("button",{on:{click:this.onClick}},[this._v("\n    "+this._s(this.buttonText)+"\n")])};k._withStripped=!0;var A=o({data:()=>({toggled:!1}),props:{untoggledText:String,toggledText:String,value:Boolean},computed:{buttonText(){return this.toggled?this.toggledText:this.untoggledText}},methods:{onClick(e){this.toggled=!this.toggled,this.$emit("input",this.toggled)}},created(){this.toggled=this.value}},k,[],!1,null,null,null);A.options.__file="Scripts/Components/toggle_button.vue";var V=A.exports,E=function(){var e=this,t=e.$createElement,s=e._self._c||t;return s("div",{staticClass:"modal-background",class:{hidden:!e.isShowing},on:{click:function(t){return e.hide()}}},[s("div",{staticClass:"modal",on:{click:function(e){e.stopPropagation()}}},[s("div",{staticClass:"header"},[e._t("header")],2),e._v(" "),s("div",{staticClass:"body"},[e._t("body")],2),e._v(" "),s("div",{staticClass:"footer"},[e._t("footer")],2)])])};E._withStripped=!0;var N=o({data:()=>({isShowing:!1}),methods:{show(){this.isShowing=!0},hide(){this.isShowing=!1}},created(){document.addEventListener("keydown",e=>{"Escape"===e.key&&this.isShowing&&this.hide()})}},E,[],!1,null,null,null);N.options.__file="Scripts/Components/modal.vue";var T=N.exports,P=function(){var e=this,t=e.$createElement,s=e._self._c||t;return s("modal",{ref:"modal",scopedSlots:e._u([{key:"header",fn:function(){return[s("h2",[e._v("Delete")])]},proxy:!0},{key:"body",fn:function(){return[s("div",{staticStyle:{"flex-direction":"column",display:"flex",width:"100%"}},[s("p",{staticStyle:{"margin-top":"0"}},[e._v("To delete this item, please enter DELETE into the box below.")]),e._v(" "),s("p",[e._v("THIS ACTION CANNOT BE UNDONE.")]),e._v(" "),s("input",{directives:[{name:"model",rawName:"v-model",value:e.deleteText,expression:"deleteText"}],staticClass:"farm-master",attrs:{type:"text",placeholder:"Please enter DELETE"},domProps:{value:e.deleteText},on:{input:function(t){t.target.composing||(e.deleteText=t.target.value)}}})])]},proxy:!0},{key:"footer",fn:function(){return[s("div",{staticClass:"button-group"},[s("button",{on:{click:function(t){return e.hide()}}},[e._v("\n                Cancel\n            ")]),e._v(" "),s("button",{staticClass:"red",attrs:{disabled:"DELETE"!==e.deleteText},on:{click:function(t){return e.$emit("click",t)}}},[e._v("\n                Delete\n            ")])])]},proxy:!0}])})};P._withStripped=!0;var R=o({data:()=>({deleteText:""}),methods:{show(){this.$refs.modal.show()},hide(){this.$refs.modal.hide(),this.deleteText=""}},components:{modal:T}},P,[],!1,null,null,null);R.options.__file="Scripts/Components/delete_modal.vue";var I=R.exports,O=function(){var e=this,t=e.$createElement,s=e._self._c||t;return s("div",{staticClass:"notes"},["edit"===e.mode?[s("div",{staticClass:"input-wrapper inline"},[s("label",[e._v("Category")]),e._v(" "),s("textbox",{attrs:{mode:e.mode,placeholder:"Keep notes organised"},model:{value:e.category,callback:function(t){e.category=t},expression:"category"}})],1),e._v(" "),s("div",{staticClass:"input-wrapper inline"},[s("label",[e._v("Note")]),e._v(" "),s("textbox",{attrs:{mode:e.mode,placeholder:"Andy Smells"},model:{value:e.note,callback:function(t){e.note=t},expression:"note"}})],1),e._v(" "),s("button",{staticClass:"blue top bottom margined",on:{click:e.onCreateNote}},[e._v("\n            Create note\n        ")])]:e._e(),e._v(" "),s("value-table",{attrs:{selection:"edit"===e.mode?"multiple":"none",rows:[{name:"Category",bind:"category",sort:!0},{name:"Note",bind:"content",sort:!1}],values:e.notes},on:{selected:e.onSelected},scopedSlots:e._u([{key:"tfoot",fn:function(){return["edit"===e.mode?s("button",{staticClass:"red",attrs:{disabled:0===e.selectedNotes.length},on:{click:e.onDeleteNotes}},[e._v("\n                Delete selected notes ("+e._s(e.selectedNotes.length)+")\n            ")]):e._e()]},proxy:!0}])})],2)};function q(e){return`query GetNotes($id:ID!) {\n        ${e}(id:$id) {\n            notes {\n                id\n                category\n                content\n            }    \n        }    \n    }`}function j(e){return`mutation AddNote($id:ID!, $category:String!, $content:String!) {\n        ${e}(id:$id) {\n            addNote(category:$category, content:$content)\n        }\n    }`}function D(e){return`mutation DeleteNotes($id:ID!, $noteIds:[ID!]!) {\n        ${e}(id:$id) {\n            deleteNotes(ids:$noteIds)    \n        }    \n    }`}O._withStripped=!0;const M={contact:{query:q("contact"),add:j("contact"),delete:D("contact")},species:{query:q("species"),add:j("species"),delete:D("species")},breed:{query:q("breed"),add:j("breed"),delete:D("breed")}};var B=o({data:()=>({category:"",note:"",notes:[],selectedNotes:[]}),props:{mode:{type:String,default:"edit",validator:e=>-1!==["edit","view"].indexOf(e)},parentType:{type:String,required:!0,validator:e=>-1!==Object.keys(M).indexOf(e)},parentId:{type:Number,required:!0}},methods:{onError(e){console.log(e),alert(JSON.stringify(e))},selectNotesFromQuery(e){return e[this.parentType].notes},onCreateNote(){libs.Ajax.graphql(M[this.parentType].add,{id:this.parentId,category:this.category,content:this.note}).then(e=>{this.refreshNotes(),this.note="",this.category=""}).catch(e=>this.onError(e))},onDeleteNotes(){libs.Ajax.graphql(M[this.parentType].delete,{id:this.parentId,noteIds:this.selectedNotes.map(e=>e.id)}).then(e=>this.refreshNotes()).catch(e=>this.onError(e))},refreshNotes(){libs.Ajax.graphql(M[this.parentType].query,{id:this.parentId}).then(e=>this.notes=this.selectNotesFromQuery(e)).catch(e=>this.onError(e))},onSelected(e){this.selectedNotes=e.selectedValues}},created(){this.refreshNotes()},components:{textbox:d,"value-table":y}},O,[],!1,null,null,null);B.options.__file="Scripts/Components/notes.vue";var U=B.exports,L=function(){var e=this,t=e.$createElement;return(e._self._c||t)("vue-multiselect",{attrs:{value:e.value,"track-by":"name",label:"name",placeholder:e.placeholder,"allow-empty":!1,options:e.values,disabled:e.disabled},on:{input:function(t){return e.$emit("input",t)}}})};L._withStripped=!0;const F={species:{query:"query {\n            specieses {\n                items {\n                    id\n                    name    \n                }\n            }\n        }",accessor:"specieses"}};var Q=o({data:()=>({values:[]}),props:{value:Object,placeholder:String,mode:a.vueProps.viewOrEdit,entityType:{required:!0,type:String,validator:e=>-1!==Object.keys(F).indexOf(e)}},created(){this.updateValues()},computed:{disabled(){return"edit"!==this.mode}},methods:{updateValues(){libs.Ajax.graphql(F[this.entityType].query).then(e=>this.values=e[F[this.entityType].accessor].items).catch(e=>console.log(e))}}},L,[],!1,null,null,null);Q.options.__file="Scripts/Components/multiselect_graphql.vue";var W=Q.exports;Vue.component("window",r),Vue.component("textbox",d),Vue.component("alert",p),Vue.component("fm-value-table",y),Vue.component("editor-perms",S),Vue.component("confirm-button",$),Vue.component("toggle-button",V),Vue.component("modal",T),Vue.component("delete-modal",I),Vue.component("notes",U),Vue.component("vue-multiselect-graphql",W)}}).default;
//# sourceMappingURL=/js/controls.js.map