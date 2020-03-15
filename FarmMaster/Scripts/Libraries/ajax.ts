interface AjaxRequest {
    url: string;
    method: "POST" | "GET";
}

export function ajax({ url, method } : AjaxRequest) {
    fetch(url, {
        method: method
    })
    .then(response => {
        if(!response.ok)
            throw new Error(`${method} to ${url} failed with status ${response.status}: ${response.statusText}`);

        return response;
    })
    .then(response => response.json());
}