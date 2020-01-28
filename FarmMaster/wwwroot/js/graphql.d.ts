declare global {
    interface Window {
        RequestVerificationToken: string;
    }
}
export declare class GraphQL {
    static query(query: string, variables: object | null): Promise<any>;
}
export default GraphQL;
//# sourceMappingURL=graphql.d.ts.map