export function getByBindingString(object: any, bind: String) : any {
    for(const split of bind.split(".")) {
        object = object[split];
    }

    return object;
}