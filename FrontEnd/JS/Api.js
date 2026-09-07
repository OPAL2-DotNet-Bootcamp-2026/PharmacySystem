const Api = {


    async request(
        endpoint,
        options = {}
    ) {


        // =====================================
        // HEADERS
        // =====================================

        const headers = {

            "Content-Type":
                "application/json",

            ...options.headers

        };


        // =====================================
        // TOKEN
        // =====================================

        const token =
            Auth.token();


        if (token) {

            headers["Authorization"] =
                `Bearer ${token}`;

        }


        // =====================================
        // SEND REQUEST
        // =====================================

        const response =
            await fetch(

                API_BASE + endpoint,

                {

                    ...options,

                    headers

                }

            );


        // =====================================
        // READ RESPONSE
        // =====================================

        const responseText =
            await response.text();


        let data =
            null;


        if (responseText) {


            try {

                // Try JSON first

                data =
                    JSON.parse(
                        responseText
                    );

            }


            catch {

                // If it is not JSON,
                // keep it as normal text

                data =
                    responseText;

            }

        }


        // =====================================
        // ERROR
        // =====================================

        if (!response.ok) {


            console.error(
                "API ERROR:",
                response.status,
                data
            );


            let message =
                `Request failed: ${response.status}`;


            // Backend returned plain text

            if (
                typeof data === "string"
                &&
                data.trim() !== ""
            ) {

                message =
                    data;

            }


            // Backend returned:
            // { message: "..." }

            else if (
                data?.message
            ) {

                message =
                    data.message;

            }


            // ASP.NET error:
            // { title: "..." }

            else if (
                data?.title
            ) {

                message =
                    data.title;

            }


            throw new Error(
                message
            );

        }


        // =====================================
        // SUCCESS
        // =====================================

        return data;

    },


    // =========================================
    // GET
    // =========================================

    get(endpoint) {

        return this.request(
            endpoint,
            {
                method:
                    "GET"
            }
        );

    },


    // =========================================
    // POST
    // =========================================

    post(
        endpoint,
        body
    ) {

        return this.request(
            endpoint,
            {
                method:
                    "POST",

                body:
                    JSON.stringify(
                        body
                    )
            }
        );

    },


    // =========================================
    // PUT
    // =========================================

    put(
        endpoint,
        body
    ) {

        return this.request(
            endpoint,
            {
                method:
                    "PUT",

                body:
                    JSON.stringify(
                        body
                    )
            }
        );

    },


    // =========================================
    // DELETE
    // =========================================

    del(endpoint) {

        return this.request(
            endpoint,
            {
                method:
                    "DELETE"
            }
        );

    }


};