var controls=function(e){var t={};function s(l){if(t[l])return t[l].exports;var i=t[l]={i:l,l:!1,exports:{}};return e[l].call(i.exports,i,i.exports,s),i.l=!0,i.exports}return s.m=e,s.c=t,s.d=function(e,t,l){s.o(e,t)||Object.defineProperty(e,t,{enumerable:!0,get:l})},s.r=function(e){"undefined"!=typeof Symbol&&Symbol.toStringTag&&Object.defineProperty(e,Symbol.toStringTag,{value:"Module"}),Object.defineProperty(e,"__esModule",{value:!0})},s.t=function(e,t){if(1&t&&(e=s(e)),8&t)return e;if(4&t&&"object"==typeof e&&e&&e.__esModule)return e;var l=Object.create(null);if(s.r(l),Object.defineProperty(l,"default",{enumerable:!0,value:e}),2&t&&"string"!=typeof e)for(var i in e)s.d(l,i,function(t){return e[t]}.bind(null,i));return l},s.n=function(e){var t=e&&e.__esModule?function(){return e.default}:function(){return e};return s.d(t,"a",t),t},s.o=function(e,t){return Object.prototype.hasOwnProperty.call(e,t)},s.p="",s(s.s=11)}({11:function(e,t,s){"use strict";s.r(t);var l=function(){var e=this.$createElement,t=this._self._c||e;return t("div",{staticClass:"control-window"},[t("header",[t("h1",[this._v(this._s(this.title))])]),this._v(" "),t("div",{staticClass:"content"},[this._t("default")],2)])};function i(e,t,s,l,i,r,n,o){var a,c="function"==typeof e?e.options:e;if(t&&(c.render=t,c.staticRenderFns=s,c._compiled=!0),l&&(c.functional=!0),r&&(c._scopeId="data-v-"+r),n?(a=function(e){(e=e||this.$vnode&&this.$vnode.ssrContext||this.parent&&this.parent.$vnode&&this.parent.$vnode.ssrContext)||"undefined"==typeof __VUE_SSR_CONTEXT__||(e=__VUE_SSR_CONTEXT__),i&&i.call(this,e),e&&e._registeredComponents&&e._registeredComponents.add(n)},c._ssrRegister=a):i&&(a=o?function(){i.call(this,this.$root.$options.shadowRoot)}:i),a)if(c.functional){c._injectStyles=a;var d=c.render;c.render=function(e,t){return a.call(t),d(e,t)}}else{var u=c.beforeCreate;c.beforeCreate=u?[].concat(u,a):[a]}return{exports:e,options:c}}l._withStripped=!0;var r=i({props:{title:String}},l,[],!1,null,null,null);r.options.__file="Scripts/Components/window.vue";var n=r.exports,o=function(){var e=this,t=e.$createElement,s=e._self._c||t;return"edit"===e.mode?s("input",{staticClass:"farm-master",attrs:{name:e.name,type:e.type,placeholder:e.placeholder},domProps:{value:e.value},on:{input:function(t){return e.$emit("input",t.target.value)}}}):s("div",{staticClass:"farm-master"},[e._v("\n    "+e._s(this.value)+"\n")])};o._withStripped=!0;var a=i({props:{isPassword:Boolean,name:String,placeholder:String,value:String,mode:{type:String,default:"edit",validator:e=>-1!==["edit","view"].indexOf(e)}},computed:{type(){return this.isPassword?"password":"text"}}},o,[],!1,null,null,null);a.options.__file="Scripts/Components/textbox.vue";var c=a.exports,d=function(){var e,t=this.$createElement,s=this._self._c||t;return s("div",{staticClass:"alert",class:(e={},e[this.type]=!0,e.show=!this.closed,e)},[s("div",{staticClass:"content"},[this._t("default")],2),this._v(" "),s("div",{staticClass:"close",on:{click:this.toggle}},[s("i",{staticClass:"las la-times"})])])};d._withStripped=!0;var u=i({data:function(){return{closed:!1}},props:{type:{type:String,required:!0,validator:function(e){return-1!==["error","info"].indexOf(e)}},show:{type:Boolean,default:!1}},methods:{toggle(){this.closed=!this.closed}},created(){this.closed=!this.show}},d,[],!1,null,null,null);u.options.__file="Scripts/Components/alert.vue";var h=u.exports,p=function(){var e=this,t=e.$createElement,s=e._self._c||t;return s("table",[s("thead",[s("tr",["none"!==e.selection?s("th",{staticStyle:{"max-width":"60px"}},["multiple"===e.selection?s("input",{directives:[{name:"model",rawName:"v-model",value:e.selectAll,expression:"selectAll"}],attrs:{type:"checkbox"},domProps:{checked:Array.isArray(e.selectAll)?e._i(e.selectAll,null)>-1:e.selectAll},on:{change:function(t){var s=e.selectAll,l=t.target,i=!!l.checked;if(Array.isArray(s)){var r=e._i(s,null);l.checked?r<0&&(e.selectAll=s.concat([null])):r>-1&&(e.selectAll=s.slice(0,r).concat(s.slice(r+1)))}else e.selectAll=i}}}):e._e()]):e._e(),e._v(" "),e._l(e.rows,(function(t){return s("th",{key:t.name+t.bind,style:{width:e.rowWidth}},[e._v("\n                "+e._s(t.name)+"\n                \n                "),t.sort?s("div",{staticClass:"sort",attrs:{"data-name":t.name},on:{click:e.sortRow}},[s("i",{staticClass:"las",class:{"la-minus":!e.isRowSorted(t),"la-arrow-down":e.isRowSorted(t)&&!e.isRowSortedAsc(t),"la-arrow-up":e.isRowSorted(t)&&e.isRowSortedAsc(t)}})]):e._e()])}))],2)]),e._v(" "),s("tbody",e._l(e.sortedValues,(function(t,l){return s("tr",{key:l},["none"!==e.selection?s("td",{staticStyle:{"max-width":"60px"}},[s("input",{directives:[{name:"model",rawName:"v-model",value:e.selectedValueIndicies[l],expression:"selectedValueIndicies[index]"}],attrs:{type:"checkbox"},domProps:{checked:Array.isArray(e.selectedValueIndicies[l])?e._i(e.selectedValueIndicies[l],null)>-1:e.selectedValueIndicies[l]},on:{change:[function(t){var s=e.selectedValueIndicies[l],i=t.target,r=!!i.checked;if(Array.isArray(s)){var n=e._i(s,null);i.checked?n<0&&e.$set(e.selectedValueIndicies,l,s.concat([null])):n>-1&&e.$set(e.selectedValueIndicies,l,s.slice(0,n).concat(s.slice(n+1)))}else e.$set(e.selectedValueIndicies,l,r)},function(t){return e.onValueChecked(l)}]}})]):e._e(),e._v(" "),e._l(e.rows,(function(i){return s("td",{key:i.name+l+i.bind},[t[i.bind]&&t[i.bind].href?s("a",{attrs:{href:t[i.bind].href}},[e._v("\n                    "+e._s(t[i.bind].value)+"\n                ")]):[e._v("\n                    "+e._s(t[i.bind])+"\n                ")]],2)}))],2)})),0),e._v(" "),s("tfoot",[s("tr",[e._t("tfoot")],2)])])};p._withStripped=!0;var f=i({data:()=>({sortedRow:{row:null,asc:!1},selectedValueIndicies:[],selectAll:!1,ignoreNextSelectAll:!1}),props:{rows:{type:Array,required:!0},values:Array,selection:{type:String,default:"none",validator:function(e){return-1!==["none","single","multiple"].indexOf(e)}}},watch:{selectAll(e,t){if(this.ignoreNextSelectAll)this.ignoreNextSelectAll=!1;else{for(let t=0;t<this.values.length;t++)this.selectedValueIndicies[t]=e;this.$emit("selected",{triggerValue:null,selectedValues:e?this.values:[]})}}},computed:{rowWidth(){return Math.round(100/this.rows.length)+"%"},sortedValues(){if(!this.sortedRow.row)return this.values;const e=this.values.slice();return e.sort((e,t)=>{let s=e[this.sortedRow.row.bind],l=t[this.sortedRow.row.bind];return s&&s.value&&(s=s.value),l&&l.value&&(l=l.value),"function"==typeof this.sortedRow.row.sort?this.sortedRow.row.sort(s,l):s>l}),this.sortedRow.asc?e.reverse():e},selectedValues(){return this.values.filter((e,t)=>this.selectedValueIndicies[t])}},methods:{isRowSorted(e){return this.sortedRow.row===e},isRowSortedAsc(e){return this.isRowSorted(e)&&this.sortedRow.asc},sortRow(e){let t=e.target;t.dataset.name||(t=t.parentNode);const s=t.dataset.name,l=this.sortedRow.row;this.sortedRow.row=this.rows[this.rows.findIndex(e=>e.name===s)],l!==this.sortedRow.row?this.sortedRow.asc=!1:this.sortedRow.asc?this.sortedRow.row=null:this.sortedRow.asc=!0;for(let e=0;e<this.selectedValueIndicies.length;e++)this.selectedValueIndicies[e]=!1;this.selectAll&&(this.ignoreNextSelectAll=!0),this.selectAll=!1,this.$emit("selected",{triggerValue:null,selectedValues:[]})},onValueChecked(e){if("multiple"==this.selection){let e=this.selectedValueIndicies.length===this.values.length;if(e)for(let t=0;t<this.selectedValueIndicies.length;t++)if(!this.selectedValueIndicies[t]){e=!1;break}e!==this.selectAll&&(this.ignoreNextSelectAll=!0,this.selectAll=e)}if("single"===this.selection)for(let t=0;t<this.selectedValueIndicies.length;t++)t!==e&&(this.selectedValueIndicies[t]=!1);this.$emit("selected",{triggerValue:e>0?this.values[e]:null,selectedValues:this.selectedValues})}}},p,[],!1,null,null,null);f.options.__file="Scripts/Components/table.vue";var v=f.exports;Vue.component("window",n),Vue.component("textbox",c),Vue.component("alert",h),Vue.component("farm-table",v)}}).default;
//# sourceMappingURL=/js/controls.js.map