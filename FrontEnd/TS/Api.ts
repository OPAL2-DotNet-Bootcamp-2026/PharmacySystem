// TYPES

type ApiErrorResponse = {
    message?: string;
    title?: string;
};


// API

const Api = {

    async request<TResponse>(
        endpoint: string,
        options: RequestInit = {}
    ): Promise<TResponse> {


        // HEADERS

        const headers = new Headers(options.headers);
        headers.set( "Content-Type","application/json"  );
           

        // TOKEN

        const token: string | null = Auth.token();
           
        if (token) {

            headers.set("Authorization",`Bearer ${token}`  );

        }


        // SEND REQUEST

        const response: Response =
            await fetch(

                API_BASE + endpoint,

                {
                    ...options,
                    headers
                }

            );


        // READ RESPONSE

        const responseText: string = await response.text();
           


        let data: unknown =null;
            


        if (responseText) {

            try {

                // Try JSON first

                data =JSON.parse(responseText);
     
            }

            catch {

                // If it is not JSON,
                // keep it as normal text

                data =responseText;
                    

            }

        }


        // ERROR

        if (!response.ok) {


            console.error(
                "API ERROR:",
                response.status,
                data
            );

            let message: string =`Request failed: ${response.status}`;
 
            // Backend returned plain text

            if (
                typeof data === "string"
                &&
                data.trim() !== ""
            ) {

                message = data;

            }


            // Backend returned object

            else if (
                typeof data === "object"
                &&
                data !== null
            ) {

                const errorData =data as ApiErrorResponse;
                    

                // Backend returned:
                // { message: "..." }

                if (errorData.message) {

                    message =errorData.message;
                        

                }


                // ASP.NET error:
                // { title: "..." }

                else if (errorData.title) {

                    message =errorData.title;
                        

                }

            }


            throw new Error( message );

        }


        // SUCCESS

        return data as TResponse;

    },


    // GET

    get<TResponse>(
        endpoint: string
    ): Promise<TResponse> {

        return this.request<TResponse>(
            endpoint,
            {
                method: "GET"
            }
        );

    },


    
    // POST

    post<TResponse, TBody>(
        endpoint: string,
        body: TBody
    ): Promise<TResponse> {

        return this.request<TResponse>(
            endpoint,
            {
                method: "POST",

                body:
                    JSON.stringify(
                        body
                    )
            }
        );

    },


    // PUT

    put<TResponse, TBody>(
        endpoint: string,
        body: TBody
    ): Promise<TResponse> {

        return this.request<TResponse>(
            endpoint,
            {
                method: "PUT",

                body:
                    JSON.stringify(
                        body
                    )
            }
        );

    },


    // DELETE

    del<TResponse>(
        endpoint: string
    ): Promise<TResponse> {

        return this.request<TResponse>(
            endpoint,
            {
                method: "DELETE"
            }
        );

    }

};