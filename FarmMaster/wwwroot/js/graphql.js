const GRAPHQL_ENDPOINT = "/graphql";
export class GraphQL {
    static query(query, variables) {
        return fetch(GRAPHQL_ENDPOINT, {
            headers: {
                "Accept": "application/json",
                "Content-Type": "application/json"
            },
            method: "POST",
            body: JSON.stringify({
                query,
                variables
            })
        })
            .then(r => r.json())
            .then(json => {
            if (json.errors && json.errors.length > 0)
                throw json.errors;
            return json;
        })
            .then(json => {
            console.log(json);
            return json;
        });
    }
}
export default GraphQL;
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiZ3JhcGhxbC5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIi4uLy4uL1NjcmlwdHMvZ3JhcGhxbC50cyJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiQUFBQSxNQUFNLGdCQUFnQixHQUFXLFVBQVUsQ0FBQztBQUU1QyxNQUFNLE9BQU8sT0FBTztJQUNULE1BQU0sQ0FBQyxLQUFLLENBQUMsS0FBYSxFQUFFLFNBQXdCO1FBQ3ZELE9BQU8sS0FBSyxDQUNSLGdCQUFnQixFQUNoQjtZQUNJLE9BQU8sRUFBRTtnQkFDTCxRQUFRLEVBQUUsa0JBQWtCO2dCQUM1QixjQUFjLEVBQUUsa0JBQWtCO2FBQ3JDO1lBQ0QsTUFBTSxFQUFFLE1BQU07WUFDZCxJQUFJLEVBQUUsSUFBSSxDQUFDLFNBQVMsQ0FBQztnQkFDakIsS0FBSztnQkFDTCxTQUFTO2FBQ1osQ0FBQztTQUNMLENBQ0o7YUFDQSxJQUFJLENBQUMsQ0FBQyxDQUFDLEVBQUUsQ0FBQyxDQUFDLENBQUMsSUFBSSxFQUFFLENBQUM7YUFDbkIsSUFBSSxDQUFDLElBQUksQ0FBQyxFQUFFO1lBQ1QsSUFBSSxJQUFJLENBQUMsTUFBTSxJQUFJLElBQUksQ0FBQyxNQUFNLENBQUMsTUFBTSxHQUFHLENBQUM7Z0JBQ3JDLE1BQU0sSUFBSSxDQUFDLE1BQU0sQ0FBQztZQUN0QixPQUFPLElBQUksQ0FBQztRQUNoQixDQUFDLENBQUM7YUFDRCxJQUFJLENBQUMsSUFBSSxDQUFDLEVBQUU7WUFDVCxPQUFPLENBQUMsR0FBRyxDQUFDLElBQUksQ0FBQyxDQUFBO1lBQ2pCLE9BQU8sSUFBSSxDQUFDO1FBQ2hCLENBQUMsQ0FBQyxDQUFDO0lBQ1AsQ0FBQztDQUNKO0FBRUQsZUFBZSxPQUFPLENBQUMiLCJzb3VyY2VzQ29udGVudCI6WyJjb25zdCBHUkFQSFFMX0VORFBPSU5UOiBzdHJpbmcgPSBcIi9ncmFwaHFsXCI7XHJcblxyXG5leHBvcnQgY2xhc3MgR3JhcGhRTCB7XHJcbiAgICBwdWJsaWMgc3RhdGljIHF1ZXJ5KHF1ZXJ5OiBzdHJpbmcsIHZhcmlhYmxlczogb2JqZWN0IHwgbnVsbCk6IFByb21pc2U8YW55PiB7XHJcbiAgICAgICAgcmV0dXJuIGZldGNoKFxyXG4gICAgICAgICAgICBHUkFQSFFMX0VORFBPSU5ULFxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICBoZWFkZXJzOiB7XHJcbiAgICAgICAgICAgICAgICAgICAgXCJBY2NlcHRcIjogXCJhcHBsaWNhdGlvbi9qc29uXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgXCJDb250ZW50LVR5cGVcIjogXCJhcHBsaWNhdGlvbi9qc29uXCJcclxuICAgICAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgICAgICBtZXRob2Q6IFwiUE9TVFwiLFxyXG4gICAgICAgICAgICAgICAgYm9keTogSlNPTi5zdHJpbmdpZnkoe1xyXG4gICAgICAgICAgICAgICAgICAgIHF1ZXJ5LFxyXG4gICAgICAgICAgICAgICAgICAgIHZhcmlhYmxlc1xyXG4gICAgICAgICAgICAgICAgfSlcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIClcclxuICAgICAgICAudGhlbihyID0+IHIuanNvbigpKVxyXG4gICAgICAgIC50aGVuKGpzb24gPT4ge1xyXG4gICAgICAgICAgICBpZiAoanNvbi5lcnJvcnMgJiYganNvbi5lcnJvcnMubGVuZ3RoID4gMClcclxuICAgICAgICAgICAgICAgIHRocm93IGpzb24uZXJyb3JzO1xyXG4gICAgICAgICAgICByZXR1cm4ganNvbjtcclxuICAgICAgICB9KVxyXG4gICAgICAgIC50aGVuKGpzb24gPT4ge1xyXG4gICAgICAgICAgICBjb25zb2xlLmxvZyhqc29uKVxyXG4gICAgICAgICAgICByZXR1cm4ganNvbjtcclxuICAgICAgICB9KTtcclxuICAgIH1cclxufVxyXG5cclxuZXhwb3J0IGRlZmF1bHQgR3JhcGhRTDsiXX0=