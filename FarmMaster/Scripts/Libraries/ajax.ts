export function post(url: string, data: object) {
    return ajax(url, "POST", data);
}

export function get(url: string) {
    return ajax(url, "GET", {});
}

export function graphql(query: string, variables: object) {
    console.log({
        _: "Sending Query",
        variables: variables,
        query: query
    });
    return ajax("/graphql", "POST", { query, variables })
    .then((json: { errors?: { message: string }[], data: any }) => {
        if(json.errors)
        {
            console.log({
                _: "ERROR DURING GRAPHQL QUERY",
                errors: json.errors
            });
            throw {
                errors: json.errors.map(e => e.message.split("\r\n")[0].split('\n')[0])
            };
        }

        return json.data;
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