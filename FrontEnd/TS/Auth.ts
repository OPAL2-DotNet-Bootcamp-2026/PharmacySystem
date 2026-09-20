const Auth: AuthClient = {

    // SAVE USER DATA

    save(data: AuthSession): void {

        localStorage.setItem(
            "token",
            data.token
        );

        localStorage.setItem(
            "username",
            data.username
        );

        localStorage.setItem(
            "role",
            data.role
        );
    },


    // GET TOKEN

    token(): string | null {

        return localStorage.getItem(
            "token"
        );
    },


    // GET ROLE

    role(): UserRole | null {

        return localStorage.getItem(
            "role"
        ) as UserRole | null;
    },


    // CHECK LOGIN

    isLoggedIn(): boolean {

        return !!this.token();
    },


    // LOGOUT

    logout(): void {

        localStorage.clear();

        window.location.href ="login.html";
            
    }

};