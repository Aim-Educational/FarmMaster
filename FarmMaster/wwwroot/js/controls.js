var controls=function(e){var t={};function s(o){if(t[o])return t[o].exports;var n=t[o]={i:o,l:!1,exports:{}};return e[o].call(n.exports,n,n.exports,s),n.l=!0,n.exports}return s.m=e,s.c=t,s.d=function(e,t,o){s.o(e,t)||Object.defineProperty(e,t,{enumerable:!0,get:o})},s.r=function(e){"undefined"!=typeof Symbol&&Symbol.toStringTag&&Object.defineProperty(e,Symbol.toStringTag,{value:"Module"}),Object.defineProperty(e,"__esModule",{value:!0})},s.t=function(e,t){if(1&t&&(e=s(e)),8&t)return e;if(4&t&&"object"==typeof e&&e&&e.__esModule)return e;var o=Object.create(null);if(s.r(o),Object.defineProperty(o,"default",{enumerable:!0,value:e}),2&t&&"string"!=typeof e)for(var n in e)s.d(o,n,function(t){return e[t]}.bind(null,n));return o},s.n=function(e){var t=e&&e.__esModule?function(){return e.default}:function(){return e};return s.d(t,"a",t),t},s.o=function(e,t){return Object.prototype.hasOwnProperty.call(e,t)},s.p="",s(s.s=11)}({11:function(e,t,s){"use strict";s.r(t);var o=function(){var e=this.$createElement,t=this._self._c||e;return t("div",{staticClass:"control-window"},[t("header",[t("h1",[this._v(this._s(this.title))])]),this._v(" "),t("div",{staticClass:"content"},[this._t("default")],2)])};function n(e,t,s,o,n,i,r,l){var a,c="function"==typeof e?e.options:e;if(t&&(c.render=t,c.staticRenderFns=s,c._compiled=!0),o&&(c.functional=!0),i&&(c._scopeId="data-v-"+i),r?(a=function(e){(e=e||this.$vnode&&this.$vnode.ssrContext||this.parent&&this.parent.$vnode&&this.parent.$vnode.ssrContext)||"undefined"==typeof __VUE_SSR_CONTEXT__||(e=__VUE_SSR_CONTEXT__),n&&n.call(this,e),e&&e._registeredComponents&&e._registeredComponents.add(r)},c._ssrRegister=a):n&&(a=l?function(){n.call(this,this.$root.$options.shadowRoot)}:n),a)if(c.functional){c._injectStyles=a;var d=c.render;c.render=function(e,t){return a.call(t),d(e,t)}}else{var u=c.beforeCreate;c.beforeCreate=u?[].concat(u,a):[a]}return{exports:e,options:c}}o._withStripped=!0;var i=n({props:{title:String}},o,[],!1,null,null,null);i.options.__file="Scripts/Components/window.vue";var r=i.exports,l=function(){var e=this,t=e.$createElement,s=e._self._c||t;return"edit"===e.mode?s("input",{staticClass:"farm-master",attrs:{name:e.name,type:e.type,placeholder:e.placeholder},domProps:{value:e.value},on:{input:function(t){return e.$emit("input",t.target.value)}}}):s("div",[e._v("\n    "+e._s(this.value)+"\n")])};l._withStripped=!0;var a=n({props:{isPassword:Boolean,name:String,placeholder:String,value:String,mode:{type:String,default:"edit",validator:e=>-1!==["edit","view"].indexOf(e)}},computed:{type(){return this.isPassword?"password":"text"}}},l,[],!1,null,null,null);a.options.__file="Scripts/Components/textbox.vue";var c=a.exports,d=function(){var e,t=this.$createElement,s=this._self._c||t;return s("div",{staticClass:"alert",class:(e={},e[this.type]=!0,e.show=!this.closed,e)},[s("div",{staticClass:"content"},[this._t("default")],2),this._v(" "),s("div",{staticClass:"close",on:{click:this.toggle}},[s("i",{staticClass:"las la-times"})])])};d._withStripped=!0;var u=n({data:function(){return{closed:!1}},props:{type:{type:String,required:!0,validator:function(e){return-1!==["error","info"].indexOf(e)}},show:{type:Boolean,default:!1}},methods:{toggle(){this.closed=!this.closed}},created(){this.closed=!this.show}},d,[],!1,null,null,null);u.options.__file="Scripts/Components/alert.vue";var h=u.exports,p=function(){var e=this,t=e.$createElement;return(e._self._c||t)("fm-table",e._g({ref:"table",attrs:{rows:e.rows,selection:e.selection,values:e.sortedValues},on:{sort:e.onSort},scopedSlots:e._u([{key:"tfoot",fn:function(){return[e._t("tfoot")]},proxy:!0}],null,!0)},e.$listeners))};p._withStripped=!0;var m=function(){var e=this,t=e.$createElement,s=e._self._c||t;return s("table",[s("thead",[s("tr",["none"!==e.selection?s("th",{staticStyle:{"max-width":"60px"}},["multiple"===e.selection?s("input",{directives:[{name:"model",rawName:"v-model",value:e.selectAll,expression:"selectAll"}],attrs:{type:"checkbox"},domProps:{checked:Array.isArray(e.selectAll)?e._i(e.selectAll,null)>-1:e.selectAll},on:{change:function(t){var s=e.selectAll,o=t.target,n=!!o.checked;if(Array.isArray(s)){var i=e._i(s,null);o.checked?i<0&&(e.selectAll=s.concat([null])):i>-1&&(e.selectAll=s.slice(0,i).concat(s.slice(i+1)))}else e.selectAll=n}}}):e._e()]):e._e(),e._v(" "),e._l(e.rows,(function(t){return s("th",{key:t.name+t.bind,style:{width:e.rowWidth}},[e._v("\n                "+e._s(t.name)+"\n                \n                "),t.sort?s("div",{staticClass:"sort",attrs:{"data-name":t.name},on:{click:e.onSortRow}},[s("i",{staticClass:"las",class:{"la-minus":!e.isRowSorted(t),"la-arrow-down":e.isRowSorted(t)&&!e.isRowSortedAsc(t),"la-arrow-up":e.isRowSorted(t)&&e.isRowSortedAsc(t)}})]):e._e()])}))],2)]),e._v(" "),s("tbody",e._l(e.values,(function(t,o){return s("tr",{key:o},["none"!==e.selection?s("td",{staticStyle:{"max-width":"60px"}},[s("input",{directives:[{name:"model",rawName:"v-model",value:e.selectedValueIndicies[o],expression:"selectedValueIndicies[index]"}],attrs:{type:"checkbox"},domProps:{checked:Array.isArray(e.selectedValueIndicies[o])?e._i(e.selectedValueIndicies[o],null)>-1:e.selectedValueIndicies[o]},on:{change:[function(t){var s=e.selectedValueIndicies[o],n=t.target,i=!!n.checked;if(Array.isArray(s)){var r=e._i(s,null);n.checked?r<0&&e.$set(e.selectedValueIndicies,o,s.concat([null])):r>-1&&e.$set(e.selectedValueIndicies,o,s.slice(0,r).concat(s.slice(r+1)))}else e.$set(e.selectedValueIndicies,o,i)},function(t){return e.onValueChecked(o)}]}})]):e._e(),e._v(" "),e._l(e.rows,(function(n){return s("td",{key:n.name+o+n.bind},[t[n.bind]&&t[n.bind].href?s("a",{attrs:{href:t[n.bind].href}},[e._v("\n                    "+e._s(t[n.bind].value)+"\n                ")]):[e._v("\n                    "+e._s(t[n.bind])+"\n                ")]],2)}))],2)})),0),e._v(" "),s("tfoot",[s("tr",[e._t("tfoot")],2)])])};m._withStripped=!0;var v=n({data:()=>({sortedRow:{row:null,asc:!1},selectedValueIndicies:[],selectAll:!1,ignoreNextSelectAll:!1}),props:{rows:{type:Array,required:!0},values:Array,selection:{type:String,default:"none",validator:function(e){return-1!==["none","single","multiple"].indexOf(e)}}},created(){for(const e of this.rows)if(e.sortByDefault){this.sortedRow.row=e,this.sortedRow.asc="asc"===e.sortByDefault,this.$emit("sort",{row:e,asc:this.sortedRow.asc});break}},watch:{selectAll(e,t){if(this.ignoreNextSelectAll)this.ignoreNextSelectAll=!1;else{for(let t=0;t<this.values.length;t++)this.selectedValueIndicies[t]=e;this.$emit("selected",{triggerValue:null,selectedValues:e?this.values:[]})}},values(){this.clearSelection(),this.selectAll=!1}},computed:{rowWidth(){return Math.round(100/this.rows.length)+"%"},selectedValues(){return this.values.filter((e,t)=>this.selectedValueIndicies[t])}},methods:{isRowSorted(e){return this.sortedRow.row===e},isRowSortedAsc(e){return this.isRowSorted(e)&&this.sortedRow.asc},onSortRow(e){let t=e.target;t.dataset.name||(t=t.parentNode);const s=t.dataset.name,o=this.sortedRow.row;this.sortedRow.row=this.rows[this.rows.findIndex(e=>e.name===s)],o!==this.sortedRow.row?this.sortedRow.asc=!1:this.sortedRow.asc?this.sortedRow.row=null:this.sortedRow.asc=!0,this.clearSelection(),this.selectAll&&(this.ignoreNextSelectAll=!0),this.selectAll=!1,this.$emit("sort",{row:this.sortedRow.row,asc:this.sortedRow.asc})},onValueChecked(e){if("multiple"==this.selection){let e=this.selectedValueIndicies.length===this.values.length;if(e)for(let t=0;t<this.selectedValueIndicies.length;t++)if(!this.selectedValueIndicies[t]){e=!1;break}e!==this.selectAll&&(this.ignoreNextSelectAll=!0,this.selectAll=e)}if("single"===this.selection)for(let t=0;t<this.selectedValueIndicies.length;t++)t!==e&&(this.selectedValueIndicies[t]=!1);this.$emit("selected",{triggerValue:e>0?this.values[e]:null,selectedValues:this.selectedValues})},clearSelection(){for(let e=0;e<this.selectedValueIndicies.length;e++)this.selectedValueIndicies[e]=!1;this.$emit("selected",{triggerValue:null,selectedValues:[]})}},util:{getRowValue(e,t){let s=e[t.bind];return s&&s.value&&(s=s.value),s},sortRowValue:(e,t,s)=>"function"==typeof s.sort?s.sort(e,t):e<t}},m,[],!1,null,null,null);v.options.__file="Scripts/Components/table_base.vue";var f=v.exports,_=n({props:{rows:f.props.rows,selection:f.props.selection,values:Array},data:function(){return{sortEvent:null}},methods:{onSort(e){this.sortEvent=e}},computed:{sortedValues(){if(!this.sortEvent||!this.sortEvent.row)return this.values;const e=this.values.slice();return e.sort((e,t)=>{let s=f.util.getRowValue(e,this.sortEvent.row),o=f.util.getRowValue(t,this.sortEvent.row);return f.util.sortRowValue(s,o,this.sortEvent.row)}),this.sortEvent.asc||e.reverse(),e}},components:{"fm-table":f}},p,[],!1,null,null,null);_.options.__file="Scripts/Components/table_values.vue";var g=_.exports,w=function(){var e=this,t=e.$createElement,s=e._self._c||t;return s("div",{staticClass:"editor-permissions"},[s("alert",{staticClass:"bottom margined",class:{show:e.errors.length>0},attrs:{type:"error"}},[s("header",[e._v("Error")]),e._v(" "),s("ul",e._l(e.errors,(function(t){return s("li",{key:t},[e._v("\n                   "+e._s(t)+"\n                ")])})),0)]),e._v(" "),s("alert",{staticClass:"bottom margined",attrs:{type:"info",show:""}},[s("header",[e._v("Note")]),e._v(" "),s("p",[e._v("\n            Users will have to sign out, then log back in before their new permissions take affect.\n        ")])]),e._v(" "),e._l(e.allPermissions,(function(t){return s("div",{key:t,staticClass:"perm"},[s("input",{directives:[{name:"model",rawName:"v-model",value:e.myPermissions,expression:"myPermissions"}],attrs:{type:"checkbox",disabled:"view"===e.mode},domProps:{value:t,checked:Array.isArray(e.myPermissions)?e._i(e.myPermissions,t)>-1:e.myPermissions},on:{change:function(s){var o=e.myPermissions,n=s.target,i=!!n.checked;if(Array.isArray(o)){var r=t,l=e._i(o,r);n.checked?l<0&&(e.myPermissions=o.concat([r])):l>-1&&(e.myPermissions=o.slice(0,l).concat(o.slice(l+1)))}else e.myPermissions=i}}}),e._v(" "),s("label",[e._v(e._s(t))])])})),e._v(" "),"edit"==e.mode?s("button",{staticClass:"top margined blue fluid",on:{click:e.updatePerms}},[e._v("\n        Update permissions\n    ")]):e._e()],2)};w._withStripped=!0;var y=n({props:{mode:String,username:{type:String,required:!0}},data:function(){return{allPermissions:[],myPermissions:[],errors:[]}},created(){libs.Ajax.graphql("\n        query($username: String!) {\n            permissions\n            user(username:$username) {\n                permissions\n            }\n        }",{username:this.username}).then(e=>{this.allPermissions=e.permissions.sort(),this.myPermissions=e.user.permissions}).catch(e=>this.errors=e.errors)},methods:{updatePerms(){const e=this.allPermissions.filter(e=>-1!==this.myPermissions.indexOf(e)),t=this.allPermissions.filter(e=>-1===this.myPermissions.indexOf(e));libs.Ajax.graphql("\n            mutation($username: String!, $permsToAdd: [String]!, $permsToRemove: [String]!) {\n                user(username:$username) {\n                    grantPermissions(permissions:$permsToAdd)\n                    revokePermissions(permissions:$permsToRemove)\n                }\n            }",{username:this.username,permsToAdd:e,permsToRemove:t}).then(e=>(this.$emit("on-valid-update"),e)).catch(e=>{console.log(e),this.errors=e&&e.errors||[e.message]})}},components:{alert:h}},w,[],!1,null,null,null);y.options.__file="Scripts/Components/editor_permissions.vue";var S=y.exports,b=function(){var e=this.$createElement;return(this._self._c||e)("button",{on:{click:this.onClick,mouseout:this.onMouseOut}},[this._v("\n        "+this._s(this.buttonText)+"\n    ")])};b._withStripped=!0;var x=n({data:()=>({text:"",showConfirm:!1}),props:{prompt:String,confirm:String},computed:{buttonText(){return this.showConfirm?this.confirm:this.prompt}},methods:{onClick(e){this.showConfirm?(this.showConfirm=!1,this.$emit("click",e)):this.showConfirm=!0},onMouseOut(){this.showConfirm=!1}}},b,[],!1,null,null,null);x.options.__file="Scripts/Components/confirm_button.vue";var C=x.exports,$=function(){var e=this.$createElement;return(this._self._c||e)("button",{on:{click:this.onClick}},[this._v("\n    "+this._s(this.buttonText)+"\n")])};$._withStripped=!0;var k=n({data:()=>({toggled:!1}),props:{untoggledText:String,toggledText:String,value:Boolean},computed:{buttonText(){return this.toggled?this.toggledText:this.untoggledText}},methods:{onClick(e){this.toggled=!this.toggled,this.$emit("input",this.toggled)}},created(){this.toggled=this.value}},$,[],!1,null,null,null);k.options.__file="Scripts/Components/toggle_button.vue";var A=k.exports,V=function(){var e=this,t=e.$createElement,s=e._self._c||t;return s("div",{staticClass:"modal-background",class:{hidden:!e.isShowing},on:{click:function(t){return e.hide()}}},[s("div",{staticClass:"modal",on:{click:function(e){e.stopPropagation()}}},[s("div",{staticClass:"header"},[e._t("header")],2),e._v(" "),s("div",{staticClass:"body"},[e._t("body")],2),e._v(" "),s("div",{staticClass:"footer"},[e._t("footer")],2)])])};V._withStripped=!0;var N=n({data:()=>({isShowing:!1}),methods:{show(){this.isShowing=!0},hide(){this.isShowing=!1}},created(){document.addEventListener("keydown",e=>{"Escape"===e.key&&this.isShowing&&this.hide()})}},V,[],!1,null,null,null);N.options.__file="Scripts/Components/modal.vue";var E=N.exports,T=function(){var e=this,t=e.$createElement,s=e._self._c||t;return s("modal",{ref:"modal",scopedSlots:e._u([{key:"header",fn:function(){return[s("h2",[e._v("Delete")])]},proxy:!0},{key:"body",fn:function(){return[s("div",{staticStyle:{"flex-direction":"column",display:"flex",width:"100%"}},[s("p",{staticStyle:{"margin-top":"0"}},[e._v("To delete this item, please enter DELETE into the box below.")]),e._v(" "),s("p",[e._v("THIS ACTION CANNOT BE UNDONE.")]),e._v(" "),s("input",{directives:[{name:"model",rawName:"v-model",value:e.deleteText,expression:"deleteText"}],staticClass:"farm-master",attrs:{type:"text",placeholder:"Please enter DELETE"},domProps:{value:e.deleteText},on:{input:function(t){t.target.composing||(e.deleteText=t.target.value)}}})])]},proxy:!0},{key:"footer",fn:function(){return[s("div",{staticClass:"button-group"},[s("button",{on:{click:function(t){return e.hide()}}},[e._v("\n                Cancel\n            ")]),e._v(" "),s("button",{staticClass:"red",attrs:{disabled:"DELETE"!==e.deleteText},on:{click:function(t){return e.$emit("click",t)}}},[e._v("\n                Delete\n            ")])])]},proxy:!0}])})};T._withStripped=!0;var R=n({data:()=>({deleteText:""}),methods:{show(){this.$refs.modal.show()},hide(){this.$refs.modal.hide(),this.deleteText=""}},components:{modal:E}},T,[],!1,null,null,null);R.options.__file="Scripts/Components/delete_modal.vue";var P=R.exports,I=function(){var e=this,t=e.$createElement,s=e._self._c||t;return s("div",{staticClass:"notes"},["edit"===e.mode?[s("div",{staticClass:"input-wrapper inline"},[s("label",[e._v("Category")]),e._v(" "),s("textbox",{attrs:{mode:e.mode,placeholder:"Keep notes organised"},model:{value:e.category,callback:function(t){e.category=t},expression:"category"}})],1),e._v(" "),s("div",{staticClass:"input-wrapper inline"},[s("label",[e._v("Note")]),e._v(" "),s("textbox",{attrs:{mode:e.mode,placeholder:"Andy Smells"},model:{value:e.note,callback:function(t){e.note=t},expression:"note"}})],1),e._v(" "),s("button",{staticClass:"blue top bottom margined",on:{click:e.onCreateNote}},[e._v("\n            Create note\n        ")])]:e._e(),e._v(" "),s("value-table",{attrs:{selection:"edit"===e.mode?"multiple":"none",rows:[{name:"Category",bind:"category",sort:!0},{name:"Note",bind:"content",sort:!1}],values:e.notes},on:{selected:e.onSelected},scopedSlots:e._u([{key:"tfoot",fn:function(){return["edit"===e.mode?s("button",{staticClass:"red",attrs:{disabled:0===e.selectedNotes.length},on:{click:e.onDeleteNotes}},[e._v("\n                Delete selected notes ("+e._s(e.selectedNotes.length)+")\n            ")]):e._e()]},proxy:!0}])})],2)};I._withStripped=!0;const O={contact:{query:"query GetContactNotes($id:ID!) {\n            contact(id:$id) {\n                notes {\n                    id\n                    category\n                    content\n                }    \n            }    \n        }",add:"mutation AddNoteToContact($id:ID!, $category:String!, $content:String!) {\n            contact(id:$id) {\n                addNote(category:$category, content:$content)\n            }\n        }",delete:"mutation DeleteNotesFromContact($id:ID!, $noteIds:[ID!]!) {\n            contact(id:$id) {\n                deleteNotes(ids:$noteIds)    \n            }    \n        }"}};var D=n({data:()=>({category:"",note:"",notes:[],selectedNotes:[]}),props:{mode:{type:String,default:"edit",validator:e=>-1!==["edit","view"].indexOf(e)},parentType:{type:String,required:!0,validator:e=>-1!==["contact"].indexOf(e)},parentId:{type:Number,required:!0}},methods:{onError(e){alert(JSON.stringify(e))},selectNotesFromQuery(e){switch(this.parentType){case"contact":return e.contact.notes;default:throw this.parentType}},onCreateNote(){libs.Ajax.graphql(O[this.parentType].add,{id:this.parentId,category:this.category,content:this.note}).then(e=>{this.refreshNotes(),this.note="",this.category=""}).catch(e=>this.onError(e))},onDeleteNotes(){libs.Ajax.graphql(O[this.parentType].delete,{id:this.parentId,noteIds:this.selectedNotes.map(e=>e.id)}).then(e=>this.refreshNotes()).catch(e=>this.onError(e))},refreshNotes(){libs.Ajax.graphql(O[this.parentType].query,{id:this.parentId}).then(e=>this.notes=this.selectNotesFromQuery(e)).catch(e=>this.onError(e))},onSelected(e){this.selectedNotes=e.selectedValues}},created(){this.refreshNotes()},components:{textbox:c,"value-table":g}},I,[],!1,null,null,null);D.options.__file="Scripts/Components/notes.vue";var q=D.exports;Vue.component("window",r),Vue.component("textbox",c),Vue.component("alert",h),Vue.component("fm-value-table",g),Vue.component("editor-perms",S),Vue.component("confirm-button",C),Vue.component("toggle-button",A),Vue.component("modal",E),Vue.component("delete-modal",P),Vue.component("notes",q)}}).default;
//# sourceMappingURL=/js/controls.js.map