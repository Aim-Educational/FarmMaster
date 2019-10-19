const GRAPHQL_ENDPOINT: string = "/graphql";

export class GraphQL {
    public static query(query: string, variables: object | null): Promise<any> {
        return fetch(
            GRAPHQL_ENDPOINT,
            {
                headers: {
                    "Accept": "application/json",
                    "Content-Type": "application/json"
                },
                method: "POST",
                body: JSON.stringify({
                    query,
                    variables
                })
            }
        )
        .then(r => r.json())
        .then(json => {
            if (json.errors && json.errors.length > 0)
                throw json.errors;
            return json.data;
        })
        .then(json => {
            console.log(json)
            return json;
        });
    }
}

export default GraphQL;