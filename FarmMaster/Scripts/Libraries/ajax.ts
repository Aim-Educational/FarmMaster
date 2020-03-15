export function post(url: string, data: object) {
    return fetch(url, {
        method: "POST",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json"
        },
        body: JSON.stringify(data)
    })
    .then(response => {
        if(!response.ok)
            throw new Error(`POST to ${url} failed with status ${response.status}: ${response.statusText}`);

        return response;
    })
    .then(response => response.json());
}