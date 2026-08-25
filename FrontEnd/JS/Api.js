

const Api = {
    async request(endpoint, options = {}) {
        const headers = {
            "Content-Type": "application/json",
            ...options.headers
        };


        const token = Auth.token();
        if (token) {
            headers["Authorization"] = `Bearer ${token}`;
        }
        const response = await fetch(API_BASE + endpoint, {
            ...options,
            headers
        });


        
        let data = null;
        try {
            data = await response.json();
        } catch {
            data = null;
        }
        if (!response.ok) {
            throw new Error(
                data?.message || "Something went wrong."
            );
        }
        return data;
    },
    get(endpoint) {
        return this.request(endpoint, {
            method: "GET"
        });
    },
    post(endpoint, body) {
        return this.request(endpoint, {
            method: "POST",
            body: JSON.stringify(body)
        });
    },
    put(endpoint, body) {
        return this.request(endpoint, {
            method: "PUT",
            body: JSON.stringify(body)
        });
    },
    del(endpoint) {
        return this.request(endpoint, {
            method: "DELETE"
        });
    }
};