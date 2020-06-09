// https://github.com/you-dont-need/You-Dont-Need-Lodash-Underscore#_debounce
export function debounce(func : Function, wait : number, immediate : boolean) : Function {
    let timeout : any;
    return function(this: any) {
        var context = this, args = arguments;
        clearTimeout(timeout);
        timeout = setTimeout(function() {
            timeout = null;
            if (!immediate) func.apply(context, args);
        }, wait);
        if (immediate && !timeout) func.apply(context, args);
    };
}