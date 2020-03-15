export function post(url: string, data: object) {
    return ajax(url, "POST", data);
}

export function get(url: string) {
    return ajax(url, "GET", {});
}

export function graphql(query: string, variables: object) {
    return ajax("/graphql", "POST", { query, variables })
    .then(json => {
        if(json.errors)
            throw new Error(`Query failed: ${JSON.stringify(json.errors)}`);
    });
}

function ajax(url: string, method: "GET" | "POST", data: object) {
    return fetch(url, {
        method: method,
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json"
        },
        body: JSON.stringify(data)
    })
    .then(response => {
        if(!response.ok)
            throw new Error(`${method} to ${url} failed with status ${response.status}: ${response.statusText}`);

        return response;
    })
    .then(response => response.json());
}